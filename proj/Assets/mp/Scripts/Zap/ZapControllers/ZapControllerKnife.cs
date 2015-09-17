using UnityEngine;
using System.Collections;
//using System; //This allows the IComparable Interface

//[System.Serializable]
public class ZapControllerKnife : ZapController {
	
	public float WalkSpeed = 1.5f;
	public float WalkBackSpeed = 1.5f;
	public float RunSpeed = 5.7f;
	//public float JumpSpeed = 3.8f;
	//public float JumpLongSpeed = 4.9f;
	public float CrouchSpeed = 1.8f;

	//public float JumpImpulse = 7.0f; 
	//public float JumpLongImpulse = 7.15f; 
	public float GravityForce = -20.0f;
	public float MaxSpeedY = 15.0f;

	public float SpeedUpParam = 10.0f; // ile jednosek predkosci hamuje na sekund
	public float SlowDownParam = 20.0f; // ile jednosek predkosci hamuje na sekunde

	public float TURN_LEFTRIGHT_DURATION = 0.2f;
	public float CrouchInOutDuration = 0.2f;

	public ZapControllerKnife (Zap zapPlayer) 
		: base(zapPlayer,"Knife")
	{
	}
	
	bool canPullUp;

	float distToMove;
	Vector3 oldPos;
	float newPosX;
	
	Vector3 climbBeforePos;
	Vector3 climbAfterPos;
	Vector3 climbDistToClimb;
	float climbToJumpDuration;

	float groundUnderFeet;
	
