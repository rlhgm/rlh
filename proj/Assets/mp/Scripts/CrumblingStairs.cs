﻿using UnityEngine;
using System.Collections;

public class CrumblingStairs : MonoBehaviour
{
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

        mount.gameObject.SetActive(false);

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

    public void Crumble()
    {
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
    }

    public bool HaveNextCrumbled()
    {
        return LimitSteps.Length > crumbled;
    }

    public void Reset()
    {

    }
}