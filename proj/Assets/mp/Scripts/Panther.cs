using UnityEngine;
using System.Collections;

public class Panther : MonoBehaviour {

	public Transform markA = null;
	public Transform markB = null;

	public float LongDistance = 4f;
	public float JumpDistance = 2f;
	public float JumpFailure = 0f; // Random.Range(2,5);
	public float RecoveryTime = 2f;
	public float IdleDistance = 6f;
	public float AttackDistance = 1.2f;
	public int LifePoints = 4;

	Animator animator = null;
	float calmMinX = 0f;
	float calmMaxX = 0f;

	void Awake(){
		animator = transform.GetComponent<Animator> ();
	}

	void Start () {
		currentActionTime = 0f;
		currentStateTime = 0f;
		setState (State.CALM);
		setAction (Action.IDLE);
	}
	
	void Update () {
		currentActionTime += Time.deltaTime;

		switch (action) {
		case Action.IDLE_IN:
			IDLE_IN();
			break;
		case Action.IDLE:
			IDLE();
			break;
		case Action.WALK:
			WALK();
			break;
		case Action.RUN:
			RUN ();
			break;
		case Action.WALK_TURNBACK:
			WALK_TURNBACK();
			break;
		case Action.ROAR_IN:
			ROAR_IN();
			break;
		case Action.ROAR:
			ROAR();
			break;
		case Action.ATTACK_JUMP:
			ATTACK_JUMP();
			break;
		case Action.ATTACK_LANDING_TURNBACK:
			ATTACK_LANDING_TURNBACK();
			break;
		case Action.ATTACK_LANDING_FAILURE:
			ATTACK_LANDING_FAILURE();
			break;
		case Action.HIT:
			HIT();
			break;
		case Action.RECOVERY:
			RECOVERY();
			break;
		}
	}

	public enum State{
		UNDEF = 0,
		CALM,
		FIGHT
	}

	public enum Action{
		UNDEF = 0,
		IDLE_IN,
		IDLE,
		WALK,
		RUN,
		WALK_TURNBACK,
		ROAR_IN,
		ROAR,
		ATTACK_JUMP,
		ATTACK_LANDING_TURNBACK,
		ATTACK_LANDING_FAILURE,
		HIT,
		RECOVERY
	};

	public State state;
	float currentStateTime;
	public Action action;
	float currentActionTime;

	bool setAction(Action newAction, int param = 0){
		if (action == newAction)
			return false;
		
		action = newAction;
		currentActionTime = 0f;
		animator.speed = 1f;
		
		switch (newAction) {
		case Action.IDLE_IN:
			animator.Play ("idle_in");
			break;
		case Action.IDLE:
			animator.Play ("idle");
			break;
		case Action.WALK:
			animator.Play ("walk");
			break;
		case Action.RUN:
			animator.Play ("run");
			break;
		case Action.WALK_TURNBACK:
			animator.Play ("walk_turnback");
			break;
		case Action.ROAR_IN:
			animator.Play ("roar_in");
			break;
		case Action.ROAR:
			animator.Play ("roar");
			break;
		case Action.ATTACK_JUMP:
			animator.Play ("attack_jump");
			break;
		case Action.ATTACK_LANDING_TURNBACK:
			animator.Play ("attack_landing_turnback");
			break;
		case Action.ATTACK_LANDING_FAILURE:
			animator.Play ("attack_landing_failure");
			break;
		case Action.HIT:
			animator.Play ("hit");
			break;
		case Action.RECOVERY:
			animator.Play ("recovery");
			break;
		}
		return true;
	}
	bool isInAction(Action test) {
		return action == test;
	}
	bool isNotInAction(Action test){
		return action != test;
	}

	bool setState(State newState){
		if (state == newState)
			return false;
		
		state = newState;
		currentStateTime = 0f;

		switch (state) {
		case State.CALM:
			break;
		case State.FIGHT:
			break;
		}
		return true;
	}
	bool isInState(State test) {
		return state == test;
	}
	bool isNotInAction(State test){
		return state != test;
	}

	int dir(){
		return transform.localScale.x > 0f ? -1 : 1;
	}
	
//	void turnLeft(){
//		if (state != State.ACTIVE)
//			return;
//		animator.SetTrigger("turn_right");
//		Vector3 scl = transform.localScale;
//		scl.x = Mathf.Abs(scl.x) * 1.0f;
//		transform.localScale = scl;
//		
//		turnTime = 0f;
//		state = State.TURN;
//	}
//	void turnRight(){
//		if (state != State.ACTIVE)
//			return;
//		animator.SetTrigger("turn_left");
//		Vector3 scl = transform.localScale;
//		scl.x = Mathf.Abs(scl.x) * -1.0f;
//		transform.localScale = scl;
//		
//		turnTime = 0f;
//		state = State.TURN;
//	}

	void IDLE_IN(){
	}
	void IDLE(){
	}
	void WALK(){
	}
	void RUN(){
	}
	void WALK_TURNBACK(){
	}
	void ROAR_IN(){
	}
	void ROAR(){
	}
	void ATTACK_JUMP(){
	}
	void ATTACK_LANDING_TURNBACK(){
	}
	void ATTACK_LANDING_FAILURE(){
	}
	void HIT(){
	}
	void RECOVERY(){
	}
}
