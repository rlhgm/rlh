using UnityEngine;
using System.Collections;

public class Snake : MonoBehaviour {

	Animator animator;
	GameObject target;
	Transform attackPoint;
	int layerIdPlayerMask;

	void Awake(){
		animator = transform.GetComponent<Animator>();
		attackPoint = transform.Find ("attackPoint").transform;
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
		if( target ){
			if( target.transform.position.x < transform.position.x ){
				turnLeft();
			}else{
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
			playerController.die (666);
		}
		//fromLastBite = 0f;
	}

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

}
