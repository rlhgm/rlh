using UnityEngine;
using System.Collections;

public class Rat : MonoBehaviour
{
    public int ControlID = 0;

    public float WalkSpeed = 2f;
    public float RunSpeed = 5f;
    public float JumpSpeed = 1f;

    public float FightModeDistance = 5f;
    public float IdleInDuration = 0.25f;
    public float IdleOutDuration = 0.25f;
    public float TurnbackDuration = 0.5f;
    public float JumpDuration = 0.5f;
    public float ClimbUpDuration = 0.83f;
    public float ClimbUpInDuration = 0.25f;
    public float ClimbDownDuration = 0.5f;
    public float AttackDuration = 0.583f;

    public float LandingDuration = 0.33f;

    public float GravityForce = -20f;
    public float MaxSpeedY = 15.0f;

    // Use this for initialization
    void Awake()
    {
        StaticInit();

        myGfx = transform.Find("gfx");
        mySensor = transform.Find("sensor");
        myAnimator = myGfx.GetComponent<Animator>();

        myCollider = GetComponent<BoxCollider2D>();
        mySize = myCollider.size;
        myHalfSize = mySize * 0.5f;
    }

    void OnEnable()
    {
        //print("Rat::OnEnabled");    
        StaticInit();
    }

    void Start()
    {
        startPos = transform.position;

        SetMode(Mode.Normal);
        SetState(State.OnGround);
        //SetAction(Action.walk);
        //Think(ThinkCause.Boredom);
        WalkStart();

        JumpSpeed = 2.0f / JumpDuration; // jumpdist
    }

    // Update is called once per frame
    void Update()
    {
        currentDeltaTime = Time.deltaTime;

        modeJustChanged = false;
        stateJustChanged = false;
        actionJustChanged = false;

        currentModeTime += currentDeltaTime;
        currentStateTime += currentDeltaTime;
        currentActionTime += currentDeltaTime;

        lastPos = transform.position;
        newPos = transform.position;
        lastVelocity = velocity;
        newVelocity = velocity;

        targetPos = RLHScene.Instance.Zap.transform.position;
        distToTarget = Vector3.Distance(transform.position, targetPos);
        distToTargetX = transform.position.x - targetPos.x;

        CheckMode();

        switch (action)
        {
            case Action.Idle:
                ActionIdle();
                break;
            case Action.IdleIn:
                ActionIdleIn();
                break;
            case Action.IdleOut:
                ActionIdleOut();
                break;

            case Action.WalkLeft:
                ActionWalk(-1);
                break;
            case Action.WalkRight:
                ActionWalk(1);
                break;

            case Action.RunLeft:
                ActionRun(-1);
                break;
            case Action.RunRight:
                ActionRun(1);
                break;

            case Action.TurnbackLeft:
                ActionTurnbackLeft();
                break;
            case Action.TurnbackRight:
                ActionTurnbackRight();
                break;

            case Action.JumpLeft:
                ActionJump();
                break;

            case Action.JumpRight:
                ActionJump();
                break;

            case Action.Landing:
                ActionLanding();
                break;

            case Action.Fly:

                break;

            case Action.ClimbUpInLeft:
            case Action.ClimbUpInRight:
                ActionClimbUpIn();
                break;
                
            case Action.ClimbUpLeft:
                ActionClimbUp();
                break;

            case Action.ClimbUpRight:
                ActionClimbUp();
                break;

            case Action.Attack:
                ActionAttack();
                break;


            case Action.Die:
                ActionDie();
                break;
        }

        switch (state)
        {
            case State.OnGround:
                if (RLHScene.Instance.checkGround(mySensor.position, 1.0f, ref distToObstacle, ref groundAngle))
                {
                    newPos.y += (myHalfSize.y - distToObstacle);
                    //transform.rotation.z = groundAngle
                    transform.position = newPos;
                }
                else
                {
                    SetState(State.InAir);
                    SetAction(Action.Fly);
                }
                break;

            case State.InAir:
                if (!Jumping())
                {
                    velocity.y += GravityForce * currentDeltaTime;

                    if (velocity.y > MaxSpeedY)
                        velocity.y = MaxSpeedY;
                    if (velocity.y < -MaxSpeedY)
                        velocity.y = -MaxSpeedY;

                    float distToFallY = velocity.y * currentDeltaTime;

                    //bool justLanding = false;

                    if (distToFallY > 0.0f)
                    { // leci w gore
                    }
                    else if (distToFallY < 0.0f)
                    { // spada
                        if (RLHScene.Instance.checkGround(mySensor.position, myHalfSize.y + distToFallY, ref distToObstacle, ref groundAngle))
                        {
                            newPos.y += (myHalfSize.y - distToObstacle);
                            //transform.rotation.z = groundAngle
                            transform.position = newPos;

                            SetMode(Mode.BackToNormal);
                            SetState(State.OnGround);
                            SetAction(Action.Landing);

                            velocity.y = 0f;
                        }
                        else
                        {
                            newPos.y += distToFallY;
                            transform.position = newPos;
                        }
                    }
                }
                break;
        }


    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //print("Rat::OnTriggerEnter2D" + other.name);
        CreepingAITrigger cait = other.GetComponent<CreepingAITrigger>();
        if (cait)
        {
            if (ControlID == cait.controlID)
            {
                //TurnbackStart();
                Think(ThinkCause.TurnbackTriggerEnter);
            }
        }
    }

