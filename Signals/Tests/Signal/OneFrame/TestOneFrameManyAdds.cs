using BlahSignals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Tests.Signal.OneFrame
{
public class TestOneFrameManyAdds
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
                TestHelper.CheckContent(signal);
            }

            signal.Add().Value = 1;
            signal.Add().Value = 2;
            signal.Add().Value = 3;

            TestHelper.CheckContent(signal, 1, 2, 3);
        }
        TestHelper.CheckSignalPoolLength(signal, 4); //1 2 4
    }
}
}