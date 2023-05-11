using BlahSignals.Signals.Tests.Helpers;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.Resettable
{
public class TestResettable
{
    private struct TestStruct
    {
        public int X;
    }

    private class TestClass
    {
        public int Y;
    }
    
    private struct ResettableSignal : IBlahSignal, IBlahResettable
    {
        public int        IntVal;
        public string     StrVal;
        public TestStruct StructVal;
        public TestClass  ClassVal;
    }
    
    [Test]
    public void Test()
    {
        var context = new BlahSignalsContext();
        var signal  = context.Get<ResettableSignal>();

        ref var evSent = ref signal.Add();
        evSent.IntVal      = 1;
        evSent.StrVal      = "123";
        evSent.StructVal.X = 2;
        evSent.ClassVal    = new TestClass { Y = 3 };
        
        
        foreach (ref var ev in signal)
        {
            Assert.AreEqual(1, ev.IntVal);
            Assert.AreEqual("123", ev.StrVal);
            Assert.AreEqual(2, ev.StructVal.X);
            Assert.AreEqual(3, ev.ClassVal.Y);
        }

        context.OnFrameEnd();

        Assert.AreEqual(true, signal.IsEmpty);

        signal.Add();

        foreach (ref var ev in signal)
        {
            Assert.AreEqual(0, ev.IntVal);
            Assert.AreEqual(null, ev.StrVal);
            Assert.AreEqual(0, ev.StructVal.X);
            Assert.AreEqual(null, ev.ClassVal);
        }
    }
}
}
