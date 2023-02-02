using System;
using System.Collections.Generic;
using System.Reflection;

namespace BlahSignals.Injector
{
public class BlahInjector
{
	private readonly object     _provider;
	private readonly MethodInfo _providerGetMethod;
	private readonly Type       _targetFieldBaseType;

	public BlahInjector(object provider, MethodInfo providerGetMethod, Type targetFieldBaseType)
	{
		_provider            = provider;
		_providerGetMethod   = providerGetMethod;
		_targetFieldBaseType = targetFieldBaseType;
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	public void Inject(IReadOnlyCollection<object> targets)
	{
		if (targets == null)
			throw new Exception($"{nameof(targets)} must not be null!");
		
		foreach (object target in targets)
			Inject(target, target.GetType());
	}
	
	public void Inject(object[] targets)
	{
		if (targets == null)
			throw new Exception($"{nameof(targets)} must not be null!");
		
		foreach (object target in targets)
			Inject(target, target.GetType());
	}

	private void Inject(object target, Type type)
	{
		if (type.BaseType != typeof(System.Object))
			Inject(target, type.BaseType);
		
		var fields = type.GetFields(
			BindingFlags.Instance |
			BindingFlags.Static |
			BindingFlags.Public |
			BindingFlags.NonPublic);
		foreach (var field in fields)
		{
			var fieldType = field.FieldType;
			if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == _targetFieldBaseType)
			{
				var genericTypes = fieldType.GenericTypeArguments;
				object value = _providerGetMethod.MakeGenericMethod(genericTypes)
				                                 .Invoke(_provider, null);
				field.SetValue(target, value);
			}
		}
	}
}
}
