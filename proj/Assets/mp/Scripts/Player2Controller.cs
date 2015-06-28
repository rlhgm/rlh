using UnityEngine;
using System.Collections;

public class Player2Controller : MonoBehaviour {

	public BoxCollider2D coll;

	BoxCollider2D handerLeft;
	BoxCollider2D handerRight;
	Animator animator;

	public Vector3 velocity;
	public Vector3 impulse;
	
	public float jumpImpulse = 4.0f; 
	public float jumpLongImpulse = 4.5f; 
	public float gravityForce = -1.0f;
	//public float flySlowDown = 1.0f;
	public float MAX_SPEED_Y = 15.0f;

	public float WALK_SPEED = 1.0f;
	public float RUN_SPEED = 2.0f;
	public float JUMP_SPEED = 1.0f;
	public float JUMP_LONG_SPEED = 2.0f;
	public float CLIMB_DURATION = 1.5f;

	public float MountSpeed = 2.0f; // ile na sek.
	public float MountJumpDist = 4.0f; // następnie naciskasz spacje a on skacze
		//[2015-06-18 17:58:40] Rafał Sankowski: i jeśli nadal trzymasz spacje
		//[2015-06-18 17:59:02] Rafał Sankowski: to po przeskoczeniu ustalonej wartości (mowilismy o tym) się lapie
	Vector3 mountJumpStartPos;

	public float flyControlParam = 2.0f; // ile przyspiesza na sekunde lecac
	public float flySlowDownParam = 1.0f; // ile hamuje na sekunde lecac
	/// <summary>
	/// ile jednosek predkosci hamuje na sekunde
	/// </summary>
	public float speedUpParam = 7.0f; // ile jednosek predkosci hamuje na sekunde
	public float breakParam = 4.0f; // ile jednosek predkosci hamuje na sekunde
	float desiredSpeedX = 0.0f;
	//public float stopToWalkDuration = 0.5f; // ile sekund zajmuje przejscie ze stania do chodu
	//public float stopToRunDuration = 0.5f; // ile sekund zajmuje przejscie ze stania do chodu
	//public float walkToRunDuration = 1.5f; // ile sekund zajmuje przejscie z chodu do biegu

	//float timeFromStartWalkRun = 0.0f;
	//float walkRunStartSpeedX = 0.0f;
	//float currentWalkRunDuration = 0.0f;
	//bool walkRunSpeedUp = false;
	//bool walkRunSlowDown = false;
	//float timeToSpeedUp;
	//float speedDiff;

	public Transform respawnPoint;

	float currentActionTime = 0.0f;
	float currentStateTime = 0.0f;

	public KeyCode keyLeft = KeyCode.LeftArrow;
	public KeyCode keyRight = KeyCode.RightArrow;
	public KeyCode keyRun = KeyCode.LeftShift;
	public KeyCode keyUp = KeyCode.UpArrow;
	public KeyCode keyDown = KeyCode.DownArrow;
	public KeyCode keyJump = KeyCode.Space;

	private Transform sensorLeft1;
	private Transform sensorLeft2;
	private Transform sensorLeft3;
	private Transform sensorRight1;
	private Transform sensorRight2;
	private Transform sensorRight3;
	private Transform sensorDown1;
	private Transform sensorDown2;
	private Transform sensorDown3;

	//private Transform sensorHandleL1;
	private Transform sensorHandleL2;
	//private Transform sensorHandleR1;
	private Transform sensorHandleR2;

	private Transform gfx;
	
	float myWidth;
	float myHalfWidth;
	float myHeight;
	public float myHalfHeight;

	private int _layerIdGroundMask;
	private int _layerIdGroundHandlesMask;
	private int _layerIdGroundFarMask;
	private int _layerIdGroundFarHandlesMask;
	private int currentLayerIdGroundMask;
	private int currentLayerIdGroundHandlesMask;

	private int layerIdMountMask;

	GameObject catchedClimbHandle;
	public GameObject lastCatchedClimbHandle;
	bool canPullUp;



	bool jumpFromMount = false;
	private float climbDistFromWall;
	private float climbDuration;
	private Vector2 climbDir;
	
	private Vector3 climbBeforePos;
	private Vector3 climbAfterPos;
	private Vector3 climbDistToClimb;
	private float climbToJumpDuration;
	//private Vector3 climbJumpPos;
	//private Vector3 climbDistToJump;
	
	public float CLIMBDUR_PREPARE_TO_JUMP = 0.5f;
	//public float CLIMBDUR_JUMP_TO_CATCH_SPEED = 0.2f; // jednostka w 0.2f
	public float CLIMBDUR_JUMP_TO_CATCH = 0.2f; // jednostka w 0.2f
	public float CLIMBDUR_CATCH = 0.5f;
	public float CLIMBDUR_CLIMB = 0.75f;
	
	public float groundUnderFeet;
	//public float toNextHandleDuration = 0.5f;
	//float justLetGoHandle = 0.0f;


	bool gamePaused = false;
	float distToMove;
	Vector3 oldPos;
	float newPosX;

	Vector3 lastHandlePos;
	bool lastFrameHande;

	public int playerCurrentLayer;

