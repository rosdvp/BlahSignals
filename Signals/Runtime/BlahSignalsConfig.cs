namespace BlahSignals
{
public class BlahSignalsConfig
{
	/// <summary>
	/// The initial number of simultaneous instances of one signal.<br/>
	/// If this is not enough at runtime, the number will be automatically increased.
	/// </summary>
	public readonly int PoolBaseCapacity = 1;
	/// <summary>
	/// The initial number of Add/Del operations on one signal during foreach iteration. <br/>
	/// If this is not enough at runtime, the number will be automatically increased.
	/// </summary>
	public readonly int DelayedOpsBaseCapacity = 2;
}
}