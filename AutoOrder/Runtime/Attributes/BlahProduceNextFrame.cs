using System;

namespace BlahSignals.AutoOrder.Attributes
{
/// <summary>
/// Mark that this System produces Signal only on next frame, so it should not affect order.
/// </summary>
[AttributeUsage(AttributeTargets.Field,AllowMultiple = true, Inherited = false)]
public class BlahProduceNextFrame : Attribute
{
}
}