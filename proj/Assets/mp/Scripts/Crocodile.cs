using UnityEngine;
using System.Collections;

public class Crocodile : MonoBehaviour {

	BoxCollider2D coll;
	Animator animator;
	public Water water;
	public Player2Controller player;
	Vector2 mySize;

	Vector3 swingStartPos;
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
		animator.speed = 0f;
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

		//print (mySize);
		//print(coll.size + " " + transform.localScale);

		swingTargetLimits.x = water.getWidth () - mySize.x;
		swingTargetLimits.y = water.getDepth () - mySize.y;

		//state = State.CALM;

		Vector3 startPos = new Vector3 ();
		startPos.x = water.transform.position.x + water.getWidth () * 0.5f;
		startPos.y = water.transform.position.y - water.getDepth () * 0.5f;

		//startPos = water.transform.TransformPoint(new Vector3(0,-1,0));

		transform.position = startPos;

		swingStartPos = transform.position;

		//swingToTarget = false;

		CalmSpeed = 0.75f; // jednostek na sek.
		SneakSpeed = 2.5f; // jednostek na sek.
		AttackSpeed = 6.5f; // jednostek na sek.

//		print ("==============================================");
//		print (water.transform.position);
//		print (water.getSize());
//		print (water.transform.TransformVector(new Vector3(0,0,0)) );
//		print (water.transform.TransformVector(new Vector3(1,0,0)) );
//		print (water.transform.TransformVector(new Vector3(0,1,0)) );
//		print (water.transform.TransformVector(new Vector3(1,1,0)) );
//		print (water.transform.TransformPoint(new Vector3(0,0,0)) );
//		print (water.transform.TransformPoint(new Vector3(1,0,0)) );
//		print (water.transform.TransformPoint(new Vector3(0,1,0)) );
//		print (water.transform.TransformPoint(new Vector3(1,1,0)) );
//		print ("==============================================");

		leftLimit = water.transform.TransformPoint ( new Vector3(0.3f,-0.25f,0f) );
		rightLimit = water.transform.TransformPoint ( new Vector3(0.7f,-0.25f,0f) );

		waterLeftLimit = water.transform.TransformPoint ( new Vector3(0f,0f,0f) );
		waterRightLimit = water.transform.TransformPoint ( new Vector3(1f,0f,0f) );

		//swingFinalPos.x = Random.Range (0.1f, 0.7f);
		//if( 
		//swingFinalPos.y = Random.Range (-0.4f,-0.01f);
		//swingFinalPos.y = -0.25f;
		//swingFinalPos = water.transform.TransformPoint (swingFinalPos);
		//distToSwing = swingFinalPos - swingStartPos;

		transform.position = rightLimit;
		calmGoToRight = true;

