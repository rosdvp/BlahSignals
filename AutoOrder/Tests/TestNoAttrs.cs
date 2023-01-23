using System;
using System.Collections.Generic;
using BlahSignals.AutoOrder.Editor;
using NUnit.Framework;

namespace BlahSignals.AutoOrder.Tests
{
public class TestNoAttrs
{
    [Test]
    public void Test()
    {
        var systems = new List<Type>
        {
            typeof(System0),
            typeof(System1),
            typeof(System2),
            typeof(System3),
        };

        var result = BlahAutoOrderer.OrderAndGroupSystems(systems);

        Assert.AreEqual(systems.Count, result.Count);
        Assert.AreEqual(systems[0], result[0]);
        Assert.AreEqual(systems[1], result[1]);
        Assert.AreEqual(systems[2], result[2]);
        Assert.AreEqual(systems[3], result[3]);
    }

    private class System0 { }
    private class System1 { }
    private class System2 { }
    private class System3 { }
}
}
