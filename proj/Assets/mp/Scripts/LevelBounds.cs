using UnityEngine;
using System.Collections;

public class LevelBounds : MonoBehaviour
{
    Vector2 sceneMin = new Vector2();
    Vector2 sceneMax = new Vector2();
    Vector4 sceneMinMax = new Vector4();
    Vector2 center2 = new Vector2();
    Vector3 center3 = new Vector3();
    BoxCollider2D boxCollider = null;
    Vector2 sceneSize = new Vector2();

    public Vector2 SceneMin
    {
        get
        {
            return sceneMin;
        }
    }

    public Vector2 SceneMax
    {
        get
        {
            return sceneMax;
        }
    }

    public Vector4 SceneMinMax
    {
        get
        {
            return SceneMinMax;
        }
    }

    public Vector2 Center2
    {
        get
        {
            return center2;
        }
    }

    public Vector3 Center3
    {
        get
        {
            return center3;
        }
    }

    public Vector2 SceneSize
    {
        get
        {
            return sceneSize;
        }
    }


    // Use this for initialization
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (!boxCollider) return;

        float top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
        float btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
        float left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
        float right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

        //Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        //Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        //Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        //Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        //Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        sceneMax = transform.TransformPoint(new Vector3(right, top, 0f));
        sceneMin = transform.TransformPoint(new Vector3(left, btm, 0f));
        sceneMinMax.x = SceneMin.x;
        sceneMinMax.y = SceneMin.y;
        sceneMinMax.z = SceneMin.x;
        sceneMinMax.w = SceneMin.y;
        //Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        center3 = sceneMin + (SceneMax - sceneMin) * 0.5f;
        center2 = center3;

        sceneSize = sceneMax - sceneMin;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
