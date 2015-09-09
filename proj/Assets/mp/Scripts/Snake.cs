using UnityEngine;
using System.Collections;

public class Snake : MonoBehaviour {

	public float getUpDistance = 4.0f;
	public float getDownDistance = 5.0f;
	public State state;

	Animator animator;
	GameObject target;
	Transform attackPoint;
	int layerIdPlayerMask;

	public enum State{
		ACTIVE = 1,
		SLEEP = 2
	};

	void Awake(){
		animator = transform.GetComponent<Animator>();
		attackPoint = transform.Find ("attackPoint").transform;
		state = State.ACTIVE;
	}

	// Use this for initialization
	void Start () {
		layerIdPlayerMask = 1 << LayerMask.NameToLayer("Player");

		if (target == null) {
			GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
			if( targets.Length == 1 ){
				target = targets[0];
				//print ( this + " jest target");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		float distToTarget = (target.transform.position - transform.position).magnitude;

		if (state == State.ACTIVE) {

			if( distToTarget > getDownDistance ){
				getDown();
			}

		} else if (state == State.SLEEP) {

			if( distToTarget < getUpDistance ){
				getUp();
			}
		}

		if( target ){
			if( target.transform.position.x < transform.position.x ){

				if( dir() > 0 )
					turnLeft();

			}else{

				if( dir () < 0 )
					turnRight();

			}
		}

		if (bitting) {
			if( (biteTime+=Time.deltaTime) > 0.35f ){
				bite ();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			Player2Controller playerController = target.GetComponent<Player2Controller> ();
			if( !playerController.isDead() ){
				biteStart();
			}
		}
	}
	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			if( !bitting ){
				Player2Controller playerController = target.GetComponent<Player2Controller> ();
				if( !playerController.isDead() ){
					if( (fromLastBite += Time.deltaTime) > toNextBite )
						biteStart();
					else if( lastBiteTargetPos != target.transform.position)
						biteStart();
				}
			}
		}
	}

	bool bitting = false;
	float biteTime = 0f;
	float fromLastBite = 0f;
	float toNextBite = 1.5f;
	Vector3 lastBiteTargetPos = new Vector3();

	void biteStart(){
		animator.SetTrigger("attack");
		biteTime = 0f;
		bitting = true;
		fromLastBite = 0f;
		toNextBite = Random.Range (3f, 5f);
	}

	void bite(){
		lastBiteTargetPos = target.transform.position;
		biteTime = 0f;
		bitting = false;

		Vector3 attackDir = attackPoint.position - transform.position;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, attackDir, attackDir.magnitude, layerIdPlayerMask);
		if (hit.collider != null) {
			Player2Controller playerController = target.GetComponent<Player2Controller> ();
			playerController.die (Player2Controller.DeathType.SNAKE);
		}
		//fromLastBite = 0f;
	}

	int dir(){
		return transform.localScale.x > 0f ? -1 : 1;
	}

	void turnLeft(){
		animator.SetTrigger("turn_right");
		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x) * 1.0f;
		transform.localScale = scl;
	}
	void turnRight(){
		animator.SetTrigger("turn_left");
		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x) * -1.0f;
		transform.localScale = scl;
	}


	void getDown(){
		animator.SetTrigger("gets_down");
		state = State.SLEEP;
	}

	void getUp(){
		animator.SetTrigger("gets_up");
		state = State.ACTIVE;
	}
}
