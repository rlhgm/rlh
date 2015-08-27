using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Player2Controller))]
[CanEditMultipleObjects]
public class RLHEditor : Editor{
	
	void OnSceneGUI () {
		//Debug.Log("Scene Mouse Down");
//		if (Event.current.type == EventType.MouseDown) {
//			Debug.Log("Scene Mouse Down");
//			Event.current.Use();
//		}
	}

}
