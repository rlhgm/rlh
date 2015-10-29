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
    bool permanentlyDead = false;

    public enum State{
		ACTIVE = 1,
		SLEEP = 2,
		TURN = 3,
		GETS_UP = 4,
		GETS_DOWN = 5,
		BITTING = 6,
		DEAD = 7
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

	public void cut(){
        //animator.SetTrigger("gets_down");
        animator.Play("die");
        turnTime = 0f;
		state = State.DEAD;
	}

    public void reset()
    {
        if (!permanentlyDead)
        {
            state = State.SLEEP;
        }
    }

    public void checkPointReached()
    {
        if( state == State.DEAD)
        {
            permanentlyDead = true;
        }
    }

    // Update is called once per frame
    void Update () {

		turnTime+=Time.deltaTime;

		if (state == State.TURN) {
			if( turnTime > 0.33f ){
				state = State.ACTIVE;
			}
			return;
		}

		if (state == State.GETS_DOWN ) {
			if( turnTime > 0.5f ){
				state = State.SLEEP;
			}
			return;
		}
		if (state == State.GETS_UP ) {
			if( turnTime > 0.5f ){
				state = State.ACTIVE;
			}
			return;
		}

		if (state == State.BITTING) {
			if( turnTime > 0.35f ){
				bite ();
			}
		}

		float distToTarget = (target.transform.position - transform.position).magnitude;

		if (state == State.ACTIVE) {

			if( distToTarget > getDownDistance ){
				getDown();
				return;
			}

		} else if (state == State.SLEEP) {

			if( distToTarget < getUpDistance ){
				getUp();
				return;
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
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			Zap playerController = target.GetComponent<Zap> ();
			if( !playerController.isDead() && state == State.ACTIVE){
				biteStart();
			}
		}
	}
	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			if( state == State.ACTIVE){
				Zap playerController = target.GetComponent<Zap> ();
				if( !playerController.isDead() ){
					if( (fromLastBite += Time.deltaTime) > toNextBite )
						biteStart();
					else if( lastBiteTargetPos != target.transform.position)
						biteStart();
				}
			}
		}
	}

	//bool bitting = false;
	//float biteTime = 0f;
	float fromLastBite = 0f;
	float toNextBite = 1.5f;
	Vector3 lastBiteTargetPos = new Vector3();

	void biteStart(){
		animator.SetTrigger("attack");
		//biteTime = 0f;
		//bitting = true;
		fromLastBite = 0f;
		toNextBite = Random.Range (3f, 5f);

		state = State.BITTING;
		turnTime = 0f;
	}

	void bite(){
		lastBiteTargetPos = target.transform.position;
		//biteTime = 0f;
		//bitting = false;
		turnTime = 0f;
		state = State.ACTIVE;

		Vector3 attackDir = attackPoint.position - transform.position;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, attackDir, attackDir.magnitude, layerIdPlayerMask);
		if (hit.collider != null) {
			Zap playerController = target.GetComponent<Zap> ();
			playerController.die (Zap.DeathType.SNAKE);
		}
		//fromLastBite = 0f;
	}

	int dir(){
		return transform.localScale.x > 0f ? -1 : 1;
	}

	float turnTime = 0f;

	void turnLeft(){
		if (state != State.ACTIVE)
			return;
		animator.SetTrigger("turn_right");
		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x) * 1.0f;
		transform.localScale = scl;

		turnTime = 0f;
		state = State.TURN;
	}
	void turnRight(){
		if (state != State.ACTIVE)
			return;
		animator.SetTrigger("turn_left");
		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x) * -1.0f;
		transform.localScale = scl;

		turnTime = 0f;
		state = State.TURN;
	}


	void getDown(){
		animator.SetTrigger("gets_down");
		turnTime = 0f;
		state = State.GETS_DOWN;
	}

	void getUp(){
		animator.SetTrigger("gets_up");
		turnTime = 0f;
		state = State.GETS_UP;
	}
}
