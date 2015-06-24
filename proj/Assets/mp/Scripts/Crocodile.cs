using UnityEngine;
using System.Collections;

public class Crocodile : MonoBehaviour {

	BoxCollider2D coll;
	public Water water;
	public Player2Controller player;
	Vector2 mySize;

	public Vector3 swingStartPos;
	public Vector3 swingFinalPos;
	public Vector3 distToSwing;
	public float swingTime;
	public float swingDuration;
	public Vector2 swingTargetLimits;
	public bool swingToTarget;

	public float CalmSpeed = 0.75f; // jednostek na sek.
	public float HuntSpeed = 1.25f; // jednostek na sek.
	public float AttackSpeed = 3.75f; // jednostek na sek.

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

		state = State.CALM;

		Vector3 startPos = new Vector3 ();
		startPos.x = water.transform.position.x + water.getWidth () * 0.5f;
		startPos.y = water.transform.position.y - water.getDepth () * 0.5f;

		transform.position = startPos;

		swingStartPos = transform.position;

		swingToTarget = false;

		print ("==============================================");
		print (water.transform.position);
		print (water.getSize());
		print (water.transform.TransformVector(new Vector3(0,0,0)) );
		print (water.transform.TransformVector(new Vector3(1,0,0)) );
		print (water.transform.TransformVector(new Vector3(0,1,0)) );
		print (water.transform.TransformVector(new Vector3(1,1,0)) );
		print (water.transform.TransformPoint(new Vector3(0,0,0)) );
		print (water.transform.TransformPoint(new Vector3(1,0,0)) );
		print (water.transform.TransformPoint(new Vector3(0,1,0)) );
		print (water.transform.TransformPoint(new Vector3(1,1,0)) );
		print ("==============================================");

		//setCalmSwingTarget ();
	}

	enum State{
		CALM,
		HUNT,
		ATTACK
	};

	// Update is called once per frame
	void Update () {
		if (swingToTarget) {
			swingTime += Time.deltaTime;
			float swingRatio = swingTime / swingDuration;
			transform.position = swingStartPos + distToSwing * swingRatio;
			if( swingTime >= swingDuration ){
				setCalmSwingTarget();
			}
		}
	}

	void setCalmSwingTarget(){
		swingStartPos = transform.position;

		swingFinalPos.x = Random.Range (0.0f, swingTargetLimits.x);
		swingFinalPos.y = Random.Range (0.0f, swingTargetLimits.y);

		swingFinalPos = water.transform.TransformVector (swingFinalPos);

		distToSwing = swingFinalPos - swingStartPos;

		swingTime = 0.0f;
		swingDuration = distToSwing.magnitude / CalmSpeed;

		swingToTarget = true;
	}

	State state;
}