	public override void Update (float deltaTime) {	
		//Debug.Log ("ZapContrllerNormal::Update : " + deltaTime);

		currentActionTime = zap.getCurrentActionTime();
		
		oldPos = transform.position;
		newPosX = oldPos.x;
		distToMove = 0.0f;
		
		switch (action) {
		case Action.IDLE:
			Action_IDLE();
			break;

		case Action.PREPARE_TO_JUMP:
			if( currentActionTime >= 0.2f ){
				//jump();
			}
			break;

		case Action.WALK_LEFT:
		case Action.WALKBACK_LEFT:
			Action_WALK(-1);
			break;

		case Action.WALK_RIGHT:
		case Action.WALKBACK_RIGHT:
			Action_WALK(1);
			break;

		case Action.TURN_STAND_LEFT:
			if (Input.GetKeyDown (zap.keyJump)) {
				wantJumpAfter = true;
			}
			if( currentActionTime >= TURN_LEFTRIGHT_DURATION ){
				zap.turnLeft();
				turnLeftFinish();
			}
			break;
			
		case Action.TURN_STAND_RIGHT:
			if (Input.GetKeyDown (zap.keyJump)) {
				wantJumpAfter = true;
			}
			if( currentActionTime >= TURN_LEFTRIGHT_DURATION ){
				zap.turnRight();
				turnRightFinish();
			}
			break;

		case Action.CROUCH_IN:
			Action_CROUCH_IN();
			break;
			
		case Action.GET_UP:
			Action_GET_UP();
			break;
			
		case Action.CROUCH_IDLE:
			Action_CROUCH_IDLE();
			break;
			
		case Action.CROUCH_LEFT:
		case Action.CROUCH_LEFT_BACK:
			Action_CROUCH_LEFTRIGHT(-1);
			break;
			
		case Action.CROUCH_RIGHT:
		case Action.CROUCH_RIGHT_BACK:
			Action_CROUCH_LEFTRIGHT(1);
			break;
		};
		
		if (wantGetUp) {
			if( zap.canGetUp() ){
				setAction(Action.GET_UP);
				wantGetUp = false;
			}
		}
		
		switch (zap.getState()) {
				
		case Zap.State.IN_AIR:

			
//			zap.AddImpulse(new Vector2(0.0f, GravityForce * deltaTime));
//			
//			if( isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG) ){
//				
//				if( Input.GetKey(zap.keyLeft) ){
//					zap.velocity.x -= (FlyUserControlParam * deltaTime);
//					
//					if( isInAction(Action.JUMP_LEFT) ){
//						if( Mathf.Abs( zap.velocity.x ) > JumpSpeed )
//							zap.velocity.x = -JumpSpeed;
//					}else{
//						if( Mathf.Abs( zap.velocity.x ) > JumpLongSpeed )
//							zap.velocity.x = -JumpLongSpeed;
//					}
//					
//				}else if ( Input.GetKey(zap.keyRight) ){
//					zap.velocity.x += (FlyUserControlParam * deltaTime);
//					if( zap.velocity.x > 0.0f ) zap.velocity.x = 0.0f;
//				}
//			}else if( isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG) ){
//				if( Input.GetKey(zap.keyRight) ){
//					zap.velocity.x += (FlyUserControlParam * deltaTime);
//					
//					if( isInAction(Action.JUMP_RIGHT) ){
//						if( Mathf.Abs( zap.velocity.x ) > JumpSpeed )
//							zap.velocity.x = JumpSpeed;
//					}else{
//						if( Mathf.Abs( zap.velocity.x ) > JumpLongSpeed )
//							zap.velocity.x = JumpLongSpeed;
//					}
//					
//				}else if( Input.GetKey(zap.keyLeft) ) {
//					zap.velocity.x -= (FlyUserControlParam * deltaTime);
//					if( zap.velocity.x < 0.0f ) zap.velocity.x = 0.0f;
//				} 
//			}else if( isInAction(Action.JUMP) ){
//				if( Input.GetKey(zap.keyLeft) ){
//					zap.velocity.x -= (FlyUpUserControlParam * deltaTime);
//					if( Mathf.Abs( zap.velocity.x ) > JumpSpeed )
//						zap.velocity.x = -JumpSpeed;
//				}
//				if( Input.GetKey(zap.keyRight) ){
//					zap.velocity.x += (FlyUpUserControlParam * deltaTime);
//					if( Mathf.Abs( zap.velocity.x ) > JumpSpeed )
//						zap.velocity.x = JumpSpeed;
//				}
//				
//				if( zap.velocity.x > 0.0f ){
//					zap.turnRight();
//				}else if(zap.velocity.x < 0.0f) {
//					zap.turnLeft();
//				}
//			}
//			
//			Vector3 distToFall = new Vector3();
//			distToFall.x = zap.velocity.x * deltaTime;
//			
//			if( distToFall.x > 0.0f ){
//				float obstacleOnRoad = zap.checkRight(distToFall.x + 0.01f,!zap.stateJustChanged);
//				if( obstacleOnRoad >= 0.0f ){
//					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
//						distToFall.x = obstacleOnRoad;
//						zap.velocity.x = 0.0f;
//					}
//				}
//			}else if( distToFall.x < 0.0f ){
//				float obstacleOnRoad = zap.checkLeft( Mathf.Abs(distToFall.x) + 0.01f,!zap.stateJustChanged);
//				if( obstacleOnRoad >= 0.0f ){
//					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
//						distToFall.x = -obstacleOnRoad;
//						zap.velocity.x = 0.0f;
//					}
//				}
//			}
//			
//			transform.position = transform.position + distToFall;
//			distToFall.x = 0f;
//			
//			zap.velocity.y += zap.GetImpulse().y;
//			if(zap.velocity.y > MaxSpeedY)
//				zap.velocity.y = MaxSpeedY;
//			if(zap.velocity.y < -MaxSpeedY)
//				zap.velocity.y = -MaxSpeedY;
//			
//			distToFall.y = zap.velocity.y * deltaTime;
//			
//			bool justLanding = false;
//			
//			if( distToFall.y > 0.0f ) { // leci w gore
//				//transform.position = transform.position + distToFall;
//			} else if( distToFall.y < 0.0f ) { // spada
//				if( zap.lastVelocity.y >= 0.0f ) { // zaczyna spadac
//					// badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
//					zap.startFallPos = transform.position;
//					//print ( "zap.startFallPos : " + zap.startFallPos );
//					if( zap.lastVelocity.y > 0.0f ){
//						lastCatchedClimbHandle = null;
//					}
//				}
//				groundUnderFeet = zap.checkDown( Mathf.Abs(distToFall.y) + 0.01f);
//				if( groundUnderFeet >= 0.0f ){
//					if( (groundUnderFeet < Mathf.Abs(distToFall.y)) || Mathf.Abs( groundUnderFeet - Mathf.Abs(distToFall.y)) < 0.01f  ){
//						lastCatchedClimbHandle = null;
//						distToFall.y = -groundUnderFeet;
//						justLanding = true;
//					}
//				}
//			}
//			
//			transform.position = transform.position + distToFall;
//			
//			if( justLanding ){
//				
//				if( zap.landingSound )
//					zap.getAudioSource().PlayOneShot( zap.landingSound );
//				
//				zap.setFuddledFromBrid( false );
//				
//				zap.setState(Zap.State.ON_GROUND);
//				zap.velocity.y = 0.0f;
//				
//				Vector3 fallDist = zap.startFallPos - transform.position;
//				
//				if( fallDist.y >= VeryHardLandingHeight ){
//					zap.die(Zap.DeathType.VERY_HARD_LANDING);
//				} else if( fallDist.y >= HardLandingHeight ){
//					
//					zap.velocity.x = 0.0f;
//					setAction (Action.LANDING_HARD);
//					
//				}else{
//					
//					resetActionAndState();
//					
//				}
//			}
			
			break;
			
		case Zap.State.ON_GROUND:
			float distToGround = 0.0f;
			bool groundUnderFeet2 = zap.checkGround (true, zap.layerIdGroundAllMask, ref distToGround);
			if (groundUnderFeet2) {
				
			}else{
				zap.setState(Zap.State.IN_AIR);
				setAction(Action.JUMP);
				wantGetUp = false;
			}
			
			break;

		};
		
		zap.lastVelocity = zap.velocity;
		
	}
	
