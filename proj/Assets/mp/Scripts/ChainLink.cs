using UnityEngine;
using System.Collections;

public class ChainLink : MonoBehaviour {

	DistanceJoint2D joint;
	Rigidbody2D body;

	void Awake(){
		joint = GetComponent<DistanceJoint2D> ();
		body = GetComponent<Rigidbody2D> ();

		//print (joint);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			//joint.connectedAnchor
			body.AddForce( new Vector2(-500,0) );
		}

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			body.AddForce( new Vector2(500,0) );
		}

	}

	void FixedUpdate(){

	}

}