    public enum Mode
    {
        Undef = 0,
        Normal,
        //FightChaseTarget,
        //FightCantReachTarget,
        Fight,
        BackToNormal,
        PissedOff
    };

    public enum State
    {
        Undef = 0,
        OnGround,
        InAir,
        Climb,
        Dead,
    };

    public enum Action
    {
        Undef = 0,
        Idle,
        IdleIn,
        IdleOut,

        WalkLeft,
        WalkRight,
        RunLeft,
        RunRight,
        TurnbackLeft,
        TurnbackRight,

        JumpLeft,
        JumpRight,

        Die,

        ClimbUpLeft,
        ClimbUpRight,
        ClimbUpInLeft,
        ClimbUpInRight,
        Attack,

        Landing,
        Fly,

        // next actions:
        NextIdle,
        NextWalk,
        NextRun,
        NextTurnback
    };

    public float CurrentStateTime
    {
        get { return currentStateTime; }
    }

    Mode mode;
    State state;
    Action action;
    Action nextAction;
    float currentModeTime = 0.0f;
    float currentStateTime = 0.0f;
    float currentActionTime = 0.0f;
    bool modeJustChanged = false;
    bool stateJustChanged = false;
    bool actionJustChanged = false;
    Transform myGfx;
    Transform mySensor;
    Animator myAnimator;
    BoxCollider2D myCollider;
    Vector2 mySize;
    Vector2 myHalfSize;

    Vector3 startPos = new Vector3(0f, 0f, 0f);
    Vector3 modeChangedPos = new Vector3(0f, 0f, 0f);
    Vector3 stateChangedPos = new Vector3(0f, 0f, 0f);
    Vector3 actionChangedPos = new Vector3(0f, 0f, 0f);

    //Zap zap;

    Vector2 velocity = new Vector2(0f, 0f);
    Vector2 lastVelocity = new Vector2(0f, 0f);
    Vector2 newVelocity = new Vector2(0f, 0f);
    Vector3 lastPos = new Vector3(0f, 0f, 0f);
    Vector3 newPos = new Vector3(0f, 0f, 0f);
    float distToMove = 0;
    float distToObstacle = 0.0f;
    float groundAngle = 0.0f;

    float currentDeltaTime = 0f;
    Vector3 targetPos = new Vector3(0f, 0f, 0f);
    float distToTarget = 0f;
    float distToTargetX = 0f;

    float helpDuration1 = 0f;
    Vector3 helpPos1 = new Vector3(0f, 0f, 0f);
    ArrayList _rats = new ArrayList(3);

    public bool SetState(State newState)
    {
        if (state == newState)
            return false;

        stateChangedPos = transform.position;
        currentStateTime = 0f;
        stateJustChanged = true;
        state = newState;

        return true;
    }
    public bool IsInState(State test)
    {
        return state == test;
    }
    public bool IsNotInState(State test)
    {
        return state != test;
    }

