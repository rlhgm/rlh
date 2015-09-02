using UnityEngine;
using System.Collections;

public class Crocodile : MonoBehaviour {

	BoxCollider2D coll;
	Animator animator;
	public Water water;
	public Player2Controller player;
	Vector2 mySize;

	//Vector3 swingStartPos;
	Vector3 swingFinalPos;
	Vector3 distToSwing;
	float swingTime;
	float swingDuration;
	float swingRatio;
	Vector2 swingTargetLimits;
	//bool swingToTarget;

	public float CalmSpeed = 1.75f; // jednostek na sek.
	public float SneakSpeed = 2.25f; // jednostek na sek.
	public float AttackSpeed = 3.75f; // jednostek na sek.

	public Vector3 T1 = new Vector3();
	public Vector3 T2 = new Vector3();

	void Awake(){
		coll = GetComponent<BoxCollider2D> ();
		mySize = new Vector2 (coll.size.x * transform.localScale.x, coll.size.y * transform.localScale.y);
		animator = GetComponent<Animator> ();

		//animator.ResetTrigger ();
		//animator.speed = 0f;
	}

	// Use this for initialization
	void Start () {

		//print (water);
		if (water) {
			//print (water.getSize ());
		}

		//print (player);
		if (player) {
			//print (player.transform.position);
		}

		swingTargetLimits.x = water.getWidth () - mySize.x;
		swingTargetLimits.y = water.getDepth () - mySize.y;

		Vector3 startPos = new Vector3 ();
		startPos.x = water.transform.position.x + water.getWidth () * 0.5f;
		startPos.y = water.transform.position.y - water.getDepth () * 0.5f;

		transform.position = startPos;

		//swingStartPos = transform.position;

		CalmSpeed = 0.75f; // jednostek na sek.
		SneakSpeed = 2.5f; // jednostek na sek.
		AttackSpeed = 6.5f; // jednostek na sek.

		//leftLimit = water.transform.TransformPoint ( new Vector3(0.3f,-0.05f,0f) );
		rightLimit = water.transform.TransformPoint ( new Vector3(0.7f,-0.05f,0f) );

		waterLeftLimit = water.transform.TransformPoint ( new Vector3(0f,0f,0f) );
		waterRightLimit = water.transform.TransformPoint ( new Vector3(1f,0f,0f) );
		
		transform.position = rightLimit;
	}

//	public enum State{
//		CALM,
//		SNEAK,
//		ATTACK,
//		WAIT
//	};

	void turnLeft(){
		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x) * 1.0f;
		transform.localScale = scl;
		
	}
	void turnRight(){
		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x) * -1.0f;
		transform.localScale = scl;
	}

	// Update is called once per frame
	void Update () {

		Vector3 pos = transform.position;

		int wit = whereIsTarget ();

		Vector3 desiredPos = transform.position;

		switch (wit){
		case -1:
			pos.x = waterLeftLimit.x+mySize.x*0.5f;
			break;
		case 0:
			pos.x = player.transform.position.x;
			if( pos.x < waterLeftLimit.x+mySize.x*0.5f ) pos.x = waterLeftLimit.x+mySize.x*0.5f;
			if( pos.x > waterRightLimit.x-mySize.x*0.5f ) pos.x = waterRightLimit.x-mySize.x*0.5f;
			break;
		case 1:
			pos.x = waterRightLimit.x-mySize.x*0.5f;
			break;
		}

		fromLastFlipTime += Time.deltaTime;

		desiredPos.x = pos.x;
		Vector3 distToSwim = desiredPos - transform.position;
		float dtsm = Mathf.Abs( distToSwim.magnitude );
//		if (dtsm > 2.0f) {
//			animator.speed = 1.0f;
//		} else {
//			animator.speed = dtsm/2.0f;
//		}

		if (dtsm > 1.0f) {

			///if (dtsm > 2.0f) {
			//	animator.speed = 1.0f;
			//} else {
			//	animator.speed = dtsm/2.0f;
			//}

			Vector3 dts = distToSwim.normalized * (AttackSpeed * Time.deltaTime);

			transform.position = transform.position + dts;

			if (fromLastFlipTime > 0.5f) {
				if (lastPos.x > transform.position.x) {
					turnLeft ();
				} else {
					turnRight ();
				}
				fromLastFlipTime = 0.0f;
			}
		} else {

			//animator.speed = 0.0f;

		}

		//lastPlayerPos = player.transform.position;
		lastPos = transform.position;
	}

	Vector3 lastPos = new Vector3 ();
	//Vector3 lastPlayerPos = new Vector3 ();

	//Vector3 leftLimit = new Vector3();
	Vector3 rightLimit = new Vector3();
	//bool calmGoToRight = false;
	float fromLastFlipTime = 0.0f;

	public Vector3 waterLeftLimit = new Vector3();
	public Vector3 waterRightLimit = new Vector3();

	int whereIsTarget(){
		Vector3 playerPos = player.transform.position;
		if (waterLeftLimit.x > playerPos.x)
			return -1;
		if (waterRightLimit.x < playerPos.x)
			return 1;
		return 0;
	}

	//bool attacking = false;

	public void attackStart(){
		animator.Play ("croc_zap_attack");
		animator.speed = 1.0f;
	}

	void attackStop(){
	
	}
	//public State state;
}
