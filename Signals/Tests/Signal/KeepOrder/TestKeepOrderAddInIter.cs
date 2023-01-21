using BlahSignals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Tests.Signal.KeepOrder
{
public class TestKeepOrderAddInIter
{
    [Test]
    public void Test()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.KeepOrderSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value = 1;
            signal.Add().Value = 2;
            signal.Add().Value = 3;
            TestHelper.CheckContentOrder(signal, 1, 2, 3);

            foreach (ref var ev in signal)
            {
                if (ev.Value == 2)
                    signal.Add().Value = 4;
            }
            TestHelper.CheckContentOrder(signal, 1, 2, 3, 4);

            foreach (ref var ev in signal)
                signal.DelCurrentInIteration();
        }
        TestHelper.CheckSignalPoolLength(signal, 4);
    }
}
}
