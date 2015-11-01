using UnityEngine;
using UnityEditor;

public class ZapControllerKnifeCreateAsset
{
	[MenuItem("Assets/Create/ZapControllerKnife")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<ZapControllerKnife> ();
	}
}
