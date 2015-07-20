using UnityEngine;
using System.Collections;

public class Player2Controller : MonoBehaviour {

//	public float WalkSpeed = 3.0f;
//	public float RunSpeed = 5.0f;
//	public float JumpSpeed = 3.5f;
//	public float JumpLongSpeed = 4.1f;
//	public float CrouchSpeed = WalkSpeed*0.5f;
//	public float MountSpeed = 2.0f; // ile na sek.
//	public float MountJumpDist = 4.0f; // następnie naciskasz spacje a on skacze
//	public float SpeedUpParam = 7.0f; // ile jednosek predkosci hamuje na sekund
//	public float SlowDownParam = WalkSpeed*2.0f; // ile jednosek predkosci hamuje na sekunde
//	public float FlyUserControlParam = 8.0f; // ile przyspiesza na sekunde lecac
//	public float FlySlowDownParam = 5.0f; // ile hamuje na sekunde lecac
//	
//	public float JumpImpulse = 7.0f; 
//	public float JumpLongImpulse = 7.15f; 
//	public float GravityForce = -20.0f;
//	public float MaxSpeedY = 15.0f;

	public float WalkSpeed = 3.0f;
	public float RunSpeed = 4.0f;
	public float JumpSpeed = 3.5f;
	public float JumpLongSpeed = 4.1f;
	public float CrouchSpeed = 1.5f;
	public float MountSpeed = 2.0f; // ile na sek.
	public float MountJumpDist = 4.0f; // następnie naciskasz spacje a on skacze
	//[2015-06-18 17:58:40] Rafał Sankowski: i jeśli nadal trzymasz spacje
	//[2015-06-18 17:59:02] Rafał Sankowski: to po przeskoczeniu ustalonej wartości (mowilismy o tym) się lapie
	public float SpeedUpParam = 7.0f; // ile jednosek predkosci hamuje na sekund
	/// <summary>
	/// ile jednosek predkosci hamuje na sekunde
	/// </summary>
	public float SlowDownParam = 6.0f; // ile jednosek predkosci hamuje na sekunde
	public float FlyUserControlParam = 8.0f; // ile przyspiesza na sekunde lecac
	public float FlyUpUserControlParam = 9.0f; // ile przyspiesza na sekunde lecac
	public float FlySlowDownParam = 5.0f; // ile hamuje na sekunde lecac

	public float JumpImpulse = 7.0f; 
	public float JumpLongImpulse = 7.15f; 
	public float GravityForce = -20.0f;
	public float MaxSpeedY = 15.0f;

	public float HardLandingHeight = 3.0f;
	public float VeryHardLandingHeight = 6.0f;

	public float RopeSwingForce = 500f;
	public float RopeClimbSpeed = 1.0f;

	public float CLIMB_DURATION = 1.5f;
	public float CLIMBDUR_PREPARE_TO_JUMP = 0.5f;
	public float CLIMBDUR_JUMP_TO_CATCH = 0.2f; // jednostka w 0.2f
	public float CLIMBDUR_CATCH = 0.5f;
	public float CLIMBDUR_CLIMB = 0.75f;
	public float LANDING_HARD_DURATION = 0.5f;
	
	public KeyCode keyLeft = KeyCode.LeftArrow;
	public KeyCode keyRight = KeyCode.RightArrow;
	public KeyCode keyRun = KeyCode.LeftShift;
	public KeyCode keyUp = KeyCode.UpArrow;
	public KeyCode keyDown = KeyCode.DownArrow;
	public KeyCode keyJump = KeyCode.Space;

	public Transform respawnPoint;

	void Awake(){
		coll = GetComponent<BoxCollider2D> ();
		gfx  = transform.Find("gfx").transform;
		animator = transform.Find("gfx").GetComponent<Animator>();

		sensorLeft1 = transform.Find("sensorLeft1").transform;
		sensorLeft2 = transform.Find("sensorLeft2").transform;
		sensorLeft3 = transform.Find("sensorLeft3").transform;
		sensorRight1 = transform.Find("sensorRight1").transform;
		sensorRight2 = transform.Find("sensorRight2").transform;
		sensorRight3 = transform.Find("sensorRight3").transform;
		sensorDown1 = transform.Find("sensorDown1").transform;
		sensorDown2 = transform.Find("sensorDown2").transform;
		sensorDown3 = transform.Find("sensorDown3").transform;

		sensorHandleL2 = transform.Find("handlerL2").transform;
		sensorHandleR2 = transform.Find("handlerR2").transform;

		layerIdGroundMask = 1 << LayerMask.NameToLayer("Ground");
		layerIdGroundPermeableMask = 1 << LayerMask.NameToLayer("GroundPermeable");
		layerIdGroundAllMask = layerIdGroundMask | layerIdGroundPermeableMask;
		layerIdLastGroundTypeTouchedMask = layerIdGroundMask;
		
		layerIdGroundHandlesMask = 1 << LayerMask.NameToLayer("GroundHandles");
		
		layerIdMountMask = 1 << LayerMask.NameToLayer("Mount");
		layerIdRopesMask = 1 << LayerMask.NameToLayer("Ropes");

//		CLIMB_DURATION = 1.5f;
//		CLIMBDUR_PREPARE_TO_JUMP = 0.5f;
//		CLIMBDUR_JUMP_TO_CATCH = 0.2f; // jednostka w 0.2f
//		CLIMBDUR_CATCH = 0.5f;
//		CLIMBDUR_CLIMB = 0.75f;
		
//		WalkSpeed = 3.0f;
//		RunSpeed = 5.0f;
//		JumpSpeed = 3.5f;
//		JumpLongSpeed = 4.1f;
//		CrouchSpeed = WalkSpeed * 0.5f;
//		MountSpeed = 2.0f; // ile na sek.
//		MountJumpDist = 4.0f; // następnie naciskasz spacje a on skacze
//		SpeedUpParam = 7.0f; //WALK_SPEED; // ile jednosek predkosci przyspiesza na sekunde - teraz do pelnej predkosci chodu w 1.s
//		SlowDownParam = WalkSpeed * 2.0f; // ile jednosek predkosci hamuje na sekunde - teraz z automatu w 0.5 sek.
//		FlyUserControlParam = 8.0f; // ile przyspiesza na sekunde lecac
//		FlySlowDownParam = 5.0f; // ile hamuje na sekunde lecac
//
//		JumpImpulse = 7.0f;
//		JumpLongImpulse = 7.15f;
//		GravityForce = -20.0f;
//		MaxSpeedY = 15.0f;


//		public float WalkSpeed = 3.0f;
//		public float RunSpeed = 5.0f;
//		public float JumpSpeed = 3.5f;
//		public float JumpLongSpeed = 4.1f;
//		public float CrouchSpeed = WalkSpeed*0.5f;
//		public float MountSpeed = 2.0f; // ile na sek.
//		public float MountJumpDist = 4.0f; // następnie naciskasz spacje a on skacze
//		public float SpeedUpParam = 7.0f; // ile jednosek predkosci hamuje na sekund
//		public float SlowDownParam = WalkSpeed*2.0f; // ile jednosek predkosci hamuje na sekunde
//		public float FlyUserControlParam = 8.0f; // ile przyspiesza na sekunde lecac
//		public float FlySlowDownParam = 5.0f; // ile hamuje na sekunde lecac
//		
//		public float JumpImpulse = 7.0f; 
//		public float JumpLongImpulse = 7.15f; 
//		public float GravityForce = -20.0f;
//		public float MaxSpeedY = 15.0f;

		myWidth = coll.size.x;
		myHalfWidth = myWidth * 0.5f;
		myHeight = coll.size.y;
		myHalfHeight = myHeight * 0.5f;
		desiredSpeedX = 0.0f;

		lastHandlePos = new Vector3();
		lastFrameHande = false;
	}
	
	void Start () {

		velocity = new Vector3 (0, 0, 0);
		impulse = new Vector3 (0, 0, 0);
		desiredSpeedX = 0.0f;
		startFallPos = transform.position;

		setState (State.ON_GROUND);
		//setAction (Action.IDLE);
		action = Action.IDLE;

		climbDuration = 0.0f;
		catchedClimbHandle = null;
		canPullUp = false;

		jumpFromMount = false;
	}

	public void die(){
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		setAction (Action.IDLE);
		setState (State.ON_GROUND);
		transform.position = respawnPoint.position;
	}

	void OnTriggerEnter2D(Collider2D other) {
		//print( "PLAYER OnTriggerEnter" + other.gameObject.tag);
		if (other.gameObject.tag == "Bird") {
			if( isInState(State.MOUNT) ){
				velocity.x = 0.0f;
				velocity.y = 0.0f;
				setAction(Action.JUMP);
				setState(State.IN_AIR);
			}
		}
	}

	bool jumpKeyPressed = false;
	float timeFromJumpKeyPressed = 0.0f;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	
		if (Input.GetKeyDown (KeyCode.P)) {
			gamePaused = !gamePaused;
		}

		if( gamePaused ){

			if (Input.GetKey("f")) {
				transform.position = transform.position + new Vector3(-0.1f,0.0f,0.0f);
			}
			else if (Input.GetKey("h")) {
				transform.position = transform.position + new Vector3(0.1f,0.0f,0.0f);
			}
			else if (Input.GetKey("t")) {
				transform.position = transform.position + new Vector3(0.0f,0.1f,0.0f);
			}
			else if (Input.GetKey("g")) {
				transform.position = transform.position + new Vector3(0.0f,-0.1f,0.0f);
			}

			return;
		}

		SetImpulse(new Vector2(0.0f, 0.0f));

		if (!jumpKeyPressed) {
			if (Input.GetKeyDown (keyJump)) {
				timeFromJumpKeyPressed = 0.0f;
				jumpKeyPressed = true;
			}
		} else {
			timeFromJumpKeyPressed += Time.deltaTime;
			if( timeFromJumpKeyPressed >= 0.06f ){
				timeFromJumpKeyPressed = 0.0f;
				jumpKeyPressed = false;

				keyJumpDown();
			}
		}

//		if (Input.GetKeyDown (keyJump)) {
//			keyJumpDown ();
//		} else if (Input.GetKeyUp (keyJump)) {
//			keyJumpUp ();
//		}

