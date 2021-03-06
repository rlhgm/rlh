﻿using UnityEngine;
using System.Collections;

public class GroundMoveable : MonoBehaviour
{
    //public SoundSets soundsSets;
    public string SoundTagTagImpact = "";
    public string SoundTagBreakOff = "";

    //public GameObject TryToBreakOffParticles = null;
    //public GameObject BreakOffPartices = null;
    public ParticleSet particles = null;

    public string ParticleTagImpact = "";
    public string ParticleTagTryBreakOff = "";
    public string ParticleTagBreakOff = "";

    int resetLayer = 0;
    Vector2 resetPosition;
    float resetRotation;
    Vector2 resetVelocity;
    float resetAngularVelocity;
    bool resetHanging;
    Vector2 fakeWorldCenterOfMass;
    bool resetActive;

    //BoxCollider2D boxCollider = null;
    public Rigidbody2D physic = null;
    //NewRope connectedRope;

    Vector2[] handles;
    bool[] handlesActive;
    Vector2[] normals;

    bool lastSleeped = false;
    bool isClockwise = false;

    //bool hanging = false;
    public float toBreakOffDist = 1f;

    bool isCollapsableFootbridge = false;

    SpriteRenderer gfx = null;
    
    public bool IsHanging()
    {
        return physic.isKinematic;
    }
    public void Hang()
    {
        if (IsHanging()) return;
        physic.isKinematic = true;
    }
    void SetHanging(bool newHanging)
    {
        physic.isKinematic = newHanging;
    }
    public void BreakOff()
    {
        if (!IsHanging()) return;
        physic.isKinematic = false;
        if(SoundTagBreakOff != "") SoundPlayer.Play(gameObject,SoundTagBreakOff);
        if (gfx) gfx.transform.localPosition = gfxStaticPos;
        if (OnBreakOffAction) OnBreakOffAction.Perform();
        RLHScene.Instance.CamController.ShakeImpulseStart(2f, 0.25f, 8f);

        if (particles && ParticleTagBreakOff != null)
        {
            ParticleData _pd = particles.GetParticleData(ParticleTagBreakOff);
            Vector3 particlePos = transform.position;
            Rigidbody2D _rb = GetComponent<Rigidbody2D>();
            if (_rb)
            {
                particlePos = _rb.worldCenterOfMass;
            }
            ParticleInseter.Insert(_pd, particlePos, transform.rotation);
        }

        if (ParticleTryBreakOff)
        {
            Destroy(ParticleTryBreakOff);
            ParticleTryBreakOff = null;
        }
    }

    //void ShakeUpdate()
    //{

    //}

    Object ParticleTryBreakOff;

    public void GGDragStart()
    {
        if (particles && ParticleTagTryBreakOff != null)
        {
            ParticleData _pd = particles.GetParticleData(ParticleTagTryBreakOff);
            Vector3 particlePos = transform.position;
            Rigidbody2D _rb = GetComponent<Rigidbody2D>();
            if (_rb)
            {
                particlePos = _rb.worldCenterOfMass;
            }
            ParticleTryBreakOff = ParticleInseter.Insert(_pd, particlePos, transform.rotation, false);
        }
    }

    public void GGDragStop()
    {
        if( ParticleTryBreakOff)
        {
            Destroy(ParticleTryBreakOff);
            ParticleTryBreakOff = null;
        }
    }

    bool shaking = false;
    float shakeTime = 0f;
    //Vector2 shakeMaxSpeed = new Vector2(5f, 5f);
    Vector2 shakeSpeed = new Vector2(8f, 8f);
    Vector2 shakeMaxAmplitude = new Vector2(0.25f, 0.25f);
    Vector2 shakeAmplitude = new Vector2(0f, 0f);
    Vector3 gfxStaticPos = new Vector3();
    Vector3 posOffset = new Vector3();

    public void ShakeStop()
    {
        if (gfx)
        {
            gfx.transform.localPosition = gfxStaticPos;
        }
    }

    //void Update()
    //{
    //    applyShake();
    //}

