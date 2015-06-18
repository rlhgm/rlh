using UnityEngine;
using System.Collections;

public class ncc2 : MonoBehaviour {
	
	//Rigidbody2D rb;
	BoxCollider2D coll;
	public Vector3 velocity;
	public Vector3 impulse;

	public float jumpImpulse = 4.0f; //690.0f;
	public float jumpLongImpulse = 4.5f; //690.0f;
	public float gravityForce = -1.0f;//-1035.0f;
	public float flySlowDown = 1.0f;
	public float MAX_SPEED_Y = 15.0f;
	//public float MAX_SPEED_X_GROUND = 2.0f;
	//public float MAX_SPEED_X_AIR = 2.0f;

	public float WALK_SPEED = 1.0f;
	public float RUN_SPEED = 2.0f;
	public float JUMP_SPEED = 1.0f;
	public float JUMP_LONG_SPEED = 2.0f;
	public float CLIMB_DURATION = 1.5f;

	public KeyCode keyLeft = KeyCode.LeftArrow;
	public KeyCode keyRight = KeyCode.RightArrow;
	public KeyCode keyRun = KeyCode.LeftShift;
	public KeyCode keyJump = KeyCode.Space;
	public KeyCode keyUp = KeyCode.UpArrow;
	public KeyCode keyDown = KeyCode.DownArrow;

	private Animator animator;

	Vector3 moveTarget;
	bool moveTargetSetting;

	private Transform sensorLeft1;
	private Transform sensorLeft2;
	private Transform sensorLeft3;
	private Transform sensorRight1;
	private Transform sensorRight2;
	private Transform sensorRight3;
	private Transform sensorDown1;
	private Transform sensorDown2;
	private Transform sensorClimb1;
	private Transform sensorClimb2;

	private Transform gfx;

	private float myWidth;
	private float myHalfWidth;

	private bool canJump;
	private bool canLongJump;
	private bool wantJump;
	private bool wantJumpLong;
	private float wantJumpStartX;
	private float jumpStartX;

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
		sensorClimb1 = transform.Find("sensorClimb1").transform;
		sensorClimb2 = transform.Find("sensorClimb2").transform;
		
		CLIMBDUR_PREPARE_TO_JUMP = 0.5f;
		CLIMBDUR_JUMP_TO_CATCH = 0.2f; // jednostka w 0.2f
		CLIMBDUR_CATCH = 0.5f;
		CLIMBDUR_CLIMB = 0.75f;
		
		WALK_SPEED = 2.0f;
		RUN_SPEED = 3.5f;
		flySlowDown = 1.5f;
		JUMP_SPEED = 3.5f;
		JUMP_LONG_SPEED = 4.1f;
		
		jumpImpulse = 3.0f;
		jumpLongImpulse = 3.5f;
		gravityForce = -9.0f;
		MAX_SPEED_Y = 15.0f;
		
		CLIMB_DURATION = 1.5f;
		
