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

        SetMode(Mode.NORMAL);
        SetState(State.ON_GROUND);
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

        switch (action)
        {
            case Action.IDLE:
                Action_IDLE();
                break;
            case Action.IDLE_IN:
                Action_IDLE_IN();
                break;
            case Action.IDLE_OUT:
                Action_IDLE_OUT();
                break;

            case Action.WALK_LEFT:
                Action_WALK(-1);
                break;
            case Action.WALK_RIGHT:
                Action_WALK(1);
                break;

            case Action.RUN_LEFT:
                Action_RUN(-1);
                break;
            case Action.RUN_RIGHT:
                Action_RUN(1);
                break;

            case Action.TURNBACK_LEFT:
                Action_TURNBACK_LEFT();
                break;
            case Action.TURNBACK_RIGHT:
                Action_TURNBACK_RIGHT();
                break;

            case Action.DIE:
                Action_DIE();
                break;
        }

        switch (state)
        {
            case State.ON_GROUND:
                if (RLHScene.Instance.checkGround(mySensor.position, 1.0f, ref distToObstacle, ref groundAngle))
                {
                    newPos.y += (myHalfSize.y - distToObstacle);
                    //transform.rotation.z = groundAngle
                    transform.position = newPos;
                }
                else
                {
                    SetState(State.IN_AIR);
                    SetAction(Action.UNDEF);
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
                TurnbackStart(/*Action.NEXT_WALK*/);
            }
        }
    }

    public enum Mode
    {
        UNDEF = 0,
        NORMAL,
        FIGHT_CHASE_TARGET,
        FIGHT_CANT_REACH_TARGET
    };

    public enum State
    {
        UNDEF = 0,
        ON_GROUND,
        IN_AIR,
        DEAD,
    };

    public enum Action
    {
        UNDEF = 0,
        IDLE,
        IDLE_IN,
        IDLE_OUT,

        WALK_LEFT,
        WALK_RIGHT,
        RUN_LEFT,
        RUN_RIGHT,
        TURNBACK_LEFT,
        TURNBACK_RIGHT,
        
        DIE,

        // next actions:
        NEXT_IDLE,
        NEXT_WALK,
        NEXT_RUN
    };
    
    public float CurrentStateTime
    {
        get { return currentStateTime; }
    }

    Mode mode;
    State state;
    Action action;
    //Action nextAction;
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
            case Action.IDLE:
                myAnimator.Play(idleAnimStateHash);
                break;

            case Action.IDLE_IN:
                myAnimator.Play(idleInAnimStateHash);
                break;

            case Action.IDLE_OUT:
                myAnimator.Play(idleOutAnimStateHash);
                break;

            case Action.WALK_LEFT:
                myAnimator.Play(walkAnimStateHash);
                break;

            case Action.WALK_RIGHT:
                myAnimator.Play(walkAnimStateHash);
                break;

            case Action.RUN_LEFT:
                myAnimator.Play(runAnimStateHash);
                break;

            case Action.RUN_RIGHT:
                myAnimator.Play(runAnimStateHash);
                break;

            case Action.TURNBACK_LEFT:
                myAnimator.Play(turnbackAnimStateHash);
                break;

            case Action.TURNBACK_RIGHT:
                myAnimator.Play(turnbackAnimStateHash);
                break;

            case Action.DIE:
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

    void Think()
    {
        Vector3 targetPos = RLHScene.Instance.Zap.transform.position;
        float distToTarget = Vector3.Distance(transform.position, targetPos);

        switch (mode)
        {
            case Mode.NORMAL:
                if (distToTarget < FightModeDistance)
                {
                    SetMode(Mode.FIGHT_CHASE_TARGET);

                    if( transform.position.x < targetPos.x)
                    {
                        if (FaceLeft()) TurnbackStart();
                    }
                    else
                    {
                        if (FaceRight()) TurnbackStart();
                    }
                }
                else
                {

                }
                break;

           case Mode.FIGHT_CHASE_TARGET:
                break;
        }
    }

    Action Think_MustTurnBack()
    {

        return Action.UNDEF;
    }
    //void determineNext()
    //{

    //}

    bool SetMode(Mode newMode, int param = 0)
    {
        if (mode == newMode)
            return false;

        modeChangedPos = transform.position;
        currentModeTime = 0f;
        modeJustChanged = true;
        mode = newMode;

        switch (mode)
        {
            case Mode.NORMAL:
                break;

            //case Mode.FIGHT:
            //    break;
        }

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
            
    void Action_IDLE()
    {

    }
    void Action_IDLE_IN()
    {
        if (currentActionTime >= IdleInDuration)
        {
            SetAction(Action.IDLE);
        }
    }
    void Action_IDLE_OUT()
    {
        if (currentActionTime >= IdleInDuration)
        {
            //setAction(nextAction);
            Think();
        }
    }
    void Action_WALK(int moveDir)
    {
        distToMove = WalkSpeed * currentDeltaTime;
        
        if (RLHScene.Instance.checkObstacle(mySensor.position, Dir(), distToMove + myHalfSize.x, ref distToObstacle, 45f))
        {
            distToMove = distToObstacle - myHalfSize.x;
            TurnbackStart();
        }
        else
        {
            Vector2 testGroundPos = mySensor.position;
            testGroundPos.x += ( (distToMove+0.5f) * moveDir);

            if (!RLHScene.Instance.checkGround(testGroundPos, 1.0f, ref distToObstacle, ref groundAngle))
            {
                distToMove = 0.0f;
                TurnbackStart();                
            }
        }

        newPos.x += (distToMove * moveDir);
        transform.position = newPos;
    }
    void Action_RUN(int moveDir)
    {
        distToMove = RunSpeed * currentDeltaTime;

        if (RLHScene.Instance.checkObstacle(mySensor.position, Dir(), distToMove+myHalfSize.x, ref distToObstacle, 45f))
        {
            distToMove = distToObstacle - myHalfSize.x;
            TurnbackStart();
        }
        else
        {
            Vector2 testGroundPos = mySensor.position;
            testGroundPos.x += ((distToMove + 0.5f) * moveDir);

            if (!RLHScene.Instance.checkGround(testGroundPos, 1.0f, ref distToObstacle, ref groundAngle))
            {
                distToMove = 0.0f;
                TurnbackStart();
            }
        }

        newPos.x += (distToMove * moveDir);
        transform.position = newPos;
    }
    void Action_TURNBACK_LEFT()
    {
        Action_TURNBACK();
    }
    void Action_TURNBACK_RIGHT()
    {
        Action_TURNBACK();
    }
    void Action_TURNBACK()
    {
        if (currentActionTime >= TurnbackDuration)
        {
            Turnback();
            //switch (nextAction)
            //{
            //    case Action.NEXT_IDLE:
            //        setAction(Action.IDLE_IN);
            //        break;

            //    case Action.NEXT_WALK:
            //        walkStart();
            //        break;

            //    case Action.NEXT_RUN:
            //        runStart();
            //        break;

            //    default:
            //        setAction(nextAction);
            //        break;
            //}
            Think();
        }
    }

    void Action_DIE()
    {
    }
    
    void WalkStart()
    {
        if (FaceRight())
            SetAction(Action.WALK_RIGHT);
        else
            SetAction(Action.WALK_LEFT);
    }

    void RunStart()
    {
        if (FaceRight())
            SetAction(Action.RUN_RIGHT);
        else
            SetAction(Action.RUN_LEFT);
    }

    void TurnbackStart(/*Action next = Action.UNDEF*/)
    {
        //if(next == Action.UNDEF)
        //{
        //    if (walking())
        //    {
        //        nextAction = faceRight() ? Action.WALK_LEFT : Action.WALK_RIGHT;
        //    }
        //    else if (running())
        //    {
        //        nextAction = faceRight() ? Action.RUN_LEFT : Action.RUN_RIGHT;
        //    }
        //}
        //else
        //{
        //    nextAction = next;
        //}

        if (FaceRight())
            SetAction(Action.TURNBACK_LEFT);
        else
            SetAction(Action.TURNBACK_RIGHT);
    }
    void Turnback()
    {
        //Vector3 scl = myGfx.localScale;
        //scl.x = scl.x * -1.0f;
        //myGfx.localScale = scl;
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
        return IsInAction(Action.WALK_LEFT) || IsInAction(Action.WALK_RIGHT);
    }
    bool Running()
    {
        return IsInAction(Action.RUN_LEFT) || IsInAction(Action.RUN_RIGHT);
    }
    bool Moving()
    {
        return Walking() || Running();
    }
}
