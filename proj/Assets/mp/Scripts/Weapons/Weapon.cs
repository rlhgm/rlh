using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

public class Weapon : IComparable<Weapon> { //: MonoBehaviour {

	public string name;
	public Player2Controller player;

	// Use this for initialization
	public Weapon (string weaponName, Player2Controller playerController) {
		Debug.Log ("hello world - weapon");
		name = weaponName;
		player = playerController;
	}
	
	public virtual void Update (float deltaTime) {	
	}

	public virtual void FUpdate(float fDeltaTime){
	}

	public virtual void activate(){
	}
	public virtual void deactivate(){
	}

	//This method is required by the IComparable
	//interface. 
	public int CompareTo(Weapon other)
	{
		if(other == null)
		{
			return 1;
		}
		
		//Return the difference in power.
		return name.CompareTo(other.name);
	}

	public override string ToString(){
		return name;
	}
}
