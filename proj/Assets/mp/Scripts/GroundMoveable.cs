using UnityEngine;
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
    Vector2[] normals;

    bool lastSleeped = false;

    void Awake()
    {
        physic = GetComponent<Rigidbody2D>();

        //boxCollider = GetComponent<BoxCollider2D>();

        if (GetComponent<PolygonCollider2D>())
        { 
            PolygonCollider2D coll = GetComponent<PolygonCollider2D>();
            normals = new Vector2[coll.points.Length];
            handles = new Vector2[coll.points.Length];
            updateWorldNormalsAndHandles(coll);
        }
        else if(GetComponent<BoxCollider2D>())
        {
            BoxCollider2D coll = GetComponent<BoxCollider2D>();
            normals = new Vector2[4];
            handles = new Vector2[4];
            updateWorldNormalsAndHandles(coll);
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
                    updateWorldNormalsAndHandles(GetComponent<PolygonCollider2D>());
                }
                else if (GetComponent<BoxCollider2D>())
                {
                    updateWorldNormalsAndHandles(GetComponent<BoxCollider2D>());
                }
                else
                {
                    Debug.LogError("GroundMoveable nie ma colidera");
                }
            }
            drawEdgesAndNormals();
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

    void updateWorldNormalsAndHandles(PolygonCollider2D coll)
    {
        int i = 0;
        for (; i < coll.points.Length - 1; ++i)
        {
            handles[i] = transform.TransformPoint(coll.points[i]);            
            addNormal(i, coll.points[i], coll.points[i + 1]);
        }
        handles[i] = transform.TransformPoint(coll.points[i]);
        addNormal(i, coll.points[i], coll.points[0]);
    }

    void updateWorldNormalsAndHandles(BoxCollider2D coll)
    {
        //int i = 0;
        //for (; i < coll.points.Length - 1; ++i)
        //{
        //    //print(coll.points[i]);
        //    addNormal(i, coll.points[i], coll.points[i + 1]);
        //}
        //addNormal(i, coll.points[i], coll.points[0]);
    }

    //Vector2 p1;
    //Vector2 p2;
    Vector2 pn1;
    Vector2 pn2;
    Vector2 wn;
    void addNormal(int index, Vector2 p1, Vector2 p2)
    {
        //Vector2 diff = p2 - p1;
        //normals[index] = diff.Rotate(90).normalized * 0.25f;

        //Vector2 _p1 = p1 + (p2 - p1) * 0.5f;
        //pn1 = transform.TransformPoint(_p1);
        //pn2 = transform.TransformPoint(_p1 + normals[i]);
        //Debug.DrawLine(pn1, pn2);

        //wn = pn2 - pn1;
        //float wnAngle = Vector2.Angle(Vector2.up, wn);

        //if (Mathf.Abs(wnAngle) < 45.0f)
        //{
        //    Debug.DrawLine(transform.TransformPoint(p1), transform.TransformPoint(p2), Color.red);
        //}

        pn1 = transform.TransformPoint(p1);
        pn2 = transform.TransformPoint(p2);

        Vector2 diff = pn2 - pn1;
        normals[index] = diff.Rotate(90).normalized * 0.25f;

    }

    void drawEdgesAndNormals()
    {
        
        int i = 0;
        for (; i < handles.Length - 1; ++i)
        {
            drawEdgeAndNormal(handles[i], handles[i + 1]);
        }
        drawEdgeAndNormal(handles[i], handles[0]);
    }
    
    void drawEdgeAndNormal(Vector2 p1, Vector2 p2)
    {
        Debug.DrawLine(p1, p2);

        ////p1 = coll.points[p1Ind];
        ////p2 = coll.points[p2Ind];
        //Vector2 _p1 = p1 + (p2 - p1) * 0.5f;
        //pn1 = transform.TransformPoint(_p1);
        //pn2 = transform.TransformPoint(_p1 + normals[i]);
        //Debug.DrawLine(pn1, pn2);
        ////return pn2-pn1;

        //wn = pn2 - pn1;
        //float wnAngle = Vector2.Angle(Vector2.up, wn);

        //if (Mathf.Abs(wnAngle) < 45.0f)
        //{
        //    Debug.DrawLine(transform.TransformPoint(p1), transform.TransformPoint(p2), Color.red);
        //}
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
