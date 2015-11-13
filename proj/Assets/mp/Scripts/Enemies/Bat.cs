using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour
{
    float quaverDuration = 3f;
    float quaverTime = 0;
    Vector3 quaverStartPos;
    Vector3 quaverPos = new Vector3(0f, 0f, 0f);
    bool quavering = false;

    float TurnbackDuration = 0.25f;

    void Awake()
    {
        StaticInit();
        myGfx = transform.Find("gfx");
        myAnimator = myGfx.GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        //quaverBegin();
        lastPos = transform.position;
        velocity = new Vector2(0f, 0f);
        lastVelocity = new Vector2(0f, 0f);
        newVelocity = new Vector2(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        currentDeltaTime = Time.deltaTime;
        currentActionTime += currentDeltaTime;

        lastVelocity = velocity;
        lastPos = transform.position;

        switch (action)
        {
            case Action.Turnback:
                ActionTurnback();
                break;
        }

        if (quavering) QuaverStep();
        else QuaverBegin();

        CalculateVelocity();
        
        if( !TurnAccordingVelocity() )
        {
            //SetAnimatorSpeedAccordingVelocity();
        }

        if (IsInAction(Action.Fly))
        {
            SetAnimatorSpeedAccordingVelocity();
        }
    }

    void CalculateVelocity()
    {
        _pos = transform.position - lastPos;
        velocity = _pos / currentDeltaTime;
    }
    void SetAnimatorSpeedAccordingVelocity()
    {
        //float velocityDiff = velocity.magnitude - lastVelocity.magnitude;
        float velocityDiff = 0f;
        if( velocity.x > 0f )
        {
            velocityDiff += (velocity.x-lastVelocity.x);
        }
        else
        {
            velocityDiff += (lastVelocity.x-velocity.x);
        }
        //velocityDiff += (velocity.x-lastVelocity.x);
        velocityDiff += (velocity.y - lastVelocity.y);
        if (velocityDiff > 0)
        {
            myAnimator.speed = Mathf.Min(myAnimator.speed + velocityDiff, 3f);
        }
        else
        {
            myAnimator.speed = Mathf.Max(myAnimator.speed + velocityDiff, 1.0f);
        }
        print(velocityDiff + " " + myAnimator.speed);
        //myAnimator.speed = 3f;
    }
    bool TurnAccordingVelocity()
    {
        if (velocity.x > 0f)
        {
            if (FaceLeft())
            {
                TurnbackStart();
                return true;
            }
        }
        else if (velocity.x < 0f)
        {
            if (FaceRight())
            {
                TurnbackStart();
                return true;
            }
        }

        return false;
    }
    void QuaverBegin()
    {
        quavering = true;
        quaverTime = 0f;
        quaverStartPos = transform.position;

        //print( myAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash );
        //print(flyAnimStateHash);
    }

    void QuaverFinish()
    {
        quavering = false;
    }

    void QuaverStep()
    {
        bool _quaverEnded = false;
        quaverTime += currentDeltaTime;
        if (quaverTime >= quaverDuration)
        {
            quaverTime = quaverDuration;
            _quaverEnded = true;
        }

        float timeRatio = quaverTime / quaverDuration;

        quaverPos.x = Mathf.Sin(timeRatio * Mathf.PI * 2) * 3.0f;
        quaverPos.y = Mathf.Sin(timeRatio * Mathf.PI * 4) * 0.5f;

        transform.position = quaverStartPos + quaverPos;

        if (_quaverEnded)
            QuaverFinish();
    }

    Transform myGfx = null;
    Animator myAnimator = null;

    static bool staticInitiated = false;
    static int flyAnimStateHash;
    static int turnbackAnimStateHash;
    static int attackAnimStateHash;

    static bool StaticInit()
    {
        if (staticInitiated) return false;

        flyAnimStateHash = Animator.StringToHash("Fly");
        turnbackAnimStateHash = Animator.StringToHash("Turnback");
        attackAnimStateHash = Animator.StringToHash("Attack");

        staticInitiated = true;
        return true;
    }

    public enum Action
    {
        Undef = 0,
        Fly,
        Turnback,
        Attack
    };

    Action action;

    bool SetAction(Action newAction, int param = 0)
    {
        if (action == newAction)
            return false;

        actionChangedPos = transform.position;
        currentActionTime = 0f;
        actionJustChanged = true;
        action = newAction;

        myAnimator.speed = 1f;

        //print(action);

        //myAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        switch (action)
        {
            case Action.Fly:
                myAnimator.Play(flyAnimStateHash);
                break;

            case Action.Turnback:
                myAnimator.Play(turnbackAnimStateHash);
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

    void ActionTurnback()
    {
        if (currentActionTime >= TurnbackDuration)
        {
            Turnback();
            SetAction(Action.Fly);
        }
    }

    void TurnbackStart()
    {
        SetAction(Action.Turnback);
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
    public Vector2 DirN()
    {
        return myGfx.localScale.x < 0.0f ? -Vector2.right : Vector2.right;
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

    Vector2 velocity = new Vector2(0f, 0f);
    Vector2 lastVelocity = new Vector2(0f, 0f);
    Vector2 newVelocity = new Vector2(0f, 0f);
    Vector3 lastPos = new Vector3(0f, 0f, 0f);
    Vector3 _pos = new Vector3(0f, 0f, 0f);

    Vector3 actionChangedPos = new Vector3(0f, 0f, 0f);
    float currentDeltaTime = 0.0f;
    float currentActionTime = 0.0f;
    bool actionJustChanged = false;
}
