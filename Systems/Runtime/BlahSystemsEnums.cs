namespace BlahSignals
{
public enum EGroupSwitchMode
{
	/// <summary>
	/// The switch will happen immediately.
	/// </summary>
	SwitchNow,
	/// <summary>
	/// The switch will happen at the beginning of next <see cref="BlahSystemsGroupsContext{TGroupId}.Run"/> method.
	/// </summary>
	SwitchBeforeNextRun
}
}