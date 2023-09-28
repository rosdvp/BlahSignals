using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BlahSignals.Signals
{
public class BlahSignal<T> : IBlahSignalPool where T : struct
{
	private readonly bool _isOneFrame;
	private readonly bool _isKeepOrder;
	
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

	public BlahSignal(BlahSignalsConfig config, bool isOneFrame, bool isKeepOrder)
	{
		_isOneFrame                 = isOneFrame;
		_isKeepOrder                = isKeepOrder;
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
		AddDelayedOp(_aliveIdxs[_iteratorsIdxInAliveByLevel[0]], false);
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
	/// Very slow.<br/>
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

		var sortedAliveIdxs = new int[_aliveCount];
		Array.Copy(_aliveIdxs, sortedAliveIdxs, _aliveCount);
		Array.Sort(sortedAliveIdxs, (idxA, idxB) => comp.Invoke(_pool[idxA], _pool[idxB]));
		Array.Copy(sortedAliveIdxs, _aliveIdxs, _aliveCount);
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	void IBlahSignalPool.OnFrameEnd()
	{
		if (_isOneFrame)
		{
			if (_nextFrameCount > 0)
			{
				for (var i = 0; i < _aliveCount; i++)
					_releasedIdxs[_releasedCount++] = _aliveIdxs[i];
			}
			else
			{
				_poolCount     = 0;
				_releasedCount = 0;
			}
			_aliveCount = 0;
		}
		if (_nextFrameCount > 0)
		{
			for (var i = 0; i < _nextFrameCount; i++)
				_aliveIdxs[_aliveCount++] = _nextFrameIdxs[i];
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
		op.IdxInPool = idxInPoolOrInAlive;
		op.IsAdd              = isAdd;
	}

	private void ApplyDelayedOps()
	{
		if (_isKeepOrder)
		{
			var isAnyDeleted = false;
			for (var opIdx = 0; opIdx < _delayedOpsCount; opIdx++)
			{
				ref var op = ref _delayedOps[opIdx];
				if (op.IsAdd)
					_aliveIdxs[_aliveCount++] = op.IdxInPool;
				else
				{
					for (var i = 0; i < _aliveCount; i++)
						if (_aliveIdxs[i] == op.IdxInPool)
						{
							_releasedIdxs[_releasedCount++] = op.IdxInPool;
							_aliveIdxs[i]                   = -1;
							break;
						}
					isAnyDeleted = true;
				}
			}
			if (isAnyDeleted)
			{
				var isAnyLeft = false;
				for (var i = 0; i < _aliveCount; i++)
				{
					if (_aliveIdxs[i] != -1)
						continue;
					isAnyLeft = false;
					for (int j = i+1; j < _aliveCount; j++)
						if (_aliveIdxs[j] != -1)
						{
							_aliveIdxs[i] = _aliveIdxs[j];
							_aliveIdxs[j] = -1;
							isAnyLeft     = true;
							break;
						}
					if (!isAnyLeft)
					{
						_aliveCount = i;
						break;
					}
				}
			}
		}
		else
		{
			for (var opIdx = 0; opIdx < _delayedOpsCount; opIdx++)
			{
				ref var op = ref _delayedOps[opIdx];
				if (op.IsAdd)
					_aliveIdxs[_aliveCount++] = op.IdxInPool;
				else
				{
					for (var i = 0; i < _aliveCount; i++)
						if (_aliveIdxs[i] == op.IdxInPool)
						{
							_releasedIdxs[_releasedCount++] = _aliveIdxs[i];
							_aliveIdxs[i]                   = _aliveIdxs[--_aliveCount];
							break;
						}
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
		public int  IdxInPool;
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