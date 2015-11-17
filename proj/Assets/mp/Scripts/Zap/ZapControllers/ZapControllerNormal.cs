using UnityEngine;
using System.Collections;
//using System; //This allows the IComparable Interface

//[System.Serializable]
public class ZapControllerNormal : ZapController
{

    public float WalkSpeed = 2.5f;
    public float RunSpeed = 5.7f;
    public float JumpSpeed = 3.8f;
    public float JumpLongSpeed = 4.9f;
    public float FromClimbJumpSpeed = 5.5f;
    public float CrouchSpeed = 1.8f;
    public float MountSpeed = 2.0f; // ile na sek.
    public float MountJumpDist = 10.0f; // następnie naciskasz spacje a on skacze
    public float SpeedUpParam = 10.0f; // ile jednosek predkosci hamuje na sekund
    public float SlowDownParam = 20.0f; // ile jednosek predkosci hamuje na sekunde
    public float FlyUserControlParam = 8.0f; // ile przyspiesza na sekunde lecac
    public float FlyUpUserControlParam = 9.0f; // ile przyspiesza na sekunde lecac
    public float FlySlowDownParam = 5.0f; // ile hamuje na sekunde lecac
    public float PushMaxSpeed = 2f;
    public float PullMaxSpeed = 2f;

    public float JumpImpulse = 7.0f;
    public float JumpLongImpulse = 7.15f;
    public float FromClimbJumpImpulse = 5.0f;
    public float FromClimbJumpSideImpulse = 5.0f;
    public float GravityForce = -20.0f;
    public float MaxSpeedY = 15.0f;

    public float HardLandingHeight = 2.0f;
    public float VeryHardLandingHeight = 6.0f;
    public float MaxFallDistToCatch = 6.0f;

    public float RopeSwingForce = 4250f;
    public float RopeClimbSpeedUp = 1.0f;
    public float RopeClimbSpeedDown = 3.0f;

    public float CLIMB_DURATION = 1.5f;
    public float CLIMBDUR_PREPARE_TO_JUMP = 0.5f;
    public float CLIMBDUR_JUMP_TO_CATCH = 0.2f; // jednostka w 0.2f
    public float CLIMBDUR_CATCH = 0.5f;
    /*public*/
    float CLIMBDUR_CLIMB = 0.65f;
    public float LANDING_HARD_DURATION = 0.3f;

    public float TURN_LEFTRIGHT_DURATION = 0.2f;
    public float MOUNT_ATTACK_DURATION = 0.5f;

    public float BIRD_HIT_DURATION = 0.33f;

    public AudioClip ropeCatchSound = null;
    public AudioClip ropeSwingSound = null;
    public AudioClip mountCatchSound = null;
    public AudioClip mountClimbSound = null;

    //public AudioClip landingSnd = null;
    

    //	public ZapControllerNormal (Zap zapPlayer) 
    //		: base(zapPlayer,"Normal")
    //	{
    //	}
    //	public ZapControllerNormal () 
    //		: base("Normal")
    //	{
    //	}

    GameObject catchedClimbHandle;
    GameObject lastCatchedClimbHandle;
    bool canPullUp;
    NewRope catchedRope;
    RopeLink catchedRopeLink;
    bool lastFrameHande;

    float distToMove;
    Vector3 oldPos;
    float newPosX;

    Vector3 climbBeforePos;
    Vector3 climbAfterPos;
    Vector3 climbAfterPos2;
    Vector3 climbDistToClimb;
    float climbToJumpDuration;

    Vector3 mountJumpStartPos;
    Vector3 lastHandlePos;

    float groundUnderFeet;

