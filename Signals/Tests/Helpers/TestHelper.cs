﻿using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace BlahSignals.Signals.Tests.Helpers
{
internal static class TestHelper
{
	public static void CheckSignalPoolLength<T>(T target, int expectedLength)
		=> CheckPoolLength(target, "_pool", expectedLength);
	
	private static void CheckPoolLength<T>(T target, string name, int expectedLength)
	{
		// ReSharper disable once PossibleNullReferenceException
		var actualLength = ((Array)typeof(T)
		                           .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)
		                           .GetValue(target)).Length;
		Assert.AreEqual(expectedLength, actualLength);
	}

	public static void CheckContent<T>(BlahSignal<T> signal, params int[] expectedValues) where T : struct, IWithValue
	{
		var values = new List<int>(expectedValues);

		Assert.AreEqual(expectedValues.Length == 0, signal.IsEmpty);
		Assert.AreEqual(expectedValues.Length, signal.Count);
		var iterCount = 0;
		foreach (ref var ev in signal)
		{
			if (!values.Contains(ev.Val))
				Assert.Fail($"Value {ev.Val} is not expected");
			values.Remove(ev.Val);
			iterCount++;
		}
		Assert.AreEqual(expectedValues.Length, iterCount);
		foreach (int value in values)
			Assert.Fail($"Event with Value {value} is not in the pool");
	}
	
	public static void CheckContentOrder<T>(BlahSignal<T> signal, params int[] expectedValues) where T : struct, IWithValue
	{
		Assert.AreEqual(expectedValues.Length, signal.Count);
		var i = 0;
		foreach (ref var ev in signal)
		{
			if (ev.Val != expectedValues[i])
				Assert.Fail($"expected: {expectedValues[i]}, actual: {ev.Val}");
			i++;
		}
		Assert.AreEqual(expectedValues.Length, i);
	}
	

	public interface IWithValue
	{
		int Val { get; }
	}

	public struct OneFrameSignal : IWithValue, IBlahSignal
	{
		public int Value;

		int IWithValue.Val => Value;
	}

	public struct NotOneFrameSignal : IWithValue, IBlahSignal, IBlahNotOneFrame
	{
		public int Value;
		
		int IWithValue.Val => Value;
	}

	public struct KeepOrderSignal : IWithValue, IBlahSignal, IBlahNotOneFrame, IBlahKeepOrder
	{
		public int     Value;
		
		int IWithValue.Val => Value;
	}
}
}