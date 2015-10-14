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

    public float TURN_LEFTRIGHT_DURATION = 0.2f;
    public float ATTACK_DURATION = 0.5f;
    public float PULLOUT_GRAVITYGUN_DURATION = 0.3f;
    public float HIDE_GRAVITYGUN_DURATION = 0.35f;
    public float CROUCHINOUT_DURATION = 0.1f;

    public Transform draggedStone = null;
    public Transform lastFlashStone = null;
    //public int layerIdGroundMoveableMask = 0;
    //public int layerIdGroundMask = 0;

    Vector2 T;          // sila ciagu
    public float inertiaFactor = 0.09f;         // wspolczynnik oporu - u mnie raczej bezwladnosci
                                                //public float inertiaFactor2 = 0.03f; 	// wspolczynnik bezwladnosci jak gracz na siebie chce skierowac kamien
    public float maxDistance = 8f;
    //public float minDistance = 2f;
    //public float pushOutForce = 2f;
    //public float pushOutMassFactor = 10f;

    //List<Rigidbody2D> droppedStones = new List<Rigidbody2D> ();

    Vector2 V;          // predkosc
    public static float userStoneRotateSpeed = 180f;

    //	public ZapControllerGravityGun () 
    //		: base("GravityGun")
    //	{
    //		//zap.layer
    //	}

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
    }

    float distToMove;
    Vector3 oldPos;
    float newPosX;

    Vector3 climbBeforePos;
    Vector3 climbAfterPos;
    Vector3 climbDistToClimb;
    float climbToJumpDuration;

    float groundUnderFeet;

    void leftMouseNotPressed()
    {

        if (zap.isDead())
            return;

        if (draggedStone == null)
        {

            if (lastFlashStone)
            {
                unflashStone(lastFlashStone);
                lastFlashStone = null;
            }
            //Camera.main.

            Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 rayOrigin = zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
            Vector3 _df = mouseInScene - rayOrigin;

            if (_df.magnitude <= maxDistance)
            {

                RaycastHit2D hit = Physics2D.Linecast(mouseInScene, mouseInScene, zap.layerIdGroundMoveableMask);
                if (hit.collider)
                {

                    lastFlashStone = hit.collider.gameObject.transform;
                    if (lastFlashStone)
                    {
                        Rigidbody2D tsrb = lastFlashStone.GetComponent<Rigidbody2D>();
                        if (tsrb)
                        {

                            //rayOrigin = player.dir() == Vector2.right ? player.sensorRight2.position : player.sensorLeft2.position;

                            hit = Physics2D.Linecast(rayOrigin, tsrb.worldCenterOfMass, zap.layerIdGroundMask);
                            if (hit.collider)
                            {
                                lastFlashStone = null;
                            }
                            else
                            {
                                flashStone(lastFlashStone);
                            }

                        }
                        else
                        {
                            lastFlashStone = null;
                        }
                    }
                }
            }
        }
    }

    void leftMouseButtonClicked()
    {
        if (zap.isDead())
            return;

        draggedStone = null;
        shooting = true;
        zap.GravityGunBeam.gameObject.SetActive(true);

        Vector3 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Linecast(mouseInScene, mouseInScene, zap.layerIdGroundMoveableMask);
        if (hit.collider)
        {
            draggedStone = hit.collider.gameObject.transform;

            if (canBeDragged(draggedStone))
            {

                Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
                tsrb.gravityScale = 0f;
                flashStone(draggedStone);

            }
            else
            {
                draggedStone = null;
            }
        }
    }

    void stopShoot()
    {
        shooting = false;
        zap.GravityGunBeam.gameObject.SetActive(false);
        releaseStone();
    }

    public override void MUpdate(float deltaTime)
    {
        //Debug.Log ("ZapContrllerNormal::Update : " + deltaTime);

        //currentActionTime = zap.getCurrentActionTime();

        oldPos = transform.position;
        newPosX = oldPos.x;
        distToMove = 0.0f;

        //checkStartAttack ();
        //checkStartCrouchAttack ();

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
                zap.checkGround(zap.layerIdGroundAllMask, ref distToGround);
                if (!zap.groundUnder)
                {
                    zap.suddenlyInAir();
                }
                else
                {

                    //wantGetUp = false;
                    //zap.hideChoosenWeapon();
                    //zap.setState(Zap.State.IN_AIR);
                    //setAction(Action.JUMP);

                }

                break;

        };

        zap.lastVelocity = zap.velocity;

    }

    public override void FUpdate(float fDeltaTime)
    {
        if (draggedStone)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) ||
               Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X))
            {

                Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    rb.angularVelocity = 0;
                }
            }

            if (Input.GetKey(KeyCode.Z))
            { // obracam kamien w lewo ...

                Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
                if (rb)
                {

                    if (rb.angularVelocity < 180)
                        rb.angularVelocity += (fDeltaTime * userStoneRotateSpeed);

                    rb.angularVelocity = Mathf.Min(rb.angularVelocity, 180f);

                    //rb.rotation += ( fDeltaTime * userStoneRotateSpeed );
                }
            }
            else if (Input.GetKey(KeyCode.X))
            { // albo w prawo

                Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
                if (rb)
                {

                    if (rb.angularVelocity > -180)
                        rb.angularVelocity -= (fDeltaTime * userStoneRotateSpeed);

                    rb.angularVelocity = Mathf.Max(rb.angularVelocity, -180f);

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

        if (Input.GetMouseButton(0))
        {
            Vector3 touchInScene = touchCamera.ScreenToWorldPoint(currentMousePosition);
            Vector2 tis = touchInScene;

            if (draggedStone)
            {
                Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    Vector2 playerCenterPos = zap.transform.position;
                    playerCenterPos.y += 1f;
                    Vector2 stoneCenterPos = rb.worldCenterOfMass;

                    Vector2 diff = stoneCenterPos - playerCenterPos;
                    Vector2 F = new Vector2(0f, 0f);

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
                    rb.AddForce(F, ForceMode2D.Impulse);

                    if (!canBeDragged(draggedStone, tis))
                    {
                        releaseStone();
                    }

                }
            }
        }
    }
    //bool restored = false;

    public override void activateSpec(bool restore = false, bool crouch = false)
    {
        //base.activate ();
        //setAction (Action.IDLE);
        zap.GfxLegs.gameObject.SetActive(false);
        zap.GravityGunBeam.gameObject.SetActive(false);
        setAction(Action.PULLOUT_GRAVITYGUN);
        //canPullUp = false;
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

    void trackCursor(Action act, bool shoot)
    {
        Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 rayOrigin = zap.Targeter.position; // zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
        Vector3 df = mouseInScene - rayOrigin;

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
        //Debug.Log("trackCursor : " + deg);


        switch (act)
        {
            case Action.IDLE:
            case Action.WALK_LEFT:
            case Action.WALK_RIGHT:
            case Action.WALKBACK_LEFT:
            case Action.WALKBACK_RIGHT:
                if (deg < -45)
                {
                    if (shoot)
                    {
                        zap.AnimatorBody.Play("Zap_gg_body_fire_-45");
                    }
                    else
                    {
                        zap.AnimatorBody.Play("Zap_gg_body_walk_-45");
                    }
                }
                else if (deg < 45)
                {
                    if (shoot)
                    {
                        zap.AnimatorBody.Play("Zap_gg_body_fire_0");
                    }
                    else
                    {
                        zap.AnimatorBody.Play("Zap_gg_body_walk_0");
                    }
                }
                else
                {
                    if (shoot)
                    {
                        zap.AnimatorBody.Play("Zap_gg_body_fire_45");
                    }
                    else
                    {
                        zap.AnimatorBody.Play("Zap_gg_body_walk_45");
                    }
                }
                break;

                //case Action.WALK_LEFT:
                //    break;

                //case Action.WALK_RIGHT:
                //    break;

                //case Action.WALKBACK_LEFT:
                //    break;

                //case Action.WALKBACK_RIGHT:
                //    break;

                //case Action.at:
                //    break;

                //case Action.STOP_WALK:
                //  break;
        }

        if (shoot)
        {
            LineRenderer beam = zap.GravityGunBeam.GetComponent<LineRenderer>();
            Vector2 beamOrigin = zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
            //if (shoot) { }
            beam.SetPosition(0, beamOrigin);
            Vector2 beamTarget;
            if (draggedStone != null)
            {
                //line.SetPosition(1, draggedStone.GetComponent<Rigidbody2D>().worldCenterOfMass);
                beamTarget = draggedStone.GetComponent<Rigidbody2D>().worldCenterOfMass;
                //float 
            }
            else
            {
                //line.SetPosition(1, mouseInScene);
                beamTarget = mouseInScene;
            }

            Vector2 beamDist = beamTarget - beamOrigin;
            float beamDistMag = beamDist.magnitude;
            if (beamDistMag > maxDistance)
            {
                beamTarget = beamOrigin + (beamDist.normalized * maxDistance);
                beamDistMag = maxDistance;
            }
            beam.SetPosition(1, beamTarget);
            beamTargetColor.a = 1f - (beamDistMag / maxDistance);
            beam.SetColors(beamOriginColor, beamTargetColor);
            beam.SetWidth(0.1f, 0.5f * (beamDistMag / maxDistance));
        }
    }

    Color beamOriginColor = new Color(0f, 23f / 255f, 1f, 200f / 255f);
    Color beamTargetColor = new Color(0f, 23f / 255f, 1f, 200f / 255f);

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
                    zap.AnimatorBody.Play("Zap_gg_body_walk_0");
                    zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                }
                else
                {
                    zap.AnimatorBody.Play("Zap_gg_body_walk_0");
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
                    zap.getAudioSource().PlayOneShot(zap.dieSounds[Random.Range(0, zap.dieSounds.Length)], 0.3F);
                break;

            case Action.WALK_LEFT:
                //zap.AnimatorBody.Play("Zap_knife_walk");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_gg_body_walk_0");
                zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                //zap.AnimatorLegs.speed = 0f;
                //zap.AnimatorBody.speed = 0f;
                break;
            case Action.WALK_RIGHT:
                //zap.AnimatorBody.Play("Zap_knife_walk");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_gg_body_walk_0");
                zap.AnimatorLegs.Play("Zap_gg_legs_walk");
                break;

            case Action.WALKBACK_LEFT:
                //zap.AnimatorBody.Play("Zap_knife_walkback");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_gg_body_walk_0");
                zap.AnimatorLegs.Play("Zap_gg_legs_walkback");
                break;
            case Action.WALKBACK_RIGHT:
                //zap.AnimatorBody.Play("Zap_knife_walkback");
                zap.GfxLegs.gameObject.SetActive(true);
                zap.AnimatorBody.Play("Zap_gg_body_walk_0");
                zap.AnimatorLegs.Play("Zap_gg_legs_walkback");
                break;

            case Action.TURN_STAND_LEFT:
                zap.AnimatorBody.Play("Zap_knife_turnleft");
                wantJumpAfter = false;
                break;

            case Action.TURN_STAND_RIGHT:
                zap.AnimatorBody.Play("Zap_knife_turnright");
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
            if (zap.checkLeft(0.1f) >= 0.0f)
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
            if (zap.checkRight(0.1f) >= 0.0f)
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

        //Debug.Log ("ZapControllerNormal::keyJumpDown()");
        //jumpKeyPressed = true;

        switch (action)
        {
            case Action.IDLE:
                if (isInState(Zap.State.ON_GROUND))
                {
                    //preparetojump ();
                }
                break;
        }

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

        if (zap.faceRight())
        {
            if (transform.position.x > sightTarget.x)
            {
                setAction(Action.TURN_STAND_LEFT);
                return true;
            }
        }
        else
        {
            if (transform.position.x < sightTarget.x)
            {
                setAction(Action.TURN_STAND_RIGHT);
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
            //			zap._hideKnife();
            //			return 1;

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
            //setAction(Action.ATTACK,1);
            setActionIdle();
            return 1;
        }
        return 0;
    }

    int Action_HIDE_GRAVITYGUN()
    {
        if (zap.currentActionTime > HIDE_GRAVITYGUN_DURATION)
        {
            //zap._hideGravityGun();
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
        if (zap.checkObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            setActionIdle();
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        float distToGround = 0.0f;
        zap.checkGround(zap.layerIdGroundAllMask, ref distToGround);
        if (zap.groundUnder)
        {
            transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        }
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
        if (zap.checkObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            //setActionIdle();
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        //float distToGround = 0.0f;
        //bool groundUnderFeet = zap.checkGround(false, zap.layerIdGroundAllMask, ref distToGround);
        //if (groundUnderFeet)
        //{
        //    transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        //}
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

    void turnLeftStart()
    {
        setAction(Action.TURN_STAND_LEFT);

        if (Input.GetKeyDown(zap.keyJump) || (Input.GetKey(zap.keyJump) && canJumpAfter))
            wantJumpAfter = true;
    }

    void turnRightStart()
    {
        setAction(Action.TURN_STAND_RIGHT);

        if (Input.GetKeyDown(zap.keyJump) || (Input.GetKey(zap.keyJump) && canJumpAfter))
            wantJumpAfter = true;
    }

    void turnLeftFinish()
    {
        setAction(Action.IDLE);

        if (wantJumpAfter)
        {
            //jumpLeft();

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
            //jumpRight();

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
        //return isInAction(Action.JUMP) || isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_RIGHT);
        return false;
    }
    public override void zapDie(Zap.DeathType deathType)
    {
        base.zapDie(deathType);
        releaseStone();
        setAction(Action.DIE, (int)deathType);
    }
    //	public override void reborn(){
    //		if (zap.getLastTouchedCheckPoint().GetComponent<CheckPoint> ().startMounted) {
    //			zap.setState(Zap.State.MOUNT);
    //		}
    //	}
    public override bool triggerEnter(Collider2D other)
    {

        //		if (other.gameObject.tag == "Bird") {
        //			if( isInState(Zap.State.MOUNT) ){
        //				zap.velocity.x = 0.0f;
        //				zap.velocity.y = 0.0f;
        //				setAction(Action.JUMP);
        //				zap.setState(Zap.State.IN_AIR);
        //				
        //				if( zap.canBeFuddleFromBird )
        //					zap.setFuddledFromBrid(true);
        //				
        //			} else if( isInState(Zap.State.IN_AIR) ) {
        //				zap.velocity.x = 0.0f;
        //			}
        //			return true;
        //		}

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


    void getUp()
    {
        setAction(Action.IDLE);
        resetActionAndState();
    }

    public float ClimbPullDownRange = 0.511f;

    GameObject canClimbPullDown()
    {

        if (!isInState(Zap.State.ON_GROUND) || !(isInAction(Action.IDLE))) //|| isInAction(Action.CROUCH_IDLE)) )
            return null;

        // 1: sytuacja gdy zap jest swoim srodkiem nad tilem
        // 2: sytuacja gdy zap jest swoim srodkiem juz poza tilem

        RaycastHit2D hit;

        if (zap.dir() == Vector2.right)
        { //

            hit = Physics2D.Raycast(zap.sensorDown2.position, -Vector2.right, ClimbPullDownRange, zap.layerIdGroundHandlesMask);
            if (hit.collider)
            {

                if (Physics2D.Raycast(hit.collider.gameObject.transform.position, -Vector2.right, 0.5f, zap.layerIdGroundMask).collider == null)
                {
                    return hit.collider.gameObject;
                }
            }

        }
        else
        {

            hit = Physics2D.Raycast(zap.sensorDown2.position, Vector2.right, ClimbPullDownRange, zap.layerIdGroundHandlesMask);
            if (hit.collider)
            {

                if (Physics2D.Raycast(hit.collider.gameObject.transform.position, Vector2.right, 0.5f, zap.layerIdGroundMask).collider == null)
                {
                    return hit.collider.gameObject;
                }

            }
        }

        // to jest ta druga sytuacja ...

        Vector2 rayOrigin = zap.sensorDown1.position;
        hit = Physics2D.Raycast(rayOrigin, Vector2.right, zap.getMyWidth(), zap.layerIdGroundHandlesMask);

        if (hit.collider)
        {
            // badam czy stoje na krawedzi odpowiednio zwrocony
            if (zap.dir() == Vector2.right)
            { //

                // pod lewa noga musi byc przepasc
                rayOrigin = zap.sensorDown1.position;
                if (Physics2D.Raycast(rayOrigin, -Vector2.up, 0.5f, zap.layerIdGroundMask).collider) return null;
                else return hit.collider.gameObject;

            }
            else
            {

                // pod prawa noga musi byc przepasc
                rayOrigin = zap.sensorDown3.position;
                if (Physics2D.Raycast(rayOrigin, -Vector2.up, 0.5f, zap.layerIdGroundMask).collider) return null;
                else return hit.collider.gameObject;

            }

        }
        else
        {
            return null;
        }
    }

    bool catchStone(Transform stone)
    {
        return false;
    }

    void releaseStone()
    {
        if (draggedStone)
        {
            Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
            if (tsrb)
            {

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

    void flashStone(Transform stone)
    {
        setStoneOpacity(stone, 0.5f);
    }
    void unflashStone(Transform stone)
    {
        setStoneOpacity(stone, 1.0f);
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


    bool canBeDragged(Transform stone, Vector2 stoneTargetPlace)
    {

        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
        if (!rb)
            return false;

        if ((rb.worldCenterOfMass - stoneTargetPlace).magnitude > 5f)
        {
            return false;
        }

        return canBeDragged(stone);
    }

    bool canBeDragged(Transform stone)
    {

        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
        if (!rb)
            return false;

        Vector2 rayOrigin = zap.dir() == Vector2.right ? zap.sensorRight2.position : zap.sensorLeft2.position;
        Vector3 _df = rb.worldCenterOfMass - rayOrigin;

        if (_df.magnitude > maxDistance)
        {

            return false;

        }
        else
        {

            RaycastHit2D hit = Physics2D.Linecast(rayOrigin, rb.worldCenterOfMass, zap.layerIdGroundMask);
            if (hit.collider)
            {
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