	void Awake(){
		coll = GetComponent<BoxCollider2D> ();
		gfx  = transform.Find("gfx").transform;
		animator = transform.Find("gfx").GetComponent<Animator>();

		handerLeft = transform.Find("handlerL").GetComponent<BoxCollider2D>();
		handerRight = transform.Find("handlerR").GetComponent<BoxCollider2D>();

		//print (handerLeft + " " + handerRight);

		sensorLeft1 = transform.Find("sensorLeft1").transform;
		sensorLeft2 = transform.Find("sensorLeft2").transform;
		sensorLeft3 = transform.Find("sensorLeft3").transform;
		sensorRight1 = transform.Find("sensorRight1").transform;
		sensorRight2 = transform.Find("sensorRight2").transform;
		sensorRight3 = transform.Find("sensorRight3").transform;
		sensorDown1 = transform.Find("sensorDown1").transform;
		sensorDown2 = transform.Find("sensorDown2").transform;
		sensorDown3 = transform.Find("sensorDown3").transform;

		//sensorHandleL1 = transform.Find("handlerL1").transform;
		sensorHandleL2 = transform.Find("handlerL2").transform;
		//sensorHandleR1 = transform.Find("handlerR1").transform;
		sensorHandleR2 = transform.Find("handlerR2").transform;

		//print (sensorHandleL1 + " " + sensorHandleL2 + " " + sensorHandleR1 + " " + sensorHandleR2);

		CLIMBDUR_PREPARE_TO_JUMP = 0.5f;
		CLIMBDUR_JUMP_TO_CATCH = 0.2f; // jednostka w 0.2f
		CLIMBDUR_CATCH = 0.5f;
		CLIMBDUR_CLIMB = 0.5f;
		
		WALK_SPEED = 3.0f;
		RUN_SPEED = 5.0f;
		//flySlowDown = 1.5f;
		JUMP_SPEED = 3.5f;
		JUMP_LONG_SPEED = 4.1f;
		
		jumpImpulse = 7.0f;
		jumpLongImpulse = 7.15f;
		gravityForce = -20.0f;
		MAX_SPEED_Y = 15.0f;
		
		CLIMB_DURATION = 1.5f;

		MountSpeed = 2.0f; // ile na sek.
		MountJumpDist = 4.0f; // następnie naciskasz spacje a on skacze

		flyControlParam = 8.0f; // ile przyspiesza na sekunde lecac
		flySlowDownParam = 5.0f; // ile hamuje na sekunde lecac

		speedUpParam = 7.0f; //WALK_SPEED; // ile jednosek predkosci przyspiesza na sekunde - teraz do pelnej predkosci chodu w 1.s
		breakParam = WALK_SPEED * 2.0f; // ile jednosek predkosci hamuje na sekunde - teraz z automatu w 0.5 sek.
		desiredSpeedX = 0.0f;

		//stopToWalkDuration = 0.5f; // ile sekund zajmuje przejscie ze stania do chodu
		//stopToRunDuration = 0.5f; // ile sekund zajmuje przejscie z chodu do biegu
		//walkToRunDuration = 1.5f; // ile sekund zajmuje przejscie z chodu do biegu
		//timeFromStartWalkRun = 0.0f;
		//walkRunStartSpeedX = 0.0f;
		//walkRunSpeedUp = false;

		myWidth = coll.size.x;
		myHalfWidth = myWidth * 0.5f;
		myHeight = coll.size.y;
		myHalfHeight = myHeight * 0.5f;

		lastHandlePos = new Vector3();
		lastFrameHande = false;
	}

//	bool fff(ref int i){
//		i += 5;
//		return true;
//	}

	void setCurrentPlayerLayer(int newCurrentLayer){

		playerCurrentLayer = newCurrentLayer;

		switch (newCurrentLayer) {

		case 0:
			currentLayerIdGroundMask = _layerIdGroundMask;
			currentLayerIdGroundHandlesMask = _layerIdGroundHandlesMask;
			gfx.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
			break;

		case 1:
			currentLayerIdGroundMask = _layerIdGroundFarMask;
			currentLayerIdGroundHandlesMask = _layerIdGroundFarHandlesMask;
			gfx.GetComponent<SpriteRenderer>().sortingLayerName = "PlayerFar";
			break;

		}
	}

	void Start () {
		_layerIdGroundMask = 1 << LayerMask.NameToLayer("Ground");
		_layerIdGroundHandlesMask = 1 << LayerMask.NameToLayer("GroundHandles");
		_layerIdGroundFarMask = 1 << LayerMask.NameToLayer("GroundFar");
		_layerIdGroundFarHandlesMask = 1 << LayerMask.NameToLayer("GroundFarHandles");

		layerIdMountMask = 1 << LayerMask.NameToLayer("Mount");

		velocity = new Vector3 (0, 0, 0);
		impulse = new Vector3 (0, 0, 0);
		
		setState (State.ON_GROUND);
		//setAction (Action.IDLE);
		action = Action.IDLE;

		climbDuration = 0.0f;
		catchedClimbHandle = null;
		canPullUp = false;
		//toNextHandleDuration = 0.4f;

		jumpFromMount = false;

		setCurrentPlayerLayer (0);

//		int jjj = 10;
//		print (jjj);
//		fff (ref jjj);
//		print (jjj);
	}

	public void die(){
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		setAction (Action.IDLE);
		setState (State.ON_GROUND);
		transform.position = respawnPoint.position;
	}

	//public Transform birdPrefab;

	void OnTriggerEnter2D(Collider2D other) {
		//print( "PLAYER OnTriggerEnter" );
		if (other.gameObject.tag == "Bird") {
			if( isInState(State.MOUNT) ){
				velocity.x = 0.0f;
				velocity.y = 0.0f;
				setAction(Action.JUMP);
				setState(State.IN_AIR);
			}
		}
	}
	//aa

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	
		if (Input.GetKeyDown (KeyCode.P)) {
			gamePaused = !gamePaused;
		}

//		if (Input.GetKeyDown (KeyCode.B)) {
//			//Bird newBird = new Bird();
//			//newBird.transform.position = transform.position;
//			//Instantiate<Bird>(
//			Instantiate(birdPrefab,transform.position,Quaternion.identity);
//		}

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
		//justLetGoHandle = false;

		if (Input.GetKeyDown (keyJump)) {
			Vector3 newPlayerPos = new Vector3();
			if( Input.GetKey(keyUp) ){
				if( canJumpToLayer(1,ref newPlayerPos) ){
					setCurrentPlayerLayer(1);
					transform.position = newPlayerPos;
					return;
				}
			}
			if( Input.GetKey (keyDown)){
				if( canJumpToLayer(0,ref newPlayerPos) ){
					setCurrentPlayerLayer(0);
					//transform.position = newPlayerPos;
					setState(State.IN_AIR);
					setAction(Action.JUMP);
					return;
				}
			}
		}

