using UnityEngine;
using System.Collections;

public class Bat : Enemy //MonoBehaviour
{
	AudioSource audio;
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
    float DiveInDuration = 0.4125f;

    bool searchBed = false;
    bool bedFound = false;

    void Awake()
    {
        StaticInit();
        myGfx = transform.Find("gfx");
        myAnimator = myGfx.GetComponent<Animator>();
        myGfx.GetComponent<SpriteRenderer>().enabled = true;
    }

    public override void Reset()
    {
        
        //gameObject.(true);
        GetComponent<BoxCollider2D>().enabled = true;
        myGfx.gameObject.SetActive(true);
        myGfx.GetComponent<SpriteRenderer>().enabled = true;

        if (!Activator)
        {
            Debug.LogError("Nietoperz : " + name + " nie jest przypisany do zadnego aktywatora!");
        }

        SetState(State.Sleep);
        SetAction(Action.GoSleep);
        
        transform.position = homePos;
        lastPos = transform.position;
        velocity = new Vector2(0f, 0f);
        lastVelocity = new Vector2(0f, 0f);
        newVelocity = new Vector2(0f, 0f);

       // print("Bat -> Reset -> " + homePos + " " + transform.position);

        if (Random.Range(0, 2) == 1)
            Turnback();


        searchBed = false;
        bedFound = false;
        quavering = false;
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

		audio = GetComponent<AudioSource>();
        homePos = transform.position;
        lastPos = transform.position;
        //print("Bat -> Start -> " + homePos);
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
        attackTargetPos = RLHScene.Instance.Zap.Targeter.position;
        quaverTargetPos = RLHScene.Instance.Zap.transform.position;
        quaverTargetPos.y += 3.5f;

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
                    if (TryStartDive())
                    {
                        //quav
                        //print("moge pikowac....");

                        SetState(State.DiveIn);
                        SetAction(Action.DiveIn);

                        //SetState(State.WakeUp);
                        //SetAction(Action.Fly);

                        quaveringXY.x = FaceRight() ? -1 : -1;
                        quaveringXY.y = -1;// FaceRight() ? 1 : -1;

                        quavering = true;
                        quaverTime = 0f;
                        quaverStartPos = transform.position;

                        //quaverRange.x = Random.Range(1.0f, 2.0f);
                        //quaverRange.y = Random.Range(1.0f, 2.5f);
                        //quaverDuration = quaverRange.x - Random.Range(0f, 0.5f);

                        //toDiveTargetDiff = transform.position - attackTargetPos;
                        Vector3 tdtd2 = toDiveTargetDiff.normalized;
                        //stateChangedPos

                        quaverRange.x = (toDiveTargetDiff.x+tdtd2.x) * 2f; 
                        quaverRange.y = (toDiveTargetDiff.y+tdtd2.y); 
                        quaverDuration = 4f;
                    }
                    else
                    {
                        if (quavering) QuaverStep();
                        else
                        {
                            //if (flyingToTarget) FlyToTargetStep();
                            //else

                            //if (tryStartDive())
                            //{
                            //}
                            //else
                            //{
                                QuaverBegin();
                            //}
                        }
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
                    SetAction(Action.DiveIn);

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
                CalculateVelocity();
                SetAnimatorSpeedAccordingVelocity();

                if( currentActionTime > DiveInDuration )
                {
                    //SetAction(Action.Dive);
                    print("setaction dive");
                    playAnim(diveAnimStateHash);
                }

                if (currentStateTime > (quaverDuration * 0.125f))
                {
                    SetState(State.Fly);
                    playAnim(flyAnimStateHash);
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

            case State.DiveIn:
                float stateRatio = Mathf.Min(currentStateTime / DiveInDuration, 1f);

                //toDiveTargetDiff = transform.position - attackTargetPos;
                Vector3 tdtd = toDiveTargetDiff.normalized;
                transform.position = stateChangedPos + tdtd * Mathf.Sin( stateRatio * (Mathf.PI * 0.5f) );

                if (stateRatio >= 1)
                {
                    SetState(State.Dive);
                    SetAction(Action.DiveIn);
                }
                else
                {

                }
                break;

            case State.Dive:
                //if (currentStateTime > DiveInDuration)
                //{
                //    //SetAction(Action.Dive);
                //    playAnim(diveAnimStateHash);
                //}
                
                QuaverStep();
                CalculateVelocity();
                SetAnimatorSpeedAccordingVelocity();

                if (currentStateTime > (quaverDuration * 0.125f))
                {
                    SetState(State.DiveOut);
                    SetAction(Action.Fly);
                    myAnimator.speed = 4f;
                    //QuaverBegin(true);
                }
                break;

            case State.DiveOut:
                QuaverStep();
                CalculateVelocity();
                SetAnimatorSpeedAccordingVelocity();

                if (currentStateTime > (quaverDuration * (0.125f*0.75f) ))
                {
                    //print("dive out finish");

                    SetState(State.Fly);
                    SetAction(Action.Fly);
                    //QuaverBegin(false);
                    
                    //SetState(State.DiveIn);
                    //SetAction(Action.DiveIn);

                    //SetState(State.WakeUp);
                    //SetAction(Action.Fly);

                    quaveringXY.x = FaceRight() ? 1 : -1;
                    quaveringXY.y = 1;// FaceRight() ? 1 : -1;

                    //print("dive out finish " + quaveringXY);

                    quavering = true;
                    quaverTime = 0f;
                    quaverStartPos = transform.position;

                    //quaverRange.x = Random.Range(1.0f, 2.0f);
                    //quaverRange.y = Random.Range(1.0f, 2.5f);
                    //quaverDuration = quaverRange.x - Random.Range(0f, 0.5f);

                    quaverRange.x = Mathf.Abs( toDiveTargetDiff.x * 1.5f );
                    quaverRange.y = toDiveTargetDiff.y * 0.25f;
                    quaverDuration = quaverRange.x - Random.Range(0f, 0.5f);


                }
                break;

            case State.SuckZap:
                transform.position = RLHScene.Instance.Zap.Targeter.position;
                break;
        }
    }

    public void ZapIsHere(bool forceWakeUp = false)
    {
        //print(name + " zapIsHere()");
        searchBed = false;
        bedFound = false;
        if (IsInState(State.Undef) && forceWakeUp)
        {
            SetState(State.Sleep);
        }

        if (IsInState(State.Sleep))
        {
            //else if (param == 1) // krotka drzemka aby nie wszyskie wystartoway razem
            //{
            if (!forceWakeUp)
            {
                snoozeDuration = Random.Range(0f, 1f);
            }
            else
            {
                snoozeDuration = Random.Range(0.11f, 2.51f);
                //Update();
            }
            currentStateTime = 0f;
            //    print(name + " snoozeDuration " + snoozeDuration);
            //}

            //SetState(State.Sleep,1);
        }
    }
    public void ZapEscape()
    {
        searchBed = true;
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

    public void StartSuckZap()
    {
        SetState(State.SuckZap);
        myGfx.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void StopSuckZap()
    {
        SetState(State.Fly);
        QuaverBegin(true);
        myGfx.GetComponent<SpriteRenderer>().enabled = true;
    }

    bool TryStartDive()
    {
        if (RLHScene.Instance.Zap.isDead()) return false;
        if (RLHScene.Instance.Zap.currentController == RLHScene.Instance.Zap.zapControllerSuckByBat) return false;

        toDiveTargetDiff = transform.position - attackTargetPos;

        //if (toDiveTargetDiff.x < 1.0f || toDiveTargetDiff.x > 2.0f) return false;
        if (toDiveTargetDiff.y < 2.0f || toDiveTargetDiff.y > 3.0f) return false;
        
        if( FaceRight() )
        {
            if (toDiveTargetDiff.x > 0) return false;
            if (toDiveTargetDiff.x > -1.5f || toDiveTargetDiff.x < -2.5f) return false;
        }
        else
        {
            if (toDiveTargetDiff.x < 0) return false;
            if (toDiveTargetDiff.x < 1.5f || toDiveTargetDiff.x > 2.5f) return false;
        }
        
        return true;
    }

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
        else if (Activator.ZapIn && IsNotInState(State.Dive) )
        {
            Vector3 toZap = quaverTargetPos - quaverStartPos;
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
    static int diveInAnimStateHash;
    static int diveAnimStateHash;

    static bool StaticInit()
    {
        if (staticInitiated) return false;

        flyAnimStateHash = Animator.StringToHash("Fly");
        turnbackAnimStateHash = Animator.StringToHash("Turnback");
        attackAnimStateHash = Animator.StringToHash("Attack");
        goSleepAnimStateHash = Animator.StringToHash("GoSleep");
        diveInAnimStateHash = Animator.StringToHash("DiveIn");
        diveAnimStateHash = Animator.StringToHash("Dive");

        staticInitiated = true;
        return true;
    }

    public enum Action
    {
        Undef = 0,
        Fly,
        //Quaver,
        Turnback,
        Attack,
        GoSleep,
        DiveIn,
        Dive,
        WokeUp
    };
    Action action;

    public enum State
    {
        Undef = 0,
        Fly,
        Sleep,
        WakeUp,
        DiveIn,
        Dive,
        DiveOut,
        //Bunk
        SuckZap,
        Dead
    }
    State state;

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
                //myAnimator.Play(flyAnimStateHash);
                playAnim(flyAnimStateHash);
                break;

            //case Action.Quaver:
            //    break;

            case Action.Turnback:
                //myAnimator.Play(turnbackAnimStateHash);
                playAnim(turnbackAnimStateHash);
                break;

            case Action.Attack:
                //myAnimator.Play(attackAnimStateHash);
                playAnim(attackAnimStateHash);
                break;

            case Action.GoSleep:
                //myAnimator.Play(goSleepAnimStateHash);
                playAnim(goSleepAnimStateHash);
                break;

            case Action.DiveIn:
                //myAnimator.Play(diveInAnimStateHash);
                playAnim(diveInAnimStateHash);
                break;

            case Action.Dive:
                //myAnimator.Play(diveAnimStateHash);
                playAnim(diveAnimStateHash);
                break;
        }

        return true;
    }

    void playAnim(int animStateHash, bool forceRestart = false)
    {
        if (!forceRestart && myAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == animStateHash) return;
        myAnimator.Play(animStateHash);
    }

    public bool IsInAction(Action test)
    {
        return action == test;
    }

    public bool IsNotInAction(Action test)
    {
        return action != test;
    }
    
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

        //print(state);

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

            case State.Fly:
                //snoozeDuration = Random.Range(2f, 4f);
                //myGfx.Rotate(0f, 0f, FaceLeft() ? -90.0f : 90.0f);
                //snoozeDuration = Random.Range(1f, 2f);
                int ttt = 0;
                break;

            case State.Dive:
                //toDiveTargetDiff;
                break;

            case State.SuckZap:
                playAnim(flyAnimStateHash);
                myAnimator.speed = 2f;
                break;

            case State.Dead:
                //gameObject.SetActive(false);
				audio.Stop();
                GetComponent<BoxCollider2D>().enabled = false;
                myGfx.gameObject.SetActive(false);
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

    void ActionDiveIn()
    {
        if( currentActionTime >= DiveInDuration)
        {
            //SetAction(Action.Dive);
        }
    }

    void ActionDive()
    {

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

    Vector3 quaverTargetPos = new Vector3(0f, 0f, 0f);
    Vector3 attackTargetPos = new Vector3(0f, 0f, 0f);
    Vector3 toDiveTargetDiff = new Vector3();
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

    public GameObject cutParticles = null;
    public EnemyRemain remainHead = null;
    public EnemyRemain remainBody = null;
    public EnemyRemain remainWings = null;

    public void cut()
    {
        if (IsInState(State.Dead)) return;

        if (RLHScene.Instance.ZapVsBats)
        {
            RLHScene.Instance.ZapVsBats.BatDead(this);
        }
        //print(name + " cutted");

        if (cutParticles)
        {
            Object newParticleObject = Instantiate(cutParticles, transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(newParticleObject, 2.0f);
        }
        //GetComponent<SpriteRenderer>().enabled = false;
        //GetComponent<Collider2D>().enabled = false;

        //, transform.position, Quaternion.Euler(0, 0, 0)
        //remainsHeadPos

        if (remainHead)
        {
            EnemyRemain newRemain = Instantiate<EnemyRemain>(remainHead);
            newRemain.transform.position = transform.Find("remainsHeadPos").position;
            Vector2 remainVelocity = velocity;
            remainVelocity.y += 1f;
            newRemain.setVelocity(remainVelocity*1.25f);
        }
        if (remainBody)
        {
            EnemyRemain newRemain = Instantiate<EnemyRemain>(remainBody);
            newRemain.transform.position = transform.Find("remainsBodyPos").position;
            newRemain.setVelocity(velocity * 0.75f);
        }
        if (remainHead)
        {
            EnemyRemain newRemain  = Instantiate<EnemyRemain>(remainWings);
            newRemain.transform.position = transform.Find("remainsWingsPos").position;
            newRemain.setVelocity(velocity * 1f);
        }

        SetState(State.Dead);
        //Destroy(transform.gameObject);
    }
}
