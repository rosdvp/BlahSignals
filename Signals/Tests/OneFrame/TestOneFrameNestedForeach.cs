using System.Collections.Generic;
using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.OneFrame
{
public class TestOneFrameNestedForeach
{
	[Test]
	public void TestIterating()
	{
		var context = new BlahSignalsContext();
		var signal  = context.Get<TestHelper.OneFrameSignal>();

		signal.Add().Value = 1;
		signal.Add().Value = 2;
		signal.Add().Value = 3;

		var visits = new int[9];

		foreach (var evA in signal)
		foreach (var evB in signal)
			visits[(evA.Value - 1) * 3 + evB.Value - 1] += 1;
		
		for (var i = 0; i < visits.Length; i++)
			Assert.AreEqual(1, visits[i], $"idx {i}");
	}
}
}