    public override void MUpdate(float deltaTime, bool firstUpdate)
    {
        firstUpdateInFrame = firstUpdate;

        //Debug.Log ("ZapContrllerNormal::Update : " + deltaTime);

        justJumpedMount = false;

        //currentActionTime = zap.getCurrentActionTime();

        oldPos = transform.position;
        newPosX = oldPos.x;
        distToMove = 0.0f;

        switch (action)
        {
            case Action.IDLE:
                if (Action_IDLE() != 0)
                    return;
                break;

            case Action.LANDING_HARD:
                Action_LANDING_HARD();
                break;

            case Action.PREPARE_TO_JUMP:
                if (zap.currentActionTime >= 0.2f)
                {
                    jump();
                }
                break;

            case Action.CLIMB_PULLDOWN:
                Action_CLIMB_PULLDOWN();
                break;

            case Action.CLIMB_JUMP_TO_CATCH:
                Action_CLIMB_JUMP_TO_CATCH();
                break;

            case Action.CLIMB_CATCH:
                Action_CLIMB_CATCH();
                break;

            case Action.CLIMB_CLIMB:
                Action_CLIMB_CLIMB();
                break;

            case Action.WALK_LEFT:
                if (Action_WALK(-1) != 0)
                    return;
                break;
            case Action.RUN_LEFT:
                if (Action_RUN(-1) != 0)
                    return;
                break;

            case Action.WALK_RIGHT:
                if (Action_WALK(1) != 0)
                    return;
                break;
            case Action.RUN_RIGHT:
                if (Action_RUN(1) != 0)
                    return;
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

            case Action.TURN_RUN_LEFT:
                if (zap.currentActionTime >= 0.85f)
                {
                    zap.turnLeft();
                    if (wantJumpAfter)
                    {
                        jumpLeft();
                    }
                    else
                    {
                        setActionIdle();
                        resetActionAndState();
                    }
                }
                else
                {
                    int res = Action_TURN_RUN(1);
                    if (res == 1)
                    {
                    }
                }
                break;

            case Action.TURN_RUN_RIGHT:
                if (zap.currentActionTime >= 0.85f)
                {
                    zap.turnRight();
                    if (wantJumpAfter)
                    {
                        jumpRight();
                    }
                    else
                    {
                        setActionIdle();
                        resetActionAndState();
                    }
                }
                else
                {
                    int res = Action_TURN_RUN(-1);
                    if (res == 1)
                    {
                    }
                }
                break;

            case Action.CROUCH_IN:
                Action_CROUCH_IN();
                break;

            case Action.GET_UP:
                Action_GET_UP();
                break;

            case Action.CROUCH_IDLE:
                if (Action_CROUCH_IDLE() != 0)
                {
                    return;
                }
                break;

            case Action.CROUCH_LEFT:
            case Action.CROUCH_LEFT_BACK:
                if (Action_CROUCH_LEFTRIGHT(-1) != 0)
                {
                    return;
                }
                break;

            case Action.CROUCH_RIGHT:
            case Action.CROUCH_RIGHT_BACK:
                if (Action_CROUCH_LEFTRIGHT(1) != 0)
                {
                    return;
                }
                break;

            case Action.MOUNT_IDLE:
                Action_MOUNT_IDLE();
                break;

            case Action.MOUNT_BIRDHIT:
                Action_MOUNT_BIRDHIT();
                break;

            case Action.MOUNT_LEFT:
            case Action.MOUNT_RIGHT:
            case Action.MOUNT_UP:
                Action_MOUNTING();
                break;

            case Action.MOUNT_DOWN:
                Action_MOUNTING_DOWN();
                break;

            case Action.MOUNT_ATTACK_LEFT:
            case Action.MOUNT_ATTACK_RIGHT:
                Action_MOUNT_ATTACK();
                break;

            case Action.ROPECLIMB_IDLE:
                Action_ROPECLIMB_IDLE(deltaTime);
                break;

            case Action.ROPECLIMB_UP:
                Action_ROPECLIMB_UP(deltaTime);
                break;

            case Action.ROPECLIMB_DOWN:
                Action_ROPECLIMB_DOWN(deltaTime);
                break;

            //case Action.PUSH_LEFT:
            //case Action.PUSH_RIGHT:
            //    ActionPush(deltaTime);
            //    break;

            //case Action.PULL_LEFT:
            //case Action.PULL_RIGHT:
            //    ActionPull(deltaTime);
            //    break;
        };

        if (wantGetUp)
        {
            if (zap.canGetUp())
            {
                setAction(Action.GET_UP);
                wantGetUp = false;
            }
        }

        switch (zap.getState())
        {

            case Zap.State.MOUNT:
                break;

            case Zap.State.IN_AIR:

                if (zap.jumpKeyPressed)
                { //Input.GetKeyDown(zap.keyJump) || Input.GetKey(zap.keyJump) ){
                    Vector3 fallDist = zap.startFallPos - transform.position;
                    if (!zap.FuddleFromBird && (fallDist.y < MaxFallDistToCatch))
                    {
                        if (zap.checkMount())
                        {
                            if (jumpFromMount)
                            {
                                if (!justJumpedMount)
                                {
                                }
                            }
                            else
                            {
                                setActionMountIdle();
                                return;
                            }
                        }
                        else
                        {
                            jumpFromMount = false;
                        }
                    }
                }
                if (jumpFromMount && Input.GetKey(zap.keyJump))
                {
                    Vector3 fallDist = zap.startFallPos - transform.position;
                    if (!zap.FuddleFromBird && (fallDist.y < MaxFallDistToCatch))
                    {
                        Vector3 flyDist = transform.position - mountJumpStartPos;
                        if (flyDist.magnitude >= MountJumpDist)
                        {
                            setActionMountIdle();
                            jumpFromMount = false;
                            return;
                        }
                    }
                }

                if (Input.GetKey(zap.keyJump))
                {

                    if (!zap.FuddleFromBird && tryCatchRope())
                    {

                        //if (zap.ropeCatchSound)
                        //    zap.getAudioSource().PlayOneShot(zap.ropeCatchSound);

                        return;
                    }
                }

                if (Input.GetKey(zap.keyJump) || zap.autoCatchEdges)
                {
                    Vector3 fallDist = zap.startFallPos - transform.position;
                    if (!zap.FuddleFromBird && fallDist.y < MaxFallDistToCatch)
                    {
                        if (tryCatchHandle())
                        {
                            zap.lastVelocity = zap.velocity;
                            return;
                        }
                    }
                }

                if (Input.GetKeyDown(zap.keyJump))
                {
                    lastFrameHande = true;
                    if (zap.dir() == Vector2.right)
                        lastHandlePos = zap.sensorHandleR2.position;
                    else
                        lastHandlePos = zap.sensorHandleL2.position;
                }

                if (Input.GetKeyUp(zap.keyJump))
                {
                    lastFrameHande = false;
                }

                zap.AddImpulse(new Vector2(0.0f, GravityForce * deltaTime));

                if ( jumping(-1) ) //isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG))
                {
                    if (Input.GetKey(zap.keyLeft))
                    {
                        zap.velocity.x -= (FlyUserControlParam * deltaTime);

                        //if (isInAction(Action.JUMP_LEFT))
                        //{
                        //    if (Mathf.Abs(zap.velocity.x) > JumpSpeed)
                        //        zap.velocity.x = -JumpSpeed;
                        //}
                        //else
                        //{
                        //    if (Mathf.Abs(zap.velocity.x) > JumpLongSpeed)
                        //        zap.velocity.x = -JumpLongSpeed;
                        //}

                        if (Mathf.Abs(zap.velocity.x) > maxJumpSpeed)
                            zap.velocity.x = -maxJumpSpeed;
                    }
                    else if (Input.GetKey(zap.keyRight))
                    {
                        zap.velocity.x += (FlyUserControlParam * deltaTime);
                        if (zap.velocity.x > 0.0f) zap.velocity.x = 0.0f;
                    }
                }
                else if ( jumping(1) ) // (isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG))
                {
                    if (Input.GetKey(zap.keyRight))
                    {
                        zap.velocity.x += (FlyUserControlParam * deltaTime);

                        //if (isInAction(Action.JUMP_RIGHT))
                        //{
                        //    if (Mathf.Abs(zap.velocity.x) > JumpSpeed)
                        //        zap.velocity.x = JumpSpeed;
                        //}
                        //else
                        //{
                        //    if (Mathf.Abs(zap.velocity.x) > JumpLongSpeed)
                        //        zap.velocity.x = JumpLongSpeed;
                        //}

                        if (Mathf.Abs(zap.velocity.x) > maxJumpSpeed)
                            zap.velocity.x = maxJumpSpeed;
                    }
                    else if (Input.GetKey(zap.keyLeft))
                    {
                        zap.velocity.x -= (FlyUserControlParam * deltaTime);
                        if (zap.velocity.x < 0.0f) zap.velocity.x = 0.0f;
                    }
                }
                else if (isInAction(Action.JUMP))
                {
                    if (Input.GetKey(zap.keyLeft))
                    {
                        zap.velocity.x -= (FlyUpUserControlParam * deltaTime);
                        if (Mathf.Abs(zap.velocity.x) > JumpSpeed)
                            zap.velocity.x = -JumpSpeed;
                    }
                    if (Input.GetKey(zap.keyRight))
                    {
                        zap.velocity.x += (FlyUpUserControlParam * deltaTime);
                        if (Mathf.Abs(zap.velocity.x) > JumpSpeed)
                            zap.velocity.x = JumpSpeed;
                    }

                    if (zap.velocity.x > 0.0f)
                    {
                        zap.turnRight();
                    }
                    else if (zap.velocity.x < 0.0f)
                    {
                        zap.turnLeft();
                    }
                }

                Vector3 distToFall = new Vector3();
                distToFall.x = zap.velocity.x * deltaTime;

                if (distToFall.x > 0.0f)
                {
                    float obstacleOnRoad = zap.CheckRight(distToFall.x + 0.01f, true);
                    if (obstacleOnRoad >= 0.0f)
                    {
                        if (obstacleOnRoad < Mathf.Abs(distToFall.x))
                        {
                            distToFall.x = obstacleOnRoad;
                            zap.velocity.x = 0.0f;
                        }
                    }
                }
                else if (distToFall.x < 0.0f)
                {
                    float obstacleOnRoad = zap.CheckLeft(Mathf.Abs(distToFall.x) + 0.01f, true);
                    if (obstacleOnRoad >= 0.0f)
                    {
                        if (obstacleOnRoad < Mathf.Abs(distToFall.x))
                        {
                            distToFall.x = -obstacleOnRoad;
                            zap.velocity.x = 0.0f;
                        }
                    }
                }

                transform.position = transform.position + distToFall;
                distToFall.x = 0f;

                zap.velocity.y += zap.GetImpulse().y;
                if (zap.velocity.y > MaxSpeedY)
                    zap.velocity.y = MaxSpeedY;
                if (zap.velocity.y < -MaxSpeedY)
                    zap.velocity.y = -MaxSpeedY;

                distToFall.y = zap.velocity.y * deltaTime;

                bool justLanding = false;



                if (distToFall.y > 0.0f)
                { // leci w gore
                  //transform.position = transform.position + distToFall;                
                  //float spaceToCeil = distToFall.y;

                    if (zap.checkCeil(ref distToFall.y))
                    {
                        zap.velocity.y = 0f;
                    }
                }
                else if (distToFall.y < 0.0f)
                { // spada
                    if (zap.lastVelocity.y >= 0.0f)
                    { // zaczyna spadac
                      // badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
                        zap.startFallPos = transform.position;
                        //print ( "zap.startFallPos : " + zap.startFallPos );
                        if (zap.lastVelocity.y > 0.0f)
                        {
                            lastCatchedClimbHandle = null;
                        }
                    }
                    groundUnderFeet = zap.checkDown(Mathf.Abs(distToFall.y) + 0.01f);
                    if (groundUnderFeet >= 0.0f)
                    {
                        if ((groundUnderFeet < Mathf.Abs(distToFall.y)) || Mathf.Abs(groundUnderFeet - Mathf.Abs(distToFall.y)) < 0.01f)
                        {
                            lastCatchedClimbHandle = null;
                            distToFall.y = -groundUnderFeet;
                            justLanding = true;
                        }
                    }
                }

                transform.position = transform.position + distToFall;

                if (justLanding)
                {
                    zap.playSound(zap.landingSound);

                    zap.FuddleFromBird = false;

                    zap.setState(Zap.State.ON_GROUND);
                    zap.velocity.y = 0.0f;

                    Vector3 fallDist = zap.startFallPos - transform.position;

                    if (fallDist.y >= VeryHardLandingHeight)
                    {
                        zap.beforeFallController = null;
                        zap.die(Zap.DeathType.VERY_HARD_LANDING);
                    }
                    else if (fallDist.y >= HardLandingHeight)
                    {

                        //if( zap.beforeFallController == null ){
                        zap.velocity.x = 0.0f;
                        setAction(Action.LANDING_HARD);
                        //}else{
                        //}

                    }
                    else
                    {
                        if (zap.beforeFallController == null)
                        {
                            resetActionAndState();
                        }
                        else
                        {
                            zap.restoreBeforeFallController();
                            //zap.beforeFallController = null;
                        }
                    }
                }

                break;

            case Zap.State.DEAD:

                //			zap.AddImpulse(new Vector2(0.0f, GravityForce * deltaTime));
                //			
                //
                //			Vector3 distToFall = new Vector3();
                //			distToFall.x = zap.velocity.x * deltaTime;
                //			
                //			if( distToFall.x > 0.0f ){
                //				float obstacleOnRoad = zap.checkRight(distToFall.x + 0.01f,!zap.stateJustChanged);
                //				if( obstacleOnRoad >= 0.0f ){
                //					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
                //						distToFall.x = obstacleOnRoad;
                //						zap.velocity.x = 0.0f;
                //					}
                //				}
                //			}else if( distToFall.x < 0.0f ){
                //				float obstacleOnRoad = zap.checkLeft( Mathf.Abs(distToFall.x) + 0.01f,!zap.stateJustChanged);
                //				if( obstacleOnRoad >= 0.0f ){
                //					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
                //						distToFall.x = -obstacleOnRoad;
                //						zap.velocity.x = 0.0f;
                //					}
                //				}
                //			}
                //			
                //			transform.position = transform.position + distToFall;
                //			distToFall.x = 0f;
                //			
                //			zap.velocity.y += zap.GetImpulse().y;
                //			if(zap.velocity.y > MaxSpeedY)
                //				zap.velocity.y = MaxSpeedY;
                //			if(zap.velocity.y < -MaxSpeedY)
                //				zap.velocity.y = -MaxSpeedY;
                //			
                //			distToFall.y = zap.velocity.y * deltaTime;
                //			
                //			bool justLanding = false;
                //			
                //			if( distToFall.y > 0.0f ) { // leci w gore
                //				//transform.position = transform.position + distToFall;
                //			} else if( distToFall.y < 0.0f ) { // spada
                //				if( zap.lastVelocity.y >= 0.0f ) { // zaczyna spadac
                //					// badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
                //					zap.startFallPos = transform.position;
                //					//print ( "zap.startFallPos : " + zap.startFallPos );
                //					if( zap.lastVelocity.y > 0.0f ){
                //						lastCatchedClimbHandle = null;
                //					}
                //				}
                //				groundUnderFeet = zap.checkDown( Mathf.Abs(distToFall.y) + 0.01f);
                //				if( groundUnderFeet >= 0.0f ){
                //					if( (groundUnderFeet < Mathf.Abs(distToFall.y)) || Mathf.Abs( groundUnderFeet - Mathf.Abs(distToFall.y)) < 0.01f  ){
                //						lastCatchedClimbHandle = null;
                //						distToFall.y = -groundUnderFeet;
                //						justLanding = true;
                //					}
                //				}
                //			}
                //			
                //			transform.position = transform.position + distToFall;
                //			
                //			if( justLanding ){
                //				
                //				if( zap.landingSound )
                //					zap.getAudioSource().PlayOneShot( zap.landingSound );
                //				
                //				zap.setFuddledFromBrid( false );
                //				
                //				zap.setState(Zap.State.ON_GROUND);
                //				zap.velocity.y = 0.0f;
                //				
                //				Vector3 fallDist = zap.startFallPos - transform.position;
                //				
                //				if( fallDist.y >= VeryHardLandingHeight ){
                //					zap.beforeFallController = null;
                //					zap.die(Zap.DeathType.VERY_HARD_LANDING);
                //				} else if( fallDist.y >= HardLandingHeight ){
                //					
                //					//if( zap.beforeFallController == null ){
                //					zap.velocity.x = 0.0f;
                //					setAction (Action.LANDING_HARD);
                //					//}else{
                //					//}
                //					
                //				}else{
                //					if( zap.beforeFallController == null ){
                //						resetActionAndState();
                //					}else{
                //						zap.restoreBeforeFallController();
                //						//zap.beforeFallController = null;
                //					}
                //				}
                //			}
                break;

            case Zap.State.ON_GROUND:
                float distToGround = 0.0f;
                zap.checkGround(ref distToGround);
                if (zap.groundUnder == null)
                {
                    zap.setState(Zap.State.IN_AIR);
                    setAction(Action.JUMP);
                    wantGetUp = false;
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

            case Zap.State.CLIMB_ROPE:
                Vector3 linkPos = catchedRopeLink.transform.TransformPoint(new Vector3(0.0f, ropeLinkCatchOffset, 0.0f));
                transform.position = linkPos;
                transform.rotation = Quaternion.Lerp(transform.rotation, catchedRopeLink.transform.rotation, 0.1f);
                //transform.rotation = catchedRopeLink.transform.rotation;

                //Quaternion quat = new Quaternion();
                //quat.eulerAngles = new Vector3(0f, 0f, 0f);
                //weaponText.rotation = quat;

                break;
        };

        zap.lastVelocity = zap.velocity;

    }

    public override void FUpdate(float fDeltaTime)
    {
        //Debug.Log("FUpdate : " + fDeltaTime);
        switch(action)
        {
            case Action.PUSH_LEFT:
            case Action.PUSH_RIGHT:
                ActionPush(fDeltaTime);
                break;

            case Action.PULL_LEFT:
            case Action.PULL_RIGHT:
                ActionPull(fDeltaTime);
                break;
        }
    }

    public override void activateSpec(bool restore = false, bool crouch = false)
    {
        //base.activate ();

        setAction(Action.UNDEF);
        setAction(Action.IDLE);
        jumpFromMount = false;
        catchedClimbHandle = null;
        canPullUp = false;
        lastFrameHande = false;
        desiredSpeedX = 0.0f;
        lastHandlePos = new Vector3();
    }
    public override void deactivate()
    {
        base.deactivate();

    }
    public void suddenlyInAir()
    {
        setAction(Action.JUMP);
    }

    //	void SetImpulse(Vector2 imp) { impulse = imp; }
    //	Vector2 getImpulse() { return impulse; }
    //	void addImpulse(Vector3 imp) { impulse += imp; }
    //	void addImpulse(Vector2 imp) { 
    //		impulse.x += imp.x; 
    //		impulse.y += imp.y; 
    //	}

    public enum Action
    {
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
        MOUNT_ATTACK_LEFT,
        MOUNT_ATTACK_RIGHT,
        MOUNT_BIRDHIT,
        ROPECLIMB_IDLE,
        ROPECLIMB_UP,
        ROPECLIMB_DOWN,
        PUSH_LEFT,
        PUSH_RIGHT,
        PULL_LEFT,
        PULL_RIGHT,
        DIE
    };

    Action getAction()
    {
        return action;
    }

    int lastActionParam = 0;
    float maxJumpSpeed = 0f;

    bool setAction(Action newAction, int param = 0)
    {
        if (action == newAction)
            return false;

        

        action = newAction;
        zap.resetCurrentActionTime();
        zap.AnimatorBody.speed = 1f;

        zap.MountAttackLeftCollider.SetActive(false);
        zap.MountAttackRightCollider.SetActive(false);

        lastActionParam = param;
        maxJumpSpeed = 0f;

        //Debug.Log(action);

        switch (newAction)
        {
            case Action.IDLE:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_idle_R");
                else zap.AnimatorBody.Play("Zap_idle_L");
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
                zap.AnimatorBody.Play("Zap_walk_L");
                break;
            case Action.WALK_RIGHT:
                zap.AnimatorBody.Play("Zap_walk_R");
                break;

            case Action.RUN_LEFT:
                zap.AnimatorBody.Play("Zap_run_L");
                break;
            case Action.RUN_RIGHT:
                zap.AnimatorBody.Play("Zap_run_R");
                break;

            case Action.TURN_STAND_LEFT:
                zap.AnimatorBody.Play("Zap_walk_back_left");
                wantJumpAfter = false;
                break;

            case Action.TURN_STAND_RIGHT:
                zap.AnimatorBody.Play("Zap_walk_back_right");
                wantJumpAfter = false;
                break;

            case Action.TURN_RUN_LEFT:
                zap.AnimatorBody.Play("Zap_runback_R");
                wantJumpAfter = false;
                if (zap.turnRunSounds.Length != 0)
                    zap.playSound(zap.turnRunSounds[Random.Range(0, zap.turnRunSounds.Length)]);
                break;

            case Action.TURN_RUN_RIGHT:
                zap.AnimatorBody.Play("Zap_runback_L");
                wantJumpAfter = false;
                if (zap.turnRunSounds.Length != 0)
                    zap.playSound(zap.turnRunSounds[Random.Range(0, zap.turnRunSounds.Length)]);
                break;

            case Action.PREPARE_TO_JUMP:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_jump_in_R");
                else zap.AnimatorBody.Play("Zap_jump_in_L");
                break;

            case Action.JUMP:
                maxJumpSpeed = JumpLongSpeed;
                if (param == 0)
                {
                    if (zap.faceRight()) zap.AnimatorBody.Play("Zap_jump_fly_R");
                    else zap.AnimatorBody.Play("Zap_jump_fly_L");
                }
                else if (param == 1)
                {
                    if (zap.faceRight()) zap.AnimatorBody.Play("zap_rocks_climb_R");
                    else zap.AnimatorBody.Play("zap_rocks_climb_L");
                }
                if (zap.jumpSounds.Length != 0)
                    zap.playSound(zap.jumpSounds[Random.Range(0, zap.jumpSounds.Length)]);
                break;

            case Action.JUMP_LEFT:
            case Action.JUMP_RIGHT:
                maxJumpSpeed = param == 1 ? FromClimbJumpSpeed : JumpSpeed;

                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_run_jump_fly_R");
                else zap.AnimatorBody.Play("Zap_run_jump_fly_L");

                if (zap.jumpSounds.Length != 0)
                    zap.playSound(zap.jumpSounds[Random.Range(0, zap.jumpSounds.Length)]);
                break;

            case Action.JUMP_LEFT_LONG:
            case Action.JUMP_RIGHT_LONG:
                maxJumpSpeed = JumpLongSpeed;

                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_run_jump_fly_R");
                else zap.AnimatorBody.Play("Zap_run_jump_fly_L");

                if (zap.jumpSounds.Length != 0)
                    zap.playSound(zap.jumpSounds[Random.Range(0, zap.jumpSounds.Length)]);
                break;

            case Action.LANDING_HARD:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_landing_hard_R");
                else zap.AnimatorBody.Play("Zap_landing_hard_L");

                if (zap.landingSounds.Length != 0)
                    zap.playSound(zap.landingSounds[Random.Range(0, zap.landingSounds.Length)]);
                break;

            case Action.CLIMB_PREPARE_TO_JUMP:
                break;
            case Action.CLIMB_JUMP_TO_CATCH:
                if (param == 1)
                {
                    if (zap.faceRight()) zap.AnimatorBody.Play("zap_rocks_catch_position_R_2");
                    else zap.AnimatorBody.Play("zap_rocks_catch_position_L_2");
                }
                else
                {
                    if (zap.faceRight()) zap.AnimatorBody.Play("zap_rocks_catch_position_R");
                    else zap.AnimatorBody.Play("zap_rocks_catch_position_L");
                }
                zap.AnimatorBody.speed = 0f;
                break;
            case Action.CLIMB_CATCH:
                if (param == 0)
                {
                    if (zap.faceRight()) zap.AnimatorBody.Play("zap_rocks_catch_position_R");
                    else zap.AnimatorBody.Play("zap_rocks_catch_position_L");
                }
                else if (param == 1)
                {
                    if (zap.faceRight()) zap.AnimatorBody.Play("zap_rocks_catch_position_R_2");
                    else zap.AnimatorBody.Play("zap_rocks_catch_position_L_2");
                }
                else if (param == 10)
                {
                    // tu juz jest we wlasciwej klatce
                    if (zap.faceRight()) zap.AnimatorBody.Play("zap_rocks_catch_position_rev_R");
                    else zap.AnimatorBody.Play("zap_rocks_catch_position_rev_L");
                    zap.AnimatorBody.speed = 0.0f;
                }

                if (zap.catchSounds.Length != 0)
                    zap.playSound(zap.catchSounds[Random.Range(0, zap.catchSounds.Length)]);
                break;
            case Action.CLIMB_CLIMB:
                //Debug.Log(catchedClimbHandle);
                //Debug.Log(lastCatchedClimbHandle);
                zap.TrySetIgnoreCollisionWhit(catchedClimbHandle);
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_jump_climb_R");
                else zap.AnimatorBody.Play("Zap_jump_climb_L");
                break;

            case Action.CLIMB_PULLDOWN:
                //Debug.Log(catchedClimbHandle);
                //Debug.Log(lastCatchedClimbHandle);
                zap.TrySetIgnoreCollisionWhit(catchedClimbHandle);
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_drop_R");
                else zap.AnimatorBody.Play("Zap_drop_L");
                break;

            case Action.MOUNT_IDLE:
                zap.AnimatorBody.Play("Zap_climbmove_up");
                zap.AnimatorBody.speed = 0.0f;
                break;

            case Action.MOUNT_BIRDHIT:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_birdhit_R");
                else zap.AnimatorBody.Play("Zap_birdhit_L");
                break;

            case Action.MOUNT_LEFT:
                zap.AnimatorBody.Play("Zap_climbmove_left");
                break;
            case Action.MOUNT_RIGHT:
                zap.AnimatorBody.Play("Zap_climbmove_right");
                break;
            case Action.MOUNT_UP:
                zap.AnimatorBody.Play("Zap_climbmove_up");
                break;
            case Action.MOUNT_DOWN:
                zap.AnimatorBody.Play("Zap_climbmove_down");
                break;

            case Action.MOUNT_ATTACK_LEFT:
                //Vector2 lCutBegin = zap.leftKnifeHitPointHigh1.position;
                //lCutBegin.x -= 0.5f;
                //lCutBegin.y += 1.3f;
                //Vector2 lCutEnd = zap.leftKnifeHitPointLow2.position;
                //cut(lCutBegin, lCutEnd);

                //lCutBegin = zap.leftKnifeHitPointHigh2.position;
                //lCutBegin.y += 1.25f;
                //lCutEnd = zap.leftKnifeHitPointLow1.position;
                //cut(lCutBegin, lCutEnd);

                zap.MountAttackLeftCollider.SetActive(true);

                zap.AnimatorBody.Play("Zap_climb_knife_attack");
                break;

            case Action.MOUNT_ATTACK_RIGHT:
                //Vector2 rCutBegin = zap.rightKnifeHitPointHigh2.position;
                //rCutBegin.x += 0.5f;
                //rCutBegin.y += 1.3f;
                //Vector2 rCutEnd = zap.rightKnifeHitPointLow1.position;
                //cut(rCutBegin, rCutEnd);

                //rCutBegin = zap.rightKnifeHitPointHigh1.position;
                //rCutBegin.y += 1.25f;
                //rCutEnd = zap.rightKnifeHitPointLow2.position;
                //cut(rCutBegin, rCutEnd);

                zap.MountAttackRightCollider.SetActive(true);

                zap.AnimatorBody.Play("Zap_climb_knife_attack");
                break;

            case Action.CROUCH_IN:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_crouch_in_R");
                else zap.AnimatorBody.Play("Zap_crouch_in_L");
                break;

            case Action.GET_UP:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_getup_R");
                else zap.AnimatorBody.Play("Zap_getup_L");
                break;

            case Action.CROUCH_IDLE:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_crouch_move_R");
                else zap.AnimatorBody.Play("Zap_crouch_move_L");
                zap.AnimatorBody.speed = 0f;
                break;

            case Action.CROUCH_LEFT:
                zap.AnimatorBody.Play("Zap_crouch_move_L");
                break;
            case Action.CROUCH_RIGHT:
                zap.AnimatorBody.Play("Zap_crouch_move_R");
                break;

            case Action.CROUCH_LEFT_BACK:
                zap.AnimatorBody.Play("Zap_crouch_move_back_R");
                break;

            case Action.CROUCH_RIGHT_BACK:
                zap.AnimatorBody.Play("Zap_crouch_move_back_L");
                break;

            case Action.ROPECLIMB_IDLE:
                setActionRopeClimbIdle();
                break;

            case Action.ROPECLIMB_UP:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_liana_climbup_R");
                else zap.AnimatorBody.Play("Zap_liana_climbup_L");
                break;

            case Action.ROPECLIMB_DOWN:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_liana_slide_R");
                else zap.AnimatorBody.Play("Zap_liana_slide_L");
                break;

            case Action.PUSH_LEFT:
                zap.AnimatorBody.Play("Zap_push_L");
                break;

            case Action.PUSH_RIGHT:
                zap.AnimatorBody.Play("Zap_push_R");
                break;

            case Action.PULL_LEFT:
                zap.AnimatorBody.Play("Zap_pull_L");
                break;

            case Action.PULL_RIGHT:
                zap.AnimatorBody.Play("Zap_pull_R");
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
        if (isInState(Zap.State.MOUNT) && isNotInAction(Action.MOUNT_BIRDHIT))
        {
            if (!mounting())
            {
                Vector3 playerPos = transform.position;
                playerPos.y += 0.1f;
                if (zap.checkMount(playerPos))
                {
                    zap.velocity.x = 0.0f;
                    zap.velocity.y = MountSpeed;
                    setAction(Action.MOUNT_UP);
                    return 1;
                }
            }
        }
        else if (isInState(Zap.State.ON_GROUND))
        {
            if (zap.checkMount())
            {
                zap.velocity.x = 0.0f;
                zap.velocity.y = MountSpeed;
                setAction(Action.MOUNT_UP);
                zap.setState(Zap.State.MOUNT);
                return 1;
            }
            if (tryCatchHandle(true))
            {
                return 1;
            }
        }
        return 0;
    }

    public override int keyUpUp()
    {
        if (setMountIdle())
        {
            if (isInState(Zap.State.MOUNT))
            {
                if (Input.GetKey(zap.keyLeft))
                    keyLeftDown();
                else if (Input.GetKey(zap.keyRight))
                    keyRightDown();
                else if (Input.GetKey(zap.keyDown))
                    keyDownDown();
            }
        }
        return 0;
    }

    public override int keyDownDown()
    {
        if (isInState(Zap.State.MOUNT) && isNotInAction(Action.MOUNT_BIRDHIT))
        {
            if (!mounting())
            {
                Vector3 playerPos = transform.position;
                playerPos.y -= 0.1f;
                if (zap.checkMount(playerPos))
                {
                    zap.velocity.x = 0.0f;
                    zap.velocity.y = -MountSpeed;
                    setAction(Action.MOUNT_DOWN);
                    return 1;
                }
            }
        }
        else if (isInState(Zap.State.ON_GROUND))
        {
            if (tryStartClimbPullDown())
            {
                return 1;
            }
            else
            {
                if (!crouching())
                {
                    setAction(Action.CROUCH_IN);
                    return 1;
                }
            }
        }

        return 0;
    }

    public override int keyDownUp()
    {
        if (setMountIdle())
        {
            if (isInState(Zap.State.MOUNT))
            {
                if (Input.GetKey(zap.keyLeft))
                    keyLeftDown();
                else if (Input.GetKey(zap.keyRight))
                    keyRightDown();
                else if (Input.GetKey(zap.keyUp))
                    keyUpDown();
            }
        }
        else if (isInState(Zap.State.ON_GROUND))
        {
            if (crouching() || isInAction(Action.CROUCH_IN))
            {
                if (zap.canGetUp())
                {
                    setAction(Action.GET_UP);
                }
                else
                {
                    wantGetUp = true;
                }
            }
        }
        return 0;
    }

    public override int keyRunDown()
    {
        switch (action)
        {

            case Action.WALK_LEFT:
                if (Input.GetKey(zap.keyLeft))
                {
                    desiredSpeedX = RunSpeed;
                    setAction(Action.RUN_LEFT);
                }
                break;

            case Action.WALK_RIGHT:
                if (Input.GetKey(zap.keyRight))
                {
                    desiredSpeedX = RunSpeed;
                    setAction(Action.RUN_RIGHT);
                }
                break;
        };

        return 0;
    }

    public override int keyRunUp()
    {

        switch (action)
        {

            case Action.RUN_LEFT:
                if (Input.GetKey(zap.keyLeft))
                {
                    desiredSpeedX = WalkSpeed;
                    setAction(Action.WALK_LEFT);
                }
                else
                {
                    desiredSpeedX = 0.0f;
                }
                break;

            case Action.RUN_RIGHT:
                if (Input.GetKey(zap.keyRight))
                {
                    desiredSpeedX = WalkSpeed;
                    setAction(Action.WALK_RIGHT);
                }
                else
                {
                    desiredSpeedX = 0.0f;
                }
                break;
        };

        return 0;
    }

    public override int keyLeftDown()
    {
        if ((isInAction(Action.IDLE) || moving(-1) || jumping()) && isInState(Zap.State.ON_GROUND))
        {

            if (zap.dir() == Vector2.right)
            {
                turnLeftStart();
                return 1;
            }
            else
            {
                float dto = -1;
                Transform obstacle = zap.CheckLeft(0.1f, ref dto);
                if (obstacle)
                {
                    Transform obstacle2 = zap.CheckLeft(0.1f, ref dto, false, true);
                    if (!obstacle2)
                    {
                        //setActionCrouchIdle();
                        setAction(Action.CROUCH_LEFT);
                        //resetActionAndState();
                        zap.velocity.x = -CrouchSpeed;
                        Action_CROUCH_LEFTRIGHT(-1);
                        wantGetUp = true;
                    }
                    else
                    {
                        PushStart(obstacle);
                    }

                    //Transform obstacle2 = zap.CheckLeft(0.1f, ref dto, false, true);
                    //if (!obstacle2)
                    //{
                    //    setActionCrouchIdle();
                    //    resetActionAndState();
                    //    wantGetUp = true;
                    //}
                    //else
                    //{
                    //    PushStart(obstacle);
                    //}
                    
                    return 0;
                }
            }
            
            if (zap.dir() == -Vector2.right)
            {
                if (Input.GetKey(zap.keyRun))
                {
                    desiredSpeedX = RunSpeed;
                    speedLimiter(-1, desiredSpeedX + 1.0f);
                    setAction(Action.RUN_LEFT);
                    return 1;
                }
                else
                {
                    desiredSpeedX = WalkSpeed;
                    speedLimiter(-1, desiredSpeedX + 1.0f);
                    setAction(Action.WALK_LEFT);
                    return 1;
                }
            }
            else
            {
                turnLeftStart();
                return 1;
            }
        }
        else if (isInState(Zap.State.MOUNT) && isNotInAction(Action.MOUNT_BIRDHIT))
        {
            if (!mounting())
            {
                Vector3 playerPos = transform.position;
                playerPos.x -= 0.1f;
                zap.turnLeft();
                if (zap.checkMount(playerPos))
                {
                    zap.velocity.x = -MountSpeed;
                    zap.velocity.y = 0.0f;
                    setAction(Action.MOUNT_LEFT);
                    return 1;
                }
            }
        }
        else if (isInAction(Action.CROUCH_IDLE) && isInState(Zap.State.ON_GROUND))
        {
            if (zap.CheckLeft(0.1f) >= 0.0f)
            {
                return 0;
            }
            desiredSpeedX = CrouchSpeed;
            if (zap.dir() == -Vector2.right)
            {
                setAction(Action.CROUCH_LEFT);
            }
            else
            {
                setAction(Action.CROUCH_LEFT_BACK);
            }
            return 1;
        }
        return 0;
    }

    public override int keyRightDown()
    {
        if ((isInAction(Action.IDLE) || moving(1) || jumping()) && isInState(Zap.State.ON_GROUND))
        {
            if (zap.dir() == -Vector2.right)
            {
                turnRightStart();
                return 1;
            }
            else
            {
                //setAction(Action.PUSH_RIGHT);
                float dto = -1;
                Transform obstacle = zap.CheckRight(0.1f, ref dto);
                if (obstacle)
                {
                    Transform obstacle2 = zap.CheckRight(0.1f, ref dto, false, true);
                    if (!obstacle2)
                    {
                        //setActionCrouchIdle();
                        setAction(Action.CROUCH_RIGHT);
                        //resetActionAndState();
                        zap.velocity.x = CrouchSpeed;
                        Action_CROUCH_LEFTRIGHT(1);
                        wantGetUp = true;
                    }
                    else
                    {
                        PushStart(obstacle);
                    }

                    return 0;
                }
            }

            if (zap.dir() == Vector2.right)
            {
                if (Input.GetKey(zap.keyRun))
                {
                    desiredSpeedX = RunSpeed;
                    speedLimiter(1, desiredSpeedX + 1.0f);
                    setAction(Action.RUN_RIGHT);
                    return 1;
                }
                else
                {
                    desiredSpeedX = WalkSpeed;
                    speedLimiter(1, desiredSpeedX + 1.0f);
                    setAction(Action.WALK_RIGHT);
                    return 1;
                }
            }
            else
            {
                turnRightStart();
                return 1;
            }
        }
        else if (isInState(Zap.State.MOUNT) && isNotInAction(Action.MOUNT_BIRDHIT))
        {
            if (!mounting())
            {
                Vector3 playerPos = transform.position;
                playerPos.x += 0.1f;
                zap.turnRight();
                if (zap.checkMount(playerPos))
                {
                    zap.velocity.x = MountSpeed;
                    zap.velocity.y = 0.0f;
                    setAction(Action.MOUNT_RIGHT);
                    return 1;
                }
            }
        }
        else if (isInAction(Action.CROUCH_IDLE) && isInState(Zap.State.ON_GROUND))
        {
            if (zap.CheckRight(0.1f) >= 0.0f)
            {
                return 0;
            }
            desiredSpeedX = CrouchSpeed;
            if (zap.dir() == Vector2.right)
            {
                setAction(Action.CROUCH_RIGHT);
            }
            else
            {
                setAction(Action.CROUCH_RIGHT_BACK);
            }
            return 1;
        }
        return 0;
    }

    public override int keyLeftUp()
    {
        if (!setMountIdle())
        {
            if (isInState(Zap.State.ON_GROUND))
            {
                if (isInAction(Action.PUSH_LEFT) || isInAction(Action.PULL_LEFT))
                {
                    setActionIdle();
                }
                else if(isInAction(Action.PULL_LEFT))
                {
                    PullStop();
                }
                else
                {
                    desiredSpeedX = 0.0f;
                }
            }
        }
        else
        {
            if (isInState(Zap.State.MOUNT))
            {
                if (Input.GetKey(zap.keyRight))
                    keyRightDown();
                else if (Input.GetKey(zap.keyUp))
                    keyUpDown();
                else if (Input.GetKey(zap.keyDown))
                    keyDownDown();
            }
        }

        return 0;
    }

    public override int keyRightUp()
    {
        if (!setMountIdle())
        {
            if (isInState(Zap.State.ON_GROUND))
            {
                if (isInAction(Action.PUSH_RIGHT))
                {
                    setActionIdle();
                }
                if (isInAction(Action.PULL_RIGHT))
                {
                    PullStop();
                }
                else
                {
                    desiredSpeedX = 0.0f;
                }
            }
        }
        else
        {
            if (isInState(Zap.State.MOUNT))
            {
                if (Input.GetKey(zap.keyLeft))
                    keyLeftDown();
                else if (Input.GetKey(zap.keyUp))
                    keyUpDown();
                else if (Input.GetKey(zap.keyDown))
                    keyDownDown();
            }
           
        }

        return 0;
    }

    //bool jumpKeyPressed = false;

    bool canJump()
    {
        //if (zap.groundUnder == null) return false;
        //float groundUnderAngle = zap.groundUnder.eulerAngles.z % 90;
        ////float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation) % 90;
        //if (groundUnderAngle < -60.0f || groundUnderAngle > 60.0f)
        //    return false;
        float distToCeil = 0.2f;
        if (zap.checkCeil(ref distToCeil))
        {
            return false;
        }

        return true;
    }

    float getGroundUnderAngle()
    {

        return 0;
    }
    // <0;-1> w lewo <0;1> w prawo 0 nie...
    float shouldSlideDown()
    {
        if (zap.groundUnder == null) return 0f;
        int groundUnderAngleQuarter = (int)zap.groundUnder.eulerAngles.z / 90;
        //if (groundUnderAngle < -60.0f || groundUnderAngle > 60.0f)
        //  return true;


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
                    if (canJump())
                        preparetojump();
                }
                break;

            case Action.WALK_LEFT:
                if (canJump())
                    jumpLeft();
                break;
            case Action.WALK_RIGHT:
                if (canJump())
                    jumpRight();
                break;

            case Action.RUN_LEFT:
                if (canJump())
                    jumpLongLeft();
                break;
            case Action.RUN_RIGHT:
                if (canJump())
                    jumpLongRight();
                break;

            case Action.MOUNT_IDLE:
            case Action.MOUNT_UP:
            case Action.MOUNT_DOWN:

                lastFrameHande = false;
                mountJumpStartPos = transform.position;
                jumpFromMount = true;
                justJumpedMount = true;

                if (Input.GetKey(zap.keyLeft))
                {
                    jumpLeft();
                    return 0;
                }

                if (Input.GetKey(zap.keyRight))
                {
                    jumpRight();
                    return 0;
                }

                zap.velocity.x = 0.0f;
                zap.velocity.y = 0.0f;
                setAction(Action.JUMP);
                zap.setState(Zap.State.IN_AIR);

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

        return 0;
    }

    public override int keyJumpUp()
    {
        //jumpKeyPressed = false;
        jumpFromMount = false;
        justJumpedRope = null;
        canJumpAfter = true;
        return 0;
    }

    int Action_IDLE()
    {

        if (Input.GetMouseButtonDown(0))
        {

            //zap._pullOutKnife();
            //zap._pullOutGravityGun();
            return zap.pullChoosenWeapon();
            //return 1;
        }

        return 0;
    }

    int Action_WALK(int dir)
    {

        if (Input.GetButtonDown("Fire1"))
        {
            int pullRes = zap.pullChoosenWeapon();
            if (pullRes != 0)
                return pullRes;
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
        Transform obstacle = zap.CheckObstacle(dir, distToMove, ref distToObstacle);
        if (obstacle)
        {
            Transform obstacle2 = zap.CheckObstacle(dir, distToMove, ref distToObstacle, true);
            if (!obstacle2)
            {
                setActionCrouchIdle();
                resetActionAndState();
                wantGetUp = true;
            }
            else
            {
                distToMove = distToObstacle;
                //setActionIdle();
                PushStart(obstacle);
            }
        }

        //Debug.Log(distToObstacle);

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);
        
        return 0;
    }

    int Action_RUN(int dir)
    {

        if (Input.GetMouseButtonDown(0))
        {
            int pullRes = zap.pullChoosenWeapon();
            if (pullRes != 0)
                return pullRes;
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
            setAction(Action.IDLE);
            resetActionAndState();
            return 0;
        }

        float speedRatio = (Mathf.Abs(zap.velocity.x) / RunSpeed);
        bool turnBackHard = speedRatio > 0.5f;

        if (turnBackHard)
        {

            if (dir == 1)
            {

                if ((Input.GetKeyDown(zap.keyLeft) || Input.GetKey(zap.keyLeft)) &&
                   (Input.GetKeyUp(zap.keyRight) || !Input.GetKey(zap.keyRight))
                   )
                {
                    setAction(Action.TURN_RUN_LEFT);
                }

            }
            else if (dir == -1)
            {

                if ((Input.GetKeyDown(zap.keyRight) || Input.GetKey(zap.keyRight)) &&
                   (Input.GetKeyUp(zap.keyLeft) || !Input.GetKey(zap.keyLeft))
                   )
                {
                    setAction(Action.TURN_RUN_RIGHT);
                }
            }
        }

        distToMove = zap.velocity.x * zap.getCurrentDeltaTime();

        zap.AnimatorBody.speed = 0.5f + (Mathf.Abs(zap.velocity.x) / RunSpeed) * 0.5f;

        float distToObstacle = 0.0f;
        //if (zap.CheckObstacle(dir, distToMove, ref distToObstacle))
        Transform obstacle = zap.CheckObstacle(dir, distToMove, ref distToObstacle);
        if( obstacle )
        {
            //distToMove = distToObstacle;
            ////setActionIdle();
            //PushStart(obstacle);

            Transform obstacle2 = zap.CheckObstacle(dir, distToMove, ref distToObstacle, true);
            if (!obstacle2)
            {
                setActionCrouchIdle();
                resetActionAndState();
                wantGetUp = true;
            }
            else
            {
                distToMove = distToObstacle;
                //setActionIdle();
                PushStart(obstacle);
            }
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);
        
        return 0;
    }

    int Action_TURN_RUN(int dir)
    {

        int retVal = 0;

        if (Input.GetKeyDown(zap.keyJump))
        {
            wantJumpAfter = true;
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
        }

        distToMove = zap.velocity.x * zap.getCurrentDeltaTime();

        float distToObstacle = 0.0f;
        if (zap.CheckObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            retVal = 1;
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        //float distToGround = 0.0f;
        //bool groundUnderFeet = zap.checkGround(false, zap.layerIdGroundAllMask, ref distToGround);
        //if (groundUnderFeet)
        //{
        //    transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        //}

        return retVal;
    }

    int Action_CROUCH_IN()
    {
        if (zap.currentActionTime >= CrouchInOutDuration)
        {
            crouch();
        }
        return 0;
    }

    int Action_GET_UP()
    {
        if (zap.currentActionTime >= CrouchInOutDuration)
        {
            getUp();
        }

        return 0;
    }

    int Action_CROUCH_IDLE()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return zap.pullChoosenWeapon(true);
        }

        if (Input.GetKey(zap.keyDown))
        {
            tryStartClimbPullDown();
        }
        return 0;
    }

