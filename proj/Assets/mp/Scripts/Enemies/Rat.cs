using UnityEngine;
using System.Collections;

public class Rat : MonoBehaviour
{ 
    // Use this for initialization
    void Awake()
    {
        staticInit();
        myAnimator = GetComponent<Animator>();
    }
    void Start()
    {
        setState(State.ON_GROUND);
        setAction(Action.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        stateJustChanged = false;
        currentStateTime += dt;
        currentActionTime += dt;

        //oldPos = transform.position;
        //newPosX = oldPos.x;
        //distToMove = 0.0f;

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
                Action_WALK_LEFT();
                break;
            case Action.WALK_RIGHT:
                Action_WALK_RIGHT();
                break;
            case Action.RUN_LEFT:
                Action_RUN_LEFT();
                break;
            case Action.RUN_RIGHT:
                Action_RUN_RIGHT();
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
    }

    void OnTriggerEnter2D(Collider2D other)
    {

    }

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
        
        DIE
    };

    public float CurrentStateTime
    {
        get { return currentStateTime; }
    }

    State state;
    Action action;
    float currentStateTime = 0.0f;
    float currentActionTime = 0.0f;
    bool stateJustChanged = false;
    bool actionJustChanged = false;
    Animator myAnimator;

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

        switch (newAction)
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

    }
    void Action_IDLE_OUT()
    {

    }
    void Action_WALK_LEFT()
    {

    }
    void Action_WALK_RIGHT()
    {

    }
    void Action_RUN_LEFT()
    {

    }
    void Action_RUN_RIGHT()
    {

    }
    void Action_TURNBACK_LEFT()
    {

    }
    void Action_TURNBACK_RIGHT()
    {

    }
    void Action_DIE()
    {

    }
}
