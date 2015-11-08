using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RLHScene : MonoBehaviour
{
    public bool onlyKnife = false;
    public Dictionary<int, bool> ShowInfoTriggersControlls = new Dictionary<int, bool>();
    public Dictionary<int, bool> ShowInfoTriggersControllsApproved = new Dictionary<int, bool>();

    public LevelBounds levelBounds = null;

    private static RLHScene _scene = null;

    Zap zap = null;

    public static RLHScene Instance
    {
        // zakladam ze RLHScene::Awake  zawsze jest wolane jako pierwsze... (z ustawien projektu Unity)
        get { return _scene; }
    }

    public Zap Zap
    {
        get { return zap; }
    }

    void Awake()
    {
        _scene = this;
        transform.position = new Vector3(0f,0f,0f);
        Application.targetFrameRate = -1;

        layerIdGroundMask = 1 << LayerMask.NameToLayer("Ground");
        layerIdGroundMoveableMask = 1 << LayerMask.NameToLayer("GroundMoveable");
        layerIdGroundAllMask = layerIdGroundMask | layerIdGroundMoveableMask;
    }

    void OnEnable()
    {
        //print("RLHScene::OnEnabled ble ble ble");    
        _scene = this;
    }

    // Use this for initialization
    void Start()
    {
        if (zap == null)
        {
            zap = FindObjectOfType<Zap>();
            print("RLHScene::Start");
            if( zap == null )
                Debug.LogWarning("RLHScene::Zap == null");
        }

        ShowInfoTrigger[] sits = FindObjectsOfType(typeof(ShowInfoTrigger)) as ShowInfoTrigger[];
        foreach (ShowInfoTrigger sit in sits)
        {
            for( int i = 0; i < sit.controlValues.Length; ++i )
            {
                ShowInfoTriggersControlls[sit.controlValues[i]] = false;
            }
        }
        NewRope[] ropes = FindObjectsOfType(typeof(NewRope)) as NewRope[];
        foreach (NewRope rope in ropes)
        {
            for (int i = 0; i < rope.controlValues.Length; ++i)
            {
                ShowInfoTriggersControlls[rope.controlValues[i]] = false;
            }
        }
        ShowInfoTriggersControllsApproved = new Dictionary<int, bool>( ShowInfoTriggersControlls );
        //print(ShowInfoTriggersControlls);
    }

    public void activateShowInfoTriggerController(ShowInfoTriggerController sitc)
    {
        for (int i = 0; i < sitc.controlValues.Length; ++i)
        {
            ShowInfoTriggersControlls[sitc.controlValues[i]] = true;
        }
    }
    public void ropeBreakOff(NewRope cuttedRope)
    {
        for (int i = 0; i < cuttedRope.controlValues.Length; ++i)
        {
            ShowInfoTriggersControlls[cuttedRope.controlValues[i]] = true;
        }
    }

    public bool isActiveShowInfoTrigger(ShowInfoTrigger sit)
    {
        for (int i = 0; i < sit.controlValuesNeg.Length; ++i)
        {
            if (ShowInfoTriggersControlls[sit.controlValuesNeg[i]] == true)
                return false;
        }

        for (int i = 0; i < sit.controlValues.Length; ++i)
        {
            if (ShowInfoTriggersControlls[sit.controlValues[i]] == false)
                return false;
        }

        return true;
    }

    public void printShowInfoTriggersControlls()
    {
        //Dictionary<int,bool>.Enumerator e = ShowInfoTriggersControlls.GetEnumerator();
        //while
        string s = "SITC values : ";
        foreach (KeyValuePair<int, bool> v in ShowInfoTriggersControlls)
        {
            s += "[";
            s += v.Key;
            s += "=>";
            s += v.Value;
            s += "], ";
        }
        print(s);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void reset()
    {
        ShowInfoTriggersControlls = new Dictionary<int, bool>( ShowInfoTriggersControllsApproved );
    }

    public void checkPointReached()
    {
        ShowInfoTriggersControllsApproved = new Dictionary<int, bool>( ShowInfoTriggersControlls );
    }


    RaycastHit2D _hit;
    public int layerIdGroundMask;
    [HideInInspector]
    public int layerIdGroundMoveableMask;
    [HideInInspector]
    public int layerIdGroundAllMask;

    public bool checkObstacle(Vector2 from, Vector2 dir, float distToCheck, ref float distToObstacle, float maxTilt)
    {
        _hit = Physics2D.Raycast(from, dir, distToCheck, layerIdGroundAllMask);
        if (_hit.collider != null)
        {
            distToObstacle = _hit.distance;
            return Vector2.Angle(Vector2.up, _hit.normal) > maxTilt;
        }
        return false;
    }

    public bool checkGround(Vector2 from, float distToCheck, ref float distToGround, ref float groundAngle)
    {
        _hit = Physics2D.Raycast(from, Vector2.down, distToCheck, layerIdGroundAllMask);
        if (_hit.collider != null)
        {
            distToGround = _hit.distance;
            groundAngle = Vector2.Angle(Vector2.up, _hit.normal);
        }
        return _hit.collider;
    }


    //public float checkRight(float checkingDist, bool flying = false)
    //{
    //    if (!stateJustChanged)
    //    {
    //        if (flying)
    //        {
    //            hit = Physics2D.Raycast(sensorRight1.position, Vector2.right, checkingDist, layerIdGroundAllMask);
    //            if (hit.collider != null)
    //            {
    //                return Mathf.Abs(hit.point.x - sensorRight1.position.x);
    //            }
    //        }
    //        else
    //        {
    //            //Vector2 ro = sensorDown2.position;
    //            //ro.y += 0.1f;
    //            int numRes = Physics2D.RaycastNonAlloc(sensorDown2.position, Vector2.right, raycastHits, checkingDist + 0.5f, layerIdGroundAllMask);
    //            for (int i = 0; i < numRes; ++i)
    //            {
    //                //hit = raycastHits[i];
    //                //float angle = hit.collider.transform.eulerAngles.z;
    //                ////float angle = Quaternion.Angle(transform.rotation, hit.collider.transform.rotation);
    //                //angle = angle % 90;
    //                //if (angle < -45.0f || angle > 45.0f)
    //                //    return Mathf.Abs(hit.point.x - sensorRight1.position.x);

    //                hit = raycastHits[i];
    //                if (hit.fraction == 0f) continue;
    //                float angle = Vector2.Angle(Vector2.up, hit.normal);
    //                if (Mathf.Abs(angle) > 45.0f)
    //                {
    //                    Vector2 ro = sensorDown2.position;
    //                    ro.x += (hit.distance + 0.01f);
    //                    ro.y += 0.2f;
    //                    //bool hitObstacle = false;
    //                    int numRes2 = Physics2D.RaycastNonAlloc(ro, -Vector2.up, raycastHits2, 0.2f + 0.1f, layerIdGroundAllMask);
    //                    for (int j = 0; j < numRes2; ++j)
    //                    {
    //                        hit2 = raycastHits2[j];
    //                        if (hit2.collider != hit.collider) continue;

    //                        if (hit2.fraction == 0f) return Mathf.Abs(hit.point.x - sensorRight1.position.x);
    //                        float angle2 = Vector2.Angle(Vector2.up, hit2.normal);
    //                        if (Mathf.Abs(angle2) > 45.0f) return Mathf.Abs(hit.point.x - sensorRight1.position.x);
    //                    }

    //                    //return Mathf.Abs(hit.point.x - sensorRight1.position.x);
    //                }
    //            }
    //        }
    //    }
    //    hit = Physics2D.Raycast(sensorRight2.position, Vector2.right, checkingDist, layerIdGroundAllMask);
    //    if (hit.collider != null)
    //    {
    //        return Mathf.Abs(hit.point.x - sensorRight2.position.x);
    //    }

    //    if (!flying && (currentController.crouching() || (currentController != zapControllerGravityGun && currentController.isDodging())))
    //        return -1.0f;

    //    hit = Physics2D.Raycast(sensorRight3.position, Vector2.right, checkingDist, layerIdGroundAllMask);
    //    if (hit.collider != null)
    //    {
    //        return Mathf.Abs(hit.point.x - sensorRight3.position.x);
    //    }
    //    return -1f;
    //}

}