    bool SetAction(Action newAction, int param = 0)
    {
        if (action == newAction)
            return false;

        actionChangedPos = transform.position;
        currentActionTime = 0f;
        actionJustChanged = true;
        action = newAction;

        myAnimator.speed = 1f;

        switch (action)
        {
            case Action.Idle:
                myAnimator.Play(idleAnimStateHash);
                break;

            case Action.IdleIn:
                myAnimator.Play(idleInAnimStateHash);
                break;

            case Action.IdleOut:
                myAnimator.Play(idleOutAnimStateHash);
                break;

            case Action.WalkLeft:
                myAnimator.Play(walkAnimStateHash);
                break;

            case Action.WalkRight:
                myAnimator.Play(walkAnimStateHash);
                break;

            case Action.RunLeft:
                myAnimator.Play(runAnimStateHash);
                break;

            case Action.RunRight:
                myAnimator.Play(runAnimStateHash);
                break;

            case Action.TurnbackLeft:
                myAnimator.Play(turnbackAnimStateHash);
                break;

            case Action.TurnbackRight:
                myAnimator.Play(turnbackAnimStateHash);
                break;

            case Action.JumpLeft:
                myAnimator.Play(jumpAnimStateHash);
                break;

            case Action.JumpRight:
                myAnimator.Play(jumpAnimStateHash);
                break;

            case Action.Landing:
                myAnimator.Play(landingAnimStateHash);
                break;

            case Action.Fly:
                myAnimator.Play(flyAnimStateHash);
                myAnimator.speed = 0f;
                break;

            case Action.Die:
                myAnimator.Play(dieAnimStateHash);
                break;

            case Action.ClimbUpLeft:
                myAnimator.Play(climbUpAnimStateHash);
                break;

            case Action.ClimbUpRight:
                myAnimator.Play(climbUpAnimStateHash);
                break;

            case Action.ClimbUpInLeft:
                myAnimator.Play(climbUpInAnimStateHash);
                break;

            case Action.ClimbUpInRight:
                myAnimator.Play(climbUpInAnimStateHash);
                break;

            case Action.Attack:
                myAnimator.Play(attackAnimStateHash);
                break;
        }

        return true;
    }

    public bool IsInAction(Action test)
    {
        return action == test;
    }

    public bool IsNotInAction(Action test)
    {
        return action != test;
    }

    enum ThinkCause
    {
        Boredom,
        CantGoFuther,
        FinishAction,
        TurnbackTriggerEnter
    }

    ThinkCause lastThinkCause = ThinkCause.Boredom;

    bool CheckMode()
    {
        if (Jumping()) return false;

        switch (mode)
        {
            case Mode.Normal:
                if (distToTarget <= FightModeDistance)
                {
                    SetMode(Mode.Fight);
                    return true;
                }
                break;

            case Mode.Fight:
                if (distToTarget > FightModeDistance)
                {
                    SetMode(Mode.BackToNormal);
                    return true;
                }
                break;

            case Mode.BackToNormal:
                if (distToTarget <= FightModeDistance)
                {
                    SetMode(Mode.Fight);
                    return true;
                }
                else
                {
                    float distToRespawPoint = Mathf.Abs(transform.position.x - startPos.x);
                    if (distToRespawPoint < 1f)
                    {
                        SetMode(Mode.Normal);
                        return true;
                    }
                }
                break;

            case Mode.PissedOff:
                if (Mathf.Abs(modeChangedPos.x - transform.position.x) > helpDuration1)
                {
                    //print("return to fight : " + modeChangedPos.x + " " + transform.position.x);
                    SetMode(Mode.Fight);
                    Think(ThinkCause.FinishAction);
                }
                break;
        }
        return false;
    }

    void Think(ThinkCause cause, int param = 0)
    {
        switch (mode)
        {
            case Mode.Normal:
                ThinkNormal(cause, param);
                break;

            case Mode.Fight:
                ThinkFight(cause, param);
                break;

            case Mode.BackToNormal:
                ThinkBackToNormal(cause, param);
                break;

            case Mode.PissedOff:
                ThinkPissedOff(cause, param);
                break;
        }

        lastThinkCause = cause;
    }

