using UnityEngine;
using System.Collections;

public class SecretPlace : MonoBehaviour
{
    public float RevealOpacity = 0.3f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Zap>())
        {
            setChildenOpacity(transform, RevealOpacity);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Zap>())
        {
            setChildenOpacity(transform, 1.0f);
        }
    }

    void setChildenOpacity(Transform parent, float newOpacity)
    {
        SpriteRenderer renderer = parent.GetComponent<SpriteRenderer>();
        if (renderer)
        {
            Color oldColor = renderer.color;
            oldColor.a = newOpacity;
            renderer.color = oldColor;
        }

        for( int i = 0; i < parent.childCount; ++i )
        {
            setChildenOpacity(parent.GetChild(i), newOpacity);
        }
    }
}