    void applyShake(float toBreakRatio /*float dt*/)
    {
        if (!gfx) return;

        shakeTime += Time.deltaTime;

        //posOffset = new Vector3();
        //print(newPos);
        shakeAmplitude.x = toBreakRatio * shakeMaxAmplitude.x;
        shakeAmplitude.y = toBreakRatio * shakeMaxAmplitude.y;

        posOffset.x = (Mathf.PerlinNoise(shakeTime * shakeSpeed.x, 0f) - 0.5f) * shakeAmplitude.x;
        posOffset.y = (Mathf.PerlinNoise(0f, shakeTime * shakeSpeed.y) - 0.5f) * shakeAmplitude.y;

        gfx.transform.localPosition = gfxStaticPos + posOffset;

        //switch (shakeStatus)
        //{
        //    case ShakeStatus.NoShake:
        //        return;

        //    case ShakeStatus.ShakeImpulse:
        //        if (shakeTime > shakeDuration)
        //        {
        //            shakeStatus = ShakeStatus.NoShake;
        //            return;
        //        }
        //        shakeRatio = 1 - (shakeTime / shakeDuration);

        //        sample = (Mathf.PerlinNoise(shakeTime * shakeSpeed.x, 0f) - 0.5f) * shakeMaxAmplitude.x;
        //        newPos.x += (sample * shakeRatio);
        //        sample = (Mathf.PerlinNoise(0f, shakeTime * shakeSpeed.y) - 0.5f) * shakeMaxAmplitude.y;
        //        newPos.y += (sample * shakeRatio);

        //        break;

        //    case ShakeStatus.ShakePermanent:
        //        newPos.x += (Mathf.PerlinNoise(shakeTime * shakeSpeed.x, 0f) - 0.5f) * shakeMaxAmplitude.x;
        //        newPos.y += (Mathf.PerlinNoise(0f, shakeTime * shakeSpeed.y) - 0.5f) * shakeMaxAmplitude.y;
        //        break;

        //    case ShakeStatus.ShakeFadeIn:
        //        break;

        //    case ShakeStatus.ShakeFadeOut:
        //        break;
        //}

        //transform.position = newPos;

        // fajny efekt 0.125 - 0.5 
        //shakeMaxAmplitude = new Vector2(0.25f, 0.25f);
        // fajny efekt nawet 8-10
        //shakeSpeed = new Vector2(8f, 8f);


    }

    public bool TryToBreakOff(Vector2 pullPoint)
    {
        if (isCollapsableFootbridge) return false;

        if (IsHanging())
        {
            float fromCenterDist = (pullPoint - fakeWorldCenterOfMass).magnitude;
            float toBreakRatio = Mathf.Min(1f, fromCenterDist / toBreakOffDist);

            applyShake(toBreakRatio);

            //Debug.Log(pullPoint + " " + physic.fakeWorldCenterOfMass);
            //if ((pullPoint - fakeWorldCenterOfMass /*physic.worldCenterOfMass*/).magnitude > toBreakOffDist)
            if (toBreakRatio >= 1f)
            {
                BreakOff();
                //Debug.Log(pullPoint + " " + physic.worldCenterOfMass);
                return true;
            }
        }
        return false;
    }

    //void OnCollisionEnter2D(Collision collision)
    //{
    //    // Debug-draw all contact points and normals
    //    foreach (ContactPoint contact in collision.contacts)
    //    {
    //        Debug.DrawRay(contact.point, contact.normal, Color.white);
    //    }

    //    // Play a sound if the colliding objects had a big impact.		
    //    //if (collision.relativeVelocity.magnitude > 2)
    //    //    audio.Play();
    //}

    //public ParticleSet particles = null;

    public RLHAction OnBreakOffAction = null;

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (coll.gameObject.tag == "Enemy")
        //    coll.gameObject.SendMessage("ApplyDamage", 10);

        //void FixedUpdate()
        //{
        //print("GroundMoveable::OnCollisionEnter2D() " + Time.time + " " + gameObject.name);
        //}

