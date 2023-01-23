using System;
using NUnit.Framework;

namespace BlahSignals.Systems.Tests
{
public class Test
{
	[Test]
	public void TestNoGroups()
	{
		var context = new BlahSystemsGroupsContext<EDummyGroupId>(null);
		Assert.IsFalse(context.IsAnyGroupActive);
		context.SwitchToNoGroup(EGroupSwitchMode.SwitchNow);
		Assert.IsFalse(context.IsAnyGroupActive);
		context.Run();
		Assert.IsFalse(context.IsAnyGroupActive);
		try
		{
			context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchNow);
			Assert.Fail();
		}
		catch (Exception)
		{
			// ignored
		}
		try
		{
			context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchBeforeNextRun);
			Assert.Fail();
		}
		catch (Exception)
		{
			// ignored
		}
	}

	[Test]
	public void TestEmptyGroups()
	{
		var context = new BlahSystemsGroupsContext<EDummyGroupId>(null);
		context.AddGroup(EDummyGroupId.GroupA);
		context.AddGroup(EDummyGroupId.GroupB);
		
		context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchNow);
		Assert.IsTrue(context.IsAnyGroupActive);
		Assert.AreEqual(EDummyGroupId.GroupA, context.ActiveGroupId);
		context.Run();
		Assert.IsTrue(context.IsAnyGroupActive);
		Assert.AreEqual(EDummyGroupId.GroupA, context.ActiveGroupId);

		context.SwitchToGroup(EDummyGroupId.GroupB, EGroupSwitchMode.SwitchNow);
		Assert.IsTrue(context.IsAnyGroupActive);
		Assert.AreEqual(EDummyGroupId.GroupB, context.ActiveGroupId);
		context.Run();
		Assert.IsTrue(context.IsAnyGroupActive);
		Assert.AreEqual(EDummyGroupId.GroupB, context.ActiveGroupId);
		
		context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchBeforeNextRun);
		Assert.IsTrue(context.IsAnyGroupActive);
		Assert.AreEqual(EDummyGroupId.GroupB, context.ActiveGroupId);
		context.Run();
		Assert.IsTrue(context.IsAnyGroupActive);
		Assert.AreEqual(EDummyGroupId.GroupA, context.ActiveGroupId);
		context.Run();
		Assert.IsTrue(context.IsAnyGroupActive);
		Assert.AreEqual(EDummyGroupId.GroupA, context.ActiveGroupId);
		
		context.SwitchToNoGroup(EGroupSwitchMode.SwitchNow);
		Assert.IsFalse(context.IsAnyGroupActive);
	}

	[Test]
	public void TestOverwriteSwitch()
	{
		var systems = new DummyBaseSystem[]
		{
			new DummyFullSystem(),
			new DummyFullSystem(),
			new DummyFullSystem(),
		};
		var context = new BlahSystemsGroupsContext<EDummyGroupId>(null);
		context.AddGroup(EDummyGroupId.GroupA).AddSystem(systems[0]);
		context.AddGroup(EDummyGroupId.GroupB).AddSystem(systems[1]);
		context.AddGroup(EDummyGroupId.GroupC).AddSystem(systems[2]);
		
		context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchNow);
		TestsHelper.AssertSystems(
			systems,
			new[] { 0, -1, -1 },
			new[] { 1, 0, 0 },
			new[] { -1, -1, -1 },
			new[] { 0, 0, 0 },
			new[] { 0, -1, -1 },
			new[] { 1, 0, 0 },
			new[] { -1, -1, -1 },
			new[] { 0, 0, 0 });
		context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchNow);
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1 },
			new[] { 1, 0, 0 },
			new[] { -1, -1, -1 },
			new[] { 0, 0, 0 },
			new[] { -1, -1, -1 },
			new[] { 1, 0, 0 },
			new[] { -1, -1, -1 },
			new[] { 0, 0, 0 });
		
		context.SwitchToGroup(EDummyGroupId.GroupB, EGroupSwitchMode.SwitchBeforeNextRun);
		context.SwitchToGroup(EDummyGroupId.GroupC, EGroupSwitchMode.SwitchBeforeNextRun);
		context.Run();
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, 0 },
			new[] { 1, 0, 1 },
			new[] { -1, -1, 0 },
			new[] { 0, 0, 1 },
			new[] { -1, -1, 0 },
			new[] { 1, 0, 1 },
			new[] { 0, -1, -1 },
			new[] { 1, 0, 0 });
		context.Run();
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1 },
			new[] { 1, 0, 1 },
			new[] { -1, -1, 0 },
			new[] { 0, 0, 2 },
			new[] { -1, -1, -1 },
			new[] { 1, 0, 1 },
			new[] { -1, -1, -1 },
			new[] { 1, 0, 0 });
		
		context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchBeforeNextRun);
		context.SwitchToGroup(EDummyGroupId.GroupB, EGroupSwitchMode.SwitchNow);
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, 0, -1 },
			new[] { 1, 1, 1 },
			new[] { -1, -1, -1 },
			new[] { 0, 0, 2 },
			new[] { -1, 0, -1 },
			new[] { 1, 1, 1 },
			new[] { -1, -1, 0 },
			new[] { 1, 0, 1 });
		context.Run();
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1 },
			new[] { 1, 1, 1 },
			new[] { -1, 0, -1 },
			new[] { 0, 1, 2 },
			new[] { -1, -1, -1 },
			new[] { 1, 1, 1 },
			new[] { -1, -1, -1 },
			new[] { 1, 0, 1 });
	}
	

	[Test]
	public void TestFull()
	{
		var systems = new DummyBaseSystem[]
		{
			new DummyInitSystem(),
			new DummyRunSystem(),
			new DummyFullSystem(),
			new DummyFullSystem(),
			new DummyFullSystem()
		};
		var context = new BlahSystemsGroupsContext<EDummyGroupId>(null);
		context.AddGroup(EDummyGroupId.GroupA)
		       .AddSystem(systems[0])
		       .AddSystem(systems[1])
		       .AddSystem(systems[2])
		       .AddSystem(systems[3]);
		context.AddGroup(EDummyGroupId.GroupB)
		       .AddSystem(systems[4]);
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 0 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 0 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 0 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 0 });

		context.SwitchToGroup(EDummyGroupId.GroupB, EGroupSwitchMode.SwitchNow);
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, 0 },
			new[] { 0, 0, 0, 0, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 0 },
			new[] { -1, -1, -1, -1, 0 },
			new[] { 0, 0, 0, 0, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 0 });

		context.Run();
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 1 },
			new[] { -1, -1, -1, -1, 0 },
			new[] { 0, 0, 0, 0, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 0 });

		context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchNow);
		TestsHelper.AssertSystems(
			systems,
			new[] { 0, -1, 1, 2, -1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 1 },
			new[] { -1, -1, 0, 1, -1 },
			new[] { 0, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, 0 },
			new[] { 0, 0, 0, 0, 1 });

		context.Run();
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { -1, 0, 1, 2, -1 },
			new[] { 0, 1, 1, 1, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 1 });

		context.Run();
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { -1, 0, 1, 2, -1 },
			new[] { 0, 2, 2, 2, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 0, 0, 1 });

		context.SwitchToNoGroup(EGroupSwitchMode.SwitchNow);
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 2, 2, 2, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 1, 1, 1 },
			new[] { -1, -1, 0, 1, -1 },
			new[] { 0, 0, 1, 1, 1 });

		context.SwitchToGroup(EDummyGroupId.GroupA, EGroupSwitchMode.SwitchNow);
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 2, 2, 2, 1 },
			new[] { -1, -1, 0, 1, -1 },
			new[] { 0, 0, 2, 2, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 1, 1, 1 });

		context.SwitchToGroup(EDummyGroupId.GroupB, EGroupSwitchMode.SwitchBeforeNextRun);
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 2, 2, 2, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 2, 2, 1 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 1, 1, 1 });
		context.Run();
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, 0 },
			new[] { 0, 2, 2, 2, 2 },
			new[] { -1, -1, -1, -1, 0 },
			new[] { 0, 0, 2, 2, 2 },
			new[] { -1, -1, 0, 1, -1 },
			new[] { 0, 0, 2, 2, 1 });
		context.Run();
		TestsHelper.AssertSystems(
			systems,
			new[] { -1, -1, -1, -1, -1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { -1, -1, -1, -1, 0 },
			new[] { 0, 2, 2, 2, 3 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 2, 2, 2 },
			new[] { -1, -1, -1, -1, -1 },
			new[] { 0, 0, 2, 2, 1 });
	}
}
}
