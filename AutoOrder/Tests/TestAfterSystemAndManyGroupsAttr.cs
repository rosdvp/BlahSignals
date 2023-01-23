using System;
using System.Collections.Generic;
using System.Linq;
using BlahSignals.AutoOrder.Attributes;
using BlahSignals.AutoOrder.Editor;
using NUnit.Framework;

namespace BlahSignals.AutoOrder.Tests
{
public class TestAfterSystemAndManyGroupsAttr
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
            typeof(SystemE)
        };

        var result = BlahAutoOrderer.OrderAndGroupSystems(systems);

        Assert.AreEqual(systems.Count, result.Count);
        
        Assert.AreEqual(1, result.Count(r => r == systems[0]));
        Assert.AreEqual(1, result.Count(r => r == systems[1]));
        Assert.AreEqual(1, result.Count(r => r == systems[2]));
        Assert.AreEqual(1, result.Count(r => r == systems[3]));
        Assert.AreEqual(1, result.Count(r => r == systems[4]));

        var idxA = result.FindIndex(r => r == typeof(SystemA));
        var idxB = result.FindIndex(r => r == typeof(SystemB));
        var idxC = result.FindIndex(r => r == typeof(SystemC));
        var idxD = result.FindIndex(r => r == typeof(SystemD));
        var idxE = result.FindIndex(r => r == typeof(SystemE));

        Assert.AreEqual(1, Math.Abs(idxA - idxC));
        Assert.AreEqual(1, Math.Abs(idxD - idxE));
        
        Assert.AreEqual(true, idxA > idxE);
        Assert.AreEqual(true, idxC > idxE);
        Assert.AreEqual(true, idxA > idxD);
        Assert.AreEqual(true, idxC > idxD);
    }

    [BlahGroup(typeof(GroupAC))]
    [BlahAfterSystem(typeof(SystemE))]
    private class SystemA { }
    
    private class SystemB { }
    
    [BlahGroup(typeof(GroupAC))]
    private class SystemC { }
    
    [BlahGroup(typeof(GroupDE))]
    private class SystemD { }
    
    [BlahGroup(typeof(GroupDE))]
    private class SystemE { }
    
    private class GroupAC {}
    private class GroupDE {}
}
}
