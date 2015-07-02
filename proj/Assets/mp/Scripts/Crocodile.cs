using UnityEngine;
using System.Collections;

public class Crocodile : MonoBehaviour {

	BoxCollider2D coll;
	public Water water;
	public Player2Controller player;
	Vector2 mySize;

	Vector3 swingStartPos;
	Vector3 swingFinalPos;
	Vector3 distToSwing;
	float swingTime;
	float swingDuration;
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
	}

	// Use this for initialization
	void Start () {

		print (water);
		if( water )
			print (water.getSize ());

		print (player);
		if (player)
			print (player.transform.position);

		print (mySize);
		print(coll.size + " " + transform.localScale);

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

		setCalmSwingTarget ();
	}

	public enum State{
		CALM,
		SNEAK,
		ATTACK
	};

	// Update is called once per frame
	void Update () {

		//if( coll.IsTouching( player.coll ) ){
		Vector3 playerBauch2 = player.transform.position + new Vector3(0.0f,1.0f,0.0f);

		if( coll.OverlapPoint(playerBauch2) ){
			player.die ();
			state = State.CALM;
		}

		switch( state ){

		case State.SNEAK:
			if( targetInWater() ){
				state = State.ATTACK;
				break;
			}

			float tos = targetOnShore();
			if( tos == 0.0f ){
				setCalmSwingTarget();
				break;
			}

			Vector3 attackStartPos = new Vector3();
			if( tos < 0.0f ){
				attackStartPos = water.transform.TransformPoint(new Vector3(0,0,0));
			}else{
				attackStartPos = water.transform.TransformPoint(new Vector3(0.8f,0,0));
			}

			distToSwing = attackStartPos - transform.position;
			Vector3 distToMove =  distToSwing.normalized * SneakSpeed * Time.deltaTime;
			if( distToMove.magnitude < distToSwing.magnitude ){
				transform.position = transform.position + distToMove;
			}else{
				state = State.ATTACK;
			}

			break;

		case State.ATTACK:
			Vector3 playerBauch = new Vector3(0.0f,1.0f,0.0f);
			distToSwing = (player.transform.position+playerBauch) - transform.position;
			distToMove = distToSwing.normalized * AttackSpeed * Time.deltaTime;
			if( distToMove.magnitude < distToSwing.magnitude ){
				transform.position = transform.position + distToMove;
			}else{
				transform.position = transform.position + distToSwing;
				//player.die();
			}
			break;

		case State.CALM:
			if( targetInWater() ){
				state = State.ATTACK;
				break;
			}
			if( targetOnShore() != 0.0f ){
				state = State.SNEAK;
				break;
			}

			swingTime += Time.deltaTime;
			float swingRatio = swingTime / swingDuration;

			float _t = swingRatio * Mathf.PI;
			_t -= (Mathf.PI * 0.5f);
			_t = Mathf.Sin( _t );
			_t += 1.0f;
			swingRatio = (_t * 0.5f);
			transform.position = swingStartPos + distToSwing * swingRatio;
			if( swingTime >= swingDuration ){
				setCalmSwingTarget();
			}
			break;
		}

	}

	void setCalmSwingTarget(){
		state = State.CALM;

		swingStartPos = transform.position;

		//swingFinalPos.x = Random.Range (0.0f, swingTargetLimits.x);
		//swingFinalPos.y = Random.Range (-swingTargetLimits.y, 0.0f);

		swingFinalPos.x = Random.Range (0.1f, 0.7f);
		swingFinalPos.y = Random.Range (-0.4f,-0.01f);

		swingFinalPos = water.transform.TransformPoint (swingFinalPos);

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

	bool targetInWater(){
		T1 = water.transform.InverseTransformPoint (player.transform.position);
		//T2 = water.transform.InverseTransformVector (player.transform.position);

		return T1.x > 0.0f && T1.x < 1.0f && T1.y >= -1.0f && T1.y < 0.0f;
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
