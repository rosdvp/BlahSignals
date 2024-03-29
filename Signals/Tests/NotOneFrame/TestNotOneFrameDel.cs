using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.NotOneFrame
{
public class TestNotOneFrameDel
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
                    signal.DelCurrentInIteration();
            
            TestHelper.CheckContent(signal, 1, 3);
            
            foreach (ref var ev in signal)
                signal.DelCurrentInIteration();
        }
        TestHelper.CheckSignalPoolLength(signal, 4); //1 2 4
    }

    [Test]
    public void TestFullMultiple()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.NotOneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value = 1;
            signal.Add().Value = 2;
            signal.Add().Value = 3;
            signal.Add().Value = 4;
            signal.Add().Value = 5;

            context.OnFrameEnd();

            foreach (ref var ev in signal)
                if (ev.Value == 2 || ev.Value == 3)
                    signal.DelCurrentInIteration();

            TestHelper.CheckContent(signal, 1, 4, 5);
            signal.DellAll();
            TestHelper.CheckContent(signal);
        }
    }
}
}
