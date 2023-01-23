using System;
using System.Collections.Generic;

namespace BlahSignals.Systems
{
public class BlahSystemsGroupsContext<TGroupId>
{
	private readonly Dictionary<TGroupId, BlahSystemsGroup> _groupsMap = new();
	private readonly IBlahSystemsInitData                   _systemsInitData;

	/// <param name="systemsInitData">This data will be passed to <see cref="IBlahInitSystem.Init"/></param>
	public BlahSystemsGroupsContext(IBlahSystemsInitData systemsInitData)
	{
		_systemsInitData = systemsInitData;
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private TGroupId         _activeGroupId;
	private BlahSystemsGroup _activeGroup;
	private bool             _isSwitchDelayed;
	private bool             _isDelayedSwitchToNoActiveGroup;
	private TGroupId         _delayedSwitchGroupId;

	public bool IsAnyGroupActive => _activeGroup != null;
	/// <summary>
	/// Not valid if <see cref="IsAnyGroupActive"/> is false.
	/// </summary>
	public TGroupId ActiveGroupId => _activeGroupId;
	
	/// <summary>
	/// Creates new group to add systems to.<br/>
	/// This group can be resume or paused by <see cref="SwitchToGroup"/>.
	/// </summary>
	/// <exception cref="Exception">Will throw, if <paramref name="groupId"/> already exists.</exception>
	public BlahSystemsGroup AddGroup(TGroupId groupId)
	{
		if (_groupsMap.TryGetValue(groupId, out _))
			throw new Exception($"Group with id {groupId} already exists.");
		var group = new BlahSystemsGroup();
		_groupsMap[groupId] = group;
		return group;
	}

	/// <summary>
	/// Switch from previously active group to the new one.<br/>
	/// Previously active group's systems <see cref="IBlahResumePauseSystem.Pause"/> will be called.<br/>
	/// Newly active group's systems <see cref="IBlahResumePauseSystem.Resume"/> will be called
	/// no matter if it is a first active time or not.<br/>
	/// Newly active group's systems <see cref="IBlahInitSystem.Init"/> will be called
	/// only if it is a first active time.<br/>
	/// Method DOES ABORT any delayed via <see cref="EGroupSwitchMode.SwitchBeforeNextRun"/> switch.<br/>
	/// If chosen group is already active, nothing will happen and no methods will be called.
	/// </summary>
	/// <param name="groupId">Group that should become active.</param>
	/// <param name="switchMode">The way the switch should happen.</param>
	/// <exception cref="Exception">Will throw, if no group with id exists</exception>
	public void SwitchToGroup(TGroupId     groupId,
	                        EGroupSwitchMode switchMode)
	{
		if (_groupsMap.TryGetValue(groupId, out var group))
		{
			if (_activeGroup == group)
				return;
			
			if (switchMode == EGroupSwitchMode.SwitchNow)
				SwitchGroupImpl(false, groupId);
			else
			{
				_isSwitchDelayed                = true;
				_isDelayedSwitchToNoActiveGroup = false;
				_delayedSwitchGroupId           = groupId;
			}
		}
		else
			throw new Exception($"Group with id {groupId} does not exists.");
	}

	/// <summary>
	/// Switch from previously active group to no active group.<br/>
	/// Previously active group's systems <see cref="IBlahResumePauseSystem.Pause"/> will be called.<br/>
	/// Method DOES ABORT any delayed via <see cref="EGroupSwitchMode.SwitchBeforeNextRun"/> switch.<br/>
	/// If no group is active, nothing will happen and no methods will be called.
	/// </summary>
	/// <param name="switchMode">The way the switch should happen.</param>
	/// <exception cref="Exception">Will throw, if no group with id exists</exception>
	public void SwitchToNoGroup(EGroupSwitchMode switchMode)
	{
		if (_activeGroup == null)
			return;
		
		if (switchMode == EGroupSwitchMode.SwitchNow)
			SwitchGroupImpl(true, default);
		else
		{
			_isSwitchDelayed                = true;
			_isDelayedSwitchToNoActiveGroup = true;
		}
	}
	

	private void SwitchGroupImpl(bool           isSwitchToNoActiveGroup,
	                             TGroupId       groupId)
	{
		_activeGroup?.PauseSystems();
		if (isSwitchToNoActiveGroup)
		{
			_activeGroup = null;
		}
		else
		{
			_activeGroup = _groupsMap[groupId];
			_activeGroup.TryInitSystems(_systemsInitData);
			_activeGroup.ResumeSystems();
		}
		_activeGroupId = groupId;

		_isSwitchDelayed = false;
	}

	/// <summary>
	/// Calls <see cref="IBlahRunSystem.Run"/> of <see cref="ActiveGroupId"/>.<br/>
	/// If groups switch has been delayed
	/// via <see cref="SwitchToGroup"/> and <see cref="EGroupSwitchMode.SwitchBeforeNextRun"/>,
	/// the switch will happen at the beginning of this method before any <see cref="IBlahRunSystem.Run"/> call.
	/// </summary>
	public void Run()
	{
		if (_isSwitchDelayed)
			SwitchGroupImpl(_isDelayedSwitchToNoActiveGroup, _delayedSwitchGroupId);
		_activeGroup?.RunSystems();
	}
}
}