		if (Input.GetKeyDown (keyJump)) {
			keyJumpDown ();
		} else if (Input.GetKeyUp (keyJump)) {
			keyJumpUp ();
		}
		
		if (Input.GetKeyDown (keyLeft)) {
			keyLeftDown();
		} else if (Input.GetKeyDown (keyRight)) {
			keyRightDown();
		}
		
		if (Input.GetKeyUp (keyLeft)) {
			keyLeftUp();
		} else if (Input.GetKeyUp (keyRight)) {
			keyRightUp();
		}

		if (Input.GetKeyDown (keyRun)) {
			keyRunDown();
		} else if (Input.GetKeyUp (keyRun)) {
			keyRunUp();
		}

		if (Input.GetKeyDown (keyUp)) {
			keyUpDown();
		} else if (Input.GetKeyUp (keyUp)) {
			keyUpUp();
		}
		if (Input.GetKeyDown (keyDown)) {
			keyDownDown();
		} else if (Input.GetKeyUp (keyDown)) {
			keyDownUp();
		}

		currentActionTime += Time.deltaTime;
		currentStateTime += Time.deltaTime;

		oldPos = transform.position;
		//float oldPosX = oldPos.x;
		newPosX = oldPos.x;
		distToMove = 0.0f;

		switch (action) {
		case Action.IDLE:
			Act_IDLE();
			break;

		case Action.PREPARE_TO_JUMP:
			if( currentActionTime >= 0.2f ){
				//print( currentActionTime );
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

		};

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

					}else{
						velocity.x = 0.0f;
						velocity.y = 0.0f;
						setAction(Action.MOUNT_IDLE);
						setState(State.MOUNT);
					}
				}//aaa
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

				if( dir () == Vector2.right ){

					//RaycastHit2D hit = Physics2D.Linecast(sensorHandleR1.position, sensorHandleR2.position, layerIdGroundHandlesMask); 
					RaycastHit2D hit; 
					if( lastFrameHande )
						hit = Physics2D.Linecast(lastHandlePos, sensorHandleR2.position, currentLayerIdGroundHandlesMask); 
					else
						hit = Physics2D.Linecast(sensorHandleR2.position, sensorHandleR2.position, currentLayerIdGroundHandlesMask); 

//					// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
//					bool _canCatch = true;
//					if( lastCatchedClimbHandle && velocity.y > 0 ){
//						_canCatch = false;
//					}

					if( hit.collider != null ){
						//print ( hit.collider.gameObject.transform.position );

						// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
						bool _canCatch = true;
						if( (lastCatchedClimbHandle == hit.collider.gameObject) && velocity.y >= 0.0f ){
							_canCatch = false;
						}

						if( _canCatch ){
							catchedClimbHandle = hit.collider.gameObject;

      						Vector3 handlePos = catchedClimbHandle.transform.position;
							Vector3 newPos = new Vector3();
							newPos.x = handlePos.x - myHalfWidth;
							newPos.y = handlePos.y - 2.4f; //myHeight;
							//transform.position = newPos;

							canPullUp = canClimbPullUp();

							if( canPullUp ){
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

							setState(State.CLIMB); 
							setAction(Action.CLIMB_JUMP_TO_CATCH);
							climbDuration = 0.0f;

							return;
						}
					}

					lastHandlePos = sensorHandleR2.position;

				}else{

					//RaycastHit2D hit = Physics2D.Linecast(sensorHandleL1.position, sensorHandleL2.position, layerIdGroundHandlesMask); 
					RaycastHit2D hit; 
					if( lastFrameHande )
						hit = Physics2D.Linecast(lastHandlePos, sensorHandleL2.position, currentLayerIdGroundHandlesMask); 
					else
						hit = Physics2D.Linecast(sensorHandleL2.position, sensorHandleL2.position, currentLayerIdGroundHandlesMask); 


					if( hit.collider != null ){

						// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
						bool _canCatch = true;
						if( (lastCatchedClimbHandle == hit.collider.gameObject) && velocity.y >= 0.0f ){
							_canCatch = false;
						}

						if( _canCatch ){
							catchedClimbHandle = hit.collider.gameObject;

							Vector3 handlePos = catchedClimbHandle.transform.position;
							Vector3 newPos = new Vector3();
							newPos.x = handlePos.x + myHalfWidth;
							newPos.y = handlePos.y - 2.4f; //myHeight;
							//transform.position = newPos;

							canPullUp = canClimbPullUp();

							if( canPullUp ){
								//climbAfterPos.x = catchedClimbHandle.transform.position.x;
								//climbAfterPos.y = catchedClimbHandle.transform.position.y;
							}

							velocity.x = 0.0f;
							velocity.y = 0.0f;
							
							climbBeforePos = transform.position;
							climbAfterPos = newPos;
							climbDistToClimb = climbAfterPos - climbBeforePos;
							climbToJumpDuration = climbDistToClimb.magnitude * 0.5f;
							
							setState(State.CLIMB); 
							setAction(Action.CLIMB_JUMP_TO_CATCH);
							climbDuration = 0.0f;

							return;
						}
					}

					lastHandlePos = sensorHandleL2.position;

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

			addImpulse(new Vector2(0.0f, gravityForce * Time.deltaTime));
			

			if( isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG) ){
				
				if( Input.GetKey(keyLeft) ){
					velocity.x -= (flyControlParam * Time.deltaTime);
					//if( velocity.x > 0.0f ) velocity.x = 0.0f;
				}else if ( Input.GetKey(keyRight) ){
					velocity.x += (flyControlParam * Time.deltaTime);
					if( velocity.x > 0.0f ) velocity.x = 0.0f;
				}else{
					velocity.x += (flySlowDownParam * Time.deltaTime);
					if( velocity.x > 0.0f ) velocity.x = 0.0f;
				}
				
			}else if( isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG) ){

				if( Input.GetKey(keyRight) ){
					velocity.x += (flyControlParam * Time.deltaTime);
				}else if( Input.GetKey(keyLeft) ) {
					velocity.x -= (flyControlParam * Time.deltaTime);
					if( velocity.x < 0.0f ) velocity.x = 0.0f;
				}else{
					velocity.x -= (flySlowDownParam * Time.deltaTime);
					if( velocity.x < 0.0f ) velocity.x = 0.0f;
				}
				
			}else if( isInAction(Action.JUMP) ){

				if( velocity.x > 0.0f ){

					velocity.x -= (flySlowDownParam * Time.deltaTime);
					if( velocity.x < 0.0f ) velocity.x = 0.0f;

				}else if(velocity.x < 0.0f) {

					velocity.x += (flySlowDownParam * Time.deltaTime);
					if( velocity.x > 0.0f ) velocity.x = 0.0f;

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
				float obstacleOnRoad = checkLeft( distToFall.x + 0.01f);
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
			if(velocity.y > MAX_SPEED_Y)
				velocity.y = MAX_SPEED_Y;
			if(velocity.y < -MAX_SPEED_Y)
				velocity.y = -MAX_SPEED_Y;
			
			distToFall.y = velocity.y * Time.deltaTime;
			
			if( distToFall.y > 0.0f ) { // leci w gore
				//transform.position = transform.position + distToFall;
			} else if( distToFall.y < 0.0f ) { // spada
				groundUnderFeet = checkDown( Mathf.Abs(distToFall.y) + 0.01f);
				if( groundUnderFeet >= 0.0f ){
					if( (groundUnderFeet < Mathf.Abs(distToFall.y)) || Mathf.Abs( groundUnderFeet - Mathf.Abs(distToFall.y)) < 0.01f  ){

 						lastCatchedClimbHandle = null;

						distToFall.y = -groundUnderFeet;
						velocity.x = 0.0f;
						velocity.y = 0.0f;
						setState(State.ON_GROUND);
						//setAction (Action.IDLE);
						//asdf
						resetActionAndState();
					}
				}

			}

			transform.position = transform.position + distToFall;

			break;
			
		case State.ON_GROUND:
			groundUnderFeet = checkDown(0.1f);
			if( groundUnderFeet < 0.0f ) {
				setState(State.IN_AIR);
				//setAction(Action.FALL);
				setAction(Action.JUMP);
			}
			break;
		};
		
	}

	int Act_IDLE(){
		//velocity.x = 0.0f;
		if( Input.GetKeyDown(keyDown) ){
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
				
				setAction(Action.CLIMB_PULLDOWN);
				setState(State.CLIMB);
			}
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
			setAction (Action.IDLE);
			setState (State.ON_GROUND);
			climbDuration = 0.0f;
			transform.position = climbAfterPos;
			resetActionAndState ();
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

			float velocityDamp = speedUpParam * Time.deltaTime;
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
			float velocityDamp = breakParam * Time.deltaTime;
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

	int Act_WALK(int dir){

		//currentWalkRunDuration += Time.deltaTime;
		//animator.speed = 1.0f;

		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f ) {
			setAction(Action.IDLE);
			resetActionAndState();
		}

		distToMove = velocity.x * Time.deltaTime;

		float distToObstacle = 0.0f;
		if (checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			setActionIdle();
		}
		//print (distToObstacle);

		newPosX += distToMove;		
		transform.position = new Vector3 (newPosX, oldPos.y, 0.0f);

		float distToGround = 0.0f;
		bool groundUnderFeet = checkGround (ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y+distToGround, 0.0f);
		}

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
		bool groundUnderFeet = checkGround (ref distToGround);
		if (groundUnderFeet) {
			transform.position = new Vector3 (newPosX, oldPos.y+distToGround, 0.0f);
		}

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
		if ( (isInAction (Action.IDLE) || moving(-1) || jumping() ) && isInState (State.ON_GROUND)) {
			if (checkLeft (0.1f) >= 0.0f) {
				//print ("cant move left");
				return false;
			}
			turnLeft ();
			if (Input.GetKey (keyRun)) {
				//startWalkOrRun(RUN_SPEED,stopToRunDuration);
				desiredSpeedX = RUN_SPEED;
				setAction (Action.RUN_LEFT);
				return true;
			} else {
				//startWalkOrRun(WALK_SPEED,stopToWalkDuration);
				desiredSpeedX = WALK_SPEED;
				setAction (Action.WALK_LEFT);
				return true;
			}
		} else if (isInState (State.MOUNT)) {
			if( !mounting() ){
				Vector3 playerPos = transform.position;
				playerPos.x -= 0.1f;
				if( onMount(playerPos) ){
					turnLeft();
					velocity.x = -MountSpeed;
					velocity.y = 0.0f;
					setAction(Action.MOUNT_LEFT);
					return true;
				}
			}
		}
		return false;
	}
	bool keyRightDown(){
		if ( (isInAction (Action.IDLE) || moving(1) || jumping()) && isInState(State.ON_GROUND) ) {
			if( checkRight (0.01f) >= 0.0f ) {
				//print ("cant move right");
				return false;
			}
			turnRight();
			if( Input.GetKey(keyRun) ){
				//startWalkOrRun(RUN_SPEED,stopToRunDuration);
				desiredSpeedX = RUN_SPEED;
				setAction(Action.RUN_RIGHT);
				return true;
			}else{
				//startWalkOrRun(WALK_SPEED,stopToWalkDuration);
				desiredSpeedX = WALK_SPEED;
				setAction(Action.WALK_RIGHT);
				return true;
			}
		} else if (isInState (State.MOUNT)) {
			if( !mounting() ){
				Vector3 playerPos = transform.position;
				playerPos.x += 0.1f;
				if( onMount(playerPos) ){
					turnRight();
					velocity.x = MountSpeed;
					velocity.y = 0.0f;
					setAction(Action.MOUNT_RIGHT);
					return true;
				}
			}
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
				desiredSpeedX = RUN_SPEED;
				setAction(Action.RUN_LEFT);
			}
			break;
			
		case Action.WALK_RIGHT:
			if( Input.GetKey(keyRight) ){
				//startWalkOrRun(RUN_SPEED,walkToRunDuration);
				desiredSpeedX = RUN_SPEED;
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
				desiredSpeedX = WALK_SPEED;
				setAction(Action.WALK_LEFT);
			}else{
				desiredSpeedX = 0.0f;
			}
			break;
			
		case Action.RUN_RIGHT:
			//startWalkOrRun(WALK_SPEED,walkToRunDuration);
			if( Input.GetKey(keyRight) ) {
				desiredSpeedX = WALK_SPEED;
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
			velocity.x = 0.0f;
			velocity.y = 0.0f;
			setAction(Action.JUMP);
			setState (State.IN_AIR);
			mountJumpStartPos = transform.position;
			jumpFromMount = true;
			//lastHandlePos = new Vector3();
			lastFrameHande = false;
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
	void keyDownDown(){
		if (isInState (State.MOUNT)) {
			if( !mounting () ){
				Vector3 playerPos = transform.position;
				playerPos.y -= 0.1f;
				if( onMount(playerPos) ){
					velocity.x = 0.0f;
					velocity.y = -MountSpeed;
					setAction(Action.MOUNT_DOWN);
				}
			}
		}
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
		addImpulse(new Vector2(0.0f, jumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP);

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}

	void jumpLeft(){
		//print ("jumpLeft");
		velocity.x = -JUMP_SPEED;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT);
		//jumpStartX = transform.position.x;

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}
	
	void jumpRight(){
		//print ("jumpRight");
		velocity.x = JUMP_SPEED;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_RIGHT);
		//jumpStartX = transform.position.x;

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}
	
	void jumpLongLeft(){
		//print ("jumpLongLeft");
		velocity.x = -JUMP_LONG_SPEED;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpLongImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT_LONG);
		//jumpStartX = transform.position.x;

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}
	
	void jumpLongRight(){
		//print ("jumpLongRight");
		velocity.x = JUMP_LONG_SPEED;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpLongImpulse));
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

	void resetActionAndState(){
		if (isInState (State.ON_GROUND)) {
			if (Input.GetKey (keyLeft)) {
				if( !keyLeftDown () )
					setActionIdle();
			} else if (Input.GetKey (keyRight)) {
				if( !keyRightDown () )
					setActionIdle();
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

	float checkLeft(float checkingDist){
		Vector2 rayOrigin = new Vector2( sensorLeft1.position.x, sensorLeft1.position.y );
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, currentLayerIdGroundMask);
		if (hit.collider != null) {
			//print( hit.collider.gameObject.transform.rotation.eulerAngles );
			//print( hit.collider.gameObject.transform.rotation.to );
			float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
			//print (angle);
			//return Mathf.Abs (hit.point.x - sensorLeft1.position.x);
			if( angle > 45.0f ) 
				return Mathf.Abs (hit.point.x - sensorLeft1.position.x);
			else 
				return -1.0f;
		} else {
			rayOrigin = new Vector2( sensorLeft2.position.x, sensorLeft2.position.y );
			hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, currentLayerIdGroundMask);
			if (hit.collider != null){
				//print( hit.collider.gameObject.transform.rotation.eulerAngles );
				//float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
				//print (angle);
				return Mathf.Abs (hit.point.x - sensorLeft2.position.x);
			} else {
				rayOrigin = new Vector2( sensorLeft3.position.x, sensorLeft3.position.y );
				hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, currentLayerIdGroundMask);
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
	
	float checkRight(float checkingDist){
		Vector2 rayOrigin = new Vector2( sensorRight1.position.x, sensorRight1.position.y );
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist, currentLayerIdGroundMask);
		if (hit.collider != null) {
			//print( hit.collider.gameObject.transform.rotation.eulerAngles );
			float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
			//print (angle);
			//return Mathf.Abs (hit.point.x - sensorRight1.position.x);
			if( angle > 45.0f ) return Mathf.Abs (hit.point.x - sensorRight1.position.x);
			else return -1.0f;
		} else {
			rayOrigin = new Vector2( sensorRight2.position.x, sensorRight2.position.y );
			hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist, currentLayerIdGroundMask);
			if (hit.collider != null){
				//print( hit.collider.gameObject.transform.rotation.eulerAngles );
				//float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
				//print (angle);
				return Mathf.Abs (hit.point.x - sensorRight2.position.x);
			} else {
				rayOrigin = new Vector2( sensorRight3.position.x, sensorRight3.position.y );
				hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist, currentLayerIdGroundMask);
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
	
	float checkDown(float checkingDist){

		Vector2 rayOrigin = new Vector2( sensorDown1.position.x, sensorDown1.position.y );
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, currentLayerIdGroundMask);
		if (hit.collider != null) {
			return Mathf.Abs (hit.point.y - sensorDown1.position.y);
		} else {
			rayOrigin = new Vector2( sensorDown2.position.x, sensorDown2.position.y );
			hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, currentLayerIdGroundMask);
			if (hit.collider != null){
				return Mathf.Abs (hit.point.y - sensorDown2.position.y);
			} else {
				rayOrigin = new Vector2( sensorDown3.position.x, sensorDown3.position.y );
				hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, currentLayerIdGroundMask);
				if (hit.collider != null){
					return Mathf.Abs (hit.point.y - sensorDown3.position.y);
				} else {
					return -1.0f;
				}
			}
		}  
	}

	bool checkGround (ref float distToGround){
		bool groundUnderFeet = false;

		//bool groundUnderFeet1 = false;
		//bool groundUnderFeet2 = false;
		//bool groundUnderFeet3 = false;

		float checkingDist = myHeight + 0.75f;

		Vector2 rayOrigin1 = sensorDown1.position;
		rayOrigin1.y += myHeight;
		RaycastHit2D hit1 = Physics2D.Raycast (rayOrigin1, -Vector2.up, checkingDist, currentLayerIdGroundMask);

		Vector2 rayOrigin2 = sensorDown2.position;
		rayOrigin2.y += myHeight;
		RaycastHit2D hit2 = Physics2D.Raycast (rayOrigin2, -Vector2.up, checkingDist, currentLayerIdGroundMask);

		Vector2 rayOrigin3 = sensorDown3.position;
		rayOrigin3.y += myHeight;
		RaycastHit2D hit3 = Physics2D.Raycast (rayOrigin3, -Vector2.up, checkingDist, currentLayerIdGroundMask);

		float dist1;
		float dist2;
		float dist3;

		if (hit1.collider != null) {
			//dist1 = myHeight - Mathf.Abs (hit1.point.y - sensorDown1.position.y);
			dist1 = rayOrigin1.y - hit1.point.y;
			groundUnderFeet = true;
			distToGround = dist1;
		}
		if (hit2.collider != null) {
			//dist2 = myHeight - Mathf.Abs (hit2.point.y - sensorDown2.position.y);
			dist2 = rayOrigin2.y - hit2.point.y;
			if( groundUnderFeet ){
				if( distToGround > dist2) distToGround = dist2;
			}else{
				groundUnderFeet = true;
				distToGround = dist2;
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
			}
		}

		if (groundUnderFeet) {
			distToGround = myHeight - distToGround;
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

	bool canJumpToLayer(int testLayerID, ref Vector3 onLayerPlace){
		if (isNotInState (State.ON_GROUND) || isNotInAction (Action.IDLE))
			return false;

		switch( playerCurrentLayer ){

		case 0:
			if( testLayerID == 0 ) return false;

			Vector2 rayOrigin = transform.position;
			rayOrigin.y += 0.1f;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, 1.5f, _layerIdGroundFarMask);
			if( !hit.collider ) return false;

			float distToFarPlatform = Mathf.Abs (hit.point.y - rayOrigin.y);
			if( distToFarPlatform == 0.0f ) { // jezeli platforma far sasiaduje bezposrednio z near

				rayOrigin.y += 1.0f;
				hit = Physics2D.Raycast (rayOrigin, Vector2.up, 2.5f, _layerIdGroundFarMask);
				if( hit.collider ) return false;
				else{
					onLayerPlace = transform.position;
					onLayerPlace.y += 1.0f;
					return true;
				}

			}else if( Mathf.Abs((distToFarPlatform+0.1f)-1.0f) < 0.01f ){ // jezeli platforma far jest o 1.0 od near

				rayOrigin.y += 2.0f;
				hit = Physics2D.Raycast (rayOrigin, Vector2.up, 2.5f, _layerIdGroundFarMask);
				if( hit.collider ) return false;
				else{
					onLayerPlace = transform.position;
					onLayerPlace.y += 2.0f;
					return true;
				}

			}else{ // na inne nie mozemy skoczyc
				return false;
			}
			break;

		case 1:
			return testLayerID == 0;
			break;
		}

		return false;
	}

	bool canClimbPullUp(){

		if (!catchedClimbHandle)
			return false;

		Vector2 rayOrigin = catchedClimbHandle.transform.parent.transform.position;
		rayOrigin.x += 0.5f;
		rayOrigin.y += 0.5f;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, 2.0f, currentLayerIdGroundMask);
		//print ( "canClimbPullUp : " + hit);
		return !hit.collider;
	}

	GameObject canClimbPullDown(){
		if (!isInState (State.ON_GROUND) || !isInAction (Action.IDLE))
			return null;

		Vector2 rayOrigin = transform.position;
		Vector2 rayDir = (dir () == Vector2.right ? -Vector2.right : Vector2.right);
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, rayDir, 0.5f, currentLayerIdGroundHandlesMask);
		//print ( "canClimbPullDown : " + hit.collider.gameObject);
		if (hit.collider)
			return hit.collider.gameObject;
		else 
			return null;
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
		//BREAK,
		PREPARE_TO_JUMP,
		JUMP,
		JUMP_LEFT,
		JUMP_LEFT_LONG,
		JUMP_RIGHT,
		JUMP_RIGHT_LONG,
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
		MOUNT_DOWN
	};
	
	public enum State
	{
		ON_GROUND = 0,
		IN_AIR,
		CLIMB,
		MOUNT,
		OTHER
	};
	
	public State state;
	public Action action;
	
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
			animator.SetTrigger("mount_idle");
			break;

		case Action.MOUNT_LEFT:
		case Action.MOUNT_RIGHT:
		case Action.MOUNT_UP:
		case Action.MOUNT_DOWN:
			animator.SetTrigger("mount_move");
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
}

