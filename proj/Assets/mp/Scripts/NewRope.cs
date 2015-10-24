using UnityEngine;
using System.Collections;

public class NewRope : MonoBehaviour {

	public RopeLink ropeLinkPrefab;
	//Rigidbody2D driverRigidBody;
	public Transform currentLink;
	public Rigidbody2D attachedStone;

	RopeLink[] links;

	public float firstLinkSpeed;
	public float firstLinkMaxSpeed;
	public float firstLinkAngle;

	public int weakLinkIndex = -1;
	public float weakLinkBreakUpDuration = 2f;
	//[SerializeField]
	float weakLinkTimeToBreakUp = 2f;
	public bool alwaysBreakOff = false;

	public int cutedLinkIndex = -1;

	void Awake(){
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();
		Destroy (spriteRenderer);
		int numberOfLinks = (int)Mathf.Floor(transform.localScale.y) * 2;
		transform.localScale = new Vector3 (1, 1, 1);
		links = new RopeLink[numberOfLinks];

        //float linkLimitRest = 15.0f / numberOfLinks;

        //if (transform.childCount == 1) {
        //	attachedStone = transform.GetChild(0);
        //}

        if (attachedStone)
        {
            asp = attachedStone.position;
            asr = attachedStone.rotation;
        }
		createLinks ();
	}