    int Action_CROUCH_LEFTRIGHT(int dir)
    {
        if (Input.GetKey(zap.keyDown))
        {
            if( tryStartClimbPullDown() )
                return 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            return zap.pullChoosenWeapon(true);
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
            //Debug.Log("speedReached => " + zap.velocity.x);
            setActionCrouchIdle();
            if (crouching())
            {
                if (Input.GetKey(zap.keyLeft))
                {
                    keyLeftDown();
                }
                else if (Input.GetKey(zap.keyRight))
                {
                    keyRightDown();
                }
            }
        }

        distToMove = zap.velocity.x * zap.getCurrentDeltaTime();

        float distToObstacle = 0.0f;
        if (zap.CheckObstacle(dir, distToMove, ref distToObstacle))
        {
            //Debug.Log("obstacle -> " + distToObstacle);
            distToMove = distToObstacle;
            setActionCrouchIdle();
        }

        //Debug.Log("zap.velocity.x :  " + zap.velocity.x);

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

    bool slideDown()
    {
        float ssd = shouldSlideDown();
        if (ssd == 0f) return false;

        //float distToGround = 0.0f;
        //bool groundUnderFeet = zap.checkGround(false, zap.layerIdGroundAllMask, ref distToGround);
        //if (groundUnderFeet)
        //{
        //    transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        //}

        return true;
    }

    bool tryMountAttackStart()
    {
        if ( isNotInAction(Action.MOUNT_BIRDHIT) && zap.HaveKnife && Input.GetMouseButtonDown(0))
        {
            Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
            if (mouseInScene.x < transform.position.x)
            {
                if (zap.faceRight())
                {
                    zap.turnLeft();
                }
                setAction(Action.MOUNT_ATTACK_LEFT);
            }
            else
            {
                if (!zap.faceRight())
                {
                    zap.turnRight();
                }
                setAction(Action.MOUNT_ATTACK_RIGHT);
            }
            return true;
        }
        return false;
    }

    int Action_MOUNT_IDLE()
    {
        if (tryMountAttackStart())
        {
            return 0;
        }
        return 0;
    }

    int Action_MOUNT_BIRDHIT()
    {
        if( zap.currentActionTime > BIRD_HIT_DURATION )
        {
            if (zap.canBeFuddleFromBird || lastActionParam == 1) // 1 oznacza ze bat to byl...
                zap.FuddleFromBird = true;

            zap.velocity.x = 0.0f;
            zap.velocity.y = 0.0f;
            setAction(Action.JUMP);
            zap.setState(Zap.State.IN_AIR);           
        }
        return 0;
    }

    int Action_MOUNTING()
    {
        if (tryMountAttackStart())
        {
            return 0;
        }

        Vector3 newPos3 = transform.position;
        Vector3 distToMount = zap.velocity * zap.getCurrentDeltaTime();
        newPos3 += distToMount;
        if (zap.checkMount(newPos3))
        {
            transform.position = newPos3;
        }
        else
        {
            setMountIdle();
        }
        return 0;
    }

    int Action_MOUNTING_DOWN()
    {
        if (tryMountAttackStart())
        {
            return 0;
        }

        Vector3 newPos3 = transform.position;
        Vector3 distToMount = zap.velocity * zap.getCurrentDeltaTime();
        newPos3 += distToMount;

        if (distToMount.y < 0.0f)
        { // schodzi
            groundUnderFeet = zap.checkDown(Mathf.Abs(distToMount.y) + 0.01f);
            if (groundUnderFeet >= 0.0f)
            {
                if (groundUnderFeet < Mathf.Abs(distToMount.y))
                {
                    distToMount.y = -groundUnderFeet;
                    zap.velocity.x = 0.0f;
                    zap.velocity.y = 0.0f;
                    zap.setState(Zap.State.ON_GROUND);
                    setAction(Action.IDLE);
                    transform.position = transform.position + distToMount;
                }
            }
            else
            {
                if (zap.checkMount(newPos3))
                    transform.position = newPos3;
                else
                    setMountIdle();
            }
        }
        return 0;
    }

    int Action_MOUNT_ATTACK()
    {
        if (zap.currentActionTime >= MOUNT_ATTACK_DURATION)
        {
            zap.MountAttackLeftCollider.SetActive(false);
            zap.MountAttackRightCollider.SetActive(false);

            if (isInState(Zap.State.MOUNT))
            {
                setAction(Action.MOUNT_IDLE);
                if (Input.GetKey(zap.keyLeft))
                    keyLeftDown();
                else if (Input.GetKey(zap.keyRight))
                    keyRightDown();
                else if (Input.GetKey(zap.keyUp))
                    keyUpDown();
                else if (Input.GetKey(zap.keyDown))
                    keyDownDown();
                else
                    setMountIdle();
            }
        }
        return 0;
    }

    int Action_ROPECLIMB_IDLE(float deltaTime)
    {

        if (!catchedRope)
            return 0;

        bool _swing = false;

        if (Input.GetKey(zap.keyLeft))
        {

            float fla = catchedRope.firstLinkAngle;

            if (fla > -20f && fla < 0f)
            {
                //print ( "Rope swing : " + fla );
                catchedRope.swing(-Vector2.right, RopeSwingForce * zap.getCurrentDeltaTime());
                _swing = true;
            }

            //if( _swing ){
            if (zap.dir() == Vector2.right)
            {

                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_liana_swingback_R");
                else zap.AnimatorBody.Play("Zap_liana_swingback_L");
                zap.AnimatorBody.speed = 1f;

            }
            else
            {

                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_liana_swingfront_R");
                else zap.AnimatorBody.Play("Zap_liana_swingfront_L");
                zap.AnimatorBody.speed = 1f;

            }

            //if (ropeSwingSound != null)
            //{
            //    zap.getAudioSource().clip = ropeSwingSound;
            //    zap.getAudioSource().isPlaying();
            //}

            //}
        }
        else if (Input.GetKey(zap.keyRight))
        {

            float fla = catchedRope.firstLinkAngle;

            if (fla < 20f && fla >= 0f)
            {
                //print ( "Rope swing : " + fla );
                catchedRope.swing(Vector2.right, RopeSwingForce * zap.getCurrentDeltaTime());
                _swing = true;
            }

            //if( _swing ){
            if (zap.dir() == Vector2.right)
            {

                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_liana_swingfront_R");
                else zap.AnimatorBody.Play("Zap_liana_swingfront_L");
                zap.AnimatorBody.speed = 1f;

            }
            else
            {

                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_liana_swingback_R");
                else zap.AnimatorBody.Play("Zap_liana_swingback_L");
                zap.AnimatorBody.speed = 1f;

            }
            //}
        }

        if (Input.GetKeyUp(zap.keyLeft) || Input.GetKeyUp(zap.keyRight))
        { //|| !_swing) {
            setActionRopeClimbIdle();
        }

        if (tryJumpFromRope() != 0)
        {
            return 0;
        }

        if (Input.GetKey(zap.keyUp))
        {

            if (canRopeClimbUp())
            {
                setAction(Action.ROPECLIMB_UP);
            }

        }
        else if (Input.GetKey(zap.keyDown))
        {

            if (canRopeClimbDown())
            {
                setAction(Action.ROPECLIMB_DOWN);
            }
        }

        tryBreakUpRope(deltaTime);

        return 0;
    }
    int Action_LANDING_HARD()
    {
        if (zap.currentActionTime >= LANDING_HARD_DURATION)
        {
            if (zap.beforeFallController == null)
            {
                setAction(Action.IDLE);
                resetActionAndState();
            }
            else
            {
                zap.restoreBeforeFallController();
            }
        }

        return 0;
    }

    int Action_CLIMB_PULLDOWN()
    {
        if (zap.currentActionTime >= CLIMBDUR_CLIMB)
        {
            zap.removeLastIgnoredCollision();
            setAction(Action.CLIMB_CATCH, 10);
            zap.setState(Zap.State.CLIMB);
            canPullUp = true;
            transform.position = climbAfterPos;
        }
        else
        {
            float ratio = zap.currentActionTime / CLIMBDUR_CLIMB;
            transform.position = climbBeforePos + climbDistToClimb * ratio;
        }

        return 0;
    }

    int Action_CLIMB_JUMP_TO_CATCH()
    {
        // dociaganie do punktu:
        if (zap.currentActionTime >= climbToJumpDuration)
        {
            setAction(Action.CLIMB_CATCH, lastActionParam);
            transform.position = climbAfterPos;
        }
        else
        {
            float ratio = zap.currentActionTime / climbToJumpDuration;
            transform.position = climbBeforePos + climbDistToClimb * ratio;
        }

        return 0;
    }

    int Action_CLIMB_CATCH()
    {
        //if (zap.groundUnder)
        //{
        //    if (distToGround != 0f)
        //    {
        //        transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        //        zap.touchStone(zap.groundUnder);
        //    }
        //    else
        //    {
        //        zap.touchStone(zap.groundUnder);
        //    }
        //}
        //Debug.Log(climbAfterPos2);
        zap.touchStone(catchedClimbHandle.transform,climbAfterPos2);

        if ((Input.GetKeyDown(zap.keyUp) || Input.GetKey(zap.keyUp)) ) // && canPullUp)
        {
            if (canClimbPullUp2())
            {
                //climbBeforePos = transform.position;
                //climbAfterPos = newPos;
                //climbAfterPos2 = handlePos;
                //climbDistToClimb = climbAfterPos - climbBeforePos;
                //climbToJumpDuration = climbDistToClimb.magnitude * _speed;

                climbBeforePos = transform.position;
                climbDistToClimb = climbAfterPos2 - climbBeforePos;

                setAction(Action.CLIMB_CLIMB);
                catchedClimbHandle = null;
                lastCatchedClimbHandle = null;
                return 0;
            }
        }

        if (Input.GetKeyDown(zap.keyJump))
        {
            if (zap.dir() == Vector2.right && Input.GetKey(zap.keyLeft))
            {
                zap.turnLeft();
                jumpLeft(true);
                catchedClimbHandle = null;
                lastCatchedClimbHandle = null;
            }
            else if (Input.GetKey(zap.keyRight))
            {
                zap.turnRight();
                jumpRight(true);
                catchedClimbHandle = null;
                lastCatchedClimbHandle = null;
            }
            else if (Input.GetKey(zap.keyDown))
            {
                zap.velocity.x = 0.0f;
                zap.velocity.y = 0.0f;
                zap.setState(Zap.State.IN_AIR);
                setAction(Action.JUMP);
                lastCatchedClimbHandle = catchedClimbHandle;
                catchedClimbHandle = null;
            }
            else
            {
                jumpFromClimb();
                lastCatchedClimbHandle = catchedClimbHandle;
                catchedClimbHandle = null;
            }
        }

        return 0;
    }

    int Action_CLIMB_CLIMB()
    {

        if (zap.currentActionTime >= CLIMBDUR_CLIMB)
        {
            zap.removeLastIgnoredCollision();

            zap.setState(Zap.State.ON_GROUND);
            transform.position = climbAfterPos2;

            if (zap.canGetUp())
            {
                setAction(Action.IDLE);
                resetActionAndState();
            }
            else
            {
                setAction(Action.CROUCH_IDLE);
                wantGetUp = !Input.GetKey(zap.keyDown);

                if (Input.GetKey(zap.keyLeft))
                {
                    keyLeftDown();
                }
                else if (Input.GetKey(zap.keyRight))
                {
                    keyRightDown();
                }
            }

        }
        else
        {
            float ratio = zap.currentActionTime / CLIMBDUR_CLIMB;
            transform.position = climbBeforePos + climbDistToClimb * ratio;
        }

        return 0;
    }

    int Action_ROPECLIMB_UP(float deltaTime)
    {

        if (!catchedRope)
            return 0;

        if (Input.GetKeyUp(zap.keyUp))
        {
            setAction(Action.ROPECLIMB_IDLE);
            return 0;
        }

        float climbDist = RopeClimbSpeedUp * zap.getCurrentDeltaTime();

        float newRopeLinkCatchOffset = ropeLinkCatchOffset + climbDist;
        // zakladam ze nie przebedzie wiecej niz jednego ogniwa w klatce...

        if (newRopeLinkCatchOffset > 0.0f) // przekroczyłem ogniwo w gore...
        {
            if (catchedRopeLink.transform.parent)
            { // jak ogniwo ma rodzica to przechodze wyzej 

                catchedRopeLink = catchedRopeLink.transform.parent.GetComponent<RopeLink>();
                catchedRope.chooseDriver(catchedRopeLink.transform);
                ropeLinkCatchOffset = -0.5f - newRopeLinkCatchOffset;

            }
            else
            {
                ropeLinkCatchOffset = 0.0f;
                setAction(Action.ROPECLIMB_IDLE);
            }

        }
        else
        {

            ropeLinkCatchOffset = newRopeLinkCatchOffset;
        }

        tryBreakUpRope(deltaTime);

        return 0;
    }

    Vector2 lastPulledObstaclePos = new Vector2(0f, 0f);

    int ActionPush(float deltaTime)
    {
        //Debug.Log("ActionPush : " + pushPullObstacle);

        if (pushPullObstacle)
        {
            Rigidbody2D obstacleBody = pushPullObstacle.GetComponent<Rigidbody2D>();
            if (obstacleBody)
            {
                if( zap.faceRight() )
                {
                    if( Input.GetKeyDown(zap.keyLeft) || Input.GetKey(zap.keyLeft))
                    {
                        obstacleBody.velocity = new Vector2(0f, obstacleBody.velocity.y);
                        lastPulledObstaclePos = obstacleBody.position;
                        setAction(Action.PULL_LEFT);
                        return 1;
                    }
                }
                else
                {
                    if (Input.GetKeyDown(zap.keyRight) || Input.GetKey(zap.keyRight))
                    {
                        obstacleBody.velocity = new Vector2(0f, obstacleBody.velocity.y);
                        lastPulledObstaclePos = obstacleBody.position;
                        setAction(Action.PULL_RIGHT);
                        return 1;
                    }
                }

                zap.velocity.x = PushMaxSpeed * zap.dir2();
                distToMove = zap.velocity.x * deltaTime;
                
                float distToObstacle = 0.0f;
                Transform obstacle = zap.CheckObstacle(zap.dir2(), distToMove + 0.1f + zap.dir2(), ref distToObstacle);
                if (obstacle != pushPullObstacle)
                {
                    setActionIdle();
                    return 1;
                }

                //Debug.Log("Push : " + distToMove + " " + distToObstacle);

                if (Mathf.Abs(distToMove) < Mathf.Abs(distToObstacle))
                {
                    zap.velocity.x = distToMove / deltaTime;
                }
                else
                {
                    distToMove = distToObstacle;
                    zap.velocity.x = distToMove / deltaTime;


                    Vector2 force = new Vector2(0f, 0f);
                    Vector2 forcePos;
                    if (zap.faceRight())
                    {
                        force.x = pushForce /** deltaTime*/;
                        forcePos = zap.sensorRight2.position;
                        forcePos.x += 0.4f;
                        forcePos.y -= 0.3f;
                    }
                    else
                    {
                        force.x = -pushForce /** deltaTime*/;
                        forcePos = zap.sensorLeft2.position;
                        forcePos.x -= 0.4f;
                        forcePos.y -= 0.3f;
                    }
                    obstacleBody.AddForceAtPosition(force, forcePos, ForceMode2D.Force);
                }

                //distToMove = Mathf.Min(distToMove,distToObstacle);
                //zap.velocity.x = distToMove / deltaTime;

                //zap.velocity.x = Mathf.Min(Mathf.Abs(zap.velocity.x)) * zap.dir2();

                zap.AnimatorBody.speed = 0.5f + (Mathf.Abs(zap.velocity.x) / PushMaxSpeed) * 0.5f;
                
                newPosX += distToMove;
                transform.position = new Vector3(newPosX, oldPos.y, 0.0f);
                
                //if (distToObstacle < 0.15f)
                //{
                //    Vector2 force = new Vector2(0f, 0f);
                //    Vector2 forcePos;
                //    if (zap.faceRight())
                //    {
                //        force.x = pushForce/* * deltaTime*/;
                //        forcePos = zap.sensorRight2.position;
                //    }
                //    else
                //    {
                //        force.x = -pushForce/* * deltaTime*/;
                //        forcePos = zap.sensorLeft2.position;
                //    }
                //    obstacleBody.AddForceAtPosition(force, forcePos, ForceMode2D.Force);
                //}
                //return 0;
            }
        }
        return 0;
    }
    int ActionPull(float deltaTime)
    {
        if (pushPullObstacle)
        {
            Rigidbody2D obstacleBody = pushPullObstacle.GetComponent<Rigidbody2D>();
            if (obstacleBody)
            {
                //newPosX += distToMove;
                float _diffx  = obstacleBody.position.x - lastPulledObstaclePos.x;
                float obstacleOnRoad = -1;
                if (zap.faceRight())
                {
                    obstacleOnRoad = zap.CheckLeft(Mathf.Abs(_diffx) + 0.1f);
                }
                else
                {
                    obstacleOnRoad = zap.CheckRight(Mathf.Abs(_diffx) + 0.1f);
                }

                if (obstacleOnRoad > 0f)
                {
                    //transform.position = new Vector3(transform.position.x + _diffx, transform.position.y, 0.0f);
                    PullStop();
                }
                else
                {
                    transform.position = new Vector3(transform.position.x + _diffx, transform.position.y, 0.0f);
                    
                    if (Mathf.Abs(obstacleBody.velocity.x) < PullMaxSpeed)
                    {
                        Vector2 force = new Vector2(0f, 0f);
                        Vector2 forcePos;
                        if (zap.faceRight())
                        {
                            force.x = -pullForce/* * deltaTime*/;
                            forcePos = zap.sensorRight2.position;
                            forcePos.x += 0.4f;
                            forcePos.y -= 0.3f;
                        }
                        else
                        {
                            force.x = pullForce/* * deltaTime*/;
                            forcePos = zap.sensorLeft2.position;
                            forcePos.x -= 0.4f;
                            forcePos.y -= 0.3f;
                        }
                        obstacleBody.AddForceAtPosition(force, forcePos, ForceMode2D.Force);
                    }

                    lastPulledObstaclePos = obstacleBody.position;
                }
            }
        }
        return 0;
    }
    void PushStart(Transform obstacle)
    {
        //Debug.Log("PushStart : " + obstacle);
        zap.velocity.x = 0.0f;
        pushPullObstacle = obstacle;
        if (zap.faceRight())
            setAction(Action.PUSH_RIGHT);
        else
            setAction(Action.PUSH_LEFT);
    }
    void PullStart(Transform obstacle)
    {
        zap.velocity.x = 0.0f;
        pushPullObstacle = obstacle;
        if (zap.faceRight())
            setAction(Action.PULL_RIGHT);
        else
            setAction(Action.PULL_LEFT);
    }
    void PushStop()
    {
        
    }
    void PullStop()
    {
        if (pushPullObstacle)
        {
            Rigidbody2D obstacleBody = pushPullObstacle.GetComponent<Rigidbody2D>();
            if (obstacleBody)
            {
                obstacleBody.velocity = new Vector2(0f, 0f);
            }
        }
        pushPullObstacle = null;
        setActionIdle();
    }

    int Action_ROPECLIMB_DOWN(float deltaTime)
    {

        if (!catchedRope)
            return 0;

        if (Input.GetKeyUp(zap.keyDown))
        {
            setAction(Action.ROPECLIMB_IDLE);
            return 0;
        }

        if (tryJumpFromRope() != 0)
        {
            return 0;
        }

        float climbDist = RopeClimbSpeedDown * zap.getCurrentDeltaTime();

        float newRopeLinkCatchOffset = ropeLinkCatchOffset - climbDist;
        // zakladam ze nie przebedzie wiecej niz jednego ogniwa w klatce...

        if (newRopeLinkCatchOffset <= -0.5f) // przekroczyłem ogniwo w gore...
        {
            if (catchedRopeLink.transform.childCount > 0)
            { // jak ogniwo ma dzicko to przechodze niżej 

                bool _asdf = true;

                //				if( catchedRopeLink.transform.parent ){
                //					HingeJoint2D parentHingeJoint = catchedRopeLink.transform.parent.GetComponent<HingeJoint2D>();
                //					_asdf = parentHingeJoint.enabled;
                //				}

                if (_asdf && catchedRopeLink.transform.GetChild(0).transform.childCount > 0)
                { // chyba ze to jest ostatnie ogniwo

                    RopeLink newCatchedRopeLink = catchedRopeLink.transform.GetChild(0).GetComponent<RopeLink>();
                    //HingeJoint2D catchedHingeJoint = catchedRopeLink.GetComponent<HingeJoint2D>();
                    //if( catchedHingeJoint.enabled ){
                    if (catchedRope.chooseDriver(newCatchedRopeLink.transform))
                    {
                        catchedRopeLink = newCatchedRopeLink;
                        ropeLinkCatchOffset = newRopeLinkCatchOffset + 0.5f;
                    }
                    else
                    {
                        ropeLinkCatchOffset = -0.5f;
                        setAction(Action.ROPECLIMB_IDLE);
                    }
                    //}else{
                    //	ropeLinkCatchOffset = -0.5f;
                    //	setAction(Action.ROPECLIMB_IDLE);
                    //}
                }
                else
                {
                    ropeLinkCatchOffset = -0.5f;
                    setAction(Action.ROPECLIMB_IDLE);
                }

            }
            else
            {
                ropeLinkCatchOffset = -0.5f;
                setAction(Action.ROPECLIMB_IDLE);
            }

        }
        else
        {

            ropeLinkCatchOffset = newRopeLinkCatchOffset;
        }

        tryBreakUpRope(deltaTime);

        return 0;
    }

    void crouch()
    {
        if (isInState(Zap.State.ON_GROUND))
        {

            switch (action)
            {

                case Action.IDLE:
                case Action.JUMP:
                case Action.CROUCH_IN:
                    setAction(Action.CROUCH_IDLE);
                    if (Input.GetKey(zap.keyLeft))
                    {
                        keyLeftDown();
                    }
                    else if (Input.GetKey(zap.keyRight))
                    {
                        keyRightDown();
                    }
                    else
                    {
                        zap.velocity.x = 0.0f;
                        zap.velocity.y = 0.0f;
                    }
                    break;

                case Action.WALK_LEFT:
                case Action.RUN_LEFT:
                case Action.JUMP_LEFT:
                case Action.JUMP_LEFT_LONG:
                    if (Input.GetKey(zap.keyLeft))
                    {
                        zap.velocity.x = 0.0f;
                        desiredSpeedX = CrouchSpeed;
                        if (zap.dir() == -Vector2.right)
                        {
                            setAction(Action.CROUCH_LEFT);
                        }
                        else
                        {
                            setAction(Action.CROUCH_LEFT_BACK);
                        }
                    }
                    else
                    {
                        zap.velocity.x = 0.0f;
                        zap.velocity.y = 0.0f;
                        setAction(Action.CROUCH_IDLE);
                    }
                    break;

                case Action.WALK_RIGHT:
                case Action.RUN_RIGHT:
                case Action.JUMP_RIGHT:
                case Action.JUMP_RIGHT_LONG:
                    if (Input.GetKey(zap.keyRight))
                    {
                        zap.velocity.x = 0.0f;
                        desiredSpeedX = CrouchSpeed;
                        if (zap.dir() == Vector2.right)
                        {
                            setAction(Action.CROUCH_RIGHT);
                        }
                        else
                        {
                            setAction(Action.CROUCH_RIGHT_BACK);
                        }
                    }
                    else
                    {
                        zap.velocity.x = 0.0f;
                        zap.velocity.y = 0.0f;
                        setAction(Action.CROUCH_IDLE);
                    }
                    break;
            }

        }
    }

    void preparetojump()
    {
        if (isNotInState(Zap.State.ON_GROUND) || isNotInAction(Action.IDLE))
            return;

        zap.velocity.x = 0.0f;
        zap.velocity.y = 0.0f;
        setAction(Action.PREPARE_TO_JUMP);
    }

    void jump()
    {
        zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.JUMP);

        lastFrameHande = false;
    }

