using UnityEngine;
using System.Collections;

public class GroundMoveable : MonoBehaviour
{
    Vector2 resetPosition;
    float resetRotation;
    Vector2 resetVelocity;
    BoxCollider2D boxCollider = null;
    Rigidbody2D physic = null;

    void Awake()
    {
        physic = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        SaveResets();
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveResets()
    {
        resetPosition = physic.position; // transform.position;
        resetRotation = physic.rotation; // transform.rotation;      
        resetVelocity = physic.velocity;
    }
    public void Reset()
    {
        physic.position = resetPosition;
        physic.rotation = resetRotation;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //print("GetMouseButtonDown");
        }
    }

    public void printWorldVertices()
    {
        float top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
        float btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
        float left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
        float right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

        Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        Debug.Log(topLeft + " " + topRight + " " + btmLeft + " " + btmRight);

        //float rot = transform.rotation.eulerAngles.z;
    }

    public bool handleToPullDownTouched(Vector2 zapDir, Vector3 worldTouch, ref Vector2 handle, float maxTilt = 5f, float maxDist = 0.25f)
    {
        float rot = transform.rotation.eulerAngles.z;

        float top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
        float btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
        float left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
        float right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

        Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        if (rot > (90f - maxTilt) && rot < (90 + maxTilt))
        {
            if (zapDir == Vector2.right)
            {
                if ((topRight - worldTouch).magnitude < maxDist)
                {
                    handle = topRight;
                    return true;
                }
            }
            else
            {
                if ((btmRight - worldTouch).magnitude < maxDist)
                {
                    handle = btmRight;
                    return true;
                }
            }
            return false;
        }
        if (rot > (180f - maxTilt) && rot < (180 + maxTilt))
        {
            if (zapDir == Vector2.right)
            {
                if ((btmRight - worldTouch).magnitude < maxDist)
                {
                    handle = btmRight;
                    return true;
                }
            }
            else
            {
                if ((btmLeft - worldTouch).magnitude < maxDist)
                {
                    handle = btmLeft;
                    return true;
                }
            }
            return false;
        }
        if (rot > (270f - maxTilt) && rot < (270 + maxTilt))
        {
            if (zapDir == Vector2.right)
            {
                if ((btmLeft - worldTouch).magnitude < maxDist)
                {
                    handle = btmLeft;
                    return true;
                }
            }
            else
            {
                if ((topLeft - worldTouch).magnitude < maxDist)
                {
                    handle = topLeft;
                    return true;
                }
            }
            return false;
        }
        if (rot > (360 - maxTilt) || rot < maxTilt)
        {
            if (zapDir == Vector2.right)
            {
                if ((topLeft - worldTouch).magnitude < maxDist)
                {
                    handle = topLeft;
                    return true;
                }
            }
            else
            {
                if ((topRight - worldTouch).magnitude < maxDist)
                {
                    handle = topRight;
                    return true;
                }
            }
            return false;
        }

        return false;
    }

    public bool handleTouched(Vector3 worldTouch, ref Vector2 handle, float maxTilt = 5f, float maxDist = 0.25f)
    {
        float rot = transform.rotation.eulerAngles.z;

        float top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
        float btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
        float left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
        float right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

        Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        if (rot > (90f - maxTilt) && rot < (90 + maxTilt))
        {
            if ((topRight - worldTouch).magnitude < maxDist)
            {
                handle = topRight;
                return true;
            }
            if ((btmRight - worldTouch).magnitude < maxDist)
            {
                handle = btmRight;
                return true;
            }
            return false;
        }
        if (rot > (180f - maxTilt) && rot < (180 + maxTilt))
        {
            if ((btmLeft - worldTouch).magnitude < maxDist)
            {
                handle = btmLeft;
                return true;
            }
            if ((btmRight - worldTouch).magnitude < maxDist)
            {
                handle = btmRight;
                return true;
            }
            return false;
        }
        if (rot > (270f - maxTilt) && rot < (270 + maxTilt))
        {
            if ((topLeft - worldTouch).magnitude < maxDist)
            {
                handle = topLeft;
                return true;
            }
            if ((btmLeft - worldTouch).magnitude < maxDist)
            {
                handle = btmLeft;
                return true;
            }
            return false;
        }
        if (rot > (360 - maxTilt) || rot < maxTilt)
        {
            if ((topLeft - worldTouch).magnitude < maxDist)
            {
                handle = topLeft;
                return true;
            }
            if ((topRight - worldTouch).magnitude < maxDist)
            {
                handle = topRight;
                return true;
            }
            return false;
        }

        return false;
    }
}