		if (Input.GetKeyDown (keyUp)) {
			keyUpDown();
		} 
		if (Input.GetKeyUp (keyUp)) {
			keyUpUp();
		}
		if (Input.GetKeyDown (keyDown)) {
			keyDownDown();
		} 
		if (Input.GetKeyUp (keyDown)) {
			keyDownUp();
		}

		if (Input.GetKeyUp (keyJump)) {
			keyJumpUp ();
		}

		if (Input.GetKeyDown (keyLeft)) {
			keyLeftDown();
		} 
		if (Input.GetKeyDown (keyRight)) {
			keyRightDown();
		}
		
		if (Input.GetKeyUp (keyLeft)) {
			keyLeftUp();
		} 
		if (Input.GetKeyUp (keyRight)) {
			keyRightUp();
		}

		if (Input.GetKeyDown (keyRun)) {
			keyRunDown();
		} else if (Input.GetKeyUp (keyRun)) {
			keyRunUp();
		}



		currentActionTime += Time.deltaTime;
		currentStateTime += Time.deltaTime;

		oldPos = transform.position;
		newPosX = oldPos.x;
		distToMove = 0.0f;

		switch (action) {
		case Action.IDLE:
			Act_IDLE();
			break;

		case Action.LANDING_HARD:
			Act_LANDING_HARD();
			break;

		case Action.PREPARE_TO_JUMP:
			if( currentActionTime >= 0.2f ){
				jump();
			}
			break;

		case Action.CLIMB_PULLDOWN:
			Act_CLIMB_PULLDOWN();
			break;

		case Action.CLIMB_JUMP_TO_CATCH:
			Act_CLIMB_JUMP_TO_CATCH();
				break;

		case Action.CLIMB_CATCH:
			Act_CLIMB_CATCH();
			break;
			
		case Action.CLIMB_CLIMB:
			Act_CLIMB_CLIMB();
			break;

		//case Action.BREAK:
		//	break;

		case Action.WALK_LEFT:
			Act_WALK(-1);
			break;
		case Action.RUN_LEFT:
			Act_RUN(-1);
			break;

		case Action.WALK_RIGHT:
			Act_WALK(1);
			break;
		case Action.RUN_RIGHT:
			Act_RUN(1);
			break;

		case Action.TURN_STAND_LEFT:
			if( currentActionTime >= 0.2f ){
				turnLeft();
				turnLeftFinish();
			}
			break;

		case Action.TURN_STAND_RIGHT:
			if( currentActionTime >= 0.2f ){
				turnRight();
				turnRightFinish();
			}
			break;

		case Action.CROUCH_IDLE:
			Act_CROUCH_IDLE();
			break;

		case Action.CROUCH_LEFT:
			Act_CROUCH_LEFTRIGHT(-1);
			break;

		case Action.CROUCH_RIGHT:
			Act_CROUCH_LEFTRIGHT(1);
			break;

		case Action.MOUNT_IDLE:
			break;

		case Action.MOUNT_LEFT:
		case Action.MOUNT_RIGHT:
		case Action.MOUNT_UP:
			Act_MOUNTING();
			break;

		case Action.MOUNT_DOWN:
			Act_MOUNTING_DOWN();
			break;

		case Action.ROPECLIMB_IDLE:
			Act_ROPECLIMB_IDLE();
			break;

		case Action.ROPECLIMB_UP:
			Act_ROPECLIMB_UP();
			break;

		case Action.ROPECLIMB_DOWN:
			Act_ROPECLIMB_DOWN();
			break;
		};

		if (wantGetUp) {
			if( canGetUp() ){
				getUp();
				wantGetUp = false;
			}
		}

		switch (state) {

		case State.MOUNT:
			if( !onMount() ){
				setAction(Action.JUMP);
				setState(State.IN_AIR);
			}
			break;

		case State.IN_AIR:
			if( Input.GetKeyDown(keyJump) || Input.GetKey(keyJump) ){
	    		if( onMount() ){
					if( jumpFromMount ){
	//						velocity.x = 0.0f;
	//						velocity.y = 0.0f;
	//						setAction(Action.MOUNT_IDLE);
	//						setState(State.MOUNT);
					}else{
						velocity.x = 0.0f;
						velocity.y = 0.0f;
						setAction(Action.MOUNT_IDLE);
						setState(State.MOUNT);
					}
				} else {
					jumpFromMount = false;
				}

			}
			if( jumpFromMount && Input.GetKey(keyJump) ){
				Vector3 flyDist = transform.position - mountJumpStartPos;
				if( flyDist.magnitude >= MountJumpDist ){
					velocity.x = 0.0f;
					velocity.y = 0.0f;
					setAction(Action.MOUNT_IDLE);
					setState(State.MOUNT);
					jumpFromMount = false;
				}
			}

			//justLetGoHandle += Time.deltaTime;

			if( Input.GetKey(keyJump) ) { //&& justLetGoHandle>toNextHandleDuration){

				if( tryCatchHandle() ){
					lastVelocity = velocity;
					return;
				}

				if( tryCatchRope() ){
					return;
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

			addImpulse(new Vector2(0.0f, GravityForce * Time.deltaTime));
			

			if( isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG) ){
				
				if( Input.GetKey(keyLeft) ){
					velocity.x -= (FlyUserControlParam * Time.deltaTime);

					if( isInAction(Action.JUMP_LEFT) ){
						if( Mathf.Abs( velocity.x ) > JumpSpeed )
							velocity.x = -JumpSpeed;
					}else{
						if( Mathf.Abs( velocity.x ) > JumpLongSpeed )
							velocity.x = -JumpLongSpeed;
					}
					
				}else if ( Input.GetKey(keyRight) ){
					velocity.x += (FlyUserControlParam * Time.deltaTime);
					if( velocity.x > 0.0f ) velocity.x = 0.0f;
				}else{
					//velocity.x += (FlySlowDownParam * Time.deltaTime);
					//if( velocity.x > 0.0f ) velocity.x = 0.0f;
				}
				
			}else if( isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG) ){

				if( Input.GetKey(keyRight) ){
					velocity.x += (FlyUserControlParam * Time.deltaTime);

					if( isInAction(Action.JUMP_RIGHT) ){
						if( Mathf.Abs( velocity.x ) > JumpSpeed )
							velocity.x = JumpSpeed;
					}else{
						if( Mathf.Abs( velocity.x ) > JumpLongSpeed )
							velocity.x = JumpLongSpeed;
					}

				}else if( Input.GetKey(keyLeft) ) {
					velocity.x -= (FlyUserControlParam * Time.deltaTime);
					if( velocity.x < 0.0f ) velocity.x = 0.0f;
				}else{
					//velocity.x -= (FlySlowDownParam * Time.deltaTime);
					//if( velocity.x < 0.0f ) velocity.x = 0.0f;
				} 
				
			}else if( isInAction(Action.JUMP) ){

				if( Input.GetKey(keyLeft) ){
					velocity.x -= (FlyUpUserControlParam * Time.deltaTime);
					
					if( Mathf.Abs( velocity.x ) > JumpSpeed )
						velocity.x = -JumpSpeed;
				}
				if( Input.GetKey(keyRight) ){
					velocity.x += (FlyUpUserControlParam * Time.deltaTime);
					
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
			distToFall.x = velocity.x * Time.deltaTime;

			if( distToFall.x > 0.0f )
			{
				float obstacleOnRoad = checkRight(distToFall.x + 0.01f);
				if( obstacleOnRoad >= 0.0f ){
					//print("distToFall.x = " + distToFall.x + " obstacleOnRoad = " + obstacleOnRoad );
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = obstacleOnRoad;
						velocity.x = 0.0f;
						if( velocity.y > 0.0f )
							velocity.y *=  0.5f;
						//setAction(Action.FALL);
						//setAction(Action.JUMP);
					}
				}
			}
			else if( distToFall.x < 0.0f )
			{
				float obstacleOnRoad = checkLeft( Mathf.Abs(distToFall.x) + 0.01f );
				if( obstacleOnRoad >= 0.0f ){
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = -obstacleOnRoad;
						velocity.x = 0.0f;
						if( velocity.y > 0.0f )
							velocity.y *=  0.5f;
						//setAction(Action.FALL);
						//setAction(Action.JUMP);
					}
				}
			}

			velocity.y += impulse.y;
			if(velocity.y > MaxSpeedY)
				velocity.y = MaxSpeedY;
			if(velocity.y < -MaxSpeedY)
				velocity.y = -MaxSpeedY;
			
			distToFall.y = velocity.y * Time.deltaTime;

			bool justLanding = false;

			if( distToFall.y > 0.0f ) { // leci w gore
				//transform.position = transform.position + distToFall;
			} else if( distToFall.y < 0.0f ) { // spada
				if( lastVelocity.y >= 0.0f ) { // zaczyna spadac
					// badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
					startFallPos = transform.position;
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

				setState(State.ON_GROUND);
				velocity.y = 0.0f;

				Vector3 fallDist = startFallPos - transform.position;

				if( fallDist.y >= VeryHardLandingHeight ){
					die();
				} else if( fallDist.y >= HardLandingHeight ){

					velocity.x = 0.0f;
					setAction (Action.LANDING_HARD);
					//resetActionAndState();

				}else{

					//velocity.x = 0.0f;
					//setAction (Action.IDLE);
					resetActionAndState();

				}


			}

			break;
			
		case State.ON_GROUND:
//			groundUnderFeet = checkDown(0.1f);
//			if( groundUnderFeet < 0.0f ) {
//				setState(State.IN_AIR);
//				//setAction(Action.FALL);
//				setAction(Action.JUMP);
//			}

			float distToGround = 0.0f;
			bool groundUnderFeet2 = checkGround (true, layerIdGroundAllMask, ref distToGround);
			if (groundUnderFeet2) {
				//Vector3 pos = transform.position;
				//pos.y += distToGround;
				//transform.position = pos;
			}else{
				setState(State.IN_AIR);
				//setAction(Action.FALL);
				setAction(Action.JUMP);
				wantGetUp = false;
			}

			break;

		case State.CLIMB_ROPE:
			//Vector3 lastSwingPos;
			//Vector3 swingVelocity;
			Vector3 newPos = catchedRopeLink.transform.position;
			Vector3 posDiff = newPos - transform.position;

			swingVelocity = posDiff / Time.deltaTime;

			//transform.position = catchedRopeLink.transform.position;
			//Vector3 linkPos = catchedRopeLink.transform.position;
			//linkPos.y -= 0.5f;

			Vector3 linkPos = catchedRopeLink.transform.TransformPoint( new Vector3(0.0f, ropeLinkCatchOffset, 0.0f) );
			transform.position = linkPos;
			transform.rotation = catchedRopeLink.transform.rotation;

			break;

		};

		lastVelocity = velocity;
	}

