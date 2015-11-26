using UnityEngine;
using System.Collections;

public class SwingAlpha : MonoBehaviour
{
    //public float RevealOpacity = 0.3f;
    public Vector2 Range = new Vector2(0.25f, 0.9f);
    public Vector2 Speed = new Vector2(0.25f,0.9f);
    
    float currentSpeed = 0f;
    float currentAlpha = 1f;
    float targetAlpha = 1f;

    // Use this for initialization
    void Start()
    {
        currentAlpha = GetCurrentOpactiy();
        targetAlpha = currentAlpha;
        StartSwing();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAlpha != targetAlpha)
        {
            if (currentAlpha > targetAlpha)
            {
                currentAlpha = Mathf.Max(currentAlpha - currentSpeed * Time.deltaTime, targetAlpha);
            }
            else
            {
                currentAlpha = Mathf.Min(currentAlpha + currentSpeed * Time.deltaTime, targetAlpha);
            }
            SetChildenOpacity(transform, currentAlpha);
            if( currentAlpha == targetAlpha)
            {
                StartSwing();
            }
            //currentOpacity = targetOpacity;
        }
    }

    void StartSwing()
    {
        targetAlpha = Random.Range(Range.x,Range.y);
        currentSpeed = Random.Range(Speed.x, Speed.y);
    }

    void SetChildenOpacity(Transform parent, float newOpacity)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer)
        {
            Color oldColor = renderer.color;
            oldColor.a = newOpacity;
            renderer.color = oldColor;
        }
    }

    float GetCurrentOpactiy()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer)
        {
            return renderer.color.a;
        }
        return 0f;
    }
}
