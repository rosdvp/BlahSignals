using System;

namespace BlahSignals.AutoOrder.Attributes
{
/// <summary>
/// Mark that this system can produce Signal on next frame which can affect order of other systems
/// </summary>
[AttributeUsage(AttributeTargets.Field,AllowMultiple = true, Inherited = false)]
public class BlahProduceNextFrame : Attribute
{
}
}