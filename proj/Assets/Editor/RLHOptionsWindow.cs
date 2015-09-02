using UnityEngine;
using UnityEditor;
using System.Collections;

public class RLHOptionsWindow : EditorWindow{

	static bool physicVisibility;

	static string groundNormalPrefabPath = "Assets/mp/Prefabs/tile_ground1_normal.prefab";
	static string groundHandleLPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleL.prefab";
	static string groundHandleRPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleR.prefab";
	static string groundHandleLRPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleLR.prefab";
	static string groundMountPrefabPath = "Assets/mp/Prefabs/tile_mount.prefab";

	public static float gravityGunInertiaFactor = GravityGun.inertiaFactor;
	public static float gravityGunMaxDist = GravityGun.maxDistance;

	[MenuItem ("Window/RLH Options")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(RLHOptionsWindow));
		//Debug.Log( "show RLHOptionsWindow" );

		GameObject prefabGround = AssetDatabase.LoadAssetAtPath<GameObject>(groundNormalPrefabPath);
		Transform prefabGfx = prefabGround.transform.Find("gfx");

		physicVisibility = prefabGfx.GetComponent<SpriteRenderer>().enabled;
		gravityGunInertiaFactor = GravityGun.inertiaFactor;
		gravityGunMaxDist = GravityGun.maxDistance;
	}

	void OnGUI () {
		if (Event.current.type == EventType.MouseDown) {
//			//Debug.Log("Mouse Down");
//			//Event.current.Use();
//
//			//Debug.Log( Selection.activeGameObject.name );
//			//Debug.Log( Selection.activeObject.name );
//			//Debug.Log( Selection.activeInstanceID );
		}

		// The actual window code goes here
		bool newPhysicVisibility = EditorGUILayout.Toggle ("Physics visibility", physicVisibility);
		if (newPhysicVisibility != physicVisibility) {
			setPhysicVisibility(newPhysicVisibility);
		}

		//float newGravityGunInertiaFactor = EditorGUILayout.FloatField("GravityGun interia factor:", gravityGunInertiaFactor);
		float newGravityGunInertiaFactor = EditorGUILayout.Slider("GravityGun interia factor:",gravityGunInertiaFactor,0.001f, 1f);
		if (newGravityGunInertiaFactor != gravityGunInertiaFactor) {
			gravityGunInertiaFactor = newGravityGunInertiaFactor;
			GravityGun.inertiaFactor = newGravityGunInertiaFactor;
		}

		//float newGravityGunMaxDist = EditorGUILayout.FloatField("GravityGun max distance:", gravityGunMaxDist);
		float newGravityGunMaxDist = EditorGUILayout.Slider("GravityGun max distance:",gravityGunMaxDist, 0.001f, 100f);
		if (newGravityGunMaxDist != gravityGunMaxDist) {
			gravityGunMaxDist = newGravityGunMaxDist;
			GravityGun.maxDistance = newGravityGunMaxDist;
		}

		//if (GUILayout.Button ("Set")) {
		//	GravityGun.inertiaFactor = gravityGunInertiaFactor;
		//}
	}

	void setPhysicVisibility(bool newVisibility){
		physicVisibility = newVisibility;

		GameObject prefabGround = AssetDatabase.LoadAssetAtPath<GameObject>(groundNormalPrefabPath);
		prefabGround.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabGround = AssetDatabase.LoadAssetAtPath<GameObject>(groundHandleLPrefabPath);
		prefabGround.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
		prefabGround.transform.Find ("handleGfxL").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabGround = AssetDatabase.LoadAssetAtPath<GameObject>(groundHandleRPrefabPath);
		prefabGround.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
		prefabGround.transform.Find ("handleGfxR").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabGround = AssetDatabase.LoadAssetAtPath<GameObject>(groundHandleLRPrefabPath);
		prefabGround.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
		prefabGround.transform.Find ("handleGfxL").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
		prefabGround.transform.Find ("handleGfxR").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabGround = AssetDatabase.LoadAssetAtPath<GameObject>(groundMountPrefabPath);
		prefabGround.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
	}

//	void OnSceneGUI () {
//		if (Event.current.type == EventType.MouseDown) {
//			Debug.Log("Scene Mouse Down");
//			Event.current.Use();
//		}
//	}
}
