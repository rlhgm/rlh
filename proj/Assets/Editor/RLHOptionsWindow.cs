using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class RLHOptionsWindow : EditorWindow{

	static bool PhysicVisibility;
	static bool LevelArtVisibility = false;
	static GameObject LevelArtGameObject = null;

	static string GroundNormalPrefabPath = "Assets/mp/Prefabs/tile_ground1_normal.prefab";
	static string GroundHandleLPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleL.prefab";
	static string GroundHandleRPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleR.prefab";
	static string GroundHandleLRPrefabPath = "Assets/mp/Prefabs/tile_ground1_handleLR.prefab";
	static string GroundMountPrefabPath = "Assets/mp/Prefabs/tile_mount.prefab";

	static string CheckpointPrefabPath = "Assets/mp/Prefabs/CheckPoint.prefab";
	static string KillerPhysicPrefabPath = "Assets/mp/Prefabs/KillerPhysic.prefab";
	static string ShowInfoTriggerPrefabPath = "Assets/mp/Prefabs/ShowInfoTrigger.prefab";

    static string ShowInfoTriggerControllerPrefabPath = "Assets/mp/Prefabs/ShowInfoTriggerController.prefab";
    static string SmashStoneActivatorPrefabPath = "Assets/mp/Prefabs/SmashStoneActivator.prefab";

    //public static float gravityGunInertiaFactor = GravityGun.inertiaFactor;
    //public static float gravityGunMaxDist = GravityGun.maxDistance;



    [MenuItem ("Window/RLH Options")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(RLHOptionsWindow));
		//Debug.Log( "show RLHOptionsWindow" );

		GameObject prefabGround = AssetDatabase.LoadAssetAtPath<GameObject>(GroundNormalPrefabPath);
		Transform prefabGfx = prefabGround.transform.Find("gfx");

		PhysicVisibility = prefabGfx.GetComponent<SpriteRenderer>().enabled;
		//gravityGunInertiaFactor = GravityGun.inertiaFactor;
		//gravityGunMaxDist = GravityGun.maxDistance;

		LevelArtGameObject = GameObject.Find ("Level Art");
		if (LevelArtGameObject != null) {
			LevelArtVisibility = LevelArtGameObject.activeInHierarchy;
		}
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
		bool newPhysicVisibility = EditorGUILayout.Toggle ("Physics visibility", PhysicVisibility);
		if (newPhysicVisibility != PhysicVisibility) {
			setPhysicVisibility(newPhysicVisibility);
		}

		if (LevelArtGameObject != null) {
			LevelArtVisibility = LevelArtGameObject.activeInHierarchy;
			bool newLevelArtVisibility = EditorGUILayout.Toggle ("LevelArt visibility", LevelArtVisibility);
			if (newLevelArtVisibility != LevelArtVisibility) {
				setLevelArtVisibility (newLevelArtVisibility);
			}
		} else {
			EditorGUILayout.LabelField( "Nie ma noda : Level Art" );
		}

//		//float newGravityGunInertiaFactor = EditorGUILayout.FloatField("GravityGun interia factor:", gravityGunInertiaFactor);
//		float newGravityGunInertiaFactor = EditorGUILayout.Slider("GravityGun interia factor:",gravityGunInertiaFactor,0.001f, 1f);
//		if (newGravityGunInertiaFactor != gravityGunInertiaFactor) {
//			gravityGunInertiaFactor = newGravityGunInertiaFactor;
//			GravityGun.inertiaFactor = newGravityGunInertiaFactor;
//		}

//		//float newGravityGunMaxDist = EditorGUILayout.FloatField("GravityGun max distance:", gravityGunMaxDist);
//		float newGravityGunMaxDist = EditorGUILayout.Slider("GravityGun max distance:",gravityGunMaxDist, 0.001f, 100f);
//		if (newGravityGunMaxDist != gravityGunMaxDist) {
//			gravityGunMaxDist = newGravityGunMaxDist;
//			GravityGun.maxDistance = newGravityGunMaxDist;
//		}

		//if (GUILayout.Button ("Set")) {
		//	GravityGun.inertiaFactor = gravityGunInertiaFactor;
		//}
	}

	void OnHierarchyChange() {
		//EditorApplication.ExecuteMenuItem("Assets/Sync MonoDevelop Project");
//		Debug.Log("OnHierarchyChange");
//
//		LevelArtGameObject = GameObject.Find ("Level Art");
//		if (LevelArtGameObject != null) {
//			LevelArtVisibility = LevelArtGameObject.activeInHierarchy;
//		}

	}

	void OnProjectChange() {
		//EditorApplication.ExecuteMenuItem("Assets/Sync MonoDevelop Project");
		Debug.Log("OnProjectChange");
	}

	void setPhysicVisibility(bool newVisibility){
		PhysicVisibility = newVisibility;

		//Debug.Log ("RLHOptionsWindow::setPhysicVisibility : " + physicVisibility);

		GameObject prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundNormalPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundHandleLPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;
		prefabObject.transform.Find ("handleGfxL").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundHandleRPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;
		prefabObject.transform.Find ("handleGfxR").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundHandleLRPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;
		prefabObject.transform.Find ("handleGfxL").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;
		prefabObject.transform.Find ("handleGfxR").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(GroundMountPrefabPath);
		prefabObject.transform.Find ("gfx").GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(CheckpointPrefabPath);
		prefabObject.GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;

		prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(KillerPhysicPrefabPath);
		prefabObject.GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;

        prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(ShowInfoTriggerPrefabPath);
        prefabObject.GetComponent<SpriteRenderer>().enabled = PhysicVisibility;

        prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(ShowInfoTriggerControllerPrefabPath);
		prefabObject.GetComponent<SpriteRenderer> ().enabled = PhysicVisibility;

        prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(SmashStoneActivatorPrefabPath);
        prefabObject.GetComponent<SpriteRenderer>().enabled = PhysicVisibility;

        //static string ShowInfoTriggerControllerPrefabPath = "Assets/mp/Prefabs/ShowInfoTriggerController.prefab";
        //static string SmashStoneActivatorPrefabPath = "Assets/mp/Prefabs/SmashStoneActivator.prefab";

    }

	void setLevelArtVisibility(bool newVisibility){
		if (LevelArtGameObject != null) {
			LevelArtVisibility = newVisibility;
			LevelArtGameObject.SetActive( LevelArtVisibility );
		}
	}

//	void OnSceneGUI () {
//		if (Event.current.type == EventType.MouseDown) {
//			Debug.Log("Scene Mouse Down");
//			Event.current.Use();
//		}
//	}
}
