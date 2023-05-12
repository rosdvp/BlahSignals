using System;

namespace BlahSignals.AutoOrder.Attributes
{
/// <summary>
/// Mark that this System should go after another System.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class BlahAfterSystemAttribute : Attribute
{
	public Type AfterType { get; }
	public BlahAfterSystemAttribute(Type afterType)
	{
		AfterType = afterType;
	}
}
}