using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.OneFrame
{
public class TestOneFrameSingleAdd
{
    [Test]
    public void Test()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<TestHelper.OneFrameSignal>();

        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 5; j++)
            {
                if (i != 0) 
                    context.OnFrameEnd();
                Assert.AreEqual(true, signal.IsEmpty);
                TestHelper.CheckContent(signal);
            }

            ref var e = ref signal.Add();
            e.Value = 1;

            Assert.AreEqual(false, signal.IsEmpty);
            TestHelper.CheckContent(signal, 1);
        }
        TestHelper.CheckSignalPoolLength(signal, 1);
    }
}
}
