using BlahSignals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Tests.Signal.NotOneFrame
{
public class TestNotOneFrameAddInIter
{
    [Test]
    public void Test()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.NotOneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value = 1;
            signal.Add().Value = 2;

            for (var j = 0; j < 5; j++)
            {
                Assert.AreEqual(j+2, signal.Count);
                var iterCount = 0;
                foreach (ref var ev in signal)
                {
                    if (ev.Value is < 1 or > 6)
                        Assert.Fail();
                    if (ev.Value == j + 2)
                        signal.Add().Value = j + 3;
                    iterCount++;
                }
                Assert.AreEqual(j+2, iterCount);
                context.OnFrameEnd();
            }
            foreach (ref var ev in signal)
                signal.DelCurrentInIteration();
        }
        TestHelper.CheckSignalPoolLength(signal, 8); //1 2 4 8
    }
}
}
