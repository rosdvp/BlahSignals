using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BlahSignals.AutoOrder.Attributes;

namespace BlahSignals.AutoOrder.Editor
{
public static class BlahAutoOrderer
{
	public static List<Type> OrderAndGroupSystems(IReadOnlyList<Type> systems)
	{
		var orderedItems = new List<OrderItem>();
		
		//fill orderedItems by groups, systems and requirements
		foreach (var system in systems)
		{
			OrderItem groupItem = null;
			if (TryGetAttrs<BlahGroupAttribute>(system, out var groupAttrs))
			{
				var group        = groupAttrs[0].Group;
				var groupItemIdx = orderedItems.FindIndex(g => g.Group == group);

				if (groupItemIdx == -1)
				{
					groupItem = new OrderItem(group);
					orderedItems.Add(groupItem);
				}
				else
					groupItem = orderedItems[groupItemIdx];
			}

			var systemItem = CreateSystemOrderItem(system);
			if (groupItem == null)
				orderedItems.Add(systemItem);
			else
			{
				groupItem.InnerOrderedItems.Add(systemItem);
				groupItem.AllUnOrderedTypes.UnionWith(systemItem.AllUnOrderedTypes);
				groupItem.AllUnOrderedSystems.UnionWith(systemItem.AllUnOrderedSystems);
				groupItem.AfterRequirements.UnionWith(systemItem.AfterRequirements);
			}
		}

		foreach (var item in orderedItems)
			if (item.Group != null)
				OrderItems(item.InnerOrderedItems);
		OrderItems(orderedItems);

		var result = new List<Type>();
		foreach (var item in orderedItems)
			if (item.Group == null)
				result.AddRange(item.AllUnOrderedSystems);
			else
				foreach (var subItem in item.InnerOrderedItems)
					result.AddRange(subItem.AllUnOrderedSystems);
		return result;
	}
	
	
	private static void OrderItems(List<OrderItem> items)
	{
		int depthCounter = 0;
		for (var i = 0; i < items.Count; i++)
		{
			afterItemMoved: ;
			foreach (var afterSystem in items[i].AfterRequirements)
			{
				for (var j = i + 1; j < items.Count; j++)
					if (items[j].AllUnOrderedTypes.Contains(afterSystem))
					{
						if (depthCounter++ > 1000)
							throw new Exception($"Depth limit exceeded, may be cyclic EcsAfter?\n" +
							                    $"A\n{items[i]}\n"                                 +
							                    $"B\n{items[j]}");
						if (j + 1 < items.Count)
							items.Insert(j + 1, items[i]);
						else
							items.Add(items[i]);
						items.RemoveAt(i);
						goto afterItemMoved;
					}
			}
		}
	}
	
	
	private static OrderItem CreateSystemOrderItem(Type system)
	{
		var systemItem = new OrderItem(null);
		systemItem.AllUnOrderedSystems.Add(system);
		systemItem.AllUnOrderedTypes.Add(system);
		
		if (TryGetAttrs<BlahAfterSystemAttribute>(system, out var afterAttrs))
			foreach (var afterAttr in afterAttrs)
				systemItem.AfterRequirements.Add(afterAttr.AfterType);

		CollectProduceAndConsumeTypes(system,
		                              systemItem.AllUnOrderedTypes,
		                              systemItem.AfterRequirements);
		return systemItem;
	}

	private static void CollectProduceAndConsumeTypes(Type          scanType,
	                                                  HashSet<Type> produceTypes,
	                                                  HashSet<Type> consumeTypes)
	{
		while (scanType != typeof(object) && scanType != null)
		{
			var fields = scanType.GetFields(BindingFlags.Instance |
			                              BindingFlags.Public |
			                              BindingFlags.NonPublic);
			foreach (var field in fields)
			{
				var fieldType = field.FieldType;
				if (fieldType.IsGenericType)
				{
					var genType = fieldType.GetGenericArguments()[0];
					if (field.IsDefined(typeof(BlahProduceAttribute)))
						produceTypes.Add(genType);
					else if (field.IsDefined(typeof(BlahConsumeAttribute)))
						consumeTypes.Add(genType);
				}
			}
			scanType = scanType.BaseType;
		}
	}

	private static bool TryGetAttrs<T>(Type type, out T[] attrs) where T : Attribute
	{
		object[] rawAttrs = type.GetCustomAttributes(typeof(T), false);
		attrs = rawAttrs.Length > 0 ? rawAttrs.OfType<T>().ToArray() : null;
		return attrs != null;
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private class OrderItem
	{
		public readonly Type Group;

		public readonly List<OrderItem> InnerOrderedItems = new();

		/// <summary>
		/// Includes both systems types and producible types
		/// </summary>
		public readonly HashSet<Type> AllUnOrderedTypes  = new();
		
		/// <summary>
		/// Includes only systems types
		/// </summary>
		public readonly HashSet<Type> AllUnOrderedSystems = new();

		/// <summary>
		/// Includes both systems and consumables that should go before this item
		/// </summary>
		public readonly HashSet<Type> AfterRequirements = new();

		public OrderItem(Type group)
		{
			Group = group;
		}

		public override string ToString()
			=> $"Group: {Group?.Name}\n" +
			   $"Types: {string.Join(", ", AllUnOrderedTypes.Select(s => s.Name))}";
	}
}
}