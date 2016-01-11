using UnityEngine;
using System.Collections;

public class CrumblingStairs : MonoBehaviour, IGResetable
{
    //ground.gameObject.SetActive(false);
    //    handle.gameObject.SetActive(false);
    //    mount.gameObject.SetActive(true);

    //    mountBody.isKinematic = false;

    //    if (LimitSteps.Length > crumbled)
    //    {
    //        mountHinge.useLimits = true;
    //        JointAngleLimits2D angleLimits = new JointAngleLimits2D();
    //angleLimits.min = LimitSteps[crumbled];
    //        angleLimits.max = 360f;
    //        mountHinge.limits = angleLimits;
    //    }

    //    if( CamShakeTimes.Length > crumbled)
    //    {
    //        //RLHScene.Instance.CamController.ShakeImpulseStart(CamShakeTimes[crumbled], CamShakeAmplitudes[crumbled], CamShakeSpeeds[crumbled]);
    //    }

    //    currentHangTime = 0f;

    //    crumbled++;
    
    int ToResetCrumbled = 0;
    float ToResetCurrentHangTime = 0f;
    bool ToResetIsNoCrumbled = true;

    Vector3 ToResetMountPosition;
    float ToResetMoutRotation;
    JointAngleLimits2D ToResetAngleLimits = new JointAngleLimits2D();
     
    public void GResetCacheResetData()
    {
        ToResetCrumbled = crumbled;
        ToResetCurrentHangTime = currentHangTime;

        ToResetIsNoCrumbled = mountBody.isKinematic;

        ToResetMountPosition = mountBody.position;
        ToResetMoutRotation = mountBody.rotation;
        ToResetAngleLimits.min = mountHinge.limits.min;
        ToResetAngleLimits.max = mountHinge.limits.max;
        
        //angleLimits.min = LimitSteps[crumbled];
        //        angleLimits.max = 360f;
        //        mountHinge.limits = angleLimits;
    }

    public void GReset()
    {
       // mountHinge.enabled = false;

        crumbled = ToResetCrumbled;
        currentHangTime = ToResetCurrentHangTime;

        //gameObject.SetActive(!ToResetCollapsed);
        //Collapsed = ToResetCollapsed;
        //ToCollapseTime = ToResetToCollapseTime;

        ground.gameObject.SetActive(ToResetIsNoCrumbled);
        handle.gameObject.SetActive(ToResetIsNoCrumbled);

       
        //mountHinge.limits.min = ToResetAngleLimits.min;
        mountHinge.useLimits = true;
        

        mountBody.position = ToResetMountPosition;
        mountBody.rotation = ToResetMoutRotation;

        JointAngleLimits2D angleLimits = new JointAngleLimits2D();
        angleLimits.min = ToResetAngleLimits.min;
        angleLimits.max = ToResetAngleLimits.max;
        mountHinge.limits = angleLimits;

        //mount.gameObject.SetActive(!ToResetIsNoCrumbled);
        mountBody.isKinematic = ToResetIsNoCrumbled;
        //mount.gameObject.SetActive(!ToResetIsNoCrumbled);

        //mountHinge.enabled = true;

        if ( ToResetIsNoCrumbled )
        {
            
        }
        else
        {

        }
    }

    Transform ground = null;
    Transform handle = null;
    Transform mount = null;
    Transform mountHandle = null;
    HingeJoint2D mountHinge = null;
    Rigidbody2D mountBody = null;

    int crumbled = 0;

    public float[] LimitSteps;
    public float[] MaxHangTime;
    float currentHangTime = 0f;

    //rlhScene.CamController.ShakeImpulseStart(2f, 0.25f, 8f);
    public float[] CamShakeTimes;
    public float[] CamShakeAmplitudes;
    public float[] CamShakeSpeeds;

    public string SoundTagCatch = "";
    public string SoundTagCrumble = "";

    public Transform MountHandle
    {
        get { return mountHandle; }
    }

    public Transform Mount
    {
        get { return mount; }
    }

