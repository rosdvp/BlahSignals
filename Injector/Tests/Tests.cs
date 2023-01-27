using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BlahSignals.Injector.Tests
{
public class Tests
{
	[Test]
	public void TestDummy()
	{
		var signalA = new DummySignal<DummySignalA>();
		var signalB = new DummySignal<DummySignalB>();
		
		var provider = new DummyProvider();
		provider.Add(signalA);
		provider.Add(signalB);
		
		Assert.AreSame(signalA, provider.Get<DummySignalA>());
		Assert.AreSame(signalB, provider.Get<DummySignalB>());
		Assert.AreSame(null, provider.Get<DummySignalC>());
		
		Assert.AreSame(signalA, provider.Get<DummySignalA>());
		Assert.AreSame(signalB, provider.Get<DummySignalB>());
		Assert.AreSame(null, provider.Get<DummySignalC>());
	}


	[Test]
	public void TestTargetEmpty()
	{
		var (injector, signalA, signalB) = Setup();
		var targets = new ITarget[] { new TargetEmpty() };
		injector.Inject(targets);
	}

	[Test]
	public void TestTargetOther()
	{
		var (injector, signalA, signalB) = Setup();
		var targetOther = new TargetOther();
		var targets = new ITarget[] { targetOther };
		injector.Inject(targets);
		
		Assert.AreEqual(3, targetOther.GetValInt());
		Assert.AreEqual("abc", targetOther.GetValString());
		Assert.AreEqual(1, targetOther.GetValIntsList()[0]);
		Assert.AreEqual(2, targetOther.GetValIntsList()[1]);
		Assert.AreEqual(3, targetOther.GetValIntsList()[2]);
	}

	[Test]
	public void TestTargetA()
	{
		var (injector, signalA, signalB) = Setup();
		var targetA0 = new TargetA();
		var targetA1 = new TargetA();
		var targets  = new ITarget[] { targetA0, targetA1 };
		injector.Inject(targets);
		
		Assert.AreSame(signalA, targetA0.GetSignalA());
		Assert.AreSame(signalA, targetA1.GetSignalA());
	}
	
	[Test]
	public void TestTargetPublic()
	{
		var (injector, signalA, signalB) = Setup();
		var target = new TargetPublic();
		injector.Inject(new ITarget[] { target });
		
		Assert.AreSame(signalA, target.GetSignalA());
	}
	
	[Test]
	public void TestTargetStaticPrivate()
	{
		var (injector, signalA, signalB) = Setup();
		var target = new TargetStaticPrivate();
		injector.Inject(new ITarget[] { target });
		
		Assert.AreSame(signalA, target.GetSignalA());
	}
	
	[Test]
	public void TestTargetStaticPublic()
	{
		var (injector, signalA, signalB) = Setup();
		var target = new TargetStaticPublic();
		injector.Inject(new ITarget[] { target });
		
		Assert.AreSame(signalA, target.GetSignalA());
	}
	
	[Test]
	public void TestTargetProtected()
	{
		var (injector, signalA, signalB) = Setup();
		var target = new TargetProtected();
		injector.Inject(new ITarget[] { target });
		
		Assert.AreSame(signalA, target.GetSignalA());
	}
	
	[Test]
	public void TestTargetDoubleA()
	{
		var (injector, signalA, signalB) = Setup();
		var target = new TargetDoubleA();
		injector.Inject(new ITarget[] { target });
		
		Assert.AreSame(signalA, target.GetSignalA0());
		Assert.AreSame(signalA, target.GetSignalA1());
	}
	
	[Test]
	public void TestTargetPrivateReadOnly()
	{
		var (injector, signalA, signalB) = Setup();
		var target = new TargetPrivateReadOnly();
		injector.Inject(new ITarget[] { target });
		
		Assert.AreSame(signalA, target.GetSignalA());
	}
	
	[Test]
	public void TestTargetPublicReadOnly()
	{
		var (injector, signalA, signalB) = Setup();
		var target = new TargetPublicReadOnly();
		injector.Inject(new ITarget[] { target });
		
		Assert.AreSame(signalA, target.GetSignalA());
	}
	
	
	[Test]
	public void TestTargetInheritance()
	{
		var (injector, signalA, signalB) = Setup();
		var target = new TargetInhChild();
		injector.Inject(new ITarget[] { target });

		Assert.AreSame(signalA, target.GetParentSignalA());
		Assert.AreSame(signalB, target.GetChildSignalB());
		Assert.AreSame(signalA, target.GetProtectedSignalA());
	}
	

	[Test]
	public void TestTargetsAll()
	{
		var (injector, signalA, signalB) = Setup();
		var targetA0       = new TargetA();
		var targetA1       = new TargetA();
		var targetB0       = new TargetB();
		var targetB1       = new TargetB();
		var targetAB0      = new TargetAB();
		var targetAB1      = new TargetAB();
		var targetABOther0 = new TargetABOther();
		var targetABOther1 = new TargetABOther();
		var targets   = new ITarget[]
		{
			targetA0, targetA1, targetB0, targetB1, targetAB0, targetAB1, targetABOther0, targetABOther1
		};
		injector.Inject(targets);
		
		Assert.AreSame(signalA, targetA0.GetSignalA());
		Assert.AreSame(signalA, targetA1.GetSignalA());
		
		Assert.AreSame(signalB, targetB0.GetSignalB());
		Assert.AreSame(signalB, targetB1.GetSignalB());
		
		Assert.AreSame(signalA, targetAB0.GetSignalA());
		Assert.AreSame(signalB, targetAB0.GetSignalB());
		Assert.AreSame(signalA, targetAB1.GetSignalA());
		Assert.AreSame(signalB, targetAB1.GetSignalB());
		
		Assert.AreSame(signalA, targetABOther0.GetSignalA());
		Assert.AreSame(signalB, targetABOther0.GetSignalB());
		Assert.AreEqual(3, targetABOther0.GetValInt());
		Assert.AreEqual("abc", targetABOther0.GetValString());
		Assert.AreEqual(1, targetABOther0.GetValIntsList()[0]);
		Assert.AreEqual(2, targetABOther0.GetValIntsList()[1]);
		Assert.AreEqual(3, targetABOther0.GetValIntsList()[2]);
		
		Assert.AreSame(signalA, targetABOther1.GetSignalA());
		Assert.AreSame(signalB, targetABOther1.GetSignalB());
		Assert.AreEqual(3, targetABOther1.GetValInt());
		Assert.AreEqual("abc", targetABOther1.GetValString());
		Assert.AreEqual(1, targetABOther1.GetValIntsList()[0]);
		Assert.AreEqual(2, targetABOther1.GetValIntsList()[1]);
		Assert.AreEqual(3, targetABOther1.GetValIntsList()[2]);
	}

	[Test]
	public void TestTargetWithRepeat()
	{
		var (injector, signalA, signalB) = Setup();
		var targetA = new TargetA();
		var targetB = new TargetB();
		var targets = new ITarget[]
		{
			targetA, targetB
		};

		for (var i = 0; i < 10; i++)
		{
			injector.Inject(targets);
			
			Assert.AreSame(signalA, targetA.GetSignalA(), $"iter #{i}");
			Assert.AreSame(signalB, targetB.GetSignalB(), $"iter #{i}");
			
			targetA.ResetSignalA();
			targetB.ResetSignalB();
		}
	}


	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private (BlahInjector, DummySignal<DummySignalA> signalA, DummySignal<DummySignalB>) 
		Setup()
	{
		var signalA  = new DummySignal<DummySignalA>();
		var signalB  = new DummySignal<DummySignalB>();
		var provider = new DummyProvider();
		provider.Add(signalA);
		provider.Add(signalB);
		return (new BlahInjector(provider,
		                         typeof(DummyProvider).GetMethod("Get"),
		                         typeof(DummySignal<>)),
		        signalA,
		        signalB);
	}


	private class DummyProvider
	{
		private Dictionary<Type, IDummySignal> _map = new();

		public void Add<T>(DummySignal<T> signal)
		{
			_map[typeof(T)] = signal;
		}

		public DummySignal<T> Get<T>()
		{
			if (_map.TryGetValue(typeof(T), out var signal))
				return (DummySignal<T>)signal;
			return null;
		}
	}

	private interface IDummySignal { }

	private class DummySignal<T> : IDummySignal
	{
	}

	private struct DummySignalA { }
	private struct DummySignalB { }
	private struct DummySignalC { }


	private interface ITarget { }

	private class TargetEmpty : ITarget { }

	private class TargetOther : ITarget
	{
		private int       _valInt      = 3;
		public string    ValString   = "abc";
		private static List<int> _intsList = new() { 1, 2, 3 };

		public int GetValInt() => _valInt;
		public string GetValString() => ValString;
		public List<int> GetValIntsList() => _intsList;
	}

	private class TargetA : ITarget
	{
		private DummySignal<DummySignalA> _signalA;

		public DummySignal<DummySignalA> GetSignalA() => _signalA;
		public void ResetSignalA() => _signalA = null;
	}

	private class TargetB : ITarget
	{
		private DummySignal<DummySignalB> _signalB;

		public DummySignal<DummySignalB> GetSignalB() => _signalB;
		public void ResetSignalB() => _signalB = null;
	}
	
	private class TargetAB : ITarget
	{
		private DummySignal<DummySignalA> _signalA;
		private DummySignal<DummySignalB> _signalB;
		
		public DummySignal<DummySignalA> GetSignalA() => _signalA;
		public DummySignal<DummySignalB> GetSignalB() => _signalB;
	}

	private class TargetABOther : ITarget
	{
		private DummySignal<DummySignalA> _signalA;
		private DummySignal<DummySignalB> _signalB;
		
		private        int       _valInt   = 3;
		public         string    ValString = "abc";
		private static List<int> _intsList = new() { 1, 2, 3 };
		
		public DummySignal<DummySignalA> GetSignalA() => _signalA;
		public DummySignal<DummySignalB> GetSignalB() => _signalB;
		
		public int GetValInt() => _valInt;
		public string GetValString() => ValString;
		public List<int> GetValIntsList() => _intsList;
	}

	private class TargetPublic : ITarget
	{
		public DummySignal<DummySignalA> _signalA;

		public DummySignal<DummySignalA> GetSignalA() => _signalA;
	}

	private class TargetStaticPrivate : ITarget
	{
		private static DummySignal<DummySignalA> _signalA;

		public DummySignal<DummySignalA> GetSignalA() => _signalA;
	}

	private class TargetStaticPublic : ITarget
	{
		public static DummySignal<DummySignalA> _signalA;

		public DummySignal<DummySignalA> GetSignalA() => _signalA;
	}

	private class TargetProtected : ITarget
	{
		protected DummySignal<DummySignalA> _signalA;

		public DummySignal<DummySignalA> GetSignalA() => _signalA;
	}

	private class TargetDoubleA : ITarget
	{
		private DummySignal<DummySignalA> _signalA0;
		private DummySignal<DummySignalA> _signalA1;
		

		public DummySignal<DummySignalA> GetSignalA0() => _signalA0;
		public DummySignal<DummySignalA> GetSignalA1() => _signalA1;
	}

	private class TargetPublicReadOnly : ITarget
	{
		public readonly DummySignal<DummySignalA> _signalA;

		public DummySignal<DummySignalA> GetSignalA() => _signalA;	
	}

	private class TargetPrivateReadOnly : ITarget
	{
		private readonly DummySignal<DummySignalA> _signalA;

		public DummySignal<DummySignalA> GetSignalA() => _signalA;
	}

	private class TargetInhParent : ITarget
	{
		private   DummySignal<DummySignalA> _signalA;
		protected DummySignal<DummySignalA> _protectedSignalA;

		public DummySignal<DummySignalA> GetParentSignalA() => _signalA;
	}

	private class TargetInhChild : TargetInhParent
	{
		private DummySignal<DummySignalB> _signalB;

		public DummySignal<DummySignalB> GetChildSignalB() => _signalB;

		public DummySignal<DummySignalA> GetProtectedSignalA() => _protectedSignalA;
	}
}
}