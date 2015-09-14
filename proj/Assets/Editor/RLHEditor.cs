using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

//[CustomEditor(typeof(Player2Controller))]
//[CanEditMultipleObjects]
[ExecuteInEditMode]
public class RLHEditorWindow : EditorWindow{

	static EditorWindow window;
	static bool editEnabled = false;

	//public static void  ShowWindow () {
	//	EditorWindow.GetWindow(typeof(RLHEditorWindow));
		//Debug.Log( "show RLHOptionsWindow" );
	//}

	RLHEditorWindow(){
		Debug.Log ("RLHEditorWindow()");
		SceneView.onSceneGUIDelegate += OnSceneGUI; //Sets delegate for adding the OnSceneGUI event
		editEnabled = false;
		titleContent.text = "RLHEditor";
		//Debug.ClearDeveloperConsole();

		window = EditorWindow.GetWindow(typeof(RLHEditorWindow));//Initialize window
		//window.minSize = new Vector2(325,400);
		//window.titleContent.text = "RLHEditor";
	}
	~RLHEditorWindow()
	{
		Debug.Log ("~RLHEditorWindow()");
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		editEnabled = false;
	}

	[MenuItem("Window/RLHEditor")]
	public static void OnEnable() 
	{
		Debug.Log ("RLHEditorWindow()");

		//Reset variables chunk. This is for new files being added, generated, etc.
		AssetDatabase.Refresh ();
		//SceneView.onSceneGUIDelegate += OnSceneGUI; //Sets delegate for adding the OnSceneGUI event

		//window = EditorWindow.GetWindow(typeof(RLHEditorWindow));//Initialize window
		window.minSize = new Vector2(325,400);
		//window.titleContent.text = "RLHEditor";

		//Selection.selectionChanged += 
	}
	void OnDisable(){
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		//editEnabled = false;
	}

	void OnGUI () {
		//GUILayout.Label(EditorWindow.focusedWindow.ToString());



		//Debug.Log ("RLHEditorWindow::OnGUI : " + Event.current.type);

//		if (Event.current.type == EventType.MouseDown) {
//			Debug.Log("Mouse Down");
//
//			Debug.Log( Selection.activeGameObject );
//			Debug.Log( Selection.activeObject );
//			Debug.Log( Selection.assetGUIDs );
//			//Debug.Log( Selection.activeGameObject.name );
//
//						//Event.current.Use();
//			
//						//Debug.Log( Selection.activeGameObject.name );
//						//Debug.Log( Selection.activeObject.name );
//						//Debug.Log( Selection.activeInstanceID );
//		}

		if( Event.current != null ){

			//Debug.Log ("RLHEditorWindow::OnGUI : " + Event.current.type);

			if( Event.current.isKey ){
				
				Event ev = Event.current;
				
				if( ev.keyCode == KeyCode.N ){
					
					if( ev.type == EventType.KeyDown ){
						//if( !editEnabled ){
							//Debug.Log ("OnSceneGUI::Tiles mode enabled");
							//window.titleContent.text = "RLHEditor(ON)";
							editEnabled = !editEnabled;

							Debug.Log ("OnSceneGUI::Tiles mode enabled");
							Repaint();
						//}
					}
//					if( ev.type == EventType.KeyUp ){
//						if( editEnabled ){
//							Debug.Log ("OnSceneGUI::Tiles mode disabled");
//							//window.titleContent.text = "RLHEditor(OFF)";
//							editEnabled = false;
//						}
//					}
					
				}
			}
		}

		editEnabled = EditorGUILayout.Toggle ("Enabled", editEnabled);
	}

	static void OnSceneGUI( SceneView sceneview ){

		if( Event.current != null ){
			
			//Debug.Log ("RLHEditorWindow::OnGUI : " + Event.current.type);

			if( Event.current.isKey ){
				
				Event ev = Event.current;
				
				if( ev.keyCode == KeyCode.N ){
					
					if( ev.type == EventType.KeyDown ){
						//if( !editEnabled ){

							//window.titleContent.text = "RLHEditor(ON)";
							editEnabled = !editEnabled;

							Debug.Log ("OnSceneGUI::Tiles mode : " + editEnabled);
							window.Repaint();
						//}
					}
//					if( ev.type == EventType.KeyUp ){
//						if( editEnabled ){
//							Debug.Log ("OnSceneGUI::Tiles mode disabled");
//							//window.titleContent.text = "RLHEditor(OFF)";
//							editEnabled = false;
//						}
//					}
					
				}
			}
		}

		if (!editEnabled)
			return;

		//Debug.Log("OnSceneGUI");

//		if (Event.current.type == EventType.MouseDown) {
//
//			Debug.Log( Selection.activeGameObject );
//			//Debug.Log( Selection.activeObject );
//			//Debug.Log( Selection.assetGUIDs );
//
//			//if( Selection.activeObject ){
//			//	EditorGUIUtility.PingObject(Selection.activeObject);
//			//}
//
//			//Debug.Log("Scene Mouse Down");
//			Event.current.Use();
//		}

		// Retrieve the control Id
		int controlId = GUIUtility.GetControlID(FocusType.Passive);
		//HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

		// Start treating your events
		switch(Event.current.type)
		{
		case EventType.MouseDown:
			//Treat your event

			Debug.Log( Selection.activeGameObject );
			Debug.Log( Selection.activeObject );
			Debug.Log( Selection.assetGUIDs.Length );
			Debug.Log( Selection.objects.Length );

			if( Selection.activeObject ){
				//Debug.Log (Selection.activeObject.GetType());
				string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject); // relative path
				if (Directory.Exists(selectionPath)) {
					// do something
					Debug.Log ( " " + Selection.activeObject + " to jest folder");
				}
			}

			if( Selection.activeGameObject ){
				Object newObject = Instantiate( Selection.activeGameObject );

			}

			// ...
			Debug.Log("EventType.MouseDown 2");
			// Tell the UI your event is the main one to use, it override the selection in  the scene view
			GUIUtility.hotControl = controlId;
			// Don't forget to use the event
			Event.current.Use();

			break;
		}
	}

	void OnSelectionChange() {
		//selectionIDs = Selection.instanceIDs;

		//Debug.Log( Selection.activeGameObject );
		//Debug.Log( Selection.activeObject );
		//Debug.Log( Selection.assetGUIDs );

	}
}
