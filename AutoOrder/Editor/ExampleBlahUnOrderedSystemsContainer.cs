using System;
using System.Collections.Generic;

namespace BlahSignals.AutoOrder.Editor
{
/// <summary>
/// 1. Follow instructions in <see cref="ExampleBlahOrderedSystemsContainer"/>.<br/>
/// 2. Copy this file in the same folder as "BlahUnOrderedSystemsContainer".<br/>
/// 3. Change the class name and file name to BlahOrderedSystemsContainer (without Example).<br/>
/// 4. Put all your systems in the list below.<br/>
/// 5. Press Tools/Blah/Re-order systems each time when you add a new system in the list below.<br/>
/// This will re-generate "BlahOrderedSystemsContainer" with systems in correct order.<br/>
/// </summary>
#if UNITY_EDITOR
public static class ExampleBlahUnOrderedSystemsContainer
{
	private static readonly List<Type> SystemsToOrder = new()
	{
		//put your systems' types here
	};

	
	
	//Modify this template if API has been changed.
	//[SYSTEM_NAME] will be replaced during code generation.
#pragma warning disable CS0414
	private static readonly string CodeGenTemplate = "\t\tsystems.AddSystem(new [SYSTEM_NAME]());";
#pragma warning restore CS0414
}
#endif
}