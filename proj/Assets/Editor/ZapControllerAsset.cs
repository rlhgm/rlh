using UnityEngine;
using UnityEditor;

public class YourClassAsset
{
	[MenuItem("Assets/Create/ZapControllerNormal")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<ZapControllerNormal> ();
	}
}
