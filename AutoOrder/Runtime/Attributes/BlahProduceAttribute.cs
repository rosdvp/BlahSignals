using System;

namespace BlahSignals.AutoOrder.Attributes
{
/// <summary>
/// Put on the Signal field to mark that this System adds/changes the Signal.<br/>
/// Work in pair with <see cref="BlahConsumeAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class BlahProduceAttribute : Attribute
{
}
}