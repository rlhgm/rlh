using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
//using System; //This allows the IComparable Interface

//[System.Serializable]
public class ZapControllerNormal : ZapController
{
    //float CurrentJumpMaxSpeed = 0;

    public float WalkSpeed = 2.5f;
    public float RunSpeed = 5.7f;
    public float JumpWalkSpeed = 3.8f;
    public float JumpWalkImpulse = 7.0f;
    public float JumpRunMaxSpeed = 4.9f;
    public float JumpRunMaxThreshold = 0.75f;
    public float JumpRunImpulse = 7.15f;    
    public float JumpFromClimbSpeed = 5.5f;
    public float JumpFromClimbImpulse = 5.0f;
    public float JumpFromClimbSideImpulse = 5.0f;

    public float CrouchSpeed = 1.8f;
    public float MountSpeed = 2.0f; // ile na sek.
    public float MountJumpDist = 10.0f; // następnie naciskasz spacje a on skacze
    public float WalkSpeedUpParam = 10.0f; // ile jednosek predkosci hamuje na sekund
    public float WalkSlowDownParam = 20.0f; // ile jednosek predkosci hamuje na sekunde
    public float RunSpeedUpParam = 10.0f; // ile jednosek predkosci hamuje na sekund
    public float RunSlowDownParam = 20.0f; // ile jednosek predkosci hamuje na sekunde
    public float TurnRunSlowDownParam = 5.0f; // ile jednosek predkosci hamuje na sekunde
    public float CrouchSpeedUpParam = 10.0f; // ile jednosek predkosci hamuje na sekund
    public float CrouchSlowDownParam = 20.0f; // ile jednosek predkosci hamuje na sekunde
    public float JumpLeftRightSpeedUpParam = 8.0f; // ile przyspiesza na sekunde lecac
    public float JumpLeftRightSpeedUpAfterCollideParam = 8.0f; // ile przyspiesza na sekunde lecac
    public float JumpLeftRightSlowDownParam = 8.0f; // ile przyspiesza na sekunde lecac
    public float JumpUpControlParam = 9.0f; // ile przyspiesza na sekunde lecac
    public float JumpRunMaxSpeedSlowDown = 9.0f;
    public bool JumpDirChangePossible = true;
    public float PushMaxSpeed = 2f;
    public float PullMaxSpeed = 2f;
    public float PushbackMaxSpeed = 1f;
    public float HmmPullForce = 1f;

    //public float PushbackInDuration = 0.09f;




    public float GravityForce = -20.0f;
    public float MaxSpeedY = 15.0f;

    public float HardLandingHeight = 2.0f;
    public float VeryHardLandingHeight = 6.0f;
    public float MaxFallDistToCatch = 6.0f;

    public float RopeSwingForce = 4250f;
    public float RopeClimbSpeedUp = 1.0f;
    public float RopeClimbSpeedDown = 3.0f;

    public float ClimbDuration = 1.5f;
    public float ClimbDurPrepareToJump = 0.5f;
    public float ClimbDurJumpToCatch = 0.2f; // jednostka w 0.2f
    public float ClimbDurCatch = 0.5f;
    /*public*/
    float ClimbDurClimb = 0.65f;
    public float LandingHardDuration = 0.3f;

    public float TurnWalkDuration = 0.2f;
    public float TurnRunDuration = 0.53f;
    public float TurnRunSlowDown = 1f;
    public float MountAttackDuration = 0.5f;
    
    public float BirdHitDuration = 0.33f;

    public float PullUpDuration = 0.7f;
    public float PullDownDuration = 0.7f;

    public float MaxPlatformAngle = 15f;

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

    //public GameObject landingHardParticles = null;
    
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
            case Action.Idle:
                if (Action_IDLE() != 0)
                    return;
                break;

            case Action.LandingHard:
                Action_LANDING_HARD();
                break;

            //case Action.PrepareToJump:
            //    if (zap.currentActionTime >= 0.2f)
            //    {
            //        jump();
            //    }
            //    break;

            case Action.CLIMB_PULLDOWN:
                Action_CLIMB_PULLDOWN();
                break;

            case Action.CLIMB_JUMP_TO_CATCH:
                Action_CLIMB_JUMP_TO_CATCH();
                break;

            case Action.CLIMB_CATCH:
                ActionClimbCatch();
                break;

            case Action.CLIMB_CLIMB:
                Action_CLIMB_CLIMB();
                break;

            case Action.ClimbBelly:
                ActionClimbBelly();
                break;

            case Action.WalkLeft:
                if (Action_WALK(-1) != 0)
                    return;
                break;
            case Action.RunLeft:
                if (ActionRun(-1) != 0)
                    return;
                break;

            case Action.WalkRight:
                if (Action_WALK(1) != 0)
                    return;
                break;
            case Action.RunRight:
                if (ActionRun(1) != 0)
                    return;
                break;

            case Action.TurnStandLeft:
                if (Input.GetKeyDown(zap.keyJump))
                {
                    wantJumpAfter = true;
                }
                if (zap.currentActionTime >= TurnWalkDuration)
                {
                    zap.turnLeft();
                    turnLeftFinish();
                }
                break;

            case Action.TurnStandRight:
                if (Input.GetKeyDown(zap.keyJump))
                {
                    wantJumpAfter = true;
                }
                if (zap.currentActionTime >= TurnWalkDuration)
                {
                    zap.turnRight();
                    turnRightFinish();
                }
                break;

            case Action.TurnRunLeft:
                if (zap.currentActionTime >= TurnRunDuration)
                {
                    zap.turnLeft();
                    if (wantJumpAfter)
                    {
                        JumpWalkLeft();
                    }
                    else
                    {
                        setActionIdle();
                        resetActionAndState();
                        if (moving(-1))
                        {
                            if (isInAction(Action.WalkLeft))
                            {
                                zap.velocity.x = Mathf.Max(-WalkSpeed, -beforTurnRunSpeed);
                                //zap.velocity.x += TurnRunSlowDown;
                            }
                            else if (isInAction(Action.RunLeft))
                            {
                                zap.velocity.x = Mathf.Max(-RunSpeed, -beforTurnRunSpeed);
                                zap.velocity.x += TurnRunSlowDown;
                                zap.velocity.x = Mathf.Min(0f, zap.velocity.x);
                            }
                        }
                    }
                }
                else
                {
                    int res = ActionTurnRun(1);
                    if (res == 1)
                    {
                    }
                }
                break;

            case Action.TurnRunRight:
                if (zap.currentActionTime >= TurnRunDuration)
                {
                    zap.turnRight();
                    if (wantJumpAfter)
                    {
                        JumpWalkRight();
                    }
                    else
                    {
                        setActionIdle();
                        resetActionAndState();
                        if (moving(1))
                        {
                            if (isInAction(Action.WalkRight))
                            {
                                zap.velocity.x = Mathf.Min(WalkSpeed, -beforTurnRunSpeed);
                                //zap.velocity.x -= TurnRunSlowDown;
                            }
                            else if (isInAction(Action.RunRight))
                            {
                                zap.velocity.x = Mathf.Min(RunSpeed, -beforTurnRunSpeed);
                                zap.velocity.x -= TurnRunSlowDown;
                                zap.velocity.x = Mathf.Max(0f, zap.velocity.x);
                            }
                        }
                    }
                }
                else
                {
                    int res = ActionTurnRun(-1);
                    if (res == 1)
                    {
                    }
                }
                break;

            case Action.CrouchIn:
                ActionCrouchIn();
                break;

            case Action.GetUp:
                ActionGetUp();
                break;

            case Action.CrouchIdle:
                if (ActionCrouchIdle() != 0)
                {
                    return;
                }
                break;

            case Action.CrouchLeft:
            case Action.CrouchLeftBack:
                if (ActionCrouchLeftRight(-1) != 0)
                {
                    return;
                }
                break;

            case Action.CrouchRight:
            case Action.CrouchRightBack:
                if (ActionCrouchLeftRight(1) != 0)
                {
                    return;
                }
                break;

            case Action.MountIdle:
                Action_MOUNT_IDLE();
                break;

            case Action.MOUNT_BIRDHIT:
                Action_MOUNT_BIRDHIT();
                break;

            case Action.MountLeft:
            case Action.MountRight:
            case Action.MountUp:
                ActionMounting();
                break;

            case Action.MountDown:
                ActionMountingDown();
                break;

            case Action.MOUNT_ATTACK_LEFT:
            case Action.MOUNT_ATTACK_RIGHT:
                Action_MOUNT_ATTACK();
                break;

            case Action.PullUp:
                ActionPullUp();
                break;

            case Action.PullDown:
                ActionPullDown();
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

