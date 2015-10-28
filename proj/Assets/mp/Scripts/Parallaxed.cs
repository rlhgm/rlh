using UnityEngine;
using System.Collections;

public class Parallaxed : MonoBehaviour
{
    public Vector2 parallaxRatio = new Vector2(0f, 0f);

    Vector3 startPosition;
    Vector2 diff;
    Vector3 newPos;

    void Awake()
    {
        startPosition = transform.position;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PUpdate(Vector3 cameraPos)
    {
        diff = cameraPos - startPosition;
        newPos = startPosition;
        newPos.x += diff.x * parallaxRatio.x;
        newPos.y += diff.y * parallaxRatio.y;
        transform.position = newPos;
    }
}
