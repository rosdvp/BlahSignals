using System;
using System.Collections.Generic;
using System.Linq;
using BlahSignals.AutoOrder.Attributes;
using BlahSignals.AutoOrder.Editor;
using NUnit.Framework;

namespace BlahSignals.AutoOrder.Tests
{
public class TestAfterSystemAttr
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
        var idxD = result.FindIndex(r => r == typeof(SystemD));
        
        Assert.AreEqual(true, idxA > idxD);
        Assert.AreEqual(true, idxC > idxA);
    }

    [BlahAfterSystem(typeof(SystemD))]
    private class SystemA { }
    private class SystemB { }
    [BlahAfterSystem(typeof(SystemA))]
    private class SystemC { }
    private class SystemD { }
}
}
