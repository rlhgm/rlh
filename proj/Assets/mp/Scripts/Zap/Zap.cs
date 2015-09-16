using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Zap : MonoBehaviour {
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
	public float SpeedUpParam = 7.0f; // ile jednosek predkosci hamuje na sekund
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

	AudioSource myAudio;

	public Camera mainCamera;
	public Camera touchCamera;

	public bool autoCatchEdges = false;

	//Transform shadowTransform;
	//SpriteRenderer shadowSpriteRenderer;
	//Sprite shadowSprite;

	Transform shadowCenter;
	Transform shadowLeft;
	Transform shadowRight;
	SpriteRenderer shadowCenterSR;
	SpriteRenderer shadowLeftSR;
	SpriteRenderer shadowRightSR;

	ZapController currentController;
	ZapController zapControllerNormal;

	void Awake(){
		zapControllerNormal = new ZapNormalController(this);
		currentController = zapControllerNormal;

		guiCanvas = FindObjectOfType<Canvas> ();

		if (guiCanvas) {
			infoLabel = FindObjectOfType<Text> ();
			infoLabel.text = "";

			Image[] allImages = FindObjectsOfType<Image>();
			for( int i = 0 ; i < allImages.Length ; ++i ){
				Image img = allImages[i];
				if( img.gameObject.name == "mapBackgroundImage" ){
					mapBackgroundImage = img;
					Color newColor = new Color(1f,1f,1f,0f);
					mapBackgroundImage.color = newColor;
					continue;
				}

				ComicPagePart cpp = img.GetComponent<ComicPagePart>();
				if( cpp ){
					mapPartParts[cpp.partID] = cpp;
				}
			}
		}

		coll = GetComponent<BoxCollider2D> ();
		gfx  = transform.Find("gfx").transform;
		animator = transform.Find("gfx").GetComponent<Animator>();
		sprRend = gfx.GetComponent<SpriteRenderer> ();

		shadowCenter = transform.Find ("shadowCenter");
		if (shadowCenter) {
			shadowLeft = shadowCenter.Find ("shadowLeft");
			shadowRight = shadowCenter.Find ("shadowRight");

			shadowCenterSR = shadowCenter.GetComponent<SpriteRenderer>();
			shadowLeftSR = shadowLeft.GetComponent<SpriteRenderer>();
			shadowRightSR = shadowRight.GetComponent<SpriteRenderer>();
		}

		zap_idle1_beh[] behs = animator.GetBehaviours<zap_idle1_beh>();
		for( int b = 0 ; b < behs.Length ; ++b ){
			behs[b].playerController = this;
		}

		myAudio = GetComponent<AudioSource>();

		PlaySounds[] pss = animator.GetBehaviours<PlaySounds>();
		for( int b = 0 ; b < pss.Length ; ++b ){
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
		layerIdGroundMoveableMask = 1 << LayerMask.NameToLayer("GroundMoveable");
		layerIdGroundAllMask = layerIdGroundMask | layerIdGroundPermeableMask | layerIdGroundMoveableMask;
		layerIdLastGroundTypeTouchedMask = layerIdGroundMask;
		
		layerIdGroundHandlesMask = 1 << LayerMask.NameToLayer("GroundHandles");
		
		layerIdMountMask = 1 << LayerMask.NameToLayer("Mount");
		layerIdRopesMask = 1 << LayerMask.NameToLayer("Ropes");

		myWidth = coll.size.x;
		myHalfWidth = myWidth * 0.5f;
		//myHeight = coll.size.y;
		//myHalfHeight = myHeight * 0.5f;
		desiredSpeedX = 0.0f;

		lastHandlePos = new Vector3();
		lastFrameHande = false;
	}


	public AudioSource getAudioSource(){
		return myAudio;
	}
	public Transform getCameraTarget(){
		return cameraTarget;
	}

	void Start () {
		velocity = new Vector3 (0, 0, 0);
		impulse = new Vector3 (0, 0, 0);
		desiredSpeedX = 0.0f;
		startFallPos = transform.position;

		setState (State.ON_GROUND);
		setAction (Action.IDLE);

		climbDuration = 0.0f;
		catchedClimbHandle = null;
		canPullUp = false;

		jumpFromMount = false;

		lastTouchedCheckPoint = null;
	}

	public void StateIdleExit(){
	}
	public void StateIdleUpdate(float normTime){
	}

	public int IdleAnimFreq = 10;

	public void StateIdleFinish(int stateIdleNum){
		switch( stateIdleNum ){
		case 1:
		case 2:
			if( faceRight() ) animator.Play("Zap_idle_R");
			else animator.Play ("Zap_idle_L");
			break;
		case 0:
			if( IdleAnimFreq >= 3 ){
				int r = Random.Range(0,IdleAnimFreq);
				if( r == 0 ){
					if( faceRight() ) animator.Play ("Zap_idle_variation_1_R");
					else animator.Play ("Zap_idle_variation_1_L");
				}else if( r == 1 ){
					if( faceRight() ) animator.Play ("Zap_idle_variation_2_R");
					else animator.Play ("Zap_idle_variation_2_L");
				}
			}
			break;
		}
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

	public void die(DeathType deathType){
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		setAction (Action.DIE, (int)deathType);
		setState (State.OTHER);

		foreach(GameObject mapPart in collectedMapParts){

			ComicPage comicPart = mapPart.GetComponent<ComicPage>();
			int mapPartID = comicPart.partID;
			
			mapPartParts [mapPartID].collected = false;
			
			mapPart.GetComponent<SpriteRenderer> ().enabled = true;
		}

		collectedMapParts.Clear ();
	}

	public bool isDead(){
		return action == Action.DIE && state == State.OTHER;
	}

	public void reborn(){
		sprRend.enabled = true;
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		setAction (Action.IDLE);
		setState (State.ON_GROUND);

		if (lastTouchedCheckPoint) {
			transform.position = lastTouchedCheckPoint.transform.position;
		} else {
			transform.position = respawnPoint.position;
		}

		if (lastTouchedCheckPoint.GetComponent<CheckPoint> ().startMounted) {
			setState(State.MOUNT);
			setMountIdle();
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

			other.gameObject.GetComponent<Crocodile>().attackStart();
			sprRend.enabled = false;

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

	float puzzleMapShowTime = 0.0f;
	bool puzzleMapShowing = false;
	int puzzleShowingPhase = 0;
	int newPuzzleCollectedID = 0;

	List<GameObject> collectedMapParts = new List<GameObject>();

	void collectMapPart(GameObject mapPart){

		ComicPage comicPart = mapPart.GetComponent<ComicPage>();
		int mapPartID = comicPart.partID;

		if (mapPartParts.Length == 0 || mapPartParts[mapPartID] == null ) 
			return;

	 	if (mapPartParts [mapPartID].collected)
			return;

		mapPartParts [mapPartID].collect ();

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

	void FixedUpdate(){
		if (currentController != null) {
			currentController.FUpdate(Time.fixedDeltaTime);
		}
	}

	private float ConstantFrameTime = 0.0333f;
	private float CurrentDeltaTime = 0.0f;

	// Update is called once per frame
	void Update () {
		float timeSinceLastFrame = Time.deltaTime;
		//print ("Update() : " + timeSinceLastFrame);

		if (GlobalUpdate (timeSinceLastFrame)) return;

		while (timeSinceLastFrame > ConstantFrameTime) {
			ZapUpdate(ConstantFrameTime);
			timeSinceLastFrame -= ConstantFrameTime;
		}

		ZapUpdate (timeSinceLastFrame);
	}

	bool GlobalUpdate (float deltaTime) {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		if (Input.GetKeyDown (KeyCode.P)) {
			gamePaused = !gamePaused;
		}
		
		if( Input.GetMouseButtonDown(0) ){ // left
			//print ("left: " + Input.mousePosition);
		}
		if( Input.GetMouseButton(1) ){ // right
			//print ("right: " + Input.mousePosition);
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			//setPrevWeapon();
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			//setNextWeapon();
		}

		PuzzleMapUpdate (deltaTime);
		if (GamePausedUpdate (deltaTime))
			return true;

		InfoLabelUpdate (deltaTime);


//		if (Input.GetKeyDown (KeyCode.B)) {
//			print( shadowSprite.uv );
//			print( shadowSprite.border );
//			print( shadowSprite.bounds );
//			print( shadowSprite.rect );
//		}

		return false;
	}

	void PuzzleMapUpdate(float deltaTime){
		if (puzzleMapShowing) {
			puzzleMapShowTime += deltaTime;
			
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
	}
	bool GamePausedUpdate(float deltaTime){
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
			return true;
		}
		return false;
	}

	void InfoLabelUpdate(float deltaTime){
		if (infoLabelSet) {
			if (infoLabelShowDuration > 0) {
				if ((infoLabelShowTime += deltaTime) > infoLabelShowDuration) {
					infoLabelSet = false;
					infoLabel.text = "";
				}
			}
		}
	}

	void ZapUpdate (float deltaTime) {
		CurrentDeltaTime = deltaTime;

		if (isInAction (Action.DIE)) {
			if( Input.GetKeyDown(KeyCode.Space) ){
				reborn();
				return;
			}
		}

		if (userJumpKeyPressed) {
			if (Input.GetKeyDown (keyJump)) {
				timeFromJumpKeyPressed = 0.0f;
				userJumpKeyPressed = true;
			}
		} else {
			timeFromJumpKeyPressed += deltaTime;
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

		stateJustChanged = false;
		currentStateTime += deltaTime;
		currentActionTime += deltaTime;

		currentController.Update( CurrentDeltaTime );

		updateShadow ();
	}

	void updateShadow(){
		if (!shadowCenter)
			return;

		float msd = 2f;

		RaycastHit2D hit = Physics2D.Raycast (sensorDown2.position, -Vector2.up, msd, layerIdGroundMask);
		if (hit.collider) {
			shadowCenterSR.enabled = true;
			shadowCenterSR.color = new Color (1f, 1f, 1f, (msd-hit.distance) / msd);

			Vector3 shadowPos = shadowCenter.localPosition;
			shadowPos.y = -hit.distance;
			shadowCenter.localPosition = shadowPos;

			shadowCenter.rotation = hit.collider.transform.rotation;
		} else {
			shadowCenterSR.enabled = false;
		}

		hit = Physics2D.Raycast (sensorDown1.position, -Vector2.up, msd, layerIdGroundMask);
		if (hit.collider) {
			shadowLeftSR.enabled = true;
			shadowLeftSR.color = new Color (1f, 1f, 1f, (msd-hit.distance) / msd);

			float colliderRot = hit.collider.transform.rotation.eulerAngles.z;
			float r = colliderRot - shadowCenter.rotation.eulerAngles.z;
			Quaternion quat = new Quaternion ();
			quat.eulerAngles = new Vector3 (0f, 0f, r);
			shadowLeft.localRotation = quat;
		} else {
			shadowLeftSR.enabled = false;
		}

		hit = Physics2D.Raycast (sensorDown3.position, -Vector2.up, msd, layerIdGroundMask);
		if (hit.collider) {
			shadowRightSR.enabled = true;
			shadowRightSR.color = new Color (1f, 1f, 1f, (msd-hit.distance) / msd);

			float colliderRot = hit.collider.transform.rotation.eulerAngles.z;
			float r = colliderRot - shadowCenter.rotation.eulerAngles.z;
			Quaternion quat = new Quaternion ();
			quat.eulerAngles = new Vector3 (0f, 0f, r);
			shadowRight.localRotation = quat;
		} else {
			shadowRightSR.enabled = false;
		}
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
		if( potCatchedClimbHandle ){
			
			catchedClimbHandle = potCatchedClimbHandle;
			
			velocity.x = 0.0f;
			velocity.y = 0.0f;
			climbDuration = 0.0f;
			
			Vector3 handlePos = potCatchedClimbHandle.transform.position;
			
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

			float velocityDamp = SpeedUpParam * CurrentDeltaTime;
			speedX += velocityDamp;
			if( speedX > desiredSpeedX ){
				speedX = desiredSpeedX;
				velocity.x = desiredSpeedX * dir;
				return true;
			}
			velocity.x = speedX * dir;
			return false;

		} else if (speedX > desiredSpeedX) { // trzeba zwolnic
			float velocityDamp = SlowDownParam * CurrentDeltaTime;
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

	bool tryBreakUpRope(float deltaTime){

		if (catchedRopeLink) {
			if( catchedRopeLink.rope.breakUpStep( catchedRopeLink.idn, deltaTime ) ){
				tryJumpFromRope(true);
				return false;
			}
		}

		return false;
	}



	int tryJumpFromRope(bool forceJumpOff = false){

		if (Input.GetKeyDown (keyJump) || forceJumpOff) {
			
			float ropeSpeed = catchedRope.firstLinkSpeed;
			float ropeSpeedRad = ropeSpeed * Mathf.Deg2Rad;
			int crl_idn = catchedRope.currentLink.GetComponent<RopeLink> ().idn;
			float ps = ropeSpeedRad * (crl_idn + 1) * 0.5f;

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
			} else if( Input.GetKeyDown (keyDown) || Input.GetKey (keyDown) || forceJumpOff ) {
				velocity.x = 0f;
				velocity.y = 0f;
				setAction (Action.JUMP);
			}else{
				return 0;
			}
			
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

			//Quaternion quat = new Quaternion ();
			//quat.eulerAngles = new Vector3 (0f, 0f, 0f);
			//weaponText.rotation = quat;

			return 1;
		}

		return 0;
	}

	virtual int keyLeftDown(){
		return 0;
	}
	virtual int keyLeftUp(){
		return 0;
	}

	virtual int keyRightDown(){
		return 0;
	}
	virtual int keyRightUp(){
		return 0;
	}

	virtual int keyUpDown(){
		return 0;
	}
	virtual int keyUpUp(){
		return 0;
	}
	
	virtual int keyDownDown(){
		return 0;
	}
	virtual int keyDownUp(){
		return 0;
	}

	virtual int keyRunDown(){
		return 0;
	}
	virtual int keyRunUp(){
		return 0;
	}

	virtual int keyJumpDownSpec(){
		return 0;
	}
	virtual int keyJumpUpSpec(){
		return 0;
	}

	int keyJumpDown(){
		jumpKeyPressed = true;
		return keyJumpDownSpec ();
	}
	
	int keyJumpUp(){
		jumpKeyPressed = false;
		return keyJumpUpSpec ();
	}

	void getUp(){
		setAction(Action.IDLE);
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

	public Vector2 dir(){
		return gfx.localScale.x > 0.0f ? Vector2.right : -Vector2.right;
	}
	public int dir2(){
		return gfx.localScale.x > 0f ? (int)1f : (int)-1f;
	}
	public bool faceRight(){
		return gfx.localScale.x > 0f;
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

	float checkLeft(float checkingDist, bool flying = false){
		Vector2 rayOrigin;
		if (flying) {
 			rayOrigin = transform.position;
			rayOrigin.x -= myHalfWidth;
		} else {
			rayOrigin = sensorLeft1.position; // new Vector2( sensorRight1.position.x, sensorRight1.position.y );
		}

		// ponizej robie layerIdGroundAllMask - aby wchodzil na platformy ze skosow nie bedzie sie dalo klasc jednej przepuszczalnej na drugiej ale trudno
 		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundAllMask);
		if (hit.collider != null) {
			float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
			if( angle <= 0.0f || angle > 45.0f ) 
				return Mathf.Abs (hit.point.x - sensorLeft1.position.x);
			else 
			{
				return -1.0f;
			}
		} else {
			if( crouching() ) 
				return -1.0f;

			rayOrigin = new Vector2( sensorLeft2.position.x, sensorLeft2.position.y );
			hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundMask);
			if (hit.collider != null){
				return Mathf.Abs (hit.point.x - sensorLeft2.position.x);
			} else {

				rayOrigin = new Vector2( sensorLeft3.position.x, sensorLeft3.position.y );
				hit = Physics2D.Raycast (rayOrigin, -Vector2.right, checkingDist, layerIdGroundMask);
				if (hit.collider != null){
					return Mathf.Abs (hit.point.x - sensorLeft3.position.x);
				} else {
					return -1.0f;
				}
			}
		}
	}

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
			float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation );
			if( angle <= 0.0f || angle > 45.0f ) return Mathf.Abs (hit.point.x - sensorRight1.position.x);
			else return -1.0f;
		} else {
			if (crouching())
				return -1.0f;

			rayOrigin = new Vector2( sensorRight2.position.x, sensorRight2.position.y );
			hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist, layerIdGroundMask);
			if (hit.collider != null){
				return Mathf.Abs (hit.point.x - sensorRight2.position.x);
			} else {

				rayOrigin = new Vector2( sensorRight3.position.x, sensorRight3.position.y );
				hit = Physics2D.Raycast (rayOrigin, Vector2.right, checkingDist, layerIdGroundMask);
				if (hit.collider != null){
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
	}

	float checkDown(float checkingDist){

		int layerIdMask = layerIdGroundAllMask;
		Vector3 rayOrigin = sensorDown1.position;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundPermeableMask);
		if (hit.collider) {// jesetem wewnatrz wskakiwalnej platformy ... nie moge sie zatrzymac..
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
			dist1 = rayOrigin1.y - hit1.point.y;
			groundUnderFeet = true;
			distToGround = dist1;
			layerIdLastGroundTypeTouchedMask = 1 << hit1.collider.transform.gameObject.layer;
		}
		if (hit2.collider != null) {
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
	}

	bool tryCatchHandle(){
		if (dir () == Vector2.right) {
		
			RaycastHit2D hit; 
			if (lastFrameHande)
				hit = Physics2D.Linecast (lastHandlePos, sensorHandleR2.position, layerIdGroundHandlesMask);
			else
				hit = Physics2D.Linecast (sensorHandleR2.position, sensorHandleR2.position, layerIdGroundHandlesMask); 
		
			if (hit.collider != null) {
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
				
					canPullUp = canClimbPullUp ();
				
					if (canPullUp) {
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
		
			lastHandlePos = sensorHandleR2.position;
			return false;
		
		} else {
		
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
				
					canPullUp = canClimbPullUp ();
				
					if (canPullUp) {
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
					float force = RopeSwingForce * forceRatio;

					if( velocity.x < 0f ){
						catchedRope.setSwingMotor(-Vector2.right, force, 0.25f);
					}else if (velocity.x > 0){ 
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
					float force = RopeSwingForce * forceRatio;

					if( velocity.x < 0f ){
						catchedRope.setSwingMotor(-Vector2.right, force, 0.25f);
					}else if (velocity.x > 0){ 
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

	bool canClimbPullUp(){

		if (!catchedClimbHandle)
			return false;

		Vector2 rayOrigin = catchedClimbHandle.transform.parent.transform.position;
		rayOrigin.x += 0.5f;
		rayOrigin.y += 0.25f;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, 0.5f, layerIdGroundMask);
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
			if( hit.collider ){

				if( Physics2D.Raycast (hit.collider.gameObject.transform.position, -Vector2.right , 0.5f, layerIdGroundMask).collider == null ) {
					return hit.collider.gameObject;
				}
			}

		} else {
			
			hit = Physics2D.Raycast (sensorDown2.position, Vector2.right , ClimbPullDownRange, layerIdGroundHandlesMask);
			if( hit.collider ){

				if( Physics2D.Raycast (hit.collider.gameObject.transform.position, Vector2.right , 0.5f, layerIdGroundMask).collider == null ) {
					return hit.collider.gameObject;
				}

			}
		}

		// to jest ta druga sytuacja ...

		Vector2 rayOrigin = sensorDown1.position; 
	 	hit = Physics2D.Raycast (rayOrigin, Vector2.right , myWidth, layerIdGroundHandlesMask);

		if (hit.collider) { 
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
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdMountMask);

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

		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdMountMask);

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

	public enum State
	{
		ON_GROUND = 0,
		IN_AIR,
		CLIMB,
		MOUNT,
		CLIMB_ROPE,
		OTHER
	};

	public State getState() { 
		return state; 
	}

	bool stateJustChanged = false;

	bool setState(State newState){

		if (state == newState)
			return false;

		currentStateTime = 0.0f;
		stateJustChanged = true;

		state = newState;

		switch (state) {
		case State.IN_AIR:
 			startFallPos = transform.position;
			break;
		case State.CLIMB_ROPE:
			break;
		case State.MOUNT:
			animator.Play("Zap_climbmove_up");
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

	/*////////////////////////////////////////////////////////////*/

	BoxCollider2D coll;
	Animator animator;
	Transform sensorLeft1;
	public Transform sensorLeft2;
	Transform sensorLeft3;
	Transform sensorRight1;
	public Transform sensorRight2;
	Transform sensorRight3;
	Transform sensorDown1;
	Transform sensorDown2;
	Transform sensorDown3;
	
	Transform sensorHandleL2;
	Transform sensorHandleR2;

	Transform cameraTarget;

	Transform gfx;

	[SerializeField]
	Vector3 velocity;
	Vector3 lastVelocity;
	Vector3 lastSwingPos;
	[SerializeField]
	Vector3 impulse;
	Vector3 startFallPos;

	Vector3 mountJumpStartPos;

	float desiredSpeedX = 0.0f;

	float currentActionTime = 0f;
	public float getCurrentActionTime() {
		return currentActionTime;
	}
	public void resetCurrentActionTime(){
		currentActionTime = 0f;
	}
	float currentStateTime = 0.0f;
	public float getCurrentStateTime(){
		return currentStateTime;
	}

	float myWidth;
	float myHalfWidth;
	//float myHeight;
	//float myHalfHeight;
	
	int layerIdGroundMask;
	int layerIdGroundPermeableMask;
	int layerIdGroundMoveableMask;
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
	bool wantJumpAfter = false;
	bool canJumpAfter = true;

	private State state;
}
