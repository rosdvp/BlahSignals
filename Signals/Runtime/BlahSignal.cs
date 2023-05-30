using System;
using System.Runtime.CompilerServices;

namespace BlahSignals.Signals
{
public class BlahSignal<T> : IBlahSignalPool where T : struct
{
	private readonly bool _isOneFrame;
	private readonly bool _isKeepOrder;
	private readonly bool _isResettable;
	
	private T[] _pool;
	private int _poolCount;

	private int[] _aliveIdxs;
	private int   _aliveCount;

	private int[] _releasedIdxs;
	private int   _releasedCount;

	private int[] _nextFrameIdxs;
	private int   _nextFrameCount;

	private DelayedOp[] _delayedOps;
	private int         _delayedOpsCount;

	private int   _iteratorsGoingCount;
	private int[] _iteratorsIdxInAliveByLevel;

	public BlahSignal(BlahSignalsConfig config, bool isOneFrame, bool isKeepOrder, bool isResettable)
	{
		_isOneFrame                 = isOneFrame;
		_isKeepOrder                = isKeepOrder;
		_isResettable               = isResettable;
		_pool                       = new T[config.PoolBaseCapacity];
		_aliveIdxs                  = new int[_pool.Length];
		_releasedIdxs               = new int[_pool.Length];
		_nextFrameIdxs              = new int[_pool.Length];
		_delayedOps                 = new DelayedOp[config.DelayedOpsBaseCapacity];
		_iteratorsIdxInAliveByLevel = new int[1];
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	/// <summary>
	/// Is there any signals at this frame
	/// </summary>
	public bool IsEmpty => _aliveCount == 0;
	/// <summary>
	/// Count of signals at this frame
	/// </summary>
	public int  Count   => _aliveCount;

	/// <summary>
	/// Add to current frame.<br/>
	/// Allowed: ref var ev = ref signal.Add(); ev.Value = 3;<br/>
	/// Allowed: signal.Add().Value = 3;<br/>
	/// Not allowed: var ev = signal.Add(); ev.Value = 3;
	/// </summary>
	public ref T Add()
	{
		int idx = GetFreeIdxInPool();
		if (_iteratorsGoingCount > 0)
			AddDelayedOp(idx, true);
		else
			_aliveIdxs[_aliveCount++] = idx;

		if (_isResettable)
			_pool[idx] = default;
		
		return ref _pool[idx];
	}

	/// <summary>
	/// Add to next frame (after all systems Run loop).
	/// The rules are the same as for <see cref="Add"/>
	/// </summary>
	public ref T AddNextFrame()
	{
		int idx = GetFreeIdxInPool();
		ValidateCapacity(ref _nextFrameIdxs, _nextFrameCount);
		_nextFrameIdxs[_nextFrameCount++] = idx;

		if (_isResettable)
			_pool[idx] = default;
		
		return ref _pool[idx];
	}

	/// <summary>
	/// Use during foreach looping to remove current signal.
	/// </summary>
	public void DelCurrentInIteration()
	{
		if (_iteratorsGoingCount == 0)
			throw new Exception($"{nameof(DelCurrentInIteration)} only works in foreach loop");
		if (_iteratorsGoingCount > 1)
			throw new Exception($"Deleting in nested foreach loops is not supported yet");
		AddDelayedOp(_iteratorsIdxInAliveByLevel[0], false);
	}

	/// <summary>
	/// Removes all signals.
	/// </summary>
	public void DellAll()
	{
		if (_iteratorsGoingCount > 0)
			throw new Exception($"{nameof(DellAll)} is not allowed in foreach loop");
		_poolCount     = 0;
		_aliveCount    = 0;
		_releasedCount = 0;
	}
	
	/// <summary>
	/// Removes all signals, but return <see cref="IsEmpty"/> at the moment before removing.
	/// </summary>
	public bool CheckIsEmptyAndDelAll()
	{
		if (_iteratorsGoingCount > 0)
			throw new Exception($"{nameof(CheckIsEmptyAndDelAll)} is not allowed in foreach loop");
		bool isEmpty = _aliveCount == 0;
		_poolCount     = 0;
		_aliveCount    = 0;
		_releasedCount = 0;
		return isEmpty;
	}
	
	/// <summary>
	/// Returns any signal in pool.
	/// </summary>
	public ref T GetAny()
	{
		if (_aliveCount == 0)
			throw new Exception("Signal is empty!");
		return ref _pool[_aliveIdxs[0]];
	}

	/// <summary>
	/// Use this method if you need a specific order in foreach loop.<br/>
	/// Does not affect nextFrame signals.
	/// </summary>
	/// <param name="comp">
	/// Lambda should return:<br/>
	/// -1 if first less second;<br/>
	/// 0 if first equals second;<br/>
	/// 1 if first greater second;<br/> 
	/// </param>
	public void Sort(Comparison<T> comp)
	{
		if (_iteratorsGoingCount > 0)
			throw new Exception("Sorting during foreach loop is not allowed");
		Array.Sort(
			_aliveIdxs,
			(idxA, idxB) => comp.Invoke(_pool[idxA], _pool[idxB])
		);
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	void IBlahSignalPool.OnFrameEnd()
	{
		if (_isOneFrame)
		{
			_poolCount   = 0;
			_aliveCount = 0;
			_releasedCount  = 0;
		}
		if (_nextFrameCount > 0)
		{
			for (var i = 0; i < _nextFrameCount; i++)
			{
				int poolIdx = _nextFrameIdxs[i];
				_aliveIdxs[_aliveCount++] = poolIdx;

				for (var j = 0; j < _releasedCount; j++)
					if (_releasedIdxs[j] == poolIdx)
						_releasedIdxs[j] = _releasedIdxs[--_releasedCount];
			}
			_nextFrameCount = 0;
		}
	}

	void IBlahSignalPool.ClearAll()
	{
		_poolCount      = 0;
		_aliveCount     = 0;
		_releasedCount  = 0;
		_nextFrameCount = 0;
	}
	
	//-----------------------------------------------------------
	//-----------------------------------------------------------
	/// <summary>
	/// If <paramref name="isAdd"/> is true,
	/// <paramref name="idxInPoolOrInAlive"/> must be index in <see cref="_pool"/>.<br/>
	/// Otherwise,
	/// <paramref name="idxInPoolOrInAlive"/> must be index in <see cref="_aliveIdxs"/>.
	/// </summary>
	private void AddDelayedOp(int idxInPoolOrInAlive, bool isAdd)
	{
		ValidateCapacity(ref _delayedOps, _delayedOpsCount);
		ref var op = ref _delayedOps[_delayedOpsCount++];
		op.IdxInPoolOrInAlive = idxInPoolOrInAlive;
		op.IsAdd              = isAdd;
	}

	private void ApplyDelayedOps()
	{
		if (_isKeepOrder)
		{
			int emptyIdx = -1;
			for (var i = 0; i < _delayedOpsCount; i++)
			{
				ref var op = ref _delayedOps[i];
				if (op.IsAdd)
					_aliveIdxs[_aliveCount++] = op.IdxInPoolOrInAlive;
				else
				{
					_releasedIdxs[_releasedCount++]   = _aliveIdxs[op.IdxInPoolOrInAlive];
					_aliveIdxs[op.IdxInPoolOrInAlive] = -1;
					if (emptyIdx == -1 || op.IdxInPoolOrInAlive < emptyIdx)
						emptyIdx = op.IdxInPoolOrInAlive;
				}
			}
			if (emptyIdx != -1)
			{
				int tempAliveCount = _aliveCount;
				for (int i = emptyIdx+1; i < tempAliveCount; i++)
					if (_aliveIdxs[i] == -1)
						_aliveCount--;
					else
					{
						_aliveIdxs[emptyIdx] = _aliveIdxs[i];
						emptyIdx             = i;
					}
				_aliveCount--;
			}
		}
		else
		{
			for (var i = 0; i < _delayedOpsCount; i++)
			{
				ref var op = ref _delayedOps[i];
				if (op.IsAdd)
					_aliveIdxs[_aliveCount++] = op.IdxInPoolOrInAlive;
				else
				{
					_releasedIdxs[_releasedCount++]   = _aliveIdxs[op.IdxInPoolOrInAlive];
					_aliveIdxs[op.IdxInPoolOrInAlive] = _aliveIdxs[--_aliveCount];
				}
			}
		}
		_delayedOpsCount = 0;
	}
	
	private struct DelayedOp
	{
		/// <summary>
		/// If <see cref="IsAdd"/> true, it is index in <see cref="_pool"/>,
		/// otherwise it is index in <see cref="_aliveIdxs"/> 
		/// </summary>
		public int  IdxInPoolOrInAlive;
		public bool IsAdd;
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	public Enumerator GetEnumerator() => new(this);

	public readonly struct Enumerator : IDisposable
	{
		private readonly BlahSignal<T> _signal;
		private readonly int           _level;

		public Enumerator(BlahSignal<T> signal)
		{
			_signal = signal;
			_signal.ValidateCapacity(ref _signal._iteratorsIdxInAliveByLevel, _signal._iteratorsGoingCount);
			_level                                      = _signal._iteratorsGoingCount++;
			_signal._iteratorsIdxInAliveByLevel[_level] = -1;
		}

		public ref T Current => ref _signal._pool[_signal._aliveIdxs[_signal._iteratorsIdxInAliveByLevel[_level]]];

		public bool MoveNext() => ++_signal._iteratorsIdxInAliveByLevel[_level] < _signal._aliveCount;

		public void Dispose()
		{
			if (--_signal._iteratorsGoingCount == 0)
				_signal.ApplyDelayedOps();
		}
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	[MethodImpl (MethodImplOptions.AggressiveInlining)]
	private int GetFreeIdxInPool()
	{
		int idx;
		if (_releasedCount > 0)
			idx = _releasedIdxs[--_releasedCount];
		else
		{
			ValidatePoolCapacity();
			idx = _poolCount++;
		}
		return idx;
	}
	
	[MethodImpl (MethodImplOptions.AggressiveInlining)]
	private void ValidatePoolCapacity()
	{
		if (_poolCount >= _pool.Length)
		{
			int targCount = _poolCount * 2;
			Array.Resize(ref _pool, targCount);
			Array.Resize(ref _aliveIdxs, targCount);
			Array.Resize(ref _releasedIdxs, targCount);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ValidateCapacity<T2>(ref T2[] array, int currCount)
	{
		if (currCount == array.Length)
			Array.Resize(ref array, array.Length * 2);
	}
}
}