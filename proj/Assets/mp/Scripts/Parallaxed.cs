using UnityEngine;
using System.Collections;

public class Parallaxed : MonoBehaviour
{
    public Vector2 parallaxRatio = new Vector2(0f, 0f);

    Vector3 startPosition;
    Vector2 diff;
    Vector3 newPos;
    Vector3 spriteSize = new Vector3(0f,0f,0f);

    void Awake()
    {
        startPosition = transform.position;

        SpriteRenderer sprRend = GetComponent<SpriteRenderer>();
        if (sprRend)
        {
            spriteSize = sprRend.bounds.extents;
            //startPosition.x -= (spriteSize.x * 0.5f) * parallaxRatio.x;
            //startPosition.x -= 1.0f; // (spriteSize.x * 0.5f);
        }
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
        cameraPos -= spriteSize;
        diff = cameraPos - startPosition;
        newPos = startPosition;
        newPos.x += diff.x * parallaxRatio.x;
        newPos.y += diff.y * parallaxRatio.y;
        transform.position = newPos;
    }
}
