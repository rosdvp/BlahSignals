using BlahSignals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Tests.Signal.KeepOrder
{
public class TestKeepOrderNext
{
    [Test]
    public void Test()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.KeepOrderSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value          = 1;
            signal.Add().Value          = 2;
            signal.Add().Value          = 3;
            signal.AddNextFrame().Value = 4;
            foreach (ref var ev in signal)
            {
                if (ev.Value == 2)
                    signal.DelCurrentInIteration();
            }
            signal.Add().Value          = 6;
            signal.AddNextFrame().Value = 5;
            signal.Add().Value          = 7;

            TestHelper.CheckContentOrder(signal, 1, 3, 6, 7);

            context.OnFrameEnd();

            TestHelper.CheckContent(signal, 1, 3, 6, 7, 4, 5);

            foreach (ref var ev in signal)
                signal.DelCurrentInIteration();
        }
        TestHelper.CheckSignalPoolLength(signal, 8); //1 2 4 8
    }
}
}
