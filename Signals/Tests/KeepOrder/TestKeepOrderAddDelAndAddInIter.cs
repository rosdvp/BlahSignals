using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.KeepOrder
{
public class TestKeepOrderDelAddInIter
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
                {
                    signal.DelCurrentInIteration();
                    signal.Add().Value = 4;
                    signal.Add().Value = 5;
                }
            }
            TestHelper.CheckContentOrder(signal, 1, 3, 4, 5);

            foreach (ref var ev in signal)
                signal.DelCurrentInIteration();
        }
        TestHelper.CheckSignalPoolLength(signal, 8); //1 2 4 8
    }
}
}