	public override void FUpdate(float fDeltaTime){
	}
	
	public override void activate(){
		setAction (Action.IDLE);
		canPullUp = false;
		desiredSpeedX = 0.0f;
	}
	public override void deactivate(){
	}
	
	public enum Action{
		UNDEF = 0,
		IDLE,
		WALK_LEFT,
		WALK_RIGHT,
		WALKBACK_LEFT,
		WALKBACK_RIGHT,
		TURN_STAND_LEFT,
		TURN_STAND_RIGHT,
		PREPARE_TO_JUMP,
		JUMP,
		JUMP_LEFT,
		JUMP_RIGHT,
		CROUCH_IN,
		GET_UP,
		CROUCH_IDLE,
		CROUCH_LEFT,
		CROUCH_RIGHT,
		CROUCH_LEFT_BACK,
		CROUCH_RIGHT_BACK,
		FALL,
		STOP_WALK,
		STOP_RUN,
		DIE
	};
	
	Action getAction(){
		return action;
	}
	bool setAction(Action newAction, int param = 0){
		
		if (action == newAction)
			return false;
		
		action = newAction;
		zap.resetCurrentActionTime ();
		zap.getAnimator().speed = 1f;
		
		switch (newAction) {
			
		case Action.IDLE:
			//if( zap.faceRight() ) zap.getAnimator().Play("Zap_idle_R");
			//else zap.getAnimator().Play ("Zap_idle_L");

			if( zap.faceRight() ) zap.getAnimator().Play("Zap_knife_idle");
			else zap.getAnimator().Play ("Zap_knife_idle");

			break;
			
		case Action.DIE:
			Zap.DeathType dt = (Zap.DeathType)param;
			string msgInfo = "";
			
			switch( dt ){
				
			case Zap.DeathType.VERY_HARD_LANDING:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_hitground_R");
				else zap.getAnimator().Play("Zap_death_hitground_L");
				msgInfo = zap.DeathByVeryHardLandingText;
				break;
				
			case Zap.DeathType.SNAKE:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_poison_R");
				else zap.getAnimator().Play("Zap_death_poison_L");
				msgInfo = zap.DeathBySnakeText;
				break;
				
			case Zap.DeathType.POISON:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_poison_R");
				else zap.getAnimator().Play("Zap_death_poison_L");
				msgInfo = zap.DeathByPoisonText;
				break;
				
			case Zap.DeathType.CROCODILE:
				msgInfo = zap.DeathByCrocodileText;
				break;
				
			default:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_hitground_R");
				else zap.getAnimator().Play("Zap_death_hitground_L");
				msgInfo = zap.DeathByDefaultText;
				break;
				
			};
			
			zap.showInfo (msgInfo, -1);
			
			if( zap.dieSounds.Length != 0 )
				zap.getAudioSource().PlayOneShot(zap.dieSounds[Random.Range(0,zap.dieSounds.Length)], 0.3F);
			break;
			
		case Action.WALK_LEFT:
			zap.getAnimator().Play("Zap_knife_walk");
			break;
		case Action.WALK_RIGHT:
			zap.getAnimator().Play("Zap_knife_walk");
			break;

		case Action.WALKBACK_LEFT:
			zap.getAnimator().Play("Zap_knife_walkback");
			break;
		case Action.WALKBACK_RIGHT:
			zap.getAnimator().Play("Zap_knife_walkback");
			break;

		case Action.TURN_STAND_LEFT:
			zap.getAnimator().Play("Zap_knife_turnleft");
			wantJumpAfter = false;
			break;
			
		case Action.TURN_STAND_RIGHT:
			zap.getAnimator().Play("Zap_knife_turnright");
			wantJumpAfter = false;
			break;
			
		case Action.PREPARE_TO_JUMP:
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_jump_in_R");
			else zap.getAnimator().Play("Zap_jump_in_L");
			break;
			
		case Action.JUMP:
			if( param == 0 ){
				
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_jump_fly_R");
				else zap.getAnimator().Play("Zap_jump_fly_L");
				
			}else if (param == 1) {
				if( zap.faceRight() ) zap.getAnimator().Play("zap_rocks_climb_R");
				else zap.getAnimator().Play("zap_rocks_climb_L");
			}
			if( zap.jumpSounds.Length != 0 )
				zap.getAudioSource().PlayOneShot(zap.jumpSounds[Random.Range(0,zap.jumpSounds.Length)], 0.2F);
			break;
			
		case Action.JUMP_LEFT:
		case Action.JUMP_RIGHT:

			if( zap.faceRight() ) zap.getAnimator().Play("Zap_run_jump_fly_R");
			else zap.getAnimator().Play("Zap_run_jump_fly_L");
			
			if( zap.jumpSounds.Length != 0 )
				zap.getAudioSource().PlayOneShot(zap.jumpSounds[Random.Range(0,zap.jumpSounds.Length)], 0.2F);
			break;

		case Action.CROUCH_IN:
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_crouch_in_R");
			else zap.getAnimator().Play("Zap_crouch_in_L");
			break;
			
		case Action.GET_UP:
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_getup_R");
			else zap.getAnimator().Play("Zap_getup_L");
			break;
			
		case Action.CROUCH_IDLE:
			if( zap.faceRight () ) zap.getAnimator().Play("Zap_crouch_move_R");
			else zap.getAnimator().Play("Zap_crouch_move_L");
			zap.getAnimator().speed = 0f;
			break;
			
		case Action.CROUCH_LEFT:
			zap.getAnimator().Play("Zap_crouch_move_L");
			break;
		case Action.CROUCH_RIGHT:
			zap.getAnimator().Play("Zap_crouch_move_R");
			break;
			
		case Action.CROUCH_LEFT_BACK:
			zap.getAnimator().Play("Zap_crouch_move_back_R");
			break;
			
		case Action.CROUCH_RIGHT_BACK:
			zap.getAnimator().Play("Zap_crouch_move_back_L");
			break;
		};
		
		return true;
	}
	bool isInAction(Action test) {
		return action == test;
	}
	bool isNotInAction(Action test){
		return action != test;
	}

