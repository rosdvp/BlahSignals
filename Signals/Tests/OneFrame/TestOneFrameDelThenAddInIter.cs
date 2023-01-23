using BlahSignals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Tests.Signal.OneFrame
{
public class TestOneFrameDelThenAddInIter
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

            Assert.AreEqual(2, signal.Count);
            var iterCount = 0;
            foreach (ref var ev in signal)
            {
                if (ev.Value != 1 && ev.Value != 2)
                    Assert.Fail();
                if (ev.Value == 1)
                {
                    signal.DelCurrentInIteration();
                    signal.Add().Value = 3;
                }
                iterCount++;
            }
            Assert.AreEqual(2, iterCount);
            
            TestHelper.CheckContent(signal, 2, 3);
        }
        TestHelper.CheckSignalPoolLength(signal, 4); //1 2 4
    }
}
}
