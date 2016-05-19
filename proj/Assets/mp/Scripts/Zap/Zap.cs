using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Zap : MonoBehaviour, IAnimationCallbackReceiver
{
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

    //public AudioClip ropeCatchSound;


    //public AudioClip[] jumpSounds;
    //public AudioClip[] landingSounds;
    //public AudioClip[] turnRunSounds;
    //public AudioClip[] catchSounds;
    //public AudioClip[] dieSounds;

    //public AudioClip landingSound;
    
    public AudioClip ropeBreakOffSound = null;
    public AudioClip ropeCutSound = null;

    public ParticleSet particleSet = null;

    AudioSource myAudioSource = null;
    AudioSource myAudioSourceLooped = null;

    bool paused = false;
    float beforePauseAnimatorSpeed = 1f;
    public void Pause()
    {
        if (paused) return;

        paused = true;
        beforePauseAnimatorSpeed = animatorBody.speed;
        animatorBody.speed = 0f;

        if (Input.GetKey(keyRun))
        {
            currentController.keyRunUp();
        }

        if (Input.GetKey(keyUp))
        {
            currentController.keyUpUp();
        }
        if (Input.GetKey(keyDown))
        {
            currentController.keyDownUp();
        }

        if (Input.GetKey(keyJump))
        {
            jumpKeyPressed = false;
            currentController.keyJumpUp();
        }

        //if (Input.GetKeyDown(keyLeft))
        //{
        //    currentController.keyLeftDown();
        //}
        //if (Input.GetKeyDown(keyRight))
        //{
        //    currentController.keyRightDown();
        //}

        if (Input.GetKey(keyLeft))
        {
            currentController.keyLeftUp();
        }
        if (Input.GetKey(keyRight))
        {
            currentController.keyRightUp();
        }

        //if (Input.GetKeyDown(keyRun))
        //{
        //    currentController.keyRunDown();
        //}
        //else 
        

        //if (!userJumpKeyPressed)
        //{
        //    if (Input.GetKeyDown(keyJump))
        //    {
        //        if (Input.GetKey(keyLeft) || Input.GetKey(keyRight))
        //        {
        //            jumpKeyPressed = true;
        //            currentController.keyJumpDown();
        //        }
        //        else
        //        {
        //            timeFromJumpKeyPressed = 0.0f;
        //            userJumpKeyPressed = true;
        //        }
        //    }
        //}
        //else
        //{
        //    timeFromJumpKeyPressed += deltaTime;
        //    if (timeFromJumpKeyPressed >= 0.06f)
        //    {
        //        timeFromJumpKeyPressed = 0.0f;
        //        userJumpKeyPressed = false;
        //        jumpKeyPressed = true;

        //        currentController.keyJumpDown();
        //    }
        //}

        //if (currentController != zapControllerGravityGun && zapControllerGravityGun == choosenController)
        //{
        //    zapControllerGravityGun.leftMouseNotPressed();
        //}
    }
    public void Unpause()
    {
        if (!paused) return;

        animatorBody.speed = beforePauseAnimatorSpeed;
        paused = false;
    }


    public void SetAnimatorCallbackTarget()
    {
        AnimationCallback[] acs = animatorBody.GetBehaviours<AnimationCallback>();
        for (int b = 0; b < acs.Length; ++b)
        {
            acs[b].callbackTarget = this;
        }
    }

    public void NewsFromAnimator(AnimationCallbackData acd)
    {
        switch (acd.Type)
        {
            case AnimationCallback.Type.PlaySound:
                //print("NewsFromAnimator : " + acd.Type + " " + acd.Message);
                SoundPlayer.Play(gameObject, acd.Message);
                break;

            case AnimationCallback.Type.PlaySoundSurface:
                //print("NewsFromAnimator : " + acd.Type + " " + acd.Message);
                Surface surface = groundUnder.GetComponent<Surface>();
                if (surface)
                {
                    //print(acd.Type + " " + acd.Message + "*" + surface.type);
                    if (!SoundPlayer.Play(gameObject, acd.Message + "*" + surface.type))
                    {
                        //print("PROBUJE odtworzyc : " + acd.Message);
                        SoundPlayer.Play(gameObject, acd.Message);
                    }
                }
                else
                {
                    SoundPlayer.Play(gameObject, acd.Message);
                }
                break;
        }
    }

    //public void NewsFromAnimator(AnimationCallback.Type type, string message)
    //{
    //    switch(type)
    //    {
    //        case AnimationCallback.Type.PlaySound:

    //            print("NewsFromAnimator : " + type + " " + message);
    //            SoundPlayer.Play(gameObject, message + groundUnder.name);
    //            break;
    //    }
    //}

    public bool playSound(AudioClip clip, bool forceStart = true)
    {
        if (!clip || (myAudioSource.isPlaying && !forceStart)) return false;
        //myAudioSource.pitch = Random.Range(0.5f, 1.5f);
        //myAudioSource.volume = Random.Range(0.25f, 1.0f);
        myAudioSource.PlayOneShot(clip);
        return true;
    }
    public bool playSoundLooped(AudioClip clip, bool forceStart = true)
    {
        if (!clip || (myAudioSourceLooped.isPlaying && !forceStart)) return false;
        myAudioSourceLooped.clip = clip;
        myAudioSourceLooped.Play();
        return true;
    }
    public bool stopSoundLooped(AudioClip clip = null)
    {
        if( clip )
        {
            if( myAudioSourceLooped.clip == clip )
            {
                myAudioSourceLooped.Stop();
                return true;
            }
            return false;
        }
        else
        {
            myAudioSourceLooped.Stop();
            return true;
        }
        return true;
    }

    //[HideInInspector]
    //public Camera mainCamera;
    //[HideInInspector]
    //public Camera touchCamera;

    public void setTouchCamera(Camera newTC)
    {
        if (zapControllerNormal)
            zapControllerNormal.setTouchCamera(newTC);
        if (zapControllerKnife)
            zapControllerKnife.setTouchCamera(newTC);
        if (zapControllerGravityGun)
            zapControllerGravityGun.setTouchCamera(newTC);
        if (zapControllerSuckByBat)
            zapControllerSuckByBat.setTouchCamera(newTC);
    }

    public bool autoCatchEdges = false;
    public bool canJumpBackFromRope = true;
    //public float maxHandStoneTilt = 5f;

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
    public ZapControllerSuckedByBat zapControllerSuckByBat;

    public bool HaveKnife = false;
    public bool HaveGravityGun = false;

    bool _haveKnife = false;
    bool _haveGravityGun = false;

    void Awake()
    {
        climbingWallID = -1;

        guiCanvas = FindObjectOfType<Canvas>();

        if (guiCanvas)
        {
            weaponMenu = guiCanvas.GetComponent<WeaponMenu>();

            infoLabel = FindObjectOfType<Text>();
            infoLabel.text = "";

            Image[] allImages = FindObjectsOfType<Image>();
            for (int i = 0; i < allImages.Length; ++i)
            {
                Image img = allImages[i];
                if (img.gameObject.name == "mapBackgroundImage")
                {
                    mapBackgroundImage = img;
                    Color newColor = new Color(1f, 1f, 1f, 0f);
                    mapBackgroundImage.color = newColor;
                    continue;
                }

                ComicPagePart cpp = img.GetComponent<ComicPagePart>();
                if (cpp)
                {
                    mapPartParts[cpp.partID] = cpp;
                }
            }
        }
        
        coll = GetComponent<BoxCollider2D>();
        gfx = transform.Find("gfx").transform;
        gfxCollider = gfx.GetComponent<PolygonCollider2D>();
        gfxLegs = transform.Find("gfxLegs").transform;

        animatorBody = transform.Find("gfx").GetComponent<Animator>();
        sprRend = gfx.GetComponent<SpriteRenderer>();
        animatorLegs = transform.Find("gfxLegs").GetComponent<Animator>();

        shadowCenter = transform.Find("shadowCenter");

        mountAttackLeftCollider = transform.Find("mountAttackLeftCollider").gameObject;
        mountAttackRightCollider = transform.Find("mountAttackRightCollider").gameObject;
        mountAttackLeftCollider.SetActive(false);
        mountAttackRightCollider.SetActive(false);

        knifeAttackLeftHighCollider = transform.Find("knifeAttackLeftHighCollider").gameObject;
        knifeAttackRightHighCollider = transform.Find("knifeAttackRightHighCollider").gameObject;
        knifeAttackLeftHighCollider.SetActive(false);
        knifeAttackRightHighCollider.SetActive(false);

        knifeAttackLeftLowCollider = transform.Find("knifeAttackLeftLowCollider").gameObject;
        knifeAttackRightLowCollider = transform.Find("knifeAttackRightLowCollider").gameObject;
        knifeAttackLeftLowCollider.SetActive(false);
        knifeAttackRightLowCollider.SetActive(false);

        targeter = transform.Find("targeter");
        gravityGunBeam = transform.Find("GravityGunBeam");
        gravityGunBeam.GetComponent<LineRenderer>().sortingLayerName = "WaterFront";
        gravityGunBeam.GetComponent<LineRenderer>().sortingOrder = 10;

        if (shadowCenter)
        {
            shadowLeft = shadowCenter.Find("shadowLeft");
            shadowRight = shadowCenter.Find("shadowRight");

            shadowCenterSR = shadowCenter.GetComponent<SpriteRenderer>();
            shadowLeftSR = shadowLeft.GetComponent<SpriteRenderer>();
            shadowRightSR = shadowRight.GetComponent<SpriteRenderer>();
        }

        //zap_idle1_beh[] behs = animatorBody.GetBehaviours<zap_idle1_beh>();
        //for (int b = 0; b < behs.Length; ++b)
        //{
        //    behs[b].playerController = this;
        //}

        AudioSource[] audios = GetComponents<AudioSource>();
        if (audios.Length == 2)
        {
            if( audios[0].loop )
            {
                myAudioSourceLooped = audios[0];
                myAudioSource = audios[1];
                if(myAudioSource.loop )
                {
                    Debug.LogError("ZAP => nie ma nielooped AudioSource'a");
                }
            }
            else
            {
                myAudioSource = audios[0];
                myAudioSourceLooped = audios[1];
                if (!myAudioSourceLooped.loop)
                {
                    Debug.LogError("ZAP => nie ma looped AudioSource'a");
                }
            }
            //myAudio = GetComponent<AudioSource>(); asdf asdf
            //Debug.LogError("raf error");
        }


        //PlaySounds[] pss = animatorBody.GetBehaviours<PlaySounds>();
        //for (int b = 0; b < pss.Length; ++b)
        //{
        //    pss[b].zap = this;
        //}
        //AnimationCallback[] acs = animatorBody.GetBehaviours<AnimationCallback>();
        //for (int b = 0; b < acs.Length; ++b)
        //{
        //    acs[b].callbackTarget = this;
        //}
        //SetAnimatorCallbackTarget();

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

        handlerLeft = transform.Find("handlerLeft").transform;
        handlerRight = transform.Find("handlerRight").transform;

        BoxCollider2D hlcoll = handlerLeft.GetComponent<BoxCollider2D>();
        //handlerLeftSize = hlcoll.size * hlcoll.transform.lossyScale;
        handlerLeftSize = hlcoll.size;
        BoxCollider2D hrcoll = handlerRight.GetComponent<BoxCollider2D>();
        handlerRightSize = hrcoll.size;
        
        handlerBellyLeft = transform.Find("handlerBellyLeft").transform;
        handlerBellyRight = transform.Find("handlerBellyRight").transform;

        BoxCollider2D hblcoll = handlerBellyLeft.GetComponent<BoxCollider2D>();
        handlerBellyLeftSize = hblcoll.size;
        BoxCollider2D hbrcoll = handlerBellyRight.GetComponent<BoxCollider2D>();
        handlerBellyRightSize = hbrcoll.size;

        leftKnifeHitPointHigh1 = transform.Find("leftKnifeHitPointHigh1").transform;
        leftKnifeHitPointHigh2 = transform.Find("leftKnifeHitPointHigh2").transform;
        rightKnifeHitPointHigh1 = transform.Find("rightKnifeHitPointHigh1").transform;
        rightKnifeHitPointHigh2 = transform.Find("rightKnifeHitPointHigh2").transform;
        leftKnifeHitPointLow1 = transform.Find("leftKnifeHitPointLow1").transform;
        leftKnifeHitPointLow2 = transform.Find("leftKnifeHitPointLow2").transform;
        rightKnifeHitPointLow1 = transform.Find("rightKnifeHitPointLow1").transform;
        rightKnifeHitPointLow2 = transform.Find("rightKnifeHitPointLow2").transform;

        cameraTarget = transform.Find("cameraTarget").transform;
        //cameraTargetBasePos = cameraTarget.localPosition;

        layerIdGroundMask = 1 << LayerMask.NameToLayer("Ground");
        layerIdGroundFarMask = 1 << LayerMask.NameToLayer("GroundFar");
        //layerIdGroundPermeableMask = 1 << LayerMask.NameToLayer("GroundPermeable");
        layerIdGroundMoveableMask = 1 << LayerMask.NameToLayer("GroundMoveable");
        layerIdDraggedGroundMoveableMask = 1 << LayerMask.NameToLayer("DraggedGroundMoveable");

        layerIdGroundAllMask = layerIdGroundMask | layerIdGroundMoveableMask | layerIdDraggedGroundMoveableMask; // | layerIdGroundFarMask;
        layerIdGroundAllFullMask = layerIdGroundMask | layerIdGroundMoveableMask | layerIdDraggedGroundMoveableMask | layerIdGroundFarMask;
        //layerIdLastGroundTypeTouchedMask = layerIdGroundMask;

        layerIdGroundHandlesMask = 1 << LayerMask.NameToLayer("GroundHandles");

        layerIdMountMask = 1 << LayerMask.NameToLayer("Mount");
        layerIdRopesMask = 1 << LayerMask.NameToLayer("Ropes");

        myWidth = Coll.size.x;
        myHalfWidth = myWidth * 0.5f;
        //myHeight = coll.size.y;
        //myHalfHeight = myHeight * 0.5f;

        //		zapControllerNormal = new ZapControllerNormal(this);
        //		zapControllerKnife = new ZapControllerKnife(this);

        //zapControllerNormal = new ZapControllerNormal();
        //zapControllerKnife = new ZapControllerKnife();

        zapControllerNormal.setZap(this);
        zapControllerKnife.setZap(this);
        zapControllerGravityGun.setZap(this);
        zapControllerSuckByBat.setZap(this);

        RLHScene[] rlhScenes = FindObjectsOfType(typeof(RLHScene)) as RLHScene[];
        if (rlhScenes.Length == 1)
        {
            rlhScene = rlhScenes[0];
        }

        //SoundSets[] soundPlays = FindObjectsOfType(typeof(SoundSets)) as SoundSets[];
        //foreach (SoundSets sp in soundPlays)
        //{
        //    sp.GenerateHashes();
        //}

        
    }

    GroundMoveable[] allStones;
    Crate[] allCrates;
    CutSceneCameraPassing[] allCameraPassings;

    void Start()
    {
        //currentController = zapControllerNormal;
        //currentController = zapControllerKnife;


        //		if (!HaveKnife) {
        //			//weaponMenu.itemKnife.hide();
        //			zapControllerKnife.SetCtrlEnabled(
        //		}

        paused = false;

        if (rlhScene.onlyKnife)
        {
            HaveKnife = true;
            HaveGravityGun = false;

            _haveKnife = HaveKnife;
            _haveGravityGun = HaveGravityGun;

            chooseController(zapControllerKnife);

            zapControllerKnife.SetCtrlEnabled(false);
            zapControllerGravityGun.SetCtrlEnabled(false);
            zapControllerSuckByBat.SetCtrlEnabled(false);

            setCurrentController(zapControllerKnife);
        }
        else
        {
            _haveKnife = HaveKnife;
            _haveGravityGun = HaveGravityGun;
            if (HaveKnife)
            {
                chooseController(zapControllerKnife);
            }
            else if (HaveGravityGun)
            {
                chooseController(zapControllerGravityGun);
            }

            zapControllerKnife.SetCtrlEnabled(HaveKnife);
            zapControllerGravityGun.SetCtrlEnabled(HaveGravityGun);
            zapControllerSuckByBat.SetCtrlEnabled(false);

            setCurrentController(zapControllerNormal);
        }
        
        velocity = new Vector3(0, 0, 0);
        impulse = new Vector3(0, 0, 0);

        startFallPos = transform.position;

        setState(State.ON_GROUND);
        currentController.activate();

        lastTouchedCheckPoint = null;

        startPoint = transform.position;
        beforeFallController = null;

        allStones = FindObjectsOfType(typeof(GroundMoveable)) as GroundMoveable[];
        //print(allStones.Length);

        //zapControllerSuckByBat.resetSequenceKeys();

        //IGResetable[] resetables = FindObjectsOfType(typeof(IGResetable)) as IGResetable[];
        //foreach (IGResetable resetable in resetables)
        //{
        //    resetable.GCacheResetData();
        //}

        //allCrates = FindObjectsOfType(typeof(Crate)) as Crate[];
        //foreach (Crate resetable in allCrates)
        //{
        //    resetable.GResetCacheResetData();
        //}
        //allCameraPassings = FindObjectsOfType(typeof(CutSceneCameraPassing)) as CutSceneCameraPassing[];
        //foreach (CutSceneCameraPassing cameraPassing in allCameraPassings)
        //{
        //    cameraPassing.GResetCacheResetData();
        //}
        rlhScene.CacheAllResetableDatas();
    }

    public void chooseController(ZapController newController)
    {
        if (choosenController != null)
            choosenController.deselected();
        choosenController = newController;
        choosenController.selected();
    }

    public void setCurrentController(ZapController newController, bool restore = false, bool crouch = false)
    {
        if (currentController != null)
            currentController.deactivate();
        currentController = newController;
        currentController.activate(restore, crouch);
    }

    public void _pullOutKnife()
    {
        setCurrentController(zapControllerKnife);
    }
    public void _hideKnife()
    {
        setCurrentController(zapControllerNormal);
    }
    public void _pullOutGravityGun()
    {
        setCurrentController(zapControllerGravityGun);
    }
    public void _hideGravityGun()
    {
        setCurrentController(zapControllerNormal);
    }

    public int pullChoosenWeapon(bool crouch = false)
    {
        if (choosenController)
        {
            setCurrentController(choosenController, false, crouch);
            return 1;
        }
        return 0;
    }
    public void hideChoosenWeapon()
    {
        //if (choosenController) {
        //	setCurrentController(choosenController);
        //}
        setCurrentController(zapControllerNormal);
    }
    public void suddenlyInAir()
    {
        if (currentController != zapControllerNormal)
        {
            beforeFallController = currentController;
            setCurrentController(zapControllerNormal);
            setState(Zap.State.IN_AIR);
            zapControllerNormal.suddenlyInAir();
        }
    }
    public void restoreBeforeFallController()
    {
        if (beforeFallController != null)
        {
            setCurrentController(beforeFallController, true);
            beforeFallController = null;
        }
    }

    //public AudioSource getAudioSource()
    //{
    //    return myAudio;
    //}

    public Transform getCameraTarget()
    {
        return cameraTarget;
    }

    public void StateIdleExit()
    {
    }
    public void StateIdleUpdate(float normTime)
    {
    }

    public int IdleAnimFreq = 10;

    public void StateIdleFinish(int stateIdleNum)
    {
        switch (stateIdleNum)
        {
            case 1:
            case 2:
                if (faceRight()) animatorBody.Play("Zap_idle_R");
                else animatorBody.Play("Zap_idle_L");
                break;
            case 0:
                //if (faceRight()) animatorBody.Play("Zap_idle_variation_2_R");
                //else animatorBody.Play("Zap_idle_variation_2_L");

                //if (faceRight()) animatorBody.Play("Zap_idle_variation_1_R");
                //else animatorBody.Play("Zap_idle_variation_1_L");

                if (IdleAnimFreq >= 3)
                {
                    int r = Random.Range(0, IdleAnimFreq);
                    if (r == 0)
                    {
                        if (faceRight()) animatorBody.Play("Zap_idle_variation_1_R");
                        else animatorBody.Play("Zap_idle_variation_1_L");
                    }
                    else if (r == 1)
                    {
                        if (faceRight()) animatorBody.Play("Zap_idle_variation_2_R");
                        else animatorBody.Play("Zap_idle_variation_2_L");
                    }
                }
                break;
        }
    }

    public enum DeathType
    {
        VERY_HARD_LANDING = 1,
        SNAKE,
        CROCODILE,
        PANTHER,
        POISON,
        STONE_HIT,
        DRAGGED_STONE_HIT
    };

    public string DeathByVeryHardLandingText = "rozjeb... sie o skale. press space";
    public string DeathBySnakeText = "pokasal cie waz. press space";
    public string DeathByCrocodileText = "zjadl cie krokodyl. press space";
    public string DeathByPantherText = "zjadla cie pantera. press space";
    public string DeathByPoisonText = "zatrules sie. press space";
    public string DeathByStoneHitText = "pierdolnela cie skala. press space";
    public string DeathByDraggedStoneHitText = "porazila cie energia z przenoszonej skaly.";
    public string DeathByDefaultText = "zginales defaultowa smiercia. press space";

    public void die(DeathType deathType)
    {
        climbingWallID = -1;

        if (RlhScene.ZapVsBats)
        {
            RlhScene.ZapVsBats.ZapDead();
        }

        if( suckedMeBat )
        {
            suckedMeBat.StopSuckZap();
            suckedMeBat = null;
        }

        //zapControllerSuckByBat.resetSequenceKeys();

        if ( lastTouchedCheckPointCandidate != null )
        {
            lastTouchedCheckPointCandidate = null;
        }

        velocity.x = 0.0f;
        velocity.y = 0.0f;
        removeLastIgnoredCollision();
        gfxCollider.enabled = false;

        currentController.zapDie(deathType);
        setState(State.DEAD);

        foreach (GameObject mapPart in collectedMapParts)
        {

            ComicPage comicPart = mapPart.GetComponent<ComicPage>();
            int mapPartID = comicPart.partID;

            mapPartParts[mapPartID].collected = false;

            mapPart.GetComponent<SpriteRenderer>().enabled = true;
        }

        collectedMapParts.Clear();
    }

    public bool isDead()
    {
        //return action == Action.DIE && state == State.OTHER;
        return isInState(State.DEAD);
    }

    public void reborn()
    {
        paused = false;

        climbingWallID = -1;

        if (RlhScene.ZapVsBats)
        {
            RlhScene.ZapVsBats.ZapReborn();
        }

        rlhScene.CamController.ShakeStop(-1f);
        RLHScene.Instance.CamController.StartFollow();
        cameraTargetOffset = new Vector3(0f, 0f, 0f);

        //Parallaxed[] pxs = FindObjectsOfType(typeof(Parallaxed)) as Parallaxed[];
        //foreach (Parallaxed px in pxs)
        //{
        //    if (px.enabled) px.reset();
        //}
        myEnergy = myMaxEnergy;
        //zapControllerSuckByBat.resetSequenceKeys();

        removeLastIgnoredCollision();

        sprRend.enabled = true;
        gfxCollider.enabled = true;
        velocity.x = 0.0f;
        velocity.y = 0.0f;
        //setAction (Action.IDLE);
        
        currentController.beforeReborn();

        if (rlhScene.onlyKnife)
        {
            HaveKnife = true;
            HaveGravityGun = false;

            chooseController(zapControllerKnife);
            
            zapControllerKnife.SetCtrlEnabled(false);
            zapControllerGravityGun.SetCtrlEnabled(false);

            currentController = zapControllerKnife;
            //setCurrentController(zapControllerKnife);
        }
        else
        {
            HaveKnife = _haveKnife;
            HaveGravityGun = _haveGravityGun;
            if (HaveKnife && choosenController == zapControllerKnife)
            {
                chooseController(zapControllerKnife);
            }
            else if (HaveGravityGun && choosenController == zapControllerGravityGun)
            {
                chooseController(zapControllerGravityGun);
            }
            else
            {
                choosenController = null;
            }

            zapControllerKnife.SetCtrlEnabled(HaveKnife);
            zapControllerGravityGun.SetCtrlEnabled(HaveGravityGun);

            currentController = zapControllerNormal;
        }

        setState(State.ON_GROUND);
        currentController.activate();

        if (lastTouchedCheckPoint)
        {
            transform.position = lastTouchedCheckPoint.transform.position;
        }
        else
        {
            transform.position = startPoint;// respawnPoint.position;
        }

        //Camera2DFollow[] c2dfs = FindObjectsOfType(typeof(Camera2DFollow)) as Camera2DFollow[];
        //if (c2dfs.Length == 1)
        //{
        //    c2dfs[0].resetPosToTarget();
        //}

        currentController.reborn();

        resetInfo();

        rlhScene.reset();

        foreach (GroundMoveable gm in allStones)
        {
            gm.Reset();
        }

        //IResetable[] resetables = FindObjectsOfType(typeof(IResetable)) as IResetable[];
        //foreach (IResetable resetable in resetables)
        //{
        //    resetable.IReset();
        //}

        //Rat[] rats = FindObjectsOfType(typeof(Rat)) as Rat[];
        //foreach (Rat rat in rats)
        //{
        //    rat.Reset();
        //}

        //Bat[] bats = FindObjectsOfType(typeof(Bat)) as Bat[];
        //foreach (Bat bat in bats)
        //{
        //    bat.Reset();
        //}

        Enemy[] enemies = FindObjectsOfType(typeof(Enemy)) as Enemy[];
        foreach (Enemy enemy in enemies)
        {
            enemy.Reset();
        }

        EnemiesSpawner[] spawners = FindObjectsOfType(typeof(EnemiesSpawner)) as EnemiesSpawner[];
        foreach (EnemiesSpawner spawner in spawners)
        {
            spawner.Reset();
        }
        //RlhScene.ResetRats();

        Snake[] snakes = FindObjectsOfType(typeof(Snake)) as Snake[];
        foreach (Snake snake in snakes)
        {
            snake.reset();
        }

        ShowInfoTrigger[] sits = FindObjectsOfType(typeof(ShowInfoTrigger)) as ShowInfoTrigger[];
        foreach (ShowInfoTrigger sit in sits)
        {
            sit.reset();
        }

        Pickable[] pickables = FindObjectsOfType(typeof(Pickable)) as Pickable[];
        foreach (Pickable pickable in pickables)
        {
            pickable.reset();
        }
        SmashStoneActivator[] smashStoneActivators = FindObjectsOfType(typeof(SmashStoneActivator)) as SmashStoneActivator[];
        foreach (SmashStoneActivator smashStoneActivator in smashStoneActivators)
        {
            smashStoneActivator.reset();
        }

        NewRope[] ropes = FindObjectsOfType(typeof(NewRope)) as NewRope[];
        foreach (NewRope rope in ropes)
        {
            rope.reset();
        }
        CutableBush[] bushes = FindObjectsOfType(typeof(CutableBush)) as CutableBush[];
        foreach (CutableBush bush in bushes)
        {
            bush.reset();
        }
        Panther[] panthers = FindObjectsOfType(typeof(Panther)) as Panther[];
        foreach (Panther panther in panthers)
        {
            panther.reset();
        }
        
        BirdEmiter[] birdEmiters = FindObjectsOfType(typeof(BirdEmiter)) as BirdEmiter[];
        foreach (BirdEmiter birdEmiter in birdEmiters)
        {
            birdEmiter.reset();
        }
        Bird[] birds = FindObjectsOfType(typeof(Bird)) as Bird[];
        foreach (Bird bird in birds)
        {
            Destroy(bird.gameObject);
        }

        CuttedChandelierRope[] actions = FindObjectsOfType(typeof(CuttedChandelierRope)) as CuttedChandelierRope[];
        foreach (CuttedChandelierRope act in actions)
        {
            act.Reset();
        }

        DigToLightRays[] actionsDigs = FindObjectsOfType(typeof(DigToLightRays)) as DigToLightRays[];
        foreach (DigToLightRays act in actionsDigs)
        {
            act.Reset();
        }

        //IGResetable[] resetables = FindObjectsOfType(typeof(IGResetable)) as IGResetable[];
        //foreach (IGResetable resetable in resetables)
        //{
        //    resetable.GReset();
        //}

        //Crate[] resetables = FindObjectsOfType(typeof(Crate)) as Crate[];
        //foreach (Crate resetable in allCrates)
        //{
        //    resetable.GReset();
        //}
        //foreach (CutSceneCameraPassing cameraPassing in allCameraPassings)
        //{
        //    cameraPassing.GReset();
        //}
        rlhScene.ResetAllResetables();
    }

    public CheckPoint LastTouchedCheckPoint
    {
        get
        {
            return lastTouchedCheckPoint;
        }
    }
    CheckPoint lastTouchedCheckPoint;
    CheckPoint lastTouchedCheckPointCandidate;
    float lastTouchedCheckPointApproveTime = 0f;

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

    public float mass = 5f;
    //public float pushedForce = -10f;

    bool hitByStone(Transform stone)
    {
        //return false;

        Rigidbody2D stoneBody = stone.GetComponent<Rigidbody2D>();
        if (!stoneBody)
            return false;

        if (currentController == zapControllerGravityGun)
        {
            if (zapControllerGravityGun.draggedStone == stone)
            {
                die(DeathType.DRAGGED_STONE_HIT);
                return true;
            }
        }

        if (stone == groundUnder) return false;

        float stoneSpeed = stoneBody.velocity.magnitude;
        //		if (stoneSpeed > stoneDeadlySpeed) {
        //			die (DeathType.STONE_HIT);
        //			return true;
        //} else 
        if (stoneSpeed < stoneMinDeadySpeed)
        {
            return false;
        }

        float stoneMass = stoneBody.mass;
        //		if (stoneMass > stoneDeadlyMass) {
        //			die(DeathType.STONE_HIT );
        //			return true;
        //		}
        if (stoneMass < stoneMinDeadyMass)
        {
            return false;
        }

        float stoneEnergy = stoneSpeed * stoneMass;
        if (stoneEnergy > stoneDeadlyEnergy)
        {
            die(DeathType.STONE_HIT);
            return true;
        }

        return false;
    }

    public bool TryJump()
    {
        if (!groundUnder) return false;

        CollapseableFootbridge cfb = groundUnder.GetComponent<CollapseableFootbridge>();
        //if (cfb && cfb.enabled && cfb.CollapseOnJump)
        //{
        //    cfb.Collapse();
        //    return false;
        //}
        //IfJustCollapsedJumpEnabled

        if (cfb && cfb.enabled)
        {
            if (cfb.CollapseOnJump)
            {
                cfb.Collapse();
                return cfb.IfJustCollapsedJumpEnabled;
            }
            return true;
        }
        
        return true;
    }

    public bool touchStone(Transform stone)
    {
        return touchStone(stone, transform.position);
    }

    public bool touchStone(Transform stone, Vector2 forcePosition)
    {
        //return false;

        //print(Physics2D.gravity);
        CollapseableFootbridge cfb = stone.GetComponent<CollapseableFootbridge>();
        if ( cfb)
        {
            cfb.StartCollapse();
        }

        Rigidbody2D stoneBody = stone.GetComponent<Rigidbody2D>();
        if (!stoneBody)
            return false;
        

        Vector2 touchedForce = new Vector2(0f, 0f);
        touchedForce.y = mass * Physics2D.gravity.y; //pushedForce; // + (velocity.y * mass);
        
        //touchedForce.y *= -1.0f;
        stoneBody.AddForceAtPosition(touchedForce, forcePosition, ForceMode2D.Force);
        //stoneBody.

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

    Bat suckedMeBat = null;
    float myEnergy = 1f;
    float myMaxEnergy = 1f;

    public void suddenlyAttackByBat(Bat bat)
    {
        if (currentController != zapControllerSuckByBat)
        {
            suckedMeBat = bat;
            //beforeFallController = currentController;
            setCurrentController(zapControllerSuckByBat);
            //setState(Zap.State.IN_AIR);
            //zapControllerNormal.suddenlyInAir();
        }
    }
    public void restoreBeforeAttackByBatController()
    {
        if (suckedMeBat)
        {
            suckedMeBat.StopSuckZap();
        }
        suckedMeBat = null;

        setCurrentController(zapControllerNormal, true);
        //if (beforeFallController != null)
        //{
        //    setCurrentController(beforeFallController, true);
        //    beforeFallController = null;
        //}
    }

    bool BatAttack(Bat bat)
    {
        if (isDead()) return false;

        if (!bat.IsInState(Bat.State.Dive))
        {
            return false;
        }

        if (currentController.isDodging()) return false;
        if (isNotInState(State.ON_GROUND)) return false;
        if (currentController == zapControllerSuckByBat) return false;

        if (Mathf.Abs(targeter.position.y - bat.transform.position.y) < 0.5f)
        {
            //die(DeathType.PANTHER);
            bat.StartSuckZap();
            //suckMe = bat;
            suddenlyAttackByBat(bat);

            return true;
        }

        return false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead())
            return;

        if (other.gameObject.tag == "Bat")
        {
            //Debug.Log("Fuck! Its a bat!");
            Bat bat = other.GetComponent<Bat>();
            BatAttack(bat);
            return;
        }

        int lid = other.transform.gameObject.layer;
        if (lid == LayerMask.NameToLayer("GroundMoveable") || lid == LayerMask.NameToLayer("DraggedGroundMoveable"))
        { // layerIdGroundMoveableMask) { // to jest kamien 
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

        if (other.gameObject.tag == "Panther")
        {
            Panther panther = other.gameObject.GetComponent<Panther>();
            if (panther.attacking())
            {
                if (!currentController.isDodging())
                {
                    die(DeathType.PANTHER);
                }
            }
            return;
        }

        if (other.gameObject.tag == "ShowInfoTrigger")
        {
            ShowInfoTrigger touchedSIT = other.GetComponent<ShowInfoTrigger>();
            if (showedSIT != touchedSIT)
            {
                if (rlhScene.isActiveShowInfoTrigger(touchedSIT))
                {
                    //ShowInfoTrigger sit = other.gameObject.GetComponent<ShowInfoTrigger>();
                    if (!touchedSIT.used)
                    {
                        showInfo(touchedSIT);
                    }
                }
            }
            return;
        }
    }

    //void OnCollisionEnter2D(Collision2D c)
    //{
    //    print(c);
    //}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead())
            return;

        if (currentController.triggerEnter(other))
            return;

        if (other.gameObject.tag == "Bat")
        {
            //Debug.Log("Fuck! Its a bat!");

            Bat bat = other.GetComponent<Bat>();
            //if( !bat.IsInState(Bat.State.Dive))
            //{
            //    return;
            //}
            //if( Mathf.Abs(transform.position.y - bat.transform.position.y) < 0.5f )
            //{
            //    die(DeathType.PANTHER);
            //}
            BatAttack(bat);
            return;
        }

        if (other.gameObject.tag == "KillerPhysic")
        {
            die(DeathType.POISON);
            return;
        }

        if (other.gameObject.tag == "KillerPhysic2")
        {
            //die(DeathType.POISON);
            print("KillerPhysic2 : " + velocity + " " + lastVelocity + " " + gfx.GetComponent<Rigidbody2D>().velocity);
            return;
        }

        //BatActivator  other.GetComponent<BatActivator>()
        //if (  )

        int lid = other.transform.gameObject.layer;
        int lid2 = LayerMask.NameToLayer("GroundMoveable");
        if (lid == lid2)
        {// layerIdGroundMoveableMask) { // to jest kamien 
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
            lastTouchedCheckPointCandidate = other.GetComponent<CheckPoint>();
            lastTouchedCheckPointApproveTime = 1f;

            //lastTouchedCheckPoint = other.GetComponent<CheckPoint>();
            //// zatwierdzam wszystkie zdobyte kawalki mapy...
            //collectedMapParts.Clear();
            //// zatwierdzam zdobycie noza i/lub gravityguna
            //_haveKnife = HaveKnife;
            //_haveGravityGun = HaveGravityGun;

            return;
        }
       
        if (other.gameObject.tag == "Crocodile")
        {
            die(DeathType.CROCODILE);

            other.gameObject.GetComponent<Crocodile>().attackStart();
            sprRend.enabled = false;

            return;
        }
        if (other.gameObject.tag == "Panther")
        {
            Panther panther = other.gameObject.GetComponent<Panther>();
            if (panther.attacking())
            {
                if (!currentController.isDodging())
                {
                    die(DeathType.PANTHER);
                }
            }
            return;
        }
        if( other.gameObject.tag == "ShowInfoTriggerController")
        {
            rlhScene.activateShowInfoTriggerController(other.GetComponent<ShowInfoTriggerController>());
            return;
        }
        if (other.gameObject.tag == "ShowInfoTrigger")
        {
            if (rlhScene.isActiveShowInfoTrigger(other.GetComponent<ShowInfoTrigger>()))
            {
                ShowInfoTrigger sit = other.gameObject.GetComponent<ShowInfoTrigger>();
                if (!sit.used)
                {
                    showInfo(sit);
                }
            }
            return;
        }
        if (other.gameObject.tag == "ComicPage")
        {
            collectMapPart(other.gameObject);
            return;
        }
        if (other.tag == "Pickable")
        {
            //print("znalazlem.... pickabla " + other.name);
            Pickable pickable = other.GetComponent<Pickable>();
            //if (pickable.isActive)
            {
                switch (pickable.type)
                {
                    case Pickable.Type.KNIFE:
                        if (!HaveKnife)
                        {
                            HaveKnife = true;
                            zapControllerKnife.SetCtrlEnabled(HaveKnife);
                            if (!HaveGravityGun)
                                chooseController(zapControllerKnife);
                        }
                        break;

                    case Pickable.Type.GRAVITY_GUN:
                        if (!HaveGravityGun)
                        {
                            HaveGravityGun = true;
                            zapControllerGravityGun.SetCtrlEnabled(HaveGravityGun);
                            if (!HaveKnife)
                                chooseController(zapControllerGravityGun);
                        }
                        break;
                }
                pickable.activate();
            }
        }
    }

    float puzzleMapShowTime = 0.0f;
    bool puzzleMapShowing = false;
    int puzzleShowingPhase = 0;
    int newPuzzleCollectedID = 0;

    List<GameObject> collectedMapParts = new List<GameObject>();

    void collectMapPart(GameObject mapPart)
    {

        ComicPage comicPart = mapPart.GetComponent<ComicPage>();
        int mapPartID = comicPart.partID;

        if (mapPartParts.Length == 0 || mapPartParts[mapPartID] == null)
            return;

        if (mapPartParts[mapPartID].collected)
            return;

        mapPartParts[mapPartID].collect();

        mapPart.GetComponent<SpriteRenderer>().enabled = false;
        collectedMapParts.Add(mapPart);

        newPuzzleCollectedID = mapPartID;
        showPuzzleMap();
    }

    void showPuzzleMap()
    {
        puzzleMapShowTime = 0.0f;
        puzzleMapShowing = true;
        puzzleShowingPhase = 1;

        for (int i = 0; i < mapPartParts.Length; ++i)
        {
            if (i == newPuzzleCollectedID)
                continue;
            if (mapPartParts[i].collected)
            {
                mapPartParts[i].show(1.0f);
            }
        }
    }

    void hidePuzzleMap()
    {
    }

    bool userJumpKeyPressed = false;
    float timeFromJumpKeyPressed = 0.0f;
    [HideInInspector]
    public bool jumpKeyPressed = false;

    void FixedUpdate()
    {
        if (currentController != null)
        {
            //currentController.FUpdate(Time.fixedDeltaTime);
            currentController.FUpdate(Time.deltaTime);
        }
    }

    private float ConstantFrameTime = 0.0333f;
    private float CurrentDeltaTime = 0.0f;

    public float getConstantFrameTime()
    {
        return ConstantFrameTime;
    }
    public float getCurrentDeltaTime()
    {
        return CurrentDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
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

        if (GlobalUpdate(timeSinceLastFrame)) return;

        if (!paused)
        {
            bool firstUpdateInFrame = true;
            while (timeSinceLastFrame > ConstantFrameTime)
            {
                ZapUpdate(ConstantFrameTime, firstUpdateInFrame);
                firstUpdateInFrame = false;
                timeSinceLastFrame -= ConstantFrameTime;
            }

            ZapUpdate(timeSinceLastFrame, firstUpdateInFrame);
        }
    }

    float GlobalResetDestTime = 0f;
    bool GlobalResetStarted = false;

    bool GlobalUpdate(float deltaTime)
    {
        UpdateCameraOffset();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.P))
        {
            gamePaused = !gamePaused;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            rlhScene.printShowInfoTriggersControlls();
        }
        if( Input.GetKeyDown(KeyCode.T))
        {
            rlhScene.CamController.ShakeImpulseStart(2f, 0.25f, 8f);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            rlhScene.CamController.ShakePermanentStart(0.25f, 8f);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            rlhScene.CamController.ShakeStop(-1f);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            rlhScene.CamController.ShakeStop(2f);
        }

        if (lastTouchedCheckPointCandidate != null)
        {
            if ((lastTouchedCheckPointApproveTime -= deltaTime) <= 0f)
            {
                lastTouchedCheckPoint = lastTouchedCheckPointCandidate;
                lastTouchedCheckPointCandidate = null;
                // zatwierdzam wszystkie zdobyte kawalki mapy...
                collectedMapParts.Clear();
                // zatwierdzam zdobycie noza i/lub gravityguna
                _haveKnife = HaveKnife;
                _haveGravityGun = HaveGravityGun;

                rlhScene.checkPointReached();

                foreach (GroundMoveable gm in allStones)
                {
                    gm.SaveResets();
                }
                Snake[] snakes = FindObjectsOfType(typeof(Snake)) as Snake[];
                foreach (Snake snake in snakes)
                {
                    snake.checkPointReached();
                }

                Pickable[] pickables = FindObjectsOfType(typeof(Pickable)) as Pickable[];
                foreach (Pickable pickable in pickables)
                {
                    pickable.checkPointReached();
                }
                SmashStoneActivator[] smashStoneActivators = FindObjectsOfType(typeof(SmashStoneActivator)) as SmashStoneActivator[];
                foreach (SmashStoneActivator smashStoneActivator in smashStoneActivators)
                {
                    smashStoneActivator.checkPointReached();
                }
                //IGResetable[] resetables = FindObjectsOfType(typeof(IGResetable)) as IGResetable[];
                //foreach (IGResetable resetable in resetables)
                //{
                //    resetable.GCacheResetData();
                //}

                ////Crate[] resetables = FindObjectsOfType(typeof(Crate)) as Crate[];
                //foreach (Crate resetable in allCrates)
                //{
                //    resetable.GResetCacheResetData();
                //}
                //foreach (CutSceneCameraPassing cameraPassing in allCameraPassings)
                //{
                //    cameraPassing.GResetCacheResetData();
                //}

                CuttedChandelierRope[] actions = FindObjectsOfType(typeof(CuttedChandelierRope)) as CuttedChandelierRope[];
                foreach (CuttedChandelierRope act in actions)
                {
                    act.SaveResets();
                }

                DigToLightRays[] actionsDigs = FindObjectsOfType(typeof(DigToLightRays)) as DigToLightRays[];
                foreach (DigToLightRays act in actionsDigs)
                {
                    act.SaveResets();
                }

                rlhScene.CacheAllResetableDatas();
            }
        }

        if (Input.GetMouseButtonDown(0))
        { // left
          //print ("left: " + Input.mousePosition);
        }
        if (Input.GetMouseButton(1))
        { // right
          //print ("right: " + Input.mousePosition);
        }
        //
        if (Input.GetKeyDown(KeyCode.R))
        {
            reborn();
            GlobalResetDestTime = Time.time + 2f;
            GlobalResetStarted = true;
        }
        if (isDead())
        {
            if (currentStateTime > afterDeathPauseDuration)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    reborn();
                    return true;
                }
            }
        }
        if ( Input.GetKey(KeyCode.R))
        {
            if (GlobalResetStarted)
            {
                if (GlobalResetDestTime < Time.time)
                {
                    GlobalResetStarted = false;
                    print("level restart");
                    Application.LoadLevel(Application.loadedLevel);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            GlobalResetDestTime = 0f;
            GlobalResetStarted = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //setPrevWeapon();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //setNextWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //print ("Q");

            if (choosenController == zapControllerKnife)
            {
                //print ("set choosen gravitygun");
                //if (HaveGravityGun)
                //{
                //    if (choosenController == currentController && choosenController.tryDeactiveate())
                //    {
                //        chooseController(zapControllerGravityGun);
                //    }
                //    else
                //    {
                //        chooseController(zapControllerGravityGun);
                //    }
                //}
            }
            else if (choosenController == zapControllerGravityGun)
            {
                //print ("set choosen knife");
                if (HaveKnife)
                {
                    if (choosenController == currentController && choosenController.tryDeactiveate())
                    {
                        chooseController(zapControllerKnife);
                    }
                    else
                    {
                        chooseController(zapControllerKnife);
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //print ("E");

            if (choosenController == zapControllerKnife)
            {
                //print ("set choosen gravitygun");
                // jezeli to jest aktywny kontroller:
                if (HaveGravityGun)
                {
                    if (choosenController == currentController && choosenController.tryDeactiveate())
                    {
                        chooseController(zapControllerGravityGun);
                    }
                    else
                    {
                        chooseController(zapControllerGravityGun);
                    }
                }
            }
            else if (choosenController == zapControllerGravityGun)
            {
                ////print ("set choosen knife");
                //if (HaveKnife)
                //{
                //    if (choosenController == currentController && choosenController.tryDeactiveate())
                //    {
                //        chooseController(zapControllerKnife);
                //    }
                //    else
                //    {
                //        chooseController(zapControllerKnife);
                //    }
                //}
            }
        }

        PuzzleMapUpdate(deltaTime);
        if (GamePausedUpdate(deltaTime))
            return true;

        InfoLabelUpdate(deltaTime);


        //		if (Input.GetKeyDown (KeyCode.B)) {
        //			print( shadowSprite.uv );
        //			print( shadowSprite.border );
        //			print( shadowSprite.bounds );
        //			print( shadowSprite.rect );
        //		}

        return false;
    }

    public float afterDeathPauseDuration = 0f;

    void ZapUpdate(float deltaTime, bool firstUpdateInFrame)
    {
        

        CurrentDeltaTime = deltaTime;

        SetImpulse(new Vector2(0.0f, 0.0f));

        stateJustChanged = false;
        currentStateTime += deltaTime;
        currentActionTime += deltaTime;

        if (!paused)
        {
            if (Input.GetKeyDown(keyUp))
            {
                currentController.keyUpDown();
            }
            if (Input.GetKeyUp(keyUp))
            {
                currentController.keyUpUp();
            }
            if (Input.GetKeyDown(keyDown))
            {
                currentController.keyDownDown();
            }
            if (Input.GetKeyUp(keyDown))
            {
                currentController.keyDownUp();
            }

            if (Input.GetKeyUp(keyJump))
            {
                jumpKeyPressed = false;
                currentController.keyJumpUp();
            }

            if (Input.GetKeyDown(keyLeft))
            {
                currentController.keyLeftDown();
            }
            if (Input.GetKeyDown(keyRight))
            {
                currentController.keyRightDown();
            }

            if (Input.GetKeyUp(keyLeft))
            {
                currentController.keyLeftUp();
            }
            if (Input.GetKeyUp(keyRight))
            {
                currentController.keyRightUp();
            }

            if (Input.GetKeyDown(keyRun))
            {
                currentController.keyRunDown();
            }
            else if (Input.GetKeyUp(keyRun))
            {
                currentController.keyRunUp();
            }

            if (!userJumpKeyPressed)
            {
                if (Input.GetKeyDown(keyJump))
                {
                    if (Input.GetKey(keyLeft) || Input.GetKey(keyRight))
                    {
                        jumpKeyPressed = true;
                        currentController.keyJumpDown();
                    }
                    else
                    {
                        timeFromJumpKeyPressed = 0.0f;
                        userJumpKeyPressed = true;
                    }
                }
            }
            else
            {
                timeFromJumpKeyPressed += deltaTime;
                if (timeFromJumpKeyPressed >= 0.06f)
                {
                    timeFromJumpKeyPressed = 0.0f;
                    userJumpKeyPressed = false;
                    jumpKeyPressed = true;

                    currentController.keyJumpDown();
                }
            }

            if (currentController != zapControllerGravityGun && zapControllerGravityGun == choosenController)
            {
                zapControllerGravityGun.leftMouseNotPressed();
            }
        }

        currentController.MUpdate(CurrentDeltaTime, firstUpdateInFrame);

        currentController.CheckEnergy(CurrentDeltaTime);
        
        updateShadow();
    }

    void PuzzleMapUpdate(float deltaTime)
    {
        if (puzzleMapShowing)
        {
            puzzleMapShowTime += deltaTime;

            if (puzzleMapShowTime < 1.0f)
            {
                float phaseRatio = puzzleMapShowTime / 1.0f;
                Color newColor = new Color(1f, 1f, 1f, 0.75f * phaseRatio);
                mapBackgroundImage.color = newColor;
            }
            else if (puzzleMapShowTime < 2.0f)
            {
                if (puzzleShowingPhase == 1)
                {
                    puzzleShowingPhase = 2;
                    Color newColor = new Color(1f, 1f, 1f, 1f);
                    mapBackgroundImage.color = newColor;
                    mapPartParts[newPuzzleCollectedID].show(0.5f);
                }
            }
            else
            {
                if (puzzleShowingPhase == 2)
                {
                    puzzleShowingPhase = 3;
                    for (int i = 0; i < mapPartParts.Length; ++i)
                    {
                        if (mapPartParts[i].collected)
                        {
                            mapPartParts[i].hide(1.0f);
                        }
                    }
                }

                if (puzzleMapShowTime >= 3.0f)
                {
                    puzzleMapShowing = false;
                    puzzleShowingPhase = 0;
                    Color newColor = new Color(1f, 1f, 1f, 0f);
                    mapBackgroundImage.color = newColor;
                }
                else
                {
                    float phaseRatio = puzzleMapShowTime - 2.0f / 1.0f;
                    Color newColor = new Color(1f, 1f, 1f, 0.75f - 0.75f * phaseRatio);
                    mapBackgroundImage.color = newColor;
                }

            }
        }
    }
    bool GamePausedUpdate(float deltaTime)
    {
        if (gamePaused)
        {
            if (Input.GetKey("f"))
            {
                transform.position = transform.position + new Vector3(-0.1f, 0.0f, 0.0f);
                //showInfo("You press f",1f);
            }
            else if (Input.GetKey("h"))
            {
                transform.position = transform.position + new Vector3(0.1f, 0.0f, 0.0f);
                //showInfo("You press h",2f);
            }
            else if (Input.GetKey("t"))
            {
                transform.position = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
                ///showInfo("You press t",3f);
            }
            else if (Input.GetKey("g"))
            {
                transform.position = transform.position + new Vector3(0.0f, -0.1f, 0.0f);
                //showInfo("You press g",4f);
            }
            return true;
        }
        return false;
    }

    void updateShadow()
    {
        if (!shadowCenter)
            return;

        float msd = 2f;

        RaycastHit2D hit = Physics2D.Raycast(sensorDown2.position, -Vector2.up, msd, layerIdGroundMask);
        if (hit.collider)
        {
            shadowCenterSR.enabled = true;
            shadowCenterSR.color = new Color(1f, 1f, 1f, (msd - hit.distance) / msd);

            Vector3 shadowPos = shadowCenter.localPosition;
            shadowPos.y = -hit.distance;
            shadowCenter.localPosition = shadowPos;

            shadowCenter.rotation = hit.collider.transform.rotation;
        }
        else
        {
            shadowCenterSR.enabled = false;
        }

        hit = Physics2D.Raycast(sensorDown1.position, -Vector2.up, msd, layerIdGroundMask);
        if (hit.collider)
        {
            shadowLeftSR.enabled = true;
            shadowLeftSR.color = new Color(1f, 1f, 1f, (msd - hit.distance) / msd);

            float colliderRot = hit.collider.transform.rotation.eulerAngles.z;
            float r = colliderRot - shadowCenter.rotation.eulerAngles.z;
            Quaternion quat = new Quaternion();
            quat.eulerAngles = new Vector3(0f, 0f, r);
            shadowLeft.localRotation = quat;
        }
        else
        {
            shadowLeftSR.enabled = false;
        }

        hit = Physics2D.Raycast(sensorDown3.position, -Vector2.up, msd, layerIdGroundMask);
        if (hit.collider)
        {
            shadowRightSR.enabled = true;
            shadowRightSR.color = new Color(1f, 1f, 1f, (msd - hit.distance) / msd);

            float colliderRot = hit.collider.transform.rotation.eulerAngles.z;
            float r = colliderRot - shadowCenter.rotation.eulerAngles.z;
            Quaternion quat = new Quaternion();
            quat.eulerAngles = new Vector3(0f, 0f, r);
            shadowRight.localRotation = quat;
        }
        else
        {
            shadowRightSR.enabled = false;
        }
    }

    float infoLabelShowDuration = 0f;
    float infoLabelShowTime = 0f;
    
    ShowInfoTrigger showedSIT = null;
    int showSITIndex = 0;

    void showInfo(ShowInfoTrigger newSIT)
    {
        if (newSIT.getNumberOfInfos() == 0) return;

        showedSIT = newSIT;
        showSITIndex = 0;
        if (showedSIT.OnlyFirstTime) showedSIT.used = true;
        
        showInfo();
    }

    public void showInfo()
    {
        if (showedSIT == null) return;
        showInfo(showedSIT.Infos[showSITIndex], showedSIT.ShowDurations[showSITIndex]);
    }
    public void showInfo(string newInfo, float duration)
    {
        if (infoLabel)
        {
            infoLabel.text = newInfo;
            infoLabelShowTime = 0f;
            infoLabelShowDuration = duration;
            //infoLabelSet = true;
        }
    }
    public void resetInfo()
    {
        showedSIT = null;
        showSITIndex = 0;
        infoLabelShowTime = 0f;
        infoLabelShowDuration = 1f;

        if (infoLabel)
        {
            infoLabel.text = "";
        }
    }
    void InfoLabelUpdate(float deltaTime)
    {
        if (showedSIT != null)
        {
            if ((infoLabelShowTime += deltaTime) > infoLabelShowDuration)
            {
                showSITIndex += 1;
                if (showedSIT.getNumberOfInfos() > showSITIndex)
                {
                    showInfo();
                }
                else
                {
                    resetInfo();
                }
            }
        }
    }

    public Transform CheckObstacle(int dir, float distToCheck, ref float distToObstacle, bool simulateCrouch = false)
    {
        if (dir == 1)
        {
            //distToObstacle = CheckRight(Mathf.Abs(distToCheck) + 0.01f,ref distToObstacle);
            Transform obstacle = CheckRight(Mathf.Abs(distToCheck) + 0.01f, ref distToObstacle, false, simulateCrouch);
            //if (distToObstacle < 0.0f)
            if( !obstacle )
                return null;
            if (distToObstacle < distToCheck)
            {
                return obstacle;
            }
            else
            {
                //print("jest obstacle ale : distToObstacle > distToCheck : " + distToObstacle + " > " + distToCheck);
                return null;
            }
        }
        else if (dir == -1)
        {
            //distToObstacle = checkLeft(Mathf.Abs(distToCheck) + 0.01f);
            Transform obstacle = CheckLeft(Mathf.Abs(distToCheck) + 0.01f, ref distToObstacle, false, simulateCrouch);
            //if (distToObstacle < 0.0f)
            //    return false;
            if (!obstacle) return null;
            if (distToObstacle < Mathf.Abs(distToCheck))
            {
                distToObstacle *= -1.0f;
                //return true;
                return obstacle;
            }
            else
                return null;
        }
        else
        {
            return null;
        }
    }

    KeyCode getDirKey(int dir)
    {
        return dir == 1 ? keyRight : keyLeft;
    }

    public void turnLeft()
    {
        Vector3 scl = gfx.localScale;
        scl.x = Mathf.Abs(scl.x) * -1.0f;
        gfx.localScale = scl;

        scl = gfxLegs.localScale;
        scl.x = Mathf.Abs(scl.x) * -1.0f;
        gfxLegs.localScale = scl;
    }
    public void turnRight()
    {
        Vector3 scl = gfx.localScale;
        scl.x = Mathf.Abs(scl.x) * 1.0f;
        gfx.localScale = scl;

        scl = gfxLegs.localScale;
        scl.x = Mathf.Abs(scl.x) * 1.0f;
        gfxLegs.localScale = scl;
    }

    public Vector2 dir()
    {
        return gfx.localScale.x > 0.0f ? Vector2.right : -Vector2.right;
    }
    public int dir2()
    {
        return gfx.localScale.x > 0f ? 1 : -1;
    }
    public int dir2N()
    {
        return gfx.localScale.x > 0f ? -1 : 1;
    }
    public bool faceRight()
    {
        return gfx.localScale.x > 0f;
    }
    public bool faceLeft()
    {
        return gfx.localScale.x < 0f;
    }

    public bool canGetUp()
    {

        if (dir() == Vector2.right)
        {
            RaycastHit2D hit = Physics2D.Raycast(sensorLeft3.position, Vector2.right, myWidth, layerIdGroundMask);
            if (hit.collider != null)
            {
                float hpx = hit.point.x;
                float _d = Mathf.Abs(sensorLeft3.position.x + myWidth - hpx);
                if (_d > 0.0001f)
                    return false;
            }
            return true;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(sensorRight3.position, -Vector2.right, myWidth, layerIdGroundMask);
            if (hit.collider != null)
            {
                float hpx = hit.point.x;
                float _d = Mathf.Abs(sensorRight3.position.x - myWidth - hpx);
                if (_d > 0.0001f)
                    return false;
            }
            return true;
        }
    }

    public float CheckLeft(float checkingDist, bool flying = false, bool simulateCrouch = false)
    {
        float distToObstacle = -1f;
        Transform obstacle = CheckLeft(checkingDist, ref distToObstacle, flying, simulateCrouch);
        return distToObstacle;
    }

    public Transform CheckLeft(float checkingDist, ref float distToObstacle, bool flying = false, bool simulateCrouch = false)
    {
        Vector2 rayOrigin = sensorDown2.position;
        rayOrigin.y += 0.5f;

        if (!flying)
        {
            hit = Physics2D.Raycast(rayOrigin, Vector2.left, checkingDist + 0.5f, layerIdGroundAllMask);
            if (hit.collider != null)
            {
                float angle = Vector2.Angle(Vector2.up, hit.normal);
                if (Mathf.Abs(angle) > 45.0f)
                {
                    distToObstacle = hit.distance - 0.5f;
                    return hit.transform;
                }
            }

            rayOrigin.x -= (checkingDist + 0.5f);
            ////if (!flying && (simulateCrouch || currentController.crouching() || (currentController != zapControllerGravityGun && currentController.isDodging())))
            ////{
            ////    //return -1.0f;
            ////    distToObstacle = -1f;
            ////    return null;
            ////}
            float currHeight = 1.4f;
            if (currentController.crouching()) currHeight = 0.25f;
            hit = Physics2D.Raycast(rayOrigin, Vector2.up, currHeight, layerIdGroundAllMask);
            if (hit.collider)
            {
                distToObstacle = 0f;
                return hit.transform;
            }
            distToObstacle = -1f;
            return null;
        }
        else
        {
            rayOrigin = sensorDown2.position;
            rayOrigin.x -= (checkingDist + 0.5f);
            //if( stateJustChanged )
            if (velocity.y > 0f || stateJustChanged)
            {
                rayOrigin.y += 0.5f;
                hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.4f, layerIdGroundAllMask);
            }
            else
            {
                //rayOrigin.y += 0.5f;
                hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.9f, layerIdGroundAllMask);
            }
            if (hit.collider)
            {
                distToObstacle = 0f;
                return hit.transform;
            }
            distToObstacle = -1f;
            return null;
        }

        //distToObstacle = -1f;
        //return null;

        ////Vector2 rayOrigin = transform.position;
        ////rayOrigin.x -= (myHalfWidth + checkingDist);

        ////int numRes = 0;

        ////numRes = Physics2D.RaycastNonAlloc(sensorDown2.position, Vector2.left, raycastHits, checkingDist + 0.5f, layerIdGroundAllFullMask);

        ////for (int i = 0; i < numRes; ++i)
        ////{
        ////    hit = raycastHits[i];
        ////    if (hit.fraction == 0f) continue;
        ////    float angle = Vector2.Angle(Vector2.up, hit.normal);
        ////    if (Mathf.Abs(angle) > 45.0f)
        ////    {
        ////        Vector2 ro = sensorDown2.position;
        ////        ro.x -= (hit.distance + 0.01f);
        ////        ro.y += 0.2f;
        ////        int numRes2 = Physics2D.RaycastNonAlloc(ro, -Vector2.up, raycastHits2, 0.2f + 0.1f, layerIdGroundAllFullMask);
        ////        for (int j = 0; j < numRes2; ++j)
        ////        {
        ////            hit2 = raycastHits2[j];
        ////            if (hit2.collider != hit.collider) continue;
        ////            if (hit2.fraction == 0f)
        ////            {
        ////                //return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        ////                distToObstacle = Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        ////                //distToObstacle = hit.distance;
        ////                return hit.transform;
        ////            }
        ////            float angle2 = Vector2.Angle(Vector2.up, hit2.normal);
        ////            if (Mathf.Abs(angle2) > 45.0f)
        ////            {
        ////                //return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        ////                distToObstacle = Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        ////                //distToObstacle = hit.distance;
        ////                return hit.transform;
        ////            }
        ////        }
        ////    }
        ////}


        ////Vector2 collBoxOrigin = transform.position;
        ////collBoxOrigin.y += 0.95f;
        ////Vector2 collBoxSize = new Vector2(1f, 1.9f);
        ////RaycastHit2D[] _hits = Physics2D.BoxCastAll(collBoxOrigin, collBoxSize, 0f, Vector2.left, checkingDist, layerIdGroundAllFullMask);
        //////print(_hits.Length);
        ////for (int i = 0; i < _hits.Length; ++i)
        ////{
        ////    print(_hits[i].distance);
        ////}

        //if (!stateJustChanged)
        //{
        //    //if (flying)
        //    //{
        //    //    hit = Physics2D.Raycast(sensorLeft1.position, Vector2.left, checkingDist, layerIdGroundAllMask);
        //    //    if (hit.collider != null)
        //    //    {
        //    //        //return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        //    //        distToObstacle = hit.distance;
        //    //        return hit.transform;
        //    //    }
        //    //}
        //    //else
        //    {
        //        Vector2 rayOrigin = sensorDown2.position;
        //        rayOrigin.y += 0.5f;
        //        hit = Physics2D.Raycast(rayOrigin, Vector2.left, checkingDist + 0.5f, layerIdGroundAllFullMask);
        //        if (hit.collider != null)
        //        {
        //            float angle = Vector2.Angle(Vector2.up, hit.normal);
        //            if (Mathf.Abs(angle) > 45.0f)
        //            {
        //                //return Mathf.Abs(hit.point.x - sensorRight2.position.x);
        //                distToObstacle = hit.distance - 0.5f;
        //                return hit.transform;
        //            }
        //        }
        //        else
        //        {
        //            rayOrigin.x += checkingDist + 0.5f;
        //            hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.39f, layerIdGroundAllFullMask);
        //            return hit.collider != null;

        //            //if (hit.collider != null)
        //            //{
        //            //    float angle = Vector2.Angle(Vector2.up, hit.normal);
        //            //    if (Mathf.Abs(angle) > 45.0f)
        //            //    {
        //            //        //return Mathf.Abs(hit.point.x - sensorRight2.position.x);
        //            //        distToObstacle = hit.distance - 0.5f;
        //            //        return hit.transform;
        //            //    }
        //            //}
        //        }

        //        //int numRes = Physics2D.RaycastNonAlloc(sensorDown2.position, Vector2.left, raycastHits, checkingDist + 0.5f, layerIdGroundAllFullMask);
        //        //for (int i = 0; i < numRes; ++i)
        //        //{
        //        //    hit = raycastHits[i];
        //        //    if (hit.fraction == 0f) continue;
        //        //    float angle = Vector2.Angle(Vector2.up, hit.normal);
        //        //    if (Mathf.Abs(angle) > 45.0f)
        //        //    {
        //        //        Vector2 ro = sensorDown2.position;
        //        //        ro.x -= (hit.distance + 0.01f);
        //        //        ro.y += 0.2f;
        //        //        int numRes2 = Physics2D.RaycastNonAlloc(ro, -Vector2.up, raycastHits2, 0.2f + 0.1f, layerIdGroundAllFullMask);
        //        //        for (int j = 0; j < numRes2; ++j)
        //        //        {
        //        //            hit2 = raycastHits2[j];
        //        //            if (hit2.collider != hit.collider) continue;
        //        //            if (hit2.fraction == 0f)
        //        //            {
        //        //                //return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        //        //                distToObstacle = Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        //        //                //distToObstacle = hit.distance;
        //        //                return hit.transform;
        //        //            }
        //        //            float angle2 = Vector2.Angle(Vector2.up, hit2.normal);
        //        //            if (Mathf.Abs(angle2) > 45.0f)
        //        //            {
        //        //                //return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        //        //                distToObstacle = Mathf.Abs(hit.point.x - sensorLeft1.position.x);
        //        //                //distToObstacle = hit.distance;
        //        //                return hit.transform;
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //    }
        //}
        ////hit = Physics2D.Raycast(sensorLeft2.position, Vector2.left, checkingDist, layerIdGroundAllFullMask);
        ////if (hit.collider != null)
        ////{
        ////    //return Mathf.Abs(hit.point.x - sensorLeft2.position.x);
        ////    distToObstacle = hit.distance;
        ////    return hit.transform;
        ////}

        ////if (!flying && (simulateCrouch || currentController.crouching() || (currentController != zapControllerGravityGun && currentController.isDodging())))
        ////{
        ////    //return -1.0f;
        ////    distToObstacle = -1f;
        ////    return null;
        ////}

        ////hit = Physics2D.Raycast(sensorLeft3.position, Vector2.left, checkingDist, layerIdGroundAllFullMask);
        ////if (hit.collider != null)
        ////{
        ////    //return Mathf.Abs(hit.point.x - sensorLeft3.position.x);
        ////    distToObstacle = hit.distance;
        ////    return hit.transform;
        ////}
        ////distToObstacle = -1f;
        ////return null;
    }

    public float CheckRight(float checkingDist, bool flying = false, bool simulateCrouch = false)
    {
        float distToObstacle = -1f;
        Transform obstacle = CheckRight(checkingDist, ref distToObstacle, flying, simulateCrouch);
        return distToObstacle;
    }

    public Transform CheckRight(float checkingDist, ref float distToObstacle, bool flying = false, bool simulateCrouch = false)
    {
        Vector2 rayOrigin = sensorDown2.position;
        rayOrigin.y += 0.5f;

        if (!flying)
        {
            hit = Physics2D.Raycast(rayOrigin, Vector2.right, checkingDist + 0.5f, layerIdGroundAllMask);
            if (hit.collider != null)
            {
                float angle = Vector2.Angle(Vector2.up, hit.normal);
                if (Mathf.Abs(angle) > 45.0f)
                {
                    distToObstacle = hit.distance - 0.5f;
                    return hit.transform;
                }
            }

            rayOrigin.x += (checkingDist + 0.5f);
            ////if (!flying && (simulateCrouch || currentController.crouching() || (currentController != zapControllerGravityGun && currentController.isDodging())))
            ////{
            ////    //return -1.0f;
            ////    distToObstacle = -1f;
            ////    return null;
            ////}
            float currHeight = 1.4f;
            if (currentController.crouching()) currHeight = 0.25f;
            //if (simulateCrouch || currentController.crouching() || (currentController != zapControllerGravityGun && currentController.isDodging()))
            //    currHeight = 0.25f;

            hit = Physics2D.Raycast(rayOrigin, Vector2.up, currHeight, layerIdGroundAllMask);
            if (hit.collider)
            {
                distToObstacle = 0f;
                return hit.transform;
            }
            distToObstacle = -1f;
            return null;
        }
        else
        {
            rayOrigin = sensorDown2.position;
            rayOrigin.x += (checkingDist + 0.5f);
            //if (stateJustChanged) 
            if (velocity.y > 0f || stateJustChanged)
            {
                rayOrigin.y += 0.5f;
                hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.4f, layerIdGroundAllMask);
            }
            else
            {
                //rayOrigin.y += 0.5f;
                hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.9f, layerIdGroundAllMask);
            }
            if (hit.collider)
            {
                distToObstacle = 0f;
                return hit.transform;
            }
            distToObstacle = -1f;
            return null;
        }

        //Vector2 rayOrigin = sensorDown2.position;
        //rayOrigin.y += 0.5f;
        //hit = Physics2D.Raycast(rayOrigin, Vector2.right, checkingDist + 0.5f, layerIdGroundAllFullMask);
        //if (hit.collider != null)
        //{
        //    float angle = Vector2.Angle(Vector2.up, hit.normal);
        //    if (Mathf.Abs(angle) > 45.0f)
        //    {
        //        distToObstacle = hit.distance + 0.5f;
        //        return hit.transform;
        //    }
        //}
        //else
        //{
        //    rayOrigin.x += (checkingDist + 0.5f);
        //    hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.395f, layerIdGroundAllFullMask);
        //    if (hit.collider)
        //    {
        //        distToObstacle = 0f;
        //    }
        //    return hit.transform;
        //}
        //distToObstacle = -1f;
        //return null;

        //if (!stateJustChanged)
        //{
        //    //if (flying)
        //    //{
        //    //    hit = Physics2D.Raycast(sensorRight1.position, Vector2.right, checkingDist, layerIdGroundAllMask);
        //    //    if (hit.collider != null)
        //    //    {
        //    //        distToObstacle = hit.distance;
        //    //        return hit.transform;
        //    //    }
        //    //}
        //    //else
        //    {
        //        Vector2 rayOrigin = sensorDown2.position;
        //        rayOrigin.y += 0.5f;
        //        hit = Physics2D.Raycast(rayOrigin, Vector2.right, checkingDist+0.5f, layerIdGroundAllFullMask);
        //        if (hit.collider != null)
        //        {
        //            float angle = Vector2.Angle(Vector2.up, hit.normal);
        //            if (Mathf.Abs(angle) > 45.0f)
        //            {
        //                //return Mathf.Abs(hit.point.x - sensorRight2.position.x);
        //                distToObstacle = hit.distance-0.5f;
        //                return hit.transform;
        //            }
        //        }

        //        //int numRes = Physics2D.RaycastNonAlloc(sensorDown2.position, Vector2.right, raycastHits, checkingDist + 0.5f, layerIdGroundAllFullMask);
        //        //for (int i = 0; i < numRes; ++i)
        //        //{
        //        //    hit = raycastHits[i];
        //        //    if (hit.fraction == 0f) continue;
        //        //    float angle = Vector2.Angle(Vector2.up, hit.normal);
        //        //    if (Mathf.Abs(angle) > 45.0f)
        //        //    {
        //        //        Vector2 ro = sensorDown2.position;
        //        //        ro.x += (hit.distance + 0.01f);
        //        //        ro.y += 0.2f;
        //        //        int numRes2 = Physics2D.RaycastNonAlloc(ro, -Vector2.up, raycastHits2, 0.2f + 0.1f, layerIdGroundAllFullMask);
        //        //        for (int j = 0; j < numRes2; ++j)
        //        //        {
        //        //            hit2 = raycastHits2[j];
        //        //            if (hit2.collider != hit.collider) continue;

        //        //            if (hit2.fraction == 0f)
        //        //            {
        //        //                //return Mathf.Abs(hit.point.x - sensorRight1.position.x);
        //        //                distToObstacle = Mathf.Abs(hit.point.x - sensorRight1.position.x);
        //        //                //distToObstacle = hit.distance;
        //        //                return hit.transform;
        //        //            }
        //        //            float angle2 = Vector2.Angle(Vector2.up, hit2.normal);
        //        //            if (Mathf.Abs(angle2) > 45.0f)
        //        //            {
        //        //                //return Mathf.Abs(hit.point.x - sensorRight1.position.x);
        //        //                distToObstacle = Mathf.Abs(hit.point.x - sensorRight1.position.x);
        //        //                //distToObstacle = hit.distance;
        //        //                return hit.transform;
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //    }
        //}
        //hit = Physics2D.Raycast(sensorRight2.position, Vector2.right, checkingDist, layerIdGroundAllFullMask);
        //if (hit.collider != null)
        //{
        //    //return Mathf.Abs(hit.point.x - sensorRight2.position.x);
        //    distToObstacle = hit.distance;
        //    return hit.transform;
        //}

        //if (!flying && (simulateCrouch || currentController.crouching() || (currentController != zapControllerGravityGun && currentController.isDodging())))
        //{
        //    //return -1.0f;
        //    distToObstacle = -1f;
        //    return null;
        //}

        //hit = Physics2D.Raycast(sensorRight3.position, Vector2.right, checkingDist, layerIdGroundAllFullMask);
        //if (hit.collider != null)
        //{
        //    //return Mathf.Abs(hit.point.x - sensorRight3.position.x);
        //    distToObstacle = hit.distance;
        //    return hit.transform;
        //}
        //distToObstacle = -1;
        //return null;
    }

    void pushOutLeft(bool flying)
    {
        Vector2 rayOrigin = sensorDown2.position;
        rayOrigin.x += 0.5f;
        rayOrigin.y += 0.5f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.4f, layerIdGroundAllMask);
        if (hit.collider)
        {
            Vector3 p = transform.position;
            p.x -= 2f * CurrentDeltaTime;
            transform.position = p;
        }
    }
    void pushOutRight(bool flying)
    {
        Vector2 rayOrigin = sensorDown2.position;
        rayOrigin.x -= 0.5f;
        //if (flying)
        //{ 
            rayOrigin.y += 0.5f;
            //dist = 1.85;
        //}
        hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.4f, layerIdGroundAllMask);
        if (hit.collider)
        {
            Vector3 p = transform.position;
            p.x += 2f * CurrentDeltaTime;
            transform.position = p;
        }
    }

    public void pushOutFromObstacles(bool flying)
    {
        pushOutLeft(flying);
        pushOutRight(flying);
    }

    public float checkDown(float checkingDist)
    {

        //int layerIdMask = layerIdGroundAllMask;
        ////Vector3 rayOrigin = sensorDown1.position;
        //RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, myWidth, layerIdGroundPermeableMask);
        //if (hit.collider) {// jesetem wewnatrz wskakiwalnej platformy ... nie moge sie zatrzymac..
        //	layerIdMask = layerIdGroundMask;
        //}


        Vector3 rayOrigin = sensorDown2.position;
        RaycastHit2D[] _hits = Physics2D.RaycastAll(rayOrigin, -Vector2.up, checkingDist, layerIdGroundAllFullMask);
        for (int i = 0; i < _hits.Length; ++i)
        {
            if (_hits[i].fraction == 0f) continue;

            if (_hits[i].collider != null)
            {
                //layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
                //return Mathf.Abs(hit.point.y - sensorDown2.position.y);
                return _hits[i].distance;
            }
        }
        return -1f;

        //rayOrigin = new Vector2(sensorDown1.position.x, sensorDown1.position.y);
        //RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, checkingDist, layerIdGroundAllFullMask);
        //if (hit.collider != null)
        //{
        //    //layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
        //    //return Mathf.Abs(hit.point.y - sensorDown1.position.y);
        //    return hit.distance;
        //}
        //else
        //{
        //    rayOrigin = new Vector2(sensorDown2.position.x, sensorDown2.position.y);
        //    hit = Physics2D.Raycast(rayOrigin, -Vector2.up, checkingDist, layerIdGroundAllFullMask);
        //    if (hit.collider != null)
        //    {
        //        //layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
        //        //return Mathf.Abs(hit.point.y - sensorDown2.position.y);
        //        return hit.distance;
        //    }
        //    else
        //    {
        //        rayOrigin = new Vector2(sensorDown3.position.x, sensorDown3.position.y);
        //        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, checkingDist, layerIdGroundAllFullMask);
        //        if (hit.collider != null)
        //        {
        //            //layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
        //            //return Mathf.Abs(hit.point.y - sensorDown3.position.y);
        //            return hit.distance;
        //        }
        //        else
        //        {
        //            return -1.0f;
        //        }
        //    }
        //}
    }

    public void checkGround(ref float distToGround)
    {
        Transform groundUnderFeet = null;
        groundUnderAngle = 0f;
        distToGround = 100f;

        float th = 0.5f;
        float checkingDist = th + 0.1f;
        //if (fromFeet)
        //	checkingDist = 0.5f;

        //Vector2 rayOrigin1 = sensorDown1.position;
        //if( !fromFeet )
        //rayOrigin1.y += th;
        //RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin1, -Vector2.up, checkingDist, layerIdGroundAllFullMask);

        Vector2 rayOrigin2 = sensorDown2.position;
        //if( !fromFeet )
        rayOrigin2.y += th;
        RaycastHit2D[] _hits2 = Physics2D.RaycastAll(rayOrigin2, -Vector2.up, checkingDist, layerIdGroundAllFullMask);

        //Vector2 rayOrigin3 = sensorDown3.position;
        //if( !fromFeet )
        //rayOrigin3.y += th;
        //RaycastHit2D hit3 = Physics2D.Raycast(rayOrigin3, -Vector2.up, checkingDist, layerIdGroundAllFullMask);

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
        //    //dist1 = rayOrigin1.y - hit1.point.y;
        //    groundUnderFeet = hit1.collider.transform;
        //    distToGround = hit1.distance; // dist1;
        //    groundUnderAngle = Vector2.Angle(Vector2.up, hit1.normal);
        //    //layerIdLastGroundTypeTouchedMask = 1 << hit1.collider.transform.gameObject.layer;
        //}
        //if( _hits2.Length > 1)
        //{
        //    print("============================");
        //}
        for (int i = 0; i < _hits2.Length; ++i)
        {
            hit2 = _hits2[i];

            if (hit2.fraction == 0f) continue;

            //if (_hits2.Length > 1)
            //    print(hit2.fraction);
            
            if (hit2.collider != null)
            {
                if (distToGround > hit2.distance)
                {
                    //dist2 = rayOrigin2.y - hit2.point.y;
                    groundUnderFeet = hit2.collider.transform;
                    distToGround = hit2.distance; // dist1;
                    
                    groundUnderAngle = Vector2.Angle(Vector2.up, hit2.normal);
                    //Vector3 cross = Vector3.Cross(Vector2.up, hit2.normal);

                    //print(cross);
                    if (Vector3.Cross(Vector2.up, hit2.normal).z < 0) groundUnderAngle *= -1f;
                }
                //layerIdLastGroundTypeTouchedMask = 1 << hit1.collider.transform.gameObject.layer;

                ////dist2 = rayOrigin2.y - hit2.point.y;
                //if (!groundUnderFeet || (distToGround > hit2.distance))
                //{
                //    groundUnderFeet = hit2.collider.transform;
                //    distToGround = hit2.distance;
                //    groundUnderAngle = Vector2.Angle(Vector2.up, hit2.normal);
                //}
                ////}
                ////else
                ////{
                ////    groundUnderFeet = hit2.collider.transform;
                ////    distToGround = hit2.distance;
                ////    groundUnderAngle = Vector2.Angle(Vector2.up, hit2.normal);
                ////    //layerIdLastGroundTypeTouchedMask = 1 << hit2.collider.transform.gameObject.layer;
                ////}
            }
        }
        //if (_hits2.Length > 1)
        //{
        //    print("============================");
        //}
        //if (hit3.collider != null)
        //{
        //    if( !groundUnderFeet || (distToGround > hit3.distance))
        //    {
        //        groundUnderFeet = hit3.collider.transform;
        //        distToGround = hit3.distance;
        //        groundUnderAngle = Vector2.Angle(Vector2.up, hit3.normal);
        //    }

        //    //dist3 = rayOrigin3.y - hit3.point.y;
        //    //if (groundUnderFeet)
        //    //{
        //    //    if (distToGround > dist3) distToGround = dist3;
        //    //}
        //    //else
        //    //{
        //    //    groundUnderFeet = hit3.collider.transform;
        //    //    distToGround = dist3;
        //    //    groundUnderAngle = Vector2.Angle(Vector2.up, hit3.normal);
        //    //    //layerIdLastGroundTypeTouchedMask = 1 << hit3.collider.transform.gameObject.layer;
        //    //}
        //}

        if (groundUnderFeet)
        {
            //if( !fromFeet )
            distToGround = th - distToGround;
        }

        groundUnder = groundUnderFeet;
    }

    public bool checkCeil(ref float distToFly)
    {
        Vector2 ro = sensorLeft3.position;
        ro.x += 0.1f;
        ro.y += 0.1f;
        RaycastHit2D hit1 = Physics2D.Raycast(ro, Vector2.up, distToFly, layerIdGroundAllMask);
        ro = sensorRight3.position;
        ro.x -= 0.1f;
        ro.y += 0.1f;
        RaycastHit2D hit2 = Physics2D.Raycast(ro, Vector2.up, distToFly, layerIdGroundAllMask);

        if (!hit1.collider && !hit2.collider) return false;

        RaycastHit2D closestHit = hit1;

        if (hit1.collider != null)
        {
            if (hit2.collider != null)
                distToFly = Mathf.Min(hit1.distance, hit2.distance);
            else
                distToFly = hit1.distance;
        }
        else
        {
            distToFly = hit2.distance;
        }

        return true;
    }

    //public Transform CheckHandle2(int layerID)
    //{
    //    return CheckHandle(layerID);
    //}

    //public Transform CheckHandle(int layerID)
    //{
    //    Vector2 rayOrigin = sensorLeft3.transform.position; // transform.position;
    //    rayOrigin.y += 0.3f;
    //    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, myWidth, layerID);

    //    if (!hit.collider)
    //        return null;
    //    else
    //    {
    //        MountMoveable _mm = hit.collider.GetComponent<MountMoveable>();
    //        if (_mm)
    //        {
    //            //Vector3 pointToCheck = handledMountMoveable.ConvertToPointSize(handledMountMoveablePosition);
    //            ////pointToCheck += distToMount;
    //            //bool res = handledMountMoveable.LocalPointHandable(pointToCheck);
    //            ////if (!res)

    //            //Vector3 zapHandpos = zap.transform.position;
    //            //zapHandpos.y += (zap.sensorLeft3.transform.localPosition.y + 0.3f);
    //            //handledMountMoveablePosition = handledMountMoveable.transform.InverseTransformPoint(zapHandpos);

    //            Vector3 zapHandpos = transform.position;
    //            zapHandpos.y += (sensorLeft3.transform.localPosition.y + 0.3f);
    //            Vector3 _pointToCheck = _mm.transform.InverseTransformPoint(zapHandpos);
    //            _pointToCheck = _mm.ConvertToPointSize(_pointToCheck);
    //            bool res = _mm.LocalPointHandable(_pointToCheck);
    //            if( res )
    //            {
    //                return hit.collider.transform;
    //            }
    //        }
    //    }
    //    hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerID);
    //    if (!hit.collider)
    //        return null;

    //    rayOrigin.x += myWidth;
    //    hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerID);
    //    if (hit.collider)
    //        return hit.collider.transform;
    //    else
    //        return null;
    //}

    public bool CheckClimbingWall(Vector3 posToCheck, int layerID)
    {
        //print(posToCheck);

        Assert.IsTrue(layerID > 0);
        
        Vector3 sensorDiff = sensorLeft3.transform.position - transform.position; // transform.position;

        Vector2 rayOrigin = posToCheck + sensorDiff;//aaa
        rayOrigin.y += 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, myWidth, layerID);

        //print(rayOrigin);
        
        if (!hit.collider)
            return false;

        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerID);
        if (!hit.collider)
            return false;

        rayOrigin.x += myWidth;
        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerID);
        return hit.collider;
    }
    
    public void SetImpulse(Vector2 imp) { impulse = imp; }
    public Vector2 GetImpulse() { return impulse; }
    public void AddImpulse(Vector3 imp) { impulse += imp; }
    public void AddImpulse(Vector2 imp)
    {
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
        //HANG,
        CLIMB_ROPE,
        DEAD,
        OTHER
    };

    public State getState()
    {
        return state;
    }

    [HideInInspector]
    public bool stateJustChanged = false;

    //void releaseRope()
    //{
    //    Vector3 _oldPos = transform.position;
    //    _oldPos.y -= 1.65f;
    //    transform.position = _oldPos;

    //    justJumpedRope = catchedRope;

    //    catchedRope.resetDiver();
    //    catchedRope = null;
    //    catchedRopeLink = null;

    //    Quaternion quat = new Quaternion();
    //    quat.eulerAngles = new Vector3(0f, 0f, 0f);
    //    transform.rotation = quat;

    //    zap.Coll.offset = beforeRopeCollOffset;
    //    zap.GfxCollider.offset = beforeRopeGfxCollOffset;

    //    zap.setState(Zap.State.IN_AIR);
    //}

    public void SetRotation(float newRotation)
    {
        Quaternion quat = transform.rotation;
        quat.eulerAngles = new Vector3(0f, 0f, newRotation);
        transform.rotation = quat;
    }
    public void SetGfxRotation(float newRotation)
    {
        Quaternion quat = gfx.rotation;
        quat.eulerAngles = new Vector3(0f, 0f, newRotation);
        gfx.rotation = quat;
    }
    public bool setState(State newState)
    {
        if (state == newState)
            return false;

        currentStateTime = 0.0f;
        stateJustChanged = true;

        state = newState;

        //Quaternion quat = gfx.rotation;
        //quat.eulerAngles = new Vector3(0f, 0f, 0f);
        //gfx.rotation = quat;

        //SetRotation(0f);
        SetGfxRotation(0f);

        //if( state != State.MOUNT)
        //{
        //    cs
        //}

        switch (state)
        {
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
    public bool isInState(State test)
    {
        return state == test;
    }
    public bool isNotInState(State test)
    {
        return state != test;
    }

    /*////////////////////////////////////////////////////////////*/



    [HideInInspector]
    public Transform sensorLeft1;
    [HideInInspector]
    public Transform sensorLeft2;
    [HideInInspector]
    public Transform sensorLeft3;
    [HideInInspector]
    public Transform sensorRight1;
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
    public Transform handlerLeft;
    [HideInInspector]
    public Vector2 handlerLeftSize;
    [HideInInspector]
    public Transform handlerRight;
    [HideInInspector]
    public Vector2 handlerRightSize;

    [HideInInspector]
    public Transform handlerBellyLeft;
    [HideInInspector]
    public Vector2 handlerBellyLeftSize;
    [HideInInspector]
    public Transform handlerBellyRight;
    [HideInInspector]
    public Vector2 handlerBellyRightSize;

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

    Vector3 cameraTargetOffset = new Vector3();
    Vector3 targetCameraTargetOffset = new Vector3();
    //CameraTargetOffseter camTargetOffseter = null;
    float camTargetToOffsetSpeed = 0f;

    public void CameraOffsetIn(CameraTargetOffseter cto)
    {
        //camTargetOffseter = cto;

        targetCameraTargetOffset = cto.CameraOffset;
        camTargetToOffsetSpeed = cto.ToOffsetSpeedInOut.x;
    }
    public void CameraOffsetOut(CameraTargetOffseter cto)
    {
        //this.RLHAssert(camTargetOffseter, "nie bylem a wyszedlem");
        //this.RLHAssert(cto == camTargetOffseter, "wychodze z innego niz wszedlem. pewnie sie zazebiaja");
        //camTargetOffseter = null;

        targetCameraTargetOffset = new Vector3(0f, 0f, 0f);
        camTargetToOffsetSpeed = cto.ToOffsetSpeedInOut.y;
    }
    void UpdateCameraOffset()
    {
        if (camTargetToOffsetSpeed == 0f) return;
        //if (!camTargetOffseter) return;

        //Vector3 _lastCamTargetOffset = cameraTargetOffset;
        Vector3 _distToMoveV = targetCameraTargetOffset - cameraTargetOffset;
        float _distToMove = _distToMoveV.magnitude;

        float _posMoveDist = Time.deltaTime * camTargetToOffsetSpeed;
        if( _posMoveDist > _distToMove )
        {
            cameraTargetOffset = targetCameraTargetOffset;
            camTargetToOffsetSpeed = 0f;
        }
        else
        {
            cameraTargetOffset += ( _distToMoveV.normalized * _posMoveDist );
        }
    } 

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

    GameObject mountAttackLeftCollider;
    GameObject mountAttackRightCollider;
    public GameObject MountAttackLeftCollider
    {
        get
        {
            return mountAttackLeftCollider;
        }
    }
    public GameObject MountAttackRightCollider
    {
        get
        {
            return mountAttackRightCollider;
        }
    }

    GameObject knifeAttackLeftHighCollider;
    GameObject knifeAttackRightHighCollider;
    GameObject knifeAttackLeftLowCollider;
    GameObject knifeAttackRightLowCollider;

    public GameObject KnifeAttackLeftHighCollider
    {
        get
        {
            return knifeAttackLeftHighCollider;
        }
    }
    public GameObject KnifeAttackRightHighCollider
    {
        get
        {
            return knifeAttackRightHighCollider;
        }
    }

    public GameObject KnifeAttackLeftLowCollider
    {
        get
        {
            return knifeAttackLeftLowCollider;
        }
    }

    public GameObject KnifeAttackRightLowCollider
    {
        get
        {
            return knifeAttackRightLowCollider;
        }
    }

    RLHScene rlhScene = null;

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

    public BoxCollider2D Coll
    {
        get
        {
            return coll;
        }
    }

    public PolygonCollider2D GfxCollider
    {
        get
        {
            return gfxCollider;
        }
    }

    public RLHScene RlhScene
    {
        get
        {
            return rlhScene;
        }
    }

    public AudioSource MyAudioSourceLooped
    {
        get
        {
            return myAudioSourceLooped;
        }
    }

    public float MyEnergy
    {
        get
        {
            return myEnergy;
        }

        set
        {
            myEnergy = value;
        }
    }

    public float MyMaxEnergy
    {
        get
        {
            return myMaxEnergy;
        }

        set
        {
            myMaxEnergy = value;
        }
    }

    public Bat SuckedMeBat
    {
        get
        {
            return suckedMeBat;
        }

        set
        {
            suckedMeBat = value;
        }
    }

    public Collider2D TrySetIgnoreCollisionWhit(GameObject stone)
    {
        if (!stone.GetComponent<GroundMoveable>()) return null;

        Collider2D stoneCollider = stone.GetComponent<Collider2D>();
        //if (!stoneCollider) return stoneCollider;

        LastIgnoredCollider = stoneCollider;

        Physics2D.IgnoreCollision(gfxCollider, lastIgnoredCollider, true);

        return LastIgnoredCollider;
    }

    public bool removeLastIgnoredCollision()
    {
        if( LastIgnoredCollider )
        {
            Physics2D.IgnoreCollision(gfxCollider, lastIgnoredCollider, false);
            return true;
        }

        return false;
    }

    public Collider2D LastIgnoredCollider
    {
        get
        {
            return lastIgnoredCollider;
        }

        set
        {
            lastIgnoredCollider = value;
        }
    }

    public float GroundUnderAngle
    {
        get
        {
            return groundUnderAngle;
        }
    }

    public Transform Gfx
    {
        get
        {
            return gfx;
        }
    }

    public float CurrentStateTime
    {
        get { return currentStateTime; }
    }

    public Vector3 CameraTargetOffset
    {
        get
        {
            return cameraTargetOffset;
        }

        set
        {
            cameraTargetOffset = value;
        }
    }

    public bool Paused
    {
        get
        {
            return paused;
        }
    }

    

    //public CameraTargetOffseter CamTargetOffseter
    //{
    //    get
    //    {
    //        return camTargetOffseter;
    //    }

    //    set
    //    {
    //        camTargetOffseter = value;
    //    }
    //}

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
    public void resetCurrentActionTime()
    {
        currentActionTime = 0f;
    }
    float currentStateTime = 0.0f;
    //public float getCurrentStateTime()
    //{
    //    return currentStateTime;
    //}

    [HideInInspector]
    public float myWidth;
    float myHalfWidth;

    public float getMyWidth()
    {
        return myWidth;
    }
    public float getMyHalfWidth()
    {
        return myHalfWidth;
    }
    //float myHeight;
    //float myHalfHeight;

    [HideInInspector]
    public int layerIdGroundMask;
    [HideInInspector]
    public int layerIdGroundFarMask;
    [HideInInspector]
    public int layerIdGroundMoveableMask;
    [HideInInspector]
    public int layerIdDraggedGroundMoveableMask;
    [HideInInspector]
    public int layerIdGroundAllMask;
    public int layerIdGroundAllFullMask;
    [HideInInspector]
    public int layerIdGroundHandlesMask;
    [HideInInspector]
    public int layerIdRopesMask;
    [HideInInspector]
    public int layerIdMountMask;

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
    public int climbingWallID = -1;
    float groundUnderAngle = 0f;


    Collider2D lastIgnoredCollider = null;

}