            case Action.PUSHBACKIN_LEFT:
                ActionPushbackIn(deltaTime);
                break;
            case Action.PUSHBACKIN_RIGHT:
                ActionPushbackIn(deltaTime);
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
                setAction(Action.GetUp);
                wantGetUp = false;
            }
        }

        switch (zap.getState())
        {

            case Zap.State.MOUNT:
                if (cs)
                {
                    //Debug.Log("Crumble : keyUpDown");
                    cs.TryToCrumble(deltaTime);
                    //cs = null;
                }
                if (handledMountMoveable)
                {
                    Vector3 handPos = zap.sensorLeft3.position; // + 0.3f;
                    handPos.x += zap.getMyHalfWidth();
                    handPos.y += 0.3f;

                    if ( handledMountMoveable.TryToCollapse(deltaTime, handPos))
                    {
                        cs = null;
                        zap.velocity.x = 0.0f;
                        zap.velocity.y = 0.0f;
                        setAction(Action.Jump);
                        zap.setState(Zap.State.IN_AIR);
                        handledMountMoveable = null;
                        break;
                    }

                    //Vector3 worldHandledMountMoveablePosition = handledMountMoveable.transform.TransformPoint(handledMountMoveablePosition);

                    //Vector3 _p1 = worldHandledMountMoveablePosition;
                    //_p1.x -= 0.1f;
                    //_p1.y -= 0.1f;
                    //Vector3 _p2 = worldHandledMountMoveablePosition;
                    //_p2.x += 0.1f;
                    //_p2.y += 0.1f;
                    //Debug.DrawLine(_p1, _p2, Color.red);

                    //_p1 = worldHandledMountMoveablePosition;
                    //_p1.x -= 0.1f;
                    //_p1.y += 0.1f;
                    //_p2 = worldHandledMountMoveablePosition;
                    //_p2.x += 0.1f;
                    //_p2.y -= 0.1f;
                    //Debug.DrawLine(_p1, _p2, Color.red);


                    //worldHandledMountMoveablePosition.y -= (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                    //transform.position = worldHandledMountMoveablePosition;

                    //Rigidbody2D hmmBody = handledMountMoveable.GetComponent<Rigidbody2D>();
                    //if (hmmBody)
                    //{
                    //    hmmBody.AddForceAtPosition(new Vector2(0f, -HmmPullForce), worldHandledMountMoveablePosition);
                    //}
                    //else
                    //{
                    //    Debug.LogError("handledMountMoveable nie ma body");
                    //    Debug.Break();
                    //}
                }
                //zap.climbingWallID = zap.layerIdMountMask;
                //setActionMountIdle();
                break;

            case Zap.State.IN_AIR:

                if (zap.jumpKeyPressed)
                { //Input.GetKeyDown(zap.keyJump) || Input.GetKey(zap.keyJump) ){
                    //Debug.Log("zap.jumpKeyPressed");
                    Vector3 fallDist = zap.startFallPos - transform.position;
                    if (!zap.FuddleFromBird && (fallDist.y < MaxFallDistToCatch))
                    {
                        Transform handle = /*zap.*/CheckHandle(zap.layerIdMountMask);
                        if (handle)
                        {
                            if (jumpFromMount)
                            {
                                if (!justJumpedMount)
                                {
                                }
                            }
                            else
                            {
                                handledMountMoveable = handle.GetComponent<MountMoveable>();
                                if ( handledMountMoveable )
                                {
                                    //handledMountMoveablePosition;
                                    //Vector2 rayOrigin = zap.sensorLeft3.transform.position; // transform.position;
                                    //rayOrigin.y += 0.3f;
                                    Vector3 zapHandpos = zap.transform.position;
                                    zapHandpos.y += (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                                    handledMountMoveablePosition = handledMountMoveable.transform.InverseTransformPoint(zapHandpos);

                                    handledMountMoveable.Catched();
                                    
                                    //Vector3 _pointToCheck = handledMountMoveable.transform.InverseTransformPoint(zapHandpos);
                                    //bool res = handledMountMoveable.LocalPointHandable(_pointToCheck);
                                    //if( res )
                                    //{
                                    //    //return hit.collider.transform;
                                    //    zap.climbingWallID = zap.layerIdMountMask;
                                    //    setActionMountIdle();
                                    //    return;
                                    //}
                                    //Debug.Log(handledMountMoveablePosition);
                                }
                                zap.climbingWallID = zap.layerIdMountMask;
                                setActionMountIdle();
                                return;
                            }
                        }
                        else if (/*zap.*/CheckHandle(zap.layerIdGroundFarMask))
                        {
                            if (jumpFromMount)
                            {
                                if (!justJumpedMount)
                                {
                                }
                            }
                            else
                            {
                                zap.climbingWallID = zap.layerIdGroundFarMask;
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
                CurrentJumpRunMaxSpeed = Mathf.Max(0.0f, CurrentJumpRunMaxSpeed-(deltaTime * JumpRunMaxSpeedSlowDown));

                if (isInAction(Action.JumpLeft))
                {
                    if (Input.GetKey(zap.keyRight))
                    {
                        zap.velocity.x += (JumpLeftRightSlowDownParam * deltaTime);
                        if( !JumpDirChangePossible )
                        {
                            if (zap.velocity.x > 0.0f) zap.velocity.x = 0.0f;
                        }
                    }
                    else if( Input.GetKey(zap.keyLeft) )
                    {
                        //public float JumpLeftRightSpeedUpParam = 8.0f; // ile przyspiesza na sekunde lecac
                        //public float JumpLeftRightSpeedUpAfterCollideParam = 8.0f; // ile przyspiesza na sekunde lecac
                        if (zap.velocity.x > jumpStartVelocity)
                        {
                            zap.velocity.x -= (JumpLeftRightSpeedUpParam * deltaTime);
                            //zap.velocity.x = Mathf.Max(zap.velocity.x, -JumpRunMaxSpeed);
                            //zap.velocity.x = Mathf.Max(zap.velocity.x, -CurrentJumpRunMaxSpeed);
                        }
                    }
                    zap.velocity.x = Mathf.Max(zap.velocity.x, -CurrentJumpRunMaxSpeed);
                }
                else if (isInAction(Action.JumpRight))
                {
                    if (Input.GetKey(zap.keyLeft))
                    {
                        zap.velocity.x -= (JumpLeftRightSlowDownParam * deltaTime);
                        if (!JumpDirChangePossible)
                        {
                            if (zap.velocity.x < 0.0f) zap.velocity.x = 0.0f;
                        }
                    }
                    else if (Input.GetKey(zap.keyRight))
                    {
                        //public float JumpLeftRightSpeedUpParam = 8.0f; // ile przyspiesza na sekunde lecac
                        //public float JumpLeftRightSpeedUpAfterCollideParam = 8.0f; // ile przyspiesza na sekunde lecac
                        if( zap.velocity.x < jumpStartVelocity )
                        {
                            zap.velocity.x += (JumpLeftRightSpeedUpParam * deltaTime);
                            //zap.velocity.x = Mathf.Min(zap.velocity.x, JumpRunMaxSpeed);
                            //zap.velocity.x = Mathf.Min(zap.velocity.x, CurrentJumpRunMaxSpeed);
                        }
                    }
                    zap.velocity.x = Mathf.Min(zap.velocity.x, CurrentJumpRunMaxSpeed);
                }
                else if (isInAction(Action.Jump))
                {
                    if (Input.GetKey(zap.keyLeft))
                    {
                        zap.velocity.x -= (JumpUpControlParam * deltaTime);
                        if (Mathf.Abs(zap.velocity.x) > JumpWalkSpeed)
                            zap.velocity.x = -JumpWalkSpeed;
                    }
                    if (Input.GetKey(zap.keyRight))
                    {
                        zap.velocity.x += (JumpUpControlParam * deltaTime);
                        if (Mathf.Abs(zap.velocity.x) > JumpWalkSpeed)
                            zap.velocity.x = JumpWalkSpeed;
                    }
                }

                if (zap.velocity.x > 0.0f && zap.faceLeft())
                {
                    zap.turnRight();
                }
                else if (zap.velocity.x < 0.0f && zap.faceRight())
                {
                    zap.turnLeft();
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
                    //zap.playSound(zap.landingSound);

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
                        float _distToGround = 0.0f;
                        zap.checkGround(ref _distToGround);
                        //groundUnder = groundUnderFeet;

                        //if( zap.beforeFallController == null ){
                        zap.velocity.x = 0.0f;
                        setAction(Action.LandingHard);
                        //}else{
                        //}

                    }
                    else
                    {
                        float _distToGround = 0.0f;
                        zap.checkGround(ref _distToGround);

                        if (zap.groundUnder)
                        {
                            if (zap.particleSet)
                            {
                                ParticleData _pd = null;
                                Surface surface = zap.groundUnder.GetComponent<Surface>();
                                if (surface)
                                {
                                    _pd = zap.particleSet.GetParticleData("landing*" + surface.type);
                                    
                                    if (_pd != null)
                                    {
                                        ParticleInseter.Insert(_pd, transform.position);
                                    }
                                }
                                else
                                {
                                }
                            }
                        }

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

                zap.pushOutFromObstacles(true);

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
                    setAction(Action.Jump);
                    wantGetUp = false;
                }
                else if (zap.groundUnder)
                {
                    if (crouching() && !isInAction(Action.CrouchIdle))
                    {
                        zap.SetGfxRotation(zap.GroundUnderAngle);
                    }
                    else
                    {
                        zap.SetGfxRotation(0f);
                    }

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
                else
                {
                    //Quaternion quat = gfx.rotation;
                    //quat.eulerAngles = new Vector3(0f, 0f, 0f);
                    //gfx.rotation = quat;

                    //zap.SetRotation(0f);
                    zap.SetGfxRotation(0f);
                }

                if (isInAction(Action.Idle))
                {
                    zap.pushOutFromObstacles(false);
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

            //case Zap.State.MOUNT:

            //    break;
        };
        
        zap.lastVelocity = zap.velocity;
    }

    public override void FUpdate(float fDeltaTime)
    {
        //Debug.Log("FUpdate : " + fDeltaTime);
        switch (action)
        {
            case Action.PUSH_LEFT:
            case Action.PUSH_RIGHT:
                ActionPush(fDeltaTime);
                break;

            case Action.PULL_LEFT:
            case Action.PULL_RIGHT:
                ActionPull(fDeltaTime);
                break;

            case Action.PUSHBACK_LEFT:
            case Action.PUSHBACK_RIGHT:
                ActionPushback(fDeltaTime);
                break;
        }

        switch (zap.getState())
        {
            case Zap.State.MOUNT:

                if (handledMountMoveable)
                {
                    
                    if( handledMountMoveable.MovingInLocal )
                    {
                        Vector3 newHandledMountMoveablePosition = handledMountMoveablePosition;
                        bool needRepos = false;
                        if ( handledMountMoveable.MovingXEnabled )
                        {
                            float desiredLocalY = -handledMountMoveable.mySize.y * 0.5f;
                            float diffLocalY = desiredLocalY - handledMountMoveablePosition.y;
                            if (Mathf.Abs(diffLocalY) > Mathf.Abs(handledMountMoveable.mySize.y / 4.0f))
                            {
                                needRepos = true;
                                newHandledMountMoveablePosition.y += (diffLocalY * 0.5f /*fDeltaTime * 6f*/);
                            }
                        }
                        if (handledMountMoveable.MovingYEnabled)
                        {
                            float desiredLocalX = handledMountMoveable.mySize.x * 0.5f;
                            float diffLocalX = desiredLocalX - handledMountMoveablePosition.x;
                            if (Mathf.Abs(diffLocalX) > Mathf.Abs(handledMountMoveable.mySize.x / 4.0f))
                            {
                                needRepos = true;
                                newHandledMountMoveablePosition.x += (diffLocalX * 0.5f /*fDeltaTime * 6f*/);
                            }
                        }
                        if (needRepos)
                        {
                            //Debug.Log(handledMountMoveablePosition.y);
                            handledMountMoveablePosition = newHandledMountMoveablePosition;
                            //Debug.Log(handledMountMoveablePosition.y);
                        }
                    }

                    Vector3 worldHandledMountMoveablePosition = handledMountMoveable.transform.TransformPoint(handledMountMoveablePosition);

                    //Debug.Log(worldHandledMountMoveablePosition.y);

                    Vector3 _p1 = worldHandledMountMoveablePosition;
                    _p1.x -= 0.1f;
                    _p1.y -= 0.1f;
                    Vector3 _p2 = worldHandledMountMoveablePosition;
                    _p2.x += 0.1f;
                    _p2.y += 0.1f;
                    Debug.DrawLine(_p1, _p2, Color.red);

                    _p1 = worldHandledMountMoveablePosition;
                    _p1.x -= 0.1f;
                    _p1.y += 0.1f;
                    _p2 = worldHandledMountMoveablePosition;
                    _p2.x += 0.1f;
                    _p2.y -= 0.1f;
                    Debug.DrawLine(_p1, _p2, Color.red);


                    worldHandledMountMoveablePosition.y -= (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                    transform.position = worldHandledMountMoveablePosition;

                    Rigidbody2D hmmBody = handledMountMoveable.GetComponent<Rigidbody2D>();
                    if (hmmBody)
                    {
                        hmmBody.AddForceAtPosition(new Vector2(0f, -HmmPullForce), worldHandledMountMoveablePosition);
                    }
                    else
                    {
                        //Debug.LogError("handledMountMoveable nie ma body");
                        //Debug.Break();
                    }
                }
                break;
        }
    }

    public override void activateSpec(bool restore = false, bool crouch = false)
    {
        //base.activate ();

        setAction(Action.Undef);
        setAction(Action.Idle);
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
        setAction(Action.Jump);
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
        Undef = 0,
        Idle,
        WalkLeft,
        WalkRight,
        RunLeft,
        RunRight,
        TurnStandLeft,
        TurnStandRight,
        TurnRunLeft,
        TurnRunRight,
        //PrepareToJump,
        Jump,
        JumpLeft,
        JumpRight,
        CrouchIn,
        GetUp,
        CrouchIdle,
        CrouchLeft,
        CrouchRight,
        CrouchLeftBack,
        CrouchRightBack,
        LandingHard,
        Fall,
        //STOP_WALK,
        //STOP_RUN,
        CLIMB_PREPARE_TO_JUMP,
        CLIMB_JUMP_TO_CATCH,
        CLIMB_CATCH,
        CLIMB_CLIMB,
        CLIMB_PULLDOWN,
        ClimbBelly,
        MountIdle,
        MountLeft,
        MountRight,
        MountUp,
        MountDown,
        MOUNT_ATTACK_LEFT,
        MOUNT_ATTACK_RIGHT,
        MOUNT_BIRDHIT,

        PullUp,
        PullDown,

        //HANG_IDLE,
        //HANG_LEFT,
        //HANG_RIGHT,
        //HANG_UP,
        //HANG_DOWN,
        //HANG_ATTACK_LEFT,
        //HANG_ATTACK_RIGHT,
        //HANG_BIRDHIT,

        ROPECLIMB_IDLE,
        ROPECLIMB_UP,
        ROPECLIMB_DOWN,
        PUSH_LEFT,
        PUSH_RIGHT,
        PULL_LEFT,
        PULL_RIGHT,
        PUSHBACK_LEFT,
        PUSHBACK_RIGHT,
        PUSHBACKIN_LEFT,
        PUSHBACKIN_RIGHT,
        Die
    };

    Action getAction()
    {
        return action;
    }

    int lastActionParam = 0;
    Vector3 actionChangedPos = new Vector3();

    bool setAction(Action newAction, int param = 0)
    {
        if (action == newAction)
            return false;



        action = newAction;
        zap.resetCurrentActionTime();
        zap.AnimatorBody.speed = 1f;

        zap.MountAttackLeftCollider.SetActive(false);
        zap.MountAttackRightCollider.SetActive(false);
        zap.KnifeAttackLeftHighCollider.SetActive(false);
        zap.KnifeAttackRightHighCollider.SetActive(false);
        zap.KnifeAttackLeftLowCollider.SetActive(false);
        zap.KnifeAttackRightLowCollider.SetActive(false);

        lastActionParam = param;

        actionChangedPos = transform.position;

        //Debug.Log(action);

        switch (newAction)
        {
            case Action.Idle:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_idle_R");
                else zap.AnimatorBody.Play("Zap_idle_L");
                break;

            case Action.Die:
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

                //if (zap.dieSounds.Length != 0)
                //    zap.playSound(zap.dieSounds[Random.Range(0, zap.dieSounds.Length)]);
                break;

            case Action.WalkLeft:
                zap.AnimatorBody.Play("Zap_walk_L");
                break;
            case Action.WalkRight:
                zap.AnimatorBody.Play("Zap_walk_R");
                break;

            case Action.RunLeft:
                zap.velocity.x = Mathf.Min(-WalkSpeed, zap.velocity.x);
                zap.AnimatorBody.Play("Zap_run_L");
                break;
            case Action.RunRight:
                zap.velocity.x = Mathf.Max(WalkSpeed, zap.velocity.x);
                zap.AnimatorBody.Play("Zap_run_R");
                break;

            case Action.TurnStandLeft:
                zap.AnimatorBody.Play("Zap_walk_back_left");
                wantJumpAfter = false;
                break;

            case Action.TurnStandRight:
                zap.AnimatorBody.Play("Zap_walk_back_right");
                wantJumpAfter = false;
                break;

            case Action.TurnRunLeft:
                beforTurnRunSpeed = zap.velocity.x;
                zap.AnimatorBody.Play("Zap_runback_R");
                wantJumpAfter = false;
                //if (zap.turnRunSounds.Length != 0)
                //    zap.playSound(zap.turnRunSounds[Random.Range(0, zap.turnRunSounds.Length)]);
                break;

            case Action.TurnRunRight:
                beforTurnRunSpeed = zap.velocity.x;
                zap.AnimatorBody.Play("Zap_runback_L");
                wantJumpAfter = false;
                //if (zap.turnRunSounds.Length != 0)
                //    zap.playSound(zap.turnRunSounds[Random.Range(0, zap.turnRunSounds.Length)]);
                break;

            //case Action.PrepareToJump:
            //    if (zap.faceRight()) zap.AnimatorBody.Play("Zap_jump_in_R");
            //    else zap.AnimatorBody.Play("Zap_jump_in_L");
            //    break;

            case Action.Jump:
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
                //if (zap.jumpSounds.Length != 0)
                //    zap.playSound(zap.jumpSounds[Random.Range(0, zap.jumpSounds.Length)]);
                break;

            case Action.JumpLeft:
            case Action.JumpRight:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_run_jump_fly_R");
                else zap.AnimatorBody.Play("Zap_run_jump_fly_L");

                //if (zap.jumpSounds.Length != 0)
                //    zap.playSound(zap.jumpSounds[Random.Range(0, zap.jumpSounds.Length)]);
                break;
                
            case Action.LandingHard:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_landing_hard_R");
                else zap.AnimatorBody.Play("Zap_landing_hard_L");

                //if (zap.landingSounds.Length != 0)
                //    zap.playSound(zap.landingSounds[Random.Range(0, zap.landingSounds.Length)]);

                //Debug.Log(zap.groundUnder.gameObject.name);

                if (zap.groundUnder)
                {
                    if (zap.particleSet)
                    {
                        ParticleData _pd = null;
                        Surface surface = zap.groundUnder.GetComponent<Surface>();
                        if (surface)
                        {
                            _pd = zap.particleSet.GetParticleData("landingHard*" + surface.type);

                            if (_pd != null)
                            {
                                ParticleInseter.Insert(_pd, transform.position);
                            }
                        }
                        else
                        {
                        }
                    }

                    //Surface surface = zap.groundUnder.GetComponent<Surface>();
                    //if (surface)
                    //{
                    //    if( surface.ZapLandingHardParticles.ParticlePrefab != null )
                    //    {
                    //        Object newParticleObject = Instantiate(surface.ZapLandingHardParticles.ParticlePrefab, transform.position, Quaternion.Euler(0, 0, 0));
                    //        Destroy(newParticleObject, surface.ZapLandingHardParticles.LifeTime);
                    //    }
                    //    ////print(acd.Type + " " + acd.Message + "*" + surface.type);
                    //    //if (!SoundPlayer.Play(gameObject, acd.Message + "*" + surface.type))
                    //    //{
                    //    //    //print("PROBUJE odtworzyc : " + acd.Message);
                    //    //    SoundPlayer.Play(gameObject, acd.Message);
                    //    //}
                    //}
                    ////else
                    ////{
                    ////    //SoundPlayer.Play(gameObject, acd.Message);
                    ////}
                }

                //if (landingHardParticles)
                //{
                //    Object newParticleObject = Instantiate(landingHardParticles, transform.position, Quaternion.Euler(0, 0, 0));
                //    Destroy(newParticleObject, 2.0f);
                //}

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

                //if (zap.catchSounds.Length != 0)
                //    zap.playSound(zap.catchSounds[Random.Range(0, zap.catchSounds.Length)]);
                break;
            case Action.CLIMB_CLIMB:
                //Debug.Log(catchedClimbHandle);
                //Debug.Log(lastCatchedClimbHandle);
                zap.TrySetIgnoreCollisionWhit(catchedClimbHandle);
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_jump_climb_R");
                else zap.AnimatorBody.Play("Zap_jump_climb_L");
                break;

            case Action.ClimbBelly:
                zap.TrySetIgnoreCollisionWhit(catchedClimbHandle);
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_jump_belly_climb_R");
                else zap.AnimatorBody.Play("Zap_jump_belly_climb_L");
                break;

            case Action.CLIMB_PULLDOWN:
                //Debug.Log(catchedClimbHandle);
                //Debug.Log(lastCatchedClimbHandle);
                zap.TrySetIgnoreCollisionWhit(catchedClimbHandle);
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_drop_R");
                else zap.AnimatorBody.Play("Zap_drop_L");
                break;

            case Action.MountIdle:
                zap.AnimatorBody.Play("Zap_climbmove_up");
                zap.AnimatorBody.speed = 0.0f;
                break;

            case Action.MOUNT_BIRDHIT:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_birdhit_R");
                else zap.AnimatorBody.Play("Zap_birdhit_L");
                break;

            case Action.MountLeft:
                zap.AnimatorBody.Play("Zap_climbmove_left");
                break;
            case Action.MountRight:
                zap.AnimatorBody.Play("Zap_climbmove_right");
                break;
            case Action.MountUp:
                zap.AnimatorBody.Play("Zap_climbmove_up");
                break;
            case Action.MountDown:
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

            case Action.PullUp:
                zap.AnimatorBody.Play("Zap_jump_climb_R");
                break;

            case Action.PullDown:
                zap.AnimatorBody.Play("Zap_drop_R");
                break;

            case Action.CrouchIn:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_crouch_in_R");
                else zap.AnimatorBody.Play("Zap_crouch_in_L");
                break;

            case Action.GetUp:
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_getup_R");
                else zap.AnimatorBody.Play("Zap_getup_L");
                break;

            case Action.CrouchIdle:
                //if (zap.faceRight()) zap.AnimatorBody.Play("Zap_crouch_move_R");
                //else zap.AnimatorBody.Play("Zap_crouch_move_L");
                //zap.AnimatorBody.speed = 0f;
                if (zap.faceRight()) zap.AnimatorBody.Play("Zap_crouch_idle_R");
                else zap.AnimatorBody.Play("Zap_crouch_idle_L");
                zap.AnimatorBody.speed = 0f;
                break;

            case Action.CrouchLeft:
                zap.AnimatorBody.Play("Zap_crouch_move_L");
                break;
            case Action.CrouchRight:
                zap.AnimatorBody.Play("Zap_crouch_move_R");
                break;

            case Action.CrouchLeftBack:
                zap.AnimatorBody.Play("Zap_crouch_move_back_R");
                break;

            case Action.CrouchRightBack:
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

            case Action.PUSHBACK_LEFT:
                zap.AnimatorBody.Play("Zap_pushback");
                break;

            case Action.PUSHBACK_RIGHT:
                zap.AnimatorBody.Play("Zap_pushback");
                break;

            case Action.PUSHBACKIN_LEFT:
                zap.AnimatorBody.Play("Zap_pushback_in");
                break;

            case Action.PUSHBACKIN_RIGHT:
                zap.AnimatorBody.Play("Zap_pushback_in");
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
        if (isInState(Zap.State.MOUNT) && isNotInAction(Action.MOUNT_BIRDHIT) )
        {
            if (cs && cs.HaveNextCrumbled() && !zap.stateJustChanged)
            {
                //Debug.Log("Crumble : keyUpDown");
                cs.Crumble();
                //cs = null;
            }
            else if (!mounting())
            {
                if (zap.climbingWallID == zap.layerIdMountMask)
                {
                    Vector3 newPos3 = new Vector3();
                    Vector3 distToMount = new Vector3(0f, 0f, 0f);
                    distToMount.y += 0.1f;
                 
                    if (CanMountTo(distToMount, ref newPos3))
                    {
                        zap.velocity.x = 0f;
                        zap.velocity.y = MountSpeed;
                        setAction(Action.MountUp);
                        return 1;
                    }
                }
                else if (zap.climbingWallID == zap.layerIdGroundFarMask)
                {
                    if (CanPullUp())
                    {
                        setAction(Action.PullUp);
                    }
                }
                else
                {
                    Debug.LogError("zap.climbingWallID : " + zap.climbingWallID);
                    Debug.Break();
                }
            }
        }
        else if (isInState(Zap.State.ON_GROUND))
        {
            if (/*zap.*/CheckHandle(zap.layerIdMountMask))
            {
                zap.velocity.x = 0.0f;
                zap.velocity.y = MountSpeed;
                setAction(Action.MountUp);
                zap.setState(Zap.State.MOUNT);
                zap.climbingWallID = zap.layerIdMountMask;
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
        if (zap.climbingWallID == zap.layerIdMountMask)
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
        }
        else if (zap.climbingWallID == zap.layerIdGroundFarMask)
        {
        }
        //else
        //{
        //    Debug.LogError("zap.climbingWallID : " + zap.climbingWallID);
        //    Debug.Break();
        //}
        
        return 0;
    }

    public override int keyDownDown()
    {
        if (isInState(Zap.State.MOUNT) && isNotInAction(Action.MOUNT_BIRDHIT) )
        {
            if (cs && cs.HaveNextCrumbled() && !zap.stateJustChanged)
            {
                //Debug.Log("Crumble : keyDownDown");
                cs.Crumble();
                //cs = null;
            }
            else if (zap.climbingWallID == zap.layerIdMountMask)
            {
                if (!mounting())
                {
                    Vector3 newPos3 = new Vector3();
                    Vector3 distToMount = new Vector3(0f, 0f, 0f);
                    distToMount.y -= 0.1f;

                    if (CanMountTo(distToMount, ref newPos3))
                    {
                        zap.velocity.x = 0f;
                        zap.velocity.y = -MountSpeed;
                        setAction(Action.MountDown);
                        return 1;
                    }

                    //Vector3 playerPos = transform.position;
                    //playerPos.y -= 0.1f;
                    //if (zap.CheckClimbingWall(playerPos, zap.layerIdMountMask))
                    //{
                    //    zap.velocity.x = 0.0f;
                    //    zap.velocity.y = -MountSpeed;
                    //    setAction(Action.MOUNT_DOWN);
                    //    return 1;
                    //}
                }
            }
            else if(zap.climbingWallID == zap.layerIdGroundFarMask)
            {

            }
            else
            {
                Debug.LogError("zap.climbingWallID : " + zap.climbingWallID);
                Debug.Break();
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
                    setAction(Action.CrouchIn);
                    return 1;
                }
            }
        }

        return 0;
    }

    public override int keyDownUp()
    {
        if( isInAction(Action.PullDown))
        {
            return 0;
        }

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
            if (crouching() || isInAction(Action.CrouchIn))
            {
                if (zap.canGetUp())
                {
                    setAction(Action.GetUp);
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

            case Action.WalkLeft:
                if (Input.GetKey(zap.keyLeft))
                {
                    desiredSpeedX = RunSpeed;
                    setAction(Action.RunLeft);
                }
                break;

            case Action.WalkRight:
                if (Input.GetKey(zap.keyRight))
                {
                    desiredSpeedX = RunSpeed;
                    setAction(Action.RunRight);
                }
                break;
        };

        return 0;
    }

    public override int keyRunUp()
    {

        switch (action)
        {

            case Action.RunLeft:
                if (Input.GetKey(zap.keyLeft))
                {
                    desiredSpeedX = WalkSpeed;
                    setAction(Action.WalkLeft);
                }
                else
                {
                    desiredSpeedX = 0.0f;
                }
                break;

            case Action.RunRight:
                if (Input.GetKey(zap.keyRight))
                {
                    desiredSpeedX = WalkSpeed;
                    setAction(Action.WalkRight);
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
        if ((isInAction(Action.Idle) || moving(-1) || jumping()) && isInState(Zap.State.ON_GROUND))
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
                        setAction(Action.CrouchLeft);
                        //resetActionAndState();
                        zap.velocity.x = -CrouchSpeed;
                        ActionCrouchLeftRight(-1);
                        wantGetUp = true;
                    }
                    else
                    {
                        PushStart(obstacle);
                    }
                    return 0;
                }
            }

            if (zap.dir() == -Vector2.right)
            {
                if (Input.GetKey(zap.keyRun))
                {
                    desiredSpeedX = RunSpeed;
                    speedLimiter(-1, desiredSpeedX + 1.0f);
                    setAction(Action.RunLeft);
                    return 1;
                }
                else
                {
                    desiredSpeedX = WalkSpeed;
                    speedLimiter(-1, desiredSpeedX + 1.0f);
                    setAction(Action.WalkLeft);
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
            if (cs && cs.HaveNextCrumbled() && !zap.stateJustChanged)
            {
                //Debug.Log("Crumble : keyLeftDown");
                cs.Crumble();
                //cs = null;
            }
            else if (!mounting())
            {
                Vector3 newPos3 = new Vector3();
                Vector3 distToMount = new Vector3(0f, 0f, 0f);
                distToMount.x -= 0.1f;
                zap.turnLeft();
                Assert.IsTrue(zap.climbingWallID > 0);

                if (CanMountTo(distToMount, ref newPos3))
                {
                    zap.velocity.x = -MountSpeed;
                    zap.velocity.y = 0.0f;
                    setAction(Action.MountLeft);
                    return 1;
                }
            }
            
            //if (!mounting())
            //{
            //    Vector3 playerPos = transform.position;
            //    playerPos.x -= 0.1f;
            //    zap.turnLeft();
            //    //&& zap.climbingWallID != zap.layerIdMountMask
            //    //zap.climbingWallID

            //    Assert.IsTrue(zap.climbingWallID > 0);

            //    if (zap.CheckClimbingWall(playerPos, zap.climbingWallID))
            //    {
            //        zap.velocity.x = -MountSpeed;
            //        zap.velocity.y = 0.0f;
            //        setAction(Action.MOUNT_LEFT);
            //        return 1;
            //    }
            //}
        }
        else if (isInAction(Action.CrouchIdle) && isInState(Zap.State.ON_GROUND))
        {               
            if (zap.dir() == -Vector2.right)
            {
                //float dto = -1;
                //Transform obstacle = zap.CheckLeft(0.1f, ref dto);
                //if (obstacle)
                //{
                //    //PushbackStart(obstacle);
                //    PushbackInStart(obstacle);
                //    return 0;
                //}
                //else
                //{
                    desiredSpeedX = CrouchSpeed;
                    setAction(Action.CrouchLeft);
                //}
            }
            else
            {
                float dto = -1;
                Transform obstacle = zap.CheckLeft(0.1f, ref dto);
                if (obstacle)
                {
                    PushbackInStart(obstacle);
                    return 0;
                }
                else
                {
                    desiredSpeedX = CrouchSpeed;
                    setAction(Action.CrouchLeftBack);
                }
            }
            return 1;
        }
        return 0;
    }

    public override int keyRightDown()
    {
        if ((isInAction(Action.Idle) || moving(1) || jumping()) && isInState(Zap.State.ON_GROUND))
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
                        setAction(Action.CrouchRight);
                        //resetActionAndState();
                        zap.velocity.x = CrouchSpeed;
                        ActionCrouchLeftRight(1);
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
                    setAction(Action.RunRight);
                    return 1;
                }
                else
                {
                    desiredSpeedX = WalkSpeed;
                    speedLimiter(1, desiredSpeedX + 1.0f);
                    setAction(Action.WalkRight);
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
            if (cs && cs.HaveNextCrumbled() && !zap.stateJustChanged )
            {
                //Debug.Log("Crumble : keyRightDown");
                cs.Crumble();
                //cs = null;
            }
            else if (!mounting())
            {
                Vector3 newPos3 = new Vector3();
                Vector3 distToMount = new Vector3(0f,0f,0f);
                distToMount.x += 0.1f;
                zap.turnRight();
                Assert.IsTrue(zap.climbingWallID > 0);

                if (CanMountTo(distToMount, ref newPos3))
                {
                    zap.velocity.x = MountSpeed;
                    zap.velocity.y = 0.0f;
                    setAction(Action.MountRight);
                    return 1;
                }
            }
        }
        else if (isInAction(Action.CrouchIdle) && isInState(Zap.State.ON_GROUND))
        {
            //if (zap.CheckRight(0.1f) >= 0.0f)
            //{
            //    return 0;
            //}
            //float dto = -1;
            //Transform obstacle = zap.CheckRight(0.1f, ref dto);
            //if (obstacle)
            //{
            //    PushbackInStart(obstacle);
            //    return 0;
            //}

            
            if (zap.dir() == Vector2.right)
            {
                desiredSpeedX = CrouchSpeed;
                setAction(Action.CrouchRight);
            }
            else
            {
                float dto = -1;
                Transform obstacle = zap.CheckRight(0.1f, ref dto);
                if (obstacle)
                {
                    PushbackInStart(obstacle);
                    return 0;
                }
                else
                {
                    desiredSpeedX = CrouchSpeed;
                    setAction(Action.CrouchRightBack);
                }
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
                else if (isInAction(Action.PULL_LEFT))
                {
                    PullStop();
                }
                else if (isInAction(Action.PUSHBACK_LEFT))
                {
                    if (Input.GetKey(zap.keyDown))
                    {
                        setActionCrouchIdle();
                    }
                    else
                    {
                        if (zap.canGetUp()) setActionIdle();
                        else setActionCrouchIdle();
                    }
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
                else if (isInAction(Action.PUSHBACK_RIGHT))
                {
                    if (Input.GetKey(zap.keyDown))
                    {
                        setActionCrouchIdle();
                    }
                    else
                    {
                        if (zap.canGetUp()) setActionIdle();
                        else setActionCrouchIdle();
                    }
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
        if (!zap.TryJump()) return false;

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

    //float getGroundUnderAngle()
    //{

    //    return 0;
    //}
    // <0;-1> w lewo <0;1> w prawo 0 nie...
    float shouldSlideDown()
    {
        if (zap.groundUnder == null) return 0f;
        int groundUnderAngleQuarter = (int)zap.groundUnder.eulerAngles.z / 90;
        //if (groundUnderAngle < -60.0f || groundUnderAngle > 60.0f)
        //  return true;


        return 0;
    }

    void JumpOutMountMoveable()
    {
        Vector3 handPos = zap.sensorLeft3.position; // + 0.3f;
        handPos.x += zap.getMyHalfWidth();
        handPos.y += 0.3f;

        handledMountMoveable.JumpedOut(handPos);
        handledMountMoveable = null;
    }

    public override int keyJumpDown()
    {

        //Debug.Log ("ZapControllerNormal::keyJumpDown()");
        //jumpKeyPressed = true;

        if (crouching())
        {
            if (CanPullDown())
            {
                zap.velocity.x = 0.0f;
                zap.velocity.y = 0.0f;
                zap.setState(Zap.State.MOUNT);
                setAction(Action.PullDown);
                return 0;
            }
        }

        switch (action)
        {
            case Action.Idle:
                if (isInState(Zap.State.ON_GROUND))
                {
                    if (canJump())
                    {
                        //void preparetojump()
                        //{
                        if (isNotInState(Zap.State.ON_GROUND) || isNotInAction(Action.Idle))
                        {
                            break;
                        }
                        else
                        {
                            zap.velocity.x = 0.0f;
                            zap.velocity.y = 0.0f;
                            jump();
                        }
                        //    zap.velocity.x = 0.0f;
                        //    zap.velocity.y = 0.0f;
                        //    setAction(Action.PrepareToJump);
                        //}
                    }
                }
                break;

            case Action.WalkLeft:
                if (canJump())
                    JumpWalkLeft();
                break;
            case Action.WalkRight:
                if (canJump())
                    JumpWalkRight();
                break;

            case Action.RunLeft:
                if (canJump())
                    JumpRunLeft();
                break;
            case Action.RunRight:
                if (canJump())
                    JumpRunRight();
                break;

            case Action.MountIdle:
            case Action.MountUp:
            case Action.MountDown:
                if( handledMountMoveable )
                {
                    //Vector3 handPos = zap.sensorLeft3.position; // + 0.3f;
                    //handPos.x += zap.getMyHalfWidth();
                    //handPos.y += 0.3f;

                    //handledMountMoveable.JumpedOut(handPos);
                    //handledMountMoveable = null;
                    JumpOutMountMoveable();
                }

                zap.climbingWallID = -1;
                lastFrameHande = false;
                mountJumpStartPos = transform.position;
                jumpFromMount = true;
                justJumpedMount = true;
                if( cs )
                {
                    Debug.Log("Crumble : keyJumpDown");
                    //cs.ZapJumpOff();
                    cs.Crumble();
                    cs = null;
                }

                if (Input.GetKey(zap.keyLeft))
                {
                    JumpWalkLeft();
                    return 0;
                }

                if (Input.GetKey(zap.keyRight))
                {
                    JumpWalkRight();
                    return 0;
                }

                zap.velocity.x = 0.0f;
                zap.velocity.y = 0.0f;
                setAction(Action.Jump);
                zap.setState(Zap.State.IN_AIR);
                break;

            case Action.MountLeft:
                if (handledMountMoveable)
                {
                    //Vector3 handPos = zap.sensorLeft3.position; // + 0.3f;
                    //handPos.x += zap.getMyHalfWidth();
                    //handPos.y += 0.3f;

                    //handledMountMoveable.JumpedOut(handPos);
                    //handledMountMoveable = null;

                    JumpOutMountMoveable();
                }
                zap.climbingWallID = -1;
                mountJumpStartPos = transform.position;
                jumpFromMount = true;
                justJumpedMount = true;
                JumpWalkLeft();
                break;

            case Action.MountRight:
                if (handledMountMoveable)
                {
                    //Vector3 handPos = zap.sensorLeft3.position; // + 0.3f;
                    //handPos.x += zap.getMyHalfWidth();
                    //handPos.y += 0.3f;

                    //handledMountMoveable.JumpedOut(handPos);
                    //handledMountMoveable = null;

                    JumpOutMountMoveable();
                }
                zap.climbingWallID = -1;
                mountJumpStartPos = transform.position;
                jumpFromMount = true;
                justJumpedMount = true;
                JumpWalkRight();
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
            setAction(Action.Idle);
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

    int ActionRun(int dir)
    {
        //Debug.Log(zap.velocity.x);
        Debug.DrawLine(transform.position, transform.position + new Vector3(1f, 5f, 0f));
        if (Input.GetMouseButtonDown(0))
        {
            int pullRes = zap.pullChoosenWeapon();
            if (pullRes != 0)
                return pullRes;
        }

        bool speedReached = checkSpeed(dir);
        if (speedReached && desiredSpeedX == 0.0f)
        {
            setAction(Action.Idle);
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
                    setAction(Action.TurnRunLeft);
                }

            }
            else if (dir == -1)
            {

                if ((Input.GetKeyDown(zap.keyRight) || Input.GetKey(zap.keyRight)) &&
                   (Input.GetKeyUp(zap.keyLeft) || !Input.GetKey(zap.keyLeft))
                   )
                {
                    setAction(Action.TurnRunRight);
                }
            }
        }

        distToMove = zap.velocity.x * zap.getCurrentDeltaTime();

        zap.AnimatorBody.speed = 0.5f + (Mathf.Abs(zap.velocity.x) / RunSpeed) * 0.5f;

        float distToObstacle = 0.0f;
        //if (zap.CheckObstacle(dir, distToMove, ref distToObstacle))
        Transform obstacle = zap.CheckObstacle(dir, distToMove, ref distToObstacle);
        if (obstacle)
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

    int ActionTurnRun(int dir)
    {

        int retVal = 0;

        if (Input.GetKeyDown(zap.keyJump))
        {
            wantJumpAfter = true;
        }

        //Debug.Log(desiredSpeedX);

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

    int ActionCrouchIn()
    {
        if (zap.currentActionTime >= CrouchInOutDuration)
        {
            crouch();
        }
        return 0;
    }

    int ActionGetUp()
    {
        if (zap.currentActionTime >= CrouchInOutDuration)
        {
            getUp();
        }

        return 0;
    }

    int ActionCrouchIdle()
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

    int ActionPullUp()
    {
        float actionRatio = Mathf.Min(zap.currentActionTime / PullUpDuration, 1f);

        Vector3 posDiff = new Vector3();

        if (actionRatio >= 1f)
        {
            posDiff.y += (zap.sensorLeft3.localPosition.y + 0.35f);
            transform.position = actionChangedPos + posDiff;

            zap.setState(Zap.State.ON_GROUND);
            setAction(Action.Idle);
            
            return 1;
        }
        
        posDiff.y += (actionRatio * zap.sensorLeft3.localPosition.y + 0.35f);
        transform.position = actionChangedPos + posDiff;

        return 0;
    }
    int ActionPullDown()
    {
        float actionRatio = Mathf.Min(zap.currentActionTime / PullDownDuration, 1f);

        Vector3 posDiff = new Vector3();

        if (actionRatio >= 1f)
        {
            posDiff.y -= (zap.sensorLeft3.localPosition.y + 0.35f);
            transform.position = actionChangedPos + posDiff;

            zap.climbingWallID = zap.layerIdGroundFarMask;
            setAction(Action.MountIdle);
            Assert.IsTrue(/*zap.*/CheckHandle(zap.layerIdGroundFarMask));
            return 1;
        }
        
        posDiff.y -= (actionRatio * zap.sensorLeft3.localPosition.y + 0.35f);
        transform.position = actionChangedPos + posDiff;
        
        return 0;
    }

    int _Action_WALK(int dir)
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
            setAction(Action.Idle);
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

    int ActionCrouchLeftRight(int dir)
    {
        if (Input.GetKey(zap.keyDown))
        {
            if (tryStartClimbPullDown())
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

        //float distToObstacle = 0.0f;
        //Transform obstacle = zap.CheckObstacle(dir, distToMove, ref distToObstacle);
        //if (obstacle)
        //{
        //    Transform obstacle2 = zap.CheckObstacle(dir, distToMove, ref distToObstacle, true);
        //    if (!obstacle2)
        //    {
        //        setActionCrouchIdle();
        //        resetActionAndState();
        //        wantGetUp = true;
        //    }
        //    else
        //    {
        //        distToMove = distToObstacle;
        //        //setActionIdle();
        //        PushStart(obstacle);
        //    }
        //}

        float distToObstacle = 0.0f;
        Transform obstacle = zap.CheckObstacle(dir, distToMove, ref distToObstacle);
        if (obstacle)
        {
            //Debug.Log("obstacle -> " + distToObstacle);
            distToMove = distToObstacle;
            //setActionCrouchIdle();
            if (dir == 1 && isInAction(Action.CrouchRightBack))
            {
                //Debug.Log(action);
                PushbackInStart(obstacle);
            }
            else if (dir == -1 && isInAction(Action.CrouchLeftBack))
            {
                //Debug.Log(action);
                PushbackInStart(obstacle);
            }
            else
            {
                setActionCrouchIdle();
            }
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
        if (isNotInAction(Action.MOUNT_BIRDHIT) && zap.HaveKnife && Input.GetMouseButtonDown(0))
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
        if (zap.currentActionTime > BirdHitDuration)
        {
            if (zap.canBeFuddleFromBird || lastActionParam == 1) // 1 oznacza ze bat to byl...
                zap.FuddleFromBird = true;

            zap.velocity.x = 0.0f;
            zap.velocity.y = 0.0f;
            setAction(Action.Jump);
            zap.setState(Zap.State.IN_AIR);
        }
        return 0;
    }

    bool CanMountTo(Vector3 distToMount, ref Vector3 newPos3)
    {
        if (handledMountMoveable)
        {
            if (distToMount.x != 0f && !handledMountMoveable.MovingXEnabled)
            {
                return false;
            }
            if (distToMount.y != 0f && !handledMountMoveable.MovingYEnabled)
            {
                return false;
            }

            if (handledMountMoveable.MovingInLocal)
            {
                Vector3 pointToCheck = handledMountMoveable.ConvertToPointSize(handledMountMoveablePosition);
                pointToCheck += distToMount;

                //Debug.Log(handledMountMoveablePosition + " " + pointToCheck + " " + distToMount );

                bool res = handledMountMoveable.LocalPointHandable(pointToCheck);
                if (!res)
                {
                    //Debug.Log("LocalPointHandable - false");
                    return false;
                }
                newPos3 = transform.position;

                Vector2 tmp = distToMount;
                tmp = tmp.Rotate(handledMountMoveable.transform.eulerAngles.z);
                distToMount.x = tmp.x;
                distToMount.y = tmp.y;

                newPos3 += distToMount;

                return true;
            }
        }
        else
        {

        }

        newPos3 = transform.position;
        newPos3 += distToMount;
        if (zap.CheckClimbingWall(newPos3, zap.climbingWallID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    int ActionMounting()
    {
        if (tryMountAttackStart())
        {
            return 0;
        }
        
        if( handledMountMoveable )
        {
            Vector3 newPos3 = new Vector3();
            Vector3 distToMount = zap.velocity * zap.getCurrentDeltaTime();
            if( CanMountTo(distToMount, ref newPos3) )
            {
                transform.position = newPos3;

                Vector3 zapHandpos = transform.position;
                zapHandpos.y += (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                handledMountMoveablePosition = handledMountMoveable.transform.InverseTransformPoint(zapHandpos);
            }
            else
            {
                setMountIdle();
            }
        }
        else
        {
            Vector3 newPos3 = transform.position;
            Vector3 distToMount = zap.velocity * zap.getCurrentDeltaTime();
            newPos3 += distToMount;
            if (zap.CheckClimbingWall(newPos3, zap.climbingWallID))
            {
                transform.position = newPos3;
            }
            else
            {
                setMountIdle();
            }
        }
        
        return 0;
    }

    int ActionMountingDown()
    {
        if (tryMountAttackStart())
        {
            return 0;
        }

        Vector3 distToMount = zap.velocity * zap.getCurrentDeltaTime();
        Vector3 newPos3 = transform.position;

        if (handledMountMoveable)
        {
            //newPos3 = new Vector3();
            if (CanMountTo(distToMount, ref newPos3))
            {
                transform.position = newPos3;

                Vector3 zapHandpos = transform.position;
                zapHandpos.y += (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                handledMountMoveablePosition = handledMountMoveable.transform.InverseTransformPoint(zapHandpos);
            }
            else
            {
                Debug.Log("ActionMountingDown => setMountIdle()");
                setMountIdle();
            }
        }
        else
        {
            newPos3 = transform.position;
            newPos3 += distToMount;
            if (zap.CheckClimbingWall(newPos3, zap.climbingWallID))
            {
                transform.position = newPos3;
            }
            else
            {
                setMountIdle();
            }
        }

        //Vector3 newPos3 = transform.position;
        //Vector3 distToMount = zap.velocity * zap.getCurrentDeltaTime();
        //newPos3 += distToMount;

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
                    setAction(Action.Idle);
                    transform.position = transform.position + distToMount;
                }
            }
            else 
            {
                if (!handledMountMoveable)
                {
                    if (zap.CheckClimbingWall(newPos3, zap.climbingWallID)) transform.position = newPos3;
                    else setMountIdle();
                }
            }
        }
        return 0;
    }

    int Action_MOUNT_ATTACK()
    {
        if (zap.currentActionTime >= MountAttackDuration)
        {
            zap.MountAttackLeftCollider.SetActive(false);
            zap.MountAttackRightCollider.SetActive(false);

            if (isInState(Zap.State.MOUNT))
            {
                setAction(Action.MountIdle);
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
        if (zap.currentActionTime >= LandingHardDuration)
        {
            if (zap.beforeFallController == null)
            {
                setAction(Action.Idle);
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
        if (zap.currentActionTime >= ClimbDurClimb)
        {
            zap.removeLastIgnoredCollision();
            setAction(Action.CLIMB_CATCH, 10);
            zap.setState(Zap.State.CLIMB);
            canPullUp = true;
            transform.position = climbAfterPos;
        }
        else
        {
            float ratio = zap.currentActionTime / ClimbDurClimb;
            transform.position = climbBeforePos + climbDistToClimb * ratio;
        }

        return 0;
    }

    CrumblingStairs cs = null;

    void CatchCrumblingStairs()
    {
        cs = catchedClimbHandle.transform.parent.parent.GetComponent<CrumblingStairs>();
        if (!cs)
        {
            Debug.LogError("Nie ma CrumblingStairs");
            Debug.Break();
        }

        if (cs.SoundTagCatch != "") SoundPlayer.Play(cs.gameObject, cs.SoundTagCatch);

        RLHScene.Instance.CamController.ShakeImpulseStart(3f, 0.25f, 8f);

        handledMountMoveable = cs.Mount.GetComponent<MountMoveable>();
        if (!handledMountMoveable)
        {
            Debug.LogError("Nie ma CrumblingStairs handledMountMoveable");
            Debug.Break();
        }

        Vector3 newZapPos = cs.MountHandle.position;
        newZapPos.y -= (zap.sensorLeft3.transform.localPosition.y + 0.3f);
        zap.transform.position = newZapPos;

        Vector3 zapHandpos = zap.transform.position;
        zapHandpos.y += (zap.sensorLeft3.transform.localPosition.y + 0.3f);
        handledMountMoveablePosition = handledMountMoveable.transform.InverseTransformPoint(zapHandpos);

        zap.climbingWallID = zap.layerIdMountMask;
        setActionMountIdle();

        cs.Crumble();
    }

    int Action_CLIMB_JUMP_TO_CATCH()
    {
        // dociaganie do punktu:
        if (zap.currentActionTime >= climbToJumpDuration)
        {
            if (catchedClimbHandle.tag == "CrumblingStairs")
            {
                //cs = catchedClimbHandle.transform.parent.parent.GetComponent<CrumblingStairs>();
                //if(!cs)
                //{
                //    Debug.LogError("Nie ma CrumblingStairs");
                //    Debug.Break();
                //}

                //RLHScene.Instance.CamController.ShakeImpulseStart(3f, 0.25f, 8f);

                //handledMountMoveable = cs.Mount.GetComponent<MountMoveable>();
                //if (!handledMountMoveable)
                //{
                //    Debug.LogError("Nie ma CrumblingStairs handledMountMoveable");
                //    Debug.Break();
                //}

                //Vector3 newZapPos = cs.MountHandle.position;
                //newZapPos.y -= (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                //zap.transform.position = newZapPos;

                //Vector3 zapHandpos = zap.transform.position;
                //zapHandpos.y += (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                //handledMountMoveablePosition = handledMountMoveable.transform.InverseTransformPoint(zapHandpos);

                //zap.climbingWallID = zap.layerIdMountMask;
                //setActionMountIdle();

                //cs.Crumble();

                CatchCrumblingStairs();
            }
            else
            {
                setAction(Action.CLIMB_CATCH, lastActionParam);
                transform.position = climbAfterPos;
            }
        }
        else
        {
            float ratio = zap.currentActionTime / climbToJumpDuration;
            transform.position = climbBeforePos + climbDistToClimb * ratio;
        }

        return 0;
    }

    int ActionClimbCatch()
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
        zap.touchStone(catchedClimbHandle.transform, climbAfterPos2);

        if( ShouldFallFromPlatform() )
        {
            FallFromPlatform();
            return 0;
        }

        if ((Input.GetKeyDown(zap.keyUp) || Input.GetKey(zap.keyUp))) // && canPullUp)
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
                JumpWalkLeft(true);
                catchedClimbHandle = null;
                lastCatchedClimbHandle = null;
            }
            else if (Input.GetKey(zap.keyRight))
            {
                zap.turnRight();
                JumpWalkRight(true);
                catchedClimbHandle = null;
                lastCatchedClimbHandle = null;
            }
            else if (Input.GetKey(zap.keyDown))
            {
                //zap.velocity.x = 0.0f;
                //zap.velocity.y = 0.0f;
                //zap.setState(Zap.State.IN_AIR);
                //setAction(Action.Jump);
                //lastCatchedClimbHandle = catchedClimbHandle;
                //catchedClimbHandle = null;
                FallFromPlatform();
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

    bool ShouldFallFromPlatform()
    {
        if (!catchedClimbHandle) return false;
        PlatformHandle platformHandle = catchedClimbHandle.GetComponent<PlatformHandle>();
        if (!platformHandle) return false;
        if (!platformHandle.CantClimb) return false;

        if (zap.CurrentStateTime > platformHandle.MaxHangTime) return true;

        if (Input.anyKeyDown) return true;
        return false;
    }

    //bool AnyKeyPressed()
    //{
    //    //return 
    //}

    void FallFromPlatform()
    {
        zap.velocity.x = 0.0f;
        zap.velocity.y = 0.0f;
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.Jump);
        lastCatchedClimbHandle = catchedClimbHandle;
        catchedClimbHandle = null;
    }

    int ActionClimbBelly()
    {
        if (zap.currentActionTime >= ClimbDurClimb)
        {
            zap.removeLastIgnoredCollision();

            zap.setState(Zap.State.ON_GROUND);
            transform.position = climbAfterPos2;

            if (zap.canGetUp())
            {
                setAction(Action.Idle);
                resetActionAndState();
            }
            else
            {
                setAction(Action.CrouchIdle);
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
            float ratio = zap.currentActionTime / ClimbDurClimb;
            transform.position = climbBeforePos + climbDistToClimb * ratio;
        }

        return 0;
    }

    int Action_CLIMB_CLIMB()
    {

        if (zap.currentActionTime >= ClimbDurClimb)
        {
            zap.removeLastIgnoredCollision();

            zap.setState(Zap.State.ON_GROUND);
            transform.position = climbAfterPos2;

            if (zap.canGetUp())
            {
                setAction(Action.Idle);
                resetActionAndState();
            }
            else
            {
                setAction(Action.CrouchIdle);
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
            float ratio = zap.currentActionTime / ClimbDurClimb;
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
                if (zap.faceRight())
                {
                    if (Input.GetKeyDown(zap.keyLeft) || Input.GetKey(zap.keyLeft))
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
                Transform obstacle = zap.CheckObstacle(zap.dir2(), distToMove + 0.1f * zap.dir2(), ref distToObstacle);
                if (obstacle != pushPullObstacle)
                {
                    setActionIdle();
                    return 1;
                }

                //Debug.Log("Push : " + distToMove + " " + distToObstacle);

                if (Mathf.Abs(distToMove) < Mathf.Abs(distToObstacle))
                {
                    zap.velocity.x = distToMove / deltaTime;
                    //zap.velocity.x = 0.0f;
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

                if (Mathf.Abs(zap.velocity.x) < 0.01f)
                {
                    zap.AnimatorBody.speed = 0.0f;
                }
                else
                {
                    zap.AnimatorBody.speed = 0.25f + (Mathf.Abs(zap.velocity.x) / PushMaxSpeed) * 0.75f;
                    //zap.AnimatorBody.speed = Mathf.Min(Mathf.Abs(zap.velocity.x) / PushMaxSpeed, 1f);
                }

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
            else
            {
                zap.AnimatorBody.speed = 0.0f;
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
                float _diffx = obstacleBody.position.x - lastPulledObstaclePos.x;
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

    int ActionPushbackIn(float deltaTime)
    {
        if (zap.currentActionTime >= 0.09f/*PushbackInDuration*/)
        {
            //Debug.Log("ActionPushbackIn : " + zap.currentActionTime );
            PushbackStart(pushPullObstacle);
        }
        return 0;
    }

    int ActionPushback(float deltaTime)
    {
        

        if (pushPullObstacle)
        {
            Rigidbody2D obstacleBody = pushPullObstacle.GetComponent<Rigidbody2D>();
            if (obstacleBody)
            {
                //if (zap.faceRight())
                //{
                //    if (Input.GetKeyDown(zap.keyLeft) || Input.GetKey(zap.keyLeft))
                //    {
                //        obstacleBody.velocity = new Vector2(0f, obstacleBody.velocity.y);
                //        lastPulledObstaclePos = obstacleBody.position;
                //        setAction(Action.PULL_LEFT);
                //        return 1;
                //    }
                //}
                //else
                //{
                //    if (Input.GetKeyDown(zap.keyRight) || Input.GetKey(zap.keyRight))
                //    {
                //        obstacleBody.velocity = new Vector2(0f, obstacleBody.velocity.y);
                //        lastPulledObstaclePos = obstacleBody.position;
                //        setAction(Action.PULL_RIGHT);
                //        return 1;
                //    }
                //}

                zap.velocity.x = PushbackMaxSpeed * zap.dir2N();
                distToMove = zap.velocity.x * deltaTime;

                float distToObstacle = 0.0f;
                Transform obstacle = zap.CheckObstacle(zap.dir2N(), distToMove + 0.1f * zap.dir2N(), ref distToObstacle);
                if (obstacle != pushPullObstacle)
                {
                    setActionCrouchIdle();
                    return 1;
                }

                //Debug.Log("Push : " + distToMove + " " + distToObstacle);

                if (Mathf.Abs(distToMove) < Mathf.Abs(distToObstacle))
                {
                    zap.velocity.x = distToMove / deltaTime;
                    //zap.velocity.x = 0.0f;
                }
                else
                {
                    distToMove = distToObstacle;
                    zap.velocity.x = distToMove / deltaTime;


                    Vector2 force = new Vector2(0f, 0f);
                    Vector2 forcePos;
                    if (zap.faceRight())
                    {
                        force.x = -pushForce /** deltaTime*/;
                        forcePos = zap.sensorLeft2.position;
                        forcePos.x -= 0.4f;
                        forcePos.y -= 0.3f;
                    }
                    else
                    {
                        force.x = pushForce /** deltaTime*/;
                        forcePos = zap.sensorRight2.position;
                        forcePos.x += 0.4f;
                        forcePos.y -= 0.3f;
                    }
                    obstacleBody.AddForceAtPosition(force, forcePos, ForceMode2D.Force);
                }

                //distToMove = Mathf.Min(distToMove,distToObstacle);
                //zap.velocity.x = distToMove / deltaTime;

                //zap.velocity.x = Mathf.Min(Mathf.Abs(zap.velocity.x)) * zap.dir2();

                if (Mathf.Abs(zap.velocity.x) < 0.01f)
                {
                    zap.AnimatorBody.speed = 0.0f;
                }
                else
                {
                    zap.AnimatorBody.speed = 0.25f + (Mathf.Abs(zap.velocity.x) / PushMaxSpeed) * 0.75f;
                    //zap.AnimatorBody.speed = Mathf.Min(Mathf.Abs(zap.velocity.x) / PushMaxSpeed, 1f);
                }

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
            else
            {
                zap.AnimatorBody.speed = 0.0f;
            }
        }

        return 0;
    }

    void PushbackInStart(Transform obstacle)
    {
        //Debug.Log("PushbackInStart : " + obstacle);
        zap.velocity.x = 0.0f;
        pushPullObstacle = obstacle;
        if (zap.faceRight())
            setAction(Action.PUSHBACKIN_LEFT);
        else
            setAction(Action.PUSHBACKIN_RIGHT);

    }
    void PushbackStart(Transform obstacle)
    {
        //Debug.Log("PushStart : " + obstacle);
        zap.velocity.x = 0.0f;
        pushPullObstacle = obstacle;
        if (zap.faceRight())
            setAction(Action.PUSHBACK_LEFT);
        else
            setAction(Action.PUSHBACK_RIGHT);

        //Debug.Log(action);
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
                    if (newCatchedRopeLink && catchedRope.chooseDriver(newCatchedRopeLink.transform))
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

                case Action.Idle:
                case Action.Jump:
                case Action.CrouchIn:
                    setAction(Action.CrouchIdle);
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

                case Action.WalkLeft:
                case Action.RunLeft:
                case Action.JumpLeft:
                    if (Input.GetKey(zap.keyLeft))
                    {
                        zap.velocity.x = 0.0f;
                        desiredSpeedX = CrouchSpeed;
                        if (zap.dir() == -Vector2.right)
                        {
                            setAction(Action.CrouchLeft);
                        }
                        else
                        {
                            setAction(Action.CrouchLeftBack);
                        }
                    }
                    else
                    {
                        zap.velocity.x = 0.0f;
                        zap.velocity.y = 0.0f;
                        setAction(Action.CrouchIdle);
                    }
                    break;

                case Action.WalkRight:
                case Action.RunRight:
                case Action.JumpRight:
                    if (Input.GetKey(zap.keyRight))
                    {
                        zap.velocity.x = 0.0f;
                        desiredSpeedX = CrouchSpeed;
                        if (zap.dir() == Vector2.right)
                        {
                            setAction(Action.CrouchRight);
                        }
                        else
                        {
                            setAction(Action.CrouchRightBack);
                        }
                    }
                    else
                    {
                        zap.velocity.x = 0.0f;
                        zap.velocity.y = 0.0f;
                        setAction(Action.CrouchIdle);
                    }
                    break;
            }

        }
    }

    //void preparetojump()
    //{
    //    if (isNotInState(Zap.State.ON_GROUND) || isNotInAction(Action.Idle))
    //        return;

    //    zap.velocity.x = 0.0f;
    //    zap.velocity.y = 0.0f;
    //    setAction(Action.PrepareToJump);
    //}

    void jump()
    {
        zap.AddImpulse(new Vector2(0.0f, JumpWalkImpulse));
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.Jump);

        lastFrameHande = false;
    }

    void jumpFromClimb()
    {
        //zap.AddImpulse(new Vector2(0.0f, JumpImpulse));
        zap.AddImpulse(new Vector2(0.0f, JumpFromClimbImpulse));
        zap.setState(Zap.State.IN_AIR);
        setAction(Action.Jump, 1);
        lastFrameHande = false;
    }

    void JumpWalkLeft(bool fromClimb = false)
    {
        JumpLeftRight(-1, fromClimb);
    }

    void JumpWalkRight(bool fromClimb = false)
    {
        JumpLeftRight(1, fromClimb);
    }

    float jumpStartVelocity = 0f;
    float CurrentJumpRunMaxSpeed = 0f;

    void JumpLeftRight(int jumpDir, bool fromClimb)
    {
        zap.velocity.y = 0.0f;
        if (fromClimb)
        {
            zap.velocity.x = jumpDir * JumpFromClimbSpeed;
            zap.AddImpulse(new Vector2(0.0f, JumpFromClimbSideImpulse));
        }
        else
        {
            zap.velocity.x = jumpDir * JumpWalkSpeed;
            zap.AddImpulse(new Vector2(0.0f, JumpWalkImpulse));
        }
        jumpStartVelocity = zap.velocity.x;
        CurrentJumpRunMaxSpeed = JumpRunMaxSpeed;
        zap.setState(Zap.State.IN_AIR);
        setAction(jumpDir == 1 ? Action.JumpRight : Action.JumpLeft, fromClimb ? 1 : 0);
        //CurrentJumpMaxSpeed = JumpWalkSpeed;

        lastFrameHande = false;
    }

    void JumpRunLeft()
    {
        JumpRunLeftRight(-1);
    }

    void JumpRunRight()
    {
        JumpRunLeftRight(1);
    }

    void JumpRunLeftRight(int jumpDir)
    {
        //zap.velocity.x = jumpDir * JumpRunMaxSpeed;
        float runSpeedRatio = Mathf.Abs( zap.velocity.x / RunSpeed );
        float jumpSpeedDiff = JumpRunMaxSpeed - JumpWalkSpeed;
        //(1f / JumpRunMaxThreshold) +
        float fromRunSpeedBonus = Mathf.Min(JumpRunMaxSpeed, (1f / JumpRunMaxThreshold) * runSpeedRatio * jumpSpeedDiff );

        zap.velocity.x = jumpDir * (JumpWalkSpeed + fromRunSpeedBonus);
        CurrentJumpRunMaxSpeed = JumpRunMaxSpeed;
        jumpStartVelocity = zap.velocity.x;
        zap.velocity.y = 0.0f;
        zap.AddImpulse(new Vector2(0.0f, JumpRunImpulse));
        zap.setState(Zap.State.IN_AIR);
        setAction(jumpDir == 1 ? Action.JumpRight : Action.JumpLeft);
        //CurrentJumpMaxSpeed = JumpRunMaxSpeed;

        lastFrameHande = false;
    }

    void turnLeftStart()
    {
        setAction(Action.TurnStandLeft);

        if (Input.GetKeyDown(zap.keyJump) || (Input.GetKey(zap.keyJump) && canJumpAfter))
            wantJumpAfter = true;
    }

    void turnRightStart()
    {
        setAction(Action.TurnStandRight);

        if (Input.GetKeyDown(zap.keyJump) || (Input.GetKey(zap.keyJump) && canJumpAfter))
            wantJumpAfter = true;
    }

    void turnLeftFinish()
    {
        setAction(Action.Idle);

        if (wantJumpAfter)
        {
            JumpWalkLeft();

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
        setAction(Action.Idle);

        if (wantJumpAfter)
        {
            JumpWalkRight();

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
        setAction(Action.Idle);
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
        setAction(Action.CrouchIdle);
    }
    void setActionMountIdle()
    {
        zap.velocity.x = 0.0f;
        zap.velocity.y = 0.0f;
        zap.setState(Zap.State.MOUNT);
        setAction(Action.MountIdle);
        resetActionAndState();
    }
    bool setMountIdle()
    {
        if (isInState(Zap.State.MOUNT) && isNotInAction(Action.MOUNT_BIRDHIT) && isNotInAction(Action.MOUNT_ATTACK_LEFT) && isNotInAction(Action.MOUNT_ATTACK_RIGHT))
        {
            zap.velocity.x = 0.0f;
            zap.velocity.y = 0.0f;
            setAction(Action.MountIdle);

            return true;
        }
        return false;
    }
    //void SetActionHangIdle()
    //{
    //    zap.velocity.x = 0.0f;
    //    zap.velocity.y = 0.0f;
    //    zap.setState(Zap.State.HANG);
    //    setAction(Action.HANG_IDLE);
    //    resetActionAndState();
    //}

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
        if (isInAction(Action.WalkRight))
            return 1;
        if (isInAction(Action.WalkLeft))
            return -1;
        return 0;
    }

    int running()
    {
        if (isInAction(Action.RunRight))
            return 1;
        if (isInAction(Action.RunLeft))
            return -1;
        return 0;
    }

    int TurningRun()
    {
        if (isInAction(Action.TurnRunRight))
            return 1;
        if (isInAction(Action.TurnRunLeft))
            return -1;
        return 0;
    }

    bool moving(Vector2 dir)
    {
        if (dir == Vector2.right)
            return isInAction(Action.WalkRight) || isInAction(Action.RunRight);
        else
            return isInAction(Action.WalkLeft) || isInAction(Action.RunLeft);
    }

    bool moving(int dir)
    {
        if (dir == 1)
            return isInAction(Action.WalkRight) || isInAction(Action.RunRight);
        else
            return isInAction(Action.WalkLeft) || isInAction(Action.RunLeft);
    }

    bool jumping()
    {
        return isInAction(Action.Jump) || isInAction(Action.JumpLeft) || isInAction(Action.JumpRight);
    }
    
    bool mounting()
    {
        return isInAction(Action.MountLeft) || isInAction(Action.MountRight)
            || isInAction(Action.MountUp) || isInAction(Action.MountDown)
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
        return isInAction(Action.CrouchIdle) ||
            isInAction(Action.CrouchLeft) || isInAction(Action.CrouchLeftBack) ||
                isInAction(Action.CrouchRight) || isInAction(Action.CrouchRightBack);
    }
    
    public override void zapDie(Zap.DeathType deathType)
    {
        if (zap.isInState(Zap.State.CLIMB_ROPE))
        {
            releaseRope();
            catchedRope = null;
            justJumpedRope = null;
        }
        setAction(Action.Die, (int)deathType);
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
                setAction(Action.Jump);

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
                setAction(Action.Jump);
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
        //Debug.Log("" + speedX);

        if (speedX < desiredSpeedX)
        { // trzeba przyspieszyc

            float speedUpParam = 0f;
            if (walking() != 0)
                speedUpParam = WalkSpeedUpParam;
            else if (running() != 0)
                speedUpParam = RunSpeedUpParam;
            else if (crouching())
                speedUpParam = CrouchSpeedUpParam;
            else if (TurningRun() != 0)
                speedUpParam = 0f; // CrouchSpeedUpParam;
            else
                Debug.LogError("checkSpeed");

            float velocityDamp = speedUpParam * zap.getCurrentDeltaTime();
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
            
            float slowDownParam = 0f;
            if (walking() != 0)
                slowDownParam = WalkSlowDownParam;
            else if (running() != 0)
                slowDownParam = RunSlowDownParam;
            else if (crouching())
                slowDownParam = CrouchSlowDownParam;
            else if (TurningRun() != 0)
            {
                slowDownParam = TurnRunSlowDownParam;
                //Debug.Log(slowDownParam);
            }
            else
                Debug.LogError("checkSpeed");

            float velocityDamp = slowDownParam * zap.getCurrentDeltaTime();
            speedX -= velocityDamp;
            if (speedX < desiredSpeedX)
            {
                speedX = desiredSpeedX;
                zap.velocity.x = desiredSpeedX * dir;
                return true;
            }

            zap.velocity.x = speedX * dir;

            //Debug.Log("" + speedX);
            //Debug.Log("++++++++++++++++++++++++++++++++++++++");
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

    bool CanPullUp()
    {
        Vector2 rayOrigin = zap.sensorLeft3.transform.position;
        rayOrigin.y += 0.3f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.9f, zap.layerIdGroundAllMask);
        if (hit.collider) return false;

        rayOrigin = zap.sensorRight3.transform.position;
        rayOrigin.y += 0.3f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.9f, zap.layerIdGroundAllMask);
        if (hit.collider) return false;

        return true;
    }

    //bool CanPullUp(Transform handle, int dir)
    //{
    //    Vector2 rayOrigin = zap.sensorLeft3.transform.position;
    //    rayOrigin.y += 0.3f;
    //    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.9f, zap.layerIdGroundAllMask);
    //    if (hit.collider) return false;

    //    rayOrigin = zap.sensorRight3.transform.position;
    //    rayOrigin.y += 0.3f;
    //    hit = Physics2D.Raycast(rayOrigin, Vector2.up, 1.9f, zap.layerIdGroundAllMask);
    //    if (hit.collider) return false;

    //    return true;
    //}

    bool CanPullDown()
    {
        Vector2 rayOrigin = transform.position;
        rayOrigin.x -= zap.sensorLeft3.localPosition.x;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 1.9f, zap.layerIdGroundAllMask);
        if (hit.collider) return false;

        rayOrigin = transform.position;
        rayOrigin.x -= zap.sensorRight3.localPosition.x;
        hit = Physics2D.Raycast(rayOrigin, Vector2.down, 1.9f, zap.layerIdGroundAllMask);
        if (hit.collider) return false;

        return true;
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
                    return catchedRopeLink.transform.GetChild(0).GetComponent<RopeLink>();
                    //return true;
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
            //float ropeSpeed = catchedRope.firstLinkSpeed;
            //float ropeSpeedRad = ropeSpeed * Mathf.Deg2Rad;
            //int crl_idn = catchedRope.currentLink.GetComponent<RopeLink>().idn;
            //float ps = ropeSpeedRad * (crl_idn + 1) * 0.5f;

            int zapDir = zap.dir2();

            if (Input.GetKey(zap.keyLeft) && (zap.canJumpBackFromRope || zapDir == -1) && !forceJumpOff)
            { //skacze w lewo
                zap.turnLeft();
                JumpRunLeft();

                //if (ropeSpeed > 0f)
                //{ // lina tez leci w lewo
                //    jumpLongLeft();
                //    zap.velocity.x -= ps;
                //}
                //else
                //{
                //    jumpLeft();
                //}
            }
            else if (Input.GetKey(zap.keyRight) && (zap.canJumpBackFromRope || zapDir == 1) && !forceJumpOff)
            { //skacze w prawo
                zap.turnRight();
                JumpRunRight();
                //jumpRight();

                //if (ropeSpeed < 0f)
                //{ // lina tez leci w prawo
                //    jumpLongRight();
                //    zap.velocity.y += ps;
                //}
                //else
                //{
                //    jumpRight();
                //}
            }
            else if (Input.GetKeyDown(zap.keyDown) || Input.GetKey(zap.keyDown) || forceJumpOff)
            {
                zap.velocity.x = 0f;
                zap.velocity.y = 0f;
                setAction(Action.Jump);
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
        setAction(Action.Idle);
        resetActionAndState();
    }

    void StartPullUpFromBelly1(Transform bellyHandler)
    {
        catchedClimbHandle = bellyHandler.gameObject;

        Vector3 handlePos = catchedClimbHandle.transform.position;
        Vector3 newPos = new Vector3();
        newPos.x = handlePos.x - zap.getMyHalfWidth();// + 0.2f;
        newPos.y = handlePos.y - 1.75f; //myHeight;

        climbBeforePos = transform.position;
        climbAfterPos2 = handlePos;
        climbAfterPos2.x += (zap.dir2() * 0.1f);
        climbDistToClimb = climbAfterPos2 - climbBeforePos;
    }

    void StartPullUpFromBelly2(/*Transform bellyHandler*/)
    {
        if (catchedClimbHandle.tag == "CrumblingStairs")
        {
            CatchCrumblingStairs();
        }
        else
        {
            zap.velocity.x = 0.0f;
            zap.velocity.y = 0.0f;

            zap.setState(Zap.State.CLIMB);
            setAction(Action.ClimbBelly);
            lastFrameHande = false;
        }
    }

    void StartPullUpFromBelly3(Transform bellyHandler, Vector3 _handle)
    {
        //catchedClimbHandle = hit.collider.gameObject;

        //Vector3 handlePos = _handle; //catchedClimbHandle.transform.position;
        //Vector3 newPos = new Vector3();
        //newPos.x = handlePos.x - zap.getMyHalfWidth();
        //newPos.y = handlePos.y - 1.75f; //myHeight;

        //zap.velocity.x = 0.0f;
        //zap.velocity.y = 0.0f;

        //climbBeforePos = transform.position;
        //climbAfterPos = newPos;
        //climbAfterPos2 = handlePos;
        //climbAfterPos2.x += 0.1f;
        //climbDistToClimb = climbAfterPos - climbBeforePos;
        //climbToJumpDuration = climbDistToClimb.magnitude * _speed;

        ////canPullUp = canClimbPullUp2();
        ////if (canPullUp)
        ////{
        ////}

        //zap.setState(Zap.State.CLIMB);
        //setAction(Action.CLIMB_JUMP_TO_CATCH, fromGround ? 1 : 0);
        //lastFrameHande = false;
        //return true;

        catchedClimbHandle = bellyHandler.gameObject;

        Vector3 handlePos = _handle;// catchedClimbHandle.transform.position;
        Vector3 newPos = new Vector3();
        newPos.x = handlePos.x - zap.getMyHalfWidth();// + 0.2f;
        newPos.y = handlePos.y - 1.75f; //myHeight;

        climbBeforePos = transform.position;
        climbAfterPos2 = handlePos;
        climbAfterPos2.x += (zap.dir2() * 0.1f);
        climbDistToClimb = climbAfterPos2 - climbBeforePos;
    }

    void StartPullUpFromBelly4(/*Transform bellyHandler*/)
    {
        if (catchedClimbHandle.tag == "CrumblingStairs")
        {
            CatchCrumblingStairs();
        }
        else
        {
            zap.velocity.x = 0.0f;
            zap.velocity.y = 0.0f;

            zap.setState(Zap.State.CLIMB);
            setAction(Action.ClimbBelly);
            lastFrameHande = false;
        }
    }

    bool tryCatchHandle(bool fromGround = false)
    {
        //bool catchByBelly = false;
        bool _cpu = false;

        if (zap.dir() == Vector2.right)
        {
            float _speed = 0.2f;
            RaycastHit2D hit;
            if (fromGround)
            {
                hit = Physics2D.BoxCast(zap.handlerBellyRight.position, zap.handlerBellyRightSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundHandlesMask);
                if (hit.collider)
                {
                    StartPullUpFromBelly1(hit.transform);
                    _cpu = canClimbPullUp2();
                    if (_cpu)
                    {
                        StartPullUpFromBelly2();
                        return true;
                    }
                    else
                    {
                        catchedClimbHandle = null;
                    }
                }
                hit = Physics2D.BoxCast(zap.handlerRight.position, zap.handlerRightSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundHandlesMask);
                _speed = 0.2f;
            }
            else
            {
                hit = Physics2D.BoxCast(zap.handlerBellyRight.position, zap.handlerBellyRightSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundHandlesMask);
                if (hit.collider)
                {
                    StartPullUpFromBelly1(hit.transform);
                    _cpu = canClimbPullUp2();
                    if (_cpu)
                    {
                        StartPullUpFromBelly2();
                        return true;
                    }
                    else
                    {
                        catchedClimbHandle = null;
                    }
                }
                hit = Physics2D.BoxCast(zap.handlerRight.position, zap.handlerRightSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundHandlesMask);
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
                    climbAfterPos2.x += 0.1f;
                    climbDistToClimb = climbAfterPos - climbBeforePos;
                    climbToJumpDuration = climbDistToClimb.magnitude * _speed;

                    zap.setState(Zap.State.CLIMB);
                    setAction(Action.CLIMB_JUMP_TO_CATCH, fromGround ? 1 : 0);
                    lastFrameHande = false;

                    return true;
                }
            }

            // łapanie sie kamieni brzuchem
            Vector2 _handle = new Vector2();
            Vector2 ro = zap.handlerBellyRight.position;
            ro.x += 0.15f;
            
            hit = Physics2D.BoxCast(ro, zap.handlerBellyRightSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundMoveableMask);
            if (hit.collider)
            {
                //if (hit.collider.GetComponent<GroundMoveable>().handleToPullDownTouched(zap.dir(), ro, ref _handle))
                GroundMoveable _gm_ = hit.collider.GetComponent<GroundMoveable>();
                if (_gm_ && _gm_.handleToPullDownTouched(zap.dir(), ro, ref _handle))
                {
                    StartPullUpFromBelly3(hit.transform,_handle);
                    _cpu = canClimbPullUp2();
                    if (_cpu)
                    {
                        StartPullUpFromBelly4();
                        return true;
                    }
                    else
                    {
                        catchedClimbHandle = null;
                    }
                }
                else
                {
                    catchedClimbHandle = null;
                }
            }

            _handle = new Vector2();
            ro = transform.position;
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
                //if (hit.collider.GetComponent<GroundMoveable>().handleToPullDownTouched(zap.dir(), ro, ref _handle))
                GroundMoveable _gm_ = hit.collider.GetComponent<GroundMoveable>();
                if (_gm_ && _gm_.handleToPullDownTouched(zap.dir(), ro, ref _handle))
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
                        climbAfterPos2.x += 0.1f;
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
                hit = Physics2D.BoxCast(zap.handlerBellyLeft.position, zap.handlerBellyLeftSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundHandlesMask);
                if (hit.collider)
                {
                    StartPullUpFromBelly1(hit.transform);
                    _cpu = canClimbPullUp2();
                    if (_cpu)
                    {
                        StartPullUpFromBelly2();
                        return true;
                    }
                    else
                    {
                        catchedClimbHandle = null;
                    }
                }
                hit = Physics2D.BoxCast(zap.handlerLeft.position, zap.handlerLeftSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundHandlesMask);
                _speed = 0.2f;
            }
            else
            {
                hit = Physics2D.BoxCast(zap.handlerBellyLeft.position, zap.handlerBellyLeftSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundHandlesMask);
                if (hit.collider)
                {
                    StartPullUpFromBelly1(hit.transform);
                    _cpu = canClimbPullUp2();
                    if (_cpu)
                    {
                        StartPullUpFromBelly2();
                        return true;
                    }
                    else
                    {
                        catchedClimbHandle = null;
                    }
                }
                hit = Physics2D.BoxCast(zap.handlerLeft.position, zap.handlerLeftSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundHandlesMask);
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
                    climbAfterPos2.x -= 0.1f;
                    climbDistToClimb = climbAfterPos - climbBeforePos;
                    climbToJumpDuration = climbDistToClimb.magnitude * _speed;

                    zap.setState(Zap.State.CLIMB);
                    setAction(Action.CLIMB_JUMP_TO_CATCH, fromGround ? 1 : 0);
                    lastFrameHande = false;
                    
                    return true;
                }
            }

            // łapanie sie kamieni brzuchem
            Vector2 _handle = new Vector2();
            Vector2 ro = zap.handlerBellyLeft.position;
            ro.x -= 0.15f;

            hit = Physics2D.BoxCast(ro, zap.handlerBellyLeftSize, 0.0f, Vector2.left, 0.0f, zap.layerIdGroundMoveableMask);
            if (hit.collider)
            {
                GroundMoveable _gm_ = hit.collider.GetComponent<GroundMoveable>();
                if (_gm_ && _gm_.handleToPullDownTouched(zap.dir(), ro, ref _handle))
                {
                    StartPullUpFromBelly3(hit.transform,_handle);
                    _cpu = canClimbPullUp2();
                    if (_cpu)
                    {
                        StartPullUpFromBelly4();
                        return true;
                    }
                    else
                    {
                        catchedClimbHandle = null;
                    }
                }
                else
                {
                    catchedClimbHandle = null;
                }
            }

            _handle = new Vector2();
            ro = transform.position;
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
                //if (hit.collider.GetComponent<GroundMoveable>().handleToPullDownTouched(zap.dir(), ro, ref _handle))
                GroundMoveable _gm_ = hit.collider.GetComponent<GroundMoveable>();
                if (_gm_ && _gm_.handleToPullDownTouched(zap.dir(), ro, ref _handle))
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
                        climbAfterPos2.x -= 0.1f;
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

    Vector3 _lastZapSensorLeft3Pos = new Vector3();

    public Transform CheckHandle(int layerID)
    {
        RaycastHit2D hit;
        Vector2 rayOrigin = zap.sensorLeft3.transform.position;

        //if ( lastFrameHande )
        //{
        //    //zap.sensorHandleR2.position
        //    //lastFrameHande;
        //    //Vector2 rayOrigin = zap.sensorLeft3.transform.position; // transform.position;
        //    //                                                        //Vector2 rayOrigin = zap.sensorHandleR2.position.transform.position; // transform.position;
        //    //rayOrigin.y += 0.3f;
        //    //hit = Physics2D.Raycast(rayOrigin, Vector2.right, zap.myWidth, layerID);

        //    //rayOrigin ;
        //    rayOrigin.y += 0.3f;
        //    hit = Physics2D.Raycast(rayOrigin, Vector2.right, zap.myWidth, layerID);
        //    if (!hit.collider)
        //    {
        //        //Debug.DrawLine(_lastZapSensorLeft3Pos, rayOrigin);

        //        hit = Physics2D.Linecast(_lastZapSensorLeft3Pos, rayOrigin, layerID);
        //        if (!hit.collider)
        //        {
        //            _lastZapSensorLeft3Pos.x += zap.myWidth;
        //            rayOrigin.x += zap.myWidth;
        //            //Debug.DrawLine(_lastZapSensorLeft3Pos, rayOrigin);
        //            hit = Physics2D.Linecast(_lastZapSensorLeft3Pos, rayOrigin, layerID);
        //        }
        //    }
        //}
        //else
        {
            //zap.sensorHandleR2.position
            //lastFrameHande;
            //rayOrigin = zap.sensorLeft3.transform.position; // transform.position;
                                                                    //Vector2 rayOrigin = zap.sensorHandleR2.position.transform.position; // transform.position;
            rayOrigin.y += 0.3f;
            hit = Physics2D.Raycast(rayOrigin, Vector2.right, zap.myWidth, layerID);
        }
        

        if (!hit.collider)
        {
            _lastZapSensorLeft3Pos = zap.sensorLeft3.transform.position;
            return null;
        }
        else
        {
            MountMoveable _mm = hit.collider.GetComponent<MountMoveable>();
            if (_mm)
            {
                //Debug.Log("JEST MM");
                //Vector3 pointToCheck = handledMountMoveable.ConvertToPointSize(handledMountMoveablePosition);
                ////pointToCheck += distToMount;
                //bool res = handledMountMoveable.LocalPointHandable(pointToCheck);
                ////if (!res)

                //Vector3 zapHandpos = zap.transform.position;
                //zapHandpos.y += (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                //handledMountMoveablePosition = handledMountMoveable.transform.InverseTransformPoint(zapHandpos);

                Vector3 zapHandpos = transform.position;
                zapHandpos.y += (zap.sensorLeft3.transform.localPosition.y + 0.3f);
                Vector3 _pointToCheck = _mm.transform.InverseTransformPoint(zapHandpos);
                _pointToCheck = _mm.ConvertToPointSize(_pointToCheck);
                bool res = _mm.LocalPointHandable(_pointToCheck);
                if (res)
                {
                    _lastZapSensorLeft3Pos = zap.sensorLeft3.transform.position;
                    return hit.collider.transform;
                }
            }
        }
        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerID);
        if (!hit.collider)
        {
            _lastZapSensorLeft3Pos = zap.sensorLeft3.transform.position;
            return null;
        }

        rayOrigin.x += zap.myWidth;
        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerID);
        if (hit.collider)
        {
            _lastZapSensorLeft3Pos = zap.sensorLeft3.transform.position;
            return hit.collider.transform;
        }
        else
        {
            _lastZapSensorLeft3Pos = zap.sensorLeft3.transform.position;
            return null;
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
                    if( !catchedRopeLink.rope.Catchable )
                    {
                        catchedRopeLink = null;
                        lastHandlePos = zap.sensorHandleR2.position;
                        return false;
                    }
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
                    if (!catchedRopeLink.rope.Catchable)
                    {
                        catchedRopeLink = null;
                        lastHandlePos = zap.sensorHandleR2.position;
                        return false;
                    }
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

        float forceRatio = Mathf.Abs(zap.velocity.x) / JumpRunMaxSpeed;
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

        float tgAlpha = Mathf.Tan(Mathf.Deg2Rad * MaxPlatformAngle);

        Vector2 rayOrigin = climbAfterPos2; // catchedClimbHandle.transform.position;
        rayOrigin.y += tgAlpha * 0.5f; //  0.14f;

        //if (zap.dir() == Vector2.right) rayOrigin.x += 0.5f;
        //else rayOrigin.x -= 0.5f;
        float dtf = 0.5f;
        if (zap.checkCeil(ref dtf)) return false;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, 0.825f, zap.layerIdGroundAllMask);
        if (hit.collider) return false;

        rayOrigin.x -= zap.faceRight() ? 0.6f : 0.4f;
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

        if (!isInState(Zap.State.ON_GROUND) || !(isInAction(Action.Idle) || crouching()))
            return null;

        // 1: sytuacja gdy zap jest swoim srodkiem nad tilem
        // 2: sytuacja gdy zap jest swoim srodkiem juz poza tilem

        if (zap.groundUnder)
        {
            GroundMoveable gm = zap.groundUnder.GetComponent<GroundMoveable>();
            if (gm)
            {
                Vector2 _handle = new Vector2();
                if (gm.handleToPullDownTouched(zap.dir(), transform.position, ref _handle, 0.51f))
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
                Vector2 ro = hit.collider.gameObject.transform.position;
                ro.x -= 0.1f;
                if (Physics2D.Raycast(ro, -Vector2.right, 0.4f, zap.layerIdGroundMask).collider == null)
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
                Vector2 ro = hit.collider.gameObject.transform.position;
                ro.x += 0.1f;
                if (Physics2D.Raycast(ro, Vector2.right, 0.4f, zap.layerIdGroundMask).collider == null)
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
    float beforTurnRunSpeed = 0f;
    MountMoveable handledMountMoveable = null;
    Vector3 handledMountMoveablePosition = new Vector3();

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