    void jumpFromClimb()
    {
        //zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
        zap.AddImpulse(new Vector2(0.0f, FromClimbJumpImpulse));
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.JUMP, 1);
        lastFrameHande = false;
    }

    void jumpLeft(bool fromClimb = false)
    {
        zap.velocity.y = 0.0f;
        if (fromClimb)
        {
            zap.velocity.x = -FromClimbJumpSpeed;
            zap.AddImpulse(new Vector2(0.0f, FromClimbJumpSideImpulse));
        }
        else
        {
            zap.velocity.x = -JumpSpeed;
            zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
        }
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.JUMP_LEFT, fromClimb ? 1 : 0);

        lastFrameHande = false;
    }

    void jumpRight(bool fromClimb = false)
    {
        zap.velocity.y = 0.0f;
        if (fromClimb)
        {
            zap.velocity.x = FromClimbJumpSpeed;
            zap.AddImpulse(new Vector2(0.0f, FromClimbJumpSideImpulse));
        }
        else
        {
            zap.velocity.x = JumpSpeed;
            zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
        }
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.JUMP_RIGHT, fromClimb ? 1 : 0);

        lastFrameHande = false;
    }

    void jumpLongLeft()
    {
        zap.velocity.x = -JumpLongSpeed;
        zap.velocity.y = 0.0f;
        zap.AddImpulse(new Vector2(0.0f, JumpLongImpulse));
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.JUMP_LEFT_LONG);

        lastFrameHande = false;
    }

    void jumpLongRight()
    {
        zap.velocity.x = JumpLongSpeed;
        zap.velocity.y = 0.0f;
        zap.AddImpulse(new Vector2(0.0f, JumpLongImpulse));
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.JUMP_RIGHT_LONG);

        lastFrameHande = false;
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
            jumpLeft();

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
            jumpRight();

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
    void setActionRopeClimbIdle()
    {
        if (zap.faceRight()) zap.AnimatorBody.Play("Zap_liana_climbup_R");
        else zap.AnimatorBody.Play("Zap_liana_climbup_L");
        zap.AnimatorBody.speed = 0f;
    }
    void setActionCrouchIdle()
    {
        zap.velocity.x = 0.0f;
        setAction(Action.CROUCH_IDLE);
    }
    void setActionMountIdle()
    {
        zap.velocity.x = 0.0f;
        zap.velocity.y = 0.0f;
        zap.setState(Zap.State.MOUNT);
        setAction(Action.MOUNT_IDLE);
        resetActionAndState();
    }
    bool setMountIdle()
    {
        if (isInState(Zap.State.MOUNT) && isNotInAction(Action.MOUNT_BIRDHIT) && isNotInAction(Action.MOUNT_ATTACK_LEFT) && isNotInAction(Action.MOUNT_ATTACK_RIGHT))
        {
            zap.velocity.x = 0.0f;
            zap.velocity.y = 0.0f;
            setAction(Action.MOUNT_IDLE);

            return true;
        }
        return false;
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
        else if (isInState(Zap.State.MOUNT))
        {

            if (Input.GetKey(zap.keyDown))
            { //&& (Input.GetKey(zap.keyLeft) || Input.GetKey(zap.keyRight)) ){
                if (keyDownDown() == 0)
                    setMountIdle();
            }
            else if (Input.GetKey(zap.keyUp))
            { //&& (Input.GetKey(zap.keyLeft) || Input.GetKey(zap.keyRight)) ){
                if (keyUpDown() == 0)
                    setMountIdle();
            }
            else if (Input.GetKey(zap.keyLeft))
            {
                if (keyLeftDown() == 0)
                    setMountIdle();
            }
            else if (Input.GetKey(zap.keyRight))
            {
                if (keyRightDown() == 0)
                    setMountIdle();
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
        if (isInAction(Action.WALK_RIGHT))
            return 1;
        if (isInAction(Action.WALK_LEFT))
            return -1;
        return 0;
    }

    int running()
    {
        if (isInAction(Action.RUN_RIGHT))
            return 1;
        if (isInAction(Action.RUN_LEFT))
            return -1;
        return 0;
    }

    bool moving(Vector2 dir)
    {
        if (dir == Vector2.right)
            return isInAction(Action.WALK_RIGHT) || isInAction(Action.RUN_RIGHT);
        else
            return isInAction(Action.WALK_LEFT) || isInAction(Action.RUN_LEFT);
    }

    bool moving(int dir)
    {
        if (dir == 1)
            return isInAction(Action.WALK_RIGHT) || isInAction(Action.RUN_RIGHT);
        else
            return isInAction(Action.WALK_LEFT) || isInAction(Action.RUN_LEFT);
    }

    bool jumping()
    {
        return isInAction(Action.JUMP) || isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG) || isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG);
    }

    bool jumping(int jumpdir)
    {
        if (jumpdir > 0)
        {
            return isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG);
        }
        else if (jumpdir < 0)
        {
            return isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG);
        }
        return false;
    }

    bool mounting()
    {
        return isInAction(Action.MOUNT_LEFT) || isInAction(Action.MOUNT_RIGHT)
            || isInAction(Action.MOUNT_UP) || isInAction(Action.MOUNT_DOWN)
            || isInAction(Action.MOUNT_ATTACK_LEFT) || isInAction(Action.MOUNT_ATTACK_RIGHT);
    }

    bool Pushing()
    {
        return isInAction(Action.PUSH_LEFT) || isInAction(Action.PUSH_RIGHT);
    }

    bool Pulling()
    {
        return isInAction(Action.PULL_LEFT) || isInAction(Action.PULL_RIGHT);
    }

    public override bool crouching()
    {
        return isInAction(Action.CROUCH_IDLE) ||
            isInAction(Action.CROUCH_LEFT) || isInAction(Action.CROUCH_LEFT_BACK) ||
                isInAction(Action.CROUCH_RIGHT) || isInAction(Action.CROUCH_RIGHT_BACK);
    }
    
    public override void zapDie(Zap.DeathType deathType)
    {
        if (zap.isInState(Zap.State.CLIMB_ROPE))
        {
            releaseRope();
            catchedRope = null;
            justJumpedRope = null;
        }
        setAction(Action.DIE, (int)deathType);
    }
    public override void beforeReborn()
    {
        if (zap.isInState(Zap.State.CLIMB_ROPE))
        {
            releaseRope();
            catchedRope = null;
            justJumpedRope = null;
        }
    }
    public override void reborn()
    {
        if (zap.LastTouchedCheckPoint)
        {
            if (zap.LastTouchedCheckPoint.startMounted)
            {
                zap.setState(Zap.State.MOUNT);
                setMountIdle();/////
            }
        }
    }
    public override bool triggerEnter(Collider2D other)
    {
        bool itsBat = (other.gameObject.tag == "Bat");
        if ( itsBat || other.gameObject.tag == "Bird")
        {
            //Debug.Log("Fuck! Its a bat!");
            if (isInState(Zap.State.CLIMB_ROPE))
            {
                zap.velocity.x = 0.0f;
                zap.velocity.y = 0.0f;
                setAction(Action.JUMP);

                releaseRope();

                //if (zap.canBeFuddleFromBird)
                zap.FuddleFromBird = true;
                return true;
            }
            else if( isInAction(Action.CLIMB_CATCH) )
            {
                zap.velocity.x = 0.0f;
                zap.velocity.y = 0.0f;
                zap.setState(Zap.State.IN_AIR);
                setAction(Action.JUMP);
                lastCatchedClimbHandle = catchedClimbHandle;
                catchedClimbHandle = null;
                zap.FuddleFromBird = true;
            }
            else if (isInState(Zap.State.MOUNT))
            {
                setAction(Action.MOUNT_BIRDHIT, itsBat ? 1 : 0);
                return true;
            }
            else if (isInState(Zap.State.IN_AIR))
            {
                zap.velocity.x = 0.0f;
                return true;
            }
            // jesli to bat to trzeba szukac dalej....
            if (itsBat)
            {

                //return true;
            }
            return false;
        }

        //if (other.gameObject.tag == "Bird")
        //{
        //    if (isInState(Zap.State.CLIMB_ROPE))
        //    {
        //        zap.velocity.x = 0.0f;
        //        zap.velocity.y = 0.0f;
        //        setAction(Action.JUMP);
                
        //        releaseRope();
                
        //        if (zap.canBeFuddleFromBird)
        //            zap.FuddleFromBird = true;
        //    }
        //    else if (isInState(Zap.State.MOUNT))
        //    {
        //        setAction(Action.MOUNT_BIRDHIT);
        //    }
        //    else if (isInState(Zap.State.IN_AIR))
        //    {
        //        zap.velocity.x = 0.0f;
        //    }
        //    return true;
        //}

        return false;
    }

    bool tryStartClimbPullDown()
    {
        GameObject potCatchedClimbHandle = canClimbPullDown();
        if (potCatchedClimbHandle)
        {
            catchedClimbHandle = potCatchedClimbHandle;

            zap.velocity.x = 0.0f;
            zap.velocity.y = 0.0f;

            Vector3 handlePos = climbAfterPos2; //potCatchedClimbHandle.transform.position;

            climbAfterPos.y = handlePos.y - 1.75f; //myHeight;
            if (zap.dir() == Vector2.right)
            {
                climbAfterPos.x = handlePos.x - zap.getMyHalfWidth();
            }
            else
            {
                climbAfterPos.x = handlePos.x + zap.getMyHalfWidth();
            }

            //canClimbPullUp2();
            climbBeforePos = transform.position;
            climbDistToClimb = climbAfterPos - climbBeforePos;

            //if (!catchedClimbHandle)
            //    return false;

            Vector2 rayOrigin = climbAfterPos; // catchedClimbHandle.transform.position;
            //rayOrigin.y += 0.14f;

            ////if (zap.dir() == Vector2.right) rayOrigin.x += 0.5f;
            ////else rayOrigin.x -= 0.5f;
            //float dtf = 0.5f;
            //if (zap.checkCeil(ref dtf)) return false;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.75f + 0.5f, zap.layerIdGroundAllMask);
            if (hit.collider) return false;

            if (zap.dir() == Vector2.right)
            {
                rayOrigin.x -= 0.25f; //zap.getMyHalfWidth();
                hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.75f + 0.5f, zap.layerIdGroundAllMask);
                if (hit.collider) return false;

            }
            else
            {
                rayOrigin.x += 0.25f; //zap.getMyHalfWidth();
                hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.75f + 0.5f, zap.layerIdGroundAllMask);
                if (hit.collider) return false;
            }

            //rayOrigin.x -= 0.5f;
            //hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.825f, zap.layerIdGroundAllMask);
            //if (hit.collider) return false;

            //rayOrigin.x += 1.0f;
            //hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.825f, zap.layerIdGroundAllMask);
            //if (hit.collider) return false;

            //return true;

            wantGetUp = false;
            setAction(Action.CLIMB_PULLDOWN);
            zap.setState(Zap.State.CLIMB);

            return true;
        }
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

    bool canRopeClimbUp()
    {
        if (ropeLinkCatchOffset == 0f)
        {
            return catchedRopeLink.transform.parent;
        }
        return true;
    }

    bool canRopeClimbDown()
    {
        if (ropeLinkCatchOffset == -0.5f)
        {

            if (catchedRopeLink.transform.childCount > 0)
            { // jak ogniwo ma dzicko to przechodze niżej 

                if (catchedRopeLink.transform.GetChild(0).transform.childCount > 0)
                { // chyba ze to jest ostatnie ogniwo
                    return true;
                }

            }

            return false;
        }
        return true;
    }

    bool tryBreakUpRope(float deltaTime)
    {

        if (catchedRopeLink)
        {
            if (catchedRopeLink.rope.breakUpStep(catchedRopeLink.idn, deltaTime))
            {
                zap.playSound(zap.ropeBreakOffSound);
                zap.RlhScene.ropeBreakOff(catchedRopeLink.rope);
                tryJumpFromRope(true);
                return false;
            }
        }
        return false;
    }

    int tryJumpFromRope(bool forceJumpOff = false)
    {
        if (Input.GetKeyDown(zap.keyJump) || forceJumpOff)
        {
            float ropeSpeed = catchedRope.firstLinkSpeed;
            float ropeSpeedRad = ropeSpeed * Mathf.Deg2Rad;
            int crl_idn = catchedRope.currentLink.GetComponent<RopeLink>().idn;
            float ps = ropeSpeedRad * (crl_idn + 1) * 0.5f;

            int zapDir = zap.dir2();

            if (Input.GetKey(zap.keyLeft) && (zap.canJumpBackFromRope || zapDir == -1) && !forceJumpOff)
            { //skacze w lewo
                zap.turnLeft();

                if (ropeSpeed > 0f)
                { // lina tez leci w lewo
                    jumpLongLeft();
                    zap.velocity.x -= ps;
                }
                else
                {
                    jumpLeft();
                }
            }
            else if (Input.GetKey(zap.keyRight) && (zap.canJumpBackFromRope || zapDir == 1) && !forceJumpOff)
            { //skacze w prawo
                zap.turnRight();

                if (ropeSpeed < 0f)
                { // lina tez leci w prawo
                    jumpLongRight();
                    zap.velocity.y += ps;
                }
                else
                {
                    jumpRight();
                }
            }
            else if (Input.GetKeyDown(zap.keyDown) || Input.GetKey(zap.keyDown) || forceJumpOff)
            {
                zap.velocity.x = 0f;
                zap.velocity.y = 0f;
                setAction(Action.JUMP);
            }
            else
            {
                return 0;
            }

            if (catchedRope.alwaysBreakOff)
            {
                catchedRope.breakUp();
            }

           

            releaseRope();
            //Quaternion quat = new Quaternion ();
            //quat.eulerAngles = new Vector3 (0f, 0f, 0f);
            //weaponText.rotation = quat;

            return 1;
        }

        return 0;
    }

    void getUp()
    {
        setAction(Action.IDLE);
        resetActionAndState();
    }

    bool tryCatchHandle(bool fromGround = false)
    {
        if (zap.dir() == Vector2.right)
        {
            float _speed = 0.2f;
            RaycastHit2D hit;
            if (fromGround)
            {
                hit = Physics2D.Raycast(zap.sensorHandleR2.position, -Vector2.up, 0.5f, zap.layerIdGroundHandlesMask);
                _speed = 0.2f;
            }
            else
            {
                if (lastFrameHande)
                    hit = Physics2D.Linecast(lastHandlePos, zap.sensorHandleR2.position, zap.layerIdGroundHandlesMask);
                else
                    hit = Physics2D.Linecast(zap.sensorHandleR2.position, zap.sensorHandleR2.position, zap.layerIdGroundHandlesMask);
            }
            if (hit.collider != null)
            {
                // tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
                bool _canCatch = true;
                if ((lastCatchedClimbHandle == hit.collider.gameObject))
                { //{ && zap.velocity.y >= 0.0f) {
                    _canCatch = false;
                }

                if (_canCatch)
                {
                    catchedClimbHandle = hit.collider.gameObject;

                    Vector3 handlePos = catchedClimbHandle.transform.position;
                    Vector3 newPos = new Vector3();
                    newPos.x = handlePos.x - zap.getMyHalfWidth();// + 0.2f;
                    newPos.y = handlePos.y - 1.75f; //myHeight;

                    //canPullUp = canClimbPullUp2();

                    //if (canPullUp)
                    //{
                    //}

                    zap.velocity.x = 0.0f;
                    zap.velocity.y = 0.0f;

                    climbBeforePos = transform.position;
                    climbAfterPos = newPos;
                    climbAfterPos2 = handlePos;
                    climbDistToClimb = climbAfterPos - climbBeforePos;
                    climbToJumpDuration = climbDistToClimb.magnitude * _speed;

                    zap.setState(Zap.State.CLIMB);
                    setAction(Action.CLIMB_JUMP_TO_CATCH, fromGround ? 1 : 0);
                    lastFrameHande = false;

                    return true;
                }
            }


            Vector2 _handle = new Vector2();
            Vector2 ro = transform.position;
            if (fromGround)
            {
                ro.y += 2f;
            }
            else
            {
                ro.y += 2.4f;
            }
            hit = Physics2D.Raycast(ro, Vector2.right, 1.0f, zap.layerIdGroundMoveableMask);
            if (hit.collider)
            {
                ro.x += 0.4f;
                if (hit.collider.GetComponent<GroundMoveable>().handleToPullDownTouched(zap.dir(), ro, ref _handle, zap.maxHandStoneTilt))
                {
                    //Debug.Log(_handle);
                    // tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
                    bool _canCatch = true;
                    if ((lastCatchedClimbHandle == hit.collider.gameObject))
                    { //{ && zap.velocity.y >= 0.0f) {
                        _canCatch = false;
                    }

                    if (_canCatch)
                    {
                        catchedClimbHandle = hit.collider.gameObject;

                        Vector3 handlePos = _handle; //catchedClimbHandle.transform.position;
                        Vector3 newPos = new Vector3();
                        newPos.x = handlePos.x - zap.getMyHalfWidth();
                        newPos.y = handlePos.y - 1.75f; //myHeight;

                        zap.velocity.x = 0.0f;
                        zap.velocity.y = 0.0f;

                        climbBeforePos = transform.position;
                        climbAfterPos = newPos;
                        climbAfterPos2 = handlePos;
                        climbDistToClimb = climbAfterPos - climbBeforePos;
                        climbToJumpDuration = climbDistToClimb.magnitude * _speed;

                        //canPullUp = canClimbPullUp2();
                        //if (canPullUp)
                        //{
                        //}

                        zap.setState(Zap.State.CLIMB);
                        setAction(Action.CLIMB_JUMP_TO_CATCH, fromGround ? 1 : 0);
                        lastFrameHande = false;
                        return true;
                    }
                }
            }

            lastHandlePos = zap.sensorHandleR2.position;
            return false;

        }
        else
        {
            float _speed = 0.2f;
            RaycastHit2D hit;
            if (fromGround)
            {
                hit = Physics2D.Raycast(zap.sensorHandleL2.position, -Vector2.up, 0.5f, zap.layerIdGroundHandlesMask);
                _speed = 0.2f;
            }
            else
            {
                if (lastFrameHande)
                    hit = Physics2D.Linecast(lastHandlePos, zap.sensorHandleL2.position, zap.layerIdGroundHandlesMask);
                else
                    hit = Physics2D.Linecast(zap.sensorHandleL2.position, zap.sensorHandleL2.position, zap.layerIdGroundHandlesMask);
            }

            if (hit.collider != null)
            {

                // tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
                bool _canCatch = true;
                if ((lastCatchedClimbHandle == hit.collider.gameObject))
                { // && zap.velocity.y >= 0.0f) {
                    _canCatch = false;
                }

                if (_canCatch)
                {
                    catchedClimbHandle = hit.collider.gameObject;

                    Vector3 handlePos = catchedClimbHandle.transform.position;
                    Vector3 newPos = new Vector3();
                    newPos.x = handlePos.x + zap.getMyHalfWidth();
                    newPos.y = handlePos.y - 1.75f; //myHeight;

                    //canPullUp = canClimbPullUp2();
                    //if (canPullUp)
                    //{
                    //}

                    zap.velocity.x = 0.0f;
                    zap.velocity.y = 0.0f;

                    climbBeforePos = transform.position;
                    climbAfterPos = newPos;
                    climbAfterPos2 = handlePos;
                    climbDistToClimb = climbAfterPos - climbBeforePos;
                    climbToJumpDuration = climbDistToClimb.magnitude * _speed;

                    zap.setState(Zap.State.CLIMB);
                    setAction(Action.CLIMB_JUMP_TO_CATCH, fromGround ? 1 : 0);
                    lastFrameHande = false;

                    //zap.velocity.x = 0.0f;
                    //zap.velocity.y = 0.0f;

                    //climbBeforePos = transform.position;
                    //climbAfterPos = newPos;
                    //climbDistToClimb = climbAfterPos - climbBeforePos;
                    //climbToJumpDuration = climbDistToClimb.magnitude * _speed;

                    //zap.setState(Zap.State.CLIMB);
                    //setAction(Action.CLIMB_JUMP_TO_CATCH);
                    //lastFrameHande = false;

                    return true;
                }
            }

            Vector2 _handle = new Vector2();
            Vector2 ro = transform.position;
            if (fromGround)
            {
                ro.y += 2f;
            }
            else
            {
                ro.y += 2.4f;
            }
            hit = Physics2D.Raycast(ro, Vector2.left, 1.0f, zap.layerIdGroundMoveableMask);
            if (hit.collider)
            {
                ro.x -= 0.4f;
                if (hit.collider.GetComponent<GroundMoveable>().handleToPullDownTouched(zap.dir(), ro, ref _handle))
                {
                    //Debug.Log(_handle);
                    // tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
                    bool _canCatch = true;
                    if ((lastCatchedClimbHandle == hit.collider.gameObject))
                    { //{ && zap.velocity.y >= 0.0f) {
                        _canCatch = false;
                    }

                    if (_canCatch)
                    {
                        catchedClimbHandle = hit.collider.gameObject;

                        Vector3 handlePos = _handle; //catchedClimbHandle.transform.position;
                        Vector3 newPos = new Vector3();
                        newPos.x = handlePos.x + zap.getMyHalfWidth();
                        newPos.y = handlePos.y - 1.75f; //myHeight;

                        zap.velocity.x = 0.0f;
                        zap.velocity.y = 0.0f;

                        climbBeforePos = transform.position;
                        climbAfterPos = newPos;
                        climbAfterPos2 = handlePos;
                        climbDistToClimb = climbAfterPos - climbBeforePos;
                        climbToJumpDuration = climbDistToClimb.magnitude * _speed;

                        //canPullUp = canClimbPullUp2();
                        //if (canPullUp)
                        //{
                        //}

                        zap.setState(Zap.State.CLIMB);
                        setAction(Action.CLIMB_JUMP_TO_CATCH, fromGround ? 1 : 0);
                        lastFrameHande = false;
                        return true;
                    }
                }
            }

            lastHandlePos = zap.sensorHandleL2.position;
            return false;
        }
    }

    bool tryCatchRope()
    {
        if (zap.dir() == Vector2.right)
        {
            RaycastHit2D hit;
            if (lastFrameHande)
                hit = Physics2D.Linecast(lastHandlePos, zap.sensorHandleR2.position, zap.layerIdRopesMask);
            else
                hit = Physics2D.Linecast(zap.sensorHandleR2.position, zap.sensorHandleR2.position, zap.layerIdRopesMask);

            if (hit.collider == null)
            {
                hit = Physics2D.Linecast(zap.sensorHandleL2.position, zap.sensorHandleR2.position, zap.layerIdRopesMask);
            }

            if (hit.collider != null)
            {
                // tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
                bool _canCatch = true;

                if (_canCatch)
                {
                    catchedRopeLink = hit.collider.transform.GetComponent<RopeLink>();

                    if (justJumpedRope == catchedRopeLink.rope)
                    {
                        catchedRopeLink = null;
                        lastHandlePos = zap.sensorHandleR2.position;
                        return false;
                    }
                    catchRope();
                    return true;
                }
            }

            lastHandlePos = zap.sensorHandleR2.position;
            return false;

        }
        else
        {

            RaycastHit2D hit;
            if (lastFrameHande)
                hit = Physics2D.Linecast(lastHandlePos, zap.sensorHandleL2.position, zap.layerIdRopesMask);
            else
                hit = Physics2D.Linecast(zap.sensorHandleL2.position, zap.sensorHandleL2.position, zap.layerIdRopesMask);

            if (hit.collider == null)
            {
                hit = Physics2D.Linecast(zap.sensorHandleL2.position, zap.sensorHandleR2.position, zap.layerIdRopesMask);
            }

            if (hit.collider != null)
            {

                // tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
                bool _canCatch = true;
                if (_canCatch)
                {
                    catchedRopeLink = hit.collider.transform.GetComponent<RopeLink>();

                    if (justJumpedRope == catchedRopeLink.rope)
                    {
                        catchedRopeLink = null;
                        lastHandlePos = zap.sensorHandleL2.position;
                        return false;
                    }
                    catchRope();
                    return true;
                }
            }

            lastHandlePos = zap.sensorHandleL2.position;
            return false;
        }
    }

    Vector2 beforeRopeCollOffset = new Vector2();
    Vector2 beforeRopeGfxCollOffset = new Vector2();

    void catchRope()
    {
        catchedRope = catchedRopeLink.rope;
        catchedRope.chooseDriver(catchedRopeLink.transform);

        float forceRatio = Mathf.Abs(zap.velocity.x) / JumpLongSpeed;
        float force = RopeSwingForce * forceRatio;

        if (zap.velocity.x < 0f)
        {
            catchedRope.setSwingMotor(-Vector2.right, force, 0.25f);
        }
        else if (zap.velocity.x > 0)
        {
            catchedRope.setSwingMotor(Vector2.right, force, 0.25f);
        }

        zap.velocity.x = 0.0f;
        zap.velocity.y = 0.0f;

        zap.setState(Zap.State.CLIMB_ROPE);
        setAction(Action.ROPECLIMB_IDLE);

        transform.position = catchedRopeLink.transform.position;
        transform.rotation = catchedRopeLink.transform.rotation;

        ropeLinkCatchOffset = 0.0f;

        beforeRopeCollOffset = zap.Coll.offset;
        beforeRopeGfxCollOffset = zap.GfxCollider.offset;

        zap.Coll.offset = new Vector2(0f,-1f);
        zap.GfxCollider.offset = new Vector2(0f, -2f);

        zap.Targeter.localPosition = new Vector3(0f, -1f, 0f);
        zap.playSound(ropeCatchSound);
    }

    void releaseRope()
    {
        Vector3 _oldPos = transform.position;
        _oldPos.y -= 1.65f;
        transform.position = _oldPos;

        
        if (catchedRope)
        {
            justJumpedRope = catchedRope;

            catchedRope.resetDiver();
            catchedRope = null;
            catchedRopeLink = null;

            zap.stopSoundLooped(ropeSwingSound);
        }

        Quaternion quat = new Quaternion();
        quat.eulerAngles = new Vector3(0f, 0f, 0f);
        transform.rotation = quat;

        zap.Coll.offset = beforeRopeCollOffset;
        zap.GfxCollider.offset = beforeRopeGfxCollOffset;

        zap.Targeter.localPosition = new Vector3(0f, 1f, 0f);

        zap.setState(Zap.State.IN_AIR);
    }

    float ropeLinkCatchOffset = 0.0f;

    //bool canClimbPullUp()
    //{
    //    if (!catchedClimbHandle)
    //        return false;

    //    Vector2 rayOrigin = catchedClimbHandle.transform.parent.transform.position;
    //    rayOrigin.x += 0.5f;
    //    rayOrigin.y += 0.14f;
    //    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.85f, zap.layerIdGroundAllMask);
    //    if (hit.collider) return false;

    //    rayOrigin.x -= 0.5f;
    //    hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.85f, zap.layerIdGroundAllMask);
    //    if (hit.collider) return false;

    //    rayOrigin.x += 1.0f;
    //    hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.85f, zap.layerIdGroundAllMask);
    //    if (hit.collider) return false;

    //    return true;
    //}
    bool canClimbPullUp2()
    {
        if (!catchedClimbHandle)
            return false;

        Vector2 rayOrigin = climbAfterPos2; // catchedClimbHandle.transform.position;
        rayOrigin.y += 0.14f;

        //if (zap.dir() == Vector2.right) rayOrigin.x += 0.5f;
        //else rayOrigin.x -= 0.5f;
        float dtf = 0.5f;
        if (zap.checkCeil(ref dtf)) return false;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.825f, zap.layerIdGroundAllMask);
        if (hit.collider) return false;

        rayOrigin.x -= 0.5f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.825f, zap.layerIdGroundAllMask);
        if (hit.collider) return false;

        rayOrigin.x += 1.0f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.825f, zap.layerIdGroundAllMask);
        if (hit.collider) return false;

        return true;
    }

    public float ClimbPullDownRange = 0.511f;

    GameObject canClimbPullDown()
    {

        if (!isInState(Zap.State.ON_GROUND) || !(isInAction(Action.IDLE) || crouching()))
            return null;

        // 1: sytuacja gdy zap jest swoim srodkiem nad tilem
        // 2: sytuacja gdy zap jest swoim srodkiem juz poza tilem

        if (zap.groundUnder)
        {
            GroundMoveable gm = zap.groundUnder.GetComponent<GroundMoveable>();
            if (gm)
            {
                Vector2 _handle = new Vector2();
                if (gm.handleToPullDownTouched(zap.dir(), transform.position, ref _handle, 5f, 0.51f))
                {
                    climbAfterPos2 = _handle;
                    return zap.groundUnder.gameObject;
                }
            }
        }

        RaycastHit2D hit;

        if (zap.dir() == Vector2.right)
        { //

            hit = Physics2D.Raycast(zap.sensorDown2.position, -Vector2.right, ClimbPullDownRange, zap.layerIdGroundHandlesMask);
            if (hit.collider)
            {
                if (Physics2D.Raycast(hit.collider.gameObject.transform.position, -Vector2.right, 0.5f, zap.layerIdGroundMask).collider == null)
                {
                    climbAfterPos2 = hit.collider.transform.position;
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
                    climbAfterPos2 = hit.collider.transform.position;
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
                else
                {
                    climbAfterPos2 = hit.collider.transform.position;
                    return hit.collider.gameObject;
                }
            }
            else
            {

                // pod prawa noga musi byc przepasc
                rayOrigin = zap.sensorDown3.position;
                if (Physics2D.Raycast(rayOrigin, -Vector2.up, 0.5f, zap.layerIdGroundMask).collider) return null;
                else
                {
                    climbAfterPos2 = hit.collider.transform.position;
                    return hit.collider.gameObject;
                }
            }

        }
        else
        {
            return null;
        }
    }

    bool jumpFromMount = false;
    bool wantGetUp = false;
    bool wantJumpAfter = false;
    bool canJumpAfter = true;
    float desiredSpeedX = 0.0f;

    Action action;
    public float CrouchInOutDuration = 0.2f;
    bool justJumpedMount = false;
    //float currentActionTime = 0f;
    NewRope justJumpedRope = null;
    //Vector3 impulse;

    Transform pushPullObstacle = null;
    public float pushForce = 5.0f;
    public float pullForce = 5.0f;
}
