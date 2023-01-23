using BlahSignals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Tests.Signal.OneFrame
{
public class TestOneFrameNext
{
    [Test]
    public void Test()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value          = 1;
            signal.AddNextFrame().Value = 2;

            TestHelper.CheckContent(signal, 1);
            
            context.OnFrameEnd();

            TestHelper.CheckContent(signal, 2);
            
            context.OnFrameEnd();

            TestHelper.CheckContent(signal);
        }
        TestHelper.CheckSignalPoolLength(signal, 2);
    }
}
}
