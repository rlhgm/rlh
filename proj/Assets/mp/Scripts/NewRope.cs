using UnityEngine;
using System.Collections;

public class NewRope : MonoBehaviour {

	public RopeLink ropeLinkPrefab;
	//Rigidbody2D driverRigidBody;
	public Transform currentLink;

	RopeLink[] links;

	public float firstLinkSpeed;
	public float firstLinkMaxSpeed;
	public float firstLinkAngle;

	void Awake(){
		//joint = GetComponent<DistanceJoint2D> ();
		//body = GetComponent<Rigidbody2D> ();
		
		//print (joint);

		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();
		Destroy (spriteRenderer);
		int numberOfLinks = (int)Mathf.Floor(transform.localScale.y) * 2;
		transform.localScale = new Vector3 (1, 1, 1);
		//print( "SCALE : " + transform.localScale );
		links = new RopeLink[numberOfLinks];

		float linkLimitRest = 15.0f / numberOfLinks;

		RopeLink lastLink = null;
		for (int i = 0; i < numberOfLinks; ++i) {
			RopeLink newLink = Instantiate<RopeLink>(ropeLinkPrefab);
			newLink.idn = i+1;
			newLink.transform.gameObject.layer = LayerMask.NameToLayer("Ropes");
			newLink.rope = this;

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

//			hingeJoint.useLimits = true;
//			JointAngleLimits2D limits = new JointAngleLimits2D();
//			limits.min = -10 - linkLimitRest * i;
//			limits.max = 10 + linkLimitRest * i;
//			hingeJoint.limits = limits; 

			lastLink = newLink;
			currentLink = newLink.transform;

			links[i] = newLink;
		}

		//chooseDriver (currentLink);
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown (KeyCode.Z)) {
//			//joint.connectedAnchor
//			//print(driverRigidBody.velocity);
//			//driverRigidBody.AddForce( new Vector2(-500,0) );
//			currentLink.GetComponent<Rigidbody2D>().AddForce( new Vector2(-500,0) );
//		}
//		
//		if (Input.GetKeyDown (KeyCode.X)) {
//			//print(driverRigidBody.velocity);
//			//driverRigidBody.AddForce( new Vector2(500,0) );
//			currentLink.GetComponent<Rigidbody2D>().AddForce( new Vector2(500,0) );
//		}//aaa
//
//		if (Input.GetKeyDown (KeyCode.C)) {
//			if( currentLink.parent ) chooseDriver(currentLink.parent);
//		}
//		
//		if (Input.GetKeyDown (KeyCode.V)) {
//			if( currentLink.childCount > 0 ) chooseDriver(currentLink.GetChild(0));
//		}
		if (links.Length == 0)
			return;

		HingeJoint2D hingeJoint = links [0].GetComponent<HingeJoint2D> ();
		firstLinkSpeed = hingeJoint.jointSpeed;
		firstLinkAngle = hingeJoint.jointAngle;

		if (firstLinkMaxSpeed < Mathf.Abs (firstLinkSpeed)) {
			firstLinkMaxSpeed = Mathf.Abs (firstLinkSpeed);
		}

		if (swingMotor) {

			swing( swingMotorDir, swingMotorForce * Time.deltaTime );

			swingMotorDuration += Time.deltaTime;
			if( swingMotorDuration >= swingMotorTime) 
				swingMotor = false;
		}

//		//string s = new string("");
//		for (int i = 0; i < links.Length; ++i) {
//			//HingeJoint2D hingeJoint = links [i].GetComponent<HingeJoint2D> ();
//		
//			//s += ( i + " : " + hingeJoint.jointSpeed + " " + hingeJoint.jointAngle );
//			//print ( i + " : " + hingeJoint.jointSpeed + " " + hingeJoint.jointAngle );
//
//			firstLinkSpeed = hingeJoint.jointSpeed;
//		}

		//print (s);
	}

	public void swing (Vector2 dir, float force){
		//if (dir == Vector2.right) {
		//}
		//else if( dir == 

		Vector2 dirForce = dir * force;
		currentLink.GetComponent<Rigidbody2D>().AddForce( dirForce );
		if( currentLink.transform.childCount > 0 ) {
			Transform nextLink = currentLink.transform.GetChild(0);
			nextLink.GetComponent<Rigidbody2D>().AddForce( dirForce );
		}
	}

	bool swingMotor = false;
	Vector2 swingMotorDir;
	float swingMotorForce;
	float swingMotorTime;
	float swingMotorDuration;

	public void setSwingMotor(Vector2 dir, float force, float time){
		swingMotor = true;
		swingMotorDir = dir;
		swingMotorForce = force;
		swingMotorTime = time;
		swingMotorDuration = 0f;
	}

	void FixedUpdate(){
		
	}

	public void chooseDriver(Transform newLink){
		if (currentLink) {
			currentLink.GetComponent<SpriteRenderer> ().color = Color.white;
			currentLink.GetComponent<Rigidbody2D> ().mass = 1.0f;
			//currentLink.GetComponent<Rigidbody2D> ().gravityScale = 1.0f;

			if( currentLink.transform.childCount > 0 ) {
				Transform nextLink = currentLink.transform.GetChild(0);
				nextLink.GetComponent<SpriteRenderer> ().color = Color.white;
				nextLink.GetComponent<Rigidbody2D> ().mass = 1.0f;
				//nextLink.GetComponent<Rigidbody2D> ().gravityScale = 1.0f;
			}
		}

		currentLink = newLink;

		newLink.GetComponent<SpriteRenderer> ().color = Color.red;
		newLink.GetComponent<Rigidbody2D> ().mass = 9.0f;
		//newLink.GetComponent<Rigidbody2D> ().gravityScale = 1.5f;

		if( currentLink.transform.childCount > 0 ) {
			Transform nextLink = currentLink.transform.GetChild(0);
			nextLink.GetComponent<SpriteRenderer> ().color = Color.red;
			nextLink.GetComponent<Rigidbody2D> ().mass = 5.0f;
			//nextLink.GetComponent<Rigidbody2D> ().gravityScale = 1.25f;
		}
	}

	public void resetDiver(){
		if (currentLink) {
			currentLink.GetComponent<SpriteRenderer> ().color = Color.white;
			currentLink.GetComponent<Rigidbody2D> ().mass = 1.0f;
			
			if( currentLink.transform.childCount > 0 ) {
				Transform nextLink = currentLink.transform.GetChild(0);
				nextLink.GetComponent<SpriteRenderer> ().color = Color.white;
				nextLink.GetComponent<Rigidbody2D> ().mass = 1.0f;
			}
		}
		
		currentLink = null;
	}
}
