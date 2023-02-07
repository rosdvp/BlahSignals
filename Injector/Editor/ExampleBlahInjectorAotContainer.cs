using UnityEngine.Scripting;

namespace BlahSignals.Injector.Editor
{
/// <summary>
/// 1. Copy this class to any folder and rename it to "BlahInjectorAotContainer".
/// 2. Uncomment line 14; but do not ever touch //START-CODEGEN and //END-CODEGEN lines!
/// 3. Press Tools/Blah/Generate Injector AOT, and this file will be updated.
/// </summary>
public class ExampleBlahInjectorAotContainer
{
	[Preserve]
	private void AOT()
	{
		//var signals = new BlahSignalsContext();
		
		//START-CODEGEN
		//END-CODEGEN
	}
	
	
	//Modify this template if API has been changed.
	//[SIGNAL_NAME] will be replaced during code generation.
#pragma warning disable CS0414
	private static readonly string CodeGenTemplate = "\t\tsignals.Get<[SIGNAL_NAME]>();";
#pragma warning restore CS0414
}
}
