using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

public class Weapon : IComparable<Weapon> { //: MonoBehaviour {

	public string name;

	// Use this for initialization
	public Weapon (string weaponName) {
		Debug.Log ("hello world - weapon");
		name = weaponName;
	}
	
//	// Update is called once per frame
//	void Update () {
//	
//	}

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

	public string ToString(){
		return name;
	}
}