	bool tryStartClimbPullDown(){
		GameObject potCatchedClimbHandle = canClimbPullDown();
		//print(potCatchedClimbHandle);
		if( potCatchedClimbHandle ){
			
			catchedClimbHandle = potCatchedClimbHandle;
			
			velocity.x = 0.0f;
			velocity.y = 0.0f;
			climbDuration = 0.0f;
			
			Vector3 handlePos = potCatchedClimbHandle.transform.position;
			
			//climbAfterPos
			climbAfterPos.y = handlePos.y - 2.4f; //myHeight;
			if( dir() == Vector2.right ){
				climbAfterPos.x = handlePos.x - myHalfWidth;
			}else{
				climbAfterPos.x = handlePos.x + myHalfWidth;
			}
			
			climbBeforePos = transform.position;
			climbDistToClimb = climbAfterPos - climbBeforePos;

			wantGetUp = false;
			setAction(Action.CLIMB_PULLDOWN);
			setState(State.CLIMB);

			return true;
		}
		return false;
	}
	int Act_IDLE(){
		//velocity.x = 0.0f;
		if( Input.GetKeyDown(keyDown) ){
			tryStartClimbPullDown();
		}
		return 0;
	}

	int Act_LANDING_HARD(){
		if (currentActionTime >= LANDING_HARD_DURATION) {
			setAction(Action.IDLE);
			resetActionAndState();
		}

		return 0;
	}

	int Act_CLIMB_PULLDOWN(){
		climbDuration += Time.deltaTime;
		
		if( climbDuration >= CLIMBDUR_CLIMB ){
			setAction(Action.CLIMB_CATCH);
			setState(State.CLIMB);
			climbDuration = 0.0f;
			canPullUp = true;
			transform.position = climbAfterPos;
		} else {
			float ratio = climbDuration / CLIMBDUR_CLIMB;
			//transform.position = climbBeforePos + climbDistToClimb*ratio;
			if( ratio > 0.5f )
				transform.position = climbBeforePos + climbDistToClimb*((ratio-0.5f) * 2.0f);
		}
		
		return 0;
	}
	
	int Act_CLIMB_JUMP_TO_CATCH(){
		// dociaganie do punktu:
		climbDuration += Time.deltaTime;
		
		if (climbDuration >= climbToJumpDuration) {
			setAction (Action.CLIMB_CATCH);
			//setState(State.CLIMB);
			climbDuration = 0.0f;
			//canPullUp = true;
			transform.position = climbAfterPos;
		} else {
			float ratio = climbDuration / climbToJumpDuration;
			transform.position = climbBeforePos + climbDistToClimb * ratio;
		}
		
		return 0;
	}
	
	int Act_CLIMB_CATCH(){
		if ( (Input.GetKeyDown (keyUp) || Input.GetKey(keyUp)) && canPullUp) {
			
			climbAfterPos.x = catchedClimbHandle.transform.position.x;
			climbAfterPos.y = catchedClimbHandle.transform.position.y;
			
			climbBeforePos = transform.position;
			climbDistToClimb = climbAfterPos - climbBeforePos;
			
			setAction (Action.CLIMB_CLIMB);
			climbDuration = 0.0f;
			
			catchedClimbHandle = null;
			lastCatchedClimbHandle = null;
		} else if (Input.GetKeyDown (keyDown)) {
			velocity.x = 0.0f;
			velocity.y = 0.0f;
			setState (State.IN_AIR);
			setAction (Action.JUMP);
			catchedClimbHandle = null;
			lastCatchedClimbHandle = null;
			//justLetGoHandle = 0.0f;
		} else if (Input.GetKeyDown (keyJump)) {
			if (dir () == Vector2.right && Input.GetKey (keyLeft)) {
				//print("try jump left");
				turnLeft ();
				jumpLeft ();
				catchedClimbHandle = null;
				lastCatchedClimbHandle = null;
				//justLetGoHandle = 0.0f;
			} else if (Input.GetKey (keyRight)) {
				//print("try jump right");
				turnRight ();
				jumpRight ();
				catchedClimbHandle = null;
				lastCatchedClimbHandle = null;
				//justLetGoHandle = 0.0f;
			} else {
				//print("try jump up");
				jump ();
				lastCatchedClimbHandle = catchedClimbHandle;
				catchedClimbHandle = null;
				//justLetGoHandle = 0.0f;
			}
		}
		
		return 0;
	}
	
	int Act_CLIMB_CLIMB(){
		
		climbDuration += Time.deltaTime;
		
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

	bool checkObstacle(int dir, float distToCheck, ref float distToObstacle){
		if (dir == 1) {
			distToObstacle = checkRight (Mathf.Abs (distToCheck) + 0.01f);
			if (distToObstacle < 0.0f)
				return false;
			if (distToObstacle < distToCheck) {
				return true;
			} else
				return false;
		} else if (dir == -1) {
			distToObstacle = checkLeft (Mathf.Abs (distToCheck) + 0.01f);
			if (distToObstacle < 0.0f)
				return false;
			if (distToObstacle < Mathf.Abs(distToCheck)) {
				distToObstacle *= -1.0f;
				return true;
			} else
				return false;
		} else {
			return false;
		}
	}

	KeyCode getDirKey(int dir){
		return dir == 1 ? keyRight : keyLeft;
	}
	
	bool checkSpeed(int dir){
		float speedX = Mathf.Abs (velocity.x);
		if (speedX < desiredSpeedX) { // trzeba przyspieszyc

			float velocityDamp = SpeedUpParam * Time.deltaTime;
			//print ( velocityDamp + " " + speedX + " " + desiredSpeedX );
			speedX += velocityDamp;
			if( speedX > desiredSpeedX ){
				speedX = desiredSpeedX;
				velocity.x = desiredSpeedX * dir;
				return true;
			}
			velocity.x = speedX * dir;
			return false;

		} else if (speedX > desiredSpeedX) { // trzeba zwolnic
			float velocityDamp = SlowDownParam * Time.deltaTime;
			speedX -= velocityDamp;
			if( speedX < desiredSpeedX ){
				speedX = desiredSpeedX;
				velocity.x = desiredSpeedX * dir;
				return true;
			}
			velocity.x = speedX * dir;
			return false;
		}
		return true;
	}

	bool speedLimiter(int dir, float absMaxSpeed){
		if( dir == -1 ){
			if( velocity.x < 0.0f && Mathf.Abs(velocity.x) > absMaxSpeed ){
				velocity.x = -absMaxSpeed;
				return true;
			}
		}else if( dir == 1 ){
			if( velocity.x > 0.0f && Mathf.Abs(velocity.x) > absMaxSpeed ){
				velocity.x = absMaxSpeed;
				return true;
			}
		}
		//aa
		return false;
	}

	int Act_WALK(int dir){

		//currentWalkRunDuration += Time.deltaTime;
		//animator.speed = 1.0f;

		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f ) {
			setAction(Action.IDLE);
			resetActionAndState();
		}

		distToMove = velocity.x * Time.deltaTime;

		//animator.speed = 0.25f + (Mathf.Abs( velocity.x ) / WalkSpeed ) * 0.75f;
		//print (animator.speed);

		float distToObstacle = 0.0f;
		if (checkObstacle (dir, distToMove, ref distToObstacle)) {
			//print ("STOP STOP STOP");
			distToMove = distToObstacle;
			setActionIdle();
		}
		//print (distToObstacle);

		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);

		float distToGround = 0.0f;
		bool groundUnderFeet = checkGround (false, layerIdLastGroundTypeTouchedMask, ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
		}
//		} else {
//			groundUnderFeet = checkGround (false, layerIdGroundAllMask, ref distToGround);	
//			if( groundUnderFeet ){
//				transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
//			} else {
//				setState(State.IN_AIR);
//				setAction(Action.JUMP);
//			}
//		}

		return 0;
	}

	int Act_RUN(int dir){

		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f) {
			setAction(Action.IDLE);
			resetActionAndState ();
		}

		distToMove = velocity.x * Time.deltaTime;
		
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
//		} else {
//			groundUnderFeet = checkGround (false, layerIdGroundAllMask, ref distToGround);	
//			if( groundUnderFeet ){
//				transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
//			} else {
//				setState(State.IN_AIR);
//				setAction(Action.JUMP);
//			}
//		}

		return 0;
	}

	int Act_CROUCH_IDLE(){
		if( Input.GetKey(keyDown) ){
			tryStartClimbPullDown();
		}
		return 0;
	}

	int Act_CROUCH_LEFTRIGHT(int dir){
		
		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f) {
			//setAction(Action.CROUCH_IDLE);
			setActionCrouchIdle();
			//resetActionAndState ();
			//setActionCrouchIdle();
			if( crouching() ) { // Input.GetKey(keyDown) ){ - raczej zbedne
				if( Input.GetKey(keyLeft) ) {
					keyLeftDown();
				} else if( Input.GetKey(keyRight) ){
					keyRightDown();
				}
			}
		}
		
		distToMove = velocity.x * Time.deltaTime;
		
		float distToObstacle = 0.0f;
		if (checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			//setActionIdle();
			setActionCrouchIdle();
		}
		
		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);
		
		float distToGround = 0.0f;
		bool groundUnderFeet = checkGround (false, layerIdLastGroundTypeTouchedMask, ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
		}