    // Use this for initialization
    void Start()
    {        
        ground = transform.Find("ground");
        handle = transform.Find("handle");
        mount = transform.Find("mount");
        mountHandle = transform.Find("mountHandle");
        mountHinge = mount.GetComponent<HingeJoint2D>();
        mountBody = mount.GetComponent<Rigidbody2D>();
        
        if (!ground)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma ground");
            Debug.Break();
        }
        if (!handle)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma handle");
            Debug.Break();
        }
        if (!mount)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma mount");
            Debug.Break();
        }
        if (!mountHandle)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma mountHandle");
            Debug.Break();
        }
        if (!mountHinge)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma mountHinge");
            Debug.Break();
        }
        if (!mountBody)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma mountBody");
            Debug.Break();
        }

        //Rigidbody2D mountBody = mount.GetComponent<Rigidbody2D>();
        //mountBody.centerOfMass = new Vector2(0f, 0f);

       // mount.gameObject.SetActive(false);

        //if( LimitSteps.Length > 0)
        //{
        //    mountHinge.useLimits = true;
        //    JointAngleLimits2D angleLimits = new JointAngleLimits2D();
        //    angleLimits.min = LimitSteps[0];
        //    angleLimits.max = 360f;
        //    mountHinge.limits = angleLimits;
        //}

        if (CamShakeTimes.Length != CamShakeAmplitudes.Length || CamShakeAmplitudes.Length != CamShakeSpeeds.Length)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie zgadzaja sie liczby parametrow CamShake");
            Debug.Break();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (crumbled > 0)
        //{
        //    if (mountBody.sleepMode)
        //}
    }

    public bool TryToCrumble(float deltaTime)
    {
        if( MaxHangTime.Length > crumbled )
        {
            if((currentHangTime+=deltaTime) > MaxHangTime[crumbled-1] )
            {
                Crumble();
            }
        }
        return false;
    }

    public void ZapJumpOff()
    {
        ground.gameObject.SetActive(false);
        handle.gameObject.SetActive(false);
        mount.gameObject.SetActive(true);
        mountBody.isKinematic = false;

        //if (LimitSteps.Length > crumbled)
        {
            mountHinge.useLimits = false;
            //JointAngleLimits2D angleLimits = new JointAngleLimits2D();
            //angleLimits.min = LimitSteps[crumbled];
            //angleLimits.max = 360f;
            //mountHinge.limits = angleLimits;
        }

        //if (CamShakeTimes.Length > crumbled)
        //{
        //    //RLHScene.Instance.CamController.ShakeImpulseStart(CamShakeTimes[crumbled], CamShakeAmplitudes[crumbled], CamShakeSpeeds[crumbled]);
        //}

        currentHangTime = 0f;
        //crumbled++;

        //if (SoundTagCrumble != "") SoundPlayer.Play(gameObject, SoundTagCrumble);
    }

    public void Crumble()
    {
        print("Crumble");

        ground.gameObject.SetActive(false);
        handle.gameObject.SetActive(false);
        mount.gameObject.SetActive(true);

        mountBody.isKinematic = false;

        if (LimitSteps.Length > crumbled)
        {
            mountHinge.useLimits = true;
            JointAngleLimits2D angleLimits = new JointAngleLimits2D();
            angleLimits.min = LimitSteps[crumbled];
            angleLimits.max = 360f;
            mountHinge.limits = angleLimits;
        }

        if( CamShakeTimes.Length > crumbled)
        {
            //RLHScene.Instance.CamController.ShakeImpulseStart(CamShakeTimes[crumbled], CamShakeAmplitudes[crumbled], CamShakeSpeeds[crumbled]);
        }

        currentHangTime = 0f;

        crumbled++;

        if( SoundTagCrumble != "" ) SoundPlayer.Play(gameObject, SoundTagCrumble);
    }

    public bool HaveNextCrumbled()
    {
        return LimitSteps.Length > crumbled;
    }

    //public void Reset()
    //{

    //}
}
