using BlahSignals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Tests.Signal.NotOneFrame
{
public class TestNotOneFrameDelete
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
}
}
