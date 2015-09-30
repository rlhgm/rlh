using UnityEngine;
using System.Collections;

public class Twitchable : MonoBehaviour {

    Vector3 startPositon;
    Vector3 currentPosition;
    float time = 0f;

    public Vector2 TwithRange = new Vector2(1f, 1f);
    public Vector2 TwithTime = new Vector2(1f, 1f);

    // Use this for initialization
    void Start()
    {
        startPositon = transform.position;
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        currentPosition = startPositon;

        if (TwithTime.x > 0f && TwithRange.x > 0f)
        {
            float _dtx = (time / TwithTime.x) * Mathf.PI * 2f;
            float dtx = Mathf.Sin(_dtx);
            currentPosition.x += dtx * TwithRange.x;
        }
        
        if (TwithTime.y > 0f && TwithRange.y > 0f)
        {
            float _dty = (time / TwithTime.y) * Mathf.PI * 2f;
            float dty = Mathf.Sin(_dty);
            currentPosition.y += dty * TwithRange.y;
        }
        
        transform.position = currentPosition;
    }
}
