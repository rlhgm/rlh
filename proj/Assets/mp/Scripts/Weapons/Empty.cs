using UnityEngine;
using System.Collections;

public class Empty : Weapon {
	
	public Empty (Player2Controller playerController) 
		: base("None", playerController)
	{
		Debug.Log ("hello world - none");
	}
	
	//	// Update is called once per frame
	//	void Update () {
	//	
	//	}
}

