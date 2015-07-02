using UnityEngine;
using System.Collections;

public class NewRope : MonoBehaviour {

	public RopeLink ropeLinkPrefab;
	//Rigidbody2D driverRigidBody;
	Transform currentLink;

	void Awake(){
		//joint = GetComponent<DistanceJoint2D> ();
		//body = GetComponent<Rigidbody2D> ();
		
		//print (joint);

		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();
		Destroy (spriteRenderer);
		int numberOfLinks = (int)Mathf.Floor(transform.localScale.y) * 2;
		transform.localScale = new Vector3 (1, 1, 1);
		//print( "SCALE : " + transform.localScale );

		RopeLink lastLink = null;
		for (int i = 0; i < numberOfLinks; ++i) {
			RopeLink newLink = Instantiate<RopeLink>(ropeLinkPrefab);

			HingeJoint2D hingeJoint = newLink.GetComponent<HingeJoint2D>();

			hingeJoint.anchor = new Vector2(0f,0f);

			if( lastLink ){
				Rigidbody2D lastRigidBody = lastLink.GetComponent<Rigidbody2D>();
				hingeJoint.connectedBody = lastRigidBody;
				newLink.transform.SetParent( lastLink.transform );
				hingeJoint.connectedAnchor = new Vector2(0f,-0.55f);
				newLink.transform.position = lastLink.transform.position + new Vector3(0f,-0.5f);
			}else{
				//newLink.transform.SetParent( transform );
				hingeJoint.connectedAnchor = transform.position;
				newLink.transform.position = transform.position;
			}

			lastLink = newLink;
			currentLink = newLink.transform;
		}

		chooseDriver (currentLink);
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Z)) {
			//joint.connectedAnchor
			//print(driverRigidBody.velocity);
			//driverRigidBody.AddForce( new Vector2(-500,0) );
			currentLink.GetComponent<Rigidbody2D>().AddForce( new Vector2(-500,0) );
		}
		
		if (Input.GetKeyDown (KeyCode.X)) {
			//print(driverRigidBody.velocity);
			//driverRigidBody.AddForce( new Vector2(500,0) );
			currentLink.GetComponent<Rigidbody2D>().AddForce( new Vector2(500,0) );
		}//aaa

		if (Input.GetKeyDown (KeyCode.C)) {
			if( currentLink.parent ) chooseDriver(currentLink.parent);
		}
		
		if (Input.GetKeyDown (KeyCode.V)) {
			if( currentLink.childCount > 0 ) chooseDriver(currentLink.GetChild(0));
		}

	}
	
	void FixedUpdate(){
		
	}

	void chooseDriver(Transform newLink){
		if( currentLink )
			currentLink.GetComponent<SpriteRenderer> ().color = Color.white;
			currentLink.GetComponent<Rigidbody2D> ().mass = 1.0f;

		currentLink = newLink;

		newLink.GetComponent<SpriteRenderer> ().color = Color.red;
		newLink.GetComponent<Rigidbody2D> ().mass = 8.0f;
	}
}
