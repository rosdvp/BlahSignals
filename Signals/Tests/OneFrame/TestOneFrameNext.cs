using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.OneFrame
{
public class TestOneFrameNext
{
    [Test]
    public void TestOnlyNextFrame()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.AddNextFrame().Value = 1;
            TestHelper.CheckContent(signal);
            
            context.OnFrameEnd();
            
            TestHelper.CheckContent(signal, 1);

            signal.AddNextFrame().Value = 2;
            
            TestHelper.CheckContent(signal, 1);
            
            context.OnFrameEnd();
            
            TestHelper.CheckContent(signal, 2);
            
            context.OnFrameEnd();
            
            TestHelper.CheckContent(signal);
        }
    }
    
    [Test]
    public void TestCurrFrameAndNextFrame()
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
