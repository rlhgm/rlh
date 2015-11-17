using UnityEngine;
using System.Collections;

public class RatsHole : MonoBehaviour
{
    public RatsHole ExitHole = null;
    
    // Use this for initialization
    void Start()
    {
        if (!ExitHole)
        {
            Debug.LogError("RatsHole : " + name + " nie ma zdefiniowanego wyjscia!");
        }

        boxCollider = GetComponent<BoxCollider2D>();
        exitBoxCollider = ExitHole.GetComponent<BoxCollider2D>();
        findWorldVertices();
    }

    // Update is called once per frame
    void Update()
    {
    }

    BoxCollider2D boxCollider;
    Vector2 topLeft;
    Vector2 topRight;
    Vector2 btmLeft;
    Vector2 btmRight;
    BoxCollider2D exitBoxCollider;
    Vector2 exitTopLeft;
    Vector2 exitTopRight;
    Vector2 exitBtmLeft;
    Vector2 exitBtmRight;
    RaycastHit2D hit;

    public bool EntryIsOpen()
    {
        hit = Physics2D.Linecast(btmLeft, topRight, RLHScene.Instance.layerIdGroundAllMask);
        if (hit.collider) return false;
        hit = Physics2D.Linecast(topLeft, btmRight, RLHScene.Instance.layerIdGroundAllMask);
        if (hit.collider) return false;
        return true;
    }


    public bool ExitIsOpen()
    {
        hit = Physics2D.Linecast(exitBtmLeft, exitTopRight, RLHScene.Instance.layerIdGroundAllMask);
        if (hit.collider) return false;
        hit = Physics2D.Linecast(exitTopLeft, exitBtmRight, RLHScene.Instance.layerIdGroundAllMask);
        if (hit.collider) return false;
        return true;
    }

    void findWorldVertices()
    {
        float top = exitBoxCollider.offset.y + (exitBoxCollider.size.y * 0.5f);
        float btm = exitBoxCollider.offset.y - (exitBoxCollider.size.y * 0.5f);
        float left = exitBoxCollider.offset.x - (exitBoxCollider.size.x * 0.5f);
        float right = exitBoxCollider.offset.x + (exitBoxCollider.size.x * 0.5f);

        exitTopLeft = ExitHole.transform.TransformPoint(new Vector3(left, top, 0f));
        exitTopRight = ExitHole.transform.TransformPoint(new Vector3(right, top, 0f));
        exitBtmLeft = ExitHole.transform.TransformPoint(new Vector3(left, btm, 0f));
        exitBtmRight = ExitHole.transform.TransformPoint(new Vector3(right, btm, 0f));


        //Debug.Log(topLeft + " " + topRight + " " + btmLeft + " " + btmRight);

        top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
        btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
        left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
        right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

        topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        //Debug.Log(topLeft + " " + topRight + " " + btmLeft + " " + btmRight);
    }
}