	public override int keyUpDown(){
		if (isInState (Zap.State.ON_GROUND)) {
		}
		return 0;
	}
	
	public override int keyUpUp(){
//		if ( setMountIdle ()) {
//			if (isInState (Zap.State.MOUNT)) {
//				if( Input.GetKey(zap.keyLeft) )
//					keyLeftDown();
//				else if(Input.GetKey(zap.keyRight) )
//					keyRightDown();
//				else if(Input.GetKey(zap.keyDown) )
//					keyDownDown();
//			}
//		}
		return 0;
	}
	
	public override int keyDownDown(){
		if (isInState (Zap.State.ON_GROUND)) {			
			setAction(Action.CROUCH_IN);
			return 1;
		}
		
		return 0;
	}
	
	public override int keyDownUp(){
//		if ( setMountIdle ()) {
//			if (isInState (Zap.State.MOUNT)) {
//				if( Input.GetKey(zap.keyLeft) )
//					keyLeftDown();
//				else if(Input.GetKey(zap.keyRight) )
//					keyRightDown();
//				else if(Input.GetKey(zap.keyUp) )
//					keyUpDown();
//			}
		//} else
		if (isInState (Zap.State.ON_GROUND)) {
			if( crouching() || isInAction(Action.CROUCH_IN) ){
				if( zap.canGetUp() ){
					setAction(Action.GET_UP);
				}else{
					wantGetUp = true;
				}
			}
		}
		return 0;
	}
	
	public override int keyRunDown(){
		return 0;
	}
	
	public override int keyRunUp(){
		return 0;
	}
	
	public override int keyLeftDown(){
		if ((isInAction (Action.IDLE) || moving (-1) || jumping ()) && isInState (Zap.State.ON_GROUND)) {
			if (zap.checkLeft (0.1f) >= 0.0f) {
				if( zap.dir() == Vector2.right ){
					//turnLeftStart();
				}
				return 0;
			}
			
			if( zap.dir() == -Vector2.right ){
				desiredSpeedX = WalkSpeed;
				speedLimiter(-1,desiredSpeedX+1.0f);
				setAction (Action.WALK_LEFT);
				return 1;

			} else {
				//turnLeftStart();
				desiredSpeedX = WalkBackSpeed;
				speedLimiter(-1,desiredSpeedX+1.0f);
				setAction (Action.WALKBACK_LEFT);
				return 1;
			}
		} else if (isInAction (Action.CROUCH_IDLE) && isInState (Zap.State.ON_GROUND)) {
			if( zap.checkLeft(0.1f) >= 0.0f ){
				return 0;
			}
			desiredSpeedX = CrouchSpeed;
			if( zap.dir () == -Vector2.right ){
				setAction(Action.CROUCH_LEFT);
			}else{
				setAction(Action.CROUCH_LEFT_BACK);
			}
			return 1;
		}
		return 0;
	}
	
	public override int keyRightDown(){
		if ( (isInAction (Action.IDLE) || moving(1) || jumping()) && isInState(Zap.State.ON_GROUND) ) {
			if( zap.checkRight (0.1f) >= 0.0f ) {
				if( zap.dir () == -Vector2.right){
					//turnRightStart();
				}
				return 0;
			}
			if( zap.dir() == Vector2.right ){
				desiredSpeedX = WalkSpeed;
				speedLimiter(1,desiredSpeedX+1.0f);
				setAction(Action.WALK_RIGHT);
				return 1;
			}else{
				//turnRightStart();
				desiredSpeedX = WalkBackSpeed;
				speedLimiter(1,desiredSpeedX+1.0f);
				setAction(Action.WALKBACK_RIGHT);
				return 1;
			}
		} else if (isInAction (Action.CROUCH_IDLE) && isInState (Zap.State.ON_GROUND)) {
			if( zap.checkRight(0.1f) >= 0.0f ){
				return 0;
			}
			desiredSpeedX = CrouchSpeed;
			if( zap.dir () == Vector2.right ){
				setAction(Action.CROUCH_RIGHT);
			}else{
				setAction(Action.CROUCH_RIGHT_BACK);
			}
			return 1;
		}
		return 0;
	}
	
