using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BlahSignals.AutoOrder.Editor
{
public static class BlahAutoOrderFileGenerator
{
	private const string UN_ORDERED_CONTAINER_TYPE_NAME = "BlahUnOrderedSystemsContainer";
	private const string UN_ORDERED_SYSTEMS_FIELD_NAME  = "SystemsToOrder";
	private const string CODE_GEN_TEMPLATE_FIELD_NAME  = "CodeGenTemplate";
	
	private const string ORDERED_CONTAINER_TYPE_NAME = "BlahOrderedSystemsContainer";
	
	[MenuItem("Tools/Blah/Re-order systems")]
	public static void EditorCreateFileWithOrderedSystems()
	{
		var unOrderedContainer = FindUnOrderedContainer();
		var unOrderedSystems   = ExtractUnOrderedSystems(unOrderedContainer);
		var codeGenTemplate    = ExtractCodeGenTemplate(unOrderedContainer);
		
		var orderedSystems = BlahAutoOrderer.OrderAndGroupSystems(unOrderedSystems);
		
		var generatedLines  = new List<string>();
		foreach (var system in orderedSystems)
			generatedLines.Add(codeGenTemplate.Replace("[SYSTEM_NAME]", system.Name));

		var path     = GetFilePath(ORDERED_CONTAINER_TYPE_NAME);
		var lines    = File.ReadAllLines(path).ToList();
		var startIdx = lines.FindIndex(s => s.Contains("//START-CODEGEN"));
		var endIdx   = lines.FindIndex(s => s.Contains("//END-CODEGEN"));
		lines.RemoveRange(startIdx + 1, endIdx - startIdx - 1);
		lines.InsertRange(startIdx + 1, generatedLines);
		File.WriteAllLines(path, lines);
		AssetDatabase.ImportAsset(path);

		Debug.Log("[Blah] systems re-ordered!");
	}


	private static Type FindUnOrderedContainer()
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		foreach (var type in assembly.ExportedTypes)
		{
			if (type.Name == UN_ORDERED_CONTAINER_TYPE_NAME)
				return type;
		}
		throw new Exception($"Failed to find {UN_ORDERED_CONTAINER_TYPE_NAME} type");
	}

	private static List<Type> ExtractUnOrderedSystems(Type unOrderedContainer)
	{
		var field = unOrderedContainer.GetField(UN_ORDERED_SYSTEMS_FIELD_NAME,
		                                        BindingFlags.NonPublic | BindingFlags.Static);
		if (field == null)
			throw new Exception(
				$"Failed to find field {UN_ORDERED_SYSTEMS_FIELD_NAME} of {UN_ORDERED_CONTAINER_TYPE_NAME}");
		return (List<Type>)field.GetValue(null);
	}

	private static string ExtractCodeGenTemplate(Type unOrderedContainer)
	{
		var field = unOrderedContainer.GetField(CODE_GEN_TEMPLATE_FIELD_NAME,
		                                        BindingFlags.NonPublic | BindingFlags.Static);
		if (field == null)
			throw new Exception(
				$"Failed to find field {CODE_GEN_TEMPLATE_FIELD_NAME} of {UN_ORDERED_CONTAINER_TYPE_NAME}");
		return (string)field.GetValue(null);
	}
	
	private static string GetFilePath(string fileName)
	{
		var guids = AssetDatabase.FindAssets(fileName);
		if (guids == null || guids.Length == 0)
			throw new Exception($"Failed to find path to {fileName}");
		return AssetDatabase.GUIDToAssetPath(guids[0]);	
	}
}
}