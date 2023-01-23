using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.OneFrame
{
public class TestOneFrameDelete
{
    [Test]
    public void Test()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 5; j++)
            {
                if (i != 0) 
                    context.OnFrameEnd();
                TestHelper.CheckContent(signal);
            }

            signal.Add().Value = 1;
            signal.Add().Value = 2;
            signal.Add().Value = 3;

            int iterCount = 0;
            foreach (ref var ev in signal)
            {
                if (ev.Value == 2)
                    signal.DelCurrentInIteration();
                iterCount++;
            }
            Assert.AreEqual(3, iterCount);
            
            TestHelper.CheckContent(signal, 1, 3);
        }
        TestHelper.CheckSignalPoolLength(signal, 4); //1 2 4
    }
}
}