    void ThinkNormal(ThinkCause cause, int param = 0)
    {
        switch (cause)
        {
            case ThinkCause.Boredom:
                break;

            case ThinkCause.CantGoFuther:
                if (param == 1 && Random.Range(0, 2) == 1)
                {
                    SetAction(Action.IdleIn);
                    nextAction = Action.NextTurnback;
                    helpDuration1 = Random.Range(0.5f, 2f);
                }
                else
                {
                    TurnbackStart();
                }
                break;

            case ThinkCause.FinishAction:
                if (TurningBack())
                {
                    helpDuration1 = Random.Range(1.5f, 3f);
                    WalkStart();
                }
                else if (Climbing())
                {
                    switch (Random.Range(0, 3))
                    {
                        case 0:
                            helpDuration1 = Random.Range(1.5f, 3f);
                            WalkStart();
                            break;

                        case 1:
                            SetAction(Action.IdleIn);
                            nextAction = Random.Range(0, 2) == 1 ? Action.NextTurnback : Action.NextWalk;
                            helpDuration1 = Random.Range(0.5f, 2f);
                            break;

                        case 2:
                            TurnbackStart();
                            break;
                    }
                }
                else if (Walking() || Running())
                {
                    SetAction(Action.IdleIn);
                    nextAction = Random.Range(0, 2) == 1 ? Action.NextTurnback : Action.NextWalk;
                    helpDuration1 = Random.Range(0.5f, 2f);
                }
                else if (Jumping())
                {
                    switch (Random.Range(0, 2))
                    {
                        case 0:
                            SetAction(nextAction);
                            break;

                        case 1:
                            SetAction(Action.IdleIn);
                            //nextAction = Random.Range(0, 2) == 1 ? Action.NextTurnback : Action.NextWalk;
                            nextAction = Action.NextWalk;
                            helpDuration1 = Random.Range(0.5f, 2f);
                            break;
                    }                    
                }
                else if (IsInAction(Action.IdleOut))
                {
                    if (nextAction == Action.NextTurnback)
                    {
                        TurnbackStart();
                    }
                    else
                    {
                        WalkStart();
                    }
                }
                break;

            case ThinkCause.TurnbackTriggerEnter:
                TurnbackStart();
                break;
        }
    }

    void ThinkFight(ThinkCause cause, int param = 0)
    {
        switch (cause)
        {
            case ThinkCause.Boredom:
                break;

            case ThinkCause.CantGoFuther:
                //bool canIdled = true;
                RLHScene.Instance.getRatsOnPosition(transform.position, 0.5f, 3, ref _rats);
                for (int i = 0; i < _rats.Count; ++i)
                {
                    Rat brother = (Rat)_rats[i];
                    if (brother == this) continue;
                    if (brother.IsInAction(Action.Idle) || brother.IsInAction(Action.IdleIn))
                    {
                        //canIdled = false;
                        SetMode(Mode.PissedOff);
                        TurnbackStart();
                        return;
                    }
                }

                //if (canIdled)
                //{
                SetAction(Action.IdleIn);
                nextAction = Action.NextTurnback;
                helpDuration1 = Random.Range(0.5f, 2f);
                //}
                break;

            case ThinkCause.FinishAction:
                if (IsInAction(Action.IdleOut) || Walking() || TurningBack() /*|| Jumping()*/ )
                {
                    if (SetFaceToTarget())
                    {
                        break;
                    }
                    else
                    {
                        RunStart();
                    }
                }
                else if (Climbing())
                {
                    SetAction(nextAction);
                }
                else if (Running())
                {
                    //bool canIdled = true;
                    RLHScene.Instance.getRatsOnPosition(transform.position, 0.5f, 3, ref _rats);
                    for (int i = 0; i < _rats.Count; ++i)
                    {
                        Rat brother = (Rat)_rats[i];
                        if (brother == this) continue;
                        if (brother.IsInAction(Action.Idle) || brother.IsInAction(Action.IdleIn))
                        {
                            //canIdled = false;
                            SetMode(Mode.PissedOff);
                            TurnbackStart();
                            return;
                        }
                    }

                    SetAction(Action.IdleIn);
                    //nextAction = Action.NextTurnback;
                    //helpDuration1 = -1;
                    nextAction = Random.Range(0, 2) == 1 ? Action.NextTurnback : Action.NextRun;
                    helpDuration1 = Random.Range(0.5f, 2f);
                    //print("ThinkFight => run finish");
                }
                else if (Jumping())
                {
                    SetAction(nextAction);
                    
                    //SetAction(Action.IdleIn);
                    //helpDuration1 = Random.Range(0.5f, 2f);
                }

                break;

            case ThinkCause.TurnbackTriggerEnter:
                break;
        }
    }

