using System;
using System.Collections.Generic;

namespace BlahSignals.Signals
{
public class BlahSignalsContext
{
	private readonly BlahSignalsConfig                 _config;
	private readonly Dictionary<Type, IBlahSignalPool> _map = new();
	private readonly List<IBlahSignalPool>             _all = new();
	
	public BlahSignalsContext(BlahSignalsConfig config = null)
	{
		_config = config ?? new BlahSignalsConfig();
	}
	
	/// <summary>
	/// Get a reference to signals pool.
	/// The reference caching is recommended.
	/// </summary>
	public BlahSignal<T> Get<T>() where T : struct, IBlahSignal
	{
		var type = typeof(T);
		if (_map.TryGetValue(type, out var cachedSignal))
			return (BlahSignal<T>)cachedSignal;

		bool isOneFrame  = type.GetInterface(nameof(IBlaNotOneFrame)) == null;
		bool isKeepOrder = type.GetInterface(nameof(IBlahKeepOrder)) != null;
		
		var signal     = new BlahSignal<T>(_config, isOneFrame, isKeepOrder);
		_all.Add(signal);
		_map[type] = signal;

		return signal;
	}

	/// <summary>
	/// Delete all OneFrame signals (these signals do not have <see cref="IBlaNotOneFrame"/>).
	/// </summary>
	public void OnFrameEnd()
	{
		for (var i = 0; i < _all.Count; i++)
			_all[i].OnFrameEnd();
	}

	/// <summary>
	/// Delete all signals, both OneFrame and not OneFrame.
	/// </summary>
	public void ClearAll()
	{
		for (var i = 0; i < _all.Count; i++)
			_all[i].ClearAll();
	}
}

internal interface IBlahSignalPool
{
	void OnFrameEnd();
	void ClearAll();
}

/// <summary>
/// Mark that struct is a signal.
/// All signals are OneFrame by default.
/// Use <see cref="IBlaNotOneFrame"/> to make a signal not OneFrame.
/// </summary>
public interface IBlahSignal { }
/// <summary>
/// Use with <see cref="IBlahSignal"/> only.
/// Mark that the content of current frame should be removed after all systems Run loop
/// </summary>
public interface IBlaNotOneFrame { }
/// <summary>
/// Use with <see cref="IBlahSignal"/> only.
/// Mark that the order of signals matter and should be kept
/// </summary>
public interface IBlahKeepOrder { }
}