//		} else {
//			groundUnderFeet = checkGround (false, layerIdGroundAllMask, ref distToGround);	
//			if( groundUnderFeet ){
//				transform.position = new Vector3 (newPosX, oldPos.y + distToGround, 0.0f);
//			} else {
//				setState(State.IN_AIR);
//				setAction(Action.JUMP);
//			}
//		}
		
		return 0;
	}

	int Act_MOUNTING(){
		Vector3 newPos3 = transform.position;
		Vector3 distToMount = velocity * Time.deltaTime;
		newPos3 += distToMount;
		if (onMount (newPos3)) {
			transform.position = newPos3;
		} else {
			setMountIdle();
		}
		return 0;
	}
	
	int Act_MOUNTING_DOWN(){
		Vector3 newPos3 = transform.position;
		Vector3 distToMount = velocity * Time.deltaTime;
		newPos3 += distToMount;
		//transform.position = newPos3;
		
		if (distToMount.y < 0.0f) { // schodzi
			groundUnderFeet = checkDown ( Mathf.Abs(distToMount.y) + 0.01f);
			if (groundUnderFeet >= 0.0f) {
				if (groundUnderFeet < Mathf.Abs (distToMount.y)) {
					distToMount.y = -groundUnderFeet;
					velocity.x = 0.0f;
					velocity.y = 0.0f;
					setState (State.ON_GROUND);
					setAction (Action.IDLE);
					//resetActionAndState();
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

	bool justJumpFromRope = false;

	int Act_ROPECLIMB_IDLE(){

		if (!catchedRope)
			return 0;

		if (Input.GetKey (keyLeft)) {
			turnLeft();

			float fla = catchedRope.firstLinkAngle;
			//float fls = catchedRope.firstLinkSpeed;

			if( fla < 10f && fla > -15f){
				//catchedRope.swing(-Vector2.right,RopeSwingForce * (fla/50f) );
				//print( "swing left : fla " + fla );
				catchedRope.swing(-Vector2.right, RopeSwingForce * Time.deltaTime );
			}
//			}else if( fla == 0f && fls == 0f){
//				catchedRope.swing(-Vector2.right,RopeSwingForce);
//			}
		}
		if (Input.GetKey (keyRight)) {

			turnRight();

			float fla = catchedRope.firstLinkAngle;
			//float fls = catchedRope.firstLinkSpeed;

			if( fla > -10f && fla < 15f){
				//catchedRope.swing(Vector2.right, RopeSwingForce * (-fla/50f) );
				//print( "swing right : fla " + fla );
				catchedRope.swing(Vector2.right, RopeSwingForce * Time.deltaTime );
			}
//			}else if( fla == 0f && fls == 0f ){
//				catchedRope.swing(Vector2.right, RopeSwingForce );
//			}
		}

		if (Input.GetKeyDown (keyJump)) {
			//catchedRope = null;
			//catchedRopeLink = null;

			float ropeSpeed = catchedRope.firstLinkSpeed;

			float ropeSpeedRad = ropeSpeed * Mathf.Deg2Rad;

			int crl_idn = catchedRope.currentLink.GetComponent<RopeLink>().idn;

			float ps =  ropeSpeedRad * (crl_idn+1) * 0.5f;

			float ropeAngle = Mathf.Abs( catchedRope.firstLinkAngle );

			//Quaternion.

			if( Input.GetKey(keyLeft) ){ //skacze w lewo

				turnLeft();

				if( ropeSpeed > 0f ){ // lina tez leci w lewo
					//setAction(Action.JUMP_LEFT_LONG);
					jumpLongLeft();
					velocity.x -= ps;
				}else{
					//setAction(Action.JUMP_LEFT);
					jumpLeft();
				}

			} else if( Input.GetKey(keyRight) ) { //skacze w prawo

				turnRight();

				if( ropeSpeed < 0f ){ // lina tez leci w prawo
					jumpLongRight();
					velocity.y += ps;
				}else{
					jumpRight();
				}

			}else{

				//jump();
				velocity.x = -ps;
				velocity.y = (ropeAngle/30.0f) * JumpLongImpulse;
				setAction(Action.JUMP);
			}

//			if( ps < 0f ){
//
//      			turnRight();
//				
//				if( Mathf.Abs( ropeSpeed ) >= JumpLongSpeed ){
//					setAction(Action.JUMP_RIGHT_LONG);
//				}else{
//					setAction(Action.JUMP_RIGHT);
//				}
//				//velocity = swingVelocity;
//				velocity.x = -ps;
//				velocity.y = (ropeAngle/30.0f) * JumpLongImpulse;
//
//
//			}else if (ps > 0f){
//			
//				turnLeft();
//			
//				if( Mathf.Abs( ropeSpeed ) >= JumpLongSpeed ){
//					setAction(Action.JUMP_LEFT_LONG);
//				}else{
//					setAction(Action.JUMP_LEFT);
//				}
//				//velocity = swingVelocity;
//				velocity.x = -ps;
//				velocity.y = (ropeAngle/30.0f) * JumpLongImpulse;
//			
//			}else{
//				
//				setAction(Action.JUMP);
//				velocity.x = 0.0f;
//				velocity.y = 0.0f;
//				
//			}


//			if( swingVelocity.x > 0 ){
//
//				turnRight();
//
//				if( Mathf.Abs( swingVelocity.x) >= JumpLongSpeed ){
//					setAction(Action.JUMP_RIGHT_LONG);
//				}else{
//					setAction(Action.JUMP_RIGHT);
//				}
//				velocity = swingVelocity;
//
//				//catchedRope.resetDiver();
//
//			}else if (swingVelocity.x < 0){
//
//				turnLeft();
//
//				if( Mathf.Abs( swingVelocity.x) >= JumpLongSpeed ){
//					setAction(Action.JUMP_LEFT_LONG);
//				}else{
//					setAction(Action.JUMP_LEFT);
//				}
//				velocity = swingVelocity;
//
//				//catchedRope.resetDiver();
//
//			}else{
//
//				setAction(Action.JUMP);
//				velocity.x = 0.0f;
//				velocity.y = 0.0f;
//
//				//catchedRope.resetDiver();
//			}

			//Vector3 posInWorld = transform.TransformPoint( 0f,-1.65f,0f );
			//transform.position = posInWorld;
			Vector3 oldPos = transform.position;
			oldPos.y -= 1.65f;
			transform.position = oldPos;

			catchedRope.resetDiver();
			catchedRope = null;
			catchedRopeLink = null;

			//transform.rotation.eulerAngles = new Vector3(0f,0f,0f);
			Quaternion quat = new Quaternion();
			quat.eulerAngles = new Vector3(0f,0f,0f);
			transform.rotation = quat;
			setState(State.IN_AIR);

			justJumpFromRope = true;

			return 0;
		}

		if (Input.GetKey (keyUp)) { 

			setAction(Action.ROPECLIMB_UP);

		} else if (Input.GetKey (keyDown)) {

			setAction(Action.ROPECLIMB_DOWN);

		}

		return 0;
	}

	int Act_ROPECLIMB_UP(){

		if (!catchedRope)
			return 0;

		if (Input.GetKeyUp (keyUp)) { 
			setAction(Action.ROPECLIMB_IDLE);
			return 0;
		} 

		float climbDist = RopeClimbSpeed * Time.deltaTime;

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
			}

		} else {

			ropeLinkCatchOffset = newRopeLinkCatchOffset;
		}


		return 0;
	}

	int Act_ROPECLIMB_DOWN(){

		if (!catchedRope)
			return 0;

		if (Input.GetKeyUp (keyDown)) {
			setAction(Action.ROPECLIMB_IDLE);
			return 0;
		}

		float climbDist = RopeClimbSpeed * Time.deltaTime;
		
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
				}
				
			}else {
				ropeLinkCatchOffset = -0.5f;
			}
			
		} else {
			
			ropeLinkCatchOffset = newRopeLinkCatchOffset;
		}


		return 0;
	}

	
	void startWalkOrRun(float desiredSpeed, float accTime){
//		timeFromStartWalkRun = 0.0f;
//		walkRunStartSpeedX = velocity.x;
//		
//		speedDiff = (desiredSpeed - Mathf.Abs (walkRunStartSpeedX));
//		if (speedDiff > 0.0f) {
//			walkRunSpeedUp = true;
//			timeToSpeedUp = (speedDiff * accTime) / desiredSpeed;
//		}
	}

	bool keyLeftDown(){
		if ((isInAction (Action.IDLE) || moving (-1) || jumping ()) && isInState (State.ON_GROUND)) {
			if (checkLeft (0.1f) >= 0.0f) {
				//print ("cant move left");
				turnLeftStart();
				return false;
			}

			if( dir() == -Vector2.right )
			{
				if (Input.GetKey (keyRun)) {
					//startWalkOrRun(RUN_SPEED,stopToRunDuration);
					desiredSpeedX = RunSpeed;
					speedLimiter(-1,desiredSpeedX+1.0f);
					setAction (Action.RUN_LEFT);
					return true;
				} else {
					//startWalkOrRun(WALK_SPEED,stopToWalkDuration);
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
					//turnLeft ();
					velocity.x = -MountSpeed;
					velocity.y = 0.0f;
					setAction (Action.MOUNT_LEFT);
					return true;
				}
			}
		} else if (isInAction (Action.CROUCH_IDLE) && isInState (State.ON_GROUND)) {
			if( checkLeft(0.1f) >= 0.0f ){
				turnLeft();
				return false;
			}
			turnLeft();
			desiredSpeedX = CrouchSpeed;
			setAction(Action.CROUCH_LEFT);
			return true;
		}
		return false;
	}
	bool keyRightDown(){
		if ( (isInAction (Action.IDLE) || moving(1) || jumping()) && isInState(State.ON_GROUND) ) {
			if( checkRight (0.1f) >= 0.0f ) {
				//print ("cant move right");
				turnRightStart();
				return false;
			}
			//turnRight();
			if( dir() == Vector2.right ){
				if( Input.GetKey(keyRun) ){
					//startWalkOrRun(RUN_SPEED,stopToRunDuration);
					desiredSpeedX = RunSpeed;
					speedLimiter(1,desiredSpeedX+1.0f);
					setAction(Action.RUN_RIGHT);
					return true;
				}else{
					//startWalkOrRun(WALK_SPEED,stopToWalkDuration);
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
					//turnRight();
					velocity.x = MountSpeed;
					velocity.y = 0.0f;
					setAction(Action.MOUNT_RIGHT);
					return true;
				}
			}
		} else if (isInAction (Action.CROUCH_IDLE) && isInState (State.ON_GROUND)) {
			if( checkRight(0.1f) >= 0.0f ){
				turnRight();
				return false;
			}
			turnRight();
			desiredSpeedX = CrouchSpeed;
			setAction(Action.CROUCH_RIGHT);
			return true;
		}
		return false;
	}
	
	void keyLeftUp(){

		if ( !setMountIdle() ) {
			if (isInState (State.ON_GROUND)){
				
				//if( walking() != 0 || running() != 0 )
				//	setAction(Action.BREAK);
				//else
				//setAction (Action.IDLE);

				desiredSpeedX = 0.0f;
			}
			//resetActionAndState ();
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
	void keyRightUp(){
		if (!setMountIdle ()) {
			if (isInState (State.ON_GROUND)) {
				
				//if( walking() != 0 || running() != 0 )
				//	setAction (Action.BREAK);
				//else
				//setAction(Action.IDLE);

				desiredSpeedX = 0.0f;
			}
			//resetActionAndState ();
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
	
	void keyRunDown(){
		switch (action) {
			
		case Action.WALK_LEFT:
			if( Input.GetKey(keyLeft) ){
				//startWalkOrRun(RUN_SPEED,walkToRunDuration);
				desiredSpeedX = RunSpeed;
				setAction(Action.RUN_LEFT);
			}
			break;
			
		case Action.WALK_RIGHT:
			if( Input.GetKey(keyRight) ){
				//startWalkOrRun(RUN_SPEED,walkToRunDuration);
				desiredSpeedX = RunSpeed;
				setAction(Action.RUN_RIGHT);
			}
			break;
		};
	}
	void keyRunUp(){

		switch (action) {
			
		case Action.RUN_LEFT:
			//startWalkOrRun(WALK_SPEED,walkToRunDuration);
			if( Input.GetKey(keyLeft) ){
				desiredSpeedX = WalkSpeed;
				setAction(Action.WALK_LEFT);
			}else{
				desiredSpeedX = 0.0f;
			}
			break;
			
		case Action.RUN_RIGHT:
			//startWalkOrRun(WALK_SPEED,walkToRunDuration);
			if( Input.GetKey(keyRight) ) {
				desiredSpeedX = WalkSpeed;
				setAction(Action.WALK_RIGHT);
			}else{
				desiredSpeedX = 0.0f;
			}
			break;
		};

//		if( isInState(State.ON_GROUND) )
//			setAction (Action.IDLE);
//		resetActionAndState ();
	}
	
	void keyJumpDown(){
		//string s;
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
			jumpLeft();
			break;
			
		case Action.MOUNT_RIGHT:
			mountJumpStartPos = transform.position;
			jumpFromMount = true;
			jumpRight();
			break;
		};
	}
	
	void keyJumpUp(){
		jumpFromMount = false;
		justJumpFromRope = false;
	}
	
	void keyUpDown(){
		if (isInState (State.MOUNT)) {
			if( !mounting () ){
				Vector3 playerPos = transform.position;
				playerPos.y += 0.1f;
				if( onMount(playerPos) ){
					velocity.x = 0.0f;
					velocity.y = MountSpeed;
					setAction (Action.MOUNT_UP);
				}
			}
		} else if (isInState (State.ON_GROUND)) {
			if( onMount() ){
				velocity.x = 0.0f;
				velocity.y = MountSpeed;
				setAction (Action.MOUNT_UP);
				setState(State.MOUNT);

			}
		}

	}
	void keyUpUp(){
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
	}

	bool keyDownDown(){
		if (isInState (State.MOUNT)) {
			if (!mounting ()) {
				Vector3 playerPos = transform.position;
				playerPos.y -= 0.1f;
				if (onMount (playerPos)) {
					velocity.x = 0.0f;
					velocity.y = -MountSpeed;
					setAction (Action.MOUNT_DOWN);
					return true;
				}
			}
		} else if (isInState (State.ON_GROUND)) {

			crouch();
			return true;
		}

		return false;
	}
	void keyDownUp(){
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
			if( crouching() ){
				if( canGetUp() ){
					getUp();
				}else{
					wantGetUp = true;
				}
			}
		}
	}
	
	void getUp(){
		//velocity.x = 0.0f;
		//velocity.y = 0.0f;
		setAction(Action.IDLE);
		//action = Action.IDLE;
		resetActionAndState ();
	}

	void crouch(){
		if (isInState (State.ON_GROUND)) {
		
			switch (action) {
			
			case Action.IDLE:
			case Action.JUMP:
				//velocity.x = 0.0f;
				//velocity.y = 0.0f;
				setAction (Action.CROUCH_IDLE);
				if( Input.GetKey(keyLeft) ){
					keyLeftDown();
				} else if( Input.GetKey(keyRight) ){
					keyRightDown();
				}else{
					velocity.x = 0.0f;
					velocity.y = 0.0f;
					//setAction (Action.CROUCH_IDLE);
				}
				break;
			//aaa
			case Action.WALK_LEFT:
			case Action.RUN_LEFT:
			case Action.JUMP_LEFT:
			case Action.JUMP_LEFT_LONG:
				if( Input.GetKey(keyLeft)){
					velocity.x = 0.0f;
					desiredSpeedX = CrouchSpeed;
					setAction (Action.CROUCH_LEFT);
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
					setAction (Action.CROUCH_RIGHT);
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

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}

	void jumpLeft(){
		//print ("jumpLeft");
		velocity.x = -JumpSpeed;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, JumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT);
		//jumpStartX = transform.position.x;

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}
	
	void jumpRight(){
		//print ("jumpRight");
		velocity.x = JumpSpeed;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, JumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_RIGHT);
		//jumpStartX = transform.position.x;

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}
	
	void jumpLongLeft(){
		//print ("jumpLongLeft");
		velocity.x = -JumpLongSpeed;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, JumpLongImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT_LONG);
		//jumpStartX = transform.position.x;

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}
	
	void jumpLongRight(){
		//print ("jumpLongRight");
		velocity.x = JumpLongSpeed;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, JumpLongImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_RIGHT_LONG);
		//jumpStartX = transform.position.x;

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
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

	void turnLeftStart(){
		setAction (Action.TURN_STAND_LEFT);
	}

	void turnRightStart(){
		setAction (Action.TURN_STAND_RIGHT);
	}

	void turnLeftFinish(){
		setAction (Action.IDLE);
		//action = Action.IDLE;
		resetActionAndState ();
	}
	
	void turnRightFinish(){
		setAction (Action.IDLE);
		//action = Action.IDLE;
		resetActionAndState ();
	}

	void turnLeft(){
		Vector3 scl = gfx.localScale;
		scl.x = Mathf.Abs(scl.x) * -1.0f;
		gfx.localScale = scl;

	}
	void turnRight(){
		Vector3 scl = gfx.localScale;
		scl.x = Mathf.Abs(scl.x) * 1.0f;
		gfx.localScale = scl;
	}
	
	//bool dir(){
	//	return gfx.localScale.x > 0.0f;
	//}
	
	Vector2 dir(){
		return gfx.localScale.x > 0.0f ? Vector2.right : -Vector2.right;
	}

	void setActionIdle(){
		velocity.x = 0.0f;
		setAction (Action.IDLE);
	}
	void setActionCrouchIdle(){
		velocity.x = 0.0f;
		setAction (Action.CROUCH_IDLE);
	}

	void resetActionAndState(){
		if (isInState (State.ON_GROUND)) {
			if( Input.GetKey(keyDown) ) { //&& (Input.GetKey(keyLeft) || Input.GetKey(keyRight)) ){
				if( !keyDownDown() )
					setActionIdle();
			} else if (Input.GetKey (keyLeft)) {
			//if (Input.GetKey (keyLeft)) {
				if( !keyLeftDown () )
					setActionIdle();
			} else if (Input.GetKey (keyRight)) {
				if( !keyRightDown () )
					setActionIdle();
			//} else if( Input.GetKey(keyDown) ) { //&& (Input.GetKey(keyLeft) || Input.GetKey(keyRight)) ){
			//		if( !keyDownDown() )
			//			setActionIdle();
			} else {
				if (isInState (State.ON_GROUND)) {
					setActionIdle();
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
		return isInAction(Action.CROUCH_IDLE) || isInAction(Action.CROUCH_LEFT) || isInAction(Action.CROUCH_RIGHT);
	}

	float checkLeft(float checkingDist){
		Vector2 rayOrigin = new Vector2( sensorLeft1.position.x, sensorLeft1.position.y );
		// ponizej robie layerIdGroundAllMask - aby wchodzil na platformy ze skosow nie bedzie sie dalo klasc jednej przepuszczalnej na drugiej ale trudno
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundAllMask);
		if (hit.collider != null) {
			//print( hit.collider.gameObject.transform.rotation.eulerAngles );
			//print( hit.collider.gameObject.transform.rotation.to );
			float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
			//print (angle);
			//return Mathf.Abs (hit.point.x - sensorLeft1.position.x);
			if( angle <= 0.0f || angle > 45.0f ) 
				return Mathf.Abs (hit.point.x - sensorLeft1.position.x);
			else 
			{
				//if( isInState(State.IN_AIR) ){
				//	return Mathf.Abs (hit.point.x - sensorLeft1.position.x);
				//}
				//else{
					return -1.0f;
				//}
			}
		} else {
			if( crouching() ) 
				return -1.0f;

			rayOrigin = new Vector2( sensorLeft2.position.x, sensorLeft2.position.y );
			hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundMask);
			if (hit.collider != null){
				//print( hit.collider.gameObject.transform.rotation.eulerAngles );
				//float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
				//print (angle);
				return Mathf.Abs (hit.point.x - sensorLeft2.position.x);
			} else {

				rayOrigin = new Vector2( sensorLeft3.position.x, sensorLeft3.position.y );
				hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundMask);
				if (hit.collider != null){
					//print( hit.collider.gameObject.transform.rotation.eulerAngles );
					//float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
					//print (angle);
					return Mathf.Abs (hit.point.x - sensorLeft3.position.x);
				} else {
					return -1.0f;
				}
			}
		}
	}

//	float checkLeftInFlight(float checkingDist){
//		//print( hit.collider.gameObject.transform.rotation.eulerAngles );
//		//print( hit.collider.gameObject.transform.rotation.to );
//
//		//Vector2 rayOrigin = new Vector2( sensorLeft1.position.x, sensorLeft1.position.y );
//		Vector2 rayOrigin = transform.position;
//		rayOrigin.x -= myHalfWidth;
//		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundAllMask);
//		if (hit.collider != null) {
//			float angle = Quaternion.Angle (transform.rotation, hit.collider.transform.rotation);
//			//print (angle);
//			//return Mathf.Abs (hit.point.x - sensorLeft1.position.x);
//			int collLayer = hit.collider.gameObject.layer;
//			if (angle <= 0.0f || angle > 45.0f) {
//				if (collLayer & layerIdGroundPermeableMask) { // przez takie cos powienien przelatywac - plaskie lub stojace{
//				} else {
//					// to jest jakas zwykla przeszkoda zatrzymujemy sie na niej...
//					//return Mathf.Abs (hit.point.x - sensorLeft1.position.x);
//				}
//			} else {
//				return -1.0f;
//			}
//		}
//
//		rayOrigin = new Vector2( sensorLeft2.position.x, sensorLeft2.position.y );
//		hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundMask);
//		if (hit.collider != null){
//			//print( hit.collider.gameObject.transform.rotation.eulerAngles );
//			//float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
//			//print (angle);
//			return Mathf.Abs (hit.point.x - sensorLeft2.position.x);
//		} else {
//			rayOrigin = new Vector2( sensorLeft3.position.x, sensorLeft3.position.y );
//			hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundMask);
//			if (hit.collider != null){
//				//print( hit.collider.gameObject.transform.rotation.eulerAngles );
//				//float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
//				//print (angle);
//				return Mathf.Abs (hit.point.x - sensorLeft3.position.x);
//			} else {
//				return -1.0f;
//			}
//		}
//	}

	float checkRight(float checkingDist){
		Vector2 rayOrigin = new Vector2( sensorRight1.position.x, sensorRight1.position.y );
		// ponizej robie layerIdGroundAllMask - aby wchodzil na platformy ze skosow nie bedzie sie dalo klasc jednej przepuszczalnej na drugiej ale trudno
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist, layerIdGroundAllMask);
		if (hit.collider != null) {
			//print( hit.collider.gameObject.transform.rotation.eulerAngles );
			float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
			//print (angle);
			//return Mathf.Abs (hit.point.x - sensorRight1.position.x);
			if( angle <= 0.0f || angle > 45.0f ) return Mathf.Abs (hit.point.x - sensorRight1.position.x);
			else return -1.0f;
		} else {
			if (crouching())
				return -1.0f;

			rayOrigin = new Vector2( sensorRight2.position.x, sensorRight2.position.y );
			hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist, layerIdGroundMask);
			if (hit.collider != null){
				//print( hit.collider.gameObject.transform.rotation.eulerAngles );
				//float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
				//print (angle);
				return Mathf.Abs (hit.point.x - sensorRight2.position.x);
			} else {

				rayOrigin = new Vector2( sensorRight3.position.x, sensorRight3.position.y );
				hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist, layerIdGroundMask);
				if (hit.collider != null){
					//print( hit.collider.gameObject.transform.rotation.eulerAngles );
					//float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
					//print (angle);
					return Mathf.Abs (hit.point.x - sensorRight3.position.x);
				} else {
					return -1.0f;
				}
			}
		} 
	}

	bool canGetUp(){

		RaycastHit2D hit = Physics2D.Raycast (sensorLeft3.position, Vector2.right, myWidth, layerIdGroundMask);
		if (hit.collider != null)
		{
			if( Mathf.Abs (sensorLeft3.position.x+myWidth - hit.point.x) > 0.0001f )
				return false;
		}
		return true;

//		if (dir () == Vector2.right) {
//
//			Vector2 rayOrigin = new Vector2(transform.position.x-myHalfWidth,transform.position.y+1.0f);
//			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundMask);
//			if (hit.collider != null)
//			{
//				if( Mathf.Abs (rayOrigin.x+myWidth - hit.point.x) > 0.0001f )
//					return false;
//			}
//			rayOrigin.y += 1;
//
//			//return Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundMask).collider == null;
//			hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundMask);
//			if (hit.collider != null)
//			{
//				if( Mathf.Abs (rayOrigin.x+myWidth - hit.point.x) > 0.0001f )
//					return false;
//			}
//
//			return true;
//
//		} else {
//
//			Vector2 rayOrigin = new Vector2(transform.position.x+myHalfWidth,transform.position.y+1.0f);
//			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.right, myWidth, layerIdGroundMask);
//			if (hit.collider != null)
//			{
//				if( Mathf.Abs (rayOrigin.x-myWidth - hit.point.x) > 0.0001f )
//					return false;
//			}
//			rayOrigin.y += 1;
//			
//			//return Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundMask).collider == null;
//			hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundMask);
//			if (hit.collider != null)
//			{
//				if( Mathf.Abs (rayOrigin.x-myWidth - hit.point.x) > 0.0001f )
//					return false;
//			}
//			
//			return true;
//		}

	}

	float checkDown(float checkingDist){

		int layerIdMask = layerIdGroundAllMask;
		Vector3 rayOrigin = sensorDown1.position;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundPermeableMask);
		if (hit.collider) {// jesetem wewnatrz wskakiwalnej platformy ... nie moge sie zatrzymac..
			//return -1.0f;
			layerIdMask = layerIdGroundMask;
		}

		rayOrigin = new Vector2( sensorDown1.position.x, sensorDown1.position.y );
		hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, layerIdMask);
		if (hit.collider != null) {
			layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
			return Mathf.Abs (hit.point.y - sensorDown1.position.y);
		} else {
			rayOrigin = new Vector2( sensorDown2.position.x, sensorDown2.position.y );
			hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, layerIdMask);
			if (hit.collider != null){
				layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
				return Mathf.Abs (hit.point.y - sensorDown2.position.y);
			} else {
				rayOrigin = new Vector2( sensorDown3.position.x, sensorDown3.position.y );
				hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, layerIdMask);
				if (hit.collider != null){
					layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
					return Mathf.Abs (hit.point.y - sensorDown3.position.y);
				} else {
					return -1.0f;
				}
			}
		}  
	}

	bool checkGround (bool fromFeet, int layerIdMask, ref float distToGround){
		bool groundUnderFeet = false;

		//bool groundUnderFeet1 = false;
		//bool groundUnderFeet2 = false;
		//bool groundUnderFeet3 = false;
		float th = 0.9f;
		float checkingDist = th + 0.5f;
		if (fromFeet)
			checkingDist = 0.5f;

		Vector2 rayOrigin1 = sensorDown1.position;
		if( !fromFeet )
			rayOrigin1.y += th;
		RaycastHit2D hit1 = Physics2D.Raycast (rayOrigin1, -Vector2.up, checkingDist, layerIdMask);

		Vector2 rayOrigin2 = sensorDown2.position;
		if( !fromFeet )
			rayOrigin2.y += th;
		RaycastHit2D hit2 = Physics2D.Raycast (rayOrigin2, -Vector2.up, checkingDist, layerIdMask);

		Vector2 rayOrigin3 = sensorDown3.position;
		if( !fromFeet )
			rayOrigin3.y += th;
		RaycastHit2D hit3 = Physics2D.Raycast (rayOrigin3, -Vector2.up, checkingDist, layerIdMask);

		float dist1;
		float dist2;
		float dist3;

		if (hit1.collider != null) {
			//dist1 = myHeight - Mathf.Abs (hit1.point.y - sensorDown1.position.y);aa
			dist1 = rayOrigin1.y - hit1.point.y;
			groundUnderFeet = true;
			distToGround = dist1;
			layerIdLastGroundTypeTouchedMask = 1 << hit1.collider.transform.gameObject.layer;
		}
		if (hit2.collider != null) {
			//dist2 = myHeight - Mathf.Abs (hit2.point.y - sensorDown2.position.y);
			//hit1.collider.transform.gameObject.layer
			dist2 = rayOrigin2.y - hit2.point.y;
			if( groundUnderFeet ){
				if( distToGround > dist2) distToGround = dist2;
			}else{
				groundUnderFeet = true;
				distToGround = dist2;
				layerIdLastGroundTypeTouchedMask = 1 << hit2.collider.transform.gameObject.layer;
			}
		}
		if (hit3.collider != null) {
			//dist3 = myHeight - Mathf.Abs (hit3.point.y - sensorDown3.position.y);
			dist3 = rayOrigin3.y - hit3.point.y;
			if( groundUnderFeet ){
				if( distToGround > dist3) distToGround = dist3;
			}else{
				groundUnderFeet = true;
				distToGround = dist3;
				layerIdLastGroundTypeTouchedMask = 1 << hit3.collider.transform.gameObject.layer;
			}
		}

		if (groundUnderFeet) {
			if( !fromFeet )
				distToGround = th - distToGround;
		}

		return groundUnderFeet;

//		if (groundUnderFeet) {
//			if (dist1 < 0.0f && dist2 < 0.0f && dist3 < 0.0f) { // w całości odleciał
//
//				distToGround = dist1;
//				if( distToGround > dist2 ) distToGround = dist2;
//				if( distToGround > dist3 ) distToGround = dist3;
//
//			} else { // czymś się wbija
//
//
//
//			}
//		}


//		if (hit.collider != null) {
//			return Mathf.Abs (hit.point.y - sensorDown1.position.y);
//		} else {
//			rayOrigin = new Vector2( sensorDown2.position.x, sensorDown2.position.y );
//			hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, currentLayerIdGroundMask);
//			if (hit.collider != null){
//				return Mathf.Abs (hit.point.y - sensorDown2.position.y);
//			} else {
//				rayOrigin = new Vector2( sensorDown3.position.x, sensorDown3.position.y );
//				hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, currentLayerIdGroundMask);
//				if (hit.collider != null){
//					return Mathf.Abs (hit.point.y - sensorDown3.position.y);
//				} else {
//					return -1.0f;
//				}
//			}
//		}  
	}

	bool tryCatchHandle(){
		if (dir () == Vector2.right) {
		
			//RaycastHit2D hit = Physics2D.Linecast(sensorHandleR1.position, sensorHandleR2.position, layerIdGroundHandlesMask); 
			RaycastHit2D hit; 
			if (lastFrameHande)
				hit = Physics2D.Linecast (lastHandlePos, sensorHandleR2.position, layerIdGroundHandlesMask);
			else
				hit = Physics2D.Linecast (sensorHandleR2.position, sensorHandleR2.position, layerIdGroundHandlesMask); 
		
			//					// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
			//					bool _canCatch = true;
			//					if( lastCatchedClimbHandle && velocity.y > 0 ){
			//						_canCatch = false;
			//					}
		
			if (hit.collider != null) {
				//print ( hit.collider.gameObject.transform.position );
			
				// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
				bool _canCatch = true;
				if ((lastCatchedClimbHandle == hit.collider.gameObject) && velocity.y >= 0.0f) {
					_canCatch = false;
				}
			
				if (_canCatch) {
					catchedClimbHandle = hit.collider.gameObject;
				
					Vector3 handlePos = catchedClimbHandle.transform.position;
					Vector3 newPos = new Vector3 ();
					newPos.x = handlePos.x - myHalfWidth;
					newPos.y = handlePos.y - 2.4f; //myHeight;
					//transform.position = newPos;
				
					canPullUp = canClimbPullUp ();
				
					if (canPullUp) {
						//climbAfterPos.x = catchedClimbHandle.transform.position.x;
						//climbAfterPos.y = catchedClimbHandle.transform.position.y;
					}
				
					velocity.x = 0.0f;
					velocity.y = 0.0f;
					//impulse.y = 0.0f;
				
					climbBeforePos = transform.position;
					climbAfterPos = newPos;
					climbDistToClimb = climbAfterPos - climbBeforePos;
					climbToJumpDuration = climbDistToClimb.magnitude * 0.5f;
				
					setState (State.CLIMB); 
					setAction (Action.CLIMB_JUMP_TO_CATCH);
					climbDuration = 0.0f;
					lastFrameHande = false;

 					return true;
				}
			}
		
			lastHandlePos = sensorHandleR2.position;
			return false;
		
		} else {
		
			//RaycastHit2D hit = Physics2D.Linecast(sensorHandleL1.position, sensorHandleL2.position, layerIdGroundHandlesMask); 
			RaycastHit2D hit; 
			if (lastFrameHande)
				hit = Physics2D.Linecast (lastHandlePos, sensorHandleL2.position, layerIdGroundHandlesMask);
			else
				hit = Physics2D.Linecast (sensorHandleL2.position, sensorHandleL2.position, layerIdGroundHandlesMask); 
		
		
			if (hit.collider != null) {
			
				// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
				bool _canCatch = true;
				if ((lastCatchedClimbHandle == hit.collider.gameObject) && velocity.y >= 0.0f) {
					_canCatch = false;
				}
			
				if (_canCatch) {
					catchedClimbHandle = hit.collider.gameObject;
				
					Vector3 handlePos = catchedClimbHandle.transform.position;
					Vector3 newPos = new Vector3 ();
					newPos.x = handlePos.x + myHalfWidth;
					newPos.y = handlePos.y - 2.4f; //myHeight;
					//transform.position = newPos;
				
					canPullUp = canClimbPullUp ();
				
					if (canPullUp) {
						//climbAfterPos.x = catchedClimbHandle.transform.position.x;
						//climbAfterPos.y = catchedClimbHandle.transform.position.y;
					}
				
					velocity.x = 0.0f;
					velocity.y = 0.0f;
				
					climbBeforePos = transform.position;
					climbAfterPos = newPos;
					climbDistToClimb = climbAfterPos - climbBeforePos;
					climbToJumpDuration = climbDistToClimb.magnitude * 0.5f;
				
					setState (State.CLIMB); 
					setAction (Action.CLIMB_JUMP_TO_CATCH);
					climbDuration = 0.0f;
					lastFrameHande = false;

					return true;
				}
			}
		
			lastHandlePos = sensorHandleL2.position;
			return false;
		}
	}

	bool tryCatchRope(){

		if (dir () == Vector2.right) {

			if (justJumpFromRope){
				lastHandlePos = sensorHandleR2.position;
				return false;
			}

			RaycastHit2D hit; 
			if (lastFrameHande)
				hit = Physics2D.Linecast (lastHandlePos, sensorHandleR2.position, layerIdRopesMask);
			else
				hit = Physics2D.Linecast (sensorHandleR2.position, sensorHandleR2.position, layerIdRopesMask); 

			if (hit.collider != null) {
				// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
				bool _canCatch = true;
//				if ((lastCatchedClimbHandle == hit.collider.gameObject) && velocity.y >= 0.0f) {
//					_canCatch = false;
//				}
				
				if (_canCatch) {

					catchedRopeLink = hit.collider.transform.GetComponent<RopeLink>();
					catchedRope = catchedRopeLink.rope;

					catchedRope.chooseDriver(catchedRopeLink.transform);

					float forceRatio = Mathf.Abs( velocity.x ) / JumpLongSpeed;
					float force = RopeSwingForce * forceRatio;// * Time.deltaTime;

					if( velocity.x < 0f ){
						//catchedRope.swing(-Vector2.right, force);
						catchedRope.setSwingMotor(-Vector2.right, force, 0.25f);
					}else if (velocity.x > 0){ 
						//catchedRope.swing(Vector2.right, force);
						catchedRope.setSwingMotor(Vector2.right, force, 0.25f);
					}

					velocity.x = 0.0f;
					velocity.y = 0.0f;

					setState(State.CLIMB_ROPE);
					setAction(Action.ROPECLIMB_IDLE);

					transform.position = catchedRopeLink.transform.position;
					transform.rotation = catchedRopeLink.transform.rotation;

					ropeLinkCatchOffset = 0.0f;

					return true;
				}
			}
			
			lastHandlePos = sensorHandleR2.position;
			return false;
			
		} else {

			if (justJumpFromRope){
				lastHandlePos = sensorHandleL2.position;
				return false;
			}

			//RaycastHit2D hit = Physics2D.Linecast(sensorHandleL1.position, sensorHandleL2.position, layerIdGroundHandlesMask); 
			RaycastHit2D hit; 
			if (lastFrameHande)
				hit = Physics2D.Linecast (lastHandlePos, sensorHandleL2.position, layerIdRopesMask);
			else
				hit = Physics2D.Linecast (sensorHandleL2.position, sensorHandleL2.position, layerIdRopesMask); 
			
			
			if (hit.collider != null) {
				
				// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
				bool _canCatch = true;
//				if ((lastCatchedClimbHandle == hit.collider.gameObject) && velocity.y >= 0.0f) {
//					_canCatch = false;
//				}				
				if (_canCatch) {
//					catchedClimbHandle = hit.collider.gameObject;
//					
//					Vector3 handlePos = catchedClimbHandle.transform.position;
//					Vector3 newPos = new Vector3 ();
//					newPos.x = handlePos.x + myHalfWidth;
//					newPos.y = handlePos.y - 2.4f; //myHeight;
//					//transform.position = newPos;
//					
//					canPullUp = canClimbPullUp ();
//					
//					if (canPullUp) {
//						//climbAfterPos.x = catchedClimbHandle.transform.position.x;
//						//climbAfterPos.y = catchedClimbHandle.transform.position.y;
//					}
//					
//					velocity.x = 0.0f;
//					velocity.y = 0.0f;
//					
//					climbBeforePos = transform.position;
//					climbAfterPos = newPos;
//					climbDistToClimb = climbAfterPos - climbBeforePos;
//					climbToJumpDuration = climbDistToClimb.magnitude * 0.5f;
//					
//					setState (State.CLIMB); 
//					setAction (Action.CLIMB_JUMP_TO_CATCH);
//					climbDuration = 0.0f;
//					lastFrameHande = false;

					catchedRopeLink = hit.collider.transform.GetComponent<RopeLink>();
					catchedRope = catchedRopeLink.rope;
					
					catchedRope.chooseDriver(catchedRopeLink.transform);

					float forceRatio = Mathf.Abs( velocity.x ) / JumpLongSpeed;
					float force = RopeSwingForce * forceRatio;// * Time.deltaTime;

					if( velocity.x < 0f ){
						//catchedRope.swing(-Vector2.right, force);
						catchedRope.setSwingMotor(-Vector2.right, force, 0.25f);
					}else if (velocity.x > 0){ 
						//catchedRope.swing(Vector2.right, force);
						catchedRope.setSwingMotor(Vector2.right, force, 0.25f);
					}

					velocity.x = 0.0f;
					velocity.y = 0.0f;
					
					setState(State.CLIMB_ROPE);
					setAction(Action.ROPECLIMB_IDLE);
					
					transform.position = catchedRopeLink.transform.position;
					transform.rotation = catchedRopeLink.transform.rotation;

					ropeLinkCatchOffset = 0.0f;
					return true;
				}
			}
			
			lastHandlePos = sensorHandleL2.position;
			return false;
		}
	}

	float ropeLinkCatchOffset = 0.0f;
