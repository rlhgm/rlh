using UnityEngine;
using System.Collections;

public class Pickable : MonoBehaviour {

	public enum Type{
		KNIFE,
		GRAVITY_GUN
	}

	public Type type = Type.KNIFE;
	public bool isActive = true;

	// Use this for initialization
	void Start () {
		soh ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void activate(){
		isActive = true;
		soh ();
	}

	public void deactivate(){
		isActive = false;
		soh ();
	}

	void soh(){
		GetComponent<SpriteRenderer> ().enabled = isActive;
	}
}
