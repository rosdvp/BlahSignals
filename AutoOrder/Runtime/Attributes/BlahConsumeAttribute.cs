using System;

namespace BlahSignals.AutoOrder.Attributes
{
/// <summary>
/// Put on the Signal field to mark that this System should go after all Systems
/// that produce (<see cref="BlahProduceAttribute"/>) the Signal.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class BlahConsumeAttribute : Attribute
{
}
}