//// Update is called once per frame
//void Update () {
//	if (Input.GetKey(KeyCode.Escape))
//		Application.Quit();
//	
//	SetImpulse(new Vector2(0.0f, 0.0f));
//	
//	//		if (Input.GetKeyDown (keyJump)) {
//	//			keyJumpDown ();
//	//		} else if (Input.GetKeyUp (keyJump)) {
//	//			keyJumpUp ();
//	//		}
//	
//	if (Input.GetKeyDown (keyLeft)) {
//		keyLeftDown();
//	} else if (Input.GetKeyDown (keyRight)) {
//		keyRightDown();
//	}
//	
//	if (Input.GetKeyUp (keyLeft)) {
//		keyLeftUp();
//	} else if (Input.GetKeyUp (keyRight)) {
//		keyRightUp();
//	}
//	
//	Vector3 oldPos = transform.position;
//	//float oldPosX = oldPos.x;
//	float newPosX = oldPos.x;
//	float distToMove;
//	
//	switch (action) {
//	case Action.IDLE:
//		velocity.x = 0.0f;
//		break;
//		
//	case Action.CLIMB_PREPARE_TO_JUMP:
//		climbDuration += Time.deltaTime;
//		if( climbDuration >= CLIMBDUR_PREPARE_TO_JUMP ){
//			setAction(Action.CLIMB_JUMP_TO_CATCH);
//			climbDuration = 0.0f;
//		}
//		break;
//	case Action.CLIMB_JUMP_TO_CATCH:
//		climbDuration += Time.deltaTime;
//		
//		if( climbDuration >= CLIMBDUR_JUMP_TO_CATCH ){
//			setAction(Action.CLIMB_CATCH);
//			climbDuration = 0.0f;
//			//transform.position = climbAfterPos;
//			transform.position = climbJumpPos;
//			
//			climbDistToClimb = climbAfterPos - climbJumpPos;
//		} else {
//			float ratio = climbDuration / CLIMBDUR_JUMP_TO_CATCH;
//			transform.position = climbBeforePos + climbDistToJump*ratio;
//		}
//		
//		break;
//	case Action.CLIMB_CATCH:
//		//			climbDuration += Time.deltaTime;
//		//			if( climbDuration >= CLIMBDUR_CATCH ){
//		//				setAction(Action.CLIMB_CLIMB);
//		//				climbDuration = 0.0f;
//		//			}
//		if( Input.GetKeyDown(keyUp) ){
//			setAction(Action.CLIMB_CLIMB);
//			climbDuration = 0.0f;
//		}
//		break;
//		
//	case Action.CLIMB_CLIMB:
//		//			climbDuration += Time.deltaTime;
//		//			if( climbDuration >= CLIMBDUR_CLIMB ){
//		//				setAction(Action.IDLE);
//		//				setState(State.ON_GROUND);
//		//				climbDuration = 0.0f;
//		//				transform.position = climbAfterPos;
//		//			}
//		
//		climbDuration += Time.deltaTime;
//		
//		if( climbDuration >= CLIMBDUR_CLIMB ){
//			setAction(Action.IDLE);
//			setState(State.ON_GROUND);
//			climbDuration = 0.0f;
//			transform.position = climbAfterPos;
//			//transform.position = climbJumpPos;
//		} else {
//			float ratio = climbDuration / CLIMBDUR_CLIMB;
//			transform.position = climbJumpPos + climbDistToClimb*ratio;
//		}
//		break;
//		
//	case Action.WALK_LEFT:
//		distToMove = -WALK_SPEED * Time.deltaTime;
//		newPosX += distToMove;
//		if( newPosX < moveTarget.x ){
//			newPosX = moveTarget.x;
//			setAction(Action.IDLE);
//			transform.position = new Vector3(newPosX,oldPos.y,0.0f);
//			//if( Input.GetKey(keyLeft) ) { keyLeftDown(); }
//			if( wantJump ){
//				jumpLeft();	
//			}else if( Input.GetKey(keyLeft) ) { 
//				keyLeftDown(); 
//			}
//		}else{
//			//transform.position.Set(newPosX,oldPos.y,0.0f);
//			transform.position = new Vector3(newPosX,oldPos.y,0.0f);
//		}
//		break;
//		
//	case Action.WALK_RIGHT:
//		distToMove = WALK_SPEED * Time.deltaTime;
//		newPosX += distToMove;
//		if( newPosX > moveTarget.x ){
//			newPosX = moveTarget.x;
//			setAction(Action.IDLE);
//			transform.position = new Vector3(newPosX,oldPos.y,0.0f);
//			//if( Input.GetKey(keyRight) ) { keyRightDown(); }
//			if( wantJump ){
//				jumpRight();	
//			}else if( Input.GetKey(keyRight) ) { 
//				keyRightDown(); 
//			}
//		}else{
//			//transform.position.Set(newPosX,oldPos.y,0.0f);
//			transform.position = new Vector3(newPosX,oldPos.y,0.0f);
//		}
//		break;
//		
//	case Action.RUN_LEFT:
//		distToMove = -RUN_SPEED * Time.deltaTime;
//		newPosX += distToMove;
//		if( newPosX < moveTarget.x ){
//			newPosX = moveTarget.x;
//			setAction(Action.IDLE);
//			transform.position = new Vector3(newPosX,oldPos.y,0.0f);
//			if( wantJumpLong ){
//				jumpLongLeft();
//			}else if( Input.GetKey(keyLeft) ) { 
//				keyLeftDown(); 
//			}
//		}else{
//			//transform.position.Set(newPosX,oldPos.y,0.0f);
//			transform.position = new Vector3(newPosX,oldPos.y,0.0f);
//		}
//		break;
//		
//	case Action.RUN_RIGHT:
//		distToMove = RUN_SPEED * Time.deltaTime;
//		newPosX += distToMove;
//		if( newPosX > moveTarget.x ){
//			newPosX = moveTarget.x;
//			setAction(Action.IDLE);
//			transform.position = new Vector3(newPosX,oldPos.y,0.0f);
//			//if( Input.GetKey(keyRight) ) { keyRightDown(); }
//			if( wantJumpLong ){
//				jumpLongRight();
//			}else if( Input.GetKey(keyRight) ) { 
//				keyRightDown(); 
//			}
//		}else{
//			//transform.position.Set(newPosX,oldPos.y,0.0f);
//			transform.position = new Vector3(newPosX,oldPos.y,0.0f);
//		}
//		break;
//	};
//	
//	if (isNotInState (State.CLIMB)) {
//		bool shouldStop = (checkLeft (0.1f) >= 0.0f) || (checkRight (0.1f) >= 0.0f);
//		if (shouldStop)
//			setAction (Action.IDLE);
//	}
//	
//	float groundUnderFeet;
//	switch (state) {
//	case State.IN_AIR:
//		
//		addImpulse(new Vector2(0.0f, gravityForce * Time.deltaTime));
//		
//		//velocity.x = 0.0f;
//		if( isInAction(Action.FALL) ){
//			if( velocity.x > 0.0f ){
//				velocity.x -= (flySlowDown * Time.deltaTime);
//				if( velocity.x < 0.0f ) velocity.x = 0.0f;
//			}else if(velocity.x < 0.0f) {
//				velocity.x += (flySlowDown * Time.deltaTime);
//				if( velocity.x > 0.0f ) velocity.x = 0.0f;
//			}
//		} else {
//			float flyDist;
//			switch( action ){
//			case Action.JUMP:
//				break;
//				
//			case Action.JUMP_LEFT:
//			case Action.JUMP_RIGHT:
//				flyDist = Mathf.Abs( wantJumpStartX - transform.position.x );
//				if( flyDist >= 1.0f ){
//					setAction(Action.FALL);
//				}
//				break;
//			case Action.JUMP_LEFT_LONG:
//			case Action.JUMP_RIGHT_LONG:
//				flyDist = Mathf.Abs( wantJumpStartX - transform.position.x );
//				if( flyDist >= 2.0f ){
//					setAction(Action.FALL);
//				}
//				break;
//				
//			default:
//				break;
//			};
//		}
//		
//		Vector3 distToFall = new Vector3();
//		distToFall.x = velocity.x * Time.deltaTime;
//		
//		if( distToFall.x > 0.0f )
//		{
//			float obstacleOnRoad = checkRight( distToFall.x + 1.0f);
//			if( obstacleOnRoad >= 0.0f ){
//				if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
//					distToFall.x = obstacleOnRoad;
//					velocity.x = 0.0f;
//					setAction(Action.FALL);
//				}
//			}
//		}
//		else if( distToFall.x < 0.0f )
//		{
//			float obstacleOnRoad = checkLeft( distToFall.x - 1.0f);
//			if( obstacleOnRoad >= 0.0f ){
//				if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
//					distToFall.x = -obstacleOnRoad;
//					velocity.x = 0.0f;
//					setAction(Action.FALL);
//				}
//			}
//		}
//		
//		velocity.y += impulse.y;
//		if(velocity.y > MAX_SPEED_Y)
//			velocity.y = MAX_SPEED_Y;
//		if(velocity.y < -MAX_SPEED_Y)
//			velocity.y = -MAX_SPEED_Y;
//		
//		distToFall.y = velocity.y * Time.deltaTime;
//		
//		if( velocity.y > 0.0f ) { // leci w gore
//			transform.position = transform.position + distToFall;
//		} else if( velocity.y < 0.0f ) { // spada
//			groundUnderFeet = checkDown( distToFall.y + 1.0f);
//			if( groundUnderFeet >= 0.0f ){
//				if( groundUnderFeet < Mathf.Abs(distToFall.y) ){
//					distToFall.y = -groundUnderFeet;
//					velocity.x = 0.0f;
//					velocity.y = 0.0f;
//					setState(State.ON_GROUND);
//					setAction (Action.IDLE);
//				}
//			}
//			transform.position = transform.position + distToFall;
//		}
//		break;
//		
//	case State.ON_GROUND:
//		//			if (Input.GetKeyDown("f")) {
//		//				//transform.position = transform.position + distToFall;
//		//			}
//		//			else if (Input.GetKeyDown("h")) {
//		//				//addImpulse(new Vector2(MAX_SPEED_X_GROUND,0.0f));
//		//			}
//		//			else if (Input.GetKeyDown("t")) {
//		//				transform.position = transform.position + new Vector3(0.0f,0.1f,0.0f);
//		//			}
//		//			else if (Input.GetKeyDown("g")) {
//		//				transform.position = transform.position + new Vector3(0.0f,-0.1f,0.0f);
//		//			}
//		
//		
//		groundUnderFeet = checkDown(0.1f);
//		if( groundUnderFeet < 0.0f ) {
//			setState(State.IN_AIR);
//			setAction(Action.FALL);
//		}
//		break;
//	};
//	
//}