    void ThinkPissedOff(ThinkCause cause, int param = 0)
    {
        switch (cause)
        {
            case ThinkCause.Boredom:
                break;

            case ThinkCause.CantGoFuther:
                SetMode(Mode.Fight);
                //SetAction(Action.IdleIn);
                //nextAction = Action.NextTurnback;
                //helpDuration1 = Random.Range(0.5f, 2f);
                Think(ThinkCause.FinishAction);
                break;

            case ThinkCause.FinishAction:
                if (TurningBack() || Climbing())
                {
                    //RunStart();
                    helpDuration1 = Random.Range(0.25f, 1.0f);
                    //print("pissed off = > " + helpDuration1);
                    if (Random.Range(0, 2) == 1)
                    {
                        RunStart();
                    }
                    else
                    {
                        //print("start walk pissed off");
                        WalkStart();
                    }
                    //print("pissed off = > RunStart");
                }
                else if (IsInAction(Action.IdleOut))
                {
                    if (nextAction == Action.NextTurnback)
                    {
                        TurnbackStart();
                        //print("pissed off = > TurnbackStart" );
                    }
                    else if (nextAction == Action.NextRun)
                    {
                        helpDuration1 = Random.Range(0.25f, 1.0f);
                        //print("pissed off = > " + helpDuration1);
                        if (Random.Range(0, 2) == 1)
                        {
                            RunStart();
                        }
                        else
                        {
                            //print("start walk pissed off");
                            WalkStart();
                        }
                    }
                }
                else if (Jumping())
                {
                    SetMode(Mode.Fight);
                    SetAction(Action.IdleIn);
                    nextAction = Random.Range(0, 2) == 1 ? Action.NextTurnback : Action.NextRun;
                    helpDuration1 = Random.Range(0.5f, 2f);
                }

                break;

            case ThinkCause.TurnbackTriggerEnter:
                break;
        }
    }

    void ThinkBackToNormal(ThinkCause cause, int param = 0)
    {
        switch (cause)
        {
            case ThinkCause.Boredom:
                break;

            case ThinkCause.CantGoFuther:
                break;

            case ThinkCause.FinishAction:
                if (SetFaceToTarget(startPos.x))
                {

                }
                else
                {
                    WalkStart();
                }

                //if (TurningBack())
                //{
                //    //helpDuration1 = -1; Random.Range(1.5f, 3f);
                //    WalkStart();
                //}
                //else if (IsInAction(Action.IdleOut))
                //{
                //    if (nextAction == Action.NextTurnback)
                //    {
                //        TurnbackStart();
                //    }
                //    else
                //    {
                //        WalkStart();
                //    }
                //}
                //else if (Running())
                //{
                //    //SetAction(Action.IdleIn);
                //    //nextAction = Random.Range(0, 2) == 1 ? Action.NextTurnback : Action.NextWalk;
                //    //helpDuration1 = Random.Range(0.5f, 2f);
                //}
                break;

            case ThinkCause.TurnbackTriggerEnter:
                break;
        }
    }

    bool SetFaceToTarget(float targetPosX)
    {
        if (((transform.position.x < targetPosX) && FaceLeft()) || ((transform.position.x > targetPosX) && FaceRight()))
        {
            TurnbackStart();
            return true;
        }
        return false;
    }

    bool SetFaceToTarget()
    {
        //if (((transform.position.x < targetPos.x) && FaceLeft()) || ((transform.position.x > targetPos.x) && FaceRight()))
        //{
        //    TurnbackStart();
        //    return true;
        //}
        //return false;
        return SetFaceToTarget(targetPos.x);
    }

