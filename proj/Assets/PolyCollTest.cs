using UnityEngine;
using System.Collections;

public class PolyCollTest : MonoBehaviour
{
    PolygonCollider2D coll;
    Vector2[] normals;
    // Use this for initialization
    void Start()
    {
        coll = GetComponent<PolygonCollider2D>();
        normals = new Vector2[coll.points.Length];

        //print("===================");
        int i = 0;
        for (; i < coll.points.Length - 1; ++i)
        {
            //print(coll.points[i]);
            addNormal(i, coll.points[i], coll.points[i + 1]);
        }
        addNormal(i, coll.points[i], coll.points[0]);
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        for (; i < coll.points.Length - 1; ++i)
        {
            drawNormal(coll.points[i], coll.points[i + 1], i);
        }
        drawNormal(coll.points[i], coll.points[0], i);
    }

    void addNormal(int index, Vector2 p1, Vector2 p2)
    {
        Vector2 diff = p2 - p1;
        normals[index] = diff.Rotate(90).normalized * 0.25f;
    }

    //Vector2 p1;
    //Vector2 p2;
    Vector2 pn1;
    Vector2 pn2;
    Vector2 wn;

    void drawNormal(Vector2 p1, Vector2 p2, int i)
    {
        //p1 = coll.points[p1Ind];
        //p2 = coll.points[p2Ind];
        Vector2 _p1 = p1 + (p2 - p1) * 0.5f;
        pn1 = transform.TransformPoint(_p1);
        pn2 = transform.TransformPoint(_p1 + normals[i]);
        Debug.DrawLine(pn1, pn2);
        //return pn2-pn1;

        wn = pn2 - pn1;
        float wnAngle = Vector2.Angle(Vector2.up, wn);

        if (Mathf.Abs(wnAngle) < 45.0f)
        {
            Debug.DrawLine( transform.TransformPoint(p1), transform.TransformPoint(p2), Color.red);
        }
    }
}