//void climb(Vector2 clmbdir, float distFromWall){
//	if (isNotInState (State.ON_GROUND))
//		return;
//	if( isNotInAction(Action.IDLE))
//		return;
//	
//	climbDuration = 0.0f;
//	climbDistFromWall = distFromWall;
//	climbDir = clmbdir;
//	
//	climbBeforePos = transform.position;
//	climbJumpPos = transform.position;
//	climbAfterPos = transform.position;
//	if( climbDir == Vector2.right ){
//		climbAfterPos.x = Mathf.Floor( transform.position.x ) + 0.5f + 1.0f;
//		climbJumpPos.x = climbAfterPos.x - 0.75f; 
//		//string s3 = string.Format ( "right => {0} {1}", climbBeforePos, climbAfterPos);
//		//print (s3);
//	}else{
//		climbAfterPos.x = Mathf.Ceil( transform.position.x ) - 0.5f - 1.0f;
//		climbJumpPos.x = climbAfterPos.x + 0.75f;
//		//string s3 = string.Format ( "right <= {0} {1}", climbBeforePos, climbAfterPos);
//		//print (s3);
//	}
//	climbAfterPos.y += 2;
//	climbJumpPos.y += 0.5f;
//	//climbDistToJump = climbAfterPos - climbBeforePos;
//	climbDistToJump = climbJumpPos - climbBeforePos;
//	
//	//string s2 = string.Format ("{0} {1} {2}", climbBeforePos, climbAfterPos, climbDistToJump);
//	//print (s2);
//	
//	
//	setState (State.CLIMB);
//	setAction (Action.CLIMB_PREPARE_TO_JUMP);
//}

