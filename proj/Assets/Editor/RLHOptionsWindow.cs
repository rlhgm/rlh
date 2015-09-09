using UnityEngine;
using UnityEditor;
using System.Collections;

public class RLHOptionsWindow : EditorWindow{

	static bool physicVisibility;

	static string GroundNormalPrefabPath = "Assets/mp/Prefabs/tile_ground1_normal.prefab";
	static string GroundHandleLPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleL.prefab";
	static string GroundHandleRPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleR.prefab";
	static string GroundHandleLRPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleLR.prefab";
	static string GroundMountPrefabPath = "Assets/mp/Prefabs/tile_mount.prefab";

	static string CheckpointPrefabPath = "Assets/mp/Prefabs/CheckPoint.prefab";
	static string KillerPhysicPrefabPath = "Assets/mp/Prefabs/KillerPhysic.prefab";
	static string ShowInfoTriggerPrefabPath = "Assets/mp/Prefabs/ShowInfoTrigger.prefab";

	public static float gravityGunInertiaFactor = GravityGun.inertiaFactor;
	public static float gravityGunMaxDist = GravityGun.maxDistance;

	[MenuItem ("Window/RLH Options")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(RLHOptionsWindow));
		//Debug.Log( "show RLHOptionsWindow" );

		GameObject prefabGround = AssetDatabase.LoadAssetAtPath<GameObject>(GroundNormalPrefabPath);
		Transform prefabGfx = prefabGround.transform.Find("gfx");

		physicVisibility = prefabGfx.GetComponent<SpriteRenderer>().enabled;
		gravityGunInertiaFactor = GravityGun.inertiaFactor;
		gravityGunMaxDist = GravityGun.maxDistance;
	}

	void SceneGUI(SceneView sceneView)
	{
		// This will have scene events including mouse down on scenes objects
		Event cur = Event.current;

		Debug.Log ("RLHOptionsWindow::SceneGUI : " + cur.type);

		if (cur.type == EventType.MouseDown) {
			Debug.Log ("Mouse Down");
		}

	}

	void OnGUI () {

		//Debug.Log ("RLHOptionsWindow::OnGUI : " + Event.current.type);

		if (Event.current.type == EventType.MouseDown) {
			//Debug.Log("Mouse Down");
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

		GameObject prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundNormalPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundHandleLPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
		prefabObject.transform.Find ("handleGfxL").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundHandleRPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
		prefabObject.transform.Find ("handleGfxR").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundHandleLRPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
		prefabObject.transform.Find ("handleGfxL").GetComponent<SpriteRenderer> ().enabled = physicVisibility;
		prefabObject.transform.Find ("handleGfxR").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundMountPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(CheckpointPrefabPath);
		prefabObject.GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(KillerPhysicPrefabPath);
		prefabObject.GetComponent<SpriteRenderer> ().enabled = physicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(ShowInfoTriggerPrefabPath);
		prefabObject.GetComponent<SpriteRenderer> ().enabled = physicVisibility;
	}

//	void OnSceneGUI () {
//		if (Event.current.type == EventType.MouseDown) {
//			Debug.Log("Scene Mouse Down");
//			Event.current.Use();
//		}
//	}
}
