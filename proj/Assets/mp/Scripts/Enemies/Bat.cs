﻿using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour
{
    BatActivator activator;

    Vector3 homePos;

    float quaverDuration = 3f;
    float quaverTime = 0;
    Vector3 quaverStartPos;
    Vector3 quaverPos = new Vector3(0f, 0f, 0f);
    Vector2 quaverRange = new Vector2(0f, 0f);
    Vector2 quaveringXY = new Vector2(0f, 0f);
    bool quavering = false;
    float QuaverToZapSpeed = 0.5f;
    float QuaverToHomeSpeed = 1.0f;
    //Vector2 FlyToTargetSpeedRange = new Vector2(2f,4f);
    //float FlyToTargetSpeed = 0f;
    //bool _gftTryIterrupt = false;

    //bool flyingToTarget = false;

    float TurnbackDuration = 0.25f;

    bool searchBed = false;
    bool bedFound = false;

    void Awake()
    {
        StaticInit();
        myGfx = transform.Find("gfx");
        myAnimator = myGfx.GetComponent<Animator>();

    }

    // Use this for initialization
    void Start()
    {
        if (!Activator)
        {
            Debug.LogError("Nietoperz : " + name + " nie jest przypisany do zadnego aktywatora!");
        }

        SetState(State.Sleep);
        SetAction(Action.GoSleep);
        //SetState(State.Fly);
        //SetAction(Action.Fly);

        homePos = transform.position;
        lastPos = transform.position;
        velocity = new Vector2(0f, 0f);
        lastVelocity = new Vector2(0f, 0f);
        newVelocity = new Vector2(0f, 0f);

        if (Random.Range(0, 2) == 1)
            Turnback();
    }

    // Update is called once per frame
    void Update()
    {
        currentDeltaTime = Time.deltaTime;
        currentActionTime += currentDeltaTime;
        currentStateTime += currentDeltaTime;

        actionJustChanged = false;
        stateJustChanged = false;

        lastVelocity = velocity;
        lastPos = transform.position;

        //if( Activator.ZapIn )
        targetPos = RLHScene.Instance.Zap.Targeter.position;

        switch (state)
        {
            case State.Fly:
                switch (action)
                {
                    case Action.Turnback:
                        ActionTurnback();
                        break;
                }

                if (IsNotInAction(Action.Turnback))
                {
                    if (quavering) QuaverStep();
                    else
                    {
                        //if (flyingToTarget) FlyToTargetStep();
                        //else
                            QuaverBegin();
                    }
                }

                CalculateVelocity();

                if (!TurnAccordingVelocity())
                {
                    //SetAnimatorSpeedAccordingVelocity();
                }

                if (IsInAction(Action.Fly))
                {
                    SetAnimatorSpeedAccordingVelocity();
                }

                if (!Activator.ZapIn)
                {
                    if (currentStateTime >= snoozeDuration)
                    {
                        if (!searchBed)
                        {
                            //flyingToTarget = false;
                            searchBed = true;
                            //print("szukam domu");
                        }
                    }
                }
                break;

            case State.Sleep:
                if (currentStateTime >= snoozeDuration)
                {
                    //SetState(State.WakeUp);
                    SetState(State.WakeUp);
                    SetAction(Action.Fly);

                    quaveringXY.x = FaceRight() ? 1 : -1;
                    quaveringXY.y = -1;// FaceRight() ? 1 : -1;

                    quavering = true;
                    quaverTime = 0f;
                    quaverStartPos = transform.position;

                    quaverRange.x = Random.Range(1.0f, 2.0f);
                    quaverRange.y = Random.Range(1.0f, 2.5f);
                    quaverDuration = quaverRange.x - Random.Range(0f, 0.5f);
                }
                break;

            case State.WakeUp:
                QuaverStep();
                if (currentStateTime > (quaverDuration * 0.125f))
                {
                    SetState(State.Fly);
                    //if (Activator.ZapIn)
                    //{
                    //    float distToZapX = transform.position.x - targetPos.x;
                    //    if (Mathf.Abs(distToZapX) > 2.5f)
                    //    {
                    //        flyingToTargetPos = transform.position;

                    //        if (distToZapX > 0.0f) // jezeli zap na lewo ode mnie
                    //        {
                    //            flyingToTargetPos.x = targetPos.x + Random.Range(0f, 2.5f);
                    //        }
                    //        else
                    //        {
                    //            flyingToTargetPos.x = targetPos.x - Random.Range(0f, 2.5f);
                    //        }

                    //        FlyToTargetBegin();
                    //    }
                    //    else
                    //    {
                    //        QuaverBegin(true);
                    //    }
                    //}
                    //else
                    //{
                        QuaverBegin(true);
                    //}
                }
                break;
        }
    }

    public void ZapIsHere()
    {
        //print(name + " zapIsHere()");
        searchBed = false;
        bedFound = false;
        if (IsInState(State.Sleep))
        {
            //else if (param == 1) // krotka drzemka aby nie wszyskie wystartoway razem
            //{
            snoozeDuration = Random.Range(0f, 1f);
            currentStateTime = 0f;
            //    print(name + " snoozeDuration " + snoozeDuration);
            //}

            //SetState(State.Sleep,1);
        }
    }
    public void ZapEscape()
    {
        searchBed = false;
    }

    void CalculateVelocity()
    {
        _pos = transform.position - lastPos;
        velocity = _pos / currentDeltaTime;
    }
    void SetAnimatorSpeedAccordingVelocity()
    {
        // tu wersja dla przyspieszenia po x i y w gore....
        //float velocityDiff = velocity.magnitude - lastVelocity.magnitude;

        float velocityDiff = 0f;
        if (velocity.x > 0f)
        {
            velocityDiff += (velocity.x - lastVelocity.x);
        }
        else
        {
            velocityDiff += (lastVelocity.x - velocity.x);
        }
        //velocityDiff += (velocity.x-lastVelocity.x);
        velocityDiff += (velocity.y - lastVelocity.y);

        // tu zrobie wersje ze jak predkosc nie zmienia sie to macha coraz wolniej... i odwrotnie
        //float velocityDiff = velocity.magnitude - lastVelocity.magnitude;

        //velocityDiff = Mathf.Abs(velocityDiff)

        if (velocityDiff > 0)
        {
            myAnimator.speed = Mathf.Min(myAnimator.speed + velocityDiff, 3.0f);
        }
        else
        {
            myAnimator.speed = Mathf.Max(myAnimator.speed + velocityDiff * 2, 1.0f);
        }

        //print(velocity + " " + velocityDiff + " " + myAnimator.speed);
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

    //void FlyToTargetBegin()
    //{
    //    print("FlyToTargetBegin() " + flyingToTargetPos);
    //    flyingToTarget = true;
    //    //actionChangedPos = transform.position;
    //    stateChangedPos = transform.position;
    //    FlyToTargetSpeed = Random.Range(FlyToTargetSpeedRange.x, FlyToTargetSpeedRange.y);
    //    if( stateChangedPos.x > flyingToTargetPos.x )
    //    {
    //        FlyToTargetSpeed *= -1f;
    //    }
    //    print("FlyToTargetSpeed " + FlyToTargetSpeed);

    //    // uzywam quaverStartPos bo teraz niepotrzebne a szkoda robic nowy wektor tylko po to
    //    //quaverStartPos = flyingToTargetPos - stateChangedPos;
    //}
    //void FlyToTargetFinish()
    //{
    //    print("FlyToTargetFinish()");
    //    flyingToTarget = false;
    //    QuaverBegin(true);
    //}
    //void FlyToTargetStep()
    //{
    //    quaverPos.x = 0f;
    //    quaverPos.y = 0f;
    //    quaverPos.x = FlyToTargetSpeed * currentDeltaTime; 
    //    transform.position += quaverPos;

    //    if( stateChangedPos.x > flyingToTargetPos.x )
    //    {
    //        if (transform.position.x < flyingToTargetPos.x) // przelecial za target juz
    //        {
    //            FlyToTargetFinish();
    //        }
    //    }
    //    else
    //    {
    //        if (transform.position.x > flyingToTargetPos.x) // przelecial za target juz
    //        {
    //            FlyToTargetFinish();
    //        }
    //    }
    //}

    void QuaverBegin(bool keepDirX = false)
    {
        //quavering = Random.Range(0,2) == 1 ? 1 : -1;
        if (!keepDirX)
            quaveringXY.x = Random.Range(0, 2) == 1 ? 1 : -1;
        quaveringXY.y = Random.Range(0, 2) == 1 ? 1 : -1;
        quavering = true;
        quaverTime = 0f;
        quaverStartPos = transform.position;

        if (searchBed)
        {
            quaverRange.x = Random.Range(1.5f, 2.5f);
            quaverRange.y = Random.Range(0.1f, 0.5f);
        }
        else if (bedFound)
        {
            quaverRange.x = Random.Range(1.0f, 2f);
            quaverRange.y = Random.Range(0.1f, 0.25f);
        }
        else
        {
            quaverRange.x = Random.Range(2.5f, 4f);
            quaverRange.y = Random.Range(0.1f, 0.75f);
        }
        quaverDuration = quaverRange.x - Random.Range(0f, 1f);

        //print( myAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash );
        //print(flyAnimStateHash);
    }

    void QuaverFinish()
    {

        //_gftTryIterrupt = false;
        quaveringXY.x = 0f;
        quaveringXY.y = 0f;
        quavering = false;
    }

    void QuaverStep()
    {
        if (searchBed)
        {
            Vector3 toTarget = homePos - quaverStartPos;
            Vector3 distToDisplace = (toTarget.normalized * QuaverToHomeSpeed * currentDeltaTime);
            if (distToDisplace.magnitude > toTarget.magnitude)
            {
                if (!bedFound)
                {
                    //print("znalazlem");
                    quaverStartPos = homePos;
                    bedFound = true;
                }
            }
            else
            {
                quaverStartPos += distToDisplace;
            }
        }
        else if (Activator.ZapIn)
        {
            Vector3 toZap = targetPos - quaverStartPos;
            //Vector3 distToDisplace = (toZap.normalized * QuaverToZapSpeed * currentDeltaTime);
            //if (distToDisplace.magnitude > toZap.magnitude)
            //{
            //    //if (!bedFound)
            //    //{
            //    //    //print("znalazlem");
            //    //    quaverStartPos = homePos;
            //    //    bedFound = true;
            //    //}
            //}
            //else
            //{
            //    quaverStartPos += distToDisplace;
            //}
            quaverStartPos += (toZap.normalized * QuaverToZapSpeed * currentDeltaTime);
        }

        bool _quaverEnded = false;
        quaverTime += currentDeltaTime;
        if (quaverTime >= quaverDuration)
        {
            quaverTime = quaverDuration;
            _quaverEnded = true;
        }

        Vector2 timeRatioXY = new Vector2();
        if (quaveringXY.x == 1f)
            timeRatioXY.x = quaverTime / quaverDuration;
        else
            timeRatioXY.x = 1f - (quaverTime / quaverDuration);

        if (quaveringXY.y == 1f)
            timeRatioXY.y = quaverTime / quaverDuration;
        else
            timeRatioXY.y = 1f - (quaverTime / quaverDuration);

        //if( !_gftTryIterrupt && timeRatio > .25f)
        //{
        //    _gftTryIterrupt = true;

        //    if (Random.Range(0, 2) == 1)
        //    {
        //        QuaverFinish();
        //        return;
        //    }
        //}
        quaverPos.x = Mathf.Sin(timeRatioXY.x * Mathf.PI * 2) * quaverRange.x;
        quaverPos.y = Mathf.Sin(timeRatioXY.y * Mathf.PI * 4) * quaverRange.y;

        transform.position = quaverStartPos + quaverPos;

        if (_quaverEnded)
        {
            if (bedFound)
            {
                //print("ide spac");
                QuaverFinish();
                transform.position = homePos;
                SetState(State.Sleep);
                SetAction(Action.GoSleep);
                bedFound = false;
                searchBed = false;
                //return;
            }
            else
            {
                QuaverFinish();
            }
        }
    }

    Transform myGfx = null;
    Animator myAnimator = null;

    static bool staticInitiated = false;
    static int flyAnimStateHash;
    static int turnbackAnimStateHash;
    static int attackAnimStateHash;
    static int goSleepAnimStateHash;

    static bool StaticInit()
    {
        if (staticInitiated) return false;

        flyAnimStateHash = Animator.StringToHash("Fly");
        turnbackAnimStateHash = Animator.StringToHash("Turnback");
        attackAnimStateHash = Animator.StringToHash("Attack");
        goSleepAnimStateHash = Animator.StringToHash("GoSleep");

        staticInitiated = true;
        return true;
    }

    public enum Action
    {
        Undef = 0,
        Fly,
        Turnback,
        //WokeUp,
        Attack,
        GoSleep
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

            case Action.GoSleep:
                myAnimator.Play(goSleepAnimStateHash);
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

    public enum State
    {
        Undef = 0,
        Fly,
        Sleep,
        WakeUp,
        Bunk
    }
    State state;
    Vector3 stateChangedPos = new Vector3(0f, 0f, 0f);
    float currentStateTime = 0.0f;
    bool stateJustChanged = false;

    public bool SetState(State newState, int param = 0)
    {
        if (state == newState)
            return false;

        stateChangedPos = transform.position;
        currentStateTime = 0f;
        stateJustChanged = true;
        state = newState;

        switch (state)
        {
            case State.Sleep:
                //if (param == 0)
                //{
                snoozeDuration = Random.Range(3f, 7f);
                //}
                //else if( param == 1) // krotka drzemka aby nie wszyskie wystartoway razem
                //{
                //    snoozeDuration = Random.Range(0f, 1f);
                //    print(name + " snoozeDuration " + snoozeDuration);
                //}
                //myGfx.Rotate(0f,0f,FaceLeft() ? 90.0f : -90.0f);
                //snoozeDuration = Random.Range(1f, 2f);
                break;
            case State.WakeUp:
                snoozeDuration = Random.Range(2f, 4f);
                //myGfx.Rotate(0f, 0f, FaceLeft() ? -90.0f : 90.0f);
                //snoozeDuration = Random.Range(1f, 2f);
                break;
        }
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

    Vector3 targetPos = new Vector3(0f, 0f, 0f);
    //Vector3 flyingToTargetPos = new Vector3(0f, 0f, 0f);

    float snoozeDuration = 5f;

    public BatActivator Activator
    {
        get
        {
            return activator;
        }

        set
        {
            activator = value;
        }
    }
}