    bool SetMode(Mode newMode, int param = 0)
    {
        if (mode == newMode)
            return false;

        modeChangedPos = transform.position;
        currentModeTime = 0f;
        modeJustChanged = true;
        mode = newMode;

        //print(mode);

        //switch (mode)
        //{
        //    case Mode.Normal:
        //        break;

        //    //case Mode.FIGHT:
        //    //    break;
        //}

        return true;
    }

    public bool IsInMode(Mode test)
    {
        return mode == test;
    }

    public bool IsNotInMode(Mode test)
    {
        return mode != test;
    }

    static bool staticInitiated = false;
    static int idleAnimStateHash;
    static int idleInAnimStateHash;
    static int idleOutAnimStateHash;
    static int walkAnimStateHash;
    static int runAnimStateHash;
    static int turnbackAnimStateHash;
    static int jumpAnimStateHash;
    static int landingAnimStateHash;
    static int flyAnimStateHash;
    static int dieAnimStateHash;
    static int climbUpAnimStateHash;
    static int climbUpInAnimStateHash;
    static int attackAnimStateHash;

    static bool StaticInit()
    {
        if (staticInitiated) return false;

        idleAnimStateHash = Animator.StringToHash("Idle");
        idleInAnimStateHash = Animator.StringToHash("IdleIn");
        idleOutAnimStateHash = Animator.StringToHash("IdleOut");
        walkAnimStateHash = Animator.StringToHash("Walk");
        runAnimStateHash = Animator.StringToHash("Run");
        turnbackAnimStateHash = Animator.StringToHash("Turnback");
        jumpAnimStateHash = Animator.StringToHash("Jump");
        landingAnimStateHash = Animator.StringToHash("Landing2");
        flyAnimStateHash = Animator.StringToHash("Fly");
        dieAnimStateHash = Animator.StringToHash("Die");
        climbUpAnimStateHash = Animator.StringToHash("ClimbUp");
        climbUpInAnimStateHash = Animator.StringToHash("ClimbUpIn");
        attackAnimStateHash = Animator.StringToHash("Attack");

        staticInitiated = true;
        return true;
    }

    void ActionIdle()
    {
        switch (mode)
        {
            case Mode.Normal:
                if (currentActionTime >= helpDuration1 || (modeJustChanged && IsInMode(Mode.Fight)))
                {
                    SetAction(Action.IdleOut);
                }
                break;

            case Mode.BackToNormal:
                if (currentActionTime >= helpDuration1)
                {
                    SetAction(Action.IdleOut);
                }
                break;

            case Mode.Fight:
                //if ( Mathf.Abs(distToTargetX) > 1f ) //&& lastThinkCause != ThinkCause.CantGoFuther)
                if ((Mathf.Abs(distToTargetX) > 0.75f && lastThinkCause != ThinkCause.CantGoFuther)
                    || ((distToTargetX > 0 && FaceRight()) || (distToTargetX < 0 && FaceLeft())))
                {
                    SetAction(Action.IdleOut);
                    break;
                }

                if (modeJustChanged && IsInMode(Mode.BackToNormal))
                {
                    SetAction(Action.IdleOut);
                }

                if (currentActionTime >= helpDuration1)
                {
                    SetMode(Mode.PissedOff);
                    SetAction(Action.IdleOut);
                    Think(ThinkCause.FinishAction);
                }

                break;
        }
    }
    void ActionIdleIn()
    {
        if (currentActionTime >= IdleInDuration)
        {
            SetAction(Action.Idle);
        }
    }
    void ActionIdleOut()
    {
        if (currentActionTime >= IdleInDuration)
        {
            Think(ThinkCause.FinishAction);
        }
    }
    void ActionWalk(int moveDir)
    {
        switch (mode)
        {
            case Mode.Normal:
                if (currentActionTime >= helpDuration1)
                {
                    Think(ThinkCause.FinishAction);
                    return;
                }
                break;

            case Mode.Fight:
                Think(ThinkCause.FinishAction);
                return;

            case Mode.BackToNormal:
                break;
        }

        ActionMove(moveDir, WalkSpeed);
    }
    void ActionRun(int moveDir)
    {
        switch (mode)
        {
            case Mode.Normal:
                Think(ThinkCause.FinishAction);
                return;

            case Mode.Fight:
                if (Mathf.Abs(distToTargetX) < 0.75f)
                {
                    Think(ThinkCause.FinishAction);
                    return;
                }
                break;

            case Mode.BackToNormal:
                Think(ThinkCause.FinishAction);
                break;
        }

        ActionMove(moveDir, RunSpeed);
    }
    void ActionMove(int moveDir, float speed)
    {
        distToMove = speed * currentDeltaTime;

        if (RLHScene.Instance.checkObstacle(mySensor.position, Dir(), distToMove + myHalfSize.x, ref distToObstacle, 45f))
        {
            distToMove = distToObstacle - myHalfSize.x;
            if (canClimbUp(false))
            {
                //print("can climbup");
                ClimbUpInStart();
            }
            else
            {
                Think(ThinkCause.CantGoFuther);
            }
        }
        else
        {
            Vector2 testGroundPos = mySensor.position;
            testGroundPos.x += ((distToMove + 0.5f) * moveDir);

            if (!RLHScene.Instance.checkGround(testGroundPos, 1.0f, ref distToObstacle, ref groundAngle))
            {
                if (canJump(mySensor.position, Dir()))
                {
                    //print("can jump");
                    JumpStart();
                }
                else
                {
                    distToMove = 0.0f;
                    Think(ThinkCause.CantGoFuther, 1);
                }
            }
        }

        newPos.x += (distToMove * moveDir);
        transform.position = newPos;
    }
    void ActionTurnbackLeft()
    {
        ActionTurnback();
    }
    void ActionTurnbackRight()
    {
        ActionTurnback();
    }
    void ActionTurnback()
    {
        if (currentActionTime >= TurnbackDuration)
        {
            Turnback();
            Think(ThinkCause.FinishAction);
        }
    }

