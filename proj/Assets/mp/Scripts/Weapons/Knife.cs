using UnityEngine;
using System.Collections;

public class Knife : Weapon {

	public Knife (Player2Controller playerController) 
		: base("Knife", playerController)
	{
		Debug.Log ("hello world - knife");
	}
	
	public override void Update () {
		
	}
	
	public override void FUpdate () {
		
	}
}
