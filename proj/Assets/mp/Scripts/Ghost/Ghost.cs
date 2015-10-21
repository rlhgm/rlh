using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ghost : MonoBehaviour
{
   
    Vector3 startPoint = new Vector3();

    public bool autoCatchEdges = false;

    [HideInInspector]
    public GhostController currentController;
    //[HideInInspector]
    //public GhostController beforeFallController;
    //[HideInInspector]
    //public GhostController choosenController;
    public GhostControllerByKeyTriggers ghostControllerByKeyTriggers = new GhostControllerByKeyTriggers();

    //public void OnEnable()
    //{
    //    hideFlags = HideFlags.HideAndDontSave;
    //    if (ghostControllerNormal == null)
    //    {
    //        ghostControllerNormal = ScriptableObject.CreateInstance<GhostControllerNormal>();
    //    }
    //}

    void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        gfx = transform.Find("gfx").transform;

        animatorBody = transform.Find("gfx").GetComponent<Animator>();
        sprRend = gfx.GetComponent<SpriteRenderer>();
        //print(sprRend.material.shader);
        //print(sprRend.material.GetFloat("_Frequency"));
        //sprRend.material.SetFloat("_Frequency", .5f);
        //print(sprRend.material.GetFloat("_Frequency"));

        sensorLeft1 = transform.Find("sensorLeft1").transform;
        sensorLeft2 = transform.Find("sensorLeft2").transform;
        sensorLeft3 = transform.Find("sensorLeft3").transform;
        sensorRight1 = transform.Find("sensorRight1").transform;
        sensorRight2 = transform.Find("sensorRight2").transform;
        sensorRight3 = transform.Find("sensorRight3").transform;
        sensorDown1 = transform.Find("sensorDown1").transform;
        sensorDown2 = transform.Find("sensorDown2").transform;
        sensorDown3 = transform.Find("sensorDown3").transform;

        sensorHandleL2 = transform.Find("handlerL2").transform;
        sensorHandleR2 = transform.Find("handlerR2").transform;

        layerIdGroundMask = 1 << LayerMask.NameToLayer("Ground");
        layerIdGroundPermeableMask = 1 << LayerMask.NameToLayer("GroundPermeable");
        layerIdGroundMoveableMask = 1 << LayerMask.NameToLayer("GroundMoveable");
        layerIdGroundAllMask = layerIdGroundMask | layerIdGroundPermeableMask | layerIdGroundMoveableMask;
        layerIdLastGroundTypeTouchedMask = layerIdGroundMask;

        layerIdGroundHandlesMask = 1 << LayerMask.NameToLayer("GroundHandles");

        layerIdMountMask = 1 << LayerMask.NameToLayer("Mount");
        layerIdRopesMask = 1 << LayerMask.NameToLayer("Ropes");

        myWidth = coll.size.x;
        myHalfWidth = myWidth * 0.5f;
        //myHeight = coll.size.y;
        //myHalfHeight = myHeight * 0.5f;
        //ghostControllerNormal = ScriptableObject.CreateInstance<GhostControllerNormal>();
        ghostControllerByKeyTriggers.setOwner(this);
    }

    void Start()
    {
        setCurrentController(ghostControllerByKeyTriggers);

        velocity = new Vector3(0, 0, 0);
        impulse = new Vector3(0, 0, 0);

        startFallPos = transform.position;

        setState(State.ON_GROUND);
        currentController.activate();

        startPoint = transform.position;
        //beforeFallController = null;
    }

    public void setCurrentController(GhostController newController, bool restore = false, bool crouch = false)
    {
        if (currentController != null)
            currentController.deactivate();
        currentController = newController;
        currentController.activate(restore, crouch);
    }

    //public void suddenlyInAir()
    //{
    //    //if (currentController != zapControllerNormal) {
    //    //	beforeFallController = currentController;
    //    //	setCurrentController(zapControllerNormal);
    //    //	setState (Zap.State.IN_AIR);
    //    //	zapControllerNormal.suddenlyInAir(); 
    //    //}
    //}
    //public void restoreBeforeFallController()
    //{
    //    if (beforeFallController != null)
    //    {
    //        setCurrentController(beforeFallController, true);
    //        beforeFallController = null;
    //    }
    //}

    public void StateIdleExit()
    {
    }
    public void StateIdleUpdate(float normTime)
    {
    }

    public int IdleAnimFreq = 10;

    public void StateIdleFinish(int stateIdleNum)
    {
        switch (stateIdleNum)
        {
            case 1:
            case 2:
                if (faceRight()) animatorBody.Play("Zap_idle_R");
                else animatorBody.Play("Zap_idle_L");
                break;
            case 0:
                if (IdleAnimFreq >= 3)
                {
                    int r = Random.Range(0, IdleAnimFreq);
                    if (r == 0)
                    {
                        if (faceRight()) animatorBody.Play("Zap_idle_variation_1_R");
                        else animatorBody.Play("Zap_idle_variation_1_L");
                    }
                    else if (r == 1)
                    {
                        if (faceRight()) animatorBody.Play("Zap_idle_variation_2_R");
                        else animatorBody.Play("Zap_idle_variation_2_L");
                    }
                }
                break;
        }
    }

    public enum DeathType
    {
        VERY_HARD_LANDING = 1,
        SNAKE,
        CROCODILE,
        PANTHER,
        POISON,
        STONE_HIT
    };

    public void die(DeathType deathType)
    {
        velocity.x = 0.0f;
        velocity.y = 0.0f;
        currentController.die(deathType);
        setState(State.DEAD);
    }

    public bool isDead()
    {
        return isInState(State.DEAD);
    }

    public bool canBeFuddleFromBird = true;
    bool fuddledFromBrid = false;
    public bool FuddleFromBird
    {
        set
        {
            fuddledFromBrid = value;
        }
        get
        {
            return fuddledFromBrid;
        }
    }

    //public float stoneDeadlySpeed = 8f;
    //public float stoneDeadlyMass = 8f;
    public float stoneDeadlyEnergy = 20f;
    public float stoneMinDeadySpeed = 1f;
    public float stoneMinDeadyMass = 0.5f;

    bool hitByStone(Transform stone)
    {
        Rigidbody2D stoneBody = stone.GetComponent<Rigidbody2D>();
        if (!stoneBody)
            return false;

        float stoneSpeed = stoneBody.velocity.magnitude;
        if (stoneSpeed < stoneMinDeadySpeed)
        {
            return false;
        }

        float stoneMass = stoneBody.mass;
        if (stoneMass < stoneMinDeadyMass)
        {
            return false;
        }

        float stoneEnergy = stoneSpeed * stoneMass;
        if (stoneEnergy > stoneDeadlyEnergy)
        {
            die(DeathType.STONE_HIT);
            return true;
        }

        return false;
    }


    //	void OnCollisionEnter2D	(Collider2D other){
    //		if (other.transform.gameObject.layer == layerIdGroundMoveableMask) { // to jest kamien 
    //			hitByStone( other.transform );
    //			return;
    //		}
    //	}
    //
    //	void OnCollisionStay2D(Collider2D other){
    //		if (other.transform.gameObject.layer == layerIdGroundMoveableMask) { // to jest kamien 
    //			hitByStone( other.transform );
    //			return;
    //		}
    //	}

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead())
            return;

        int lid = other.transform.gameObject.layer;
        if (lid == LayerMask.NameToLayer("GroundMoveable"))
        { // layerIdGroundMoveableMask) { // to jest kamien 
            hitByStone(other.transform);
            return;
        }

        if (other.gameObject.tag == "Panther")
        {
            Panther panther = other.gameObject.GetComponent<Panther>();
            if (panther.attacking())
            {
                if (!currentController.isDodging())
                {
                    die(DeathType.PANTHER);
                }
            }
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead())
            return;

        if (currentController.triggerEnter(other))
            return;

        int lid = other.transform.gameObject.layer;
        int lid2 = LayerMask.NameToLayer("GroundMoveable");
        if (lid == lid2)
        {// layerIdGroundMoveableMask) { // to jest kamien 
            hitByStone(other.transform);
            return;
        }

        //		if (other.gameObject.tag == "Bird") {
        //			if( isInState(State.MOUNT) ){
        //				velocity.x = 0.0f;
        //				velocity.y = 0.0f;
        //				setAction(Action.JUMP);
        //				setState(State.IN_AIR);
        //
        //				if( canBeFuddleFromBird )
        //					fuddledFromBrid = true;
        //
        //			} else if( isInState(State.IN_AIR) ) {
        //				velocity.x = 0.0f;
        //			}
        //			return;
        //		}

        if (other.gameObject.tag == "KillerPhysic")
        {
            die(DeathType.POISON);
            return;
        }
        if (other.gameObject.tag == "Crocodile")
        {
            die(DeathType.CROCODILE);

            other.gameObject.GetComponent<Crocodile>().attackStart();
            sprRend.enabled = false;

            return;
        }
        if (other.gameObject.tag == "Panther")
        {
            Panther panther = other.gameObject.GetComponent<Panther>();
            if (panther.attacking())
            {
                if (!currentController.isDodging())
                {
                    die(DeathType.PANTHER);
                }
            }
            return;
        }
    }

    //bool userJumpKeyPressed = false;
    //float timeFromJumpKeyPressed = 0.0f;
    //[HideInInspector]
    //public bool jumpKeyPressed = false;

    void FixedUpdate()
    {
        if (currentController != null)
        {
            currentController.FUpdate(Time.fixedDeltaTime);
        }
    }

    private float ConstantFrameTime = 0.0333f;
    private float CurrentDeltaTime = 0.0f;

    public float getConstantFrameTime()
    {
        return ConstantFrameTime;
    }
    public float getCurrentDeltaTime()
    {
        return CurrentDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        float timeSinceLastFrame = Time.deltaTime;

        if ( currentController.GlobalUpdate(timeSinceLastFrame)) return;

        while (timeSinceLastFrame > ConstantFrameTime)
        {
            GhostUpdate(ConstantFrameTime);
            timeSinceLastFrame -= ConstantFrameTime;
        }

        GhostUpdate(timeSinceLastFrame);
    }

    //bool GlobalUpdate(float deltaTime)
    //{
    //    currentController.GlobalUpdate(float)
    //    return false;
    //}

    void GhostUpdate(float deltaTime)
    {
        if (isDead())
        {
            return;
        }

        CurrentDeltaTime = deltaTime;

        SetImpulse(new Vector2(0.0f, 0.0f));

        stateJustChanged = false;
        currentStateTime += deltaTime;
        currentActionTime += deltaTime;
        
        currentController.MUpdate(CurrentDeltaTime);
    }

    public bool checkObstacle(int dir, float distToCheck, ref float distToObstacle)
    {
        if (dir == 1)
        {
            distToObstacle = checkRight(Mathf.Abs(distToCheck) + 0.01f);
            if (distToObstacle < 0.0f)
                return false;
            if (distToObstacle < distToCheck)
            {
                return true;
            }
            else
                return false;
        }
        else if (dir == -1)
        {
            distToObstacle = checkLeft(Mathf.Abs(distToCheck) + 0.01f);
            if (distToObstacle < 0.0f)
                return false;
            if (distToObstacle < Mathf.Abs(distToCheck))
            {
                distToObstacle *= -1.0f;
                return true;
            }
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    //KeyCode getDirKey(int dir)
    //{
    //    return dir == 1 ? keyRight : keyLeft;
    //}

    public void turnLeft()
    {
        Vector3 scl = gfx.localScale;
        scl.x = Mathf.Abs(scl.x) * -1.0f;
        gfx.localScale = scl;
    }
    public void turnRight()
    {
        Vector3 scl = gfx.localScale;
        scl.x = Mathf.Abs(scl.x) * 1.0f;
        gfx.localScale = scl;
    }

    public Vector2 dir()
    {
        return gfx.localScale.x > 0.0f ? Vector2.right : -Vector2.right;
    }
    public int dir2()
    {
        return gfx.localScale.x > 0f ? (int)1f : (int)-1f;
    }
    public bool faceRight()
    {
        return gfx.localScale.x > 0f;
    }
    public bool faceLeft()
    {
        return gfx.localScale.x < 0f;
    }

    public bool canGetUp()
    {

        if (dir() == Vector2.right)
        {
            RaycastHit2D hit = Physics2D.Raycast(sensorLeft3.position, Vector2.right, myWidth, layerIdGroundMask);
            if (hit.collider != null)
            {
                float hpx = hit.point.x;
                float _d = Mathf.Abs(sensorLeft3.position.x + myWidth - hpx);
                if (_d > 0.0001f)
                    return false;
            }
            return true;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(sensorRight3.position, -Vector2.right, myWidth, layerIdGroundMask);
            if (hit.collider != null)
            {
                float hpx = hit.point.x;
                float _d = Mathf.Abs(sensorRight3.position.x - myWidth - hpx);
                if (_d > 0.0001f)
                    return false;
            }
            return true;
        }
    }

    public float checkLeft(float checkingDist, bool flying = false)
    {
        Vector2 rayOrigin;
        if (flying)
        {
            rayOrigin = transform.position;
            rayOrigin.x -= myHalfWidth;
        }
        else
        {
            rayOrigin = sensorLeft1.position; // new Vector2( sensorRight1.position.x, sensorRight1.position.y );
        }

        // ponizej robie layerIdGroundAllMask - aby wchodzil na platformy ze skosow nie bedzie sie dalo klasc jednej przepuszczalnej na drugiej ale trudno
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.right, checkingDist, layerIdGroundAllMask);
        if (hit.collider != null)
        {
            float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation);
            if (angle <= 0.0f || angle > 45.0f)
                return Mathf.Abs(hit.point.x - sensorLeft1.position.x);
            else
            {
                return -1.0f;
            }
        }
        else
        {
            if (currentController.crouching() || currentController.isDodging())
                return -1.0f;

            rayOrigin = new Vector2(sensorLeft2.position.x, sensorLeft2.position.y);
            hit = Physics2D.Raycast(rayOrigin, -Vector2.right, checkingDist, layerIdGroundMask);
            if (hit.collider != null)
            {
                return Mathf.Abs(hit.point.x - sensorLeft2.position.x);
            }
            else
            {

                rayOrigin = new Vector2(sensorLeft3.position.x, sensorLeft3.position.y);
                hit = Physics2D.Raycast(rayOrigin, -Vector2.right, checkingDist, layerIdGroundMask);
                if (hit.collider != null)
                {
                    return Mathf.Abs(hit.point.x - sensorLeft3.position.x);
                }
                else
                {
                    return -1.0f;
                }
            }
        }
    }

    public float checkRight(float checkingDist, bool flying = false)
    {
        Vector2 rayOrigin;
        if (flying)
        {
            rayOrigin = transform.position;
            rayOrigin.x += myHalfWidth;
        }
        else
        {
            rayOrigin = sensorRight1.position; // new Vector2( sensorRight1.position.x, sensorRight1.position.y );
        }
        // ponizej robie layerIdGroundAllMask - aby wchodzil na platformy ze skosow nie bedzie sie dalo klasc jednej przepuszczalnej na drugiej ale trudno
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, checkingDist, layerIdGroundAllMask);
        if (hit.collider != null)
        {
            float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation);
            if (angle <= 0.0f || angle > 45.0f) return Mathf.Abs(hit.point.x - sensorRight1.position.x);
            else return -1.0f;
        }
        else
        {
            if (currentController.crouching() || currentController.isDodging())
                return -1.0f;

            rayOrigin = new Vector2(sensorRight2.position.x, sensorRight2.position.y);
            hit = Physics2D.Raycast(rayOrigin, Vector2.right, checkingDist, layerIdGroundMask);
            if (hit.collider != null)
            {
                return Mathf.Abs(hit.point.x - sensorRight2.position.x);
            }
            else
            {

                rayOrigin = new Vector2(sensorRight3.position.x, sensorRight3.position.y);
                hit = Physics2D.Raycast(rayOrigin, Vector2.right, checkingDist, layerIdGroundMask);
                if (hit.collider != null)
                {
                    return Mathf.Abs(hit.point.x - sensorRight3.position.x);
                }
                else
                {
                    return -1.0f;
                }
            }
        }
    }

    public float checkDown(float checkingDist)
    {

        int layerIdMask = layerIdGroundAllMask;
        Vector3 rayOrigin = sensorDown1.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, myWidth, layerIdGroundPermeableMask);
        if (hit.collider)
        {// jesetem wewnatrz wskakiwalnej platformy ... nie moge sie zatrzymac..
            layerIdMask = layerIdGroundMask;
        }

        rayOrigin = new Vector2(sensorDown1.position.x, sensorDown1.position.y);
        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, checkingDist, layerIdMask);
        if (hit.collider != null)
        {
            layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
            return Mathf.Abs(hit.point.y - sensorDown1.position.y);
        }
        else
        {
            rayOrigin = new Vector2(sensorDown2.position.x, sensorDown2.position.y);
            hit = Physics2D.Raycast(rayOrigin, -Vector2.up, checkingDist, layerIdMask);
            if (hit.collider != null)
            {
                layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
                return Mathf.Abs(hit.point.y - sensorDown2.position.y);
            }
            else
            {
                rayOrigin = new Vector2(sensorDown3.position.x, sensorDown3.position.y);
                hit = Physics2D.Raycast(rayOrigin, -Vector2.up, checkingDist, layerIdMask);
                if (hit.collider != null)
                {
                    layerIdLastGroundTypeTouchedMask = 1 << hit.collider.transform.gameObject.layer;
                    return Mathf.Abs(hit.point.y - sensorDown3.position.y);
                }
                else
                {
                    return -1.0f;
                }
            }
        }
    }

    public bool checkGround(bool fromFeet, int layerIdMask, ref float distToGround)
    {
        bool groundUnderFeet = false;

        float th = 0.9f;
        float checkingDist = th + 0.5f;
        if (fromFeet)
            checkingDist = 0.5f;

        Vector2 rayOrigin1 = sensorDown1.position;
        if (!fromFeet)
            rayOrigin1.y += th;
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin1, -Vector2.up, checkingDist, layerIdMask);

        Vector2 rayOrigin2 = sensorDown2.position;
        if (!fromFeet)
            rayOrigin2.y += th;
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, -Vector2.up, checkingDist, layerIdMask);

        Vector2 rayOrigin3 = sensorDown3.position;
        if (!fromFeet)
            rayOrigin3.y += th;
        RaycastHit2D hit3 = Physics2D.Raycast(rayOrigin3, -Vector2.up, checkingDist, layerIdMask);

        float dist1;
        float dist2;
        float dist3;

        if (hit1.collider != null)
        {
            dist1 = rayOrigin1.y - hit1.point.y;
            groundUnderFeet = true;
            distToGround = dist1;
            layerIdLastGroundTypeTouchedMask = 1 << hit1.collider.transform.gameObject.layer;
        }
        if (hit2.collider != null)
        {
            dist2 = rayOrigin2.y - hit2.point.y;
            if (groundUnderFeet)
            {
                if (distToGround > dist2) distToGround = dist2;
            }
            else
            {
                groundUnderFeet = true;
                distToGround = dist2;
                layerIdLastGroundTypeTouchedMask = 1 << hit2.collider.transform.gameObject.layer;
            }
        }
        if (hit3.collider != null)
        {
            dist3 = rayOrigin3.y - hit3.point.y;
            if (groundUnderFeet)
            {
                if (distToGround > dist3) distToGround = dist3;
            }
            else
            {
                groundUnderFeet = true;
                distToGround = dist3;
                layerIdLastGroundTypeTouchedMask = 1 << hit3.collider.transform.gameObject.layer;
            }
        }

        if (groundUnderFeet)
        {
            if (!fromFeet)
                distToGround = th - distToGround;
        }

        return groundUnderFeet;
    }

    public bool checkMount()
    {
        Vector2 rayOrigin = sensorLeft3.transform.position; // transform.position;
        rayOrigin.y += 0.3f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, myWidth, layerIdMountMask);

        if (!hit.collider)
            return false;

        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerIdMountMask);
        if (!hit.collider)
            return false;

        rayOrigin.x += myWidth;
        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerIdMountMask);
        return hit.collider;
    }

    public bool checkMount(Vector3 posToCheck)
    {
        Vector3 sensorDiff = sensorLeft3.transform.position - transform.position; // transform.position;

        Vector2 rayOrigin = posToCheck + sensorDiff;//aaa
        rayOrigin.y += 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, myWidth, layerIdMountMask);

        if (!hit.collider)
            return false;

        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerIdMountMask);
        if (!hit.collider)
            return false;

        rayOrigin.x += myWidth;
        hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1f, layerIdMountMask);
        return hit.collider;
    }




    public void SetImpulse(Vector2 imp) { impulse = imp; }
    public Vector2 GetImpulse() { return impulse; }
    public void AddImpulse(Vector3 imp) { impulse += imp; }
    public void AddImpulse(Vector2 imp)
    {
        impulse.x += imp.x;
        impulse.y += imp.y;
    }

    /*////////////////////////////////////////////////////////////*/

    public enum State
    {
        ON_GROUND = 0,
        IN_AIR,
        CLIMB,
        MOUNT,
        CLIMB_ROPE,
        DEAD,
        OTHER
    };

    public State getState()
    {
        return state;
    }

    [HideInInspector]
    public bool stateJustChanged = false;

    public bool setState(State newState)
    {

        if (state == newState)
            return false;

        currentStateTime = 0.0f;
        stateJustChanged = true;

        state = newState;

        switch (state)
        {
            case State.IN_AIR:
                startFallPos = transform.position;
                break;
            case State.CLIMB_ROPE:
                break;
            case State.MOUNT:
                animatorBody.Play("Zap_climbmove_up");
                break;
        };

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

    /*////////////////////////////////////////////////////////////*/

    [HideInInspector]
    public Transform sensorLeft1;
    [HideInInspector]
    public Transform sensorLeft2;
    [HideInInspector]
    Transform sensorLeft3;
    [HideInInspector]
    Transform sensorRight1;
    [HideInInspector]
    public Transform sensorRight2;
    [HideInInspector]
    public Transform sensorRight3;
    [HideInInspector]
    public Transform sensorDown1;
    [HideInInspector]
    public Transform sensorDown2;
    [HideInInspector]
    public Transform sensorDown3;

    [HideInInspector]
    public Transform sensorHandleL2;
    [HideInInspector]
    public Transform sensorHandleR2;

    Transform gfx;

    public SpriteRenderer sprRend = null;
    BoxCollider2D coll;
    Animator animatorBody;
    public Animator AnimatorBody
    {
        get
        {
            return animatorBody;
        }
    }

    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 lastVelocity;

    Vector3 lastSwingPos;
    //[SerializeField]
    Vector3 impulse;
    [HideInInspector]
    public Vector3 startFallPos;

    float desiredSpeedX = 0.0f;

    [HideInInspector]
    public float currentActionTime = 0f;
    //	public float getCurrentActionTime() {
    //		return currentActionTime;
    //	}
    public void resetCurrentActionTime()
    {
        currentActionTime = 0f;
    }
    float currentStateTime = 0.0f;
    public float getCurrentStateTime()
    {
        return currentStateTime;
    }

    float myWidth;
    float myHalfWidth;

    public float getMyWidth()
    {
        return myWidth;
    }
    public float getMyHalfWidth()
    {
        return myHalfWidth;
    }

    [HideInInspector]
    public int layerIdGroundMask;
    int layerIdGroundPermeableMask;
    [HideInInspector]
    public int layerIdGroundMoveableMask;
    [HideInInspector]
    public int layerIdGroundAllMask;
    [HideInInspector]
    public int layerIdLastGroundTypeTouchedMask;
    [HideInInspector]
    public int layerIdGroundHandlesMask;
    [HideInInspector]
    public int layerIdRopesMask;
    int layerIdMountMask;

    float climbDistFromWall;
    Vector2 climbDir;

    bool gamePaused = false;

    int playerCurrentLayer;

    public State state;
}
