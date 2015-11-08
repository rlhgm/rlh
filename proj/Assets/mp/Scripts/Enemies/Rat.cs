using UnityEngine;
using System.Collections;

public class Rat : MonoBehaviour
{
    public int ControlID = 0;

    public float WalkSpeed = 2f;
    public float RunSpeed = 5f;

    public float IdleInDuration = 0.25f;
    public float IdleOutDuration = 0.25f;
    public float TurnbackDuration = 0.5f;

    // Use this for initialization
    void Awake()
    {
        staticInit();
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
        staticInit();
    }

    void Start()
    {
        setState(State.ON_GROUND);
        setAction(Action.RUN_LEFT);

        //zap = RLHScene.Instance.Zap;
        //if (zap == null)
        //{
        //    zap = FindObjectOfType<Zap>();
        //}
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
                    setState(State.IN_AIR);
                    setAction(Action.UNDEF);
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
                turnbackStart(Action.NEXT_WALK);
            }
        }
    }

    public enum Mode
    {
        UNDEF = 0,
        NORMAL,
        FIGHT
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
    
    public bool setState(State newState)
    {
        if (state == newState)
            return false;

        currentStateTime = 0f;
        stateJustChanged = true;
        state = newState;

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

    bool setAction(Action newAction, int param = 0)
    {
        if (action == newAction)
            return false;
        
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

    public bool isInAction(Action test)
    {
        return action == test;
    }

    public bool isNotInAction(Action test)
    {
        return action != test;
    }

    bool setMode(Mode newMode, int param = 0)
    {
        if (mode == newMode)
            return false;

        currentModeTime = 0f;
        modeJustChanged = true;
        mode = newMode;

        switch (mode)
        {
            case Mode.NORMAL:
                break;

            case Mode.FIGHT:
                break;
        }

        return true;
    }

    public bool isInMode(Mode test)
    {
        return mode == test;
    }

    public bool isNotInMode(Mode test)
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

    static bool staticInit()
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
            setAction(Action.IDLE);
    }
    void Action_IDLE_OUT()
    {
        if (currentActionTime >= IdleInDuration)
            setAction(nextAction);
    }
    void Action_WALK(int moveDir)
    {
        //bool speedReached = checkSpeed(dir);
        //if (speedReached && desiredSpeedX == 0.0f)
        //{
        //    setAction(Action.IDLE);
        //    resetActionAndState();
        //    return 0;
        //}

        //float distToMove = velocity.x * currentDeltaTime;
        distToMove = WalkSpeed * /*moveDir **/ currentDeltaTime;

        //zap.AnimatorBody.speed = 0.5f + (Mathf.Abs(zap.velocity.x) / WalkSpeed) * 0.5f;

        if (RLHScene.Instance.checkObstacle(mySensor.position, dir(), distToMove + myHalfSize.x, ref distToObstacle, 45f))
        {
            distToMove = distToObstacle - myHalfSize.x;
            //setAction(Action.IDLE_IN);
            turnbackStart();
            //nextAction = faceRight() ? Action.WALK_LEFT : Action.WALK_RIGHT;
        }
        else
        {
            Vector2 testGroundPos = mySensor.position;
            testGroundPos.x += ( (distToMove+0.5f) * moveDir);

            if (!RLHScene.Instance.checkGround(testGroundPos, 1.0f, ref distToObstacle, ref groundAngle))
            {
                distToMove = 0.0f;
                turnbackStart();                
            }
        }

        newPos.x += (distToMove * moveDir);
        transform.position = newPos;

        //return false;
    }
    void Action_RUN(int moveDir)
    {
        //bool speedReached = checkSpeed(dir);
        //if (speedReached && desiredSpeedX == 0.0f)
        //{
        //    setAction(Action.IDLE);
        //    resetActionAndState();
        //    return 0;
        //}

        //float distToMove = velocity.x * currentDeltaTime;
        distToMove = RunSpeed * /*moveDir **/ currentDeltaTime;

        //zap.AnimatorBody.speed = 0.5f + (Mathf.Abs(zap.velocity.x) / WalkSpeed) * 0.5f;

        if (RLHScene.Instance.checkObstacle(mySensor.position, dir(), distToMove+myHalfSize.x, ref distToObstacle, 45f))
        {
            distToMove = distToObstacle - myHalfSize.x;
            //setAction(Action.IDLE_IN);
            turnbackStart();
            //nextAction = faceRight() ? Action.RUN_LEFT : Action.RUN_RIGHT;
        }
        else
        {
            Vector2 testGroundPos = mySensor.position;
            testGroundPos.x += ((distToMove + 0.5f) * moveDir);

            if (!RLHScene.Instance.checkGround(testGroundPos, 1.0f, ref distToObstacle, ref groundAngle))
            {
                distToMove = 0.0f;
                turnbackStart();
            }
        }

        newPos.x += (distToMove * moveDir);
        transform.position = newPos;

        //return false;
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
            turnback();
            switch (nextAction)
            {
                case Action.NEXT_IDLE:
                    setAction(Action.IDLE_IN);
                    break;

                case Action.NEXT_WALK:
                    walkStart();
                    break;

                case Action.NEXT_RUN:
                    runStart();
                    break;

                default:
                    setAction(nextAction);
                    break;
            }
        }
    }

    void Action_DIE()
    {

    }
    
    void walkStart()
    {
        if (faceRight())
            setAction(Action.WALK_RIGHT);
        else
            setAction(Action.WALK_LEFT);
    }

    void runStart()
    {
        if (faceRight())
            setAction(Action.RUN_RIGHT);
        else
            setAction(Action.RUN_LEFT);
    }

    void turnbackStart(Action next = Action.UNDEF)
    {
        if(next == Action.UNDEF)
        {
            if (walking())
            {
                //nextAction = Action.NA_WALK;
                nextAction = faceRight() ? Action.WALK_LEFT : Action.WALK_RIGHT;
            }
            else if (running())
            {
                //nextAction = Action.NA_RUN;
                nextAction = faceRight() ? Action.RUN_LEFT : Action.RUN_RIGHT;
            }
        }
        else
        {
            nextAction = next;
        }

        if (faceRight())
            setAction(Action.TURNBACK_LEFT);
        else
            setAction(Action.TURNBACK_RIGHT);
    }
    void turnback()
    {
        //Vector3 scl = myGfx.localScale;
        //scl.x = scl.x * -1.0f;
        //myGfx.localScale = scl;
        if (faceRight()) turnLeft();
        else turnRight();
    }
    public void turnLeft()
    {
        Vector3 scl = myGfx.localScale;
        scl.x = Mathf.Abs(scl.x) * 1.0f;
        myGfx.localScale = scl;
    }
    public void turnRight()
    {
        Vector3 scl = myGfx.localScale;
        scl.x = Mathf.Abs(scl.x) * -1.0f;
        myGfx.localScale = scl;
    }

    public Vector2 dir()
    {
        return myGfx.localScale.x < 0.0f ? Vector2.right : -Vector2.right;
    }
    public int dir2()
    {
        return myGfx.localScale.x < 0f ? (int)1f : (int)-1f;
    }

    public bool faceLeft()
    {
        return myGfx.localScale.x > 0f;
    }
    public bool faceRight()
    {
        return myGfx.localScale.x < 0f;
    }
    
    bool walking()
    {
        return isInAction(Action.WALK_LEFT) || isInAction(Action.WALK_RIGHT);
    }
    bool running()
    {
        return isInAction(Action.RUN_LEFT) || isInAction(Action.RUN_RIGHT);
    }
    bool moving()
    {
        return walking() || running();
    }
}
