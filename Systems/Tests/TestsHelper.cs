using System;
using NUnit.Framework;

namespace BlahSignals.Systems.Tests
{

public static class TestsHelper
{
	public static void AssertSystems(DummyBaseSystem[] systems,
	                                 int[]             expectedInitOrders,
	                                 int[]             expectedInitCounts,
	                                 int[]             expectedRunOrders,
	                                 int[]             expectedRunCounts,
	                                 int[]             expectedResumeOrders,
	                                 int[]             expectedResumeCounts,
	                                 int[]             expectedPauseOrders,
	                                 int[]             expectedPauseCounts)
	{
		for (var i = 0; i < systems.Length; i++)
		{
			if (expectedInitOrders != null)
				Assert.AreEqual(expectedInitOrders[i], systems[i].InitOrder, $"InitOrder, system[{i}]");
			if (expectedInitCounts != null)
				Assert.AreEqual(expectedInitCounts[i], systems[i].InitCount, $"InitCount, system[{i}]");

			if (expectedRunOrders != null)
				Assert.AreEqual(expectedRunOrders[i], systems[i].RunOrder, $"RunOrder, system[{i}]");
			if (expectedRunCounts != null)
				Assert.AreEqual(expectedRunCounts[i], systems[i].RunCount, $"RunCount, system[{i}]");

			if (expectedResumeOrders != null)
				Assert.AreEqual(expectedResumeOrders[i], systems[i].ResumeOrder, $"ResumeOrder, system[{i}]");
			if (expectedResumeCounts != null)
				Assert.AreEqual(expectedResumeCounts[i], systems[i].ResumeCount, $"ResumeCount, system[{i}]");

			if (expectedPauseOrders != null)
				Assert.AreEqual(expectedPauseOrders[i], systems[i].PauseOrder, $"PauseOrder, system[{i}]");
			if (expectedPauseCounts != null)
				Assert.AreEqual(expectedPauseCounts[i], systems[i].PauseCount, $"PauseCount, system[{i}]");
			
			systems[i].ResetOrders();
		}
	}
}


public abstract class DummyBaseSystem : IBlahSystem
{
	protected static int NextInitOrder;
	protected static int NextRunOrder;
	protected static int NextResumeOrder;
	protected static int NextPauseOrder;


	public int InitOrder   { get; protected set; } = -1;
	public int InitCount   { get; protected set; }
	public int RunOrder    { get; protected set; } = -1;
	public int RunCount    { get; protected set; }
	public int ResumeOrder { get; protected set; } = -1;
	public int ResumeCount { get; protected set; }
	public int PauseOrder  { get; protected set; } = -1;
	public int PauseCount  { get; protected set; }
	
	public void ResetOrders()
	{
		NextInitOrder   = 0;
		NextRunOrder    = 0;
		NextResumeOrder = 0;
		NextPauseOrder  = 0;
		
		InitOrder   = -1;
		RunOrder    = -1;
		ResumeOrder = -1;
		PauseOrder  = -1;
	}
}

public class DummyInitSystem : DummyBaseSystem, IBlahInitSystem
{
	public void Init(IBlahSystemsInitData data)
	{
		if (InitOrder == -1)
			InitOrder = NextInitOrder++;
		InitCount += 1;
	}
}

public class DummyRunSystem : DummyBaseSystem, IBlahRunSystem
{
	public void Run()
	{
		if (RunOrder == -1)
			RunOrder = NextRunOrder++;
		RunCount += 1;
	}
}

public class DummyFullSystem : DummyBaseSystem, IBlahInitSystem, IBlahRunSystem, IBlahResumePauseSystem
{
	public void Init(IBlahSystemsInitData initData)
	{
		if (InitOrder == -1)
			InitOrder = NextInitOrder++;
		InitCount += 1;
	}

	public void Run()
	{
		if (RunOrder == -1)
			RunOrder = NextRunOrder++;
		RunCount += 1;
	}

	public void Resume()
	{
		if (ResumeOrder == -1)
			ResumeOrder = NextResumeOrder++;
		ResumeCount += 1;
	}

	public void Pause()
	{
		if (PauseOrder == -1)
			PauseOrder = NextPauseOrder++;
		PauseCount += 1;
	}
}



public enum EDummyGroupId
{
	GroupA,
	GroupB,
	GroupC
}
}