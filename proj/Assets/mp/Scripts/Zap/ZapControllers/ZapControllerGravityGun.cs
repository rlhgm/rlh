using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class ZapControllerGravityGun : ZapController
{
    public float WalkSpeed = 1.5f;
    public float WalkBackSpeed = 1.5f;

    public float rollSpeed = 4.8f;
    public float rollDuration = 0.6f;
    public float rollMaxDist = 3f;

    public float GravityForce = -20.0f;
    public float MaxSpeedY = 15.0f;

    public float SpeedUpParam = 10.0f; // ile jednosek predkosci hamuje na sekund
    public float SlowDownParam = 20.0f; // ile jednosek predkosci hamuje na sekunde

    public float TURN_LEFTRIGHT_DURATION = 0.33f;
    public float ATTACK_DURATION = 0.5f;
    public float PULLOUT_GRAVITYGUN_DURATION = 0.3f;
    public float HIDE_GRAVITYGUN_DURATION = 0.35f;
    public float CROUCHINOUT_DURATION = 0.1f;

    public float RotateMaxStoneMass = 30f;
    public float RotateMassInteriaCoef = 300f;

    public float BeamSpeed = 50f;
    public float MissedBeamDuration = 10f;

    public float CenterOnBeamSpeed = 10f;

    public Transform draggedStone = null;
    GroundMoveable draggedStoneMoveable = null;
    public Transform lastFlashStone = null;
    
    Vector2 T;          // sila ciagu
    public float inertiaFactor = 0.09f;         // wspolczynnik oporu - u mnie raczej bezwladnosci
                                                //public float inertiaFactor2 = 0.03f; 	// wspolczynnik bezwladnosci jak gracz na siebie chce skierowac kamien
    public float maxDistance = 8f;
    
    Vector2 V;          // predkosc
    public static float userStoneRotateSpeed = 180f;

    public AudioClip beginShootSound = null;
    public AudioClip finishShootSound = null;
    public AudioClip shootSound = null;
    
    public override void setZap(Zap playerController)
    {
        base.setZap(playerController);
        if (zap.weaponMenu)
        {
            weaponMenuItem = zap.weaponMenu.itemGravityGun;
        }
        if (weaponMenuItem)
        {
            weaponMenuItem.setState(WeaponMenuItem.State.OFF);
        }

        staticInit();
    }

    float distToMove;
    Vector3 oldPos;
    float newPosX;
    RaycastHit2D hit;

    public void leftMouseNotPressed()
    {
        if (zap.isDead())
            return;

        if (draggedStone == null)
        {
            Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rayOrigin = zapTargeterPos; // zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
            hit = Physics2D.Linecast(rayOrigin, mouseInScene, zap.layerIdGroundAllMask);
            if (hit.collider)
            {
                if (hit.collider.GetComponent<GroundMoveable>())
                {
                    if (hit.distance < maxDistance)
                    {
                        if (lastFlashStone != hit.collider.transform)
                        {
                            if (lastFlashStone)
                            {
                                unflashStone(lastFlashStone);
                            }

                            lastFlashStone = hit.collider.transform;
                            flashStone(lastFlashStone);
                        }
                    }
                }
            }
            else
            {
                if (lastFlashStone)
                {
                    unflashStone(lastFlashStone);
                    lastFlashStone = null;
                }
            }
        }
    }

    void leftMouseButtonClicked()
    {
        if (zap.isDead())
            return;

        draggedStone = null;
        draggedStoneMoveable = null;

        shooting = true;
        beamMelting = false;
        beamMissed = false;
        shootingDuration = 0f;
        zap.GravityGunBeam.gameObject.SetActive(true);

        //zap.playSound(beginShootSound);
    }
    
    void stopShoot()
    {
        if (shooting)
        {
            //zap.playSound(finishShootSound);
            //zap.stopSoundLooped(shootSound);
        }

        shooting = false;
        //zap.GravityGunBeam.gameObject.SetActive(false);
        releaseStone();
        beamMelting = true;
        beamMeltingDuration = 0f;
        
        
    }

    bool beamMelting = false;
    float beamMeltingDuration = 0f;
    //Vector2 beamMeltOrigin = new Vector2();
    //Vector2 beamMeltTarget = new Vector2();

    // grafika
    Vector2 beamOriginOK_2 = new Vector2();
    Vector2 beamTargetOK_2 = new Vector2();
    Vector2 beamDiffOK_2 = new Vector2();
    // fizyka
    //Vector2 rayOriginOK_2 = new Vector2();
    //Vector2 rayTargetOK_2 = new Vector2();

    bool beamMissed = false;
    float beamMissedDuration = 0f;

    Vector2 draggedStoneHitPos = new Vector2();
    bool draggedStoneCentered = false;

    void beamMelt()
    {
        if (!beamMelting) return;

        //Debug.Log("beamMelt");

        beamMeltingDuration += currentDeltaTime;

        LineRenderer beam = zap.GravityGunBeam.GetComponent<LineRenderer>();
        Vector2 beamAll = beamOriginOK_2 - beamTargetOK_2;
        Vector2 beamNorm = beamAll.normalized;
        float beamLengthFromTime = beamMeltingDuration * BeamSpeed;
        float beamLengthMax = beamAll.magnitude;
        float beamLength = beamLengthFromTime;
        if (beamLength > beamLengthMax)
        {
            beamLength = 0;
            //stopShoot();
            beamMelting = false;
            zap.GravityGunBeam.gameObject.SetActive(false);
            return;
        }
        else
        {
            beamLength = beamLengthMax - beamLengthFromTime;
        }
        Vector2 beamOrigin = beamTargetOK_2 + (beamNorm * beamLength);
        
        beam.SetPosition(0, beamOrigin);
    }

    float currentDeltaTime = 0f;
    public override void MUpdate(float deltaTime, bool firstUpdate)
    {
        firstUpdateInFrame = firstUpdate;

        staticInit();

        zapTargeterPos = zap.Targeter.position;

        currentDeltaTime = deltaTime;

        oldPos = transform.position;
        newPosX = oldPos.x;
        distToMove = 0.0f;
        
        beamMelt();

        if (!Input.GetMouseButton(0))
        {
            leftMouseNotPressed();
        }

        if (Input.GetMouseButtonDown(0))
        {
            leftMouseButtonClicked();
        }

        if (Input.GetMouseButtonUp(0))
        {
            stopShoot();
        }

        switch (action)
        {
            case Action.IDLE:
                if (Action_IDLE() != 0)
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
                if (Input.GetKeyDown(zap.keyJump))
                {
                    wantJumpAfter = true;
                }
                if (zap.currentActionTime >= TURN_LEFTRIGHT_DURATION)
                {
                    zap.turnLeft();
                    turnLeftFinish();
                }
                break;

            case Action.TURN_STAND_RIGHT:
                if (Input.GetKeyDown(zap.keyJump))
                {
                    wantJumpAfter = true;
                }
                if (zap.currentActionTime >= TURN_LEFTRIGHT_DURATION)
                {
                    zap.turnRight();
                    turnRightFinish();
                }
                break;
        }

        switch (zap.getState())
        {

            case Zap.State.ON_GROUND:
                float distToGround = 0.0f;
                zap.checkGround(ref distToGround);
                if (zap.groundUnder == null)
                {
                    zap.suddenlyInAir();
                }
                else if (zap.groundUnder)
                {
                    if (distToGround != 0f)
                    {
                        transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
                        zap.touchStone(zap.groundUnder);
                    }
                    else
                    {
                        zap.touchStone(zap.groundUnder);
                    }
                }

                break;

        };

        zap.lastVelocity = zap.velocity;

    }

    Vector3 zapTargeterPos;

    public override void FUpdate(float fDeltaTime)
    {
        zapTargeterPos = zap.Targeter.position;

        if (draggedStone && !draggedStoneMoveable.IsHanging())
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) ||
               Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X))
            {

                Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    //rb.angularVelocity = 0;
                }
            }

            if (Input.GetKey(KeyCode.Z))
            { // obracam kamien w lewo ...

                Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    if (rb.angularVelocity < 180)
                    {
                        float velocityGrowth = fDeltaTime * userStoneRotateSpeed;
                        float massInteriaCoef = (rb.mass / RotateMaxStoneMass) * RotateMassInteriaCoef * fDeltaTime;
                        if (massInteriaCoef > velocityGrowth) massInteriaCoef = velocityGrowth;
                        rb.angularVelocity += ( velocityGrowth - massInteriaCoef);
                    }
                    rb.angularVelocity = Mathf.Min(rb.angularVelocity, 180f);
                }
            }
            else if (Input.GetKey(KeyCode.X))
            { // albo w prawo

                Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    if (rb.angularVelocity > -180)
                    {
                        float velocityGrowth = fDeltaTime * userStoneRotateSpeed;
                        float massInteriaCoef = (rb.mass / RotateMaxStoneMass) * RotateMassInteriaCoef * fDeltaTime;
                        if (massInteriaCoef > velocityGrowth) massInteriaCoef = velocityGrowth;
                        rb.angularVelocity -= (velocityGrowth - massInteriaCoef);
                    }
                    rb.angularVelocity = Mathf.Max(rb.angularVelocity, -180f);
                }

            }
        }

        Vector3 currentMousePosition = Input.mousePosition;
        
        if (Input.GetMouseButton(0))
        {
            Vector3 touchInScene = touchCamera.ScreenToWorldPoint(currentMousePosition);
            Vector2 tis = touchInScene;

            if (draggedStone)
            {
                if (!draggedStoneMoveable.IsHanging())
                {
                    Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
                    if (rb)
                    {
                        bool tlc = true;

                        Vector2 playerCenterPos = zap.transform.position;
                        playerCenterPos.y += 1f;
                        Vector2 stoneCenterPos = rb.worldCenterOfMass;
                        if (!draggedStoneCentered)
                        {
                            Vector2 toCenter = rb.centerOfMass - draggedStoneHitPos;
                            float toCenterDist = toCenter.magnitude;
                            float fromTimeShiftDist = fDeltaTime * CenterOnBeamSpeed;
                            bool centerApprovedIfSuccess = false;
                            Vector2 newDraggedStoneHitPos;
                            if (fromTimeShiftDist > toCenterDist)
                            {
                                newDraggedStoneHitPos = rb.centerOfMass;
                                fromTimeShiftDist = toCenterDist;
                                centerApprovedIfSuccess = true;
                            }
                            else
                            {
                                newDraggedStoneHitPos = draggedStoneHitPos + (toCenter.normalized * fromTimeShiftDist);
                            }

                            Vector3 hitInWorld = draggedStone.TransformPoint(newDraggedStoneHitPos);

                            if (maxDistance > Vector3.Distance(hitInWorld, zapTargeterPos))
                            {
                                hit = Physics2D.Linecast(zapTargeterPos, hitInWorld, zap.layerIdGroundAllMask);
                                if (hit.collider)
                                {
                                    if (hit.collider.transform == draggedStone)
                                    {
                                        if (hit.distance < maxDistance)
                                        {
                                            draggedStoneHitPos = newDraggedStoneHitPos;
                                            tlc = false;        // aby nie robic tego samego ponizej jezeli tu wynik byl pozytywny
                                            if (centerApprovedIfSuccess)
                                                draggedStoneCentered = true;
                                        }
                                    }
                                }
                            }

                            stoneCenterPos = draggedStone.TransformPoint(draggedStoneHitPos);
                        }

                        T = (tis - stoneCenterPos);
                        V = rb.velocity;
                        Vector2 F = T - (inertiaFactor * V);

                        rb.AddForce(F, ForceMode2D.Impulse);

                        if (!canBeDragged(draggedStone, tis, tlc))
                        {
                            releaseStone();
                        }
                        else
                        {
                            if (draggedStone.childCount == 1)
                            {
                                SpriteRenderer sprRend = draggedStone.GetComponentInChildren<SpriteRenderer>();
                                if (sprRend)
                                {
                                    //_speedX("speedX", Range(0, 0.3)) = 0
                                    //_speedY("speedY", Range(0, 0.3)) = 0
                                    float rbvx = Mathf.Abs(rb.velocity.x);
                                    float rbvy = Mathf.Abs(rb.velocity.y);

                                    float _speedX = Mathf.Clamp((Mathf.Min(4.0f, rbvx) / 4.0f) * 0.2f, 0.0f, 0.2f);
                                    float _speedY = Mathf.Clamp((Mathf.Min(4.0f, rbvy) / 4.0f) * 0.2f, 0.0f, 0.2f);


                                    float _fav = Mathf.Abs(rb.angularVelocity / 180f) * 0.2f;
                                    _speedX += _fav;
                                    _speedY += _fav;

                                    sprRend.material.SetFloat("_speedX", _speedX);
                                    sprRend.material.SetFloat("_speedY", _speedY);

                                    sprRend.material.SetFloat("_rpx", draggedStoneHitPos.x);
                                    sprRend.material.SetFloat("_rpy", draggedStoneHitPos.y);

                                    draggedDuration += fDeltaTime;
                                    sprRend.material.SetFloat("_draggedDuration", draggedDuration);
                                    //Debug.Log(rbv + " " + _speedX + " " +_speedY);

                                    //zap.MyAudioSourceLooped.pitch = _speedX + _speedY;
                                }
                            }
                        }
                    }
                }
                else
                {
                    draggedStoneMoveable.TryToBreakOff(tis);
                }
            }
        }
    }

    public override void activateSpec(bool restore = false, bool crouch = false)
    {
        //base.activate ();
        //setAction (Action.IDLE);
        zap.GfxLegs.gameObject.SetActive(false);
        zap.GravityGunBeam.gameObject.SetActive(false);
        setAction(Action.PULLOUT_GRAVITYGUN);
        desiredSpeedX = 0.0f;
        shooting = false;
    }
    public override void deactivate()
    {
        base.deactivate();
        stopShoot();
    }

    public override bool tryDeactiveate()
    {
        if (isInAction(Action.IDLE))
        {
            setAction(Action.HIDE_GRAVITYGUN);
            return true;
        }
        return false;
    }

    public override bool isDodging()
    {
        //if(
        return isInAction(Action.ROLL_LEFT_FRONT) ||
            isInAction(Action.ROLL_LEFT_BACK) ||
            isInAction(Action.ROLL_RIGHT_FRONT) ||
            isInAction(Action.ROLL_RIGHT_BACK);
        //)
    }

    bool shooting = false;
    float shootingDuration = 0f;

    public enum Action
    {
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
        //JUMP,
        ROLL_LEFT_FRONT,
        ROLL_LEFT_BACK,
        ROLL_RIGHT_FRONT,
        ROLL_RIGHT_BACK,
        FALL,
        STOP_WALK,
        //STOP_RUN,
        DIE
    };

    Action getAction()
    {
        return action;
    }

    int getIndexOfAngle(Vector2 from, Vector2 to, int numberOfAnimations = 8)
    {
        //Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 rayOrigin = zap.Targeter.position; // zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
        Vector3 df = to - from; //mouseInScene - rayOrigin;
        float deg = Mathf.Rad2Deg * Mathf.Atan2(df.y, df.x);

        if (zap.faceLeft())
        {
            if (deg > 0)
            {
                deg = 180f - deg;
            }
            else
            {
                deg = -180f - deg;
            }
        }

        float oneAnimRange = 180.0f / (float)numberOfAnimations;
        deg += (90f - oneAnimRange*0.5f);
        deg = Mathf.Clamp(deg, 0f, 180f);
        
        //return (int)deg;
        return Mathf.Min( (int)(deg / oneAnimRange), numberOfAnimations-1 );
    }

    //int getIndexOfAngle2(Vector2 from, Vector2 to, int numberOfAnimations = 8)
    //{
    //    //Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
    //    //Vector2 rayOrigin = zap.Targeter.position; // zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
    //    Vector3 df = to - from; //mouseInScene - rayOrigin;
    //    float deg = Mathf.Rad2Deg * Mathf.Atan2(df.y, df.x);

    //    if (zap.faceLeft())
    //    {
    //        if (deg > 0)
    //        {
    //            deg = 180f - deg;
    //        }
    //        else
    //        {
    //            deg = -180f - deg;
    //        }
    //    }

    //    float oneAnimRange = 180.0f / (float)numberOfAnimations;
    //    deg += (90f - oneAnimRange * 0.5f);
    //    deg = Mathf.Clamp(deg, 0f, 180f);

    //    //return (int)deg;
    //    return Mathf.Min((int)(deg / oneAnimRange), numberOfAnimations - 1);
    //}

    static Vector2[] beamOrigins;
    static bool staticInitialized = false;
    static void staticInit()
    {
        if (staticInitialized) return;

        beamOrigins = new Vector2[8];

        beamOrigins[0] = new Vector2(0.156f, 0.614f);
        beamOrigins[1] = new Vector2(0.313f, 0.812f);
        beamOrigins[2] = new Vector2(0.204f, 0.812f);
        beamOrigins[3] = new Vector2(0.409f, 1.038f);
        beamOrigins[4] = new Vector2(0.392f, 1.362f);
        beamOrigins[5] = new Vector2(0.392f, 1.558f);
        beamOrigins[6] = new Vector2(0.520f, 1.618f);
        beamOrigins[7] = new Vector2(0.179f, 1.874f);

        staticInitialized = true;
    }

    void trackCursor(Action act, bool shoot)
    {
        Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 angleFrom = zapTargeterPos;
        //Vector2 angleTo = mouseInScene;

        bool cursorTooClose = false;
        int indexOfAngle = 0; // = getIndexOfAngle(angleFrom, angleTo);

        if (shoot)
        {
            leftMouseNotPressed();

            shootingDuration += currentDeltaTime;

            LineRenderer beam = zap.GravityGunBeam.GetComponent<LineRenderer>();

            //if (zap.faceLeft()) beamOriginOK_2.x *= -1f;
            //beamOriginOK_2 = transform.TransformPoint(beamOriginOK_2);
            //beamOriginOK_2 = transform.TransformPoint(beamOriginOK_2);

            float beamLength;
            if (draggedStone != null)
            {
                beamTargetOK_2 = draggedStone.TransformPoint(draggedStoneHitPos);

                Vector2 _tmpZTP = zapTargeterPos;
                if (Vector2.Distance(_tmpZTP, beamTargetOK_2) > 0.5f)
                {
                    indexOfAngle = getIndexOfAngle(zapTargeterPos, beamTargetOK_2);
                    beamOriginOK_2 = beamOrigins[indexOfAngle];
                    if (zap.faceLeft()) beamOriginOK_2.x *= -1f;
                    beamOriginOK_2 = transform.TransformPoint(beamOriginOK_2);
                }
                else
                {
                    cursorTooClose = true;
                }
                beamDiffOK_2 = beamTargetOK_2 - beamOriginOK_2;
                beamLength = beamDiffOK_2.magnitude;
            }
            else
            {
                Vector2 _tmpZTP = zapTargeterPos;
                if (Vector2.Distance(_tmpZTP, mouseInScene) > 0.5f)
                {
                    indexOfAngle = getIndexOfAngle(zapTargeterPos, mouseInScene);
                    beamOriginOK_2 = beamOrigins[indexOfAngle];
                    if (zap.faceLeft()) beamOriginOK_2.x *= -1f;
                    beamOriginOK_2 = transform.TransformPoint(beamOriginOK_2);
                }
                else
                {
                    cursorTooClose = true;
                }

                Vector2 ztp = new Vector2(zapTargeterPos.x, zapTargeterPos.y);
                Vector2 beamAll = mouseInScene - ztp;
                Vector2 beamNorm = beamAll.normalized;
                float beamLengthFromTime = shootingDuration * BeamSpeed;
                float beamLengthMax = beamAll.magnitude;
                beamLength = beamLengthFromTime;
                if (beamLength > beamLengthMax)
                {
                    beamLength = beamLengthMax;
                    if (!beamMissed)
                    {
                        beamMissed = true;
                        beamMissedDuration = 0f;
                    }
                    beamMissedDuration += currentDeltaTime;
                    if (beamMissedDuration >= MissedBeamDuration)
                    {
                        stopShoot();
                    }
                }
                if (beamLength > maxDistance)
                {
                    beamLength = maxDistance;
                }

                beamTargetOK_2 = ztp + (beamNorm * beamLength);

                hit = Physics2D.Linecast(zapTargeterPos, beamTargetOK_2, zap.layerIdGroundAllMask);
                if (hit.collider)
                {
                    GroundMoveable newDraggedGroundMoveable = hit.collider.GetComponent<GroundMoveable>();
                    if (newDraggedGroundMoveable)
                    {
                        //zap.playSoundLooped(shootSound);

                        draggedStone = hit.collider.transform;
                        draggedStoneMoveable = draggedStone.GetComponent<GroundMoveable>();

                        Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
                        tsrb.gravityScale = 0f;
                        flashStone2(draggedStone);

                        //Physics2D.IgnoreCollision()

                        draggedStone.gameObject.layer = LayerMask.NameToLayer("DraggedGroundMoveable"); //zap.layerIdDraggedGroundMoveableMask;

                        draggedStoneHitPos = draggedStone.InverseTransformPoint(hit.point);
                        draggedStoneCentered = false;
                    }
                    else
                    {
                        stopShoot();
                    }
                }
            }

            //beam.lay
            //Vector2.beamDiffOK_2
            beamDiffOK_2 = beamTargetOK_2 - beamOriginOK_2;
            float beamGranularity = 0.2f;
            int numBundles = 4;
            int numBeamVertices = (int)(beamLength / beamGranularity);
            //Debug.Log(numBeamVertices);
            beam.SetVertexCount(numBeamVertices * numBundles);
            Vector2 bdNor = beamDiffOK_2.normalized;
            //Debug.Log(" " + beamDiffOK_2  + " " +  bdNor);
            //Vector3.RotateTowards()

            Vector2 bdNorPrepend = bdNor.Rotate(90);
            bool bundlesFocused = draggedStone != null;
            float bundlesFocusedFocus = 0.3f;

            for (int bb = 0; bb < numBundles; ++bb)
            {
                if (bb % 2 == 0)
                {
                    beam.SetPosition(bb * numBeamVertices, beamOriginOK_2);

                    for (int ib = 1; ib < numBeamVertices - 1; ++ib)
                    {
                        Vector2 ibPos = beamOriginOK_2 + bdNor * ib * beamGranularity;

                        Vector2 _dr = bdNorPrepend;

                        //ibPos.x += Mathf.PerlinNoise(ibPos.x, ibPos.y);
                        //ibPos.y += Mathf.PerlinNoise(ibPos.x, ibPos.y);

                        //ibPos.x += Mathf.PerlinNoise(ibPos.x*Time.time, ibPos.y) * beamGranularity;
                        //ibPos.y += Mathf.PerlinNoise(ibPos.x, ibPos.y*Time.time) * beamGranularity;

                        //ibPos.y += Mathf.PerlinNoise(ibPos.x, ibPos.y * Time.time) ;

                        _dr.y *= (Mathf.PerlinNoise(ibPos.x + shootingDuration * bb, ibPos.y + shootingDuration * bb) * 2f - 1f) * (bundlesFocused ? bundlesFocusedFocus : 1f);
                        //_dr.y *= (Mathf.PerlinNoise(Random.Range(0f,1f), Random.Range(0f, 1f)) * 2f - 1f) * (bundlesFocused ? bundlesFocusedFocus : 1f);
                        //_dr.y *= (Mathf.PerlinNoise(shootingDuration * bb, shootingDuration * bb) * 2f - 1f) * (bundlesFocused ? bundlesFocusedFocus : 1f);

                        ibPos += _dr;

                        beam.SetPosition(bb * numBeamVertices + ib, ibPos);
                    }

                    beam.SetPosition((bb + 1) * numBeamVertices - 1, beamTargetOK_2);
                }
                else
                {
                    beam.SetPosition(bb * numBeamVertices, beamTargetOK_2);

                    for (int ib = 1; ib < numBeamVertices - 1; ++ib)
                    {
                        Vector2 ibPos = beamTargetOK_2 - bdNor * ib * beamGranularity;

                        Vector2 _dr = bdNorPrepend;

                        _dr.y *= (Mathf.PerlinNoise(ibPos.y + shootingDuration * bb, ibPos.x + shootingDuration * bb) * 2f - 1f) * (bundlesFocused ? bundlesFocusedFocus : 1f);
                        //_dr.y *= (Mathf.PerlinNoise(Random.Range(0f, 1f), Random.Range(0f, 1f)) * 2f - 1f) * (bundlesFocused ? bundlesFocusedFocus : 1f);

                        ibPos += _dr;

                        beam.SetPosition(bb * numBeamVertices + ib, ibPos);
                    }

                    beam.SetPosition((bb + 1) * numBeamVertices - 1, beamOriginOK_2);
                }
            }

            //beamTargetColor.a = 1f - (beamLength / maxDistance);
            //beam.SetColors(beamOriginColor, beamTargetColor);
            //beam.SetWidth(0.1f, 0.5f * (beamLength / maxDistance));
            //beamOriginColor = new Color(1f, 1f, 1f, 1f);
            //beamTargetColor = new Color(1f, 1f, 1f, 1f);
            beam.SetColors(beamOriginColor, beamTargetColor);
            beam.SetWidth(0.1f, 0.1f);
        }
        else
        {
            Vector2 _tmpZTP = zapTargeterPos;
            if (Vector2.Distance(_tmpZTP, mouseInScene) > 0.5f)
            {
                indexOfAngle = getIndexOfAngle(zapTargeterPos, mouseInScene);
            }
            else
            {
                cursorTooClose = true;
            }
        }

        if (!cursorTooClose)
        {
            switch (act)
            {
                case Action.IDLE:
                case Action.WALK_LEFT:
                case Action.WALK_RIGHT:
                case Action.WALKBACK_LEFT:
                case Action.WALKBACK_RIGHT:
                    if (indexOfAngle < 0) break;
                    if (shoot)
                    {
                        zap.AnimatorBody.Play("Zap_body_fire_GG_" + indexOfAngle);
                    }
                    else
                    {
                        zap.AnimatorBody.Play("Zap_body_walk_GG_" + indexOfAngle);
                    }
                    break;
            }
        }
    }

    //Color beamOriginColor = new Color(0f, 23f / 255f, 1f, 200f / 255f);
    //Color beamTargetColor = new Color(0f, 23f / 255f, 1f, 200f / 255f);
    Color beamOriginColor = new Color(1f, 1f, 1f, 1f);
    Color beamTargetColor = new Color(1f, 1f, 1f, 1f);

    bool setAction(Action newAction, int param = 0)
    {

        if (action == newAction)
            return false;

        action = newAction;
        zap.resetCurrentActionTime();

        zap.AnimatorBody.speed = 1f;
        zap.AnimatorLegs.speed = 1f;
        zap.GfxLegs.gameObject.SetActive(false);

        switch (newAction)
        {

            case Action.IDLE:
                zap.GfxLegs.gameObject.SetActive(true);
                if (zap.faceRight())
                {
                    zap.AnimatorBody.Play("Zap_body_walk_GG_3");
                    zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                }
                else
                {
                    zap.AnimatorBody.Play("Zap_body_walk_GG_3");
                    zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                }
                zap.AnimatorLegs.speed = 0f;
                //if( !shooting )
                //    zap.AnimatorBody.speed = 0f;
                break;

            case Action.PULLOUT_GRAVITYGUN:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_knife_pull");
                else zap.AnimatorBody.Play("Zap_knife_pull");
                break;

            case Action.HIDE_GRAVITYGUN:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_knife_hide");
                else zap.AnimatorBody.Play("Zap_knife_hide");
                break;

            case Action.DIE:
                Zap.DeathType dt = (Zap.DeathType)param;
                string msgInfo = "";

                switch (dt)
                {

                    case Zap.DeathType.STONE_HIT:
                        if (zap.faceRight()) zap.AnimatorBody.Play("Zap_death_stonehit_R");
                        else zap.AnimatorBody.Play("Zap_death_stonehit_L");
                        msgInfo = zap.DeathByStoneHitText;
                        break;

                    case Zap.DeathType.DRAGGED_STONE_HIT:
                        if (zap.faceRight()) zap.AnimatorBody.Play("Zap_death_stonehit_R");
                        else zap.AnimatorBody.Play("Zap_death_stonehit_L");
                        msgInfo = zap.DeathByDraggedStoneHitText;
                        break;

                    case Zap.DeathType.VERY_HARD_LANDING:
                        if (zap.faceRight()) zap.AnimatorBody.Play("Zap_death_hitground_R");
                        else zap.AnimatorBody.Play("Zap_death_hitground_L");
                        msgInfo = zap.DeathByVeryHardLandingText;
                        break;

                    case Zap.DeathType.SNAKE:
                        if (zap.faceRight()) zap.AnimatorBody.Play("Zap_death_poison_R");
                        else zap.AnimatorBody.Play("Zap_death_poison_L");
                        msgInfo = zap.DeathBySnakeText;
                        break;

                    case Zap.DeathType.POISON:
                        if (zap.faceRight()) zap.AnimatorBody.Play("Zap_death_poison_R");
                        else zap.AnimatorBody.Play("Zap_death_poison_L");
                        msgInfo = zap.DeathByPoisonText;
                        break;

                    case Zap.DeathType.PANTHER:
                        if (zap.faceRight()) zap.AnimatorBody.Play("Zap_death_panther");
                        else zap.AnimatorBody.Play("Zap_death_panther");
                        msgInfo = zap.DeathByPantherText;
                        break;

                    case Zap.DeathType.CROCODILE:
                        msgInfo = zap.DeathByCrocodileText;
                        break;

                    default:
                        if (zap.faceRight()) zap.AnimatorBody.Play("Zap_death_hitground_R");
                        else zap.AnimatorBody.Play("Zap_death_hitground_L");
                        msgInfo = zap.DeathByDefaultText;
                        break;

                };

                zap.showInfo(msgInfo, -1);

                if (zap.dieSounds.Length != 0)
                    zap.playSound(zap.dieSounds[Random.Range(0, zap.dieSounds.Length)]);
                break;

            case Action.WALK_LEFT:
                //zap.AnimatorBody.Play("Zap_knife_walk");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_body_walk_GG_3");
                zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                //zap.AnimatorLegs.speed = 0f;
                //zap.AnimatorBody.speed = 0f;
                break;
            case Action.WALK_RIGHT:
                //zap.AnimatorBody.Play("Zap_knife_walk");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_body_walk_GG_3");
                zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                break;

            case Action.WALKBACK_LEFT:
                //zap.AnimatorBody.Play("Zap_knife_walkback");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_body_walk_GG_3");
                zap.AnimatorLegs.Play("Zap_gg_legs_walkback");
                break;
            case Action.WALKBACK_RIGHT:
                //zap.AnimatorBody.Play("Zap_knife_walkback");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_body_walk_GG_3");
                zap.AnimatorLegs.Play("Zap_gg_legs_walkback");
                break;

            case Action.TURN_STAND_LEFT:
                //zap.AnimatorBody.Play("Zap_knife_turnleft");
                //getIndexOfAngle(zapTargeterPos,mouse)
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_body_walk_turnback_GG_"+param);
                zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                wantJumpAfter = false;
                break;

            case Action.TURN_STAND_RIGHT:
                //zap.AnimatorBody.Play("Zap_knife_turnright");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_body_walk_turnback_GG_"+param);
                zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                wantJumpAfter = false;
                break;

            case Action.ROLL_LEFT_FRONT:
                zap.AnimatorBody.Play("Zap_knife_crouch_tumblefront");
                break;

            case Action.ROLL_LEFT_BACK:
                zap.AnimatorBody.Play("Zap_knife_crouch_tumbleback");
                break;

            case Action.ROLL_RIGHT_FRONT:
                zap.AnimatorBody.Play("Zap_knife_crouch_tumblefront");
                break;

            case Action.ROLL_RIGHT_BACK:
                zap.AnimatorBody.Play("Zap_knife_crouch_tumbleback");
                break;
        };

        return true;
    }
    bool isInAction(Action test)
    {
        return action == test;
    }
    bool isNotInAction(Action test)
    {
        return action != test;
    }

    public override int keyUpDown()
    {
        if (isInState(Zap.State.ON_GROUND))
        {
        }
        return 0;
    }

    public override int keyUpUp()
    {
        return 0;
    }

    public override int keyDownDown()
    {
        return 0;
    }

    public override int keyDownUp()
    {
        return 0;
    }

    public override int keyRunDown()
    {
        return 0;
    }

    public override int keyRunUp()
    {
        return 0;
    }

    public override int keyLeftDown()
    {
        if ((isInAction(Action.IDLE) || moving(-1) || jumping()) && isInState(Zap.State.ON_GROUND))
        {
            if (zap.CheckLeft(0.1f) >= 0.0f)
            {
                if (zap.dir() == Vector2.right)
                {
                    //turnLeftStart();
                }
                return 0;
            }

            if (zap.dir() == -Vector2.right)
            {
                desiredSpeedX = WalkSpeed;
                speedLimiter(-1, desiredSpeedX + 1.0f);
                setAction(Action.WALK_LEFT);
                return 1;

            }
            else
            {
                //turnLeftStart();
                desiredSpeedX = WalkBackSpeed;
                speedLimiter(-1, desiredSpeedX + 1.0f);
                setAction(Action.WALKBACK_LEFT);
                return 1;
            }
        }
        return 0;
    }

    public override int keyRightDown()
    {
        if ((isInAction(Action.IDLE) || moving(1) || jumping()) && isInState(Zap.State.ON_GROUND))
        {
            if (zap.CheckRight(0.1f) >= 0.0f)
            {
                if (zap.dir() == -Vector2.right)
                {
                    //turnRightStart();
                }
                return 0;
            }
            if (zap.dir() == Vector2.right)
            {
                desiredSpeedX = WalkSpeed;
                speedLimiter(1, desiredSpeedX + 1.0f);
                setAction(Action.WALK_RIGHT);
                return 1;
            }
            else
            {
                //turnRightStart();
                desiredSpeedX = WalkBackSpeed;
                speedLimiter(1, desiredSpeedX + 1.0f);
                setAction(Action.WALKBACK_RIGHT);
                return 1;
            }
        }
        return 0;
    }

    public override int keyLeftUp()
    {

        if (isInState(Zap.State.ON_GROUND))
        {
            desiredSpeedX = 0.0f;
        }

        return 0;
    }

    public override int keyRightUp()
    {

        if (isInState(Zap.State.ON_GROUND))
        {
            desiredSpeedX = 0.0f;
        }

        return 0;
    }

    public override int keyJumpDown()
    {
        if (isNotInState(Zap.State.ON_GROUND))
            return 0;

        if (isInAction(Action.IDLE) || walking() != 0)
        {
            if (Input.GetKey(zap.keyLeft))
            {
                //jumpLeft();
                rollLeft();
                return 1;
            }
            if (Input.GetKey(zap.keyRight))
            {
                //jumpRight();
                rollRight();
                return 1;
            }
            return 0;
        }

        if (crouching())
        {
            if (Input.GetKey(zap.keyLeft))
            {
                rollLeft();
                return 1;
            }
            if (Input.GetKey(zap.keyRight))
            {
                rollRight();
                return 1;
            }
            return 0;
        }

        return 0;
    }

    public override int keyJumpUp()
    {
        //jumpKeyPressed = false;
        canJumpAfter = true;
        return 0;
    }

    bool checkDir()
    {
        Vector2 sightTarget;
        //Vector2 mouseInScene = touchCamera.ScreenToWorldPoint (Input.mousePosition);
        if (draggedStone)
        {
            sightTarget = draggedStone.GetComponent<Rigidbody2D>().worldCenterOfMass;
        }
        else
        {
            sightTarget = touchCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        //if (Vector2.Distance(zapTargeterPos, sightTarget) < 0.5f)
        //    return false;

        float yd = sightTarget.y - zapTargeterPos.y;
        int angleAnimIndex = 1; // getIndexOfAngle(zapTargeterPos, sightTarget, 3);
        if (yd > 0.5f) angleAnimIndex = 2;
        else if (yd < -0.5f) angleAnimIndex = 0;

        if (zap.faceRight())
        {
            if (transform.position.x > (sightTarget.x/*+0.5f*/))
            {
                setAction(Action.TURN_STAND_LEFT, angleAnimIndex);
                return true;
            }
        }
        else
        {
            if (transform.position.x < (sightTarget.x/*-0.5f*/))
            {
                setAction(Action.TURN_STAND_RIGHT, angleAnimIndex);
                return true;
            }
        }
        return false;
    }

    int Action_IDLE()
    {
        trackCursor(Action.IDLE, shooting);

        if (Input.GetMouseButtonDown(1))
        {
            setAction(Action.HIDE_GRAVITYGUN);
            return 0;
        }

        checkDir();

        return 0;
    }

    int Action_PULLOUT_GRAVITYGUN()
    {
        if (zap.currentActionTime > PULLOUT_GRAVITYGUN_DURATION)
        {
            setActionIdle();
            if( Input.GetMouseButton(0) )
            {
                leftMouseButtonClicked();
            }
            return 1;
        }
        return 0;
    }

    int Action_HIDE_GRAVITYGUN()
    {
        if (lastFlashStone)
        {
            unflashStone(lastFlashStone);
            lastFlashStone = null;
        }

        if (zap.currentActionTime > HIDE_GRAVITYGUN_DURATION)
        {
            zap.hideChoosenWeapon();
            return 1;
        }
        return 0;
    }

    int Action_WALK(int dir)
    {
        trackCursor(Action.IDLE, shooting);

        bool dirChanged = checkDir();
        if (dirChanged)
        {
            //setAction(Action.IDLE);
            //resetActionAndState();
            return 0;
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
            setAction(Action.IDLE);
            resetActionAndState();
            return 0;
        }

        distToMove = zap.velocity.x * zap.getCurrentDeltaTime();

        zap.AnimatorBody.speed = 0.5f + (Mathf.Abs(zap.velocity.x) / WalkSpeed) * 0.5f;

        float distToObstacle = 0.0f;
        if (zap.CheckObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            setActionIdle();
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        return 0;
    }

    int Action_ROLL(int dir)
    {
        if (zap.currentActionTime >= rollDuration)
        {
            setAction(Action.IDLE);
            resetActionAndState();
            return 0;
        }

        distToMove = zap.velocity.x * zap.getCurrentDeltaTime();

        float distToObstacle = 0.0f;
        if (zap.CheckObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            //setActionIdle();
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        return 0;
    }

    void rollLeft()
    {
        stopShoot();
        zap.velocity.x = -rollSpeed;
        zap.velocity.y = 0.0f;
        if (!zap.faceRight())
            setAction(Action.ROLL_LEFT_FRONT);
        else
            setAction(Action.ROLL_LEFT_BACK);
    }

    void rollRight()
    {
        stopShoot();
        zap.velocity.x = rollSpeed;
        zap.velocity.y = 0.0f;
        if (zap.faceRight())
            setAction(Action.ROLL_RIGHT_FRONT);
        else
            setAction(Action.ROLL_RIGHT_BACK);
    }

    //void turnLeftStart()
    //{
    //    setAction(Action.TURN_STAND_LEFT);

    //    if (Input.GetKeyDown(zap.keyJump) || (Input.GetKey(zap.keyJump) && canJumpAfter))
    //        wantJumpAfter = true;
    //}

    //void turnRightStart()
    //{
    //    setAction(Action.TURN_STAND_RIGHT);

    //    if (Input.GetKeyDown(zap.keyJump) || (Input.GetKey(zap.keyJump) && canJumpAfter))
    //        wantJumpAfter = true;
    //}

    void turnLeftFinish()
    {
        setAction(Action.IDLE);

        if (wantJumpAfter)
        {
            if (Input.GetKey(zap.keyJump))
                canJumpAfter = false;
        }
        else
        {
            resetActionAndState();
        }
    }

    void turnRightFinish()
    {
        setAction(Action.IDLE);

        if (wantJumpAfter)
        {
            if (Input.GetKey(zap.keyJump))
                canJumpAfter = false;
        }
        else
        {
            resetActionAndState();
        }
    }

    void setActionIdle()
    {
        zap.velocity.x = 0.0f;
        setAction(Action.IDLE);
    }

    void resetActionAndState()
    {
        if (isInState(Zap.State.ON_GROUND))
        {
            if (Input.GetKey(zap.keyDown))
            { //&& (Input.GetKey(zap.keyLeft) || Input.GetKey(zap.keyRight)) ){
                if (keyDownDown() == 0)
                    setActionIdle();
            }
            else if (Input.GetKey(zap.keyLeft))
            {
                if (keyLeftDown() == 0)
                    setActionIdle();
            }
            else if (Input.GetKey(zap.keyRight))
            {
                if (keyRightDown() == 0)
                    setActionIdle();
            }
            else
            {
                if (isInState(Zap.State.ON_GROUND))
                {
                    setActionIdle();
                }
            }
        }
    }

    int walking()
    {
        if (isInAction(Action.WALK_RIGHT) || isInAction(Action.WALKBACK_RIGHT))
            return 1;
        if (isInAction(Action.WALK_LEFT) || isInAction(Action.WALKBACK_LEFT))
            return -1;
        return 0;
    }

    bool moving(Vector2 dir)
    {
        if (dir == Vector2.right)
            return isInAction(Action.WALK_RIGHT) || isInAction(Action.WALKBACK_RIGHT);
        else
            return isInAction(Action.WALK_LEFT) || isInAction(Action.WALKBACK_LEFT);
    }
    bool moving(int dir)
    {
        if (dir == 1)
            return isInAction(Action.WALK_RIGHT) || isInAction(Action.WALKBACK_RIGHT);
        else
            return isInAction(Action.WALK_LEFT) || isInAction(Action.WALKBACK_LEFT);
    }
    bool jumping()
    {
        return false;
    }
    public override void zapDie(Zap.DeathType deathType)
    {
        base.zapDie(deathType);
        releaseStone();
        setAction(Action.DIE, (int)deathType);
    }
    
    public override bool triggerEnter(Collider2D other)
    {
        return false;
    }

    bool checkSpeed(int dir)
    {
        float speedX = Mathf.Abs(zap.velocity.x);
        if (speedX < desiredSpeedX)
        { // trzeba przyspieszyc

            float velocityDamp = SpeedUpParam * zap.getCurrentDeltaTime();
            speedX += velocityDamp;
            if (speedX > desiredSpeedX)
            {
                speedX = desiredSpeedX;
                zap.velocity.x = desiredSpeedX * dir;
                return true;
            }
            zap.velocity.x = speedX * dir;
            return false;

        }
        else if (speedX > desiredSpeedX)
        { // trzeba zwolnic
            float velocityDamp = SlowDownParam * zap.getCurrentDeltaTime();
            speedX -= velocityDamp;
            if (speedX < desiredSpeedX)
            {
                speedX = desiredSpeedX;
                zap.velocity.x = desiredSpeedX * dir;
                return true;
            }
            zap.velocity.x = speedX * dir;
            return false;
        }
        return true;
    }

    bool speedLimiter(int dir, float absMaxSpeed)
    {
        if (dir == -1)
        {
            if (zap.velocity.x < 0.0f && Mathf.Abs(zap.velocity.x) > absMaxSpeed)
            {
                zap.velocity.x = -absMaxSpeed;
                return true;
            }
        }
        else if (dir == 1)
        {
            if (zap.velocity.x > 0.0f && Mathf.Abs(zap.velocity.x) > absMaxSpeed)
            {
                zap.velocity.x = absMaxSpeed;
                return true;
            }
        }
        //aa
        return false;
    }
    
    bool catchStone(Transform stone)
    {
        return false;
    }

    void releaseStone()
    {
        if (draggedStone)
        {
            //Debug.Log("releaseStone");
            Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
            if (tsrb)
            {
                tsrb.gravityScale = 1f;
            }
            unflashStone2(draggedStone);

            //Debug.Log ( "add dropped stone: " + tsrb );
            //droppedStones.Add( tsrb );

            draggedStone.gameObject.layer = LayerMask.NameToLayer("GroundMoveable");  //zap.layerIdDraggedGroundMoveableMask;

            draggedStone = null;
            draggedStoneMoveable = null;
        }
    }

    public Material draggedStoneMaterial = null;
    public Material normalMaterial = null;

    void flashStone(Transform stone)
    {
        //setStoneOpacity(stone, 0.5f);
        //stone.GetComponent<GroundMoveable>().printWorldVertices();

        Color c = new Color(0f, 1f, 1f);
        setStoneColor(stone, c);
    }
    float draggedDuration = 0;
    void flashStone2(Transform stone)
    {
        //setStoneOpacity(stone, 0.5f);
        //stone.GetComponent<GroundMoveable>().printWorldVertices();
        Color c = new Color(1f, 1f, 1f);
        setStoneColor(stone, c);

        if( stone.childCount == 1 )
        {
            SpriteRenderer sprRend = stone.GetComponentInChildren<SpriteRenderer>();
            if( sprRend )
            {
                //Material myNewMaterial = new Material("My/DraggedStone");
                //if( myNewMaterial )
                //{
                //    Debug.Log("JEST W KONCU KURWA MATERIAL");
                //    sprRend.material = myNewMaterial;
                //}
                //Material newMat = Resources.Load("Assets/Materials/DraggedStone",typeof(Material)) as Material;
                //Instantiate<dragged>()
                //Debug.Log(sprRend.material);
                Material newMat = Instantiate<Material>(draggedStoneMaterial);
                //Debug.Log(newMat);
                //Object newMat = Resources.Load("Materials/DraggedStone");
                if (newMat)
                {
                    //Debug.Log("JEST W KONCU KURWA MATERIAL");
                    sprRend.material = newMat;
                    //_sx("sx", Range(0, 20)) = 1
                    //_sy("sy", Range(0, 20)) = 1
                    //_speedX("speedX", Range(0, 0.3)) = 0
                    //_speedY("speedY", Range(0, 0.3)) = 0
                    draggedDuration = 0f; // (Mathf.PI * 0.5f) / 1.5f;
                    BoxCollider2D sbc = stone.GetComponent<BoxCollider2D>();
                    if (sbc != null)
                    {
                        sprRend.material.SetFloat("_sx", sbc.size.x);
                        sprRend.material.SetFloat("_sy", sbc.size.y);
                        sprRend.material.SetFloat("_draggedDuration", draggedDuration);
                    }
                }
            }
        }
    }
    void unflashStone(Transform stone)
    {
        //00FFFFFF
        //setStoneOpacity(stone, 1.0f);
        Color c = new Color(1f, 1f, 1f);
        setStoneColor(stone, c);
    }
    void unflashStone2(Transform stone)
    {
        //00FFFFFF
        //setStoneOpacity(stone, 1.0f);
        Color c = new Color(1f, 1f, 1f);
        setStoneColor(stone, c);

        if (stone.childCount == 1)
        {
            SpriteRenderer sprRend = stone.GetComponentInChildren<SpriteRenderer>();
            if (sprRend)
            {
                Material newMat = Instantiate<Material>(normalMaterial);
                //Debug.Log(newMat);
                //Object newMat = Resources.Load("Materials/DraggedStone");
                if (normalMaterial)
                {
                    //Debug.Log("JEST W KONCU KURWA MATERIAL");
                    sprRend.material = normalMaterial;
                }
                 
                ////Material newMat = Resources.Load("Assets/Materials/PixelSnap", typeof(Material)) as Material;
                //if (newMat)
                //{
                //    sprRend.material = newMat;
                //}
            }
        }
    }
    void setStoneOpacity(Transform stone, float newOpacity)
    {
        SpriteRenderer sr = stone.GetComponent<SpriteRenderer>();
        if (!sr)
            return;

        Color stoneColor = sr.color;
        stoneColor.a = newOpacity;
        sr.color = stoneColor;
    }
    void setStoneColor(Transform stone, Color newColor)
    {
        if (stone.childCount != 1) return;

        SpriteRenderer sr = stone.GetChild(0).GetComponent<SpriteRenderer>();
        if (sr)
        {
            sr.color = newColor;
        }
    }

    bool canBeDragged(Transform stone, Vector3 stoneTargetPlace, bool testLinecast)
    {
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
        if (!rb)
            return false;

        //if ((rb.worldCenterOfMass - stoneTargetPlace).magnitude > 5f)
        if( (draggedStone.TransformPoint(draggedStoneHitPos) - stoneTargetPlace).magnitude > 5f)
        {
            return false;
        }

        return canBeDragged(stone,testLinecast);
    }

    bool canBeDragged(Transform stone, bool testLinecast)
    {
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
        if (!rb)
            return false;

        if (Vector3.Distance(draggedStone.TransformPoint(draggedStoneHitPos), zapTargeterPos) > maxDistance)
        {
            return false;
        }
        else if (testLinecast)
        {
            hit = Physics2D.Linecast(zapTargeterPos, draggedStone.TransformPoint(draggedStoneHitPos), zap.layerIdGroundAllMask);
            if (hit.collider)
            {
                if (hit.collider.transform != draggedStone)
                {
                    stopShoot();
                    return false;
                }
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
