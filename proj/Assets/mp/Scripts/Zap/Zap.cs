using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Zap : MonoBehaviour {
	Canvas guiCanvas = null;
	[HideInInspector]
	public WeaponMenu weaponMenu = null;
	Text infoLabel = null;
	Image mapBackgroundImage = null;
	ComicPagePart[] mapPartParts = new ComicPagePart[3];

	public KeyCode keyLeft = KeyCode.LeftArrow;
	public KeyCode keyRight = KeyCode.RightArrow;
	public KeyCode keyRun = KeyCode.LeftShift;
	public KeyCode keyUp = KeyCode.UpArrow;
	public KeyCode keyDown = KeyCode.DownArrow;
	public KeyCode keyJump = KeyCode.Space;

	//public Transform respawnPoint;
	Vector3 startPoint = new Vector3();

	public AudioClip[] jumpSounds;
	public AudioClip[] landingSounds;
	public AudioClip[] turnRunSounds;
	public AudioClip[] catchSounds;
	public AudioClip[] dieSounds;

	public AudioClip landingSound;
	public AudioClip ropeCatchSound;

	AudioSource myAudio;

	//[HideInInspector]
	//public Camera mainCamera;
	//[HideInInspector]
	//public Camera touchCamera;

	public void setTouchCamera(Camera newTC){
		if (zapControllerNormal)
			zapControllerNormal.setTouchCamera (newTC);
		if (zapControllerKnife)
			zapControllerKnife.setTouchCamera (newTC);
		if (zapControllerGravityGun)
			zapControllerGravityGun.setTouchCamera (newTC);
	}

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

	[HideInInspector]
	public ZapController currentController;
	[HideInInspector]
	public ZapController beforeFallController;
	[HideInInspector]
	public ZapController choosenController;
	public ZapControllerNormal zapControllerNormal; // = ScriptableObject.CreateInstance<ZapControllerNormal>();
	public ZapControllerKnife zapControllerKnife; // = ScriptableObject.CreateInstance<ZapControllerKnife>();
	public ZapControllerGravityGun zapControllerGravityGun; // = ScriptableObject.CreateInstance<ZapControllerKnife>();

	public bool HaveKnife = false;
	public bool HaveGravityGun = false;

	bool _haveKnife = false;
	bool _haveGravityGun = false;

	void Awake(){
		guiCanvas = FindObjectOfType<Canvas> ();

		if (guiCanvas) {
			weaponMenu = guiCanvas.GetComponent<WeaponMenu>();

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
		gfxCollider = gfx.GetComponent<PolygonCollider2D> ();
        gfxLegs = transform.Find("gfxLegs").transform;

		animatorBody = transform.Find("gfx").GetComponent<Animator>();
		sprRend = gfx.GetComponent<SpriteRenderer> ();
        animatorLegs = transform.Find("gfxLegs").GetComponent<Animator>();

		shadowCenter = transform.Find ("shadowCenter");

        targeter = transform.Find("targeter");
        gravityGunBeam = transform.Find("GravityGunBeam");
        gravityGunBeam.GetComponent<LineRenderer>().sortingLayerName = "Player";
        gravityGunBeam.GetComponent<LineRenderer>().sortingOrder = -1;

        if (shadowCenter) {
			shadowLeft = shadowCenter.Find ("shadowLeft");
			shadowRight = shadowCenter.Find ("shadowRight");

			shadowCenterSR = shadowCenter.GetComponent<SpriteRenderer>();
			shadowLeftSR = shadowLeft.GetComponent<SpriteRenderer>();
			shadowRightSR = shadowRight.GetComponent<SpriteRenderer>();
		}

		zap_idle1_beh[] behs = animatorBody.GetBehaviours<zap_idle1_beh>();
		for( int b = 0 ; b < behs.Length ; ++b ){
			behs[b].playerController = this;
		}

		myAudio = GetComponent<AudioSource>();

		PlaySounds[] pss = animatorBody.GetBehaviours<PlaySounds>();
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

		leftKnifeHitPointHigh1 = transform.Find ("leftKnifeHitPointHigh1").transform;
		leftKnifeHitPointHigh2 = transform.Find ("leftKnifeHitPointHigh2").transform;
		rightKnifeHitPointHigh1 = transform.Find ("rightKnifeHitPointHigh1").transform;
		rightKnifeHitPointHigh2 = transform.Find ("rightKnifeHitPointHigh2").transform;
		leftKnifeHitPointLow1 = transform.Find ("leftKnifeHitPointLow1").transform;
		leftKnifeHitPointLow2 = transform.Find ("leftKnifeHitPointLow2").transform;
		rightKnifeHitPointLow1 = transform.Find ("rightKnifeHitPointLow1").transform;
		rightKnifeHitPointLow2 = transform.Find ("rightKnifeHitPointLow2").transform;

		cameraTarget = transform.Find("cameraTarget").transform;

		layerIdGroundMask = 1 << LayerMask.NameToLayer("Ground");
		//layerIdGroundPermeableMask = 1 << LayerMask.NameToLayer("GroundPermeable");
		layerIdGroundMoveableMask = 1 << LayerMask.NameToLayer("GroundMoveable");
		layerIdGroundAllMask = layerIdGroundMask | layerIdGroundMoveableMask;
		//layerIdLastGroundTypeTouchedMask = layerIdGroundMask;
		
		layerIdGroundHandlesMask = 1 << LayerMask.NameToLayer("GroundHandles");
		
		layerIdMountMask = 1 << LayerMask.NameToLayer("Mount");
		layerIdRopesMask = 1 << LayerMask.NameToLayer("Ropes");

		myWidth = coll.size.x;
		myHalfWidth = myWidth * 0.5f;
		//myHeight = coll.size.y;
		//myHalfHeight = myHeight * 0.5f;

//		zapControllerNormal = new ZapControllerNormal(this);
//		zapControllerKnife = new ZapControllerKnife(this);

		//zapControllerNormal = new ZapControllerNormal();
		//zapControllerKnife = new ZapControllerKnife();

		zapControllerNormal.setZap(this);
		zapControllerKnife.setZap(this);
		zapControllerGravityGun.setZap (this);
	}

	void Start () {
		//currentController = zapControllerNormal;
		//currentController = zapControllerKnife;


//		if (!HaveKnife) {
//			//weaponMenu.itemKnife.hide();
//			zapControllerKnife.SetCtrlEnabled(
//		}

		_haveKnife = HaveKnife;
		_haveGravityGun = HaveGravityGun;
		if (HaveKnife) {
			chooseController (zapControllerKnife);
		} else if (HaveGravityGun) {
			chooseController (zapControllerGravityGun);
		}

		zapControllerKnife.SetCtrlEnabled (HaveKnife);
		zapControllerGravityGun.SetCtrlEnabled (HaveGravityGun);

		setCurrentController (zapControllerNormal);


		velocity = new Vector3 (0, 0, 0);
		impulse = new Vector3 (0, 0, 0);

		startFallPos = transform.position;

		setState (State.ON_GROUND);
		currentController.activate ();

		lastTouchedCheckPoint = null;

		startPoint = transform.position;
		beforeFallController = null;
	}

	public void chooseController(ZapController newController){
		if (choosenController != null)
			choosenController.deselected ();
		choosenController = newController;
		choosenController.selected ();
	}

	public void setCurrentController(ZapController newController, bool restore = false, bool crouch = false){
		if (currentController != null)
			currentController.deactivate ();
		currentController = newController;
		currentController.activate (restore, crouch);
	}

	public void _pullOutKnife(){
		setCurrentController (zapControllerKnife);
	}
	public void _hideKnife(){
		setCurrentController (zapControllerNormal);
	}
	public void _pullOutGravityGun(){
		setCurrentController (zapControllerGravityGun);
	}
	public void _hideGravityGun(){
		setCurrentController (zapControllerNormal);
	}

	public int pullChoosenWeapon(bool crouch = false){
		if (choosenController) {
			setCurrentController(choosenController,false,crouch);
			return 1;
		}
		return 0;
	}
	public void hideChoosenWeapon(){
		//if (choosenController) {
		//	setCurrentController(choosenController);
		//}
		setCurrentController (zapControllerNormal);
	}
	public void suddenlyInAir(){
		if (currentController != zapControllerNormal) {
			beforeFallController = currentController;
			setCurrentController(zapControllerNormal);
			setState (Zap.State.IN_AIR);
			zapControllerNormal.suddenlyInAir(); 
		}
	}
	public void restoreBeforeFallController(){
		if (beforeFallController != null) {
			setCurrentController(beforeFallController,true);
			beforeFallController = null;
		}
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
			if( faceRight() ) animatorBody.Play("Zap_idle_R");
			else animatorBody.Play ("Zap_idle_L");
			break;
		case 0:
			if( IdleAnimFreq >= 3 ){
				int r = Random.Range(0,IdleAnimFreq);
				if( r == 0 ){
					if( faceRight() ) animatorBody.Play ("Zap_idle_variation_1_R");
					else animatorBody.Play ("Zap_idle_variation_1_L");
				}else if( r == 1 ){
					if( faceRight() ) animatorBody.Play ("Zap_idle_variation_2_R");
					else animatorBody.Play ("Zap_idle_variation_2_L");
				}
			}
			break;
		}
	}

	public enum DeathType{
		VERY_HARD_LANDING = 1,
		SNAKE,
		CROCODILE,
		PANTHER,
		POISON,
		STONE_HIT
	};

	public string DeathByVeryHardLandingText = "rozjeb... sie o skale. press space";
	public string DeathBySnakeText = "pokasal cie waz. press space";
	public string DeathByCrocodileText = "zjadl cie krokodyl. press space";
	public string DeathByPantherText = "zjadla cie pantera. press space";
	public string DeathByPoisonText = "zatrules sie. press space";
	public string DeathByStoneHitText = "pierdolnela cie skala. press space";
	public string DeathByDefaultText = "zginales defaultowa smiercia. press space";

	public void die(DeathType deathType){
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		gfxCollider.enabled = false;

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
		gfxCollider.enabled = true;
		velocity.x = 0.0f;
		velocity.y = 0.0f;
		//setAction (Action.IDLE);


		HaveKnife = _haveKnife;
		HaveGravityGun = _haveGravityGun;
		if (HaveKnife) {
			chooseController (zapControllerKnife);
		} else if (HaveGravityGun) {
			chooseController (zapControllerGravityGun);
		}
		
		zapControllerKnife.SetCtrlEnabled (HaveKnife);
		zapControllerGravityGun.SetCtrlEnabled (HaveGravityGun);


		currentController = zapControllerNormal;
		setState (State.ON_GROUND);
		currentController.activate ();

		if (lastTouchedCheckPoint) {
			transform.position = lastTouchedCheckPoint.transform.position;
		} else {
			transform.position = startPoint;// respawnPoint.position;
		}

		currentController.reborn ();
		
		resetInfo ();

		NewRope[] ropes = FindObjectsOfType(typeof(NewRope)) as NewRope[];
		foreach (NewRope rope in ropes) {
			rope.reset();
		}
		CutableBush[] bushes = FindObjectsOfType(typeof(CutableBush)) as CutableBush[];
		foreach (CutableBush bush in bushes) {
			bush.reset();
		}
		Panther[] panthers = FindObjectsOfType(typeof(Panther)) as Panther[];
		foreach (Panther panther in panthers) {
			panther.reset();
		}
		Pickable[] pickables = FindObjectsOfType(typeof(Pickable)) as Pickable[];
		foreach (Pickable pickable in pickables) {
			pickable.activate();
		}
        BirdEmiter[] birdEmiters = FindObjectsOfType(typeof(BirdEmiter)) as BirdEmiter[];
        foreach (BirdEmiter birdEmiter in birdEmiters)
        {
            birdEmiter.reset();
        }
    }

    public CheckPoint LastTouchedCheckPoint
    {
        get
        {
            return lastTouchedCheckPoint;
        }
    }
    CheckPoint lastTouchedCheckPoint;

	public bool canBeFuddleFromBird = true;
	bool fuddledFromBrid = false;
    public bool FuddleFromBird
    {
        set
        {
            fuddledFromBrid = value;
        }
        get
        {
            return fuddledFromBrid;
        }
    }

    //public float stoneDeadlySpeed = 8f;
    //public float stoneDeadlyMass = 8f;
    public float stoneDeadlyEnergy = 20f;
	public float stoneMinDeadySpeed = 1f;
	public float stoneMinDeadyMass = 0.5f;

    //public float mass = 5f;
    public float pushedForce = -10f;

	bool hitByStone(Transform stone){
        //return false;

		Rigidbody2D stoneBody = stone.GetComponent<Rigidbody2D> ();
		if (!stoneBody)
			return false;

		if (currentController == zapControllerGravityGun) {
			if( zapControllerGravityGun.draggedStone == stone ){
				die(DeathType.STONE_HIT);
				return true;
			}
		}

		float stoneSpeed = stoneBody.velocity.magnitude;
//		if (stoneSpeed > stoneDeadlySpeed) {
//			die (DeathType.STONE_HIT);
//			return true;
		//} else 
		if (stoneSpeed < stoneMinDeadySpeed) {
			return false;
		}

		float stoneMass = stoneBody.mass;
//		if (stoneMass > stoneDeadlyMass) {
//			die(DeathType.STONE_HIT );
//			return true;
//		}
		if (stoneMass < stoneMinDeadyMass) {
			return false;
		}

		float stoneEnergy = stoneSpeed * stoneMass;
		if (stoneEnergy > stoneDeadlyEnergy) {
			die(DeathType.STONE_HIT );
			return true;
		}

		return false;
	}

    public bool touchStone(Transform stone)
    {
        //return false;

        Rigidbody2D stoneBody = stone.GetComponent<Rigidbody2D>();
        if (!stoneBody)
            return false;

        Vector2 touchedForce = new Vector2(0f, 0f);
        touchedForce.y = pushedForce; // + (velocity.y * mass);
        //touchedForce.y *= -1.0f;
        stoneBody.AddForceAtPosition(touchedForce, transform.position, ForceMode2D.Force);
        //print(touchedForce);

        //if (currentController == zapControllerGravityGun)
        //{
        //    if (zapControllerGravityGun.draggedStone == stone)
        //    {
        //        die(DeathType.STONE_HIT);
        //        return true;
        //    }
        //}

        //float stoneSpeed = stoneBody.velocity.magnitude;
        ////		if (stoneSpeed > stoneDeadlySpeed) {
        ////			die (DeathType.STONE_HIT);
        ////			return true;
        ////} else 
        //if (stoneSpeed < stoneMinDeadySpeed)
        //{
        //    return false;
        //}

        //float stoneMass = stoneBody.mass;
        ////		if (stoneMass > stoneDeadlyMass) {
        ////			die(DeathType.STONE_HIT );
        ////			return true;
        ////		}
        //if (stoneMass < stoneMinDeadyMass)
        //{
        //    return false;
        //}

        //float stoneEnergy = stoneSpeed * stoneMass;
        //if (stoneEnergy > stoneDeadlyEnergy)
        //{
        //    die(DeathType.STONE_HIT);
        //    return true;
        //}

        return false;
    }

    //	void OnCollisionEnter2D	(Collider2D other){
    //		if (other.transform.gameObject.layer == layerIdGroundMoveableMask) { // to jest kamien 
    //			hitByStone( other.transform );
    //			return;
    //		}
    //	}
    //
    //	void OnCollisionStay2D(Collider2D other){
    //		if (other.transform.gameObject.layer == layerIdGroundMoveableMask) { // to jest kamien 
    //			hitByStone( other.transform );
    //			return;
    //		}
    //	}

    void OnTriggerStay2D(Collider2D other) {
		if (isDead ())
			return;

		int lid = other.transform.gameObject.layer;
		if (lid == LayerMask.NameToLayer("GroundMoveable") ) { // layerIdGroundMoveableMask) { // to jest kamien 
			if( hitByStone( other.transform ))
            {
                return;
            }
            else
            {
                //touchStone(other.transform);
            }
			return;
		}

		if (other.gameObject.tag == "Panther") {
			Panther panther = other.gameObject.GetComponent<Panther>();
			if( panther.attacking() ){
				if( !currentController.isDodging() ){
					die(DeathType.PANTHER);
				}
			}
			return;
		}
	}
    //void OnCollisionEnter2D(Collision2D c)
    //{
    //    print(c);
    //}

	void OnTriggerEnter2D(Collider2D other) {
		if (isDead ())
			return;

		if (currentController.triggerEnter (other))
			return;

		int lid = other.transform.gameObject.layer;
		int lid2 = LayerMask.NameToLayer ("GroundMoveable");
		if (lid == lid2 ) {// layerIdGroundMoveableMask) { // to jest kamien 
            if (hitByStone(other.transform))
            {
                return;
            }
            else
            {
                //touchStone(other.transform);
            }
            return;
		}

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

        if (other.gameObject.tag == "CheckPoint")
        {
            lastTouchedCheckPoint = other.GetComponent<CheckPoint>();
            // zatwierdzam wszystkie zdobyte kawalki mapy...
            collectedMapParts.Clear();
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
		if (other.gameObject.tag == "Panther") {
			Panther panther = other.gameObject.GetComponent<Panther>();
			if( panther.attacking() ){
				if( !currentController.isDodging() ){
					die(DeathType.PANTHER);
				}
			}
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
		if (other.tag == "Pickable") {
			//print("znalazlem.... pickabla " + other.name);
			Pickable pickable = other.GetComponent<Pickable>();
			if( pickable.isActive ){
				switch( pickable.type ){
				case Pickable.Type.KNIFE:
					if( !HaveKnife ){
						HaveKnife = true;
						zapControllerKnife.SetCtrlEnabled (HaveKnife);
						if( !HaveGravityGun )
							chooseController(zapControllerKnife);
					}
					break;

				case Pickable.Type.GRAVITY_GUN:
					if( !HaveGravityGun){
						HaveGravityGun = true;
						zapControllerGravityGun.SetCtrlEnabled (HaveGravityGun);
						if( !HaveKnife )
							chooseController(zapControllerGravityGun);
					}
					break;
				}
				pickable.deactivate();
			}
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
//		if (Input.GetKey (keyLeft)) {
//			if( Input.GetMouseButtonDown(0) ){
//				print("tzymajac w lewo -> mouse left wlasnie wdusiles");
//			}
//			if( Input.GetMouseButton(0) ){
//				print("tzymajac w lewo -> mouse left trzymasz");
//			}
//
//			return;
//		}
//		if (Input.GetKey (keyRight)) {
//			if( Input.GetMouseButtonDown(0) ){
//				print("tzymajac w prawo -> mouse left wlasnie wdusiles");
//			}
//			if( Input.GetMouseButton(0) ){
//				print("tzymajac w prawo -> mouse left trzymasz");
//			}
//			
//			return;
//		}
//		return;

		float timeSinceLastFrame = Time.deltaTime;
		//print ("Zap::Update() : " + timeSinceLastFrame);

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
		//
		if (Input.GetKeyDown (KeyCode.R)) {
			reborn();
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			//setPrevWeapon();
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			//setNextWeapon();
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			//print ("Q");
			
			if( choosenController == zapControllerKnife ){
				//print ("set choosen gravitygun");
				if( HaveGravityGun ){
					if( choosenController == currentController && choosenController.tryDeactiveate() ){
						chooseController(zapControllerGravityGun);
					}else{
						chooseController(zapControllerGravityGun);
					}
				}
			}else if( choosenController == zapControllerGravityGun ){
				//print ("set choosen knife");
				if( HaveKnife ){
					if( choosenController == currentController && choosenController.tryDeactiveate() ){
						chooseController(zapControllerKnife);
					}else{
						chooseController(zapControllerKnife);
					}
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			//print ("E");
			
			if( choosenController == zapControllerKnife ){
				//print ("set choosen gravitygun");
				// jezeli to jest aktywny kontroller:
				if( HaveGravityGun ){
					if( choosenController == currentController && choosenController.tryDeactiveate() ){
						chooseController(zapControllerGravityGun);
					}else{
						chooseController(zapControllerGravityGun);
					}
				}
			}else if( choosenController == zapControllerGravityGun ){
				//print ("set choosen knife");
				if( HaveKnife ){
					if( choosenController == currentController && choosenController.tryDeactiveate() ){
						chooseController(zapControllerKnife);
					}else{
						chooseController(zapControllerKnife);
					}
				}
			}
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

	void ZapUpdate (float deltaTime) {

        if (isDead())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                reborn();
                return;
            }
        }

        CurrentDeltaTime = deltaTime;
		
		SetImpulse(new Vector2(0.0f, 0.0f));

		stateJustChanged = false;
		currentStateTime += deltaTime;
		currentActionTime += deltaTime;

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



	

		currentController.MUpdate( CurrentDeltaTime );
		
		updateShadow ();
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

        scl = gfxLegs.localScale;
        scl.x = Mathf.Abs(scl.x) * -1.0f;
        gfxLegs.localScale = scl;
    }
	public void turnRight(){
		Vector3 scl = gfx.localScale;
		scl.x = Mathf.Abs(scl.x) * 1.0f;
		gfx.localScale = scl;

        scl = gfxLegs.localScale;
        scl.x = Mathf.Abs(scl.x) * 1.0f;
        gfxLegs.localScale = scl;
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
    public bool faceLeft()
    {
        return gfx.localScale.x < 0f;
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

    public float checkLeft(float checkingDist, bool flying = false)
    {
        if (!stateJustChanged)
        {
            if (flying)
            {
                hit = Physics2D.Raycast(sensorLeft1.position, -Vector2.right, checkingDist, layerIdGroundAllMask);
                if (hit.collider != null)
                {
                    return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
                }
            }
            else
            {
                //hit = Physics2D.Raycast(sensorDown2.position, -Vector2.right, checkingDist+0.5f, layerIdGroundAllMask);
                //if (hit.collider != null)
                //{
                //    print(hit.normal);
                //}

                int numRes = Physics2D.RaycastNonAlloc(sensorDown2.position, -Vector2.right, raycastHits, checkingDist+0.5f, layerIdGroundAllMask);
                for (int i = 0; i < numRes; ++i)
                {
                    //hit = raycastHits[i];
                    //if (hit.fraction == 0f) continue;
                    //float angle = Vector2.Angle(Vector2.up,hit.normal);
                    //if (Mathf.Abs(angle) > 45.0f)
                    //    return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
                    
                    hit = raycastHits[i];
                    if (hit.fraction == 0f) continue;
                    float angle = Vector2.Angle(Vector2.up, hit.normal);
                    if (Mathf.Abs(angle) > 45.0f)
                    {
                        Vector2 ro = sensorDown2.position;
                        ro.x -= (hit.distance+0.01f);
                        ro.y += 0.2f;
                        int numRes2 = Physics2D.RaycastNonAlloc(ro, -Vector2.up, raycastHits2, 0.2f + 0.1f, layerIdGroundAllMask);
                        for (int j = 0; j < numRes2; ++j)
                        {
                            hit2 = raycastHits2[j];
                            if (hit2.collider != hit.collider) continue;
                            if (hit2.fraction == 0f) return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
                            float angle2 = Vector2.Angle(Vector2.up, hit2.normal);
                            if (Mathf.Abs(angle2) > 45.0f) return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
                        }

                        //return Mathf.Abs(hit.point.x - sensorRight1.position.x);
                    }
                }
            }
        }
        hit = Physics2D.Raycast(sensorLeft2.position, -Vector2.right, checkingDist, layerIdGroundAllMask);
        if (hit.collider != null)
        {
            return Mathf.Abs(hit.point.x - sensorLeft2.position.x);
        }

        if (!flying && ( currentController.crouching() || currentController.isDodging() ))
            return -1.0f;

        hit = Physics2D.Raycast(sensorLeft3.position, -Vector2.right, checkingDist, layerIdGroundAllMask);
        if (hit.collider != null)
        {
            return Mathf.Abs(hit.point.x - sensorLeft3.position.x);
        }
        return -1f;
    }

    public float checkRight(float checkingDist, bool flying = false)
    {
        if (!stateJustChanged)
        {
            if (flying)
            {
                hit = Physics2D.Raycast(sensorRight1.position, Vector2.right, checkingDist, layerIdGroundAllMask);
                if (hit.collider != null)
                {
                    return Mathf.Abs(hit.point.x - sensorRight1.position.x);
                }
            }
            else
            {
                //Vector2 ro = sensorDown2.position;
                //ro.y += 0.1f;
                int numRes = Physics2D.RaycastNonAlloc(sensorDown2.position, Vector2.right, raycastHits, checkingDist+0.5f, layerIdGroundAllMask);
                for (int i = 0; i < numRes; ++i)
                {
                    //hit = raycastHits[i];
                    //float angle = hit.collider.transform.eulerAngles.z;
                    ////float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation);
                    //angle = angle % 90;
                    //if (angle < -45.0f || angle > 45.0f)
                    //    return Mathf.Abs(hit.point.x - sensorRight1.position.x);

                    hit = raycastHits[i];
                    if (hit.fraction == 0f) continue;
                    float angle = Vector2.Angle(Vector2.up, hit.normal);
                    if (Mathf.Abs(angle) > 45.0f)
                    {
                        Vector2 ro = sensorDown2.position;
                        ro.x += (hit.distance + 0.01f);
                        ro.y += 0.2f;
                        //bool hitObstacle = false;
                        int numRes2 = Physics2D.RaycastNonAlloc(ro, -Vector2.up, raycastHits2, 0.2f+0.1f, layerIdGroundAllMask);
                        for (int j = 0; j < numRes2; ++j)
                        {
                            hit2 = raycastHits2[j];
                            if (hit2.collider != hit.collider) continue;

                            if (hit2.fraction == 0f) return Mathf.Abs(hit.point.x - sensorRight1.position.x);
                            float angle2 = Vector2.Angle(Vector2.up, hit2.normal);
                            if (Mathf.Abs(angle2) > 45.0f) return Mathf.Abs(hit.point.x - sensorRight1.position.x);
                        }

                        //return Mathf.Abs(hit.point.x - sensorRight1.position.x);
                    }
                }
            }
        }
        hit = Physics2D.Raycast(sensorRight2.position, Vector2.right, checkingDist, layerIdGroundAllMask);
        if (hit.collider != null)
        {
            return Mathf.Abs(hit.point.x - sensorRight2.position.x);
        }

        if (!flying && (currentController.crouching() || currentController.isDodging()))
            return -1.0f;

        hit = Physics2D.Raycast(sensorRight3.position, Vector2.right, checkingDist, layerIdGroundAllMask);
        if (hit.collider != null)
        {
            return Mathf.Abs(hit.point.x - sensorRight3.position.x);
        }
        return -1f;
    }

    public float checkDown(float checkingDist){

		//int layerIdMask = layerIdGroundAllMask;
		Vector3 rayOrigin = sensorDown1.position;
        //RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundPermeableMask);
        //if (hit.collider) {// jesetem wewnatrz wskakiwalnej platformy ... nie moge sie zatrzymac..
        //	layerIdMask = layerIdGroundMask;
        //}

        rayOrigin = new Vector2( sensorDown1.position.x, sensorDown1.position.y );
        RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, layerIdGroundAllMask);
		if (hit.collider != null) {
			//layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
			return Mathf.Abs (hit.point.y - sensorDown1.position.y);
		} else {
			rayOrigin = new Vector2( sensorDown2.position.x, sensorDown2.position.y );
			hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, layerIdGroundAllMask);
			if (hit.collider != null){
				//layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
				return Mathf.Abs (hit.point.y - sensorDown2.position.y);
			} else {
				rayOrigin = new Vector2( sensorDown3.position.x, sensorDown3.position.y );
				hit = Physics2D.Raycast (rayOrigin, -Vector2.up, checkingDist, layerIdGroundAllMask);
				if (hit.collider != null){
					//layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
					return Mathf.Abs (hit.point.y - sensorDown3.position.y);
				} else {
					return -1.0f;
				}
			}
		}  
	}

	public void checkGround (ref float distToGround){
		Transform groundUnderFeet = null;
        groundUnderAngle = 0f;

		float th = 0.9f;
		float checkingDist = th + 0.1f;
		//if (fromFeet)
		//	checkingDist = 0.5f;

		Vector2 rayOrigin1 = sensorDown1.position;
		//if( !fromFeet )
		rayOrigin1.y += th;
		RaycastHit2D hit1 = Physics2D.Raycast (rayOrigin1, -Vector2.up, checkingDist, layerIdGroundAllMask);

		Vector2 rayOrigin2 = sensorDown2.position;
		//if( !fromFeet )
		rayOrigin2.y += th;
		RaycastHit2D hit2 = Physics2D.Raycast (rayOrigin2, -Vector2.up, checkingDist, layerIdGroundAllMask);

		Vector2 rayOrigin3 = sensorDown3.position;
		//if( !fromFeet )
		rayOrigin3.y += th;
		RaycastHit2D hit3 = Physics2D.Raycast (rayOrigin3, -Vector2.up, checkingDist, layerIdGroundAllMask);

        //int closestSensor = 0;
        RaycastHit2D closestHit = hit1;

        if (hit2.collider != null)
        {
            if (closestHit.collider == null)
            {
                closestHit = hit2;
            }
            else
            {
               // if( hit2.distance < hit1.distance)
            }
        }


        float dist1;
		float dist2;
		float dist3;

		if (hit1.collider != null) {
			dist1 = rayOrigin1.y - hit1.point.y;
			groundUnderFeet = hit1.collider.transform;
			distToGround = dist1;
            groundUnderAngle = Vector2.Angle(Vector2.up, hit.normal);
            //layerIdLastGroundTypeTouchedMask = 1 << hit1.collider.transform.gameObject.layer;
        }
		if (hit2.collider != null) {
			dist2 = rayOrigin2.y - hit2.point.y;
			if( groundUnderFeet ){
				if( distToGround > dist2)
                    distToGround = dist2;
			}else{
				groundUnderFeet = hit2.collider.transform;
				distToGround = dist2;
				//layerIdLastGroundTypeTouchedMask = 1 << hit2.collider.transform.gameObject.layer;
			}
		}
		if (hit3.collider != null) {
			dist3 = rayOrigin3.y - hit3.point.y;
			if( groundUnderFeet ){
				if( distToGround > dist3) distToGround = dist3;
			}else{
				groundUnderFeet = hit3.collider.transform;
				distToGround = dist3;
				//layerIdLastGroundTypeTouchedMask = 1 << hit3.collider.transform.gameObject.layer;
			}
		}

		if (groundUnderFeet) {
			//if( !fromFeet )
			distToGround = th - distToGround;
		}

		groundUnder = groundUnderFeet;
	}

    public bool checkCeil(ref float distToFly)
    {
        return false;
        //Transform groundUnderFeet = null;
        //groundUnderAngle = 0f;

        //float th = 0.9f;
        //float checkingDist = th + 0.1f;
        ////if (fromFeet)
        ////	checkingDist = 0.5f;

        //Vector2 rayOrigin1 = sensorDown1.position;
        ////if( !fromFeet )
        //rayOrigin1.y += th;
        //RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin1, -Vector2.up, checkingDist, layerIdGroundAllMask);

        //Vector2 rayOrigin2 = sensorDown2.position;
        ////if( !fromFeet )
        //rayOrigin2.y += th;
        //RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, -Vector2.up, checkingDist, layerIdGroundAllMask);

        //Vector2 rayOrigin3 = sensorDown3.position;
        ////if( !fromFeet )
        //rayOrigin3.y += th;
        //RaycastHit2D hit3 = Physics2D.Raycast(rayOrigin3, -Vector2.up, checkingDist, layerIdGroundAllMask);

        ////int closestSensor = 0;
        //RaycastHit2D closestHit = hit1;

        //if (hit2.collider != null)
        //{
        //    if (closestHit.collider == null)
        //    {
        //        closestHit = hit2;
        //    }
        //    else
        //    {
        //        // if( hit2.distance < hit1.distance)
        //    }
        //}


        //float dist1;
        //float dist2;
        //float dist3;

        //if (hit1.collider != null)
        //{
        //    dist1 = rayOrigin1.y - hit1.point.y;
        //    groundUnderFeet = hit1.collider.transform;
        //    distToGround = dist1;
        //    groundUnderAngle = Vector2.Angle(Vector2.up, hit.normal);
        //    //layerIdLastGroundTypeTouchedMask = 1 << hit1.collider.transform.gameObject.layer;
        //}
        //if (hit2.collider != null)
        //{
        //    dist2 = rayOrigin2.y - hit2.point.y;
        //    if (groundUnderFeet)
        //    {
        //        if (distToGround > dist2)
        //            distToGround = dist2;
        //    }
        //    else
        //    {
        //        groundUnderFeet = hit2.collider.transform;
        //        distToGround = dist2;
        //        //layerIdLastGroundTypeTouchedMask = 1 << hit2.collider.transform.gameObject.layer;
        //    }
        //}
        //if (hit3.collider != null)
        //{
        //    dist3 = rayOrigin3.y - hit3.point.y;
        //    if (groundUnderFeet)
        //    {
        //        if (distToGround > dist3) distToGround = dist3;
        //    }
        //    else
        //    {
        //        groundUnderFeet = hit3.collider.transform;
        //        distToGround = dist3;
        //        //layerIdLastGroundTypeTouchedMask = 1 << hit3.collider.transform.gameObject.layer;
        //    }
        //}

        //if (groundUnderFeet)
        //{
        //    //if( !fromFeet )
        //    distToGround = th - distToGround;
        //}

        //groundUnder = groundUnderFeet;
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




	public void SetImpulse(Vector2 imp) { impulse = imp; }
	public Vector2 GetImpulse() { return impulse; }
	public void AddImpulse(Vector3 imp) { impulse += imp; }
	public void AddImpulse(Vector2 imp) { 
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
			animatorBody.Play("Zap_climbmove_up");
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

	

	[HideInInspector]
	public Transform sensorLeft1;
	[HideInInspector]
	public Transform sensorLeft2;
	[HideInInspector]
	Transform sensorLeft3;
	[HideInInspector]
	Transform sensorRight1;
	[HideInInspector]
	public Transform sensorRight2;
	[HideInInspector]
	public Transform sensorRight3;
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

	[HideInInspector]
	public Transform leftKnifeHitPointHigh1;
	[HideInInspector]
	public Transform leftKnifeHitPointHigh2;
	[HideInInspector]
	public Transform rightKnifeHitPointHigh1;
	[HideInInspector]
	public Transform rightKnifeHitPointHigh2;
	[HideInInspector]
	public Transform leftKnifeHitPointLow1;
	[HideInInspector]
	public Transform leftKnifeHitPointLow2;
	[HideInInspector]
	public Transform rightKnifeHitPointLow1;
	[HideInInspector]
	public Transform rightKnifeHitPointLow2;

	Transform cameraTarget;
	Transform gfx;
    Transform gfxLegs;
    Transform targeter;
    Transform gravityGunBeam;
    //SpriteRenderer 
    PolygonCollider2D gfxCollider;

    public Transform GravityGunBeam
    {
        get
        {
            return gravityGunBeam;
        }
    }

    public Transform GfxLegs
    {
        get
        {
            return gfxLegs;
        }
    }
    public Transform Targeter
    {
        get
        {
            return targeter;
        }
    }

    SpriteRenderer sprRend = null;
    BoxCollider2D coll;
    //public Animator AnimatorBody
    //{
    //    return animator;
    //}

    Animator animatorBody;
    Animator animatorLegs;

    public Animator AnimatorLegs
    {
        get
        {
            return animatorLegs;
        }
    }
    public Animator AnimatorBody
    {
        get
        {
            return animatorBody;
        }
    }

    [HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Vector3 lastVelocity;

	Vector3 lastSwingPos;
	//[SerializeField]
	Vector3 impulse;
	[HideInInspector]
	public Vector3 startFallPos;


	float desiredSpeedX = 0.0f;

	[HideInInspector]
	public float currentActionTime = 0f;
//	public float getCurrentActionTime() {
//		return currentActionTime;
//	}
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
	[HideInInspector]
	public int layerIdGroundMoveableMask;
	[HideInInspector]
	public int layerIdGroundAllMask;
	[HideInInspector]
	public int layerIdGroundHandlesMask;
	[HideInInspector]
	public int layerIdRopesMask;
	int layerIdMountMask;

	float climbDistFromWall;
	Vector2 climbDir;
	
	bool gamePaused = false;
    
	int playerCurrentLayer;

	private State state;
    RaycastHit2D[] raycastHits = new RaycastHit2D[10];
    RaycastHit2D[] raycastHits2 = new RaycastHit2D[10];
    RaycastHit2D hit;
    RaycastHit2D hit2;

    public Transform groundUnder = null;
    public float groundUnderAngle = 0f;
}