        //print(collision.collider.gameObject.name + " relVel: " + collision.relativeVelocity + " sleep: " + (physic.IsSleeping() ? "yes" : "no") + " " + physic.velocity );
        //collision.contacts[0].

        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    Debug.DrawRay(contact.point, contact.normal, Color.white);
        //}

        //collision.contacts[0].

        if (RLHScene.Instance.AddCollision(gameObject, collision.gameObject))
        {
            //print("kolizja " + gameObject.name + " z " + collision.gameObject.name);
            //print("kolizja " + physic.velocity + " z " + collision.rigidbody.);
            //print(collision.relativeVelocity);
            //collision.relativeVelocity

            //      public static float KineticEnergy(Rigidbody rb)
            //{
            //    // mass in kg, velocity in meters per second, result is joules
            //    return 0.5f * rb.mass * Mathf.Pow(rb.velocity.magnitude, 2);
            //}

            if (particles != null && ParticleTagImpact != "")
            {
                ParticleData _pd = particles.GetParticleData(ParticleTagImpact);
                for (int i = 0; i < collision.contacts.Length; ++i)
                {
                    ParticleInseter.Insert(_pd, collision.contacts[i].point);
                }
            }

            this.RLHAssert(collision.rigidbody != physic, "collision.rigidbody == physic");

            float e1 = 0.5f * physic.mass * Mathf.Pow(lastVelocity.magnitude, 2);
            float e2 = 0;
           
            GroundMoveable _gm = collision.gameObject.GetComponent<GroundMoveable>();
            if (_gm)
            {
                e2 = 0.5f * _gm.physic.mass * Mathf.Pow(_gm.physic.velocity.magnitude, 2);
                //ocv = collision.rigidbody.velocity;
                //_m = collision.rigidbody.mass;
                if (e1 > e2)
                {
                    RLHScene.Instance.StonesImpact(physic.worldCenterOfMass, e1);
                    SoundPlayer.Play(gameObject, SoundTagTagImpact);
                }
                else
                {
                    RLHScene.Instance.StonesImpact(_gm.physic.worldCenterOfMass, e2);
                    SoundPlayer.Play(_gm.gameObject, SoundTagTagImpact);
                }
                return;
            }

            RLHScene.Instance.StonesImpact(physic.worldCenterOfMass, e1);
            SoundPlayer.Play(gameObject, SoundTagTagImpact);
        }
    }

    void Awake()
    {
        physic = GetComponent<Rigidbody2D>();
        Transform gfxTransform = transform.Find("gfx");
        if( gfxTransform)
        {
            gfx = gfxTransform.GetComponent<SpriteRenderer>();
            gfxStaticPos = gfx.transform.localPosition;
        }

        //boxCollider = GetComponent<BoxCollider2D>();

        if (GetComponent<PolygonCollider2D>())
        {
            PolygonCollider2D coll = GetComponent<PolygonCollider2D>();
            normals = new Vector2[coll.points.Length];
            handles = new Vector2[coll.points.Length];
            handlesActive = new bool[coll.points.Length];
            isClockwise = PolygonIsClockwise(coll.points);
            UpdateWorldNormalsAndHandles(coll, IsHanging());
        }
        else if (GetComponent<BoxCollider2D>())
        {
            BoxCollider2D coll = GetComponent<BoxCollider2D>();
            normals = new Vector2[4];
            handles = new Vector2[4];
            handlesActive = new bool[4];
            UpdateWorldNormalsAndHandles(coll, IsHanging());
        }
        else
        {
            Debug.LogError("GroundMoveable nie ma colidera");
        }


        //lastSleeping = false;

        isCollapsableFootbridge = GetComponent<CollapseableFootbridge>();
        if (isCollapsableFootbridge && !IsHanging())
        {
            Debug.LogError(name + " to ma byc kladka a nie jest kinematyczna. Przestawiam.");
            //Debug.Break();
            physic.isKinematic = true;
        }
        SaveResets();
    }

    // Use this for initialization
    void Start()
    {
        lastSleeped = false;
        //connectedRope 
    }

    public Vector2 lastVelocity = new Vector2();
    void FixedUpdate()
    {
        //print("Grou::FixedUpdate() " + Time.time);
        lastVelocity = physic.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        //applyShake();

        if (physic.IsSleeping())
        {
            if (!lastSleeped)
            {
                if (GetComponent<PolygonCollider2D>())
                {
                    UpdateWorldNormalsAndHandles(GetComponent<PolygonCollider2D>(), false);
                }
                else if (GetComponent<BoxCollider2D>())
                {
                    UpdateWorldNormalsAndHandles(GetComponent<BoxCollider2D>(), false);
                }
                else
                {
                    Debug.LogError("GroundMoveable nie ma colidera");
                }
            }
            DrawEdgesAndNormals();
        }

        lastSleeped = physic.IsSleeping();
    }

    public void SaveResets()
    {
        //gameObject.layer = LayerMask.NameToLayer("LA-GROUND");
        resetLayer = gameObject.layer;
        resetPosition = physic.position; // transform.position;
        resetRotation = physic.rotation; // transform.rotation;      
        resetVelocity = physic.velocity;
        resetAngularVelocity = physic.angularVelocity;
        resetHanging = IsHanging();
        resetActive = gameObject.activeSelf;
    }
    public void Reset()
    {
        if (!physic)
        {
            print(gameObject.name);
            return;
        }
        gameObject.SetActive(resetActive);

        gameObject.layer = resetLayer;
        physic.position = resetPosition;
        physic.rotation = resetRotation;
        physic.velocity = resetVelocity;
        physic.angularVelocity = resetAngularVelocity;
        SetHanging(resetHanging);
        if (GetComponent<CollapseableFootbridge>())
        {
            GetComponent<CollapseableFootbridge>().Reset();
        }
        
    }

    void CalculateFakeCenterOfMass(Vector2[] points)
    {
        Vector2 pointsSum = new Vector2();
        for (int p = 0; p < points.Length; ++p)
        {
            pointsSum += points[p];
        }
        fakeWorldCenterOfMass = transform.TransformPoint(pointsSum / points.Length);
    }

    void UpdateWorldNormalsAndHandles(PolygonCollider2D coll, bool calculateFakeCenterOfMass)
    {
        if (calculateFakeCenterOfMass)
        {
            CalculateFakeCenterOfMass(coll.points);
        }

        int i = 0;
        for (; i < coll.points.Length - 1; ++i)
        {
            handles[i] = transform.TransformPoint(coll.points[i]);
            AddNormal(i, coll.points[i], coll.points[i + 1]);
        }
        handles[i] = transform.TransformPoint(coll.points[i]);
        AddNormal(i, coll.points[i], coll.points[0]);
        UpdateHandlesActivated();
    }

    void UpdateWorldNormalsAndHandles(BoxCollider2D coll, bool calculateFakeCenterOfMass)
    {
        float top = coll.offset.y + (coll.size.y * 0.5f);
        float btm = coll.offset.y - (coll.size.y * 0.5f);
        float left = coll.offset.x - (coll.size.x * 0.5f);
        float right = coll.offset.x + (coll.size.x * 0.5f);

        handles[0] = new Vector2(left, btm);
        handles[1] = new Vector2(right, btm);
        handles[2] = new Vector2(right, top);
        handles[3] = new Vector2(left, top);

        if (calculateFakeCenterOfMass)
        {
            CalculateFakeCenterOfMass(handles);
        }

        isClockwise = PolygonIsClockwise(handles);

        handles[0] = transform.TransformPoint(handles[0]);
        handles[1] = transform.TransformPoint(handles[1]);
        handles[2] = transform.TransformPoint(handles[2]);
        handles[3] = transform.TransformPoint(handles[3]);

        int i = 0;
        for (; i < handles.Length - 1; ++i)
        {
            AddNormal2(i, handles[i], handles[i + 1]);
        }
        AddNormal2(i, handles[i], handles[0]);
        UpdateHandlesActivated();
    }

    void UpdateHandlesActivated()
    {
        int i = 0;
        float wnAngle = 0.0f;
        bool b = false;

        for (; i < handlesActive.Length; ++i)
        {
            handlesActive[i] = false;
        }
        for (i = 0; i < normals.Length - 1; ++i)
        {
            wnAngle = Vector2.Angle(Vector2.up, normals[i]);
            b = Mathf.Abs(wnAngle) < 45.0f;
            if (b)
            {
                handlesActive[i] = true;
                handlesActive[i + 1] = true;
            }
        }
        wnAngle = Vector2.Angle(Vector2.up, normals[i]);
        b = Mathf.Abs(wnAngle) < 45.0f;
        if (b)
        {
            handlesActive[i] = true;
            handlesActive[0] = true;
        }
    }

    //Vector2 p1;
    //Vector2 p2;
    Vector2 pn1;
    Vector2 pn2;
    Vector2 wn;

    public bool IsCollapsableFootbridge
    {
        get { return isCollapsableFootbridge; }
    }

    void AddNormal(int index, Vector2 p1, Vector2 p2)
    {
        pn1 = transform.TransformPoint(p1);
        pn2 = transform.TransformPoint(p2);

        Vector2 diff = pn2 - pn1;
        normals[index] = diff.Rotate(isClockwise ? 90 : -90).normalized * 0.25f;

        //float wnAngle = Vector2.Angle(Vector2.up, normals[index]);
        //handlesActive[index] = Mathf.Abs(wnAngle) < 45.0f;
    }
    void AddNormal2(int index, Vector2 p1, Vector2 p2)
    {
        //pn1 = transform.TransformPoint(p1);
        //pn2 = transform.TransformPoint(p2);

        Vector2 diff = p2 - p1;
        normals[index] = diff.Rotate(isClockwise ? 90 : -90).normalized * 0.25f;

        //float wnAngle = Vector2.Angle(Vector2.up, normals[index]);
        //handlesActive[index] = Mathf.Abs(wnAngle) < 45.0f;
    }

    void DrawEdgesAndNormals()
    {
        int i = 0;
        for (; i < handles.Length - 1; ++i)
        {
            DrawEdgeAndNormal(handles[i], handles[i + 1], i);
        }
        DrawEdgeAndNormal(handles[i], handles[0], i);
    }

    void DrawEdgeAndNormal(Vector2 p1, Vector2 p2, int i)
    {
        pn1 = p1 + (p2 - p1) * 0.5f;
        pn2 = pn1 + normals[i];
        Debug.DrawLine(pn1, pn2);

        float wnAngle = Vector2.Angle(Vector2.up, normals[i]);
        if (Mathf.Abs(wnAngle) < 45.0f)
        {
            Debug.DrawLine(p1, p2, Color.red);
        }
        else
        {
            Debug.DrawLine(p1, p2, Color.blue);
        }

        if (handlesActive[i])
        {
            Debug.DrawLine(handles[i] + new Vector2(-0.1f, -0.1f), handles[i] + new Vector2(0.1f, 0.1f), Color.red);
            Debug.DrawLine(handles[i] + new Vector2(-0.1f, 0.1f), handles[i] + new Vector2(0.1f, -0.1f), Color.red);
        }
    }

    //void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        //print("GetMouseButtonDown");
    //    }
    //}

    //public void printWorldVertices()
    //{
    //    float top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
    //    float btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
    //    float left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
    //    float right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

    //    Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
    //    Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
    //    Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
    //    Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

    //    Debug.Log(topLeft + " " + topRight + " " + btmLeft + " " + btmRight);

    //    //float rot = transform.rotation.eulerAngles.z;
    //}

    bool PolygonIsClockwise(params Vector2[] points)
    {
        int l = points.Length;

        float sum = 0f;

        for (int i = 0; i < l; i++)
        {
            int n = i + 1 >= l - 1 ? 0 : i + 1;

            float x = points[n].x - points[i].x;
            float y = points[n].y + points[i].y;
            sum += (x * y);
        }

        return (sum < 0) ? false : true;
    }

    public bool handleToPullDownTouched(Vector2 zapDir, Vector2 worldTouch, ref Vector2 handle, float maxDist = 0.25f)
    {

        //if ((topRight - worldTouch).magnitude < maxDist)
        //    //        {
        //    //            handle = topRight;
        //    //            return true;
        //    //        }

        for (int i = 0; i < handles.Length; ++i)
        {
            if (!handlesActive[i]) continue;
            handle = handles[i];
            if ((handle - worldTouch).magnitude < maxDist)
            {
                return true;
            }
        }

        return false;

        //float rot = transform.rotation.eulerAngles.z;

        //float top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
        //float btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
        //float left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
        //float right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

        //Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        //Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        //Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        //Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        //if (rot > (90f - maxTilt) && rot < (90 + maxTilt))
        //{
        //    if (zapDir == Vector2.right)
        //    {
        //        if ((topRight - worldTouch).magnitude < maxDist)
        //        {
        //            handle = topRight;
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        if ((btmRight - worldTouch).magnitude < maxDist)
        //        {
        //            handle = btmRight;
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //if (rot > (180f - maxTilt) && rot < (180 + maxTilt))
        //{
        //    if (zapDir == Vector2.right)
        //    {
        //        if ((btmRight - worldTouch).magnitude < maxDist)
        //        {
        //            handle = btmRight;
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        if ((btmLeft - worldTouch).magnitude < maxDist)
        //        {
        //            handle = btmLeft;
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //if (rot > (270f - maxTilt) && rot < (270 + maxTilt))
        //{
        //    if (zapDir == Vector2.right)
        //    {
        //        if ((btmLeft - worldTouch).magnitude < maxDist)
        //        {
        //            handle = btmLeft;
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        if ((topLeft - worldTouch).magnitude < maxDist)
        //        {
        //            handle = topLeft;
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //if (rot > (360 - maxTilt) || rot < maxTilt)
        //{
        //    if (zapDir == Vector2.right)
        //    {
        //        if ((topLeft - worldTouch).magnitude < maxDist)
        //        {
        //            handle = topLeft;
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        if ((topRight - worldTouch).magnitude < maxDist)
        //        {
        //            handle = topRight;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //return false;
    }

    public bool handleTouched(Vector3 worldTouch, ref Vector2 handle, float maxTilt = 5f, float maxDist = 0.25f)
    {
        return false;

        //float rot = transform.rotation.eulerAngles.z;

        //float top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
        //float btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
        //float left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
        //float right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

        //Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        //Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        //Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        //Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        //if (rot > (90f - maxTilt) && rot < (90 + maxTilt))
        //{
        //    if ((topRight - worldTouch).magnitude < maxDist)
        //    {
        //        handle = topRight;
        //        return true;
        //    }
        //    if ((btmRight - worldTouch).magnitude < maxDist)
        //    {
        //        handle = btmRight;
        //        return true;
        //    }
        //    return false;
        //}
        //if (rot > (180f - maxTilt) && rot < (180 + maxTilt))
        //{
        //    if ((btmLeft - worldTouch).magnitude < maxDist)
        //    {
        //        handle = btmLeft;
        //        return true;
        //    }
        //    if ((btmRight - worldTouch).magnitude < maxDist)
        //    {
        //        handle = btmRight;
        //        return true;
        //    }
        //    return false;
        //}
        //if (rot > (270f - maxTilt) && rot < (270 + maxTilt))
        //{
        //    if ((topLeft - worldTouch).magnitude < maxDist)
        //    {
        //        handle = topLeft;
        //        return true;
        //    }
        //    if ((btmLeft - worldTouch).magnitude < maxDist)
        //    {
        //        handle = btmLeft;
        //        return true;
        //    }
        //    return false;
        //}
        //if (rot > (360 - maxTilt) || rot < maxTilt)
        //{
        //    if ((topLeft - worldTouch).magnitude < maxDist)
        //    {
        //        handle = topLeft;
        //        return true;
        //    }
        //    if ((topRight - worldTouch).magnitude < maxDist)
        //    {
        //        handle = topRight;
        //        return true;
        //    }
        //    return false;
        //}

        //return false;
    }
}
