using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Zap : MonoBehaviour {
	Canvas guiCanvas = null;
	Text infoLabel = null;
	Image mapBackgroundImage = null;
	ComicPagePart[] mapPartParts = new ComicPagePart[3];


	
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
			pss[b].zap = this;
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




		zapControllerNormal = new ZapControllerNormal(this);
	}

	void Start () {
		currentController = zapControllerNormal;

		velocity = new Vector3 (0, 0, 0);
		impulse = new Vector3 (0, 0, 0);

		startFallPos = transform.position;

		setState (State.ON_GROUND);
		currentController.activate ();

		//climbDuration = 0.0f;





		lastTouchedCheckPoint = null;
	}

	public AudioSource getAudioSource(){
		return myAudio;
	}
	public Transform getCameraTarget(){
		return cameraTarget;
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

		currentController.zapDie (deathType);
		setState (State.DEAD);

		foreach(GameObject mapPart in collectedMapParts){

			ComicPage comicPart = mapPart.GetComponent<ComicPage>();
			int mapPartID = comicPart.partID;
			
			mapPartParts [mapPartID].collected = false;
			
			mapPart.GetComponent<SpriteRenderer> ().enabled = true;
		}

		collectedMapParts.Clear ();

	}

	public bool isDead(){
		//return action == Action.DIE && state == State.OTHER;
		return isInState (State.DEAD);
	}

	public void reborn(){
		sprRend.enabled = true;
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		//setAction (Action.IDLE);

		currentController = zapControllerNormal;
		setState (State.ON_GROUND);
		currentController.activate ();

		if (lastTouchedCheckPoint) {
			transform.position = lastTouchedCheckPoint.transform.position;
		} else {
			transform.position = respawnPoint.position;
		}

		currentController.reborn ();


		resetInfo ();

		NewRope[] ropes = FindObjectsOfType(typeof(NewRope)) as NewRope[];
		foreach (NewRope rope in ropes) {
			rope.reset();
		}
	}
	public GameObject getLastTouchedCheckPoint(){
		return lastTouchedCheckPoint;
	}

	GameObject lastTouchedCheckPoint;

	public bool canBeFuddleFromBird = true;
	bool fuddledFromBrid = false;
	public bool isFuddledFromBrid(){
		return fuddledFromBrid;
	}
	public void setFuddledFromBrid(bool fuddled){
		fuddledFromBrid = fuddled;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (currentController.triggerEnter (other))
			return;

//		if (other.gameObject.tag == "Bird") {
//			if( isInState(State.MOUNT) ){
//				velocity.x = 0.0f;
//				velocity.y = 0.0f;
//				setAction(Action.JUMP);
//				setState(State.IN_AIR);
//
//				if( canBeFuddleFromBird )
//					fuddledFromBrid = true;
//
//			} else if( isInState(State.IN_AIR) ) {
//				velocity.x = 0.0f;
//			}
//			return;
//		}

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
	[HideInInspector]
	public bool jumpKeyPressed = false;

	void FixedUpdate(){
		if (currentController != null) {
			currentController.FUpdate(Time.fixedDeltaTime);
		}
	}

	private float ConstantFrameTime = 0.0333f;
	private float CurrentDeltaTime = 0.0f;

	public float getConstantFrameTime(){
		return ConstantFrameTime;
	}
	public float getCurrentDeltaTime(){
		return CurrentDeltaTime;
	}

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

		if (!userJumpKeyPressed) {
			if (Input.GetKeyDown (keyJump)) {
				timeFromJumpKeyPressed = 0.0f;
				userJumpKeyPressed = true;
			}
		} else {
			timeFromJumpKeyPressed += deltaTime;
			if (timeFromJumpKeyPressed >= 0.06f) {
				timeFromJumpKeyPressed = 0.0f;
				userJumpKeyPressed = false;
				jumpKeyPressed = true;

				currentController.keyJumpDown ();
			}
		}

		SetImpulse(new Vector2(0.0f, 0.0f));

		if (Input.GetKeyDown (keyUp)) {
			currentController.keyUpDown();
		} 
		if (Input.GetKeyUp (keyUp)) {
			currentController.keyUpUp();
		}
		if (Input.GetKeyDown (keyDown)) {
			currentController.keyDownDown();
		} 
		if (Input.GetKeyUp (keyDown)) {
			currentController.keyDownUp();
		}
		
		if (Input.GetKeyUp (keyJump)) {
			jumpKeyPressed = false;
			currentController.keyJumpUp ();
		}
		
		if (Input.GetKeyDown (keyLeft)) {
			currentController.keyLeftDown();
		} 
		if (Input.GetKeyDown (keyRight)) {
			currentController.keyRightDown();
		}
		
		if (Input.GetKeyUp (keyLeft)) {
			currentController.keyLeftUp();
		} 
		if (Input.GetKeyUp (keyRight)) {
			currentController.keyRightUp();
		}
		
		if (Input.GetKeyDown (keyRun)) {
			currentController.keyRunDown();
		} else if (Input.GetKeyUp (keyRun)) {
			currentController.keyRunUp();
		}

		if (isDead()) {
			if( Input.GetKeyDown(KeyCode.Space) ){
				reborn();
				return;
			}
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




	public bool checkObstacle(int dir, float distToCheck, ref float distToObstacle){
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
	
	public void turnLeft(){
		Vector3 scl = gfx.localScale;
		scl.x = Mathf.Abs(scl.x) * -1.0f;
		gfx.localScale = scl;

	}
	public void turnRight(){
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


	public bool canGetUp(){
		
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

	public float checkLeft(float checkingDist, bool flying = false){
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
			if( currentController.crouching() ) 
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

	public float checkRight(float checkingDist, bool flying = false){
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
			if (currentController.crouching())
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

	public float checkDown(float checkingDist){

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

	public bool checkGround (bool fromFeet, int layerIdMask, ref float distToGround){
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

	public bool checkMount(){
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
	
	public bool checkMount(Vector3 posToCheck){
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
		DEAD,
		OTHER
	};

	public State getState() { 
		return state; 
	}

	[HideInInspector]
	public bool stateJustChanged = false;

	public bool setState(State newState){

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
	BoxCollider2D coll;
	public Animator getAnimator(){
		return animator;
	}
	Animator animator;

	//[HideInInspector]
	public Transform sensorLeft1;
	//[HideInInspector]
	public Transform sensorLeft2;
	//[HideInInspector]
	Transform sensorLeft3;
	//[HideInInspector]
	Transform sensorRight1;
	//[HideInInspector]
	public Transform sensorRight2;
	//[HideInInspector]
	Transform sensorRight3;
	[HideInInspector]
	public Transform sensorDown1;
	[HideInInspector]
	public Transform sensorDown2;
	[HideInInspector]
	public Transform sensorDown3;

	[HideInInspector]
	public Transform sensorHandleL2;
	[HideInInspector]
	public Transform sensorHandleR2;

	Transform cameraTarget;
	Transform gfx;

	[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Vector3 lastVelocity;

	Vector3 lastSwingPos;
	[SerializeField]
	Vector3 impulse;
	[HideInInspector]
	public Vector3 startFallPos;


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

	public float getMyWidth(){
		return myWidth;
	}
	public float getMyHalfWidth(){
		return myHalfWidth;
	}
	//float myHeight;
	//float myHalfHeight;

	[HideInInspector]
	public int layerIdGroundMask;
	int layerIdGroundPermeableMask;
	int layerIdGroundMoveableMask;
	[HideInInspector]
	public int layerIdGroundAllMask;
	[HideInInspector]
	public int layerIdLastGroundTypeTouchedMask;
	[HideInInspector]
	public int layerIdGroundHandlesMask;
	[HideInInspector]
	public int layerIdRopesMask;
	int layerIdMountMask;

	float climbDistFromWall;
	//float climbDuration;
	Vector2 climbDir;
	

	

	
	bool gamePaused = false;



	int playerCurrentLayer;

	private State state;
}
