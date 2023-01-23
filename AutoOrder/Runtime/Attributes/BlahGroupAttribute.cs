using System;

namespace BlahSignals.AutoOrder.Attributes
{
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