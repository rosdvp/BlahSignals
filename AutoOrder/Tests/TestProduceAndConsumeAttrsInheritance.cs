using System;
using System.Collections.Generic;
using System.Linq;
using BlahSignals.AutoOrder.Attributes;
using BlahSignals.AutoOrder.Editor;
using NUnit.Framework;

namespace BlahSignals.AutoOrder.Tests
{
public class TestProduceAndConsumeAttrsInheritance
{
    [Test]
    public void Test()
    {
        var systems = new List<Type>
        {
            typeof(SystemA),
            typeof(SystemB),
            typeof(SystemC),
            typeof(SystemD),
        };

        var result = BlahAutoOrderer.OrderAndGroupSystems(systems);

        Assert.AreEqual(systems.Count, result.Count);
        
        Assert.AreEqual(1, result.Count(r => r == systems[0]));
        Assert.AreEqual(1, result.Count(r => r == systems[1]));
        Assert.AreEqual(1, result.Count(r => r == systems[2]));
        Assert.AreEqual(1, result.Count(r => r == systems[3]));

        var idxA = result.FindIndex(r => r == typeof(SystemA));
        var idxC = result.FindIndex(r => r == typeof(SystemC));
        
        Assert.AreEqual(true, idxA > idxC);
    }

    private class SystemA : SystemBaseA { }
    private class SystemB { }
    private class SystemC : SystemBaseC { }
    private class SystemD { }

    private class SystemBaseA
    {
        [BlahConsume]
        private DummySignal<SignalAlpha> _signal;
    }
    private class SystemBaseC
    {
        [BlahProduce]
        private DummySignal<SignalAlpha> _signal;
    }
    
    private class SignalAlpha { }
    private class DummySignal<T> { }
}
}