	public override int keyLeftUp(){

		if (isInState (Zap.State.ON_GROUND)){
			desiredSpeedX = 0.0f;
		}

//		if ( !setMountIdle() ) {
//			if (isInState (Zap.State.ON_GROUND)){
//				desiredSpeedX = 0.0f;
//			}
//		} else {
//			if (isInState (Zap.State.MOUNT)) {
//				if( Input.GetKey(zap.keyRight) )
//					keyRightDown();
//				else if(Input.GetKey(zap.keyUp) )
//					keyUpDown();
//				else if(Input.GetKey(zap.keyDown) )
//					keyDownDown();
//			}
//		}
		
		return 0;
	}
	
	public override int keyRightUp(){

		if (isInState (Zap.State.ON_GROUND)) {
			desiredSpeedX = 0.0f;
		}

//		if (!setMountIdle ()) {
//			if (isInState (Zap.State.ON_GROUND)) {
//				desiredSpeedX = 0.0f;
//			}
//		} else {
//			if (isInState (Zap.State.MOUNT)) {
//				if( Input.GetKey(zap.keyLeft) )
//					keyLeftDown();
//				else if(Input.GetKey(zap.keyUp) )
//					keyUpDown();
//				else if(Input.GetKey(zap.keyDown) )
//					keyDownDown();
//			}
//		}
		
		return 0;
	}
	
	//bool jumpKeyPressed = false;
	
	public override int keyJumpDown(){
		
		Debug.Log ("ZapControllerNormal::keyJumpDown()");
		//jumpKeyPressed = true;
		
		switch (action) {
		case Action.IDLE:
			if (isInState (Zap.State.ON_GROUND)) {
				//preparetojump ();
			}
			break;
			
		case Action.WALK_LEFT:
			//jumpLeft ();
			break;
		case Action.WALK_RIGHT:
			//jumpRight ();
			break;
		}
		return 0;
	}
	
	public override int keyJumpUp(){
		//jumpKeyPressed = false;
		canJumpAfter = true;
		return 0;
	}

	bool checkDir(){
		Vector2 mouseInScene = zap.touchCamera.ScreenToWorldPoint (Input.mousePosition);
		if (zap.faceRight ()) {
			if (transform.position.x > mouseInScene.x){
				setAction (Action.TURN_STAND_LEFT);
				return true;
			}
		} else {
			if (transform.position.x < mouseInScene.x){
				setAction (Action.TURN_STAND_RIGHT);
				return true;
			}
		}
		return false;
	}

	int Action_IDLE(){

		checkDir ();

		return 0;
	}
	
	int Action_WALK(int dir){

		bool dirChanged = checkDir ();
		if (dirChanged) {
			//setAction(Action.IDLE);
			//resetActionAndState();
			return 0;
		}

		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f ) {
			setAction(Action.IDLE);
			resetActionAndState();
			return 0;
		}
		
		distToMove = zap.velocity.x * zap.getCurrentDeltaTime();
		
		zap.getAnimator().speed = 0.5f + (Mathf.Abs( zap.velocity.x ) / WalkSpeed ) * 0.5f;
		
		float distToObstacle = 0.0f;
		if (zap.checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			setActionIdle();
		}
		
		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);
		
		float distToGround = 0.0f;
		bool groundUnderFeet = zap.checkGround (false, zap.layerIdLastGroundTypeTouchedMask, ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
		}
		return 0;
	}

	int Action_CROUCH_IN(){
		
		if (currentActionTime >= CrouchInOutDuration) {
			crouch();
		}
		return 0;
	}
	
	int Action_GET_UP(){
		
		if (currentActionTime >= CrouchInOutDuration) {
			getUp();			
		}
		
		return 0;
	}
	
	int Action_CROUCH_IDLE(){
		if( Input.GetKey(zap.keyDown) ){
			//tryStartClimbPullDown();
		}
		return 0;
	}
	
	int Action_CROUCH_LEFTRIGHT(int dir){
		
		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f) {
			setActionCrouchIdle();
			if( crouching() ) {
				if( Input.GetKey(zap.keyLeft) ) {
					keyLeftDown();
				} else if( Input.GetKey(zap.keyRight) ){
					keyRightDown();
				}
			}
		}
		
		distToMove = zap.velocity.x * zap.getCurrentDeltaTime();
		
		float distToObstacle = 0.0f;
		if (zap.checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			setActionCrouchIdle();
		}
		
		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);
		
		//		float distToGround = 0.0f;
		//		bool groundUnderFeet = zap.checkGround (false, zap.layerIdLastGroundTypeTouchedMask, ref distToGround);
		//		if (groundUnderFeet) {
		//			transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
		//		}
		
		return 0;
	}