    void JumpLeft()
    {
        ActionJump();
    }

    void JumpRight()
    {
        ActionJump();
    }

    void ActionJump()
    {
        if (currentActionTime >= JumpDuration)
        {
            Vector3 finalPos = actionChangedPos;
            finalPos.x += 2f * Dir2();
            transform.position = finalPos;
            SetState(State.OnGround);
            Think(ThinkCause.FinishAction);
            return;
        }

        distToMove = JumpSpeed * currentDeltaTime;

        newPos.x += (distToMove * Dir2());
        transform.position = newPos;
    }

    void ActionDie()
    {
    }

    void ActionLanding()
    {
        if (currentActionTime >= LandingDuration)
        {
            SetMode(Mode.BackToNormal);
            Think(ThinkCause.FinishAction);
        }
    }

    void ActionFly()
    {

    }

    void ActionClimbUpIn()
    {
        if (currentActionTime >= ClimbUpInDuration)
        {
            ClimbUpStart();
        }
    }

    void ActionClimbUp()
    {
        float ratio = Mathf.Min(currentActionTime / ClimbUpDuration, 1f);
        
        if (ratio < 0.33f)
        {
            helpPos1 = actionChangedPos;
            helpPos1.y += (ratio * 3);
            helpPos1.x += ((ratio * 3) * Dir2() * myHalfSize.x);
            transform.position = helpPos1;
        }
        else if (ratio > 0.75f)
        {
            _rayOrigin = helpPos1;
            _rayOrigin.x += ((ratio - 0.75f) * Dir2());
            transform.position = _rayOrigin;
        }
        
        if (currentActionTime >= ClimbUpDuration)
        {
            SetState(State.OnGround);
            //SetAction(nextAction);
            //helpDuration1 = Random.Range(0.5f, 2f);
            Think(ThinkCause.FinishAction);
        }
    }

    void ActionAttack()
    {
        if (currentActionTime >= AttackDuration)
        {
            //SetMode(Mode.BackToNormal);
            Think(ThinkCause.FinishAction);
        }
    }

    void JumpStart()
    {
        nextAction = action;

        if (FaceRight())
            SetAction(Action.JumpRight);
        else
            SetAction(Action.JumpLeft);

        SetState(State.InAir);
    }

    void ClimbUpInStart()
    {
        nextAction = action;
        if (FaceRight())
            SetAction(Action.ClimbUpInRight);
        else
            SetAction(Action.ClimbUpInLeft);
    }

    void ClimbUpStart()
    {
        if (FaceRight())
            SetAction(Action.ClimbUpRight);
        else
            SetAction(Action.ClimbUpLeft);

        SetState(State.Climb);
    }

