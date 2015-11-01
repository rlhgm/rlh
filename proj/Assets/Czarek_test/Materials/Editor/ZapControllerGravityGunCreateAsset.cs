using UnityEngine;
using UnityEditor;

public class ZapControllerGravityGunCreateAsset
{
	[MenuItem("Assets/Create/ZapControllerGravityGun")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<ZapControllerGravityGun> ();
	}
}
