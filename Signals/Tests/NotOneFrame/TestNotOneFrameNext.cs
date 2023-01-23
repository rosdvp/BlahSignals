using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.NotOneFrame
{
public class TestNotOneFrameNext
{
    [Test]
    public void Test()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.NotOneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value          = 1;
            signal.AddNextFrame().Value = 2;

            TestHelper.CheckContent(signal, 1);
            
            context.OnFrameEnd();
            
            TestHelper.CheckContent(signal, 1, 2);
            
            foreach (ref var ev in signal)
                signal.DelCurrentInIteration();
        }
        TestHelper.CheckSignalPoolLength(signal, 2);
    }
}
}
