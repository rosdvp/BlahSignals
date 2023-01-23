using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.NotOneFrame
{
public class TestNotOneFrameDelThenAddInIter
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
            signal.Add().Value = 3;
            
            context.OnFrameEnd();

            foreach (ref var ev in signal)
                if (ev.Value == 2)
                {
                    signal.DelCurrentInIteration();
                    signal.Add().Value = 4;
                }

            TestHelper.CheckContent(signal, 1, 3, 4);
            
            context.OnFrameEnd();

            TestHelper.CheckContent(signal, 1, 3, 4);

            foreach (ref var ev in signal)
                signal.DelCurrentInIteration();
        }
        TestHelper.CheckSignalPoolLength(signal, 4);
    }
}
}
