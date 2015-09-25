using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class ZapControllerGravityGun : ZapController {
	
	public float WalkSpeed = 1.5f;
	public float WalkBackSpeed = 1.5f;

	public float rollSpeed = 4.8f;
	public float rollDuration = 0.6f;
	public float rollMaxDist = 3f;
	
	public float GravityForce = -20.0f;
	public float MaxSpeedY = 15.0f;
	
	public float SpeedUpParam = 10.0f; // ile jednosek predkosci hamuje na sekund
	public float SlowDownParam = 20.0f; // ile jednosek predkosci hamuje na sekunde
	
	public float TURN_LEFTRIGHT_DURATION = 0.2f;
	public float ATTACK_DURATION = 0.5f;
	public float PULLOUT_GRAVITYGUN_DURATION = 0.3f;
	public float HIDE_GRAVITYGUN_DURATION = 0.35f;
	public float CROUCHINOUT_DURATION = 0.1f;

	public Transform draggedStone = null;
	public Transform lastFlashStone = null;
	//public int layerIdGroundMoveableMask = 0;
	//public int layerIdGroundMask = 0;

	Vector2 T; 			// sila ciagu
	public float inertiaFactor = 0.09f; 		// wspolczynnik oporu - u mnie raczej bezwladnosci
	//public float inertiaFactor2 = 0.03f; 	// wspolczynnik bezwladnosci jak gracz na siebie chce skierowac kamien
	public float maxDistance = 8f;
	//public float minDistance = 2f;
	//public float pushOutForce = 2f;
	//public float pushOutMassFactor = 10f;
	
	//List<Rigidbody2D> droppedStones = new List<Rigidbody2D> ();
	
	Vector2 V; 			// predkosc
	public static float userStoneRotateSpeed = 180f;

//	public ZapControllerGravityGun () 
//		: base("GravityGun")
//	{
//		//zap.layer
//	}

	public override void setZap(Zap playerController){
		base.setZap (playerController);
		if (zap.weaponMenu) {
			weaponMenuItem = zap.weaponMenu.itemGravityGun;
		}
		if (weaponMenuItem) {
			weaponMenuItem.setState(WeaponMenuItem.State.OFF);
		}
	}

	float distToMove;
	Vector3 oldPos;
	float newPosX;
	
	Vector3 climbBeforePos;
	Vector3 climbAfterPos;
	Vector3 climbDistToClimb;
	float climbToJumpDuration;
	
	float groundUnderFeet;

	void leftMouseNotPressed(){

		if (zap.isDead ())
			return;

		if (draggedStone == null) {

			if (lastFlashStone) {
				unflashStone (lastFlashStone);
				lastFlashStone = null;
			}
			//Camera.main.
								
			Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
			
			Vector2 rayOrigin = zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
			Vector3 _df = mouseInScene - rayOrigin;
							
			if( _df.magnitude <= maxDistance ){

				RaycastHit2D hit = Physics2D.Linecast (mouseInScene, mouseInScene, zap.layerIdGroundMoveableMask);
				if( hit.collider ){

					lastFlashStone = hit.collider.gameObject.transform;
					if( lastFlashStone ){
						Rigidbody2D tsrb = lastFlashStone.GetComponent<Rigidbody2D>();
						if( tsrb ){

							//rayOrigin = player.dir() == Vector2.right ? player.sensorRight2.position : player.sensorLeft2.position;

							hit = Physics2D.Linecast (rayOrigin, tsrb.worldCenterOfMass, zap.layerIdGroundMask);
							if( hit.collider ){
								lastFlashStone = null;
							}else{
								flashStone(lastFlashStone);
							}

						}else{
							lastFlashStone = null;
						}
					}
				}
			}
		}
	}

	void leftMouseButtonClicked(){			
		if (zap.isDead ())
			return;

		draggedStone = null;

		Vector3 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
		
		RaycastHit2D hit = Physics2D.Linecast( mouseInScene, mouseInScene, zap.layerIdGroundMoveableMask );
		if( hit.collider ){
			draggedStone = hit.collider.gameObject.transform;

			if( canBeDragged(draggedStone) ){

				Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
				tsrb.gravityScale = 0f;
				flashStone(draggedStone);

			} else {
				draggedStone = null;
			}
		}
	}

	public override void MUpdate (float deltaTime) {	
		//Debug.Log ("ZapContrllerNormal::Update : " + deltaTime);
		
		//currentActionTime = zap.getCurrentActionTime();
		
		oldPos = transform.position;
		newPosX = oldPos.x;
		distToMove = 0.0f;
		
		//checkStartAttack ();
		//checkStartCrouchAttack ();

		if( !Input.GetMouseButton(0) ){ 
			leftMouseNotPressed();
		}

		if (Input.GetMouseButtonDown (0)) {
			leftMouseButtonClicked();
		}

		if (Input.GetMouseButtonUp (0)) {
			releaseStone();
		}

		switch (action) {
		case Action.IDLE:
			if( Action_IDLE() != 0 )
				return;
			break;
			
		case Action.PULLOUT_GRAVITYGUN:
			Action_PULLOUT_GRAVITYGUN();
			break;
			
		case Action.HIDE_GRAVITYGUN:
			Action_HIDE_GRAVITYGUN();
			break;

		case Action.WALK_LEFT:
		case Action.WALKBACK_LEFT:
			Action_WALK(-1);
			break;
			
		case Action.WALK_RIGHT:
		case Action.WALKBACK_RIGHT:
			Action_WALK(1);
			break;
			
		case Action.ROLL_LEFT_BACK:
		case Action.ROLL_LEFT_FRONT:
			Action_ROLL(-1);
			break;
			
		case Action.ROLL_RIGHT_BACK:
		case Action.ROLL_RIGHT_FRONT:
			Action_ROLL(1);
			break;
			
		case Action.TURN_STAND_LEFT:
			if (Input.GetKeyDown (zap.keyJump)) {
				wantJumpAfter = true;
			}
			if( zap.currentActionTime >= TURN_LEFTRIGHT_DURATION ){
				zap.turnLeft();
				turnLeftFinish();
			}
			break;
			
		case Action.TURN_STAND_RIGHT:
			if (Input.GetKeyDown (zap.keyJump)) {
				wantJumpAfter = true;
			}
			if( zap.currentActionTime >= TURN_LEFTRIGHT_DURATION ){
				zap.turnRight();
				turnRightFinish();
			}
			break;
		}
		
		switch (zap.getState()) {
			
		case Zap.State.IN_AIR:
			
			
			zap.AddImpulse(new Vector2(0.0f, GravityForce * deltaTime));
			
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
			
			Vector3 distToFall = new Vector3();
			distToFall.x = zap.velocity.x * deltaTime;
			
			if( distToFall.x > 0.0f ){
				float obstacleOnRoad = zap.checkRight(distToFall.x + 0.01f,!zap.stateJustChanged);
				if( obstacleOnRoad >= 0.0f ){
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = obstacleOnRoad;
						zap.velocity.x = 0.0f;
					}
				}
			}else if( distToFall.x < 0.0f ){
				float obstacleOnRoad = zap.checkLeft( Mathf.Abs(distToFall.x) + 0.01f,!zap.stateJustChanged);
				if( obstacleOnRoad >= 0.0f ){
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = -obstacleOnRoad;
						zap.velocity.x = 0.0f;
					}
				}
			}
			
			transform.position = transform.position + distToFall;
			distToFall.x = 0f;
			
			zap.velocity.y += zap.GetImpulse().y;
			if(zap.velocity.y > MaxSpeedY)
				zap.velocity.y = MaxSpeedY;
			if(zap.velocity.y < -MaxSpeedY)
				zap.velocity.y = -MaxSpeedY;
			
			distToFall.y = zap.velocity.y * deltaTime;
			
			bool justLanding = false;
			
			if( distToFall.y > 0.0f ) { // leci w gore
				//transform.position = transform.position + distToFall;
			} else if( distToFall.y < 0.0f ) { // spada
				if( zap.lastVelocity.y >= 0.0f ) { // zaczyna spadac
					// badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
					zap.startFallPos = transform.position;
					//print ( "zap.startFallPos : " + zap.startFallPos );
					if( zap.lastVelocity.y > 0.0f ){
						//lastCatchedClimbHandle = null;
					}
				}
				groundUnderFeet = zap.checkDown( Mathf.Abs(distToFall.y) + 0.01f);
				if( groundUnderFeet >= 0.0f ){
					if( (groundUnderFeet < Mathf.Abs(distToFall.y)) || Mathf.Abs( groundUnderFeet - Mathf.Abs(distToFall.y)) < 0.01f  ){
						//lastCatchedClimbHandle = null;
						distToFall.y = -groundUnderFeet;
						justLanding = true;
					}
				}
			}
			
			transform.position = transform.position + distToFall;
			
			if( justLanding ){
				
				if( zap.landingSound )
					zap.getAudioSource().PlayOneShot( zap.landingSound );
				
				zap.setFuddledFromBrid( false );
				
				zap.setState(Zap.State.ON_GROUND);
				zap.velocity.y = 0.0f;
				
				Vector3 fallDist = zap.startFallPos - transform.position;
				
				//				if( fallDist.y >= VeryHardLandingHeight ){
				//					zap.die(Zap.DeathType.VERY_HARD_LANDING);
				//				} else if( fallDist.y >= HardLandingHeight ){
				//					
				//					zap.velocity.x = 0.0f;
				//					setAction (Action.LANDING_HARD);
				//					
				//				}else{
				
				resetActionAndState();
				
				//}
			}
			
			break;
			
		case Zap.State.ON_GROUND:
			float distToGround = 0.0f;
			bool groundUnderFeet2 = zap.checkGround (true, zap.layerIdGroundAllMask, ref distToGround);
			if (groundUnderFeet2) {
				
			}else{
				zap.setState(Zap.State.IN_AIR);
				//setAction(Action.JUMP);
				//wantGetUp = false;
			}
			
			break;
			
		};
		
		zap.lastVelocity = zap.velocity;
		
	}
	
	public override void FUpdate(float fDeltaTime){
		if( draggedStone ){ 
			if( Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) ||
			   Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X) ){

				Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				if( rb ){
					rb.angularVelocity = 0;
				}
			}

			if( Input.GetKey(KeyCode.Z) ){ // obracam kamien w lewo ...

				Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				if( rb ){

					if( rb.angularVelocity < 180 )
						rb.angularVelocity += ( fDeltaTime * userStoneRotateSpeed );

					rb.angularVelocity = Mathf.Min( rb.angularVelocity, 180f);

					//rb.rotation += ( fDeltaTime * userStoneRotateSpeed );
				}
			}
			else if( Input.GetKey(KeyCode.X) ){ // albo w prawo

				Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				if( rb ){

					if( rb.angularVelocity > -180 )
						rb.angularVelocity -= ( fDeltaTime * userStoneRotateSpeed );

					rb.angularVelocity = Mathf.Max( rb.angularVelocity, -180f);

					//rb.rotation -= ( fDeltaTime * userStoneRotateSpeed );
				}

			}
		}

		Vector3 currentMousePosition = Input.mousePosition;
			