//	int Action_CLIMB_PULLDOWN(){
//		if( currentActionTime >= CLIMBDUR_CLIMB ){
//			setAction(Action.CLIMB_CATCH,1);
//			zap.setState(Zap.State.CLIMB);
//			canPullUp = true;
//			transform.position = climbAfterPos;
//		} else {
//		}
//		
//		return 0;
//	}
	
//	int Action_CLIMB_JUMP_TO_CATCH(){
//		// dociaganie do punktu:
//		if (currentActionTime >= climbToJumpDuration) {
//			setAction (Action.CLIMB_CATCH);
//			transform.position = climbAfterPos;
//		} else {
//			float ratio = currentActionTime / climbToJumpDuration;
//			transform.position = climbBeforePos + climbDistToClimb * ratio;
//		}
//		
//		return 0;
//	}
	
//	int Action_CLIMB_CATCH(){
//		if ( (Input.GetKeyDown (zap.keyUp) || Input.GetKey(zap.keyUp)) && canPullUp) {
//			
//			climbAfterPos.x = catchedClimbHandle.transform.position.x;
//			climbAfterPos.y = catchedClimbHandle.transform.position.y;
//			
//			climbBeforePos = transform.position;
//			climbDistToClimb = climbAfterPos - climbBeforePos;
//			
//			setAction (Action.CLIMB_CLIMB);
//			catchedClimbHandle = null;
//			lastCatchedClimbHandle = null;
//		} else if ( Input.GetKeyDown (zap.keyJump)) {
//			if (zap.dir () == Vector2.right && Input.GetKey (zap.keyLeft)) {
//				zap.turnLeft ();
//				jumpLeft ();
//				catchedClimbHandle = null;
//				lastCatchedClimbHandle = null;
//			} else if (Input.GetKey (zap.keyRight)) {
//				zap.turnRight ();
//				jumpRight ();
//				catchedClimbHandle = null;
//				lastCatchedClimbHandle = null;
//			} else if( Input.GetKey(zap.keyDown) ){
//				zap.velocity.x = 0.0f;
//				zap.velocity.y = 0.0f;
//				zap.setState (Zap.State.IN_AIR);
//				setAction (Action.JUMP);
//				lastCatchedClimbHandle = catchedClimbHandle;
//				catchedClimbHandle = null;
//			} else {
//				jumpFromClimb ();
//				lastCatchedClimbHandle = catchedClimbHandle;
//				catchedClimbHandle = null;
//			}
//		}
//		
//		return 0;
//	}
	
//	int Action_CLIMB_CLIMB(){
//		
//		if (currentActionTime >= CLIMBDUR_CLIMB) {
//			zap.setState (Zap.State.ON_GROUND);
//			transform.position = climbAfterPos; 
//			
//			if( zap.canGetUp() ){
//				setAction (Action.IDLE);
//				resetActionAndState ();
//			}else{
//				setAction (Action.CROUCH_IDLE);
//				wantGetUp = !Input.GetKey(zap.keyDown);
//				
//				if( Input.GetKey(zap.keyLeft) ) {
//					keyLeftDown();
//				} else if( Input.GetKey(zap.keyRight) ){
//					keyRightDown();
//				}
//			}
//			
//		} else {
//			float ratio = currentActionTime / CLIMBDUR_CLIMB;
//			transform.position = climbBeforePos + climbDistToClimb * ratio;
//		}
//		
//		return 0;
//	}

	void crouch(){
		if (isInState (Zap.State.ON_GROUND)) {
			
			switch (action) {
				
			case Action.IDLE:
			case Action.JUMP:
			case Action.CROUCH_IN:
				setAction (Action.CROUCH_IDLE);
				if( Input.GetKey(zap.keyLeft) ){
					keyLeftDown();
				} else if( Input.GetKey(zap.keyRight) ){
					keyRightDown();
				}else{
					zap.velocity.x = 0.0f;
					zap.velocity.y = 0.0f;
				}
				break;
				
			case Action.WALK_LEFT:
			case Action.JUMP_LEFT:
				if( Input.GetKey(zap.keyLeft)){
					zap.velocity.x = 0.0f;
					desiredSpeedX = CrouchSpeed;
					if( zap.dir () == -Vector2.right ){
						setAction(Action.CROUCH_LEFT);
					}else{
						setAction(Action.CROUCH_LEFT_BACK);
					}
				}else{
					zap.velocity.x = 0.0f;
					zap.velocity.y = 0.0f;
					setAction (Action.CROUCH_IDLE);
				}
				break;
				
			case Action.WALK_RIGHT:
			case Action.JUMP_RIGHT:
				if( Input.GetKey(zap.keyRight)){
					zap.velocity.x = 0.0f;
					desiredSpeedX = CrouchSpeed;
					if( zap.dir () == Vector2.right ){
						setAction(Action.CROUCH_RIGHT);
					}else{
						setAction(Action.CROUCH_RIGHT_BACK);
					}
				}else{
					zap.velocity.x = 0.0f;
					zap.velocity.y = 0.0f;
					setAction (Action.CROUCH_IDLE);
				}
				break;
			}
			
		}
	}
	
