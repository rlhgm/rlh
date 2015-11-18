using UnityEngine;
using System.Collections;

public class SecretPlace : MonoBehaviour
{
    public float RevealOpacity = 0.3f;
    public float RevealOpacitySpeed = 0.5f;
    float currentOpacity = 1f;
    float targetOpacity = 1f;
    
    // Use this for initialization
    void Start()
    {
        //startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if( currentOpacity != targetOpacity )
        {
            if( currentOpacity > targetOpacity )
            {
                currentOpacity = Mathf.Max(currentOpacity - RevealOpacitySpeed * Time.deltaTime,targetOpacity);
            }
            else
            {
                currentOpacity = Mathf.Min(currentOpacity + RevealOpacitySpeed * Time.deltaTime, targetOpacity);
            }
            setChildenOpacity(transform, currentOpacity);
            //currentOpacity = targetOpacity;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        targetOpacity = RevealOpacity;

        //if (other.GetComponent<Zap>())
        //{
        //    setChildenOpacity(transform, RevealOpacity);
        //}
    }

    void OnTriggerExit2D(Collider2D other)
    {
        targetOpacity = 1f;

        //if (other.GetComponent<Zap>())
        //{
        //    setChildenOpacity(transform, 1.0f);
        //}
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