		myWidth = coll.size.x;
		myHalfWidth = myWidth * 0.5f;
	}

	void Start () {
		velocity = new Vector3 (0, 0, 0);
		impulse = new Vector3 (0, 0, 0);

		setState (State.ON_GROUND);
		setAction (Action.IDLE);

		//animator.SetTrigger("idle");

		moveTarget = transform.position;
		moveTargetSetting = false;

		wantJump = false;
		wantJumpLong = false;
		canJump = false;
		canLongJump = false;
	}

	void takeStep(){

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

	void keyLeftDown(){
		if (isInAction (Action.IDLE) && isInState(State.ON_GROUND) ) {
			if( checkLeft (0.1f) >= 0.0f ) return;
			float tpx = Mathf.Ceil( transform.position.x ) - 0.5f;
			turnLeft();
			if( Input.GetKey(keyRun) ){
				moveTarget.x = tpx - 2;
				velocity.x = -RUN_SPEED;
				setAction(Action.RUN_LEFT);
			}else{
				moveTarget.x = tpx - 1;
				velocity.x = -WALK_SPEED;
				setAction(Action.WALK_LEFT);
			}
		}
	}
	void keyRightDown(){
		if (isInAction (Action.IDLE) && isInState(State.ON_GROUND) ) {
			if( checkRight (0.1f) >= 0.0f ) return;
			float tpx = Mathf.Floor( transform.position.x ) + 0.5f;
			turnRight();
			if( Input.GetKey(keyRun) ){
				moveTarget.x = tpx + 2;
				velocity.x = RUN_SPEED;
				setAction(Action.RUN_RIGHT);
			}else{
				moveTarget.x = tpx + 1;
				velocity.x = WALK_SPEED;
				setAction(Action.WALK_RIGHT);
			}
		}
	}
	void keyLeftUp(){
		wantJump = false;
		wantJumpLong = false;
		canJump = false;
		canLongJump = false;

		moveTargetSetting = true;
	}
	void keyRightUp(){
		wantJump = false;
		wantJumpLong = false;
		canJump = false;
		canLongJump = false;

		moveTargetSetting = true;
	}

	void keyJumpDown(){
		//if (isInAction (Action.IDLE) && isInState(State.ON_GROUND)) {
		//	jump ();
		//}

		string s;
		switch (action) {
		case Action.IDLE:
			if( isInState(State.ON_GROUND)) {
				Vector2 d = dir ();
				float distFromWall = canClimb( dir () );
				if( distFromWall > 0.0f ){
					climb(d,distFromWall);
				}else{
					jump ();
				}
			}
			break;

		case Action.WALK_LEFT:
			wantJump = true;
			//jumpPlaceX
			moveTarget.x = Mathf.Ceil( transform.position.x ) - 0.5f - 1.0f;
			wantJumpStartX = Mathf.Ceil( transform.position.x ) - 0.5f;
			//s = string.Format("wantJumpLeft moveTarget.x: {0} wantJumpStartX: {1}",moveTarget.x,wantJumpStartX);
			//print (s);
			break;
		case Action.WALK_RIGHT:
			wantJump = true;
			//jumpPlaceX
			moveTarget.x = Mathf.Floor( transform.position.x ) + 0.5f + 1.0f;
			wantJumpStartX = Mathf.Floor( transform.position.x ) + 0.5f;
			break;

		case Action.RUN_LEFT:
			wantJumpLong = true;
			//jumpPlaceX
			moveTarget.x = Mathf.Ceil( transform.position.x ) - 0.5f - 1.0f; //2.0f;
			wantJumpStartX = Mathf.Ceil( transform.position.x ) - 0.5f;
			break;
		case Action.RUN_RIGHT:
			wantJumpLong = true;
			//jumpPlaceX
			moveTarget.x = Mathf.Floor( transform.position.x ) + 0.5f + 1.0f; //2.0f;
			wantJumpStartX = Mathf.Floor( transform.position.x ) + 0.5f;
			break;
		};
	}

	void keyJumpUp(){
		wantJump = false;
		wantJumpLong = false;
	}

	private float climbDistFromWall;
	private float climbDuration;
	private Vector2 climbDir;

	private Vector3 climbBeforePos;
	private Vector3 climbAfterPos;
	private Vector3 climbJumpPos;
	private Vector3 climbDistToJump;
	private Vector3 climbDistToClimb;

	public float CLIMBDUR_PREPARE_TO_JUMP = 0.5f;
	//public float CLIMBDUR_JUMP_TO_CATCH_SPEED = 0.2f; // jednostka w 0.2f
	public float CLIMBDUR_JUMP_TO_CATCH = 0.2f; // jednostka w 0.2f
	public float CLIMBDUR_CATCH = 0.5f;
	public float CLIMBDUR_CLIMB = 0.75f;

	void climb(Vector2 clmbdir, float distFromWall){
		if (isNotInState (State.ON_GROUND))
			return;
		if( isNotInAction(Action.IDLE))
			return;

		climbDuration = 0.0f;
		climbDistFromWall = distFromWall;
		climbDir = clmbdir;

		climbBeforePos = transform.position;
		climbJumpPos = transform.position;
		climbAfterPos = transform.position;
		if( climbDir == Vector2.right ){
			climbAfterPos.x = Mathf.Floor( transform.position.x ) + 0.5f + 1.0f;
			climbJumpPos.x = climbAfterPos.x - 0.75f; 
			//string s3 = string.Format ( "right => {0} {1}", climbBeforePos, climbAfterPos);
			//print (s3);
		}else{
			climbAfterPos.x = Mathf.Ceil( transform.position.x ) - 0.5f - 1.0f;
			climbJumpPos.x = climbAfterPos.x + 0.75f;
			//string s3 = string.Format ( "right <= {0} {1}", climbBeforePos, climbAfterPos);
			//print (s3);
		}
		climbAfterPos.y += 2;
		climbJumpPos.y += 0.5f;
		//climbDistToJump = climbAfterPos - climbBeforePos;
		climbDistToJump = climbJumpPos - climbBeforePos;

		//string s2 = string.Format ("{0} {1} {2}", climbBeforePos, climbAfterPos, climbDistToJump);
		//print (s2);
		

		setState (State.CLIMB);
		setAction (Action.CLIMB_PREPARE_TO_JUMP);
	}
	
	void jump(){
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP);
	}

	void jumpLeft(){
		print ("jumpLeft");
		velocity.x = -JUMP_SPEED;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT);
		jumpStartX = transform.position.x;
	}

	void jumpRight(){
		print ("jumpRight");
		velocity.x = JUMP_SPEED;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT);
		jumpStartX = transform.position.x;
	}

	void jumpLongLeft(){
		print ("jumpLongLeft");
		velocity.x = -JUMP_LONG_SPEED;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpLongImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_LEFT_LONG);
		jumpStartX = transform.position.x;
	}

	void jumpLongRight(){
		print ("jumpLongRight");
		velocity.x = JUMP_LONG_SPEED;
		velocity.y = 0.0f;
		addImpulse(new Vector2(0.0f, jumpLongImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP_RIGHT_LONG);
		jumpStartX = transform.position.x;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();

		SetImpulse(new Vector2(0.0f, 0.0f));

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

		Vector3 oldPos = transform.position;
		//float oldPosX = oldPos.x;
		float newPosX = oldPos.x;
		float distToMove;

		switch (action) {
		case Action.IDLE:
			velocity.x = 0.0f;
			break;

		case Action.CLIMB_PREPARE_TO_JUMP:
			climbDuration += Time.deltaTime;
			if( climbDuration >= CLIMBDUR_PREPARE_TO_JUMP ){
				setAction(Action.CLIMB_JUMP_TO_CATCH);
				climbDuration = 0.0f;
			}
			break;
		case Action.CLIMB_JUMP_TO_CATCH:
			climbDuration += Time.deltaTime;

			if( climbDuration >= CLIMBDUR_JUMP_TO_CATCH ){
				setAction(Action.CLIMB_CATCH);
				climbDuration = 0.0f;
				//transform.position = climbAfterPos;
				transform.position = climbJumpPos;

				climbDistToClimb = climbAfterPos - climbJumpPos;
			} else {
				float ratio = climbDuration / CLIMBDUR_JUMP_TO_CATCH;
				transform.position = climbBeforePos + climbDistToJump*ratio;
			}

			break;
		case Action.CLIMB_CATCH:
//			climbDuration += Time.deltaTime;
//			if( climbDuration >= CLIMBDUR_CATCH ){
//				setAction(Action.CLIMB_CLIMB);
//				climbDuration = 0.0f;
//			}
			if( Input.GetKeyDown(keyUp) ){
				setAction(Action.CLIMB_CLIMB);
				climbDuration = 0.0f;
			}
			break;

		case Action.CLIMB_CLIMB:
//			climbDuration += Time.deltaTime;
//			if( climbDuration >= CLIMBDUR_CLIMB ){
//				setAction(Action.IDLE);
//				setState(State.ON_GROUND);
//				climbDuration = 0.0f;
//				transform.position = climbAfterPos;
//			}

			climbDuration += Time.deltaTime;
			
			if( climbDuration >= CLIMBDUR_CLIMB ){
				setAction(Action.IDLE);
				setState(State.ON_GROUND);
				climbDuration = 0.0f;
				transform.position = climbAfterPos;
				//transform.position = climbJumpPos;
			} else {
				float ratio = climbDuration / CLIMBDUR_CLIMB;
				transform.position = climbJumpPos + climbDistToClimb*ratio;
			}
			break;
			
		case Action.WALK_LEFT:
			distToMove = -WALK_SPEED * Time.deltaTime;
			newPosX += distToMove;
			if( newPosX < moveTarget.x ){
				newPosX = moveTarget.x;
				setAction(Action.IDLE);
				transform.position = new Vector3(newPosX,oldPos.y,0.0f);
				//if( Input.GetKey(keyLeft) ) { keyLeftDown(); }
				if( wantJump ){
					jumpLeft();	
				}else if( Input.GetKey(keyLeft) ) { 
					keyLeftDown(); 
				}
			}else{
				//transform.position.Set(newPosX,oldPos.y,0.0f);
				transform.position = new Vector3(newPosX,oldPos.y,0.0f);
			}
			break;

		case Action.WALK_RIGHT:
			distToMove = WALK_SPEED * Time.deltaTime;
			newPosX += distToMove;
			if( newPosX > moveTarget.x ){
				newPosX = moveTarget.x;
				setAction(Action.IDLE);
				transform.position = new Vector3(newPosX,oldPos.y,0.0f);
				//if( Input.GetKey(keyRight) ) { keyRightDown(); }
				if( wantJump ){
					jumpRight();	
				}else if( Input.GetKey(keyRight) ) { 
					keyRightDown(); 
				}
			}else{
				//transform.position.Set(newPosX,oldPos.y,0.0f);
				transform.position = new Vector3(newPosX,oldPos.y,0.0f);
			}
			break;

		case Action.RUN_LEFT:
			distToMove = -RUN_SPEED * Time.deltaTime;
			newPosX += distToMove;
			if( newPosX < moveTarget.x ){
				newPosX = moveTarget.x;
				setAction(Action.IDLE);
				transform.position = new Vector3(newPosX,oldPos.y,0.0f);
				if( wantJumpLong ){
					jumpLongLeft();
				}else if( Input.GetKey(keyLeft) ) { 
					keyLeftDown(); 
				}
			}else{
				//transform.position.Set(newPosX,oldPos.y,0.0f);
				transform.position = new Vector3(newPosX,oldPos.y,0.0f);
			}
			break;
			
		case Action.RUN_RIGHT:
			distToMove = RUN_SPEED * Time.deltaTime;
			newPosX += distToMove;
			if( newPosX > moveTarget.x ){
				newPosX = moveTarget.x;
				setAction(Action.IDLE);
				transform.position = new Vector3(newPosX,oldPos.y,0.0f);
				//if( Input.GetKey(keyRight) ) { keyRightDown(); }
				if( wantJumpLong ){
					jumpLongRight();
				}else if( Input.GetKey(keyRight) ) { 
					keyRightDown(); 
				}
			}else{
				//transform.position.Set(newPosX,oldPos.y,0.0f);
				transform.position = new Vector3(newPosX,oldPos.y,0.0f);
			}
			break;
		};

		if (isNotInState (State.CLIMB)) {
			bool shouldStop = (checkLeft (0.1f) >= 0.0f) || (checkRight (0.1f) >= 0.0f);
			if (shouldStop)
				setAction (Action.IDLE);
		}

		float groundUnderFeet;
		switch (state) {
		case State.IN_AIR:

			addImpulse(new Vector2(0.0f, gravityForce * Time.deltaTime));

			//velocity.x = 0.0f;
			if( isInAction(Action.FALL) ){
				if( velocity.x > 0.0f ){
					velocity.x -= (flySlowDown * Time.deltaTime);
					if( velocity.x < 0.0f ) velocity.x = 0.0f;
				}else if(velocity.x < 0.0f) {
					velocity.x += (flySlowDown * Time.deltaTime);
					if( velocity.x > 0.0f ) velocity.x = 0.0f;
				}
			} else {
				float flyDist;
				switch( action ){
				case Action.JUMP:
					break;

				case Action.JUMP_LEFT:
				case Action.JUMP_RIGHT:
					flyDist = Mathf.Abs( wantJumpStartX - transform.position.x );
					if( flyDist >= 1.0f ){
						setAction(Action.FALL);
					}
					break;
				case Action.JUMP_LEFT_LONG:
				case Action.JUMP_RIGHT_LONG:
					flyDist = Mathf.Abs( wantJumpStartX - transform.position.x );
					if( flyDist >= 2.0f ){
						setAction(Action.FALL);
					}
					break;

				default:
					break;
				};
			}

			Vector3 distToFall = new Vector3();
			distToFall.x = velocity.x * Time.deltaTime;

			if( distToFall.x > 0.0f )
			{
				float obstacleOnRoad = checkRight( distToFall.x + 1.0f);
				if( obstacleOnRoad >= 0.0f ){
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = obstacleOnRoad;
						velocity.x = 0.0f;
						setAction(Action.FALL);
					}
				}
			}
			else if( distToFall.x < 0.0f )
			{
				float obstacleOnRoad = checkLeft( distToFall.x - 1.0f);
				if( obstacleOnRoad >= 0.0f ){
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = -obstacleOnRoad;
						velocity.x = 0.0f;
						setAction(Action.FALL);
					}
				}
			}

			velocity.y += impulse.y;
			if(velocity.y > MAX_SPEED_Y)
				velocity.y = MAX_SPEED_Y;
			if(velocity.y < -MAX_SPEED_Y)
				velocity.y = -MAX_SPEED_Y;
		
			distToFall.y = velocity.y * Time.deltaTime;

			if( velocity.y > 0.0f ) { // leci w gore
				transform.position = transform.position + distToFall;
			} else if( velocity.y < 0.0f ) { // spada
				groundUnderFeet = checkDown( distToFall.y + 1.0f);
				if( groundUnderFeet >= 0.0f ){
					if( groundUnderFeet < Mathf.Abs(distToFall.y) ){
						distToFall.y = -groundUnderFeet;
						velocity.x = 0.0f;
						velocity.y = 0.0f;
						setState(State.ON_GROUND);
						setAction (Action.IDLE);
					}
				}
				transform.position = transform.position + distToFall;
			}
			break;

		case State.ON_GROUND:
//			if (Input.GetKeyDown("f")) {
//				//transform.position = transform.position + distToFall;
//			}
//			else if (Input.GetKeyDown("h")) {
//				//addImpulse(new Vector2(MAX_SPEED_X_GROUND,0.0f));
//			}
//			else if (Input.GetKeyDown("t")) {
//				transform.position = transform.position + new Vector3(0.0f,0.1f,0.0f);
//			}
//			else if (Input.GetKeyDown("g")) {
//				transform.position = transform.position + new Vector3(0.0f,-0.1f,0.0f);
//			}


			groundUnderFeet = checkDown(0.1f);
			if( groundUnderFeet < 0.0f ) {
				setState(State.IN_AIR);
				setAction(Action.FALL);
			}
			break;
		};

	}

	float checkLeft(float checkingDist){
		Vector2 rayOrigin = new Vector2( sensorLeft1.position.x, sensorLeft1.position.y );
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist);
		if (hit.collider != null) {
			return Mathf.Abs (hit.point.y - sensorLeft1.position.x);
		} else {
			rayOrigin = new Vector2( sensorLeft2.position.x, sensorLeft2.position.y );
			hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist);
			if (hit.collider != null){
				return Mathf.Abs (hit.point.x - sensorLeft2.position.x);
			} else {
				rayOrigin = new Vector2( sensorLeft3.position.x, sensorLeft3.position.y );
				hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist);
				if (hit.collider != null){
					return Mathf.Abs (hit.point.x - sensorLeft3.position.x);
				} else {
					return -1.0f;
				}
			}
		}
	}

	float checkRight(float checkingDist){
		Vector2 rayOrigin = new Vector2( sensorRight1.position.x, sensorRight1.position.y );
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist);
		if (hit.collider != null) {
			return Mathf.Abs (hit.point.y - sensorRight1.position.x);
		} else {
			rayOrigin = new Vector2( sensorRight2.position.x, sensorRight2.position.y );
			hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist);
			if (hit.collider != null){
				return Mathf.Abs (hit.point.x - sensorRight2.position.x);
			} else {
				rayOrigin = new Vector2( sensorRight3.position.x, sensorRight3.position.y );
				hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist);
				if (hit.collider != null){
					return Mathf.Abs (hit.point.x - sensorRight3.position.x);
				} else {
					return -1.0f;
				}
			}
		} 
	}

	float checkDown(float checkingDist){
		Vector2 rayOrigin = new Vector2( sensorDown1.position.x, sensorDown1.position.y );
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist);
		if (hit.collider != null) {
			return Mathf.Abs (hit.point.y - sensorDown1.position.y);
		} else {
			rayOrigin = new Vector2( sensorDown2.position.x, sensorDown2.position.y );
			hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist);
			if (hit.collider != null){
				return Mathf.Abs (hit.point.y - sensorDown2.position.y);
			} else {
				return -1.0f;
			}
		}  
	}

	float canClimb(Vector2 dir){
		Vector2 rayOrigin = new Vector2( sensorClimb1.position.x, sensorClimb1.position.y );
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, dir, 1.0f);
		if (hit.collider == null) {
			return -1.0f;
		} else {
			//float dist = Mathf.Abs (hit.point.x - sensorDown1.position.x);
			float distToWall = Mathf.Abs (hit.point.x - transform.position.x);

			rayOrigin = new Vector2( sensorClimb2.position.x, sensorClimb2.position.y );
			hit = Physics2D.Raycast (rayOrigin, dir, distToWall + 0.5f + 0.25f);
			if (hit.collider != null){
				return -1.0f;
			} else {
				return distToWall;
			}
		}  
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
		CLIMB_CLIMB
	};
	
	public enum State
	{
		ON_GROUND = 0,
		IN_AIR,
		CLIMB,
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

		//print ("setAction oldAction : " + action);
		//print ("setAction newAction : " + newAction);

		if (action == newAction)
			return false;

		//print ("setAction : " + newAction + " ustawiona");
		//print ("============================");

		action = newAction;

		//animator = transform.Find("gfx").GetComponent<Animator>();

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

		case Action.JUMP:
		case Action.JUMP_LEFT:
		case Action.JUMP_LEFT_LONG:
		case Action.JUMP_RIGHT:
		case Action.JUMP_RIGHT_LONG:
			animator.SetTrigger("jump");
			break;

		case Action.CLIMB_PREPARE_TO_JUMP:
			animator.SetTrigger("climb_preparetojump");
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

	void OnCollisionEnter2D(Collision2D coll) {
//		print( "colisionEnter" );
//		
//		for (int i = 0; i < coll.contacts.Length; ++i) {
//			Vector2 n = coll.contacts[i].normal;
//			float atn = Mathf.Atan2( n.y, n.x );
//			float atnd = atn * Mathf.Rad2Deg;
//			if( atnd > 45 && atnd < 135 ) 
//			{
//				setState (State.ON_GROUND);
//				velocity.y = 0.0f;
//				print("LANDing");
//				return;
//			}
//		}
	}
	
	void OnCollisionExit2D(Collision2D coll) {
//		print( "colisionExit" );
//		
//		for (int i = 0; i < coll.contacts.Length; ++i) {
//			Vector2 n = coll.contacts[i].normal;
//			float atn = Mathf.Atan2( n.y, n.x );
//			float atnd = atn * Mathf.Rad2Deg;
//			if( atnd > 45 && atnd < 135 ) 
//			{
//				setState (State.IN_AIR);
//				//velocity.y = 0.0f;
//				print("FLYing");
//				return;
//			}
//		}
	}
	
	void OnCollisionStay2D(Collision2D coll) {
		//print( "collisionStay" );
	}
	
	
	//void FixedUpdate(){
	//Vector3 oldPos = transform.position;
	//transform.position = transform.position + velocity * Time.deltaTime;
	//}

	bool StayOnGround(){
		//public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity); 
		Vector2 rayOrigin = new Vector2( transform.position.x, transform.position.y - coll.size.y * 0.5f);
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, 2.0f);
		if (hit.collider != null) {
			float distance = Mathf.Abs(hit.point.y - rayOrigin.y);
			//print( hit.collider.gameObject.name );
			//print ( distance );
			if( distance < 0.001f )
			{
				//hit.normal
				return true;
			}
		}
		return false;
	}

}

