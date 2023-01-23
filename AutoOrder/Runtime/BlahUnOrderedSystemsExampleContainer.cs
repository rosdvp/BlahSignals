
using System.Collections.Generic;
#if UNITY_EDITOR
using System;
#endif

namespace BlahSignals.AutoOrder
{
/// <summary>
/// 1. Follow instructions in <see cref="BlahOrderedSystemsExampleContainer"/>.<br/>
/// 2. Copy this file in the same folder as <see cref="BlahOrderedSystemsContainer"/>.<br/>
/// 3. Change the class name and file name to BlahOrderedSystemsContainer (without Example).<br/>
/// 4. Put all your EcsSystems in the list below.<br/>
/// 5. Press Tools/EcsAutoOrder/Re-order systems each time when you add a new system in the list below.<br/>
/// This will re-generate <see cref="BlahOrderedSystemsContainer"/> with systems in correct order.<br/>
/// </summary>
#if UNITY_EDITOR
public static class BlahUnOrderedSystemsExampleContainer
{
	private static readonly List<Type> SystemsToOrder = new()
	{
		//put your systems' types here
	};

	
	
	//Modify this template if API has been changed.
	//[SYSTEM_NAME] will be replaced during code generation.
#pragma warning disable CS0414
	private static readonly string CodeGenTemplate = "\t\tsystems.Add(new [SYSTEM_NAME]());";
#pragma warning restore CS0414
}
#endif
}