    void WalkStart()
    {
        if (FaceRight())
            SetAction(Action.WalkRight);
        else
            SetAction(Action.WalkLeft);
    }

    void RunStart()
    {
        if (FaceRight())
            SetAction(Action.RunRight);
        else
            SetAction(Action.RunLeft);
    }

    void TurnbackStart()
    {
        if (FaceRight())
            SetAction(Action.TurnbackLeft);
        else
            SetAction(Action.TurnbackRight);
    }
    void Turnback()
    {
        if (FaceRight()) TurnLeft();
        else TurnRight();
    }
    public void TurnLeft()
    {
        Vector3 scl = myGfx.localScale;
        scl.x = Mathf.Abs(scl.x) * 1.0f;
        myGfx.localScale = scl;
    }
    public void TurnRight()
    {
        Vector3 scl = myGfx.localScale;
        scl.x = Mathf.Abs(scl.x) * -1.0f;
        myGfx.localScale = scl;
    }

    public Vector2 Dir()
    {
        return myGfx.localScale.x < 0.0f ? Vector2.right : -Vector2.right;
    }
    public int Dir2()
    {
        return myGfx.localScale.x < 0f ? (int)1f : (int)-1f;
    }

    public bool FaceLeft()
    {
        return myGfx.localScale.x > 0f;
    }
    public bool FaceRight()
    {
        return myGfx.localScale.x < 0f;
    }

    bool Walking()
    {
        return IsInAction(Action.WalkLeft) || IsInAction(Action.WalkRight);
    }
    bool Running()
    {
        return IsInAction(Action.RunLeft) || IsInAction(Action.RunRight);
    }
    bool Moving()
    {
        return Walking() || Running();
    }
    bool Jumping()
    {
        return IsInAction(Action.JumpLeft) || IsInAction(Action.JumpRight);
    }
    bool TurningBack()
    {
        return IsInAction(Action.TurnbackLeft) || IsInAction(Action.TurnbackRight);
    }
    bool Climbing()
    {
        return IsInAction(Action.ClimbUpLeft) || IsInAction(Action.ClimbUpRight);
    }
    RaycastHit2D _hit;
    Vector2 _rayOrigin = new Vector2(0f, 0f);
    
    public bool canJump(Vector2 from, Vector2 dir)
    {
        _hit = Physics2D.Raycast(from, dir, 2.5f, RLHScene.Instance.layerIdGroundAllMask);
        if (_hit.collider != null)
        {
            return false;
        }

        //Vector2 offset = new Vector2(dir.x * 1.75f, 0f);
        from.x += dir.x * 1.75f;
        _hit = Physics2D.Raycast(from, Vector2.down, 0.5f, RLHScene.Instance.layerIdGroundAllMask);
        if (_hit.collider == null)
        {
            return false;
        }

        from.x += dir.x * 0.5f;
        _hit = Physics2D.Raycast(from, Vector2.down, 0.5f, RLHScene.Instance.layerIdGroundAllMask);
        if (_hit.collider == null)
        {
            return false;
        }

        return true;
    }

    public bool canClimbUp(bool checkObstacle)
    {
        if (checkObstacle)
        {
            _hit = Physics2D.Raycast(mySensor.position, Dir(), myHalfSize.x + 0.1f, RLHScene.Instance.layerIdGroundAllMask);
            if (_hit.collider == null)
            {
                return false;
            }
        }

        _rayOrigin = mySensor.position;
        _hit = Physics2D.Raycast(_rayOrigin, Vector2.up, 1.0f, RLHScene.Instance.layerIdGroundAllMask);
        if (_hit.collider != null)
        {
            return false;
        }

        _rayOrigin.y += 1.0f;
        _hit = Physics2D.Raycast(_rayOrigin, Dir(), myHalfSize.x+1.0f, RLHScene.Instance.layerIdGroundAllMask);
        if (_hit.collider != null)
        {
            return false;
        }

        return true;
    }

    //bool NormalMode()
    //{
    //    return IsInMode(Mode.Normal);
    //}
    //bool FightingMode()
    //{
    //    return IsInMode(Mode.)
    //}
}
