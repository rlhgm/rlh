using UnityEngine;
using System.Collections;

public class LevelBounds : MonoBehaviour
{
    Vector2 sceneMin = new Vector2();
    Vector2 sceneMax = new Vector2();
    Vector4 sceneMinMax = new Vector4();
    BoxCollider2D boxCollider = null;

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
        sceneMin = transform.TransformPoint(new Vector3(right, top, 0f));
        sceneMax = transform.TransformPoint(new Vector3(left, btm, 0f));
        //sceneMinMax.x
        //Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
