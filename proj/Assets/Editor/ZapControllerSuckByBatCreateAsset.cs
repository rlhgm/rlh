using UnityEngine;
using UnityEditor;

public class ZapControllerSuckByBatCreateAsset
{
	[MenuItem("Assets/Create/ZapControllerSuckByBat")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<ZapControllerSuckedByBat> ();
	}
}