//	void preparetojump(){
//		if (isNotInState (Zap.State.ON_GROUND) || isNotInAction (Action.IDLE))
//			return;
//		
//		zap.velocity.x = 0.0f;
//		zap.velocity.y = 0.0f;
//		setAction (Action.PREPARE_TO_JUMP);
//	}
	
//	void jump(){
//		zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
//		zap.setState(Zap.State.IN_AIR);
//		setAction (Action.JUMP);
//		
//		lastFrameHande = false;
//	}
	
//	void jumpFromClimb(){
//		zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
//		zap.setState(Zap.State.IN_AIR);
//		setAction (Action.JUMP,1);
//		lastFrameHande = false;
//	}
	
//	void jumpLeft(){
//		zap.velocity.x = -JumpSpeed;
//		zap.velocity.y = 0.0f;
//		zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
//		zap.setState(Zap.State.IN_AIR);
//		setAction (Action.JUMP_LEFT);
//		
//		lastFrameHande = false;
//	}
//	
//	void jumpRight(){
//		zap.velocity.x = JumpSpeed;
//		zap.velocity.y = 0.0f;
//		zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
//		zap.setState(Zap.State.IN_AIR);
//		setAction (Action.JUMP_RIGHT);
//		
//		lastFrameHande = false;
//	}

	void turnLeftStart(){
		setAction (Action.TURN_STAND_LEFT);
		
		if (Input.GetKeyDown (zap.keyJump) || ( Input.GetKey (zap.keyJump) && canJumpAfter) )
			wantJumpAfter = true;
	}
	
	void turnRightStart(){
		setAction (Action.TURN_STAND_RIGHT);
		
		if (Input.GetKeyDown (zap.keyJump) || (Input.GetKey (zap.keyJump) && canJumpAfter) )
			wantJumpAfter = true;
	}
	
	void turnLeftFinish(){
		setAction (Action.IDLE);
		
		if( wantJumpAfter ) {
			//jumpLeft();
			
			if( Input.GetKey(zap.keyJump) )
				canJumpAfter = false;
			
		} else {
			resetActionAndState ();
		}
	}
	
	void turnRightFinish(){
		setAction (Action.IDLE);
		
		if( wantJumpAfter) {
			//jumpRight();
			
			if( Input.GetKey(zap.keyJump) )
				canJumpAfter = false;
			
			
		} else {
			resetActionAndState ();
		}
	}
	
	void setActionIdle(){
		zap.velocity.x = 0.0f;
		setAction (Action.IDLE);
	}
	void setActionCrouchIdle(){
		zap.velocity.x = 0.0f;
		setAction (Action.CROUCH_IDLE);
	}
	
	void resetActionAndState(){
		if (isInState (Zap.State.ON_GROUND)) {
			if (Input.GetKey (zap.keyDown)) { //&& (Input.GetKey(zap.keyLeft) || Input.GetKey(zap.keyRight)) ){
				if (keyDownDown () == 0)
					setActionIdle ();
			} else if (Input.GetKey (zap.keyLeft)) {
				if (keyLeftDown () == 0)
					setActionIdle ();
			} else if (Input.GetKey (zap.keyRight)) {
				if (keyRightDown () == 0)
					setActionIdle ();
			} else {
				if (isInState (Zap.State.ON_GROUND)) {
					setActionIdle ();
				}
			}
		} 
	}
	
