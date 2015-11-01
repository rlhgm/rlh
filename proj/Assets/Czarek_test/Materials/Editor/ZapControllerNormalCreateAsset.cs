using UnityEngine;
using UnityEditor;

public class ZapControllerNormalCreateAsset
{
	[MenuItem("Assets/Create/ZapControllerNormal")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<ZapControllerNormal> ();
	}
}
