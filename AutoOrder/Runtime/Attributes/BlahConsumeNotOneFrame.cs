using System;

namespace BlahSignals.AutoOrder.Attributes
{
/// <summary>
/// Mark that this System works with NotOneFrame signal, so it should not affect order.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class BlahNotOneFrameAttribute : Attribute
{
}
}