//	int walking(){
//		if (isInAction (Action.WALK_RIGHT))
//			return 1;
//		if (isInAction (Action.WALK_LEFT))
//			return -1;
//		return 0;
//	}
	
	bool moving(Vector2 dir){
		if (dir == Vector2.right)
			return isInAction(Action.WALK_RIGHT) || isInAction(Action.WALKBACK_RIGHT);
		else 
			return isInAction(Action.WALK_LEFT) || isInAction(Action.WALKBACK_LEFT);
	}
	bool moving(int dir){
		if (dir == 1)
			return isInAction(Action.WALK_RIGHT) || isInAction(Action.WALKBACK_RIGHT);
		else 
			return isInAction(Action.WALK_LEFT) || isInAction(Action.WALKBACK_LEFT);
	}
	bool jumping(){
		return isInAction(Action.JUMP) || isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_RIGHT);
	}
	public override bool crouching(){
		return isInAction(Action.CROUCH_IDLE) || 
			isInAction(Action.CROUCH_LEFT) || isInAction(Action.CROUCH_LEFT_BACK) ||
				isInAction(Action.CROUCH_RIGHT) || isInAction(Action.CROUCH_RIGHT_BACK);
	}
	public override void zapDie (Zap.DeathType deathType){
		setAction (Action.DIE, (int)deathType);
	}
	public override void reborn(){
		if (zap.getLastTouchedCheckPoint().GetComponent<CheckPoint> ().startMounted) {
			zap.setState(Zap.State.MOUNT);
		}
	}
	public override bool triggerEnter(Collider2D other){
		
		if (other.gameObject.tag == "Bird") {
			if( isInState(Zap.State.MOUNT) ){
				zap.velocity.x = 0.0f;
				zap.velocity.y = 0.0f;
				setAction(Action.JUMP);
				zap.setState(Zap.State.IN_AIR);
				
				if( zap.canBeFuddleFromBird )
					zap.setFuddledFromBrid(true);
				
			} else if( isInState(Zap.State.IN_AIR) ) {
				zap.velocity.x = 0.0f;
			}
			return true;
		}
		
		return false;
	}
	
	bool checkSpeed(int dir){
		float speedX = Mathf.Abs (zap.velocity.x);
		if (speedX < desiredSpeedX) { // trzeba przyspieszyc
			
			float velocityDamp = SpeedUpParam * zap.getCurrentDeltaTime();
			speedX += velocityDamp;
			if( speedX > desiredSpeedX ){
				speedX = desiredSpeedX;
				zap.velocity.x = desiredSpeedX * dir;
				return true;
			}
			zap.velocity.x = speedX * dir;
			return false;
			
		} else if (speedX > desiredSpeedX) { // trzeba zwolnic
			float velocityDamp = SlowDownParam * zap.getCurrentDeltaTime();
			speedX -= velocityDamp;
			if( speedX < desiredSpeedX ){
				speedX = desiredSpeedX;
				zap.velocity.x = desiredSpeedX * dir;
				return true;
			}
			zap.velocity.x = speedX * dir;
			return false;
		}
		return true;
	}
	
	bool speedLimiter(int dir, float absMaxSpeed){
		if( dir == -1 ){
			if( zap.velocity.x < 0.0f && Mathf.Abs(zap.velocity.x) > absMaxSpeed ){
				zap.velocity.x = -absMaxSpeed;
				return true;
			}
		}else if( dir == 1 ){
			if( zap.velocity.x > 0.0f && Mathf.Abs(zap.velocity.x) > absMaxSpeed ){
				zap.velocity.x = absMaxSpeed;
				return true;
			}
		}
		//aa
		return false;
	}
	

	void getUp(){
		setAction(Action.IDLE);
		resetActionAndState ();
	}
	
	public float ClimbPullDownRange = 0.511f;
	
	GameObject canClimbPullDown(){
		
		if (!isInState (Zap.State.ON_GROUND) || !(isInAction (Action.IDLE) || isInAction(Action.CROUCH_IDLE)) )
			return null;
		
		// 1: sytuacja gdy zap jest swoim srodkiem nad tilem
		// 2: sytuacja gdy zap jest swoim srodkiem juz poza tilem
		
		RaycastHit2D hit;
		
		if (zap.dir () == Vector2.right) { //
			
			hit = Physics2D.Raycast (zap.sensorDown2.position, -Vector2.right , ClimbPullDownRange, zap.layerIdGroundHandlesMask);
			if( hit.collider ){
				
				if( Physics2D.Raycast (hit.collider.gameObject.transform.position, -Vector2.right , 0.5f, zap.layerIdGroundMask).collider == null ) {
					return hit.collider.gameObject;
				}
			}
			
		} else {
			
			hit = Physics2D.Raycast (zap.sensorDown2.position, Vector2.right , ClimbPullDownRange, zap.layerIdGroundHandlesMask);
			if( hit.collider ){
				
				if( Physics2D.Raycast (hit.collider.gameObject.transform.position, Vector2.right , 0.5f, zap.layerIdGroundMask).collider == null ) {
					return hit.collider.gameObject;
				}
				
			}
		}
		
		// to jest ta druga sytuacja ...
		
		Vector2 rayOrigin = zap.sensorDown1.position; 
		hit = Physics2D.Raycast (rayOrigin, Vector2.right , zap.getMyWidth(), zap.layerIdGroundHandlesMask);
		
		if (hit.collider) { 
			// badam czy stoje na krawedzi odpowiednio zwrocony
			if (zap.dir () == Vector2.right) { //
				
				// pod lewa noga musi byc przepasc
				rayOrigin = zap.sensorDown1.position;
				if( Physics2D.Raycast (rayOrigin, -Vector2.up , 0.5f, zap.layerIdGroundMask).collider ) return null;
				else return hit.collider.gameObject;
				
			} else {
				
				// pod prawa noga musi byc przepasc
				rayOrigin = zap.sensorDown3.position;
				if( Physics2D.Raycast (rayOrigin, -Vector2.up , 0.5f, zap.layerIdGroundMask).collider ) return null;
				else return hit.collider.gameObject;
				
			}
			
		} else {
			return null;
		}
	}
	
	bool wantGetUp = false;
	bool wantJumpAfter = false;
	bool canJumpAfter = true;
	float desiredSpeedX = 0.0f;
	Action action;
	float currentActionTime = 0f;
}
