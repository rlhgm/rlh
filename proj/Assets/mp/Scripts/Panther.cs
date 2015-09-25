using UnityEngine;
using System.Collections;

public class Panther : MonoBehaviour {

	public Transform markA = null;
	public Transform markB = null;

	public float LongDistance = 4f;
	public float JumpDistance = 3f;
	public int JumpFailurePercent = 100; // Random.Range(2,5);
	public float RecoveryTime = 2f;
	public float IdleDistance = 6f;
	public float AttackDistance = 1.2f;
	public float FightDistance = 8f; // ???
	public int LifePoints = 4;

	public float walkSpeed = 2f;
	public float walkFightSpeed = 1.5f;
	public float runSpeed = 6;
	public float jumpInSpeed = 2;
	public float jumpSpeed = 7;

	public float flyDistance = 3;
	public float flyHight = 1.0f;

	public float walkTurnBackDuration = 0.667f;
	public float attackJumpInDuration = 0.2f;
	public float attackLandingTurnBackDuration = 0.7f;
	public float attackLandingFailureDuration = 0.7f;
	public float attackDuration = 0.33f;

	int currentLifePoints = 0;

	Vector3 startPos = new Vector3();
	int startDir = 0;

	void Awake(){
		animator = transform.GetComponent<Animator> ();
	}

	void Start () {
		if (markA && markB) {
			if( markA.position.x < markB.position.x ){
				terrainLimitXMin = markA.position.x;
				terrainLimitXMax = markB.position.x;
			}else{
				terrainLimitXMin = markB.position.x;
				terrainLimitXMax = markA.position.x;
			}
			terrainSize = terrainLimitXMax - terrainLimitXMin;
			terrainMiddleX = terrainLimitXMin + terrainSize*0.5f;
		}

		if (zap == null) {
			GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
			if( targets.Length == 1 ){
				zap = targets[0].GetComponent<Zap>();
				print ( this + " jest target");
			}
		}
		startPos = transform.position;
		myPosY = transform.position.y;
		startDir = dir ();

		currentLifePoints = LifePoints;

		currentActionTime = 0f;
		currentStateTime = 0f;
		setState (State.CALM);
		setAction (Action.IDLE);
	}

	public void reset(){
		currentLifePoints = LifePoints;

		transform.position = startPos;
		myPosY = transform.position.y;
		if (dir () != startDir)
			turn ();
		setState (State.CALM);
		setAction (Action.IDLE);
	}

	void Update () {
		currentActionTime += Time.deltaTime;
		currentStateTime += Time.deltaTime;

		recalcPaD ();

		switch (state) {
		case State.CALM:
			if( distToZap < FightDistance ){
				setState(State.FIGHT);

				// jezeli jest dupa do zapa to sie musi obrocic: i jezeli wlasnie nie zawraca...
				//if( isInAction(Action.WALK) || isInAction(Action.IDLE) || ){
				if( !walkTurningBack() ){
					if( dir () != getToZapDir() ){
						setAction(Action.WALK_TURNBACK);
					}else{
						//setAction(Action.RUN);
						whatNextInFight();
					}
				}
			}
			break;
		case State.FIGHT:
			if( distToZap > FightDistance ){
				setState(State.CALM);
			}

			break;
		}

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
		case Action.WALKFIGHT:
			WALKFIGHT();
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
		case Action.ATTACK:
			ATTACK();
			break;
		case Action.ATTACK_JUMP_IN:
			ATTACK_JUMP_IN();
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
		FIGHT,
		GOTO_EAT_ZAP,
		EAT_ZAP,
		ESCAPE
	}

	public enum Action{
		UNDEF = 0,
		IDLE_IN,
		IDLE,
		WALK,
		RUN,
		WALKFIGHT,
		WALK_TURNBACK,
		ROAR_IN,
		ROAR,
		ATTACK,
		ATTACK_JUMP_IN,
		ATTACK_JUMP,
		ATTACK_LANDING_TURNBACK,
		ATTACK_LANDING_FAILURE,
		HIT,
		EAT,
		RECOVERY
	};

	public State state;
	float currentStateTime;
	public Action action;
	float currentActionTime;

	public bool attacking(){
		return isInAction (Action.ATTACK) || isInAction (Action.ATTACK_JUMP);
	}

