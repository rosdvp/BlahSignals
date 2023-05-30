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

		int[] values       = { 3, 1, 2 };
		int[] sortedValues = { 1, 2, 3 };
		
		signal.Add().Value = values[0];
		signal.Add().Value = values[1];
		signal.Add().Value = values[2];

		var i = 0;
		foreach (ref var ev in signal)
		{
			Assert.AreEqual(values[i], ev.Value);
			i++;
		}
		
		signal.Sort((a, b) => a.Value.CompareTo(b.Value));

		i = 0;
		foreach (ref var ev in signal)
		{
			Assert.AreEqual(sortedValues[i], ev.Value);
			i++;
		}
	}
}
}