//		for (int i = 0 ; i < droppedStones.Count; ++i) {
//			Rigidbody2D rb = droppedStones[i];
//			if( rb.IsSleeping() ){
//				//Debug.Log ( "remove dropped stone: " + rb ); 
//				droppedStones.Remove(rb);
//			}
////			}else{
////				Vector2 playerCenterPos = zap.transform.position;
////				playerCenterPos.y += 1f;
////				Vector2 stoneCenterPos = rb.worldCenterOfMass;
////						
////				Vector2 diff = stoneCenterPos - playerCenterPos;
////				Vector2 F = new Vector2(0f,0f);
////				float diffMagnitude = diff.magnitude;
////						
////				if( diffMagnitude < minDistance+0.25f ){
////					//F = diff + diff * pushOutForce * (rb.mass / pushOutMassFactor);
////					//F = diff.normalized * (rb.velocity.magnitude / 10f) * 20f * (rb.mass / pushOutMassFactor);
////
////					// im blizej srodka i im szybciej tym mocniej wypycha
////					F = diff * (diffMagnitude/minDistance) * (rb.velocity.magnitude / 10f) * 20f * (rb.mass / pushOutMassFactor);
////					rb.AddForce(F,ForceMode2D.Impulse);
////				}
////			}
//		}
			
		if( Input.GetMouseButton(0) ){
			Vector3 touchInScene = touchCamera.ScreenToWorldPoint(currentMousePosition);
			Vector2 tis = touchInScene;
	
			if( draggedStone ){
				Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				if( rb ){
					Vector2 playerCenterPos = zap.transform.position;
					playerCenterPos.y += 1f;
					Vector2 stoneCenterPos = rb.worldCenterOfMass;
	
					Vector2 diff = stoneCenterPos - playerCenterPos;
					Vector2 F = new Vector2(0f,0f);
	
					float diffMagnitude = diff.magnitude;
					//if( diffMagnitude < minDistance+0.25f ){
					//	F = diff + diff * ( diffMagnitude / minDistance ) * pushOutForce * (rb.mass / pushOutMassFactor);
					//}else{
						Vector2 diff2 = tis - playerCenterPos;
						float diffMagnitude2 = diff2.magnitude;

						//if( diffMagnitude2 > minDistance ){

							T = (tis - stoneCenterPos);
							V = rb.velocity;

							F = T - (inertiaFactor * V);
						//}
//						}else{ // jednak musi przyciagac ale slabiej albo do granicy a nie 
//							T = (tis - stoneCenterPos);
//							V = rb.velocity;							
//							F = T - (inertiaFactor2 * V) ;
//							F *= (rb.mass / pushOutMassFactor);
//						}
					//}
	
					//Debug.Log("F : " + rb.velocity);
					rb.AddForce(F,ForceMode2D.Impulse);
	
					if( !canBeDragged( draggedStone, tis) ){
						releaseStone();
					}
	
				}
			}
		}
	}
	
	public override void activate(){
		base.activate ();

		//setAction (Action.IDLE);
		setAction (Action.PULLOUT_GRAVITYGUN);
		//canPullUp = false;
		desiredSpeedX = 0.0f;
	}
	public override void deactivate(){
		base.deactivate ();
	}

	public override bool tryDeactiveate(){
		if( isInAction(Action.IDLE) ){
			setAction(Action.HIDE_GRAVITYGUN);
			return true;
		}
		return false;
	}

	public enum Action{
		UNDEF = 0,
		IDLE,
		PULLOUT_GRAVITYGUN,
		HIDE_GRAVITYGUN,
		WALK_LEFT,
		WALK_RIGHT,
		WALKBACK_LEFT,
		WALKBACK_RIGHT,
		TURN_STAND_LEFT,
		TURN_STAND_RIGHT,
		JUMP,
		ROLL_LEFT_FRONT,
		ROLL_LEFT_BACK,
		ROLL_RIGHT_FRONT,
		ROLL_RIGHT_BACK,
		FALL,
		STOP_WALK,
		//STOP_RUN,
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
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_knife_idle");
			else zap.getAnimator().Play ("Zap_knife_idle");
			
			break;
			
		case Action.PULLOUT_GRAVITYGUN:
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_knife_pull");
			else zap.getAnimator().Play ("Zap_knife_pull");
			break;
			
		case Action.HIDE_GRAVITYGUN:
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_knife_hide");
			else zap.getAnimator().Play ("Zap_knife_hide");
			break;
			
		case Action.DIE:
			Zap.DeathType dt = (Zap.DeathType)param;
			string msgInfo = "";
			
			switch( dt ){

			case Zap.DeathType.STONE_HIT:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_stonehit_R");
				else zap.getAnimator().Play("Zap_death_stonehit_L");
				msgInfo = zap.DeathByStoneHitText;
				break;

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

			case Zap.DeathType.PANTHER:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_panther");
				else zap.getAnimator().Play("Zap_death_panther");
				msgInfo = zap.DeathByPantherText;
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

		case Action.ROLL_LEFT_FRONT:
			zap.getAnimator().Play("Zap_knife_crouch_tumblefront");
			break;
			
		case Action.ROLL_LEFT_BACK:
			zap.getAnimator().Play("Zap_knife_crouch_tumbleback");
			break;
			
		case Action.ROLL_RIGHT_FRONT:
			zap.getAnimator().Play("Zap_knife_crouch_tumblefront");
			break;
			
		case Action.ROLL_RIGHT_BACK:
			zap.getAnimator().Play("Zap_knife_crouch_tumbleback");
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
		return 0;
	}
	
	public override int keyDownDown(){
		return 0;
	}
	
	public override int keyDownUp(){
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
		} 
		return 0;
	}
	
	public override int keyLeftUp(){
		
		if (isInState (Zap.State.ON_GROUND)){
			desiredSpeedX = 0.0f;
		}
		
		return 0;
	}
	
	public override int keyRightUp(){
		
		if (isInState (Zap.State.ON_GROUND)) {
			desiredSpeedX = 0.0f;
		}
		
		return 0;
	}

	public override int keyJumpDown(){
		
		//Debug.Log ("ZapControllerNormal::keyJumpDown()");
		//jumpKeyPressed = true;
		
		switch (action) {
		case Action.IDLE:
			if (isInState (Zap.State.ON_GROUND)) {
				//preparetojump ();
			}
			break;
		}
		
		if (isNotInState (Zap.State.ON_GROUND))
			return 0;
		
		if ( isInAction (Action.IDLE) || walking () != 0){
			if (Input.GetKey (zap.keyLeft)) {
				//jumpLeft();
				rollLeft();
				return 1;
			}
			if (Input.GetKey (zap.keyRight)) {
				//jumpRight();
				rollRight();
				return 1;
			}
			return 0;
		}
		
		if (crouching ()) {
			if (Input.GetKey (zap.keyLeft)) {
				rollLeft();
				return 1;
			}
			if (Input.GetKey (zap.keyRight)) {
				rollRight();
				return 1;
			}
			return 0;
		}
		
		return 0;
	}
	
	public override int keyJumpUp(){
		//jumpKeyPressed = false;
		canJumpAfter = true;
		return 0;
	}
	
	bool checkDir(){
		Vector2 mouseInScene = touchCamera.ScreenToWorldPoint (Input.mousePosition);
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
		
		if (Input.GetMouseButtonDown (1)) {
			//			zap._hideKnife();
			//			return 1;
			
			setAction(Action.HIDE_GRAVITYGUN);
			return 0;
		}
		
		checkDir ();
		
		return 0;
	}
	
	int Action_PULLOUT_GRAVITYGUN(){
		if (zap.currentActionTime > PULLOUT_GRAVITYGUN_DURATION) {
			//setAction(Action.ATTACK,1);
			setActionIdle();
			return 1;
		}
		return 0;
	}
	
	int Action_HIDE_GRAVITYGUN(){
		if (zap.currentActionTime > HIDE_GRAVITYGUN_DURATION) {
			//zap._hideGravityGun();
			zap.hideChoosenWeapon();
			return 1;
		}
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
	
	int Action_ROLL(int dir){
		if (zap.currentActionTime >= rollDuration) {
			setAction(Action.IDLE);
			resetActionAndState();
			return 0;
		}
		
		distToMove = zap.velocity.x * zap.getCurrentDeltaTime();
		
		float distToObstacle = 0.0f;
		if (zap.checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			//setActionIdle();
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

	void rollLeft(){
		zap.velocity.x = -rollSpeed;
		zap.velocity.y = 0.0f;
		if( !zap.faceRight() )
			setAction (Action.ROLL_LEFT_FRONT);
		else
			setAction (Action.ROLL_LEFT_BACK);
	}
	
	void rollRight(){
		zap.velocity.x = rollSpeed;
		zap.velocity.y = 0.0f;
		if( zap.faceRight() )
			setAction (Action.ROLL_RIGHT_FRONT);
		else
			setAction (Action.ROLL_RIGHT_BACK);
	}
	
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
	
	int walking(){
		if (isInAction(Action.WALK_RIGHT) || isInAction(Action.WALKBACK_RIGHT))
			return 1;
		if (isInAction(Action.WALK_LEFT) || isInAction(Action.WALKBACK_LEFT))
			return -1;
		return 0;
	}
	
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
		//return isInAction(Action.JUMP) || isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_RIGHT);
		return false;
	}
	public override void zapDie (Zap.DeathType deathType){
		base.zapDie (deathType);
		releaseStone ();
		setAction (Action.DIE, (int)deathType);
	}
//	public override void reborn(){
//		if (zap.getLastTouchedCheckPoint().GetComponent<CheckPoint> ().startMounted) {
//			zap.setState(Zap.State.MOUNT);
//		}
//	}
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
		
		if (!isInState (Zap.State.ON_GROUND) || !(isInAction (Action.IDLE) ) ) //|| isInAction(Action.CROUCH_IDLE)) )
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
	
	bool catchStone(Transform stone){
		return false;
	}

	void releaseStone(){
		if( draggedStone ){
			Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
			if( tsrb ){
				
				//Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				tsrb.gravityScale = 1f;
				//rb.AddForce( lastToMoveDist, ForceMode2D.Impulse );
			}
			unflashStone(draggedStone);
			
			//Debug.Log ( "add dropped stone: " + tsrb );
			//droppedStones.Add( tsrb );
			draggedStone = null;
		}
	}

	void flashStone(Transform stone){
		setStoneOpacity (stone, 0.5f);
	}
	void unflashStone(Transform stone){
		setStoneOpacity (stone, 1.0f);
	}
	void setStoneOpacity(Transform stone, float newOpacity){
		SpriteRenderer sr = stone.GetComponent<SpriteRenderer> ();
		if (!sr)
			return;
		
		Color stoneColor = sr.color;
		stoneColor.a = newOpacity;
		sr.color = stoneColor;
	}


	bool canBeDragged(Transform stone, Vector2 stoneTargetPlace){
		
		Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
		if (!rb)
			return false;
		
		if( (rb.worldCenterOfMass - stoneTargetPlace).magnitude > 5f ){
			return false;
		}
		
		return canBeDragged (stone);
	}

	bool canBeDragged(Transform stone){
		
		Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
		if (!rb)
			return false;
		
		Vector2 rayOrigin = zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
		Vector3 _df = rb.worldCenterOfMass - rayOrigin;
		
		if( _df.magnitude > maxDistance ){
			
			return false;
			
		} else {
			
			RaycastHit2D hit = Physics2D.Linecast (rayOrigin, rb.worldCenterOfMass, zap.layerIdGroundMask);
			if( hit.collider ){
				return false;
			}
		}
		
		return true;
	}

//bool wantGetUp = false;
bool wantJumpAfter = false;
	bool canJumpAfter = true;
	float desiredSpeedX = 0.0f;
	Action action;
}
