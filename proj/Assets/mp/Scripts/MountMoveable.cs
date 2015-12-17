﻿using UnityEngine;
using System.Collections;

public class MountMoveable : MonoBehaviour
{
    public bool MovingXEnabled = false;
    public bool MovingYEnabled = false;
    public bool MovingInLocal = false;

    public Vector2 mySize = new Vector2();
    BoxCollider2D myBoxCollider = null;

    // Use this for initialization
    void Start()
    {
        myBoxCollider = GetComponent<BoxCollider2D>();
        if (!myBoxCollider)
        {
            Debug.LogError("MountMoveable : " + name + " nie ma BoxCollider2D");
            Debug.Break();
        }

        mySize.x = myBoxCollider.size.x * transform.localScale.x;
        mySize.y = myBoxCollider.size.y * transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool LocalPointHandable(Vector3 point)
    {
        //Vector3 rlp = new Vector3();
        //rlp.x = point.x * transform.localScale.x;
        //rlp.y = point.y * transform.localScale.y;

        if (point.x < 0.3f || point.x > (mySize.x - 0.3f)) return false;

        return true;
    }
    public Vector3 ConvertToPointSize(Vector3 point)
    {
        Vector3 rlp = new Vector3();
        rlp.x = point.x * transform.localScale.x;
        rlp.y = point.y * transform.localScale.y;
        return rlp;
    }
}