		setCalmSwingTarget ();
	}

	public enum State{
		CALM,
		SNEAK,
		ATTACK,
		WAIT
	};

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



		//if( coll.IsTouching( player.coll ) ){
		Vector3 playerBauch2 = player.transform.position + new Vector3(0.0f,1.0f,0.0f);

		if( coll.OverlapPoint(playerBauch2) ){
			player.die ();
			state = State.CALM;
		}

		switch( state ){

		case State.WAIT:
			if( !targetAboveWater() ){
				state = State.CALM;
				setCalmSwingTarget();
				break;
			}
			if( player.transform.position != lastPlayerPos ){
				state = State.SNEAK;
				setSneakSwingTarget();
			}
			lastPlayerPos = player.transform.position;

			break;

		case State.SNEAK:
			if( !targetAboveWater() ){
				state = State.CALM;
				setCalmSwingTarget();
				break;
			}

			if( player.transform.position != lastPlayerPos ){
				setSneakSwingTarget();
			}

			lastPlayerPos = player.transform.position;

			swingTime += Time.deltaTime;
			swingRatio = swingTime / swingDuration;
			
			float _t = swingRatio * Mathf.PI;
			//_t -= (Mathf.PI * 0.5f);
			//_t = Mathf.Sin( _t );
			//_t += 1.0f;
			//swingRatio = (_t * 0.5f);
			transform.position = swingStartPos + distToSwing * swingRatio;

			if( Mathf.Abs( transform.position.x - lastPlayerPos.x ) < 1f ){
				state = State.WAIT;
			}

			break;

		case State.ATTACK:
//			Vector3 playerBauch = new Vector3(0.0f,1.0f,0.0f);
//			distToSwing = (player.transform.position+playerBauch) - transform.position;
//			distToMove = distToSwing.normalized * AttackSpeed * Time.deltaTime;
//			if( distToMove.magnitude < distToSwing.magnitude ){
//				transform.position = transform.position + distToMove;
//			}else{
//				transform.position = transform.position + distToSwing;
//				//player.die();
//			}
			break;

		case State.CALM:
			//if( targetInWater() ){
			//	state = State.ATTACK;
			//	break;
			//}
			//if( targetOnShore() != 0.0f ){
			//	state = State.SNEAK;
			//	break;
			//}

			if( targetAboveWater() ){
				state = State.SNEAK;
				lastPlayerPos = player.transform.position;
				setSneakSwingTarget();
				break;
			}

			swingTime += Time.deltaTime;
			swingRatio = swingTime / swingDuration;

			float _t2 = swingRatio * Mathf.PI;
			_t2 -= (Mathf.PI * 0.5f);
			_t2 = Mathf.Sin( _t2 );
			_t2 += 1.0f;
			swingRatio = (_t2 * 0.5f);
			transform.position = swingStartPos + distToSwing * swingRatio;
			if( swingTime >= swingDuration ){
				setCalmSwingTarget();
			}
			break;
		}

		if ((fromLastFlipTime += Time.deltaTime) > 0.5f) {
			if (lastPos.x > transform.position.x) {
				turnLeft ();
			} else {
				turnRight ();
			}
			fromLastFlipTime = 0.0f;
		}
		lastPos = transform.position;
	}

	Vector3 lastPos = new Vector3 ();
	Vector3 lastPlayerPos = new Vector3 ();

	Vector3 leftLimit = new Vector3();
	Vector3 rightLimit = new Vector3();
	bool calmGoToRight = false;
	float fromLastFlipTime = 0.0f;

	public Vector3 waterLeftLimit = new Vector3();
	public Vector3 waterRightLimit = new Vector3();

	void setCalmSwingTarget(){
		state = State.CALM;

		swingStartPos = transform.position;

		//swingFinalPos.x = Random.Range (0.0f, swingTargetLimits.x);
		//swingFinalPos.y = Random.Range (-swingTargetLimits.y, 0.0f);

		//swingFinalPos.x = Random.Range (0.1f, 0.7f);
		//if( 
		//swingFinalPos.y = Random.Range (-0.4f,-0.01f);
		//swingFinalPos.y = -0.25f;

		if (calmGoToRight) { //jezeli szedl w prawo
			swingFinalPos = leftLimit;
			calmGoToRight = false;

		} else {
			swingFinalPos = rightLimit;
			calmGoToRight = true;
		}

		//swingFinalPos = water.transform.TransformPoint (swingFinalPos);

		distToSwing = swingFinalPos - swingStartPos;

		swingTime = 0.0f;
		swingDuration = distToSwing.magnitude / CalmSpeed;

//		Vector3 scl = transform.localScale;
//		if (distToSwing.x > 0.0f) {
//			scl.x = Mathf.Abs(scl.x) * -1.0f;
//		} else {
//			scl.x = Mathf.Abs(scl.x);
//		}
//		transform.localScale = scl;

		//swingToTarget = true;
	}

	void setSneakSwingTarget(){

		float ptpx = player.transform.position.x;

		swingStartPos = transform.position;

		swingFinalPos = leftLimit;
		swingFinalPos.x = ptpx;

		if( swingFinalPos.x <  leftLimit.x)
			swingFinalPos.x = leftLimit.x;
		else if( swingFinalPos.x > rightLimit.x )
			swingFinalPos.x = rightLimit.x;

		distToSwing = swingFinalPos - swingStartPos;
		
		swingTime = 0.0f;
		swingDuration = distToSwing.magnitude / SneakSpeed;
	}

	bool targetInWater(){
		T1 = water.transform.InverseTransformPoint (player.transform.position);
		//T2 = water.transform.InverseTransformVector (player.transform.position);

		return T1.x > 0.0f && T1.x < 1.0f && T1.y >= -1.0f && T1.y < 0.0f;
	}

	bool targetAboveWater(){
		Vector3 playerPos = player.transform.position;

		//if (playerPos.y <= waterLeftLimit.y)
		//	return false;

		return waterLeftLimit.x < playerPos.x && waterRightLimit.x > playerPos.x;
	}

	float targetOnShore(){
		T2 = water.transform.InverseTransformPoint (player.transform.position);

		if (T2.y < 0.0f)
			return 0.0f;

		if( Mathf.Abs(T2.y) > 0.1f ) return 0.0f;

		if (T2.x < -0.175f || T2.x > 1.175f)
			return 0.0f;

		return T2.x;
	}

	public State state;
}
