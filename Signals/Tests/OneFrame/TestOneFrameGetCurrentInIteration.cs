using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.OneFrame
{
public class TestOneFrameGetCurrentInIteration
{
	[Test]
	public void Test()
	{
		var context = new BlahSignalsContext();
		var signal  = context.Get<TestHelper.OneFrameSignal>();

		signal.Add().Value = 1;
		signal.Add().Value = 2;
		signal.Add().Value = 3;
		
		foreach (var evA in signal)
		foreach (var evB in signal)
		{
			ref var refEvA = ref signal.GetCurrentInIteration(0);
			ref var refEvB = ref signal.GetCurrentInIteration(1);
			
			Assert.AreEqual(evA.Value, refEvA.Value);
			Assert.AreEqual(evB.Value, refEvB.Value);
		}
	}
}
}