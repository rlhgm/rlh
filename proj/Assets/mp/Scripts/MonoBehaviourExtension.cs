using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MonoBehaviourExtension
{
    //public static Vector2 Rotate(this Vector2 v, float degrees)
    //{
    //    float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
    //    float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

    //    float tx = v.x;
    //    float ty = v.y;
    //    v.x = (cos * tx) - (sin * ty);
    //    v.y = (sin * tx) + (cos * ty);
    //    return v;
    //}

    public static bool RLHAssert(this MonoBehaviour mb, bool cond, string message)
    {
        if (!cond)
        {
            Debug.LogError(mb.GetType() + " : " + mb.gameObject.name + " => " + message);
            Debug.Break();
        }

        return cond;
    }
}
