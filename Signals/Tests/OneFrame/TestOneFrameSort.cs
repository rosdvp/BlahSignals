using System;
using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.OneFrame
{
public class TestOneFrameSort
{
	[Test]
	public void Test()
	{
		var context = new BlahSignalsContext();
		var signal  = context.Get<TestHelper.OneFrameSignal>();

		int[] values     = { 3, 1, 2, 3, 2 };
		int[] ascValues  = { 1, 2, 2, 3, 3 };
		int[] descValues = { 3, 3, 2, 2, 1 };
		
		signal.Add().Value = values[0];
		signal.Add().Value = values[1];
		signal.Add().Value = values[2];
		signal.Add().Value = values[3];
		signal.Add().Value = values[4];

		var i = 0;
		foreach (ref var ev in signal)
		{
			Assert.AreEqual(values[i], ev.Value);
			i++;
		}

		signal.Sort((a,        b) => a.Value.CompareTo(b.Value));
		i = 0;
		foreach (ref var ev in signal)
		{
			Assert.AreEqual(ascValues[i], ev.Value);
			i++;
		}
		
		signal.Sort((a,        b) => b.Value.CompareTo(a.Value));
		i = 0;
		foreach (ref var ev in signal)
		{
			Assert.AreEqual(descValues[i], ev.Value);
			i++;
		}
	}
}
}