//	bool canJumpToLayer(int testLayerID, ref Vector3 onLayerPlace){
//		if (isNotInState (State.ON_GROUND) || isNotInAction (Action.IDLE))
//			return false;
//
//		switch( playerCurrentLayer ){
//
//		case 0:
//			if( testLayerID == 0 ) return false;
//
//			Vector2 rayOrigin = transform.position;
//			rayOrigin.y += 0.1f;
//			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, 1.5f, _layerIdGroundFarMask);
//			if( !hit.collider ) return false;
//
//			float distToFarPlatform = Mathf.Abs (hit.point.y - rayOrigin.y);
//			if( distToFarPlatform == 0.0f ) { // jezeli platforma far sasiaduje bezposrednio z near
//
//				rayOrigin.y += 1.0f;
//				hit = Physics2D.Raycast (rayOrigin, Vector2.up, 2.5f, _layerIdGroundFarMask);
//				if( hit.collider ) return false;
//				else{
//					onLayerPlace = transform.position;
//					onLayerPlace.y += 1.0f;
//					return true;
//				}
//
//			}else if( Mathf.Abs((distToFarPlatform+0.1f)-1.0f) < 0.01f ){ // jezeli platforma far jest o 1.0 od near
//
//				rayOrigin.y += 2.0f;
//				hit = Physics2D.Raycast (rayOrigin, Vector2.up, 2.5f, _layerIdGroundFarMask);
//				if( hit.collider ) return false;
//				else{
//					onLayerPlace = transform.position;
//					onLayerPlace.y += 2.0f;
//					return true;
//				}
//
//			}else{ // na inne nie mozemy skoczyc
//				return false;
//			}
//			break;
//
//		case 1:
//			return testLayerID == 0;
//			break;
//		}
//
//		return false;
//	}

	bool canClimbPullUp(){

		if (!catchedClimbHandle)
			return false;

		Vector2 rayOrigin = catchedClimbHandle.transform.parent.transform.position;
		rayOrigin.x += 0.5f;
		rayOrigin.y += 0.25f;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, 0.5f, layerIdGroundMask);
		//print ( "canClimbPullUp : " + hit);
		return !hit.collider;
	}

	GameObject canClimbPullDown(){
		if (!isInState (State.ON_GROUND) || !(isInAction (Action.IDLE) || isInAction(Action.CROUCH_IDLE)) )
			return null;

//		// najpierw badam czy stoje na krawedzi odpowiednio zwrocony
//		if (dir () == Vector2.right) { //
//
//		} else {
//
//		}

		//Vector2 rayOrigin = sensorDown1.position; // transform.position;
		//RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up , 0.5f, layerIdGroundMask);

		Vector2 rayOrigin = sensorDown1.position; // transform.position;
		//Vector2 rayDir = (dir () == Vector2.right ? -Vector2.right : Vector2.right);
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right , myWidth, layerIdGroundHandlesMask);
		//print ( "canClimbPullDown : " + hit.collider.gameObject);

		if (hit.collider) { 
			//return hit.collider.gameObject;
			// badam czy stoje na krawedzi odpowiednio zwrocony
			if (dir () == Vector2.right) { //

				// pod lewa noga musi byc przepasc
				rayOrigin = sensorDown1.position;
				if( Physics2D.Raycast (rayOrigin, -Vector2.up , 0.5f, layerIdGroundMask).collider ) return null;
				else return hit.collider.gameObject;

			} else {

				// pod prawa noga musi byc przepasc
				rayOrigin = sensorDown3.position;
				if( Physics2D.Raycast (rayOrigin, -Vector2.up , 0.5f, layerIdGroundMask).collider ) return null;
				else return hit.collider.gameObject;

			}

		} else {
			return null;
		}
	}

	bool onMount(){
		Vector2 rayOrigin = transform.position;
		rayOrigin.y += 1.0f;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, 1.0f, layerIdMountMask);
		return hit.collider;
	}
	bool onMount(Vector3 posToCheck){
		Vector2 rayOrigin = posToCheck;
		rayOrigin.y += 1.0f;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, 1.0f, layerIdMountMask);
		return hit.collider;
	}

	void SetImpulse(Vector2 imp) { impulse = imp; }
	Vector2 getImpulse() { return impulse; }
	void addImpulse(Vector3 imp) { impulse += imp; }
	void addImpulse(Vector2 imp) { 
		impulse.x += imp.x; 
		impulse.y += imp.y; 
	}
	
	/*////////////////////////////////////////////////////////////*/
	
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
		CROUCH_IDLE,
		CROUCH_LEFT,
		CROUCH_RIGHT,
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
		ROPECLIMB_DOWN
	};
	
	public enum State
	{
		ON_GROUND = 0,
		IN_AIR,
		CLIMB,
		MOUNT,
		CLIMB_ROPE,
		OTHER
	};

	State getState() { 
		return state; 
	}
	bool setState(State newState){
		
		//print ("setState oldState : " + state);
		//print ("setState newState : " + newState);
		
		if (state == newState)
			return false;

		currentStateTime = 0.0f;

		//print ("setState : " + newState + " ustawiona");
		//print ("============================");

		state = newState;

		switch (state) {
		case State.IN_AIR:
 			startFallPos = transform.position;
			break;
		};

		return true;
	}
	bool isInState(State test) {
		return state == test;
	}
	bool isNotInState(State test) {
		return state != test;
	}
	
	/*////////////////////////////////////////////////////////////*/
	
	
	Action getAction(){
		return action;
	}
	bool setAction(Action newAction){
		
		//print ("setAction try : " + newAction);
		
		if (action == newAction)
			return false;

		//print ("setAction oldAction : " + action);
		//print ("setAction newAction : " + newAction);
		//print ("setAction : " + newAction + " ustawiona");
		//print ("============================");
		
		action = newAction;
		currentActionTime = 0.0f;

		animator.speed = 1.0f;

		switch (newAction) {
			
		case Action.IDLE:
			animator.SetTrigger("idle");
			break;
			
		case Action.WALK_LEFT:
		case Action.WALK_RIGHT:
			animator.SetTrigger("walk");
			break;
			
		case Action.RUN_LEFT:
		case Action.RUN_RIGHT:
			animator.SetTrigger("run");
			break;

		case Action.TURN_STAND_LEFT:
			animator.Play("stand_turn_left");
			break;

		case Action.TURN_STAND_RIGHT:
			animator.Play("stand_turn_right");
			break;

		case Action.PREPARE_TO_JUMP:
			animator.SetTrigger("preparetojump");
			break;

		case Action.JUMP:
			animator.SetTrigger("jump");
			break;

		case Action.JUMP_LEFT:
		case Action.JUMP_LEFT_LONG:
		case Action.JUMP_RIGHT:
		case Action.JUMP_RIGHT_LONG:
			animator.SetTrigger("jumpleftright");
			break;

		case Action.LANDING_HARD:
			animator.SetTrigger("landing_hard");
			break;

		case Action.CLIMB_PREPARE_TO_JUMP:
			//animator.SetTrigger("climb_preparetojump");
			break;
		case Action.CLIMB_JUMP_TO_CATCH:
			animator.SetTrigger("climb_jump");
			break;
		case Action.CLIMB_CATCH:
			animator.SetTrigger("climb_catch");
			break;
		case Action.CLIMB_CLIMB:
			animator.SetTrigger("climb_climb");
			break;

		case Action.CLIMB_PULLDOWN:
			animator.SetTrigger("climb_pulldown");
			break;

		case Action.MOUNT_IDLE:
			//animator.SetTrigger("mount_idle");
			//animator.StopPlayback();
			animator.speed = 0.0f;
			break;

		case Action.MOUNT_LEFT:
			//animator.SetTrigger("mount_left");
			animator.Play("mount_right");
			break;
		case Action.MOUNT_RIGHT:
			//animator.SetTrigger("mount_right");
			animator.Play("mount_left");
			break;
		case Action.MOUNT_UP:
			//animator.SetTrigger("mount_up");
			animator.Play("mount_up");
			break;
		case Action.MOUNT_DOWN:
			//animator.SetTrigger("mount_down");
			animator.Play("mount_down");
			break;

		case Action.CROUCH_IDLE:
			//animator.SetTrigger("crouchidle");
			animator.Play("crouch_leftright");
			animator.speed = 0f;
			break;

		case Action.CROUCH_LEFT:
			animator.Play("crouch_leftright");
			break;
		case Action.CROUCH_RIGHT:
			animator.Play("crouch_leftright");
			//animator.SetTrigger("crouchleftright");
			break;

		case Action.ROPECLIMB_IDLE:
			animator.SetTrigger("climbrope_idle");
			//animator.speed = 0f;
			break;

		case Action.ROPECLIMB_UP:
		case Action.ROPECLIMB_DOWN:
			animator.SetTrigger("climbrope");
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
	
	/*////////////////////////////////////////////////////////////*/

	BoxCollider2D coll;
	Animator animator;
	Transform sensorLeft1;
	Transform sensorLeft2;
	Transform sensorLeft3;
	Transform sensorRight1;
	Transform sensorRight2;
	Transform sensorRight3;
	Transform sensorDown1;
	Transform sensorDown2;
	Transform sensorDown3;
	
	Transform sensorHandleL2;
	Transform sensorHandleR2;
	
	Transform gfx;
	//aa
	[SerializeField]
	Vector3 velocity;
	Vector3 lastVelocity;
	Vector3 lastSwingPos;
	[SerializeField]
	Vector2 swingVelocity;
	Vector3 impulse;
	Vector3 startFallPos;

	Vector3 mountJumpStartPos;

	float desiredSpeedX = 0.0f;

	float currentActionTime = 0.0f;
	float currentStateTime = 0.0f;

	float myWidth;
	float myHalfWidth;
	float myHeight;
	float myHalfHeight;
	
	int layerIdGroundMask;
	int layerIdGroundPermeableMask;
	int layerIdGroundAllMask;
	int layerIdLastGroundTypeTouchedMask;
	int layerIdGroundHandlesMask;
	int layerIdRopesMask;
	
	int layerIdMountMask;
	
	GameObject catchedClimbHandle;
	GameObject lastCatchedClimbHandle;
	bool canPullUp;
	public NewRope catchedRope;
	public RopeLink catchedRopeLink;

	bool jumpFromMount = false;
	float climbDistFromWall;
	float climbDuration;
	Vector2 climbDir;
	
	Vector3 climbBeforePos;
	Vector3 climbAfterPos;
	Vector3 climbDistToClimb;
	float climbToJumpDuration;
	
	float groundUnderFeet;
	
	bool gamePaused = false;
	float distToMove;
	Vector3 oldPos;
	float newPosX;
	
	Vector3 lastHandlePos;
	bool lastFrameHande;
	
	int playerCurrentLayer;
	bool wantGetUp = false;

	[SerializeField]
	private State state;
	[SerializeField]
	private Action action;
}
