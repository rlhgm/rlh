using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

[Serializable]
public class GhostControllerByKeyTriggers : GhostController
{
    //public KeyCode keyLeft = KeyCode.LeftArrow;
    //public KeyCode keyRight = KeyCode.RightArrow;
    //public KeyCode keyRun = KeyCode.LeftShift;
    //public KeyCode keyUp = KeyCode.UpArrow;
    //public KeyCode keyDown = KeyCode.DownArrow;
    //public KeyCode keyJump = KeyCode.Space;
    
    public float WalkSpeed = 2.5f;
    public float RunSpeed = 5.7f;
    public float JumpSpeed = 3.8f;
    public float JumpLongSpeed = 4.9f;
    public float CrouchSpeed = 1.8f;
    public float MountSpeed = 2.0f; // ile na sek.
    public float MountJumpDist = 10.0f; // następnie naciskasz spacje a on skacze
    public float SpeedUpParam = 10.0f; // ile jednosek predkosci hamuje na sekund
    public float SlowDownParam = 20.0f; // ile jednosek predkosci hamuje na sekunde
    public float FlyUserControlParam = 8.0f; // ile przyspiesza na sekunde lecac
    public float FlyUpUserControlParam = 9.0f; // ile przyspiesza na sekunde lecac
    public float FlySlowDownParam = 5.0f; // ile hamuje na sekunde lecac

    public float JumpImpulse = 7.0f;
    public float JumpLongImpulse = 7.15f;
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
    Vector3 climbDistToClimb;
    float climbToJumpDuration;

    Vector3 mountJumpStartPos;
    Vector3 lastHandlePos;

    float groundUnderFeet;

    public override bool GlobalUpdate(float deltaTime)
    {
        //if (!userJumpKeyPressed)
        //{
        //    if (Input.GetKeyDown(keyJump))
        //    {
        //        timeFromJumpKeyPressed = 0.0f;
        //        userJumpKeyPressed = true;
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

        //if (Input.GetKeyDown(keyUp))
        //{
        //    currentController.keyUpDown();
        //}
        //if (Input.GetKeyUp(keyUp))
        //{
        //    currentController.keyUpUp();
        //}
        //if (Input.GetKeyDown(keyDown))
        //{
        //    currentController.keyDownDown();
        //}
        //if (Input.GetKeyUp(keyDown))
        //{
        //    currentController.keyDownUp();
        //}

        //if (Input.GetKeyUp(keyJump))
        //{
        //    jumpKeyPressed = false;
        //    currentController.keyJumpUp();
        //}

        //if (Input.GetKeyDown(keyLeft))
        //{
        //    currentController.keyLeftDown();
        //}
        //if (Input.GetKeyDown(keyRight))
        //{
        //    currentController.keyRightDown();
        //}

        //if (Input.GetKeyUp(keyLeft))
        //{
        //    currentController.keyLeftUp();
        //}
        //if (Input.GetKeyUp(keyRight))
        //{
        //    currentController.keyRightUp();
        //}

        //if (Input.GetKeyDown(keyRun))
        //{
        //    currentController.keyRunDown();
        //}
        //else if (Input.GetKeyUp(keyRun))
        //{
        //    currentController.keyRunUp();
        //}

        return false;
    }

