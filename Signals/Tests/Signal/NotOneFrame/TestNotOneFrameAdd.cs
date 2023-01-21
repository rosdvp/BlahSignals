using BlahSignals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Tests.Signal.NotOneFrame
{
public class TestNotOneFrameAdd
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
                TestHelper.CheckContent(signal, 1, 2);
                context.OnFrameEnd();
            }
            foreach (ref var ev in signal)
                signal.DelCurrentInIteration();
        }
        TestHelper.CheckSignalPoolLength(signal, 2);
    }
}
}
