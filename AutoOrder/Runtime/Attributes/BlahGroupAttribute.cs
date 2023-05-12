using System;

namespace BlahSignals.AutoOrder.Attributes
{
/// <summary>
/// Mark that System should go close together with other Systems in the same group.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class BlahGroupAttribute : Attribute
{
	public Type Group { get; }

	public BlahGroupAttribute(Type groupType)
	{
		Group = groupType;
	}
}
}