    public override void MUpdate(float deltaTime)
    {
        //return;

        justJumpedMount = false;

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
                if (owner.currentActionTime >= 0.2f)
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
                if (Input.GetKeyDown(owner.keyJump))
                {
                    wantJumpAfter = true;
                }
                if (owner.currentActionTime >= TURN_LEFTRIGHT_DURATION)
                {
                    owner.turnLeft();
                    turnLeftFinish();
                }
                break;

            case Action.TURN_STAND_RIGHT:
                if (Input.GetKeyDown(owner.keyJump))
                {
                    wantJumpAfter = true;
                }
                if (owner.currentActionTime >= TURN_LEFTRIGHT_DURATION)
                {
                    owner.turnRight();
                    turnRightFinish();
                }
                break;

            case Action.TURN_RUN_LEFT:
                if (owner.currentActionTime >= 0.85f)
                {
                    owner.turnLeft();
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
                if (owner.currentActionTime >= 0.85f)
                {
                    owner.turnRight();
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
        };

        if (wantGetUp)
        {
            if (owner.canGetUp())
            {
                setAction(Action.GET_UP);
                wantGetUp = false;
            }
        }

        switch (owner.getState())
        {

            case Ghost.State.MOUNT:
                break;

            case Ghost.State.IN_AIR:

                if (owner.jumpKeyPressed)
                { //Input.GetKeyDown(owner.keyJump) || Input.GetKey(owner.keyJump) ){
                    Vector3 fallDist = owner.startFallPos - transform.position;
                    if (!owner.FuddleFromBird && (fallDist.y < MaxFallDistToCatch))
                    {
                        if (owner.checkMount())
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
                if (jumpFromMount && Input.GetKey(owner.keyJump))
                {
                    Vector3 fallDist = owner.startFallPos - transform.position;
                    if (!owner.FuddleFromBird && (fallDist.y < MaxFallDistToCatch))
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

                if (Input.GetKey(owner.keyJump))
                {

                    if (!owner.FuddleFromBird && tryCatchRope())
                    {
                        return;
                    }
                }

                if (Input.GetKey(owner.keyJump) || owner.autoCatchEdges)
                {
                    Vector3 fallDist = owner.startFallPos - transform.position;
                    if (!owner.FuddleFromBird && fallDist.y < MaxFallDistToCatch)
                    {
                        if (tryCatchHandle())
                        {
                            owner.lastVelocity = owner.velocity;
                            return;
                        }
                    }
                }

                if (Input.GetKeyDown(owner.keyJump))
                {
                    lastFrameHande = true;
                    if (owner.dir() == Vector2.right)
                        lastHandlePos = owner.sensorHandleR2.position;
                    else
                        lastHandlePos = owner.sensorHandleL2.position;
                }

                if (Input.GetKeyUp(owner.keyJump))
                {
                    lastFrameHande = false;
                }

                owner.AddImpulse(new Vector2(0.0f, GravityForce * deltaTime));

                if (isInAction(Action.JUMP_LEFT) || isInAction(Action.JUMP_LEFT_LONG))
                {

                    if (Input.GetKey(owner.keyLeft))
                    {
                        owner.velocity.x -= (FlyUserControlParam * deltaTime);

                        if (isInAction(Action.JUMP_LEFT))
                        {
                            if (Mathf.Abs(owner.velocity.x) > JumpSpeed)
                                owner.velocity.x = -JumpSpeed;
                        }
                        else
                        {
                            if (Mathf.Abs(owner.velocity.x) > JumpLongSpeed)
                                owner.velocity.x = -JumpLongSpeed;
                        }

                    }
                    else if (Input.GetKey(owner.keyRight))
                    {
                        owner.velocity.x += (FlyUserControlParam * deltaTime);
                        if (owner.velocity.x > 0.0f) owner.velocity.x = 0.0f;
                    }
                }
                else if (isInAction(Action.JUMP_RIGHT) || isInAction(Action.JUMP_RIGHT_LONG))
                {
                    if (Input.GetKey(owner.keyRight))
                    {
                        owner.velocity.x += (FlyUserControlParam * deltaTime);

                        if (isInAction(Action.JUMP_RIGHT))
                        {
                            if (Mathf.Abs(owner.velocity.x) > JumpSpeed)
                                owner.velocity.x = JumpSpeed;
                        }
                        else
                        {
                            if (Mathf.Abs(owner.velocity.x) > JumpLongSpeed)
                                owner.velocity.x = JumpLongSpeed;
                        }

                    }
                    else if (Input.GetKey(owner.keyLeft))
                    {
                        owner.velocity.x -= (FlyUserControlParam * deltaTime);
                        if (owner.velocity.x < 0.0f) owner.velocity.x = 0.0f;
                    }
                }
                else if (isInAction(Action.JUMP))
                {
                    if (Input.GetKey(owner.keyLeft))
                    {
                        owner.velocity.x -= (FlyUpUserControlParam * deltaTime);
                        if (Mathf.Abs(owner.velocity.x) > JumpSpeed)
                            owner.velocity.x = -JumpSpeed;
                    }
                    if (Input.GetKey(owner.keyRight))
                    {
                        owner.velocity.x += (FlyUpUserControlParam * deltaTime);
                        if (Mathf.Abs(owner.velocity.x) > JumpSpeed)
                            owner.velocity.x = JumpSpeed;
                    }

                    if (owner.velocity.x > 0.0f)
                    {
                        owner.turnRight();
                    }
                    else if (owner.velocity.x < 0.0f)
                    {
                        owner.turnLeft();
                    }
                }

                Vector3 distToFall = new Vector3();
                distToFall.x = owner.velocity.x * deltaTime;

                if (distToFall.x > 0.0f)
                {
                    float obstacleOnRoad = owner.checkRight(distToFall.x + 0.01f, !owner.stateJustChanged);
                    if (obstacleOnRoad >= 0.0f)
                    {
                        if (obstacleOnRoad < Mathf.Abs(distToFall.x))
                        {
                            distToFall.x = obstacleOnRoad;
                            owner.velocity.x = 0.0f;
                        }
                    }
                }
                else if (distToFall.x < 0.0f)
                {
                    float obstacleOnRoad = owner.checkLeft(Mathf.Abs(distToFall.x) + 0.01f, !owner.stateJustChanged);
                    if (obstacleOnRoad >= 0.0f)
                    {
                        if (obstacleOnRoad < Mathf.Abs(distToFall.x))
                        {
                            distToFall.x = -obstacleOnRoad;
                            owner.velocity.x = 0.0f;
                        }
                    }
                }

                transform.position = transform.position + distToFall;
                distToFall.x = 0f;

                owner.velocity.y += owner.GetImpulse().y;
                if (owner.velocity.y > MaxSpeedY)
                    owner.velocity.y = MaxSpeedY;
                if (owner.velocity.y < -MaxSpeedY)
                    owner.velocity.y = -MaxSpeedY;

                distToFall.y = owner.velocity.y * deltaTime;

                bool justLanding = false;

                if (distToFall.y > 0.0f)
                { // leci w gore
                  //transform.position = transform.position + distToFall;
                }
                else if (distToFall.y < 0.0f)
                { // spada
                    if (owner.lastVelocity.y >= 0.0f)
                    { // zaczyna spadac
                      // badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
                        owner.startFallPos = transform.position;
                        //print ( "owner.startFallPos : " + owner.startFallPos );
                        if (owner.lastVelocity.y > 0.0f)
                        {
                            lastCatchedClimbHandle = null;
                        }
                    }
                    groundUnderFeet = owner.checkDown(Mathf.Abs(distToFall.y) + 0.01f);
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
                    owner.FuddleFromBird = false;

                    owner.setState(Ghost.State.ON_GROUND);
                    owner.velocity.y = 0.0f;

                    Vector3 fallDist = owner.startFallPos - transform.position;

                    if (fallDist.y >= VeryHardLandingHeight)
                    {
                        //owner.beforeFallController = null;
                        owner.die(Ghost.DeathType.VERY_HARD_LANDING);
                    }
                    else if (fallDist.y >= HardLandingHeight)
                    {

                        //if( owner.beforeFallController == null ){
                        owner.velocity.x = 0.0f;
                        setAction(Action.LANDING_HARD);
                        //}else{
                        //}

                    }
                    else
                    {
                        //if (owner.beforeFallController == null)
                        {
                            resetActionAndState();
                        }
                        //else
                        //{
                        //    owner.restoreBeforeFallController();
                        //    //owner.beforeFallController = null;
                        //}
                    }
                }

                break;

            case Ghost.State.DEAD:

                //			owner.AddImpulse(new Vector2(0.0f, GravityForce * deltaTime));
                //			
                //
                //			Vector3 distToFall = new Vector3();
                //			distToFall.x = owner.velocity.x * deltaTime;
                //			
                //			if( distToFall.x > 0.0f ){
                //				float obstacleOnRoad = owner.checkRight(distToFall.x + 0.01f,!owner.stateJustChanged);
                //				if( obstacleOnRoad >= 0.0f ){
                //					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
                //						distToFall.x = obstacleOnRoad;
                //						owner.velocity.x = 0.0f;
                //					}
                //				}
                //			}else if( distToFall.x < 0.0f ){
                //				float obstacleOnRoad = owner.checkLeft( Mathf.Abs(distToFall.x) + 0.01f,!owner.stateJustChanged);
                //				if( obstacleOnRoad >= 0.0f ){
                //					if( obstacleOnRoad < Mathf.Abs(distToFall.x) ){
                //						distToFall.x = -obstacleOnRoad;
                //						owner.velocity.x = 0.0f;
                //					}
                //				}
                //			}
                //			
                //			transform.position = transform.position + distToFall;
                //			distToFall.x = 0f;
                //			
                //			owner.velocity.y += owner.GetImpulse().y;
                //			if(owner.velocity.y > MaxSpeedY)
                //				owner.velocity.y = MaxSpeedY;
                //			if(owner.velocity.y < -MaxSpeedY)
                //				owner.velocity.y = -MaxSpeedY;
                //			
                //			distToFall.y = owner.velocity.y * deltaTime;
                //			
                //			bool justLanding = false;
                //			
                //			if( distToFall.y > 0.0f ) { // leci w gore
                //				//transform.position = transform.position + distToFall;
                //			} else if( distToFall.y < 0.0f ) { // spada
                //				if( owner.lastVelocity.y >= 0.0f ) { // zaczyna spadac
                //					// badam czy bohater nie "stoi" wewnatrz wskakiwalnej platformy
                //					owner.startFallPos = transform.position;
                //					//print ( "owner.startFallPos : " + owner.startFallPos );
                //					if( owner.lastVelocity.y > 0.0f ){
                //						lastCatchedClimbHandle = null;
                //					}
                //				}
                //				groundUnderFeet = owner.checkDown( Mathf.Abs(distToFall.y) + 0.01f);
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
                //				if( owner.landingSound )
                //					owner.getAudioSource().PlayOneShot( owner.landingSound );
                //				
                //				owner.setFuddledFromBrid( false );
                //				
                //				owner.setState(Ghost.State.ON_GROUND);
                //				owner.velocity.y = 0.0f;
                //				
                //				Vector3 fallDist = owner.startFallPos - transform.position;
                //				
                //				if( fallDist.y >= VeryHardLandingHeight ){
                //					owner.beforeFallController = null;
                //					owner.die(Ghost.DeathType.VERY_HARD_LANDING);
                //				} else if( fallDist.y >= HardLandingHeight ){
                //					
                //					//if( owner.beforeFallController == null ){
                //					owner.velocity.x = 0.0f;
                //					setAction (Action.LANDING_HARD);
                //					//}else{
                //					//}
                //					
                //				}else{
                //					if( owner.beforeFallController == null ){
                //						resetActionAndState();
                //					}else{
                //						owner.restoreBeforeFallController();
                //						//owner.beforeFallController = null;
                //					}
                //				}
                //			}
                break;

            case Ghost.State.ON_GROUND:
                float distToGround = 0.0f;
                bool groundUnderFeet2 = owner.checkGround(true, owner.layerIdGroundAllMask, ref distToGround);
                if (groundUnderFeet2)
                {

                }
                else
                {
                    owner.setState(Ghost.State.IN_AIR);
                    setAction(Action.JUMP);
                    wantGetUp = false;
                }

                break;

            case Ghost.State.CLIMB_ROPE:
                Vector3 linkPos = catchedRopeLink.transform.TransformPoint(new Vector3(0.0f, ropeLinkCatchOffset, 0.0f));
                transform.position = linkPos;
                transform.rotation = catchedRopeLink.transform.rotation;

                Quaternion quat = new Quaternion();
                quat.eulerAngles = new Vector3(0f, 0f, 0f);
                //weaponText.rotation = quat;

                break;
        };

        owner.sprRend.material.SetFloat("_ClipDist",  /*1.0f -*/ 0.5f * (owner.velocity.magnitude / RunSpeed));

        owner.lastVelocity = owner.velocity;

    }

    public override void FUpdate(float fDeltaTime)
    {
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
        ROPECLIMB_IDLE,
        ROPECLIMB_UP,
        ROPECLIMB_DOWN,
        DIE
    };

    Action getAction()
    {
        return action;
    }
    bool setAction(Action newAction, int param = 0)
    {

        if (action == newAction)
            return false;

        action = newAction;
        owner.resetCurrentActionTime();
        owner.AnimatorBody.speed = 1f;

        switch (newAction)
        {

            case Action.IDLE:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_idle_R");
                else owner.AnimatorBody.Play("Zap_idle_L");
                break;

            case Action.DIE:
                Ghost.DeathType dt = (Ghost.DeathType)param;
                switch (dt)
                {
                    case Ghost.DeathType.STONE_HIT:
                        if (owner.faceRight()) owner.AnimatorBody.Play("Zap_death_stonehit_R");
                        else owner.AnimatorBody.Play("Zap_death_stonehit_L");
                        break;

                    case Ghost.DeathType.VERY_HARD_LANDING:
                        if (owner.faceRight()) owner.AnimatorBody.Play("Zap_death_hitground_R");
                        else owner.AnimatorBody.Play("Zap_death_hitground_L");
                        break;

                    case Ghost.DeathType.SNAKE:
                        if (owner.faceRight()) owner.AnimatorBody.Play("Zap_death_poison_R");
                        else owner.AnimatorBody.Play("Zap_death_poison_L");
                        break;

                    case Ghost.DeathType.POISON:
                        if (owner.faceRight()) owner.AnimatorBody.Play("Zap_death_poison_R");
                        else owner.AnimatorBody.Play("Zap_death_poison_L");
                        break;

                    case Ghost.DeathType.PANTHER:
                        if (owner.faceRight()) owner.AnimatorBody.Play("Zap_death_panther");
                        else owner.AnimatorBody.Play("Zap_death_panther");
                        break;

                    case Ghost.DeathType.CROCODILE:
                        break;

                    default:
                        if (owner.faceRight()) owner.AnimatorBody.Play("Zap_death_hitground_R");
                        else owner.AnimatorBody.Play("Zap_death_hitground_L");
                        break;

                };
                break;

            case Action.WALK_LEFT:
                owner.AnimatorBody.Play("Zap_walk_L");
                break;

            case Action.WALK_RIGHT:
                owner.AnimatorBody.Play("Zap_walk_R");
                break;

            case Action.RUN_LEFT:
                owner.AnimatorBody.Play("Zap_run_L");
                break;
            case Action.RUN_RIGHT:
                owner.AnimatorBody.Play("Zap_run_L");
                break;

            case Action.TURN_STAND_LEFT:
                owner.AnimatorBody.Play("Zap_walk_back_left");
                wantJumpAfter = false;
                break;

            case Action.TURN_STAND_RIGHT:
                owner.AnimatorBody.Play("Zap_walk_back_right");
                wantJumpAfter = false;
                break;

            case Action.TURN_RUN_LEFT:
                owner.AnimatorBody.Play("Zap_runback_L");
                wantJumpAfter = false;
                break;

            case Action.TURN_RUN_RIGHT:
                owner.AnimatorBody.Play("Zap_runback_R");
                wantJumpAfter = false;
                break;

            case Action.PREPARE_TO_JUMP:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_jump_in_R");
                else owner.AnimatorBody.Play("Zap_jump_in_L");
                break;

            case Action.JUMP:
                if (param == 0)
                {

                    if (owner.faceRight()) owner.AnimatorBody.Play("Zap_jump_fly_R");
                    else owner.AnimatorBody.Play("Zap_jump_fly_L");

                }
                else if (param == 1)
                {
                    if (owner.faceRight()) owner.AnimatorBody.Play("zap_rocks_climb_R");
                    else owner.AnimatorBody.Play("zap_rocks_climb_L");
                }
                break;

            case Action.JUMP_LEFT:
            case Action.JUMP_LEFT_LONG:
            case Action.JUMP_RIGHT:
            case Action.JUMP_RIGHT_LONG:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_run_jump_fly_R");
                else owner.AnimatorBody.Play("Zap_run_jump_fly_L");
                break;

            case Action.LANDING_HARD:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_landing_hard_R");
                else owner.AnimatorBody.Play("Zap_landing_hard_L");
                break;

            case Action.CLIMB_PREPARE_TO_JUMP:
                break;

            case Action.CLIMB_JUMP_TO_CATCH:
                break;

            case Action.CLIMB_CATCH:
                if (param == 0)
                {
                    if (owner.faceRight()) owner.AnimatorBody.Play("zap_rocks_catch_position_R");
                    else owner.AnimatorBody.Play("zap_rocks_catch_position_L");
                }
                else if (param == 1)
                {
                    // tu juz jest we wlasciwej klatce
                    if (owner.faceRight()) owner.AnimatorBody.Play("zap_rocks_catch_position_rev_R");
                    else owner.AnimatorBody.Play("zap_rocks_catch_position_rev_L");
                    owner.AnimatorBody.speed = 0.0f;
                }
                break;

            case Action.CLIMB_CLIMB:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_jump_climb_R");
                else owner.AnimatorBody.Play("Zap_jump_climb_L");
                break;

            case Action.CLIMB_PULLDOWN:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_drop_R");
                else owner.AnimatorBody.Play("Zap_drop_L");
                break;

            case Action.MOUNT_IDLE:
                owner.AnimatorBody.Play("Zap_climbmove_up");
                owner.AnimatorBody.speed = 0.0f;
                break;

            case Action.MOUNT_LEFT:
                owner.AnimatorBody.Play("Zap_climbmove_left");
                break;
            case Action.MOUNT_RIGHT:
                owner.AnimatorBody.Play("Zap_climbmove_right");
                break;
            case Action.MOUNT_UP:
                owner.AnimatorBody.Play("Zap_climbmove_up");
                break;
            case Action.MOUNT_DOWN:
                owner.AnimatorBody.Play("Zap_climbmove_down");
                break;

            case Action.MOUNT_ATTACK_LEFT:
                break;

            case Action.MOUNT_ATTACK_RIGHT:
                break;

            case Action.CROUCH_IN:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_crouch_in_R");
                else owner.AnimatorBody.Play("Zap_crouch_in_L");
                break;

            case Action.GET_UP:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_getup_R");
                else owner.AnimatorBody.Play("Zap_getup_L");
                break;

            case Action.CROUCH_IDLE:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_crouch_move_R");
                else owner.AnimatorBody.Play("Zap_crouch_move_L");
                owner.AnimatorBody.speed = 0f;
                break;

            case Action.CROUCH_LEFT:
                owner.AnimatorBody.Play("Zap_crouch_move_L");
                break;
            case Action.CROUCH_RIGHT:
                owner.AnimatorBody.Play("Zap_crouch_move_R");
                break;

            case Action.CROUCH_LEFT_BACK:
                owner.AnimatorBody.Play("Zap_crouch_move_back_R");
                break;

            case Action.CROUCH_RIGHT_BACK:
                owner.AnimatorBody.Play("Zap_crouch_move_back_L");
                break;

            case Action.ROPECLIMB_IDLE:
                setActionRopeClimbIdle();
                break;

            case Action.ROPECLIMB_UP:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_liana_climbup_R");
                else owner.AnimatorBody.Play("Zap_liana_climbup_L");
                break;

            case Action.ROPECLIMB_DOWN:
                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_liana_slide_R");
                else owner.AnimatorBody.Play("Zap_liana_slide_L");
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
        if (isInState(Ghost.State.MOUNT))
        {
            if (!mounting())
            {
                Vector3 playerPos = transform.position;
                playerPos.y += 0.1f;
                if (owner.checkMount(playerPos))
                {
                    owner.velocity.x = 0.0f;
                    owner.velocity.y = MountSpeed;
                    setAction(Action.MOUNT_UP);
                    return 1;
                }
            }
        }
        else if (isInState(Ghost.State.ON_GROUND))
        {
            if (owner.checkMount())
            {
                owner.velocity.x = 0.0f;
                owner.velocity.y = MountSpeed;
                setAction(Action.MOUNT_UP);
                owner.setState(Ghost.State.MOUNT);
                return 1;
            }
        }
        return 0;
    }

    public override int keyUpUp()
    {
        if (setMountIdle())
        {
            if (isInState(Ghost.State.MOUNT))
            {
                if (Input.GetKey(owner.keyLeft))
                    keyLeftDown();
                else if (Input.GetKey(owner.keyRight))
                    keyRightDown();
                else if (Input.GetKey(owner.keyDown))
                    keyDownDown();
            }
        }
        return 0;
    }

    public override int keyDownDown()
    {
        if (isInState(Ghost.State.MOUNT))
        {
            if (!mounting())
            {
                Vector3 playerPos = transform.position;
                playerPos.y -= 0.1f;
                if (owner.checkMount(playerPos))
                {
                    owner.velocity.x = 0.0f;
                    owner.velocity.y = -MountSpeed;
                    setAction(Action.MOUNT_DOWN);
                    return 1;
                }
            }
        }
        else if (isInState(Ghost.State.ON_GROUND))
        {

            if (tryStartClimbPullDown())
            {

                return 1;

            }
            else
            {
                setAction(Action.CROUCH_IN);
                return 1;
            }
        }

        return 0;
    }

    public override int keyDownUp()
    {
        if (setMountIdle())
        {
            if (isInState(Ghost.State.MOUNT))
            {
                if (Input.GetKey(owner.keyLeft))
                    keyLeftDown();
                else if (Input.GetKey(owner.keyRight))
                    keyRightDown();
                else if (Input.GetKey(owner.keyUp))
                    keyUpDown();
            }
        }
        else if (isInState(Ghost.State.ON_GROUND))
        {
            if (crouching() || isInAction(Action.CROUCH_IN))
            {
                if (owner.canGetUp())
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
                if (Input.GetKey(owner.keyLeft))
                {
                    desiredSpeedX = RunSpeed;
                    setAction(Action.RUN_LEFT);
                }
                break;

            case Action.WALK_RIGHT:
                if (Input.GetKey(owner.keyRight))
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
                if (Input.GetKey(owner.keyLeft))
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
                if (Input.GetKey(owner.keyRight))
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
        if ((isInAction(Action.IDLE) || moving(-1) || jumping()) && isInState(Ghost.State.ON_GROUND))
        {
            if (owner.checkLeft(0.1f) >= 0.0f)
            {
                if (owner.dir() == Vector2.right)
                    turnLeftStart();
                return 0;
            }

            if (owner.dir() == -Vector2.right)
            {
                if (Input.GetKey(owner.keyRun))
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
        else if (isInState(Ghost.State.MOUNT))
        {
            if (!mounting())
            {
                Vector3 playerPos = transform.position;
                playerPos.x -= 0.1f;
                owner.turnLeft();
                if (owner.checkMount(playerPos))
                {
                    owner.velocity.x = -MountSpeed;
                    owner.velocity.y = 0.0f;
                    setAction(Action.MOUNT_LEFT);
                    return 1;
                }
            }
        }
        else if (isInAction(Action.CROUCH_IDLE) && isInState(Ghost.State.ON_GROUND))
        {
            if (owner.checkLeft(0.1f) >= 0.0f)
            {
                return 0;
            }
            desiredSpeedX = CrouchSpeed;
            if (owner.dir() == -Vector2.right)
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
        if ((isInAction(Action.IDLE) || moving(1) || jumping()) && isInState(Ghost.State.ON_GROUND))
        {
            if (owner.checkRight(0.1f) >= 0.0f)
            {
                if (owner.dir() == -Vector2.right)
                    turnRightStart();
                return 0;
            }
            if (owner.dir() == Vector2.right)
            {
                if (Input.GetKey(owner.keyRun))
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
        else if (isInState(Ghost.State.MOUNT))
        {
            if (!mounting())
            {
                Vector3 playerPos = transform.position;
                playerPos.x += 0.1f;
                owner.turnRight();
                if (owner.checkMount(playerPos))
                {
                    owner.velocity.x = MountSpeed;
                    owner.velocity.y = 0.0f;
                    setAction(Action.MOUNT_RIGHT);
                    return 1;
                }
            }
        }
        else if (isInAction(Action.CROUCH_IDLE) && isInState(Ghost.State.ON_GROUND))
        {
            if (owner.checkRight(0.1f) >= 0.0f)
            {
                return 0;
            }
            desiredSpeedX = CrouchSpeed;
            if (owner.dir() == Vector2.right)
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
            if (isInState(Ghost.State.ON_GROUND))
            {
                desiredSpeedX = 0.0f;
            }
        }
        else
        {
            if (isInState(Ghost.State.MOUNT))
            {
                if (Input.GetKey(owner.keyRight))
                    keyRightDown();
                else if (Input.GetKey(owner.keyUp))
                    keyUpDown();
                else if (Input.GetKey(owner.keyDown))
                    keyDownDown();
            }
        }

        return 0;
    }

    public override int keyRightUp()
    {
        if (!setMountIdle())
        {
            if (isInState(Ghost.State.ON_GROUND))
            {
                desiredSpeedX = 0.0f;
            }
        }
        else
        {
            if (isInState(Ghost.State.MOUNT))
            {
                if (Input.GetKey(owner.keyLeft))
                    keyLeftDown();
                else if (Input.GetKey(owner.keyUp))
                    keyUpDown();
                else if (Input.GetKey(owner.keyDown))
                    keyDownDown();
            }
        }

        return 0;
    }

    //bool jumpKeyPressed = false;

    public override int keyJumpDown()
    {

        //Debug.Log ("ZapControllerNormal::keyJumpDown()");
        //jumpKeyPressed = true;

        switch (action)
        {
            case Action.IDLE:
                if (isInState(Ghost.State.ON_GROUND))
                {
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

                if (Input.GetKey(owner.keyLeft))
                {
                    jumpLeft();
                    return 0;
                }

                if (Input.GetKey(owner.keyRight))
                {
                    jumpRight();
                    return 0;
                }

                owner.velocity.x = 0.0f;
                owner.velocity.y = 0.0f;
                setAction(Action.JUMP);
                owner.setState(Ghost.State.IN_AIR);

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
        }
        return 0;
    }

    int Action_WALK(int dir)
    {

        if (Input.GetButtonDown("Fire1"))
        {
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
            setAction(Action.IDLE);
            resetActionAndState();
            return 0;
        }

        distToMove = owner.velocity.x * owner.getCurrentDeltaTime();

        owner.AnimatorBody.speed = 0.5f + (Mathf.Abs(owner.velocity.x) / WalkSpeed) * 0.5f;

        float distToObstacle = 0.0f;
        if (owner.checkObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            setActionIdle();
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        float distToGround = 0.0f;
        bool groundUnderFeet = owner.checkGround(false, owner.layerIdLastGroundTypeTouchedMask, ref distToGround);
        if (groundUnderFeet)
        {
            transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        }
        return 0;
    }

    int Action_RUN(int dir)
    {

        if (Input.GetMouseButtonDown(0))
        {
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
            setAction(Action.IDLE);
            resetActionAndState();
            return 0;
        }

        float speedRatio = (Mathf.Abs(owner.velocity.x) / RunSpeed);
        bool turnBackHard = speedRatio > 0.5f;

        if (turnBackHard)
        {

            if (dir == 1)
            {

                if ((Input.GetKeyDown(owner.keyLeft) || Input.GetKey(owner.keyLeft)) &&
                   (Input.GetKeyUp(owner.keyRight) || !Input.GetKey(owner.keyRight))
                   )
                {
                    setAction(Action.TURN_RUN_LEFT);
                }

            }
            else if (dir == -1)
            {

                if ((Input.GetKeyDown(owner.keyRight) || Input.GetKey(owner.keyRight)) &&
                   (Input.GetKeyUp(owner.keyLeft) || !Input.GetKey(owner.keyLeft))
                   )
                {
                    setAction(Action.TURN_RUN_RIGHT);
                }

            }

        }

        distToMove = owner.velocity.x * owner.getCurrentDeltaTime();

        owner.AnimatorBody.speed = 0.5f + (Mathf.Abs(owner.velocity.x) / RunSpeed) * 0.5f;

        float distToObstacle = 0.0f;
        if (owner.checkObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            setActionIdle();
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        float distToGround = 0.0f;
        bool groundUnderFeet = owner.checkGround(false, owner.layerIdLastGroundTypeTouchedMask, ref distToGround);
        if (groundUnderFeet)
        {
            transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        }

        return 0;
    }

    int Action_TURN_RUN(int dir)
    {

        int retVal = 0;

        if (Input.GetKeyDown(owner.keyJump))
        {
            wantJumpAfter = true;
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
        }

        distToMove = owner.velocity.x * owner.getCurrentDeltaTime();

        float distToObstacle = 0.0f;
        if (owner.checkObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            retVal = 1;
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        float distToGround = 0.0f;
        bool groundUnderFeet = owner.checkGround(false, owner.layerIdLastGroundTypeTouchedMask, ref distToGround);
        if (groundUnderFeet)
        {
            transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        }

        return retVal;
    }

    int Action_CROUCH_IN()
    {

        if (owner.currentActionTime >= CrouchInOutDuration)
        {
            crouch();
        }
        return 0;
    }

    int Action_GET_UP()
    {

        if (owner.currentActionTime >= CrouchInOutDuration)
        {
            getUp();
        }

        return 0;
    }

    int Action_CROUCH_IDLE()
    {
        if (Input.GetMouseButtonDown(0))
        {
        }

        if (Input.GetKey(owner.keyDown))
        {
            tryStartClimbPullDown();
        }
        return 0;
    }

    int Action_CROUCH_LEFTRIGHT(int dir)
    {
        if (Input.GetMouseButtonDown(0))
        {
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
            setActionCrouchIdle();
            if (crouching())
            {
                if (Input.GetKey(owner.keyLeft))
                {
                    keyLeftDown();
                }
                else if (Input.GetKey(owner.keyRight))
                {
                    keyRightDown();
                }
            }
        }

        distToMove = owner.velocity.x * owner.getCurrentDeltaTime();

        float distToObstacle = 0.0f;
        if (owner.checkObstacle(dir, distToMove, ref distToObstacle))
        {
            distToMove = distToObstacle;
            setActionCrouchIdle();
        }

        newPosX += distToMove;
        transform.position = new Vector3(newPosX, oldPos.y, 0.0f);

        float distToGround = 0.0f;
        bool groundUnderFeet = owner.checkGround(false, owner.layerIdLastGroundTypeTouchedMask, ref distToGround);
        if (groundUnderFeet)
        {
            transform.position = new Vector3(newPosX, oldPos.y + distToGround, 0.0f);
        }

        return 0;
    }

    bool tryMountAttackStart()
    {
        //if (owner.HaveKnife && Input.GetMouseButtonDown(0))
        //{
        //    Vector2 mouseInScene = touchCamera.ScreenToWorldPoint(Input.mousePosition);
        //    if (mouseInScene.x < transform.position.x)
        //    {
        //        if (owner.faceRight())
        //        {
        //            owner.turnLeft();
        //        }
        //        setAction(Action.MOUNT_ATTACK_LEFT);
        //    }
        //    else
        //    {
        //        if (!owner.faceRight())
        //        {
        //            owner.turnRight();
        //        }
        //        setAction(Action.MOUNT_ATTACK_RIGHT);
        //    }
        //    return true;
        //}
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

    int Action_MOUNTING()
    {
        if (tryMountAttackStart())
        {
            return 0;
        }

        Vector3 newPos3 = transform.position;
        Vector3 distToMount = owner.velocity * owner.getCurrentDeltaTime();
        newPos3 += distToMount;
        if (owner.checkMount(newPos3))
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
        Vector3 distToMount = owner.velocity * owner.getCurrentDeltaTime();
        newPos3 += distToMount;

        if (distToMount.y < 0.0f)
        { // schodzi
            groundUnderFeet = owner.checkDown(Mathf.Abs(distToMount.y) + 0.01f);
            if (groundUnderFeet >= 0.0f)
            {
                if (groundUnderFeet < Mathf.Abs(distToMount.y))
                {
                    distToMount.y = -groundUnderFeet;
                    owner.velocity.x = 0.0f;
                    owner.velocity.y = 0.0f;
                    owner.setState(Ghost.State.ON_GROUND);
                    setAction(Action.IDLE);
                    transform.position = transform.position + distToMount;
                }
            }
            else
            {
                if (owner.checkMount(newPos3))
                    transform.position = newPos3;
                else
                    setMountIdle();
            }
        }
        return 0;
    }

    int Action_MOUNT_ATTACK()
    {
        if (owner.currentActionTime >= MOUNT_ATTACK_DURATION)
        {
            if (isInState(Ghost.State.MOUNT))
            {
                setAction(Action.MOUNT_IDLE);
                if (Input.GetKey(owner.keyLeft))
                    keyLeftDown();
                else if (Input.GetKey(owner.keyRight))
                    keyRightDown();
                else if (Input.GetKey(owner.keyUp))
                    keyUpDown();
                else if (Input.GetKey(owner.keyDown))
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

        if (Input.GetKey(owner.keyLeft))
        {

            float fla = catchedRope.firstLinkAngle;

            if (fla > -20f && fla < 0f)
            {
                //print ( "Rope swing : " + fla );
                catchedRope.swing(-Vector2.right, RopeSwingForce * owner.getCurrentDeltaTime());
                _swing = true;
            }

            //if( _swing ){
            if (owner.dir() == Vector2.right)
            {

                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_liana_swingback_R");
                else owner.AnimatorBody.Play("Zap_liana_swingback_L");
                owner.AnimatorBody.speed = 1f;

            }
            else
            {

                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_liana_swingfront_R");
                else owner.AnimatorBody.Play("Zap_liana_swingfront_L");
                owner.AnimatorBody.speed = 1f;

            }
            //}
        }
        else if (Input.GetKey(owner.keyRight))
        {

            float fla = catchedRope.firstLinkAngle;

            if (fla < 20f && fla >= 0f)
            {
                //print ( "Rope swing : " + fla );
                catchedRope.swing(Vector2.right, RopeSwingForce * owner.getCurrentDeltaTime());
                _swing = true;
            }

            //if( _swing ){
            if (owner.dir() == Vector2.right)
            {

                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_liana_swingfront_R");
                else owner.AnimatorBody.Play("Zap_liana_swingfront_L");
                owner.AnimatorBody.speed = 1f;

            }
            else
            {

                if (owner.faceRight()) owner.AnimatorBody.Play("Zap_liana_swingback_R");
                else owner.AnimatorBody.Play("Zap_liana_swingback_L");
                owner.AnimatorBody.speed = 1f;

            }
            //}
        }

        if (Input.GetKeyUp(owner.keyLeft) || Input.GetKeyUp(owner.keyRight))
        { //|| !_swing) {
            setActionRopeClimbIdle();
        }

        if (tryJumpFromRope() != 0)
        {
            return 0;
        }

        if (Input.GetKey(owner.keyUp))
        {

            if (canRopeClimbUp())
            {
                setAction(Action.ROPECLIMB_UP);
            }

        }
        else if (Input.GetKey(owner.keyDown))
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
        if (owner.currentActionTime >= LANDING_HARD_DURATION)
        {
            //if (owner.beforeFallController == null)
            {
                setAction(Action.IDLE);
                resetActionAndState();
            }
            //else
            //{
            //    owner.restoreBeforeFallController();
            //}
        }

        return 0;
    }

    int Action_CLIMB_PULLDOWN()
    {
        if (owner.currentActionTime >= CLIMBDUR_CLIMB)
        {
            setAction(Action.CLIMB_CATCH, 1);
            owner.setState(Ghost.State.CLIMB);
            canPullUp = true;
            transform.position = climbAfterPos;
        }
        else
        {
        }

        return 0;
    }

    int Action_CLIMB_JUMP_TO_CATCH()
    {
        // dociaganie do punktu:
        if (owner.currentActionTime >= climbToJumpDuration)
        {
            setAction(Action.CLIMB_CATCH);
            transform.position = climbAfterPos;
        }
        else
        {
            float ratio = owner.currentActionTime / climbToJumpDuration;
            transform.position = climbBeforePos + climbDistToClimb * ratio;
        }

        return 0;
    }

    int Action_CLIMB_CATCH()
    {
        if ((Input.GetKeyDown(owner.keyUp) || Input.GetKey(owner.keyUp)) && canPullUp)
        {

            climbAfterPos.x = catchedClimbHandle.transform.position.x;
            climbAfterPos.y = catchedClimbHandle.transform.position.y;

            climbBeforePos = transform.position;
            climbDistToClimb = climbAfterPos - climbBeforePos;

            setAction(Action.CLIMB_CLIMB);
            catchedClimbHandle = null;
            lastCatchedClimbHandle = null;
        }
        else if (Input.GetKeyDown(owner.keyJump))
        {
            if (owner.dir() == Vector2.right && Input.GetKey(owner.keyLeft))
            {
                owner.turnLeft();
                jumpLeft();
                catchedClimbHandle = null;
                lastCatchedClimbHandle = null;
            }
            else if (Input.GetKey(owner.keyRight))
            {
                owner.turnRight();
                jumpRight();
                catchedClimbHandle = null;
                lastCatchedClimbHandle = null;
            }
            else if (Input.GetKey(owner.keyDown))
            {
                owner.velocity.x = 0.0f;
                owner.velocity.y = 0.0f;
                owner.setState(Ghost.State.IN_AIR);
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

        if (owner.currentActionTime >= CLIMBDUR_CLIMB)
        {
            owner.setState(Ghost.State.ON_GROUND);
            transform.position = climbAfterPos;

            if (owner.canGetUp())
            {
                setAction(Action.IDLE);
                resetActionAndState();
            }
            else
            {
                setAction(Action.CROUCH_IDLE);
                wantGetUp = !Input.GetKey(owner.keyDown);

                if (Input.GetKey(owner.keyLeft))
                {
                    keyLeftDown();
                }
                else if (Input.GetKey(owner.keyRight))
                {
                    keyRightDown();
                }
            }

        }
        else
        {
            float ratio = owner.currentActionTime / CLIMBDUR_CLIMB;
            transform.position = climbBeforePos + climbDistToClimb * ratio;
        }

        return 0;
    }

    int Action_ROPECLIMB_UP(float deltaTime)
    {

        if (!catchedRope)
            return 0;

        if (Input.GetKeyUp(owner.keyUp))
        {
            setAction(Action.ROPECLIMB_IDLE);
            return 0;
        }

        float climbDist = RopeClimbSpeedUp * owner.getCurrentDeltaTime();

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

    int Action_ROPECLIMB_DOWN(float deltaTime)
    {

        if (!catchedRope)
            return 0;

        if (Input.GetKeyUp(owner.keyDown))
        {
            setAction(Action.ROPECLIMB_IDLE);
            return 0;
        }

        if (tryJumpFromRope() != 0)
        {
            return 0;
        }

        float climbDist = RopeClimbSpeedDown * owner.getCurrentDeltaTime();

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
        if (isInState(Ghost.State.ON_GROUND))
        {

            switch (action)
            {

                case Action.IDLE:
                case Action.JUMP:
                case Action.CROUCH_IN:
                    setAction(Action.CROUCH_IDLE);
                    if (Input.GetKey(owner.keyLeft))
                    {
                        keyLeftDown();
                    }
                    else if (Input.GetKey(owner.keyRight))
                    {
                        keyRightDown();
                    }
                    else
                    {
                        owner.velocity.x = 0.0f;
                        owner.velocity.y = 0.0f;
                    }
                    break;

                case Action.WALK_LEFT:
                case Action.RUN_LEFT:
                case Action.JUMP_LEFT:
                case Action.JUMP_LEFT_LONG:
                    if (Input.GetKey(owner.keyLeft))
                    {
                        owner.velocity.x = 0.0f;
                        desiredSpeedX = CrouchSpeed;
                        if (owner.dir() == -Vector2.right)
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
                        owner.velocity.x = 0.0f;
                        owner.velocity.y = 0.0f;
                        setAction(Action.CROUCH_IDLE);
                    }
                    break;

                case Action.WALK_RIGHT:
                case Action.RUN_RIGHT:
                case Action.JUMP_RIGHT:
                case Action.JUMP_RIGHT_LONG:
                    if (Input.GetKey(owner.keyRight))
                    {
                        owner.velocity.x = 0.0f;
                        desiredSpeedX = CrouchSpeed;
                        if (owner.dir() == Vector2.right)
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
                        owner.velocity.x = 0.0f;
                        owner.velocity.y = 0.0f;
                        setAction(Action.CROUCH_IDLE);
                    }
                    break;
            }

        }
    }

    void preparetojump()
    {
        if (isNotInState(Ghost.State.ON_GROUND) || isNotInAction(Action.IDLE))
            return;

        owner.velocity.x = 0.0f;
        owner.velocity.y = 0.0f;
        setAction(Action.PREPARE_TO_JUMP);
    }

    void jump()
    {
        owner.AddImpulse(new Vector2(0.0f, JumpImpulse));
        owner.setState(Ghost.State.IN_AIR);
        setAction(Action.JUMP);

        lastFrameHande = false;
    }

    void jumpFromClimb()
    {
        owner.AddImpulse(new Vector2(0.0f, JumpImpulse));
        owner.setState(Ghost.State.IN_AIR);
        setAction(Action.JUMP, 1);
        lastFrameHande = false;
    }

    void jumpLeft()
    {
        owner.velocity.x = -JumpSpeed;
        owner.velocity.y = 0.0f;
        owner.AddImpulse(new Vector2(0.0f, JumpImpulse));
        owner.setState(Ghost.State.IN_AIR);
        setAction(Action.JUMP_LEFT);

        lastFrameHande = false;
    }

    void jumpRight()
    {
        owner.velocity.x = JumpSpeed;
        owner.velocity.y = 0.0f;
        owner.AddImpulse(new Vector2(0.0f, JumpImpulse));
        owner.setState(Ghost.State.IN_AIR);
        setAction(Action.JUMP_RIGHT);

        lastFrameHande = false;
    }

    void jumpLongLeft()
    {
        owner.velocity.x = -JumpLongSpeed;
        owner.velocity.y = 0.0f;
        owner.AddImpulse(new Vector2(0.0f, JumpLongImpulse));
        owner.setState(Ghost.State.IN_AIR);
        setAction(Action.JUMP_LEFT_LONG);

        lastFrameHande = false;
    }

    void jumpLongRight()
    {
        owner.velocity.x = JumpLongSpeed;
        owner.velocity.y = 0.0f;
        owner.AddImpulse(new Vector2(0.0f, JumpLongImpulse));
        owner.setState(Ghost.State.IN_AIR);
        setAction(Action.JUMP_RIGHT_LONG);

        lastFrameHande = false;
    }

    void turnLeftStart()
    {
        setAction(Action.TURN_STAND_LEFT);

        if (Input.GetKeyDown(owner.keyJump) || (Input.GetKey(owner.keyJump) && canJumpAfter))
            wantJumpAfter = true;
    }

    void turnRightStart()
    {
        setAction(Action.TURN_STAND_RIGHT);

        if (Input.GetKeyDown(owner.keyJump) || (Input.GetKey(owner.keyJump) && canJumpAfter))
            wantJumpAfter = true;
    }

    void turnLeftFinish()
    {
        setAction(Action.IDLE);

        if (wantJumpAfter)
        {
            jumpLeft();

            if (Input.GetKey(owner.keyJump))
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

            if (Input.GetKey(owner.keyJump))
                canJumpAfter = false;


        }
        else
        {
            resetActionAndState();
        }
    }

    void setActionIdle()
    {
        owner.velocity.x = 0.0f;
        setAction(Action.IDLE);
    }
    void setActionRopeClimbIdle()
    {
        if (owner.faceRight()) owner.AnimatorBody.Play("Zap_liana_climbup_R");
        else owner.AnimatorBody.Play("Zap_liana_climbup_L");
        owner.AnimatorBody.speed = 0f;
    }
    void setActionCrouchIdle()
    {
        owner.velocity.x = 0.0f;
        setAction(Action.CROUCH_IDLE);
    }
    void setActionMountIdle()
    {
        owner.velocity.x = 0.0f;
        owner.velocity.y = 0.0f;
        owner.setState(Ghost.State.MOUNT);
        setAction(Action.MOUNT_IDLE);
        resetActionAndState();
    }
    bool setMountIdle()
    {
        if (isInState(Ghost.State.MOUNT) && isNotInAction(Action.MOUNT_ATTACK_LEFT) && isNotInAction(Action.MOUNT_ATTACK_RIGHT))
        {
            owner.velocity.x = 0.0f;
            owner.velocity.y = 0.0f;
            setAction(Action.MOUNT_IDLE);

            return true;
        }
        return false;
    }

    void resetActionAndState()
    {
        if (isInState(Ghost.State.ON_GROUND))
        {
            if (Input.GetKey(owner.keyDown))
            { //&& (Input.GetKey(owner.keyLeft) || Input.GetKey(owner.keyRight)) ){
                if (keyDownDown() == 0)
                    setActionIdle();
            }
            else if (Input.GetKey(owner.keyLeft))
            {
                if (keyLeftDown() == 0)
                    setActionIdle();
            }
            else if (Input.GetKey(owner.keyRight))
            {
                if (keyRightDown() == 0)
                    setActionIdle();
            }
            else
            {
                if (isInState(Ghost.State.ON_GROUND))
                {
                    setActionIdle();
                }
            }
        }
        else if (isInState(Ghost.State.MOUNT))
        {

            if (Input.GetKey(owner.keyDown))
            { //&& (Input.GetKey(owner.keyLeft) || Input.GetKey(owner.keyRight)) ){
                if (keyDownDown() == 0)
                    setMountIdle();
            }
            else if (Input.GetKey(owner.keyUp))
            { //&& (Input.GetKey(owner.keyLeft) || Input.GetKey(owner.keyRight)) ){
                if (keyUpDown() == 0)
                    setMountIdle();
            }
            else if (Input.GetKey(owner.keyLeft))
            {
                if (keyLeftDown() == 0)
                    setMountIdle();
            }
            else if (Input.GetKey(owner.keyRight))
            {
                if (keyRightDown() == 0)
                    setMountIdle();
            }
            else
            {
                if (isInState(Ghost.State.ON_GROUND))
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
    bool mounting()
    {
        return isInAction(Action.MOUNT_LEFT) || isInAction(Action.MOUNT_RIGHT)
            || isInAction(Action.MOUNT_UP) || isInAction(Action.MOUNT_DOWN)
            || isInAction(Action.MOUNT_ATTACK_LEFT) || isInAction(Action.MOUNT_ATTACK_RIGHT);
    }
    public override bool crouching()
    {
        return isInAction(Action.CROUCH_IDLE) ||
            isInAction(Action.CROUCH_LEFT) || isInAction(Action.CROUCH_LEFT_BACK) ||
                isInAction(Action.CROUCH_RIGHT) || isInAction(Action.CROUCH_RIGHT_BACK);
    }

    public override void die(Ghost.DeathType deathType)
    {
        setAction(Action.DIE, (int)deathType);
    }

    public override bool triggerEnter(Collider2D other)
    {

        if (other.gameObject.tag == "Bird")
        {
            if (isInState(Ghost.State.MOUNT) || isInState(Ghost.State.CLIMB_ROPE))
            {
                owner.velocity.x = 0.0f;
                owner.velocity.y = 0.0f;
                setAction(Action.JUMP);
                owner.setState(Ghost.State.IN_AIR);

                if (owner.canBeFuddleFromBird)
                    owner.FuddleFromBird = true;

            }
            else if (isInState(Ghost.State.IN_AIR))
            {
                owner.velocity.x = 0.0f;
            }
            return true;
        }

        return false;
    }

    bool tryStartClimbPullDown()
    {
        GameObject potCatchedClimbHandle = canClimbPullDown();
        if (potCatchedClimbHandle)
        {

            catchedClimbHandle = potCatchedClimbHandle;

            owner.velocity.x = 0.0f;
            owner.velocity.y = 0.0f;

            Vector3 handlePos = potCatchedClimbHandle.transform.position;

            climbAfterPos.y = handlePos.y - 2.4f; //myHeight;
            if (owner.dir() == Vector2.right)
            {
                climbAfterPos.x = handlePos.x - owner.getMyHalfWidth();
            }
            else
            {
                climbAfterPos.x = handlePos.x + owner.getMyHalfWidth();
            }

            climbBeforePos = transform.position;
            climbDistToClimb = climbAfterPos - climbBeforePos;

            wantGetUp = false;
            setAction(Action.CLIMB_PULLDOWN);
            owner.setState(Ghost.State.CLIMB);

            return true;
        }
        return false;
    }

    bool checkSpeed(int dir)
    {
        float speedX = Mathf.Abs(owner.velocity.x);
        if (speedX < desiredSpeedX)
        { // trzeba przyspieszyc

            float velocityDamp = SpeedUpParam * owner.getCurrentDeltaTime();
            speedX += velocityDamp;
            if (speedX > desiredSpeedX)
            {
                speedX = desiredSpeedX;
                owner.velocity.x = desiredSpeedX * dir;
                return true;
            }
            owner.velocity.x = speedX * dir;
            return false;

        }
        else if (speedX > desiredSpeedX)
        { // trzeba zwolnic
            float velocityDamp = SlowDownParam * owner.getCurrentDeltaTime();
            speedX -= velocityDamp;
            if (speedX < desiredSpeedX)
            {
                speedX = desiredSpeedX;
                owner.velocity.x = desiredSpeedX * dir;
                return true;
            }
            owner.velocity.x = speedX * dir;
            return false;
        }
        return true;
    }

    bool speedLimiter(int dir, float absMaxSpeed)
    {
        if (dir == -1)
        {
            if (owner.velocity.x < 0.0f && Mathf.Abs(owner.velocity.x) > absMaxSpeed)
            {
                owner.velocity.x = -absMaxSpeed;
                return true;
            }
        }
        else if (dir == 1)
        {
            if (owner.velocity.x > 0.0f && Mathf.Abs(owner.velocity.x) > absMaxSpeed)
            {
                owner.velocity.x = absMaxSpeed;
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
                tryJumpFromRope(true);
                return false;
            }
        }
        return false;
    }

    int tryJumpFromRope(bool forceJumpOff = false)
    {

        if (Input.GetKeyDown(owner.keyJump) || forceJumpOff)
        {

            float ropeSpeed = catchedRope.firstLinkSpeed;
            float ropeSpeedRad = ropeSpeed * Mathf.Deg2Rad;
            int crl_idn = catchedRope.currentLink.GetComponent<RopeLink>().idn;
            float ps = ropeSpeedRad * (crl_idn + 1) * 0.5f;

            if (Input.GetKey(owner.keyLeft) && !forceJumpOff)
            { //skacze w lewo
                owner.turnLeft();

                if (ropeSpeed > 0f)
                { // lina tez leci w lewo
                    jumpLongLeft();
                    owner.velocity.x -= ps;
                }
                else
                {
                    jumpLeft();
                }
            }
            else if (Input.GetKey(owner.keyRight) && !forceJumpOff)
            { //skacze w prawo
                owner.turnRight();

                if (ropeSpeed < 0f)
                { // lina tez leci w prawo
                    jumpLongRight();
                    owner.velocity.y += ps;
                }
                else
                {
                    jumpRight();
                }
            }
            else if (Input.GetKeyDown(owner.keyDown) || Input.GetKey(owner.keyDown) || forceJumpOff)
            {
                owner.velocity.x = 0f;
                owner.velocity.y = 0f;
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

            Vector3 _oldPos = transform.position;
            _oldPos.y -= 1.65f;
            transform.position = _oldPos;

            justJumpedRope = catchedRope;

            catchedRope.resetDiver();
            catchedRope = null;
            catchedRopeLink = null;

            Quaternion quat = new Quaternion();
            quat.eulerAngles = new Vector3(0f, 0f, 0f);
            transform.rotation = quat;

            owner.setState(Ghost.State.IN_AIR);

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

    bool tryCatchHandle()
    {
        if (owner.dir() == Vector2.right)
        {

            RaycastHit2D hit;
            if (lastFrameHande)
                hit = Physics2D.Linecast(lastHandlePos, owner.sensorHandleR2.position, owner.layerIdGroundHandlesMask);
            else
                hit = Physics2D.Linecast(owner.sensorHandleR2.position, owner.sensorHandleR2.position, owner.layerIdGroundHandlesMask);

            if (hit.collider != null)
            {
                // tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
                bool _canCatch = true;
                if ((lastCatchedClimbHandle == hit.collider.gameObject))
                { //{ && owner.velocity.y >= 0.0f) {
                    _canCatch = false;
                }

                if (_canCatch)
                {
                    catchedClimbHandle = hit.collider.gameObject;

                    Vector3 handlePos = catchedClimbHandle.transform.position;
                    Vector3 newPos = new Vector3();
                    newPos.x = handlePos.x - owner.getMyHalfWidth();
                    newPos.y = handlePos.y - 2.4f; //myHeight;

                    canPullUp = canClimbPullUp();

                    if (canPullUp)
                    {
                    }

                    owner.velocity.x = 0.0f;
                    owner.velocity.y = 0.0f;

                    climbBeforePos = transform.position;
                    climbAfterPos = newPos;
                    climbDistToClimb = climbAfterPos - climbBeforePos;
                    climbToJumpDuration = climbDistToClimb.magnitude * 0.5f;

                    owner.setState(Ghost.State.CLIMB);
                    setAction(Action.CLIMB_JUMP_TO_CATCH);
                    lastFrameHande = false;

                    return true;
                }
            }

            lastHandlePos = owner.sensorHandleR2.position;
            return false;

        }
        else
        {

            RaycastHit2D hit;
            if (lastFrameHande)
                hit = Physics2D.Linecast(lastHandlePos, owner.sensorHandleL2.position, owner.layerIdGroundHandlesMask);
            else
                hit = Physics2D.Linecast(owner.sensorHandleL2.position, owner.sensorHandleL2.position, owner.layerIdGroundHandlesMask);


            if (hit.collider != null)
            {

                // tu takie zabezpieczenie dodatkowe aby nie lapal sie od razu tego co ma pod reka
                bool _canCatch = true;
                if ((lastCatchedClimbHandle == hit.collider.gameObject))
                { // && owner.velocity.y >= 0.0f) {
                    _canCatch = false;
                }

                if (_canCatch)
                {
                    catchedClimbHandle = hit.collider.gameObject;

                    Vector3 handlePos = catchedClimbHandle.transform.position;
                    Vector3 newPos = new Vector3();
                    newPos.x = handlePos.x + owner.getMyHalfWidth();
                    newPos.y = handlePos.y - 2.4f; //myHeight;

                    canPullUp = canClimbPullUp();

                    if (canPullUp)
                    {
                    }

                    owner.velocity.x = 0.0f;
                    owner.velocity.y = 0.0f;

                    climbBeforePos = transform.position;
                    climbAfterPos = newPos;
                    climbDistToClimb = climbAfterPos - climbBeforePos;
                    climbToJumpDuration = climbDistToClimb.magnitude * 0.5f;

                    owner.setState(Ghost.State.CLIMB);
                    setAction(Action.CLIMB_JUMP_TO_CATCH);
                    lastFrameHande = false;

                    return true;
                }
            }

            lastHandlePos = owner.sensorHandleL2.position;
            return false;
        }
    }

    bool tryCatchRope()
    {

        if (owner.dir() == Vector2.right)
        {


            RaycastHit2D hit;
            if (lastFrameHande)
                hit = Physics2D.Linecast(lastHandlePos, owner.sensorHandleR2.position, owner.layerIdRopesMask);
            else
                hit = Physics2D.Linecast(owner.sensorHandleR2.position, owner.sensorHandleR2.position, owner.layerIdRopesMask);

            if (hit.collider == null)
            {
                hit = Physics2D.Linecast(owner.sensorHandleL2.position, owner.sensorHandleR2.position, owner.layerIdRopesMask);
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
                        lastHandlePos = owner.sensorHandleR2.position;
                        return false;
                    }

                    catchedRope = catchedRopeLink.rope;

                    catchedRope.chooseDriver(catchedRopeLink.transform);

                    float forceRatio = Mathf.Abs(owner.velocity.x) / JumpLongSpeed;
                    float force = RopeSwingForce * forceRatio;

                    if (owner.velocity.x < 0f)
                    {
                        catchedRope.setSwingMotor(-Vector2.right, force, 0.25f);
                    }
                    else if (owner.velocity.x > 0)
                    {
                        catchedRope.setSwingMotor(Vector2.right, force, 0.25f);
                    }

                    owner.velocity.x = 0.0f;
                    owner.velocity.y = 0.0f;

                    owner.setState(Ghost.State.CLIMB_ROPE);
                    setAction(Action.ROPECLIMB_IDLE);

                    transform.position = catchedRopeLink.transform.position;
                    transform.rotation = catchedRopeLink.transform.rotation;

                    ropeLinkCatchOffset = 0.0f;

                    return true;
                }
            }

            lastHandlePos = owner.sensorHandleR2.position;
            return false;

        }
        else
        {

            RaycastHit2D hit;
            if (lastFrameHande)
                hit = Physics2D.Linecast(lastHandlePos, owner.sensorHandleL2.position, owner.layerIdRopesMask);
            else
                hit = Physics2D.Linecast(owner.sensorHandleL2.position, owner.sensorHandleL2.position, owner.layerIdRopesMask);

            if (hit.collider == null)
            {
                hit = Physics2D.Linecast(owner.sensorHandleL2.position, owner.sensorHandleR2.position, owner.layerIdRopesMask);
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
                        lastHandlePos = owner.sensorHandleL2.position;
                        return false;
                    }

                    catchedRope = catchedRopeLink.rope;

                    catchedRope.chooseDriver(catchedRopeLink.transform);

                    float forceRatio = Mathf.Abs(owner.velocity.x) / JumpLongSpeed;
                    float force = RopeSwingForce * forceRatio;

                    if (owner.velocity.x < 0f)
                    {
                        catchedRope.setSwingMotor(-Vector2.right, force, 0.25f);
                    }
                    else if (owner.velocity.x > 0)
                    {
                        catchedRope.setSwingMotor(Vector2.right, force, 0.25f);
                    }

                    owner.velocity.x = 0.0f;
                    owner.velocity.y = 0.0f;

                    owner.setState(Ghost.State.CLIMB_ROPE);
                    setAction(Action.ROPECLIMB_IDLE);

                    transform.position = catchedRopeLink.transform.position;
                    transform.rotation = catchedRopeLink.transform.rotation;

                    ropeLinkCatchOffset = 0.0f;
                    return true;
                }
            }

            lastHandlePos = owner.sensorHandleL2.position;
            return false;
        }
    }

    float ropeLinkCatchOffset = 0.0f;

    bool canClimbPullUp()
    {

        if (!catchedClimbHandle)
            return false;

        Vector2 rayOrigin = catchedClimbHandle.transform.parent.transform.position;
        rayOrigin.x += 0.5f;
        rayOrigin.y += 0.25f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.5f, owner.layerIdGroundMask);
        return !hit.collider;
    }

    public float ClimbPullDownRange = 0.511f;

    GameObject canClimbPullDown()
    {

        if (!isInState(Ghost.State.ON_GROUND) || !(isInAction(Action.IDLE) || isInAction(Action.CROUCH_IDLE)))
            return null;

        // 1: sytuacja gdy zap jest swoim srodkiem nad tilem
        // 2: sytuacja gdy zap jest swoim srodkiem juz poza tilem

        RaycastHit2D hit;

        if (owner.dir() == Vector2.right)
        { //

            hit = Physics2D.Raycast(owner.sensorDown2.position, -Vector2.right, ClimbPullDownRange, owner.layerIdGroundHandlesMask);
            if (hit.collider)
            {

                if (Physics2D.Raycast(hit.collider.gameObject.transform.position, -Vector2.right, 0.5f, owner.layerIdGroundMask).collider == null)
                {
                    return hit.collider.gameObject;
                }
            }

        }
        else
        {

            hit = Physics2D.Raycast(owner.sensorDown2.position, Vector2.right, ClimbPullDownRange, owner.layerIdGroundHandlesMask);
            if (hit.collider)
            {

                if (Physics2D.Raycast(hit.collider.gameObject.transform.position, Vector2.right, 0.5f, owner.layerIdGroundMask).collider == null)
                {
                    return hit.collider.gameObject;
                }

            }
        }

        // to jest ta druga sytuacja ...

        Vector2 rayOrigin = owner.sensorDown1.position;
        hit = Physics2D.Raycast(rayOrigin, Vector2.right, owner.getMyWidth(), owner.layerIdGroundHandlesMask);

        if (hit.collider)
        {
            // badam czy stoje na krawedzi odpowiednio zwrocony
            if (owner.dir() == Vector2.right)
            { //

                // pod lewa noga musi byc przepasc
                rayOrigin = owner.sensorDown1.position;
                if (Physics2D.Raycast(rayOrigin, -Vector2.up, 0.5f, owner.layerIdGroundMask).collider) return null;
                else return hit.collider.gameObject;

            }
            else
            {

                // pod prawa noga musi byc przepasc
                rayOrigin = owner.sensorDown3.position;
                if (Physics2D.Raycast(rayOrigin, -Vector2.up, 0.5f, owner.layerIdGroundMask).collider) return null;
                else return hit.collider.gameObject;

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
}
