using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BlahSignals.Injector.Editor
{
public class BlahInjectorFileGenerator
{
	private const string SIGNAL_INTERFACE_NAME        = "IBlahSignal";
	private const string CONTAINER_CLASS_TYPE         = "BlahInjectorAotContainer";
	private const string CODE_GEN_TEMPLATE_FIELD_NAME = "CodeGenTemplate";

	[MenuItem("Tools/Blah/Generate Injector AOT")]
	public static void EditorGenerateAOT()
	{
		var    containerType   = FindContainerType();
		string codeGenTemplate = ExtractCodeGenTemplate(containerType);
		
		var generatedLines  = new List<string>();
		foreach (var signalName in FindAllSignalsNames())
			generatedLines.Add(codeGenTemplate.Replace("[SIGNAL_NAME]", signalName));

		string path     = GetFilePath(CONTAINER_CLASS_TYPE);
		var    lines    = File.ReadAllLines(path).ToList();
		int    startIdx = lines.FindIndex(s => s.Contains("//START-CODEGEN"));
		int    endIdx   = lines.FindIndex(s => s.Contains("//END-CODEGEN"));
		lines.RemoveRange(startIdx + 1, endIdx - startIdx - 1);
		lines.InsertRange(startIdx + 1, generatedLines);
		File.WriteAllLines(path, lines);
		AssetDatabase.ImportAsset(path);

		Debug.Log("[Blah] Injector AOT generated!");
	}


	private static Type FindContainerType()
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		foreach (var type in assembly.ExportedTypes)
			if (type.Name == CONTAINER_CLASS_TYPE)
				return type;
		throw new Exception($"Failed to find {CONTAINER_CLASS_TYPE} type");
	}

	private static string ExtractCodeGenTemplate(Type containerType)
	{
		var field = containerType.GetField(CODE_GEN_TEMPLATE_FIELD_NAME,
		                                   BindingFlags.NonPublic | BindingFlags.Static);
		if (field == null)
			throw new Exception(
				$"Failed to find field {CODE_GEN_TEMPLATE_FIELD_NAME} of {CONTAINER_CLASS_TYPE}");
		return (string)field.GetValue(null);
	}
	
	private static IEnumerable<string> FindAllSignalsNames()
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			if (!assembly.IsDynamic)
				foreach (var type in assembly.ExportedTypes)
					if (type.GetInterface(SIGNAL_INTERFACE_NAME) != null)
						yield return type.Name;
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