/*
 * SetImpulse(new Vector2(0.0f, 0.0f));
		
		//StayOnGround();
		
		if (Input.GetKey("a")) {
			addImpulse(new Vector2(-MAX_SPEED_X_GROUND,0.0f));
			//animator.SetTrigger("");
		}
		else if (Input.GetKey("d")) {
			addImpulse(new Vector2(MAX_SPEED_X_GROUND,0.0f));
			
		}
		
		if (Input.GetKey("f")) {
			addImpulse(new Vector2(-MAX_SPEED_X_GROUND,0.0f));
		}
		else if (Input.GetKey("h")) {
			addImpulse(new Vector2(MAX_SPEED_X_GROUND,0.0f));
		}
		else if (Input.GetKey("t")) {
			addImpulse(new Vector2(0.0f,MAX_SPEED_X_GROUND));
		}
		else if (Input.GetKey("g")) {
			addImpulse(new Vector2(0.0f,-MAX_SPEED_X_GROUND));
		}
		
		//if( impulse.x == 0.0f )
		
		if ((Input.GetKeyDown("w") || Input.GetKeyDown("space")) && isNotInState(State.IN_AIR) ) {
			velocity.y = 0.0f;
			addImpulse(new Vector2(0.0f, jumpImpulse));
			setState(State.IN_AIR);
		}
		
		if( state == State.IN_AIR )
		{
			addImpulse(new Vector2(0.0f, gravityForce * Time.deltaTime));
		}
		
		
		// Ustalanie prędkości gracza na podstawie impulse
		velocity.x = 0.0f;
		//velocity.y = 0.0f;
		velocity.x += impulse.x;
		velocity.y += impulse.y;
		
		float maxSpeed = state == State.ON_GROUND ? MAX_SPEED_X_GROUND : MAX_SPEED_X_AIR;
		if(velocity.x > maxSpeed)
			velocity.x = maxSpeed;
		if(velocity.x < -maxSpeed)
			velocity.x = -maxSpeed;
		
		//if(state == STATE_ON_GROUND && !onGroundUp && velocity.y < minVelocityOnGround)
		//	velocity.y = minVelocityOnGround;
		//else
		if( state == State.IN_AIR )
		{
			velocity.y += impulse.y;
			if(velocity.y > MAX_SPEED_Y)
				velocity.y = MAX_SPEED_Y;
			if(velocity.y < -MAX_SPEED_Y)
				velocity.y = -MAX_SPEED_Y;
		}
		
		//velocity += impulse;
		
		//print (velocity);
		//print (impulse);
		*/