	void createLinks(){
		cutedLinkIndex = -1;
		int numberOfLinks = links.Length;
		RopeLink lastLink = null;
		for (int i = 0; i < numberOfLinks; ++i) {
			RopeLink newLink = Instantiate<RopeLink>(ropeLinkPrefab);
			newLink.idn = i+1;
			newLink.transform.gameObject.layer = LayerMask.NameToLayer("Ropes");
			newLink.rope = this;
			newLink.GetComponent<Rigidbody2D>().mass = linkMass;

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
			
			links[i] = newLink;
		}

		if (weakLinkIndex >= links.Length)
			weakLinkIndex = -1;
		
		weakLinkTimeToBreakUp = weakLinkBreakUpDuration;

		if (attachedStone) {
            //			RopeLink specialLink = Instantiate<RopeLink>(ropeLinkPrefab);
            //			specialLink.transform.gameObject.layer = LayerMask.NameToLayer("Ropes");
            //			specialLink.rope = this;
            //			specialLink.GetComponent<Rigidbody2D>().mass = linkMass;
            //
            //
            //			Rigidbody2D lastRigidBody = lastLink.GetComponent<Rigidbody2D>();
            //			hingeJoint.connectedBody = attachedStone.GetComponent<Rigidbody2D>();
            //			newLink.transform.SetParent( lastLink.transform );
            //			hingeJoint.connectedAnchor = new Vector2(0f,-0.55f);
            //			newLink.transform.position = lastLink.transform.position + new Vector3(0f,-0.5f);
            //
            //			attachedStone.SetParent( links[numberOfLinks-1].transform );
            //			attachedStone.position = new Vector3(0f,0f,0f);

            //DistanceJoint2D distJoint = Instantiate<DistanceJoint2D>();
            //distJoint.distance = 0.5f;
            //distJoint.enableCollision = false;
            //distJoint.

            //asp = attachedStone.position;
            
			DistanceJoint2D distJoint = lastLink.gameObject.AddComponent<DistanceJoint2D>();
			distJoint.distance = 0.1f;
			distJoint.enableCollision = false;
			distJoint.anchor = new Vector2(0.0f,-0.5f);
			distJoint.connectedAnchor = new Vector2(0f,0f);
			distJoint.connectedBody = attachedStone;
            //print(attachedStone.transform.parent);
			attachedStone.transform.SetParent( lastLink.transform );
			//attachedStone.transform.position = lastLink.transform.position + new Vector3(0f,-0.6f);
			attachedStone.MovePosition( asp );

			//Vector3 locScale = attachedStone.localScale;

			//attachedStone.localScale = locScale;
		}
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

		if (links [0] == null)
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

	public bool breakUpStep(int linkIndex, float deltaTime){
		if (weakLinkIndex < 0)
			return false;
		if (linkIndex < weakLinkIndex)
			return false;

		weakLinkTimeToBreakUp -= deltaTime;

		RopeLink weakLink = links [weakLinkIndex];
		SpriteRenderer wlsr = weakLink.GetComponent<SpriteRenderer> ();
		Color c = wlsr.color;
		c.a = Mathf.Max(0f, (weakLinkTimeToBreakUp / weakLinkBreakUpDuration) );
		wlsr.color = c;

		if (weakLinkTimeToBreakUp <= 0f) {
			breakUp();
			return true;
		}

		return false;
	}

	public void breakUp(){
		if (weakLinkIndex < 0 || weakLinkIndex >= links.Length)
			return;

		cut (weakLinkIndex);
	}

	public void cut(int linkIndex){
        if( attachedStone)
        {
            RopeLink lastLink = links[links.Length - 1];
            DistanceJoint2D distJoint = lastLink.gameObject.AddComponent<DistanceJoint2D>();
            distJoint.connectedBody = null;
            distJoint.enabled = false;
            //links[links.Length-1].con distJoint.connectedBody = attachedStone;
            attachedStone.transform.SetParent(null);
            //attachedStone.transform.position = lastLink.transform.position + new Vector3(0f,-0.6f);
            //attachedStone.MovePosition(asp);
            //attachedStone
        }
        cutedLinkIndex = linkIndex;
		RopeLink weakLink = links [linkIndex];
		HingeJoint2D weakHingeJoint = weakLink.GetComponent<HingeJoint2D>();
		weakHingeJoint.enabled = false;
	}

	public void swing (Vector2 dir, float force){
		//if (dir == Vector2.right) {
		//}
		//else if( dir == 

		if (currentLink) {
			Vector2 dirForce = dir * force;
			currentLink.GetComponent<Rigidbody2D> ().AddForce (dirForce);
			if (currentLink.transform.childCount > 0) {
				Transform nextLink = currentLink.transform.GetChild (0);
				nextLink.GetComponent<Rigidbody2D> ().AddForce (dirForce);
			}
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

	Vector3 asp;
    float asr;

	bool fasp = true;

	void FixedUpdate(){
		if (fasp) {
			if( attachedStone ){
				attachedStone.MovePosition( asp );			
			}
			fasp = false;
		}
	}

	float linkMass = 1f;
	float firstDriverLinkMass = 9f;
	float secondDriverLinkMass = 5f;

	public bool chooseDriver(Transform newLink){
		if (cutedLinkIndex>=0 && newLink.GetComponent<RopeLink> ().idn > cutedLinkIndex)
			return false;

		if (currentLink) {
			currentLink.GetComponent<SpriteRenderer> ().color = Color.white;
			currentLink.GetComponent<Rigidbody2D> ().mass = linkMass;
			//currentLink.GetComponent<Rigidbody2D> ().gravityScale = 1.0f;

			if( currentLink.transform.childCount > 0 ) {
				Transform nextLink = currentLink.transform.GetChild(0);
				nextLink.GetComponent<SpriteRenderer> ().color = Color.white;
				nextLink.GetComponent<Rigidbody2D> ().mass = linkMass;
				//nextLink.GetComponent<Rigidbody2D> ().gravityScale = 1.0f;
			}
		}

		currentLink = newLink;

		newLink.GetComponent<SpriteRenderer> ().color = Color.red;
		newLink.GetComponent<Rigidbody2D> ().mass = firstDriverLinkMass;
		//newLink.GetComponent<Rigidbody2D> ().gravityScale = 1.5f;

		if( currentLink.transform.childCount > 0 ) {
			Transform nextLink = currentLink.transform.GetChild(0);
			nextLink.GetComponent<SpriteRenderer> ().color = Color.red;
			nextLink.GetComponent<Rigidbody2D> ().mass = secondDriverLinkMass;
			//nextLink.GetComponent<Rigidbody2D> ().gravityScale = 1.25f;
		}

		return true;
	}

	public void resetDiver(){
		if (currentLink) {
			currentLink.GetComponent<SpriteRenderer> ().color = Color.white;
			currentLink.GetComponent<Rigidbody2D> ().mass = linkMass;
			
			if( currentLink.transform.childCount > 0 ) {
				Transform nextLink = currentLink.transform.GetChild(0);
				nextLink.GetComponent<SpriteRenderer> ().color = Color.white;
				nextLink.GetComponent<Rigidbody2D> ().mass = linkMass;
			}
		}
		
		currentLink = null;
	}

	public void reset(){

        if( attachedStone)
        {
            RopeLink lastLink = links[links.Length - 1];
            DistanceJoint2D distJoint = lastLink.gameObject.AddComponent<DistanceJoint2D>();
            distJoint.connectedBody = null;
            distJoint.enabled = false;
            //links[links.Length-1].con distJoint.connectedBody = attachedStone;
            attachedStone.transform.SetParent(null);
            //attachedStone.transform.position = lastLink.transform.position + new Vector3(0f,-0.6f);
            //attachedStone.MovePosition(asp);
            //attachedStone

            attachedStone.velocity = new Vector2(0f, 0f);
            attachedStone.position = asp;
            attachedStone.rotation = asr;
        }

        for (int i = 0; i < links.Length; ++i) {
			Destroy(links[i].gameObject);
			links[i] = null;
//			HingeJoint2D hingeJoint = links [i].GetComponent<HingeJoint2D> ();
//			if(hingeJoint.connectedBody){
//				hingeJoint.connectedBody.velocity = new Vector2(0f,0f);
//				hingeJoint.connectedBody.angularVelocity = 0f;
//			}
		}
        
        createLinks();
	}

}
