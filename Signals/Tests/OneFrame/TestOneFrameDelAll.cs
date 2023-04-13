using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.OneFrame
{
public class TestOneFrameDelAll
{
    [Test]
    public void TestDellAll()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();
        
        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value = 1;
            signal.Add().Value = 2;
            signal.Add().Value = 3;

            Assert.AreEqual(3, signal.Count);
            TestHelper.CheckContent(signal, 1, 2, 3);

            signal.DellAll();

            Assert.AreEqual(0, signal.Count);
            TestHelper.CheckContent(signal);
            
            context.OnFrameEnd();
        }
        
        TestHelper.CheckSignalPoolLength(signal, 4); //1 2 4
    }

    [Test]
    public void TestDellAllWithNextFrame()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value          = 1;
            signal.AddNextFrame().Value = 2;
            signal.Add().Value          = 3;
            TestHelper.CheckContent(signal, 1, 3);
            
            signal.DellAll();
            TestHelper.CheckContent(signal);
            
            context.OnFrameEnd();
            TestHelper.CheckContent(signal, 2);
            context.OnFrameEnd();
            TestHelper.CheckContent(signal);
        }
    }

    [Test]
    public void TestCheckIsEmptyAndDellAll()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();
        
        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value = 1;
            signal.Add().Value = 2;
            signal.Add().Value = 3;

            TestHelper.CheckContent(signal, 1, 2, 3);

            Assert.AreEqual(false, signal.CheckIsEmptyAndDelAll());

            TestHelper.CheckContent(signal);

            Assert.AreEqual(true, signal.CheckIsEmptyAndDelAll());
            
            context.OnFrameEnd();
        }
        
        TestHelper.CheckSignalPoolLength(signal, 4); //1 2 4
    }
    
    [Test]
    public void TestCheckIsEmptyAndDellAllWithNextFrame()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value          = 1;
            signal.AddNextFrame().Value = 2;
            signal.Add().Value          = 3;
            TestHelper.CheckContent(signal, 1, 3);
            
            Assert.AreEqual(false, signal.CheckIsEmptyAndDelAll());
            TestHelper.CheckContent(signal);
            
            context.OnFrameEnd();
            TestHelper.CheckContent(signal, 2);
            context.OnFrameEnd();
            TestHelper.CheckContent(signal);
        }
    }
}
}
