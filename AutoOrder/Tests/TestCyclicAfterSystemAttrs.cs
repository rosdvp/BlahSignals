using System;
using System.Collections.Generic;
using BlahSignals.AutoOrder.Attributes;
using BlahSignals.AutoOrder.Editor;
using NUnit.Framework;

namespace BlahSignals.AutoOrder.Tests
{
public class TestCyclicAfterSystemAttrs
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

        try 
        {
            BlahAutoOrderer.OrderAndGroupSystems(systems);
            Assert.Fail();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    [BlahAfterSystem(typeof(SystemD))]
    private class SystemA { }
    private class SystemB { }
    private class SystemC { }
    [BlahAfterSystem(typeof(SystemA))]
    private class SystemD { }
}
}
