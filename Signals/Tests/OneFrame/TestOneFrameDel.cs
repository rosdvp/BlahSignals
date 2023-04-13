using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;
using UnityEngine;

namespace BlahSignals.Signals.Tests.OneFrame
{
public class TestOneFrameDel
{
    [Test]
    public void TestDelSingle()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 5; j++)
            {
                if (i != 0) 
                    context.OnFrameEnd();
                TestHelper.CheckContent(signal);
            }

            signal.Add().Value = 1;
            signal.Add().Value = 2;
            signal.Add().Value = 3;

            int iterCount = 0;
            foreach (ref var ev in signal)
            {
                if (ev.Value == 2)
                    signal.DelCurrentInIteration();
                iterCount++;
            }
            Assert.AreEqual(3, iterCount);
            
            TestHelper.CheckContent(signal, 1, 3);
        }
        TestHelper.CheckSignalPoolLength(signal, 4); //1 2 4
    }
    
    [Test]
    public void TestDelMultiple()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 5; j++)
            {
                if (i != 0) 
                    context.OnFrameEnd();
                TestHelper.CheckContent(signal);
            }

            signal.Add().Value = 1;
            signal.Add().Value = 2;
            signal.Add().Value = 3;

            var iterCount = 0;
            foreach (ref var ev in signal)
            {
                signal.DelCurrentInIteration();
                iterCount++;
            }
            Assert.AreEqual(3, iterCount);

            TestHelper.CheckContent(signal);
        }
        TestHelper.CheckSignalPoolLength(signal, 4); //1 2 4
    }

    [Test]
    public void TestDelWithNextFrame()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            signal.Add().Value          = 1;
            signal.AddNextFrame().Value = 2;
            signal.Add().Value          = 3;
            TestHelper.CheckContent(signal, 1, 3);
            
            foreach (ref var ev in signal)
                if (ev.Value == 1)
                    signal.DelCurrentInIteration();
            TestHelper.CheckContent(signal, 3);
            
            context.OnFrameEnd();
            TestHelper.CheckContent(signal, 2);
            
            context.OnFrameEnd();
            TestHelper.CheckContent(signal);
        }
        TestHelper.CheckSignalPoolLength(signal, 4); // 1 2 4
    }
}
}
