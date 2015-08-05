using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

	Canvas guiCanvas = null;
	Text infoLabel = null;
	Image mapBackgroundImage = null;
	ComicPagePart[] mapPartParts = new ComicPagePart[3];

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
	public float MaxFallDistToCatch = 3.0f;

	public float RopeSwingForce = 500f;
	public float RopeClimbSpeedUp = 1.0f;
	public float RopeClimbSpeedDown = 3.0f;

	public float CLIMB_DURATION = 1.5f;
	public float CLIMBDUR_PREPARE_TO_JUMP = 0.5f;
	public float CLIMBDUR_JUMP_TO_CATCH = 0.2f; // jednostka w 0.2f
	public float CLIMBDUR_CATCH = 0.5f;
	/*public*/ float CLIMBDUR_CLIMB = 0.65f;
	public float LANDING_HARD_DURATION = 0.5f;

	public float TURN_LEFTRIGHT_DURATION = 0.2f;
	
	public KeyCode keyLeft = KeyCode.LeftArrow;
	public KeyCode keyRight = KeyCode.RightArrow;
	public KeyCode keyRun = KeyCode.LeftShift;
	public KeyCode keyUp = KeyCode.UpArrow;
	public KeyCode keyDown = KeyCode.DownArrow;
	public KeyCode keyJump = KeyCode.Space;

	public Transform respawnPoint;


	public AudioClip[] jumpSounds;
	public AudioClip[] landingSounds;
	public AudioClip[] turnRunSounds;
	public AudioClip[] catchSounds;
	public AudioClip[] dieSounds;

	public AudioClip landingSound;
	public AudioClip ropeCatchSound;

	AudioSource audio;

	void Awake(){
		guiCanvas = FindObjectOfType<Canvas> ();
		//print (guiCanvas);
		//guiText = guiCanvas.transform.Find ("Text").ga;
		if (guiCanvas) {
			infoLabel = FindObjectOfType<Text> ();
			//infoLabel.text = "hello world";
			infoLabel.text = "";

			//guiCanvas = FindObjectOfType<
			Image[] allImages = FindObjectsOfType<Image>();
			for( int i = 0 ; i < allImages.Length ; ++i ){
				Image img = allImages[i];
				if( img.gameObject.name == "mapBackgroundImage" ){
					mapBackgroundImage = img;
					Color newColor = new Color(1f,1f,1f,0f);
					mapBackgroundImage.color = newColor;
					//print ( "jest mapBackgroundImage");
					continue;
				}

				ComicPagePart cpp = img.GetComponent<ComicPagePart>();
				if( cpp ){
					mapPartParts[cpp.partID] = cpp;
					//print ( "jest mapPart " + cpp.partID);
					//Color newColor = new Color(1f,1f,1f,0f);
					//img.color = newColor;
				}
			}
		}

		coll = GetComponent<BoxCollider2D> ();
		gfx  = transform.Find("gfx").transform;
		animator = transform.Find("gfx").GetComponent<Animator>();
		sprRend = gfx.GetComponent<SpriteRenderer> ();

		zap_idle1_beh[] behs = animator.GetBehaviours<zap_idle1_beh>();
		for( int b = 0 ; b < behs.Length ; ++b )
		{
			behs[b].playerController = this;
		}

		audio = GetComponent<AudioSource>();

		PlaySounds[] pss = animator.GetBehaviours<PlaySounds>();
		for( int b = 0 ; b < pss.Length ; ++b )
		{
			pss[b].playerController = this;
		}

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

		cameraTarget = transform.Find("cameraTarget").transform;

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

	public AudioSource getAudioSource(){
		return audio;
	}
	public Transform getCameraTarget(){
		return cameraTarget;
	}

	void Start () {

		//audio = GetComponent<AudioSource>();

		velocity = new Vector3 (0, 0, 0);
		impulse = new Vector3 (0, 0, 0);
		desiredSpeedX = 0.0f;
		startFallPos = transform.position;

		setState (State.ON_GROUND);
		setAction (Action.IDLE);
		//action = Action.IDLE;

		climbDuration = 0.0f;
		catchedClimbHandle = null;
		canPullUp = false;

		jumpFromMount = false;

		lastTouchedCheckPoint = null;

		//showInfo ("hello world", 10);
	}

	//bool isDead = false;

	public void StateIdleExit(){
		//print("StateIdleExit");
	}
	public void StateIdleUpdate(float normTime){
		//print("StateIdleUpdate " + normTime);

		//if (normTime < 1.0f)
		//	return;

		//print("StateIdleUpdate " + normTime);
		//print ("currentActionTime : " + currentActionTime);

//		AnimatorClipInfo[] acis = animator.GetCurrentAnimatorClipInfo(0);
//		if (acis.Length > 0) {
//			AnimatorClipInfo aci = acis[0];
//
//			print( "aci.clip.length : " + aci.clip.length );
//		}
		//print (animator.playbackTime);

		//AnimatorStateInfo asi = animator.GetCurrentAnimatorStateInfo (0);
		//print (asi.length + " " + asi.normalizedTime);

		//animator.
//		int r = Random.Range(0,20);
//		print (r);
//
//		if (r < 18) {
//			animator.Play("zapidle");
//		} else if (r < 19) {
//			animator.Play ("zapidle_var1");
//		} else {
//			animator.Play ("zapidle_var2");
//		}

//		switch( r ){
//		case 0:
//			animator.Play("zapidle");
//			break;
//		case 1:
//			animator.Play ("zapidle_var1");
//			break;
//		case 2:
//			animator.Play ("zapidle_var2");
//			break;
//		}
	}

	public int IdleAnimFreq = 10;

	public void StateIdleFinish(int stateIdleNum){
		//print ("StateIdleFinish");
		switch( stateIdleNum ){
		case 1:
		case 2:
			animator.Play("zapidle");
			break;
		case 0:
			if( IdleAnimFreq >= 3 ){
				//animator.Play ("zapidle");
				int r = Random.Range(0,IdleAnimFreq);
				if( r == 0 ){
					animator.Play ("zapidle_var1");
				}else if( r == 1 ){
					animator.Play ("zapidle_var2");
				}
			}
			//		print (r);
			//
			//		if (r < 18) {
			//			animator.Play("zapidle");
			//		} else if (r < 19) {
			//			animator.Play ("zapidle_var1");
			//		} else {
			//			animator.Play ("zapidle_var2");
			//		}
			break;
//		case 1:
//		case 2:
//			animator.Play ("zapidle_var2");
//			break;
		}

//		switch( stateIdleNum ){
//		case 2:
//			animator.Play("zapidle");
//			break;
//		case 0:
//			animator.Play ("zapidle_var1");
//			break;
//		case 1:
//			animator.Play ("zapidle_var2");
//			break;
//		}
	}

	public enum DeathType{
		VERY_HARD_LANDING = 1,
		SNAKE,
		CROCODILE,
		POISON
	};

	public string DeathByVeryHardLandingText = "rozjeb... sie o skale. press space";
	public string DeathBySnakeText = "pokasal cie waz. press space";
	public string DeathByCrocodileText = "zjadl cie krokodyl. press space";
	public string DeathByPoisonText = "zatrules sie. press space";
	public string DeathByDefaultText = "zginales defaultowa smiercia. press space";

	//public void die(int deathType){
	public void die(DeathType deathType){
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		setAction (Action.DIE, (int)deathType);
		setState (State.OTHER);

		//collectedMapParts
		foreach(GameObject mapPart in collectedMapParts){

			ComicPage comicPart = mapPart.GetComponent<ComicPage>();
			int mapPartID = comicPart.partID;
			
			mapPartParts [mapPartID].collected = false;
			
			mapPart.GetComponent<SpriteRenderer> ().enabled = true;
		}

		collectedMapParts.Clear ();

		//showInfo ("PRESS SPACE", -1);
	}

	public bool isDead(){
		return action == Action.DIE && state == State.OTHER;
	}

	public void reborn(){
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		setAction (Action.IDLE);
		setState (State.ON_GROUND);

		if (lastTouchedCheckPoint) {
			transform.position = lastTouchedCheckPoint.transform.position;
		} else {
			transform.position = respawnPoint.position;
		}

		resetInfo ();

		NewRope[] ropes = FindObjectsOfType(typeof(NewRope)) as NewRope[];
		foreach (NewRope rope in ropes) {
			rope.reset();
		}
	}
	GameObject lastTouchedCheckPoint;

	public bool canBeFuddleFromBird = true;
	bool fuddledFromBrid = false;

	void OnTriggerEnter2D(Collider2D other) {
		//print( "PLAYER OnTriggerEnter" + other.gameObject.tag);
		if (other.gameObject.tag == "Bird") {
			if( isInState(State.MOUNT) ){
				velocity.x = 0.0f;
				velocity.y = 0.0f;
				setAction(Action.JUMP);
				setState(State.IN_AIR);

				if( canBeFuddleFromBird )
					fuddledFromBrid = true;

			} else if( isInState(State.IN_AIR) ) {
				velocity.x = 0.0f;
			}
			return;
		}
		if (other.gameObject.tag == "CheckPoint") {
			//print( "checkpoint : " + other.gameObject.name );

			//if( 
			lastTouchedCheckPoint = other.gameObject;

			// zatwierdzam wszystkie zdobyte kawalki mapy...
			collectedMapParts.Clear ();
			return;
		}
		if (other.gameObject.tag == "KillerPhysic") {
			die(DeathType.POISON);
			return;
		}
		if (other.gameObject.tag == "Crocodile") {
			die (DeathType.CROCODILE);
			return;
		}
		if (other.gameObject.tag == "ShowInfoTrigger") {
			print("ShowInfoTrigger");
			ShowInfoTrigger sit = other.gameObject.GetComponent<ShowInfoTrigger>();
			if( !sit.used ){

				showInfo(sit.Info,sit.ShowDuration);
				if(sit.OnlyFirstTime) sit.used = true;
			}
			return;
		}
		if (other.gameObject.tag == "ComicPage") {
			collectMapPart(other.gameObject);
			return;
		}
	}

	//void OnTrigger

	float puzzleMapShowTime = 0.0f;
	bool puzzleMapShowing = false;
	int puzzleShowingPhase = 0;
	int newPuzzleCollectedID = 0;

	List<GameObject> collectedMapParts = new List<GameObject>();

	void collectMapPart(GameObject mapPart){

		ComicPage comicPart = mapPart.GetComponent<ComicPage>();
		int mapPartID = comicPart.partID;

		if (mapPartParts [mapPartID].collected)
			return;

		mapPartParts [mapPartID].collect ();

		//Destroy (mapPart);
		mapPart.GetComponent<SpriteRenderer> ().enabled = false;
		collectedMapParts.Add (mapPart);

		newPuzzleCollectedID = mapPartID;
		showPuzzleMap ();
	}

	void showPuzzleMap(){
		puzzleMapShowTime = 0.0f;
		puzzleMapShowing = true;
		puzzleShowingPhase = 1;

		for (int i = 0; i < mapPartParts.Length; ++i) {
			if (i == newPuzzleCollectedID)
				continue;
			if (mapPartParts[i].collected) {
				mapPartParts[i].show(1.0f);
			}
		}
	}

	void hidePuzzleMap(){

	}

	bool userJumpKeyPressed = false;
	float timeFromJumpKeyPressed = 0.0f;
	bool jumpKeyPressed = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	
		if (Input.GetKeyDown (KeyCode.P)) {
			gamePaused = !gamePaused;
		}

		if (puzzleMapShowing) {
			puzzleMapShowTime += Time.deltaTime;

			if( puzzleMapShowTime < 1.0f ){

				float phaseRatio = puzzleMapShowTime / 1.0f;

				Color newColor = new Color(1f,1f,1f,0.75f*phaseRatio);
				mapBackgroundImage.color = newColor;



			} else if( puzzleMapShowTime < 2.0f ){

				if( puzzleShowingPhase == 1){
					puzzleShowingPhase = 2;

					Color newColor = new Color(1f,1f,1f,1f);
					mapBackgroundImage.color = newColor;

					mapPartParts[newPuzzleCollectedID].show(0.5f);

				}

			} else {

				if( puzzleShowingPhase == 2 ){
					puzzleShowingPhase = 3;

					for (int i = 0; i < mapPartParts.Length; ++i) {
						if (mapPartParts[i].collected) {
							mapPartParts[i].hide(1.0f);
						}
					}
				}

				if( puzzleMapShowTime >= 3.0f ){

					puzzleMapShowing = false;
					puzzleShowingPhase = 0;
					Color newColor = new Color(1f,1f,1f,0f);
					mapBackgroundImage.color = newColor;

				}else{

					float phaseRatio = puzzleMapShowTime-2.0f / 1.0f;
					Color newColor = new Color(1f,1f,1f,0.75f-0.75f*phaseRatio);
					mapBackgroundImage.color = newColor;

				}

			}
		}

		if( gamePaused ){

			if (Input.GetKey("f")) {
				transform.position = transform.position + new Vector3(-0.1f,0.0f,0.0f);
				//showInfo("You press f",1f);
			}
			else if (Input.GetKey("h")) {
				transform.position = transform.position + new Vector3(0.1f,0.0f,0.0f);
				//showInfo("You press h",2f);
			}
			else if (Input.GetKey("t")) {
				transform.position = transform.position + new Vector3(0.0f,0.1f,0.0f);
				///showInfo("You press t",3f);
			}
			else if (Input.GetKey("g")) {
				transform.position = transform.position + new Vector3(0.0f,-0.1f,0.0f);
				//showInfo("You press g",4f);
			}

			return;
		}

		if (infoLabelSet) {
			if( infoLabelShowDuration > 0 ){
				if( (infoLabelShowTime+=Time.deltaTime) > infoLabelShowDuration ){
					infoLabelSet = false;
					infoLabel.text = "";
				}
			}
		}

		if (isInAction (Action.DIE)) {
			if( Input.GetKeyDown(KeyCode.Space) ){
				reborn();
				return;
			}
		}

		SetImpulse(new Vector2(0.0f, 0.0f));

		justJumpedMount = false;
		firstFrameInState = false;

		if (!userJumpKeyPressed) {
			if (Input.GetKeyDown (keyJump)) {
				timeFromJumpKeyPressed = 0.0f;
				userJumpKeyPressed = true;
			}
		} else {
			timeFromJumpKeyPressed += Time.deltaTime;
			if (timeFromJumpKeyPressed >= 0.06f) {
				timeFromJumpKeyPressed = 0.0f;
				userJumpKeyPressed = false;

				keyJumpDown ();
			}
		}

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
			if (Input.GetKeyDown (keyJump)) {
				wantJumpAfter = true;
			}
			if( currentActionTime >= TURN_LEFTRIGHT_DURATION ){
				turnLeft();
				turnLeftFinish();
			}
			break;

		case Action.TURN_STAND_RIGHT:
			if (Input.GetKeyDown (keyJump)) {
				wantJumpAfter = true;
			}
			if( currentActionTime >= TURN_LEFTRIGHT_DURATION ){
				turnRight();
				turnRightFinish();
			}
			break;

		case Action.TURN_RUN_LEFT:
			if( currentActionTime >= 0.85f ){
				turnLeft();
				if( wantJumpAfter ){
					jumpLeft();
				}else{
					setActionIdle();
					resetActionAndState();
				}
			}else{
				int res = Act_TURN_RUN(1);
				if( res == 1 ){
					//turnLeft();
					//setActionIdle();
					//resetActionAndState();
				}
			}
			break;

		case Action.TURN_RUN_RIGHT:
			if( currentActionTime >= 0.85f ){
				turnRight();
				if( wantJumpAfter ){
					jumpRight();
				}else{
					setActionIdle();
					resetActionAndState();
				}
			}else{
				int res = Act_TURN_RUN(-1);
				if( res == 1 ){
					//turnRight();
					//setActionIdle();
					//resetActionAndState();
				}
			}
			break;

		case Action.CROUCH_IN:
			Act_CROUCH_IN();
			break;

		case Action.GET_UP:
			Act_GET_UP();
			break;

		case Action.CROUCH_IDLE:
			Act_CROUCH_IDLE();
			break;

		case Action.CROUCH_LEFT:
		case Action.CROUCH_LEFT_BACK:
			Act_CROUCH_LEFTRIGHT(-1);
			break;

		case Action.CROUCH_RIGHT:
		case Action.CROUCH_RIGHT_BACK:
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
				//getUp();
				setAction(Action.GET_UP);
				wantGetUp = false;
			}
		}

		switch (state) {

		case State.MOUNT:
//			if( !onMount() ){
//				setAction(Action.JUMP);
//				setState(State.IN_AIR);
//			}
			break;

		case State.IN_AIR:

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

			//justLetGoHandle += Time.deltaTime;

			if( Input.GetKey(keyJump) ) { //&& justLetGoHandle>toNextHandleDuration){

				if( !fuddledFromBrid && tryCatchRope() ){
					
					if( ropeCatchSound )
						audio.PlayOneShot( ropeCatchSound );
					
					return;
				}

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
				float obstacleOnRoad = checkRight(distToFall.x + 0.01f,!firstFrameInState);
				if( obstacleOnRoad >= 0.0f ){
					//print("distToFall.x = " + distToFall.x + " obstacleOnRoad = " + obstacleOnRoad );
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = obstacleOnRoad;
						velocity.x = 0.0f;
						//if( velocity.y > 0.0f )
						//	velocity.y *=  0.5f;
						//setAction(Action.FALL);
						//setAction(Action.JUMP);
					}
				}
			}
			else if( distToFall.x < 0.0f )
			{
				float obstacleOnRoad = checkLeft( Mathf.Abs(distToFall.x) + 0.01f,!firstFrameInState);
				if( obstacleOnRoad >= 0.0f ){
					//print( "jest obstacle" );
					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
						distToFall.x = -obstacleOnRoad;
						//print (distToFall);
						velocity.x = 0.0f;
						//if( velocity.y > 0.0f )
						//	velocity.y *=  0.5f;
						//setAction(Action.FALL);
						//setAction(Action.JUMP);
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
			
			distToFall.y = velocity.y * Time.deltaTime;

			bool justLanding = false;

			if( distToFall.y > 0.0f ) { // leci w gore
				//transform.position = transform.position + distToFall;
			} else if( distToFall.y < 0.0f ) { // spada
				if( lastVelocity.y >= 0.0f ) { // zaczyna spadac
					// badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
					startFallPos = transform.position;
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
					audio.PlayOneShot( landingSound );

				fuddledFromBrid = false;

				setState(State.ON_GROUND);
				velocity.y = 0.0f;

				Vector3 fallDist = startFallPos - transform.position;

				if( fallDist.y >= VeryHardLandingHeight ){
					die(DeathType.VERY_HARD_LANDING);
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

	float infoLabelShowDuration = 0f;
	float infoLabelShowTime = 0f;
	bool infoLabelSet = false;

	public void showInfo(string newInfo, float duration){
		if (infoLabel) {
			infoLabel.text = newInfo;
			infoLabelShowTime = 0f;
			infoLabelShowDuration = duration;
			infoLabelSet = true;
		}
	}
	public void resetInfo(){
		if (infoLabel) {
			infoLabel.text = "";
			infoLabelShowTime = 0f;
			infoLabelShowDuration = 1f;
			infoLabelSet = true;
		}
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
			setAction(Action.CLIMB_CATCH,1);
			setState(State.CLIMB);
			climbDuration = 0.0f;
			canPullUp = true;
			transform.position = climbAfterPos;
		} else {
			//float ratio = climbDuration / CLIMBDUR_CLIMB;
			//if( ratio > 0.5f )
			//	transform.position = climbBeforePos + climbDistToClimb*((ratio-0.5f) * 2.0f);
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
		//} else if (Input.GetKeyDown (keyDown)) {
		//	velocity.x = 0.0f;
		//	velocity.y = 0.0f;
		//	setState (State.IN_AIR);
		//	setAction (Action.JUMP);
		//	catchedClimbHandle = null;
		//	lastCatchedClimbHandle = null;
			//justLetGoHandle = 0.0f;
		} else if ( Input.GetKeyDown (keyJump)) {
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
			} else if( Input.GetKey(keyDown) ){
				velocity.x = 0.0f;
				velocity.y = 0.0f;
				setState (State.IN_AIR);
				setAction (Action.JUMP);

				//lastCatchedClimbHandle = null;
				lastCatchedClimbHandle = catchedClimbHandle;
				catchedClimbHandle = null;
			} else {
				//print("try jump up");
				jumpFromClimb ();
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
			//action = Action.IDLE;
			resetActionAndState();
			return 0;
		}

		distToMove = velocity.x * Time.deltaTime;

		animator.speed = 0.5f + (Mathf.Abs( velocity.x ) / WalkSpeed ) * 0.5f;
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

	float fromStartRunBack = 0.0f;

	int Act_RUN(int dir){

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
//				if (Input.GetKeyUp (keyRight) && Input.GetKey (keyLeft)) {
//					setAction (Action.TURN_RUN_LEFT);
//				}

				if( (Input.GetKeyDown(keyLeft) || Input.GetKey(keyLeft)) &&
				   (Input.GetKeyUp(keyRight) || !Input.GetKey(keyRight))
				   )
				{
					setAction (Action.TURN_RUN_LEFT);
				}

			} else if (dir == -1) {
//				if (Input.GetKeyUp (keyLeft) && Input.GetKey (keyRight)) {
//					setAction (Action.TURN_RUN_RIGHT);
//				}

				if( (Input.GetKeyDown(keyRight) || Input.GetKey(keyRight)) &&
				   (Input.GetKeyUp(keyLeft) || !Input.GetKey(keyLeft))
				   )
				{
					setAction (Action.TURN_RUN_RIGHT);
				}

			}

		}

		distToMove = velocity.x * Time.deltaTime;

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

	int Act_TURN_RUN(int dir){

		int retVal = 0;

		if (Input.GetKeyDown (keyJump)) {
			wantJumpAfter = true;
		}

		bool speedReached = checkSpeed (dir);
		if (speedReached && desiredSpeedX == 0.0f) {
			//setAction(Action.IDLE);
			//resetActionAndState ();
		}

		distToMove = velocity.x * Time.deltaTime;
		
		//animator.speed = 0.5f + (Mathf.Abs( velocity.x ) / RunSpeed ) * 0.5f;
		
		float distToObstacle = 0.0f;
		if (checkObstacle (dir, distToMove, ref distToObstacle)) {
			distToMove = distToObstacle;
			//setActionIdle();
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

	public float CrouchInOutDuration = 0.2f;

	int Act_CROUCH_IN(){

		if (currentActionTime >= CrouchInOutDuration) {
			crouch();
		}
		return 0;
	}

	int Act_GET_UP(){

		if (currentActionTime >= CrouchInOutDuration) {
			getUp();			
		}

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

	NewRope justJumpedRope = null;
	bool justJumpedMount = false;

	int Act_ROPECLIMB_IDLE(){

		if (!catchedRope)
			return 0;

		if (Input.GetKey (keyLeft)) {
			//turnLeft();

			float fla = catchedRope.firstLinkAngle;
			if( fla < 10f && fla > -15f){
				catchedRope.swing(-Vector2.right, RopeSwingForce * Time.deltaTime );
			}

			if( dir () == Vector2.right ){
				animator.Play("Zap_liana_swingback");
			}else{
				animator.Play("Zap_liana_swingfront");
			}

//			} else {
//				animator.Play("newclimbrope_idle");
//			}

		}
		else if (Input.GetKey (keyRight)) {
			//turnRight();

			float fla = catchedRope.firstLinkAngle;

			if( fla > -10f && fla < 15f){
				catchedRope.swing(Vector2.right, RopeSwingForce * Time.deltaTime );
			}
				
			if( dir () == Vector2.right ){
				animator.Play("Zap_liana_swingfront");
			}else{
				animator.Play("Zap_liana_swingback");
			}

//			} else {
//				animator.Play("newclimbrope_idle");
//			}
		}

		if (Input.GetKeyUp (keyLeft) || Input.GetKeyUp(keyRight) ) {
			animator.Play("newclimbrope_idle");
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

		return 0;
	}

	bool canRopeClimbUp(){
		if (ropeLinkCatchOffset == 0f) {
			return catchedRopeLink.transform.parent;
		}
		return true; 
	}
	bool canRopeClimbDown(){
		if (ropeLinkCatchOffset == -0.5f) {

			if( catchedRopeLink.transform.childCount > 0 ) { // jak ogniwo ma dzicko to przechodze niżej 

				if( catchedRopeLink.transform.GetChild(0).transform.childCount > 0 ){ // chyba ze to jest ostatnie ogniwo
					return true;
				}

			}

			return false;
		}
		return true;
	}

	int Act_ROPECLIMB_UP(){

		if (!catchedRope)
			return 0;

		if (Input.GetKeyUp (keyUp)) { 
			setAction(Action.ROPECLIMB_IDLE);
			return 0;
		} 

		float climbDist = RopeClimbSpeedUp * Time.deltaTime;

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


		return 0;
	}

	int Act_ROPECLIMB_DOWN(){

		if (!catchedRope)
			return 0;

		if (Input.GetKeyUp (keyDown)) {
			setAction(Action.ROPECLIMB_IDLE);
			return 0;
		}

		if (tryJumpFromRope () != 0) {
			return 0;
		}

		float climbDist = RopeClimbSpeedDown * Time.deltaTime;
		
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


		return 0;
	}

	int tryJumpFromRope(){

		if (Input.GetKeyDown (keyJump)) {
			
			float ropeSpeed = catchedRope.firstLinkSpeed;
			float ropeSpeedRad = ropeSpeed * Mathf.Deg2Rad;
			int crl_idn = catchedRope.currentLink.GetComponent<RopeLink> ().idn;
			float ps = ropeSpeedRad * (crl_idn + 1) * 0.5f;
			float ropeAngle = Mathf.Abs (catchedRope.firstLinkAngle);
			
			if (Input.GetKey (keyLeft)) { //skacze w lewo
				turnLeft ();
				
				if (ropeSpeed > 0f) { // lina tez leci w lewo
					jumpLongLeft ();
					velocity.x -= ps;
				} else {
					jumpLeft ();
				}
			} else if (Input.GetKey (keyRight)) { //skacze w prawo
				turnRight ();
				
				if (ropeSpeed < 0f) { // lina tez leci w prawo
					jumpLongRight ();
					velocity.y += ps;
				} else {
					jumpRight ();
				}
			} else if( Input.GetKeyDown (keyDown) || Input.GetKey (keyDown) ) {
				//jump();
				//velocity.x = -ps;
				//velocity.y = (ropeAngle / 30.0f) * JumpLongImpulse;
				velocity.x = 0f;
				velocity.y = 0f;
				setAction (Action.JUMP);
			}else{
				return 0;
			}
			
			//Vector3 posInWorld = transform.TransformPoint( 0f,-1.65f,0f );
			//transform.position = posInWorld;
			Vector3 oldPos = transform.position;
			oldPos.y -= 1.65f;
			transform.position = oldPos;
			
			justJumpedRope = catchedRope;
			
			catchedRope.resetDiver ();
			catchedRope = null;
			catchedRopeLink = null;
			
			Quaternion quat = new Quaternion ();
			quat.eulerAngles = new Vector3 (0f, 0f, 0f);
			transform.rotation = quat;
			setState (State.IN_AIR);
			
			return 1;
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
				if( dir() == Vector2.right )
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
				//turnLeft();
				return false;
			}
			//turnLeft();
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
	bool keyRightDown(){
		if ( (isInAction (Action.IDLE) || moving(1) || jumping()) && isInState(State.ON_GROUND) ) {
			if( checkRight (0.1f) >= 0.0f ) {
				//print ("cant move right");
				if( dir () == -Vector2.right)
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
				//turnRight();
				return false;
			}
			//turnRight();
			desiredSpeedX = CrouchSpeed;
			//setAction(Action.CROUCH_RIGHT);
			if( dir () == Vector2.right ){
				setAction(Action.CROUCH_RIGHT);
			}else{
				setAction(Action.CROUCH_RIGHT_BACK);
			}
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

		jumpKeyPressed = true;

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
	
	void keyJumpUp(){
		jumpKeyPressed = false;
		jumpFromMount = false;
		justJumpedRope = null;
		canJumpAfter = true;
	}
	
	bool keyUpDown(){
		if (isInState (State.MOUNT)) {
			if( !mounting () ){
				Vector3 playerPos = transform.position;
				playerPos.y += 0.1f;
				if( onMount(playerPos) ){
					velocity.x = 0.0f;
					velocity.y = MountSpeed;
					setAction (Action.MOUNT_UP);
					return true;
				}
			}
		} else if (isInState (State.ON_GROUND)) {
			if( onMount() ){
				velocity.x = 0.0f;
				velocity.y = MountSpeed;
				setAction (Action.MOUNT_UP);
				setState(State.MOUNT);
				return true;
			}
		}
		return false;
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

			//crouch();
			setAction(Action.CROUCH_IN);
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
			if( crouching() || isInAction(Action.CROUCH_IN) ){
				if( canGetUp() ){
					//getUp();
					setAction(Action.GET_UP);
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
			case Action.CROUCH_IN:
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
					//setAction (Action.CROUCH_LEFT);
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
					//setAction (Action.CROUCH_RIGHT);
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

		//lastHandlePos = new Vector3();
		lastFrameHande = false;
	}

	void jumpFromClimb(){
		addImpulse(new Vector2(0.0f, JumpImpulse));
		setState(State.IN_AIR);
		setAction (Action.JUMP,1);
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

	//bool jumpAfterTurn = false;

	void turnLeftStart(){
		setAction (Action.TURN_STAND_LEFT);

		//jumpAfterTurn = false;
		//wantJumpAfter = false;
		if (Input.GetKeyDown (keyJump) || ( Input.GetKey (keyJump) && canJumpAfter) )
			wantJumpAfter = true;
	}

	void turnRightStart(){
		setAction (Action.TURN_STAND_RIGHT);

		//wantJumpAfter = false;
		if (Input.GetKeyDown (keyJump) || (Input.GetKey (keyJump) && canJumpAfter) )
			wantJumpAfter = true;
	}

	void turnLeftFinish(){
		setAction (Action.IDLE);
		//action = Action.IDLE;
		//keyJumpDown ();

		//if (jumpKeyPressed || jumpAfterTurn) {
		if( wantJumpAfter ) {
			jumpLeft();

			if( Input.GetKey(keyJump) )
				canJumpAfter = false;

//			if( Input.GetKey(keyLeft) ){
//				jumpLeft();
//			}

		} else {
			resetActionAndState ();
		}
	}
	
	void turnRightFinish(){
		setAction (Action.IDLE);
		//action = Action.IDLE;
		//resetActionAndState ();

		//if (jumpKeyPressed || jumpAfterTurn) {

		if( wantJumpAfter) {
			jumpRight();

			if( Input.GetKey(keyJump) )
				canJumpAfter = false;

//			if( Input.GetKey(keyRight) ){
//				jumpRight();
//			}
			
		} else {
			resetActionAndState ();
		}
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
			//resetActionAndState();
			return true;
		}
		return false;
	}

	void resetActionAndState(){
		if (isInState (State.ON_GROUND)) {
//			if( jumpKeyPressed ){
//
//			}
			if (Input.GetKey (keyDown)) { //&& (Input.GetKey(keyLeft) || Input.GetKey(keyRight)) ){
				if (!keyDownDown ())
					setActionIdle ();
			} else if (Input.GetKey (keyLeft)) {
				//if (Input.GetKey (keyLeft)) {
				if (!keyLeftDown ())
					setActionIdle ();
			} else if (Input.GetKey (keyRight)) {
				if (!keyRightDown ())
					setActionIdle ();
				//} else if( Input.GetKey(keyDown) ) { //&& (Input.GetKey(keyLeft) || Input.GetKey(keyRight)) ){
				//		if( !keyDownDown() )
				//			setActionIdle();
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
				//if (Input.GetKey (keyLeft)) {
				if (!keyLeftDown ())
					setMountIdle ();
			} else if (Input.GetKey (keyRight)) {
				if (!keyRightDown ())
					setMountIdle ();
				//} else if( Input.GetKey(keyDown) ) { //&& (Input.GetKey(keyLeft) || Input.GetKey(keyRight)) ){
				//		if( !keyDownDown() )
				//			setActionIdle();
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

	float checkLeft(float checkingDist, bool flying = false){
		Vector2 rayOrigin;
		if (flying) {
 			rayOrigin = transform.position;
			rayOrigin.x -= myHalfWidth;
		} else {
			rayOrigin = sensorLeft1.position; // new Vector2( sensorRight1.position.x, sensorRight1.position.y );
		}

		//Vector2 rayOrigin = new Vector2( sensorLeft1.position.x, sensorLeft1.position.y );

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

	float checkRight(float checkingDist, bool flying = false){
		Vector2 rayOrigin;
		if (flying) {
			rayOrigin = transform.position;
			rayOrigin.x += myHalfWidth;
		} else {
			rayOrigin = sensorRight1.position; // new Vector2( sensorRight1.position.x, sensorRight1.position.y );
		}
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

		if (dir () == Vector2.right) {
			RaycastHit2D hit = Physics2D.Raycast (sensorLeft3.position, Vector2.right, myWidth, layerIdGroundMask);
			if (hit.collider != null) {
				float hpx = hit.point.x;  
				float _d = Mathf.Abs (sensorLeft3.position.x + myWidth - hpx);
				if (_d > 0.0001f)
					return false;
			}
			return true;
		} else {
			RaycastHit2D hit = Physics2D.Raycast (sensorRight3.position, -Vector2.right, myWidth, layerIdGroundMask);
			if (hit.collider != null) {
				float hpx = hit.point.x;  
				float _d = Mathf.Abs (sensorRight3.position.x - myWidth - hpx);
				if (_d > 0.0001f)
					return false;
			}
			return true;
		}

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
				if ((lastCatchedClimbHandle == hit.collider.gameObject) ) { //{ && velocity.y >= 0.0f) {
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
				if ((lastCatchedClimbHandle == hit.collider.gameObject) ) { // && velocity.y >= 0.0f) {
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

		
			RaycastHit2D hit; 
			if (lastFrameHande)
				hit = Physics2D.Linecast (lastHandlePos, sensorHandleR2.position, layerIdRopesMask);
			else
				hit = Physics2D.Linecast (sensorHandleR2.position, sensorHandleR2.position, layerIdRopesMask); 

			if( hit.collider == null ){
				hit = Physics2D.Linecast( sensorHandleL2.position, sensorHandleR2.position, layerIdRopesMask); 
			}

			if (hit.collider != null) {
				// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
				bool _canCatch = true;
//				if ((lastCatchedClimbHandle == hit.collider.gameObject) && velocity.y >= 0.0f) {
//					_canCatch = false;
//				}
				
				if (_canCatch) {

					catchedRopeLink = hit.collider.transform.GetComponent<RopeLink>();

					if( justJumpedRope == catchedRopeLink.rope ){
						catchedRopeLink = null;
						lastHandlePos = sensorHandleR2.position;
						return false;
					}

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

					//cameraTarget.position.y = cameraTargetRopeDiffY;

					transform.position = catchedRopeLink.transform.position;
					transform.rotation = catchedRopeLink.transform.rotation;

					ropeLinkCatchOffset = 0.0f;

					return true;
				}
			}
			
			lastHandlePos = sensorHandleR2.position;
			return false;
			
		} else {

			//RaycastHit2D hit = Physics2D.Linecast(sensorHandleL1.position, sensorHandleL2.position, layerIdGroundHandlesMask); 
			RaycastHit2D hit; 
			if (lastFrameHande)
				hit = Physics2D.Linecast (lastHandlePos, sensorHandleL2.position, layerIdRopesMask);
			else
				hit = Physics2D.Linecast (sensorHandleL2.position, sensorHandleL2.position, layerIdRopesMask); 
			
			if( hit.collider == null ){
				hit = Physics2D.Linecast( sensorHandleL2.position, sensorHandleR2.position, layerIdRopesMask); 
			}

			if (hit.collider != null) {
				
				// tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
				bool _canCatch = true;
//				if ((lastCatchedClimbHandle == hit.collider.gameObject) && velocity.y >= 0.0f) {
//					_canCatch = false;
//				}				
				if (_canCatch) {

					catchedRopeLink = hit.collider.transform.GetComponent<RopeLink>();
					
					if( justJumpedRope == catchedRopeLink.rope ){
						catchedRopeLink = null;
						lastHandlePos = sensorHandleL2.position;
						return false;
					}

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

	public float ClimbPullDownRange = 0.511f;

	GameObject canClimbPullDown(){

		if (!isInState (State.ON_GROUND) || !(isInAction (Action.IDLE) || isInAction(Action.CROUCH_IDLE)) )
			return null;

		// 1: sytuacja gdy zap jest swoim srodkiem nad tilem
		// 2: sytuacja gdy zap jest swoim srodkiem juz poza tilem

		RaycastHit2D hit;

		if (dir () == Vector2.right) { //
			
			hit = Physics2D.Raycast (sensorDown2.position, -Vector2.right , ClimbPullDownRange, layerIdGroundHandlesMask);
			if( hit.collider )
				return hit.collider.gameObject;
			
		} else {
			
			hit = Physics2D.Raycast (sensorDown2.position, Vector2.right , ClimbPullDownRange, layerIdGroundHandlesMask);
			if( hit.collider )
				return hit.collider.gameObject;
			
		}
//
//		return null;


		// to jest ta druga sytuacja ...

		Vector2 rayOrigin = sensorDown1.position; 
	 	hit = Physics2D.Raycast (rayOrigin, Vector2.right , myWidth, layerIdGroundHandlesMask);

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
		Vector2 rayOrigin = sensorLeft3.transform.position; // transform.position;
		rayOrigin.y += 0.3f;
		//rayOrigin.y += 1.0f;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdMountMask);
		//return hit.collider;
//		if (hit.collider) {
//			return hit.distance 
//		}
//		return false;

		if (!hit.collider)
			return false;

		hit = Physics2D.Raycast (rayOrigin, -Vector2.up, 1f, layerIdMountMask);
		if (!hit.collider)
			return false;

		rayOrigin.x += myWidth;
		hit = Physics2D.Raycast (rayOrigin, -Vector2.up, 1f, layerIdMountMask);
		return hit.collider;
	}

	bool onMount(Vector3 posToCheck){
		Vector3 sensorDiff = sensorLeft3.transform.position - transform.position; // transform.position;

		Vector2 rayOrigin = posToCheck + sensorDiff;//aaa
		rayOrigin.y += 0.3f;

		//rayOrigin.y += 1.0f;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdMountMask);
		//return hit.collider;

		if (!hit.collider)
			return false;

		hit = Physics2D.Raycast (rayOrigin, -Vector2.up, 1f, layerIdMountMask);
		if (!hit.collider)
			return false;

		rayOrigin.x += myWidth;
		hit = Physics2D.Raycast (rayOrigin, -Vector2.up, 1f, layerIdMountMask);
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

	bool firstFrameInState = false;

	bool setState(State newState){
		
		//print ("setState oldState : " + state);
		//print ("setState newState : " + newState);
		
		if (state == newState)
			return false;

		currentStateTime = 0.0f;
		firstFrameInState = true;
		//print ("setState : " + newState + " ustawiona");
		//print ("============================");

		state = newState;

		//cameraTarget.localPosition = new Vector3(0f,0f,0f);

		switch (state) {
		case State.IN_AIR:
 			startFallPos = transform.position;
			break;
		case State.CLIMB_ROPE:
			//cameraTarget.localPosition = new Vector3(0f, cameraTargetRopeDiffY, 0f);
			break;
		case State.MOUNT:
			animator.Play("mount_up");
			//gfx.GetComponent<SpriteRenderer>().sprite
			break;
		};

		return true;
	}
	public bool isInState(State test) {
		return state == test;
	}
	public bool isNotInState(State test) {
		return state != test;
	}
	
	/*////////////////////////////////////////////////////////////*/

	SpriteRenderer sprRend = null;
	public Sprite mountIdleSprite = null;
	public Sprite catchIdleSprite = null;

	Action getAction(){
		return action;
	}
	bool setAction(Action newAction, int param = 0){
		
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
			animator.Play("zapidle");
			//animator.Play ("zapidle_var1");
			//animator.Play ("zapidle_var2");
			break;

		case Action.DIE:
			DeathType dt = (DeathType)param;
			string msgInfo = "";

			//public string DeathByVeryHardLandingText = "rozjeb... sie o skale. press space";
			//public string DeathBySnakeText = "pokasal cie waz. press space";
			//public string DeathByCrocodileText = "zjadl cie krokodyl. press space";
			//public string DeathByPoisonText = "zatrules sie. press space";
			//public string DeathByDefaultText = ....
			switch( dt ){

			case DeathType.VERY_HARD_LANDING:
				animator.Play ("death_hitground");
				msgInfo = DeathByVeryHardLandingText;
				break;

			case DeathType.SNAKE:
				animator.Play ("Zap_death_poison");
				msgInfo = DeathBySnakeText;
				break;

			case DeathType.POISON:
				animator.Play ("Zap_death_poison");
				msgInfo = DeathByPoisonText;
				break;

			case DeathType.CROCODILE:
				animator.Play ("zap_die");
				msgInfo = DeathByCrocodileText;
				break;

			default:
				animator.Play ("zap_die");
				msgInfo = DeathByDefaultText;
				break;

			};
			//if( param == (int)DeathType.VERY_HARD_LANDING ){
			//	animator.Play ("death_hitground");
			//}else{ //if( param == 0 ){
			//	animator.Play ("zap_die");
			//}

			showInfo (msgInfo, -1);

			if( dieSounds.Length != 0 )
				audio.PlayOneShot(dieSounds[Random.Range(0,dieSounds.Length)], 0.3F);
			break;

		case Action.WALK_LEFT:
		case Action.WALK_RIGHT:
			animator.Play ("zapwalk");
			break;
			
		case Action.RUN_LEFT:
		case Action.RUN_RIGHT:
			//animator.SetTrigger("run");
			animator.Play("zaprun");
			break;

		case Action.TURN_STAND_LEFT:
			//animator.Play("stand_turn_left");
			animator.Play("stand_turn_left");
			wantJumpAfter = false;
			break;

		case Action.TURN_STAND_RIGHT:
			//animator.Play("stand_turn_right");
			animator.Play("stand_turn_right");
			wantJumpAfter = false;
			break;

		case Action.TURN_RUN_LEFT:
			//animator.Play("stand_turn_left");
			animator.Play("run_turn_left");
			wantJumpAfter = false;
			if( turnRunSounds.Length != 0 )
				audio.PlayOneShot(turnRunSounds[Random.Range(0,turnRunSounds.Length)], 0.5F);
			break;
			
		case Action.TURN_RUN_RIGHT:
			animator.Play("run_turn_right");
			wantJumpAfter = false;
			if( turnRunSounds.Length != 0 )
				audio.PlayOneShot(turnRunSounds[Random.Range(0,turnRunSounds.Length)], 0.5F);
			break;
		case Action.PREPARE_TO_JUMP:
			animator.Play("preparetojump");
			break;

		case Action.JUMP:
			if( param == 0 ){
				animator.Play("jump");
			}else if (param == 1) {
				animator.Play("zap_jump_from_climb");
			}
			if( jumpSounds.Length != 0 )
				audio.PlayOneShot(jumpSounds[Random.Range(0,jumpSounds.Length)], 0.2F);
			break;
			
		case Action.JUMP_LEFT:
		case Action.JUMP_LEFT_LONG:
		case Action.JUMP_RIGHT:
		case Action.JUMP_RIGHT_LONG:
			animator.Play("zapjump");
			if( jumpSounds.Length != 0 )
				audio.PlayOneShot(jumpSounds[Random.Range(0,jumpSounds.Length)], 0.2F);
			break;

		case Action.LANDING_HARD:
			animator.Play("landing_hard");
			if( landingSounds.Length != 0 )
				audio.PlayOneShot(landingSounds[Random.Range(0,landingSounds.Length)], 0.15F);
			break;

		case Action.CLIMB_PREPARE_TO_JUMP:
			//animator.SetTrigger("climb_preparetojump");
			break;
		case Action.CLIMB_JUMP_TO_CATCH:
			//animator.SetTrigger("climb_jump");
			//animator.Play("zapclimbjump");
			break;
		case Action.CLIMB_CATCH:
			//animator.SetTrigger("climb_catch");
			if( param == 0 ){
				animator.Play("zapclimbcatch");
			}else if( param == 1 ){
				// tu juz jest we wlasciwej klatce
				animator.Play("zapclimbcatch_rev");
				animator.speed = 0.0f;
				//sprRend.sprite = catchIdleSprite;
			}
			//animator.speed = 0f;

			if( catchSounds.Length != 0)
				audio.PlayOneShot(catchSounds[Random.Range(0,catchSounds.Length)], 0.7F);
			break;
		case Action.CLIMB_CLIMB:
			//animator.SetTrigger("climb_climb");
			animator.Play("zapclimbclimb");
			break;

		case Action.CLIMB_PULLDOWN:
			//animator.Play("zappulldown");
			animator.Play("zapdrop");
			break;

		case Action.MOUNT_IDLE:
			//animator.SetTrigger("mount_idle");
			//animator.StopPlayback();
			//animator.Play("mount_up");
			animator.speed = 0.0f;
			sprRend.sprite = mountIdleSprite;
			break;

		case Action.MOUNT_LEFT:
			//animator.SetTrigger("mount_left");
			animator.Play("mount_left");
			break;
		case Action.MOUNT_RIGHT:
			//animator.SetTrigger("mount_right");
			animator.Play("mount_right");
			break;
		case Action.MOUNT_UP:
			animator.Play("mount_up");
			break;
		case Action.MOUNT_DOWN:
			//animator.SetTrigger("mount_down");
			animator.Play("mount_down");
			break;

		case Action.CROUCH_IN:
			animator.Play("zapcrouchin");
			break;
			
		case Action.GET_UP:
			animator.Play("zapgetup");
			break;

		case Action.CROUCH_IDLE:
			//animator.SetTrigger("crouchidle");
			//animator.Play("crouch_leftright");
			animator.Play("Zap_Crouch_walking_left");
			animator.speed = 0f;
			break;

		case Action.CROUCH_LEFT:
			//animator.Play("crouch_leftright");
			animator.Play("Zap_Crouch_walking_left");
			//animator.GetCurrentAnimatorClipInfo().
			//if( dir () != -Vector2.right ){
			//	animator.speed = -1.0f;
			//}
			break;
		case Action.CROUCH_RIGHT:
			//animator.Play("crouch_leftright");
			animator.Play("Zap_Crouch_walking_right");
			//if( dir () != Vector2.right ){
			//	animator.speed = -1.0f;
			//}
			break;

		case Action.CROUCH_LEFT_BACK:
			animator.Play("Zap_Crouch_walking_left_back");
			break;

		case Action.CROUCH_RIGHT_BACK:
			animator.Play("Zap_Crouch_walking_right_back");
			break;

		case Action.ROPECLIMB_IDLE:
			//animator.SetTrigger("climbrope_idle");
			animator.Play("newclimbrope_idle");
			//animator.speed = 0f;
			break;

		case Action.ROPECLIMB_UP:
			animator.Play("newclimbrope_updown");
			break;

		case Action.ROPECLIMB_DOWN:
			animator.Play("Zap_liana_slide");
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

	Transform cameraTarget;

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
	bool jumpFromGround = false;
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
	bool wantJumpAfter = false;
	bool canJumpAfter = true;

	[SerializeField]
	private State state;
	[SerializeField]
	private Action action;
}
