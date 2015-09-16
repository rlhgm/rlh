using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

public class ZapNormalController : ZapController {
	
	public ZapNormalController (Zap zapPlayer) 
		: base(zapPlayer,"NormalController")
	{
	}

	public override void Update (float deltaTime) {	
		
		SetImpulse(new Vector2(0.0f, 0.0f));
		
		justJumpedMount = false;

		currentActionTime = zap.getCurrentActionTime();

		oldPos = transform.position;
		newPosX = oldPos.x;
		distToMove = 0.0f;
		
		switch (action) {
		case Action.IDLE:
			Action_IDLE();
			break;
			
		case Action.LANDING_HARD:
			Action_LANDING_HARD();
			break;
			
		case Action.PREPARE_TO_JUMP:
			if( currentActionTime >= 0.2f ){
				jump();
			}
			break;
			
		case Action.CLIMB_PULLDOWN:
			Action_CLIMB_PULLDOWN();
			break;
			
		case Action.CLIMB_JUMP_TO_CATCH:
			Action_CLIMB_JUMP_TO_CATCH();
			break;
			
		case Action.CLIMB_CATCH:
			Action_CLIMB_CATCH();
			break;
			
		case Action.CLIMB_CLIMB:
			Action_CLIMB_CLIMB();
			break;

		case Action.WALK_LEFT:
			Action_WALK(-1);
			break;
		case Action.RUN_LEFT:
			Action_RUN(-1);
			break;
			
		case Action.WALK_RIGHT:
			Action_WALK(1);
			break;
		case Action.RUN_RIGHT:
			Action_RUN(1);
			break;
			
		case Action.TURN_STAND_LEFT:
			if (Input.GetKeyDown (keyJump)) {
				wantJumpAfter = true;
			}
			if( currentActionTime >= TURN_LEFTRIGHT_DURATION ){
				zap.turnLeft();
				turnLeftFinish();
			}
			break;
			
		case Action.TURN_STAND_RIGHT:
			if (Input.GetKeyDown (keyJump)) {
				wantJumpAfter = true;
			}
			if( currentActionTime >= TURN_LEFTRIGHT_DURATION ){
				zap.turnRight();
				turnRightFinish();
			}
			break;
			
		case Action.TURN_RUN_LEFT:
			if( currentActionTime >= 0.85f ){
				zap.turnLeft();
				if( wantJumpAfter ){
					jumpLeft();
				}else{
					setActionIdle();
					resetActionAndState();
				}
			}else{
				int res = Action_TURN_RUN(1);
				if( res == 1 ){
				}
			}
			break;
			
		case Action.TURN_RUN_RIGHT:
			if( currentActionTime >= 0.85f ){
				zap.turnRight();
				if( wantJumpAfter ){
					jumpRight();
				}else{
					setActionIdle();
					resetActionAndState();
				}
			}else{
				int res = Action_TURN_RUN(-1);
				if( res == 1 ){
				}
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
			
		case Action.MOUNT_IDLE:
			break;
			
		case Action.MOUNT_LEFT:
		case Action.MOUNT_RIGHT:
		case Action.MOUNT_UP:
			Action_MOUNTING();
			break;
			
		case Action.MOUNT_DOWN:
			Action_MOUNTING_DOWN();
			break;
			
		case Action.ROPECLIMB_IDLE:
			Action_ROPECLIMB_IDLE(deltaTime);
			break;
			
		case Action.ROPECLIMB_UP:
			Action_ROPECLIMB_UP(deltaTime);
			break;
			
		case Action.ROPECLIMB_DOWN:
			Action_ROPECLIMB_DOWN(deltaTime);
			break;
		};
		
		if (wantGetUp) {
			if( canGetUp() ){
				setAction(Action.GET_UP);
				wantGetUp = false;
			}
		}
		
		switch (zap.getState()) {
			
		case Zap.State.MOUNT:
			break;
			
		case Zap.State.IN_AIR:
			
			if( jumpKeyPressed ) { //Input.GetKeyDown(keyJump) || Input.GetKey(keyJump) ){
				Vector3 fallDist = startFallPos - transform.position;
				if( !fuddledFromBrid && (fallDist.y < MaxFallDistToCatch) )
				{
					if( onMount() ){
						if( jumpFromMount ){
							if( !justJumpedMount ){
							}
						}else{
							setActionMountIdle();
							return;
						}
					} else {
						jumpFromMount = false;
					}
				}
			}
			if( jumpFromMount && Input.GetKey(keyJump) ){
				Vector3 fallDist = startFallPos - transform.position;
				if( !fuddledFromBrid && (fallDist.y < MaxFallDistToCatch) )
				{
					Vector3 flyDist = transform.position - mountJumpStartPos;
					if( flyDist.magnitude >= MountJumpDist ){
						setActionMountIdle();
						jumpFromMount = false;
						return;
					}
				}
			}
			
			if( Input.GetKey(keyJump) ) { 
				
				if( !fuddledFromBrid && tryCatchRope() ){
					
					if( ropeCatchSound )
						myAudio.PlayOneShot( ropeCatchSound );
					
					return;
				}
			}
			
			if( Input.GetKey(keyJump) || autoCatchEdges ){
				Vector3 fallDist = startFallPos - transform.position;
				if( !fuddledFromBrid && fallDist.y < MaxFallDistToCatch )
				{
					if( tryCatchHandle() ){
						lastVelocity = velocity;
						return;
					}
				}
			}
			
			if( Input.GetKeyDown(keyJump) ){
				lastFrameHande = true;
				if( dir () == Vector2.right )
					lastHandlePos = sensorHandleR2.position;
				else
					lastHandlePos = sensorHandleL2.position;
			}
			
			if( Input.GetKeyUp(keyJump) ) {
				lastFrameHande = false;
			}
			
			addImpulse(new Vector2(0.0f, GravityForce * deltaTime));
			
			if( isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG) ){
				
				if( Input.GetKey(keyLeft) ){
					velocity.x -= (FlyUserControlParam * deltaTime);
					
					if( isInAction(Action.JUMP_LEFT) ){
						if( Mathf.Abs( velocity.x ) > JumpSpeed )
							velocity.x = -JumpSpeed;
					}else{
						if( Mathf.Abs( velocity.x ) > JumpLongSpeed )
							velocity.x = -JumpLongSpeed;
					}
					
				}else if ( Input.GetKey(keyRight) ){
					velocity.x += (FlyUserControlParam * deltaTime);
					if( velocity.x > 0.0f ) velocity.x = 0.0f;
				}
			}else if( isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG) ){
				if( Input.GetKey(keyRight) ){
					velocity.x += (FlyUserControlParam * deltaTime);
					
					if( isInAction(Action.JUMP_RIGHT) ){
						if( Mathf.Abs( velocity.x ) > JumpSpeed )
							velocity.x = JumpSpeed;
					}else{
						if( Mathf.Abs( velocity.x ) > JumpLongSpeed )
							velocity.x = JumpLongSpeed;
					}
					
				}else if( Input.GetKey(keyLeft) ) {
					velocity.x -= (FlyUserControlParam * deltaTime);
					if( velocity.x < 0.0f ) velocity.x = 0.0f;
				} 
			}else if( isInAction(Action.JUMP) ){
				if( Input.GetKey(keyLeft) ){
					velocity.x -= (FlyUpUserControlParam * deltaTime);
					if( Mathf.Abs( velocity.x ) > JumpSpeed )
						velocity.x = -JumpSpeed;
				}
				if( Input.GetKey(keyRight) ){
					velocity.x += (FlyUpUserControlParam * deltaTime);
					if( Mathf.Abs( velocity.x ) > JumpSpeed )
						velocity.x = JumpSpeed;
				}
				
				if( velocity.x > 0.0f ){
					turnRight();
				}else if(velocity.x < 0.0f) {
					turnLeft();
				}
			}
			
			Vector3 distToFall = new Vector3();
			distToFall.x = velocity.x * deltaTime;
			
			if( distToFall.x > 0.0f ){
				float obstacleOnRoad = checkRight(distToFall.x + 0.01f,!zap.firstFrameInState);
				if( obstacleOnRoad >= 0.0f ){
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = obstacleOnRoad;
						velocity.x = 0.0f;
					}
				}
			}else if( distToFall.x < 0.0f ){
				float obstacleOnRoad = checkLeft( Mathf.Abs(distToFall.x) + 0.01f,!firstFrameInState);
				if( obstacleOnRoad >= 0.0f ){
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = -obstacleOnRoad;
						velocity.x = 0.0f;
					}
				}
			}
			
			transform.position = transform.position + distToFall;
			distToFall.x = 0f;
			
			velocity.y += impulse.y;
			if(velocity.y > MaxSpeedY)
				velocity.y = MaxSpeedY;
			if(velocity.y < -MaxSpeedY)
				velocity.y = -MaxSpeedY;
			
			distToFall.y = velocity.y * deltaTime;
			
			bool justLanding = false;
			
			if( distToFall.y > 0.0f ) { // leci w gore
				//transform.position = transform.position + distToFall;
			} else if( distToFall.y < 0.0f ) { // spada
				if( lastVelocity.y >= 0.0f ) { // zaczyna spadac
					// badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
					startFallPos = transform.position;
					print ( "startFallPos : " + startFallPos );
					if( lastVelocity.y > 0.0f ){
						lastCatchedClimbHandle = null;
					}
				}
				groundUnderFeet = checkDown( Mathf.Abs(distToFall.y) + 0.01f);
				if( groundUnderFeet >= 0.0f ){
					if( (groundUnderFeet < Mathf.Abs(distToFall.y)) || Mathf.Abs( groundUnderFeet - Mathf.Abs(distToFall.y)) < 0.01f  ){
						lastCatchedClimbHandle = null;
						distToFall.y = -groundUnderFeet;
						justLanding = true;
					}
				}
			}
			
			transform.position = transform.position + distToFall;
			
			if( justLanding ){
				
				if( landingSound )
					myAudio.PlayOneShot( landingSound );
				
				fuddledFromBrid = false;
				
				setState(State.ON_GROUND);
				velocity.y = 0.0f;
				
				Vector3 fallDist = startFallPos - transform.position;
				
				if( fallDist.y >= VeryHardLandingHeight ){
					die(DeathType.VERY_HARD_LANDING);
				} else if( fallDist.y >= HardLandingHeight ){
					
					velocity.x = 0.0f;
					setAction (Action.LANDING_HARD);
					
				}else{
					
					resetActionAndState();
					
				}
			}
			
			break;
			
		case State.ON_GROUND:
			float distToGround = 0.0f;
			bool groundUnderFeet2 = checkGround (true, layerIdGroundAllMask, ref distToGround);
			if (groundUnderFeet2) {
				
			}else{
				setState(State.IN_AIR);
				setAction(Action.JUMP);
				wantGetUp = false;
			}
			
			break;
			
		case State.CLIMB_ROPE:
			Vector3 linkPos = catchedRopeLink.transform.TransformPoint( new Vector3(0.0f, ropeLinkCatchOffset, 0.0f) );
			transform.position = linkPos;
			transform.rotation = catchedRopeLink.transform.rotation;
			
			Quaternion quat = new Quaternion ();
			quat.eulerAngles = new Vector3 (0f, 0f, 0f);
			//weaponText.rotation = quat;
			
			break;
		};
		
		lastVelocity = velocity;

	}
	
	public override void FUpdate(float fDeltaTime){
	}
	
	public override void activate(){
	}
	public override void deactivate(){
	}

	void SetImpulse(Vector2 imp) { impulse = imp; }
	Vector2 getImpulse() { return impulse; }
	void addImpulse(Vector3 imp) { impulse += imp; }
	void addImpulse(Vector2 imp) { 
		impulse.x += imp.x; 
		impulse.y += imp.y; 
	}

	public enum Action{
		UNDEF = 0,
		IDLE,
		WALK_LEFT,
		WALK_RIGHT,
		RUN_LEFT,
		RUN_RIGHT,
		TURN_STAND_LEFT,
		TURN_STAND_RIGHT,
		TURN_RUN_LEFT,
		TURN_RUN_RIGHT,
		//BREAK,
		PREPARE_TO_JUMP,
		JUMP,
		JUMP_LEFT,
		JUMP_LEFT_LONG,
		JUMP_RIGHT,
		JUMP_RIGHT_LONG,
		CROUCH_IN,
		GET_UP,
		CROUCH_IDLE,
		CROUCH_LEFT,
		CROUCH_RIGHT,
		CROUCH_LEFT_BACK,
		CROUCH_RIGHT_BACK,
		LANDING_HARD,
		FALL,
		STOP_WALK,
		STOP_RUN,
		CLIMB_PREPARE_TO_JUMP,
		CLIMB_JUMP_TO_CATCH,
		CLIMB_CATCH,
		CLIMB_CLIMB,
		CLIMB_PULLDOWN,
		MOUNT_IDLE,
		MOUNT_LEFT,
		MOUNT_RIGHT,
		MOUNT_UP,
		MOUNT_DOWN,
		ROPECLIMB_IDLE,
		ROPECLIMB_UP,
		ROPECLIMB_DOWN,
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
		animator.speed = 1.0f;
		
		switch (newAction) {
			
		case Action.IDLE:
			if( faceRight() ) animator.Play("Zap_idle_R");
			else animator.Play ("Zap_idle_L");
			break;
			
		case Action.DIE:
			DeathType dt = (DeathType)param;
			string msgInfo = "";
			
			switch( dt ){
				
			case DeathType.VERY_HARD_LANDING:
				if( faceRight() ) animator.Play("Zap_death_hitground_R");
				else animator.Play("Zap_death_hitground_L");
				msgInfo = DeathByVeryHardLandingText;
				break;
				
			case DeathType.SNAKE:
				if( faceRight() ) animator.Play("Zap_death_poison_R");
				else animator.Play("Zap_death_poison_L");
				msgInfo = DeathBySnakeText;
				break;
				
			case DeathType.POISON:
				if( faceRight() ) animator.Play("Zap_death_poison_R");
				else animator.Play("Zap_death_poison_L");
				msgInfo = DeathByPoisonText;
				break;
				
			case DeathType.CROCODILE:
				msgInfo = DeathByCrocodileText;
				break;
				
			default:
				if( faceRight() ) animator.Play("Zap_death_hitground_R");
				else animator.Play("Zap_death_hitground_L");
				msgInfo = DeathByDefaultText;
				break;
				
			};
			
			showInfo (msgInfo, -1);
			
			if( dieSounds.Length != 0 )
				myAudio.PlayOneShot(dieSounds[Random.Range(0,dieSounds.Length)], 0.3F);
			break;
			
		case Action.WALK_LEFT:
			animator.Play("Zap_walk_L");
			break;
		case Action.WALK_RIGHT:
			animator.Play("Zap_walk_R");
			break;
			
		case Action.RUN_LEFT:
			animator.Play("Zap_run_L");
			break;
		case Action.RUN_RIGHT:
			animator.Play("Zap_run_L");
			break;
			
		case Action.TURN_STAND_LEFT:
			animator.Play("Zap_walk_back_left");
			wantJumpAfter = false;
			break;
			
		case Action.TURN_STAND_RIGHT:
			animator.Play("Zap_walk_back_right");
			wantJumpAfter = false;
			break;
			
		case Action.TURN_RUN_LEFT:
			animator.Play("Zap_runback_L");
			wantJumpAfter = false;
			if( turnRunSounds.Length != 0 )
				myAudio.PlayOneShot(turnRunSounds[Random.Range(0,turnRunSounds.Length)], 0.5F);
			break;
			
		case Action.TURN_RUN_RIGHT:
			animator.Play("Zap_runback_R");
			wantJumpAfter = false;
			if( turnRunSounds.Length != 0 )
				myAudio.PlayOneShot(turnRunSounds[Random.Range(0,turnRunSounds.Length)], 0.5F);
			break;
			
		case Action.PREPARE_TO_JUMP:
			if( faceRight() ) animator.Play("Zap_jump_in_R");
			else animator.Play("Zap_jump_in_L");
			break;
			
		case Action.JUMP:
			if( param == 0 ){
				
				if( faceRight() ) animator.Play("Zap_jump_fly_R");
				else animator.Play("Zap_jump_fly_L");
				
			}else if (param == 1) {
				if( faceRight() ) animator.Play("zap_rocks_climb_R");
				else animator.Play("zap_rocks_climb_L");
			}
			if( jumpSounds.Length != 0 )
				myAudio.PlayOneShot(jumpSounds[Random.Range(0,jumpSounds.Length)], 0.2F);
			break;
			
		case Action.JUMP_LEFT:
		case Action.JUMP_LEFT_LONG:
		case Action.JUMP_RIGHT:
		case Action.JUMP_RIGHT_LONG:
			
			if( faceRight() ) animator.Play("Zap_run_jump_fly_R");
			else animator.Play("Zap_run_jump_fly_L");
			
			if( jumpSounds.Length != 0 )
				myAudio.PlayOneShot(jumpSounds[Random.Range(0,jumpSounds.Length)], 0.2F);
			break;
			
		case Action.LANDING_HARD:
			if( faceRight() ) animator.Play("Zap_landing_hard_R");
			else animator.Play("Zap_landing_hard_L");
			
			if( landingSounds.Length != 0 )
				myAudio.PlayOneShot(landingSounds[Random.Range(0,landingSounds.Length)], 0.15F);
			break;
			
		case Action.CLIMB_PREPARE_TO_JUMP:
			break;
		case Action.CLIMB_JUMP_TO_CATCH:
			break;
		case Action.CLIMB_CATCH:
			if( param == 0 ){
				if( faceRight() ) animator.Play("zap_rocks_catch_position_R");
				else animator.Play("zap_rocks_catch_position_L");
				
			}else if( param == 1 ){
				// tu juz jest we wlasciwej klatce
				if( faceRight() ) animator.Play("zap_rocks_catch_position_rev_R");
				else animator.Play("zap_rocks_catch_position_rev_L");
				animator.speed = 0.0f;
			}
			
			if( catchSounds.Length != 0)
				myAudio.PlayOneShot(catchSounds[Random.Range(0,catchSounds.Length)], 0.7F);
			break;
		case Action.CLIMB_CLIMB:
			if( faceRight() ) animator.Play("Zap_jump_climb_R");
			else animator.Play("Zap_jump_climb_L");
			break;
			
		case Action.CLIMB_PULLDOWN:
			if( faceRight() ) animator.Play("Zap_drop_R");
			else animator.Play("Zap_drop_L");
			break;
			
		case Action.MOUNT_IDLE:
			animator.Play("Zap_climbmove_up");
			animator.speed = 0.0f;
			break;
			
		case Action.MOUNT_LEFT:
			animator.Play("Zap_climbmove_left");
			break;
		case Action.MOUNT_RIGHT:
			animator.Play("Zap_climbmove_right");
			break;
		case Action.MOUNT_UP:
			animator.Play("Zap_climbmove_up");
			break;
		case Action.MOUNT_DOWN:
			animator.Play("Zap_climbmove_down");
			break;
			
		case Action.CROUCH_IN:
			if( faceRight() ) animator.Play("Zap_crouch_in_R");
			else animator.Play("Zap_crouch_in_L");
			break;
			
		case Action.GET_UP:
			if( faceRight() ) animator.Play("Zap_getup_R");
			else animator.Play("Zap_getup_L");
			break;
			
		case Action.CROUCH_IDLE:
			if( faceRight () ) animator.Play("Zap_crouch_move_R");
			else animator.Play("Zap_crouch_move_L");
			animator.speed = 0f;
			break;
			
		case Action.CROUCH_LEFT:
			animator.Play("Zap_crouch_move_L");
			break;
		case Action.CROUCH_RIGHT:
			animator.Play("Zap_crouch_move_R");
			break;
			
		case Action.CROUCH_LEFT_BACK:
			animator.Play("Zap_crouch_move_back_R");
			break;
			
		case Action.CROUCH_RIGHT_BACK:
			animator.Play("Zap_crouch_move_back_L");
			break;
			
		case Action.ROPECLIMB_IDLE:
			setActionRopeClimbIdle();
			break;
			
		case Action.ROPECLIMB_UP:
			if( faceRight() ) animator.Play("Zap_liana_climbup_R");
			else animator.Play("Zap_liana_climbup_L");
			break;
			
		case Action.ROPECLIMB_DOWN:
			if( faceRight() ) animator.Play("Zap_liana_slide_R");
			else animator.Play("Zap_liana_slide_L");
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


	override int keyUpDown(){
		if (isInState (State.MOUNT)) {
			if( !mounting () ){
				Vector3 playerPos = transform.position;
				playerPos.y += 0.1f;
				if( onMount(playerPos) ){
					velocity.x = 0.0f;
					velocity.y = MountSpeed;
					setAction (Action.MOUNT_UP);
					return 1;
				}
			}
		} else if (isInState (State.ON_GROUND)) {
			if( onMount() ){
				velocity.x = 0.0f;
				velocity.y = MountSpeed;
				setAction (Action.MOUNT_UP);
				setState(State.MOUNT);
				return 1;
			}
		}
		return 0;
	}

	override int keyUpUp(){
		if ( setMountIdle ()) {
			if (isInState (State.MOUNT)) {
				if( Input.GetKey(keyLeft) )
					keyLeftDown();
				else if(Input.GetKey(keyRight) )
					keyRightDown();
				else if(Input.GetKey(keyDown) )
					keyDownDown();
			}
		}
		return 0;
	}

	override int keyDownDown(){
		if (isInState (State.MOUNT)) {
			if (!mounting ()) {
				Vector3 playerPos = transform.position;
				playerPos.y -= 0.1f;
				if (onMount (playerPos)) {
					velocity.x = 0.0f;
					velocity.y = -MountSpeed;
					setAction (Action.MOUNT_DOWN);
					return 1;
				}
			}
		} else if (isInState (State.ON_GROUND)) {
			
			if(	tryStartClimbPullDown() ) {
				
				return 1;
				
			} else {
				setAction(Action.CROUCH_IN);
				return 1;
			}
		}
		
		return 0;
	}

	override int keyDownUp(){
		if ( setMountIdle ()) {
			if (isInState (State.MOUNT)) {
				if( Input.GetKey(keyLeft) )
					keyLeftDown();
				else if(Input.GetKey(keyRight) )
					keyRightDown();
				else if(Input.GetKey(keyUp) )
					keyUpDown();
			}
		} else if (isInState (State.ON_GROUND)) {
			if( crouching() || isInAction(Action.CROUCH_IN) ){
				if( canGetUp() ){
					setAction(Action.GET_UP);
				}else{
					wantGetUp = true;
				}
			}
		}
		return 0;
	}

	override int keyRunDown(){
		switch (action) {
			
		case Action.WALK_LEFT:
			if( Input.GetKey(keyLeft) ){
				desiredSpeedX = RunSpeed;
				setAction(Action.RUN_LEFT);
			}
			break;
			
		case Action.WALK_RIGHT:
			if( Input.GetKey(keyRight) ){
				desiredSpeedX = RunSpeed;
				setAction(Action.RUN_RIGHT);
			}
			break;
		};

		return 0;
	}
	override int keyRunUp(){
		
		switch (action) {
			
		case Action.RUN_LEFT:
			if( Input.GetKey(keyLeft) ){
				desiredSpeedX = WalkSpeed;
				setAction(Action.WALK_LEFT);
			}else{
				desiredSpeedX = 0.0f;
			}
			break;
			
		case Action.RUN_RIGHT:
			if( Input.GetKey(keyRight) ) {
				desiredSpeedX = WalkSpeed;
				setAction(Action.WALK_RIGHT);
			}else{
				desiredSpeedX = 0.0f;
			}
			break;
		};

		return 0;
	}

	override int keyLeftDown(){
		if ((isInAction (Action.IDLE) || moving (-1) || jumping ()) && isInState (State.ON_GROUND)) {
			if (checkLeft (0.1f) >= 0.0f) {
				if( dir() == Vector2.right )
					turnLeftStart();
				return false;
			}
			
			if( dir() == -Vector2.right )
			{
				if (Input.GetKey (keyRun)) {
					desiredSpeedX = RunSpeed;
					speedLimiter(-1,desiredSpeedX+1.0f);
					setAction (Action.RUN_LEFT);
					return true;
				} else {
					desiredSpeedX = WalkSpeed;
					speedLimiter(-1,desiredSpeedX+1.0f);
					setAction (Action.WALK_LEFT);
					return true;
				}
			} else {
				turnLeftStart();
				return true;
			}
		} else if (isInState (State.MOUNT)) {
			if (!mounting ()) {
				Vector3 playerPos = transform.position;
				playerPos.x -= 0.1f;
				turnLeft();
				if (onMount (playerPos)) {
					velocity.x = -MountSpeed;
					velocity.y = 0.0f;
					setAction (Action.MOUNT_LEFT);
					return true;
				}
			}
		} else if (isInAction (Action.CROUCH_IDLE) && isInState (State.ON_GROUND)) {
			if( checkLeft(0.1f) >= 0.0f ){
				return false;
			}
			desiredSpeedX = CrouchSpeed;
			if( dir () == -Vector2.right ){
				setAction(Action.CROUCH_LEFT);
			}else{
				setAction(Action.CROUCH_LEFT_BACK);
			}
			return true;
		}
		return false;
	}
	override int keyRightDown(){
		if ( (isInAction (Action.IDLE) || moving(1) || jumping()) && isInState(State.ON_GROUND) ) {
			if( checkRight (0.1f) >= 0.0f ) {
				if( dir () == -Vector2.right)
					turnRightStart();
				return false;
			}
			if( dir() == Vector2.right ){
				if( Input.GetKey(keyRun) ){
					desiredSpeedX = RunSpeed;
					speedLimiter(1,desiredSpeedX+1.0f);
					setAction(Action.RUN_RIGHT);
					return true;
				}else{
					desiredSpeedX = WalkSpeed;
					speedLimiter(1,desiredSpeedX+1.0f);
					setAction(Action.WALK_RIGHT);
					return true;
				}
			}else{
				turnRightStart();
				return true;
			}
		} else if (isInState (State.MOUNT)) {
			if( !mounting() ){
				Vector3 playerPos = transform.position;
				playerPos.x += 0.1f;
				turnRight();
				if( onMount(playerPos) ){
					velocity.x = MountSpeed;
					velocity.y = 0.0f;
					setAction(Action.MOUNT_RIGHT);
					return true;
				}
			}
		} else if (isInAction (Action.CROUCH_IDLE) && isInState (State.ON_GROUND)) {
			if( checkRight(0.1f) >= 0.0f ){
				return false;
			}
			desiredSpeedX = CrouchSpeed;
			if( dir () == Vector2.right ){
				setAction(Action.CROUCH_RIGHT);
			}else{
				setAction(Action.CROUCH_RIGHT_BACK);
			}
			return true;
		}
		return false;
	}
	
	override int keyLeftUp(){
		
		if ( !setMountIdle() ) {
			if (isInState (State.ON_GROUND)){
				desiredSpeedX = 0.0f;
			}
		} else {
			if (isInState (State.MOUNT)) {
				if( Input.GetKey(keyRight) )
					keyRightDown();
				else if(Input.GetKey(keyUp) )
					keyUpDown();
				else if(Input.GetKey(keyDown) )
					keyDownDown();
			}
		}
	}
	override int keyRightUp(){
		if (!setMountIdle ()) {
			if (isInState (State.ON_GROUND)) {
				desiredSpeedX = 0.0f;
			}
		} else {
			if (isInState (State.MOUNT)) {
				if( Input.GetKey(keyLeft) )
					keyLeftDown();
				else if(Input.GetKey(keyUp) )
					keyUpDown();
				else if(Input.GetKey(keyDown) )
					keyDownDown();
			}
		}
	}

	override int keyJumpDownSpec(){
		
		switch (action) {
		case Action.IDLE:
			if( isInState(State.ON_GROUND)) {
				preparetojump();
			}
			break;
			
		case Action.WALK_LEFT:
			jumpLeft();
			break;
		case Action.WALK_RIGHT:
			jumpRight();
			break;
			
		case Action.RUN_LEFT:
			jumpLongLeft();
			break;
		case Action.RUN_RIGHT:
			jumpLongRight();
			break;
			
		case Action.MOUNT_IDLE:
		case Action.MOUNT_UP:
		case Action.MOUNT_DOWN:
			
			lastFrameHande = false;
			mountJumpStartPos = transform.position;
			jumpFromMount = true;
			justJumpedMount = true;
			
			if( Input.GetKey(keyLeft)){
				jumpLeft();
				return;
			}
			
			if( Input.GetKey(keyRight)){
				jumpRight();
				return;
			}
			
			velocity.x = 0.0f;
			velocity.y = 0.0f;
			setAction(Action.JUMP);
			setState (State.IN_AIR);
			
			break;
			
		case Action.MOUNT_LEFT:
			mountJumpStartPos = transform.position;
			jumpFromMount = true;
			justJumpedMount = true;
			jumpLeft();
			break;
			
		case Action.MOUNT_RIGHT:
			mountJumpStartPos = transform.position;
			jumpFromMount = true;
			justJumpedMount = true;
			jumpRight();
			break;
		};
	}
	
	override int keyJumpUp(){
		jumpFromMount = false;
		justJumpedRope = null;
		canJumpAfter = true;
	}

	int Action_IDLE(){
		return 0;
	}

	int Action_WALK(int dir){
		
		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f ) {
			setAction(Action.IDLE);
			resetActionAndState();
			return 0;
		}
		
		distToMove = velocity.x * CurrentDeltaTime;
		
		animator.speed = 0.5f + (Mathf.Abs( velocity.x ) / WalkSpeed ) * 0.5f;
		
		float distToObstacle = 0.0f;
		if (checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			setActionIdle();
		}
		
		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);
		
		float distToGround = 0.0f;
		bool groundUnderFeet = checkGround (false, layerIdLastGroundTypeTouchedMask, ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
		}
		return 0;
	}
	
	int Action_RUN(int dir){
		
		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f) {
			setAction(Action.IDLE);
			resetActionAndState ();
			return 0;
		}
		
		float speedRatio = (Mathf.Abs (velocity.x) / RunSpeed);
		bool turnBackHard = speedRatio > 0.5f;
		
		if (turnBackHard) {
			
			if (dir == 1) {
				
				if( (Input.GetKeyDown(keyLeft) || Input.GetKey(keyLeft)) &&
				   (Input.GetKeyUp(keyRight) || !Input.GetKey(keyRight))
				   )
				{
					setAction (Action.TURN_RUN_LEFT);
				}
				
			} else if (dir == -1) {
				
				if( (Input.GetKeyDown(keyRight) || Input.GetKey(keyRight)) &&
				   (Input.GetKeyUp(keyLeft) || !Input.GetKey(keyLeft))
				   )
				{
					setAction (Action.TURN_RUN_RIGHT);
				}
				
			}
			
		}
		
		distToMove = velocity.x * CurrentDeltaTime;
		
		animator.speed = 0.5f + (Mathf.Abs( velocity.x ) / RunSpeed ) * 0.5f;
		
		float distToObstacle = 0.0f;
		if (checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			setActionIdle();
		}
		
		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);
		
		float distToGround = 0.0f;
		bool groundUnderFeet = checkGround (false, layerIdLastGroundTypeTouchedMask, ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
		}
		
		return 0;
	}
	
	int Action_TURN_RUN(int dir){
		
		int retVal = 0;
		
		if (Input.GetKeyDown (keyJump)) {
			wantJumpAfter = true;
		}
		
		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f) {
		}
		
		distToMove = velocity.x * CurrentDeltaTime;
		
		float distToObstacle = 0.0f;
		if (checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			retVal = 1;
		}
		
		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);
		
		float distToGround = 0.0f;
		bool groundUnderFeet = checkGround (false, layerIdLastGroundTypeTouchedMask, ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
		}
		
		return retVal;
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
		if( Input.GetKey(keyDown) ){
			tryStartClimbPullDown();
		}
		return 0;
	}
	
	int Action_CROUCH_LEFTRIGHT(int dir){
		
		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f) {
			setActionCrouchIdle();
			if( crouching() ) {
				if( Input.GetKey(keyLeft) ) {
					keyLeftDown();
				} else if( Input.GetKey(keyRight) ){
					keyRightDown();
				}
			}
		}
		
		distToMove = velocity.x * CurrentDeltaTime;
		
		float distToObstacle = 0.0f;
		if (checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			setActionCrouchIdle();
		}
		
		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);
		
		float distToGround = 0.0f;
		bool groundUnderFeet = checkGround (false, layerIdLastGroundTypeTouchedMask, ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
		}
		
		return 0;
	}
	
	int Action_MOUNTING(){
		Vector3 newPos3 = transform.position;
		Vector3 distToMount = velocity * CurrentDeltaTime;
		newPos3 += distToMount;
		if (onMount (newPos3)) {
			transform.position = newPos3;
		} else {
			setMountIdle();
		}
		return 0;
	}
	
	int Action_MOUNTING_DOWN(){
		Vector3 newPos3 = transform.position;
		Vector3 distToMount = velocity * CurrentDeltaTime;
		newPos3 += distToMount;
		
		if (distToMount.y < 0.0f) { // schodzi
			groundUnderFeet = checkDown ( Mathf.Abs(distToMount.y) + 0.01f);
			if (groundUnderFeet >= 0.0f) {
				if (groundUnderFeet < Mathf.Abs (distToMount.y)) {
					distToMount.y = -groundUnderFeet;
					velocity.x = 0.0f;
					velocity.y = 0.0f;
					setState (State.ON_GROUND);
					setAction (Action.IDLE);
					transform.position = transform.position + distToMount;
				}
			}else{
				if( onMount(newPos3) )
					transform.position = newPos3;
				else
					setMountIdle();
			}
		}
		return 0;
	}


	
	int Action_ROPECLIMB_IDLE(float deltaTime){
		
		if (!catchedRope)
			return 0;
		
		bool _swing = false;
		
		if (Input.GetKey (keyLeft)) {
			
			float fla = catchedRope.firstLinkAngle;
			
			if( fla > -20f && fla < 0f){
				//print ( "Rope swing : " + fla );
				catchedRope.swing(-Vector2.right, RopeSwingForce * CurrentDeltaTime );
				_swing = true;
			}
			
			//if( _swing ){
			if( dir () == Vector2.right ){
				
				if( faceRight() ) animator.Play("Zap_liana_swingback_R");
				else animator.Play("Zap_liana_swingback_L");
				animator.speed = 1f;
				
			}else{
				
				if( faceRight() ) animator.Play("Zap_liana_swingfront_R");
				else animator.Play("Zap_liana_swingfront_L");
				animator.speed = 1f;
				
			}
			//}
		}
		else if (Input.GetKey (keyRight)) {
			
			float fla = catchedRope.firstLinkAngle;
			
			if( fla < 20f && fla >= 0f){
				//print ( "Rope swing : " + fla );
				catchedRope.swing(Vector2.right, RopeSwingForce * CurrentDeltaTime );
				_swing = true;
			}
			
			//if( _swing ){
			if( dir () == Vector2.right ){
				
				if( faceRight() ) animator.Play("Zap_liana_swingfront_R");
				else animator.Play("Zap_liana_swingfront_L");
				animator.speed = 1f;
				
			}else{
				
				if( faceRight() ) animator.Play("Zap_liana_swingback_R");
				else animator.Play("Zap_liana_swingback_L");
				animator.speed = 1f;
				
			}
			//}
		}
		
		if (Input.GetKeyUp (keyLeft) || Input.GetKeyUp(keyRight) ) { //|| !_swing) {
			setActionRopeClimbIdle();
		}
		
		if (tryJumpFromRope () != 0) {
			return 0;
		}
		
		if (Input.GetKey (keyUp)) { 
			
			if( canRopeClimbUp() ){
				setAction(Action.ROPECLIMB_UP);
			}
			
		} else if (Input.GetKey (keyDown)) {
			
			if( canRopeClimbDown() ) {
				setAction(Action.ROPECLIMB_DOWN);
			}
		}
		
		tryBreakUpRope (deltaTime);
		
		return 0;
	}
	int Action_LANDING_HARD(){
		if (currentActionTime >= LANDING_HARD_DURATION) {
			setAction(Action.IDLE);
			resetActionAndState();
		}
		
		return 0;
	}
	
	int Action_CLIMB_PULLDOWN(){
		climbDuration += CurrentDeltaTime;
		
		if( climbDuration >= CLIMBDUR_CLIMB ){
			setAction(Action.CLIMB_CATCH,1);
			setState(State.CLIMB);
			climbDuration = 0.0f;
			canPullUp = true;
			transform.position = climbAfterPos;
		} else {
		}
		
		return 0;
	}
	
	int Action_CLIMB_JUMP_TO_CATCH(){
		// dociaganie do punktu:
		climbDuration += CurrentDeltaTime;
		
		if (climbDuration >= climbToJumpDuration) {
			setAction (Action.CLIMB_CATCH);
			climbDuration = 0.0f;
			transform.position = climbAfterPos;
		} else {
			float ratio = climbDuration / climbToJumpDuration;
			transform.position = climbBeforePos + climbDistToClimb * ratio;
		}
		
		return 0;
	}
	
	int Action_CLIMB_CATCH(){
		if ( (Input.GetKeyDown (keyUp) || Input.GetKey(keyUp)) && canPullUp) {
			
			climbAfterPos.x = catchedClimbHandle.transform.position.x;
			climbAfterPos.y = catchedClimbHandle.transform.position.y;
			
			climbBeforePos = transform.position;
			climbDistToClimb = climbAfterPos - climbBeforePos;
			
			setAction (Action.CLIMB_CLIMB);
			climbDuration = 0.0f;
			
			catchedClimbHandle = null;
			lastCatchedClimbHandle = null;
		} else if ( Input.GetKeyDown (keyJump)) {
			if (dir () == Vector2.right && Input.GetKey (keyLeft)) {
				turnLeft ();
				jumpLeft ();
				catchedClimbHandle = null;
				lastCatchedClimbHandle = null;
			} else if (Input.GetKey (keyRight)) {
				turnRight ();
				jumpRight ();
				catchedClimbHandle = null;
				lastCatchedClimbHandle = null;
			} else if( Input.GetKey(keyDown) ){
				velocity.x = 0.0f;
				velocity.y = 0.0f;
				setState (State.IN_AIR);
				setAction (Action.JUMP);
				lastCatchedClimbHandle = catchedClimbHandle;
				catchedClimbHandle = null;
			} else {
				jumpFromClimb ();
				lastCatchedClimbHandle = catchedClimbHandle;
				catchedClimbHandle = null;
			}
		}
		
		return 0;
	}
	
	int Action_CLIMB_CLIMB(){
		climbDuration += CurrentDeltaTime;
		
		if (climbDuration >= CLIMBDUR_CLIMB) {
			setState (State.ON_GROUND);
			climbDuration = 0.0f;
			transform.position = climbAfterPos; 
			
			if( canGetUp() ){
				setAction (Action.IDLE);
				resetActionAndState ();
			}else{
				setAction (Action.CROUCH_IDLE);
				wantGetUp = !Input.GetKey(keyDown);
				
				if( Input.GetKey(keyLeft) ) {
					keyLeftDown();
				} else if( Input.GetKey(keyRight) ){
					keyRightDown();
				}
			}
			
		} else {
			float ratio = climbDuration / CLIMBDUR_CLIMB;
			transform.position = climbBeforePos + climbDistToClimb * ratio;
		}
		
		return 0;
	}

	int Action_ROPECLIMB_UP(float deltaTime){
		
		if (!catchedRope)
			return 0;
		
		if (Input.GetKeyUp (keyUp)) { 
			setAction(Action.ROPECLIMB_IDLE);
			return 0;
		} 
		
		float climbDist = RopeClimbSpeedUp * CurrentDeltaTime;
		
		float newRopeLinkCatchOffset = ropeLinkCatchOffset + climbDist;
		// zakladam ze nie przebedzie wiecej niz jednego ogniwa w klatce...
		
		if( newRopeLinkCatchOffset > 0.0f ) // przekroczyłem ogniwo w gore...
		{
			if( catchedRopeLink.transform.parent ) { // jak ogniwo ma rodzica to przechodze wyzej 
				
				catchedRopeLink = catchedRopeLink.transform.parent.GetComponent<RopeLink>();
				catchedRope.chooseDriver(catchedRopeLink.transform);
				ropeLinkCatchOffset = -0.5f - newRopeLinkCatchOffset;
				
			}else {
				ropeLinkCatchOffset = 0.0f;
				setAction(Action.ROPECLIMB_IDLE);
			}
			
		} else {
			
			ropeLinkCatchOffset = newRopeLinkCatchOffset;
		}
		
		tryBreakUpRope (deltaTime);
		
		return 0;
	}
	
	int Action_ROPECLIMB_DOWN(float deltaTime){
		
		if (!catchedRope)
			return 0;
		
		if (Input.GetKeyUp (keyDown)) {
			setAction(Action.ROPECLIMB_IDLE);
			return 0;
		}
		
		if (tryJumpFromRope () != 0) {
			return 0;
		}
		
		float climbDist = RopeClimbSpeedDown * CurrentDeltaTime;
		
		float newRopeLinkCatchOffset = ropeLinkCatchOffset - climbDist;
		// zakladam ze nie przebedzie wiecej niz jednego ogniwa w klatce...
		
		if( newRopeLinkCatchOffset <= -0.5f ) // przekroczyłem ogniwo w gore...
		{
			if( catchedRopeLink.transform.childCount > 0 ) { // jak ogniwo ma dzicko to przechodze niżej 
				
				if( catchedRopeLink.transform.GetChild(0).transform.childCount > 0 ){ // chyba ze to jest ostatnie ogniwo
					catchedRopeLink = catchedRopeLink.transform.GetChild(0).GetComponent<RopeLink>();
					catchedRope.chooseDriver(catchedRopeLink.transform);
					ropeLinkCatchOffset = newRopeLinkCatchOffset + 0.5f;
				}else{
					ropeLinkCatchOffset = -0.5f;
					setAction(Action.ROPECLIMB_IDLE);
				}
				
			}else {
				ropeLinkCatchOffset = -0.5f;
				setAction(Action.ROPECLIMB_IDLE);
			}
			
		} else {
			
			ropeLinkCatchOffset = newRopeLinkCatchOffset;
		}
		
		tryBreakUpRope (deltaTime);
		
		return 0;
	}

	void crouch(){
		if (isInState (State.ON_GROUND)) {
			
			switch (action) {
				
			case Action.IDLE:
			case Action.JUMP:
			case Action.CROUCH_IN:
				setAction (Action.CROUCH_IDLE);
				if( Input.GetKey(keyLeft) ){
					keyLeftDown();
				} else if( Input.GetKey(keyRight) ){
					keyRightDown();
				}else{
					velocity.x = 0.0f;
					velocity.y = 0.0f;
				}
				break;
				
			case Action.WALK_LEFT:
			case Action.RUN_LEFT:
			case Action.JUMP_LEFT:
			case Action.JUMP_LEFT_LONG:
				if( Input.GetKey(keyLeft)){
					velocity.x = 0.0f;
					desiredSpeedX = CrouchSpeed;
					if( dir () == -Vector2.right ){
						setAction(Action.CROUCH_LEFT);
					}else{
						setAction(Action.CROUCH_LEFT_BACK);
					}
				}else{
					velocity.x = 0.0f;
					velocity.y = 0.0f;
					setAction (Action.CROUCH_IDLE);
				}
				break;
				
			case Action.WALK_RIGHT:
			case Action.RUN_RIGHT:
			case Action.JUMP_RIGHT:
			case Action.JUMP_RIGHT_LONG:
				if( Input.GetKey(keyRight)){
					velocity.x = 0.0f;
					desiredSpeedX = CrouchSpeed;
					if( dir () == Vector2.right ){
						setAction(Action.CROUCH_RIGHT);
					}else{
						setAction(Action.CROUCH_RIGHT_BACK);
					}
				}else{
					velocity.x = 0.0f;
					velocity.y = 0.0f;
					setAction (Action.CROUCH_IDLE);
				}
				break;
			}
			
		}
	}
	
	void preparetojump(){
		if (isNotInState (State.ON_GROUND) || isNotInAction (Action.IDLE))
			return;
		
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		setAction (Action.PREPARE_TO_JUMP);
	}
	
	void jump(){
		addImpulse(new Vector2(0.0f, JumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP);
		
		lastFrameHande = false;
	}
	
	void jumpFromClimb(){
		addImpulse(new Vector2(0.0f, JumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP,1);
		lastFrameHande = false;
	}
	
	void jumpLeft(){
		velocity.x = -JumpSpeed;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, JumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT);
		
		lastFrameHande = false;
	}
	
	void jumpRight(){
		velocity.x = JumpSpeed;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, JumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_RIGHT);
		
		lastFrameHande = false;
	}
	
	void jumpLongLeft(){
		velocity.x = -JumpLongSpeed;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, JumpLongImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT_LONG);
		
		lastFrameHande = false;
	}
	
	void jumpLongRight(){
		velocity.x = JumpLongSpeed;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, JumpLongImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_RIGHT_LONG);
		
		lastFrameHande = false;
	}
	
	void turnLeftStart(){
		setAction (Action.TURN_STAND_LEFT);
		
		if (Input.GetKeyDown (keyJump) || ( Input.GetKey (keyJump) && canJumpAfter) )
			wantJumpAfter = true;
	}
	
	void turnRightStart(){
		setAction (Action.TURN_STAND_RIGHT);
		
		if (Input.GetKeyDown (keyJump) || (Input.GetKey (keyJump) && canJumpAfter) )
			wantJumpAfter = true;
	}
	
	void turnLeftFinish(){
		setAction (Action.IDLE);
		
		if( wantJumpAfter ) {
			jumpLeft();
			
			if( Input.GetKey(keyJump) )
				canJumpAfter = false;
			
		} else {
			resetActionAndState ();
		}
	}
	
	void turnRightFinish(){
		setAction (Action.IDLE);
		
		if( wantJumpAfter) {
			jumpRight();
			
			if( Input.GetKey(keyJump) )
				canJumpAfter = false;
			
			
		} else {
			resetActionAndState ();
		}
	}

	void setActionIdle(){
		velocity.x = 0.0f;
		setAction (Action.IDLE);
	}
	void setActionRopeClimbIdle(){
		if( faceRight() ) animator.Play("Zap_liana_climbup_R");
		else animator.Play("Zap_liana_climbup_L");
		animator.speed = 0f;
	}
	void setActionCrouchIdle(){
		velocity.x = 0.0f;
		setAction (Action.CROUCH_IDLE);
	}
	void setActionMountIdle(){
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		setState(State.MOUNT);
		setAction(Action.MOUNT_IDLE);
		resetActionAndState ();
	}
	bool setMountIdle(){
		if (isInState (State.MOUNT)) {
			velocity.x = 0.0f;
			velocity.y = 0.0f;
			setAction (Action.MOUNT_IDLE);
			
			return true;
		}
		return false;
	}
	
	void resetActionAndState(){
		if (isInState (State.ON_GROUND)) {
			if (Input.GetKey (keyDown)) { //&& (Input.GetKey(keyLeft) || Input.GetKey(keyRight)) ){
				if (!keyDownDown ())
					setActionIdle ();
			} else if (Input.GetKey (keyLeft)) {
				if (!keyLeftDown ())
					setActionIdle ();
			} else if (Input.GetKey (keyRight)) {
				if (!keyRightDown ())
					setActionIdle ();
			} else {
				if (isInState (State.ON_GROUND)) {
					setActionIdle ();
				}
			}
		} else if (isInState (State.MOUNT)) {
			
			if (Input.GetKey (keyDown)) { //&& (Input.GetKey(keyLeft) || Input.GetKey(keyRight)) ){
				if (!keyDownDown ())
					setMountIdle ();
			}else if( Input.GetKey (keyUp)) { //&& (Input.GetKey(keyLeft) || Input.GetKey(keyRight)) ){
				if (!keyUpDown ())
					setMountIdle ();
			} else if (Input.GetKey (keyLeft)) {
				if (!keyLeftDown ())
					setMountIdle ();
			} else if (Input.GetKey (keyRight)) {
				if (!keyRightDown ())
					setMountIdle ();
			} else {
				if (isInState (State.ON_GROUND)) {
					setActionIdle ();
				}
			}
		}
	}

	int walking(){
		if (isInAction (Action.WALK_RIGHT))
			return 1;
		if (isInAction (Action.WALK_LEFT))
			return -1;
		return 0;
	}
	
	int running(){
		if (isInAction (Action.RUN_RIGHT))
			return 1;
		if (isInAction (Action.RUN_LEFT))
			return -1;
		return 0;
	}
	
	bool moving(Vector2 dir){
		if (dir == Vector2.right)
			return isInAction(Action.WALK_RIGHT) || isInAction(Action.RUN_RIGHT);
		else 
			return isInAction(Action.WALK_LEFT) || isInAction(Action.RUN_LEFT);
	}
	bool moving(int dir){
		if (dir == 1)
			return isInAction(Action.WALK_RIGHT) || isInAction(Action.RUN_RIGHT);
		else 
			return isInAction(Action.WALK_LEFT) || isInAction(Action.RUN_LEFT);
	}
	bool jumping(){
		return isInAction(Action.JUMP) || isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG) || isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG);
	}
	bool mounting(){
		return isInAction(Action.MOUNT_LEFT) || isInAction(Action.MOUNT_RIGHT) || isInAction(Action.MOUNT_UP) || isInAction(Action.MOUNT_DOWN);
	}
	bool crouching(){
		return isInAction(Action.CROUCH_IDLE) || 
			isInAction(Action.CROUCH_LEFT) || isInAction(Action.CROUCH_LEFT_BACK) ||
				isInAction(Action.CROUCH_RIGHT) || isInAction(Action.CROUCH_RIGHT_BACK);
	}


	Action action;
	public float CrouchInOutDuration = 0.2f;
	bool justJumpedMount = false;
	float currentActionTime = 0f;
	NewRope justJumpedRope = null;
	Vector3 impulse;
}