	bool setAction(Action newAction, int param = 0){
		if (action == newAction)
			return false;
		
		action = newAction;
		currentActionTime = 0f;

		animator.speed = 1f;
		paint (Color.white);
		
		switch (newAction) {
		case Action.IDLE_IN:
			animator.Play ("idle_in");
			break;
		case Action.IDLE:
			idleDuration = Random.Range( 2f, 5f ); 
			animator.Play ("idle");
			break;
		case Action.WALK:
			animator.Play ("walk");
			break;
		case Action.RUN:
			animator.Play ("run");
			break;
		case Action.WALKFIGHT:
			animator.Play ("walkfight");
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
		case Action.ATTACK:
			animator.Play ("attack");
			break;
		case Action.ATTACK_JUMP_IN:
			animator.Play ("attack_jump_in");
			break;
		case Action.ATTACK_JUMP:
			jumpStartPosX = transform.position.x;
			animator.Play ("attack_jump");
			break;
		case Action.ATTACK_LANDING_TURNBACK:
			paint (Color.red);
			animator.Play ("attack_landing_turnback");
			break;
		case Action.ATTACK_LANDING_FAILURE:
			paint (Color.red);
			animator.Play ("attack_landing_failure");
			break;
		case Action.HIT:
			animator.Play ("hit");
			break;
		case Action.EAT:
			animator.Play ("eat");
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
	bool walkTurningBack(){
		return isInAction (Action.WALK_TURNBACK);
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
		return transform.localScale.x > 0f ? 1 : -1;
	}
	int turn(){
		Vector3 scl = transform.localScale;
		scl.x *= -1f;
		transform.localScale = scl;
		return dir ();
	}

	int inTerrainLimits(float epsilon = 1f){
		float posx = transform.position.x;
		if (posx >= (terrainLimitXMin - epsilon)) { 
			if (posx < (terrainLimitXMax + epsilon)) {
				return 0;
			}else{
				return 1; // jest na prawo od max
			}
		} else {
			return -1; // jest na lewo od min
		}
	}

	float move(float _speed){
		Vector3 pos = transform.position;
		float distToMove = _speed * Time.deltaTime * dir ();
		pos.x += distToMove;
		transform.position = pos;
		recalcPaD ();
		return pos.x;
	}

	void whatNextInFight(){
		if (zap.isDead ()) {
			setState(State.GOTO_EAT_ZAP);
			if( dir () != getToZapDir() ){
				setAction(Action.WALK_TURNBACK);
			}else{
				moveTargetX = zap.transform.position.x;
				//moveTargetX -= zap.dir2();
				//if( dir () == 1 )
				moveTargetX -= dir ();
				nextAction = Action.EAT;
				setAction(Action.WALK);
			}
			return;
		}

		if( dir () != getToZapDir() ){
			setAction(Action.WALK_TURNBACK);
		}else{
			if( distToZap >= LongDistance ){
				setAction(Action.RUN);
			}else if (distToZap > AttackDistance) {
				moveTargetX = zapPosX;
				setAction(Action.WALKFIGHT);
			}else{
				//setAction(Action.ATTACK);
			}
		}
	}
	
	void IDLE_IN(){
	}
	void IDLE(){
		if (currentActionTime > idleDuration) {
			int itl = inTerrainLimits();
			if( itl == 0 ){
				if( dir() == 1 ){
					moveTargetX = terrainLimitXMax;
				}else{
					moveTargetX = terrainLimitXMin;
				}
				nextAction = Action.WALK_TURNBACK;
				setAction(Action.WALK);
			}
		}
	}
	void WALK(){
		float posx = move (walkSpeed);
		if (dir () == 1) {
			if( posx >= moveTargetX ){		
				if( nextAction == Action.EAT ){
					setState(State.EAT_ZAP);
				}
				setAction(nextAction);
				return;
			}
		} else {
			if( posx <= moveTargetX ){
				if( nextAction == Action.EAT ){
					setState(State.EAT_ZAP);
				}
				setAction(nextAction);
				return;
			}
		}
	}
	void RUN(){
		move (runSpeed);

		if (distToZap <= JumpDistance) {
			setAction(Action.ATTACK_JUMP_IN);
		}
	}
	void WALKFIGHT(){
		float posx = move (walkFightSpeed);
		if (dir () == 1) {
			if( posx >= moveTargetX ){		
//				if( nextAction == Action.EAT ){
//					setState(State.EAT_ZAP);
//				}
				//setAction(nextAction);
				whatNextInFight();
				return;
			}
		} else {
			if( posx <= moveTargetX ){
//				if( nextAction == Action.EAT ){
//					setState(State.EAT_ZAP);
//				}
//				setAction(nextAction);
				whatNextInFight();
				return;
			}
		}
	}
	void WALK_TURNBACK(){
		if (currentActionTime > walkTurnBackDuration) {
			int newDir = turn();

			switch( state ){
			case State.CALM:
				if( Random.Range(0,2) == 1){

					if( newDir == 1 ){
						moveTargetX = terrainLimitXMax;
					}else{
						moveTargetX = terrainLimitXMin;
					}
					nextAction = Action.WALK_TURNBACK;

				}else{
					nextAction = Action.IDLE;
					moveTargetX = getPosAroundTerrainCenter(terrainSize*0.25f);
				}
				setAction(Action.WALK);
				break;

			case State.FIGHT:
			case State.GOTO_EAT_ZAP:
				whatNextInFight();
				break;
			}
		}
	}
	void ROAR_IN(){
	}
	void ROAR(){
	}
	void ATTACK(){
		if (currentActionTime >= attackDuration) {
			whatNextInFight();
		}
	}
	void ATTACK_JUMP_IN(){
		move (jumpSpeed);
		if (currentActionTime >= attackJumpInDuration) {
			setAction(Action.ATTACK_JUMP);
		}
	}
	void ATTACK_JUMP(){
		move (jumpSpeed);

		float flyDist = Mathf.Abs( myPosX - jumpStartPosX );

		float flyHightRatio = Mathf.Min(1f, flyDist / flyDistance);
	//
		Vector3 pos = transform.position;
		pos.y = myPosY + flyHight * Mathf.Sin (flyHightRatio * Mathf.PI);
		transform.position = pos;

		if (flyDist >= flyDistance) {
			//setState(State.CALM);
			int destiny = Random.Range(0,101); // liczba <0,100>
			if( destiny <= JumpFailurePercent ){ // laduje chujowo
				setAction(Action.ATTACK_LANDING_FAILURE);
			}else{
				setAction(Action.ATTACK_LANDING_TURNBACK);
			}
		}
	}
	void ATTACK_LANDING_TURNBACK(){
		float actionSpeedRatio = Mathf.Max(0f, 1f - (currentActionTime / attackLandingTurnBackDuration));
		move (actionSpeedRatio * jumpSpeed);

		if (currentActionTime >= attackLandingTurnBackDuration) {
			turn();
			//setAction(Action.RUN);
			whatNextInFight();
		}
	}
	void ATTACK_LANDING_FAILURE(){
		float actionSpeedRatio = Mathf.Max (0f, 1f - ((currentActionTime*2f) / attackLandingTurnBackDuration));
		move (actionSpeedRatio * jumpSpeed);

		if (currentActionTime >= attackLandingFailureDuration) {
			setAction(Action.RECOVERY);
		}
	}
	void HIT(){
	}
	void RECOVERY(){
		if (currentActionTime >= RecoveryTime) {
			switch(state){
			case State.FIGHT:
				whatNextInFight();
				break;
			case State.CALM:

				break;
			}
		}
	}

	Animator animator = null;
	float calmMinX = 0f;
	float calmMaxX = 0f;
	float moveTargetX = 0f;
	Action nextAction;
	Zap zap = null;
	float terrainLimitXMin = 0f;
	float terrainLimitXMax = 0f;
	float terrainMiddleX = 0f;
	float terrainSize = 0f;

	float idleDuration = 0;
	float myPosX;
	float myPosY;
	float zapPosX;
	float distToZap;
	float jumpStartPosX;

	void recalcPaD(){
		myPosX = transform.position.x;
		zapPosX = zap.transform.position.x;
		distToZap = Mathf.Abs( myPosX - zapPosX );
	}

	float getPosAroundTerrainCenter(float epsilon = 1f){
		return Random.Range (terrainMiddleX - epsilon, terrainMiddleX + epsilon);
	}

	int getToZapDir(){
		if (myPosX > zapPosX)
			return -1;
		else 
			return 1;
	}

	void paint(Color newColor){
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		sr.color = newColor;
	}
}
