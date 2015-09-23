using UnityEngine;
using System.Collections;

public class RopeLink : MonoBehaviour {

	public NewRope rope;
	public int idn;

	public void cut(){
		rope.cut (idn);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
