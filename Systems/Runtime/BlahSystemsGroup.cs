using System;
using System.Collections.Generic;

namespace BlahSignals.Systems
{
public class BlahSystemsGroup
{
	private readonly HashSet<IBlahSystem>         _allSystems         = new();
	private readonly List<IBlahInitSystem>        _initSystems        = new();
	private readonly List<IBlahRunSystem>         _runSystems         = new();
	private readonly List<IBlahResumePauseSystem> _resumePauseSystems = new();
	
	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private bool _isInited = false;
	private bool _isPaused = true;

	public IReadOnlyCollection<IBlahSystem> GetAllSystems() => _allSystems;

	public BlahSystemsGroup AddSystem(IBlahSystem system)
	{
		if (_isInited)
			throw new Exception("Impossible to add system if systems of the group is already inited.");
		
		_allSystems.Add(system);
		if (system is IBlahInitSystem initSystem)
			_initSystems.Add(initSystem);
		if (system is IBlahRunSystem runSystem)
			_runSystems.Add(runSystem);
		if (system is IBlahResumePauseSystem resumePauseSystem)
			_resumePauseSystems.Add(resumePauseSystem);
		return this;
	}

	/// <summary>
	/// Init systems if they have not been not inited yet.
	/// </summary>
	internal void TryInitSystems(IBlahSystemsInitData initData)
	{
		if (_isInited)
			return;
		_isInited = true;
		for (var i = 0; i < _initSystems.Count; i++)
			_initSystems[i].Init(initData);
	}

	internal void ResumeSystems()
	{
		if (!_isInited)
			throw new Exception("Group is not inited!");
		if (!_isPaused)
			throw new Exception("Group is not paused!");
		_isPaused = false;
		for (var i = 0; i < _resumePauseSystems.Count; i++)
			_resumePauseSystems[i].Resume();
	}

	internal void PauseSystems()
	{
		if (!_isInited)
			throw new Exception("Group is not inited!");
		if (_isPaused)
			throw new Exception("Group is already paused!");
		_isPaused = true;
		for (var i = 0; i < _resumePauseSystems.Count; i++)
			_resumePauseSystems[i].Pause();
	}
	
	/// <summary>
	/// Calls <see cref="IBlahRunSystem.Run"/> of all systems in group.
	/// </summary>
	/// <exception cref="Exception">Will throw, if systems are not inited.</exception>
	internal void RunSystems()
	{
		if (!_isInited)
			throw new Exception("Group is not inited!");
		if (_isPaused)
			throw new Exception("Group is paused!");
		
		for (var i = 0; i < _runSystems.Count; i++)
			_runSystems[i].Run();
	}
}
}