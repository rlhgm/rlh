using UnityEngine;
using System.Collections;

public class Rat : MonoBehaviour
{
    public int ControlID = 0;

    public float WalkSpeed = 2f;
    public float RunSpeed = 5f;

    public float FightModeDistance = 5f;
    public float IdleInDuration = 0.25f;
    public float IdleOutDuration = 0.25f;
    public float TurnbackDuration = 0.5f;

    public static int aaa = 0;

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
                    SetAction(Action.Undef);
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
        BackToNormal
    };

    public enum State
    {
        Undef = 0,
        OnGround,
        InAir,
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
        
        Die,

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
    Vector3 modeChangedPos = new Vector3(0f,0f,0f);
    Vector3 stateChangedPos = new Vector3(0f, 0f, 0f);
    Vector3 actionChangedPos = new Vector3(0f, 0f, 0f);

    //Zap zap;

    Vector2 velocity = new Vector2(0f,0f);
    Vector2 lastVelocity = new Vector2(0f, 0f);
    Vector2 newVelocity = new Vector2(0f, 0f);
    Vector3 lastPos = new Vector3(0f, 0f, 0f);
    Vector3 newPos = new Vector3(0f, 0f, 0f);
    float distToMove = 0;
    float distToObstacle = 0.0f;
    float groundAngle = 0.0f;

    float currentDeltaTime = 0f;
    Vector3 targetPos = new Vector3(0f,0f,0f);
    float distToTarget = 0f;

    float helpDuration1 = 0f;
     
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

            case Action.Die:
                myAnimator.Play(dieAnimStateHash);
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

    bool CheckMode()
    {
        switch(mode)
        {
            case Mode.Normal:
            case Mode.BackToNormal:
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
        }
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
                else if( Walking() )
                {
                    SetAction(Action.IdleIn);
                    nextAction = Random.Range(0,2) == 1 ? Action.NextTurnback : Action.NextWalk;
                    helpDuration1 = Random.Range(0.5f, 2f);
                }
                else if (IsInAction(Action.IdleOut))
                {
                    if( nextAction == Action.NextTurnback)
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
                break;

            case ThinkCause.FinishAction:
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
                break;

            case ThinkCause.TurnbackTriggerEnter:
                break;
        }
    }

    bool SetMode(Mode newMode, int param = 0)
    {
        if (mode == newMode)
            return false;

        modeChangedPos = transform.position;
        currentModeTime = 0f;
        modeJustChanged = true;
        mode = newMode;

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
    static int dieAnimStateHash;

    static bool StaticInit()
    {
        if (staticInitiated) return false;

        idleAnimStateHash = Animator.StringToHash("Idle");
        idleInAnimStateHash = Animator.StringToHash("IdleIn");
        idleOutAnimStateHash = Animator.StringToHash("IdleOut");
        walkAnimStateHash = Animator.StringToHash("Walk");
        runAnimStateHash = Animator.StringToHash("Run");
        turnbackAnimStateHash = Animator.StringToHash("Turnback");
        dieAnimStateHash = Animator.StringToHash("Die");
        
        staticInitiated = true;
        return true;
    } 
            
    void ActionIdle()
    {
        if( currentActionTime >= helpDuration1)
        {
            //Think(ThinkCause.FinishAction);
            SetAction(Action.IdleOut);
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
        if( IsInMode(Mode.Normal))
        {
            if( currentActionTime >= helpDuration1)
            {
                ThinkNormal(ThinkCause.FinishAction);
            }
        }

        ActionMove(moveDir, WalkSpeed);
    }
    void ActionRun(int moveDir)
    {
        ActionMove(moveDir, RunSpeed);
    }
    void ActionMove(int moveDir, float speed)
    {
        distToMove = speed * currentDeltaTime;

        if (RLHScene.Instance.checkObstacle(mySensor.position, Dir(), distToMove + myHalfSize.x, ref distToObstacle, 45f))
        {
            distToMove = distToObstacle - myHalfSize.x;
            Think(ThinkCause.CantGoFuther);
            //if (IsInMode(Mode.Normal) )
            //{
            //    TurnbackStart();
            //}
            //else if( IsInMode(Mode.FightChaseTarget) )
            //{
            //    SetMode(Mode.FightCantReachTarget);
                
            //}
        }
        else
        {
            Vector2 testGroundPos = mySensor.position;
            testGroundPos.x += ((distToMove + 0.5f) * moveDir);

            if (!RLHScene.Instance.checkGround(testGroundPos, 1.0f, ref distToObstacle, ref groundAngle))
            {
                distToMove = 0.0f;
                //TurnbackStart();
                Think(ThinkCause.CantGoFuther,1);
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

    void ActionDie()
    {
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
    bool TurningBack()
    {
        return IsInAction(Action.TurnbackLeft) || IsInAction(Action.TurnbackRight);
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
