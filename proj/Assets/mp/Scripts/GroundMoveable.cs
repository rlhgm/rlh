﻿using UnityEngine;
using System.Collections;

public class GroundMoveable : MonoBehaviour
{
    Vector2 resetPosition;
    float resetRotation;
    Vector2 resetVelocity;
    float resetAngularVelocity;

    //BoxCollider2D boxCollider = null;
    Rigidbody2D physic = null;
    //NewRope connectedRope;

    Vector2[] handles;
    bool[] handlesActive;
    Vector2[] normals;

    bool lastSleeped = false;
    bool isClockwise = false;

    void Awake()
    {
        physic = GetComponent<Rigidbody2D>();

        //boxCollider = GetComponent<BoxCollider2D>();

        if (GetComponent<PolygonCollider2D>())
        { 
            PolygonCollider2D coll = GetComponent<PolygonCollider2D>();
            normals = new Vector2[coll.points.Length];
            handles = new Vector2[coll.points.Length];
            handlesActive = new bool[coll.points.Length];
            isClockwise = PolygonIsClockwise(coll.points);
            UpdateWorldNormalsAndHandles(coll);
        }
        else if(GetComponent<BoxCollider2D>())
        {
            BoxCollider2D coll = GetComponent<BoxCollider2D>();
            normals = new Vector2[4];
            handles = new Vector2[4];
            handlesActive = new bool[4];
            UpdateWorldNormalsAndHandles(coll);
        }
        else
        {
            Debug.LogError("GroundMoveable nie ma colidera");
        }

        
        //lastSleeping = false;

        SaveResets();
    }

    // Use this for initialization
    void Start()
    {
        lastSleeped = false;
        //connectedRope 
    }

    // Update is called once per frame
    void Update()
    {
        if (physic.IsSleeping())
        {
            if( !lastSleeped)
            {
                if (GetComponent<PolygonCollider2D>())
                {
                    UpdateWorldNormalsAndHandles(GetComponent<PolygonCollider2D>());
                }
                else if (GetComponent<BoxCollider2D>())
                {
                    UpdateWorldNormalsAndHandles(GetComponent<BoxCollider2D>());
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
        resetPosition = physic.position; // transform.position;
        resetRotation = physic.rotation; // transform.rotation;      
        resetVelocity = physic.velocity;
        resetAngularVelocity = physic.angularVelocity;
    }
    public void Reset()
    {
        physic.position = resetPosition;
        physic.rotation = resetRotation;
        physic.velocity = resetVelocity;
        physic.angularVelocity = resetAngularVelocity;
    }

    void UpdateWorldNormalsAndHandles(PolygonCollider2D coll)
    {
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

    void UpdateWorldNormalsAndHandles(BoxCollider2D coll)
    {
        float top = coll.offset.y + (coll.size.y * 0.5f);
        float btm = coll.offset.y - (coll.size.y * 0.5f);
        float left = coll.offset.x - (coll.size.x * 0.5f);
        float right = coll.offset.x + (coll.size.x * 0.5f);
        
        handles[0] = transform.TransformPoint(new Vector3(left, btm, 0f));
        handles[1] = transform.TransformPoint(new Vector3(right, btm, 0f));
        handles[2] = transform.TransformPoint(new Vector3(right, top, 0f));
        handles[3] = transform.TransformPoint(new Vector3(left, top, 0f));

        isClockwise = PolygonIsClockwise(handles);

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
    void AddNormal(int index, Vector2 p1, Vector2 p2)
    {
        pn1 = transform.TransformPoint(p1);
        pn2 = transform.TransformPoint(p2);

        Vector2 diff = pn2 - pn1;
        normals[index] = diff.Rotate(isClockwise?90:-90).normalized * 0.25f;

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

        if( handlesActive[i])
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

    public bool handleToPullDownTouched(Vector2 zapDir, Vector3 worldTouch, ref Vector2 handle, float maxTilt = 5f, float maxDist = 0.25f)
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
