using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour
{
    float quaverDuration = 4f;
    float quaverTime = 0;
    Vector3 quaverStartPos;
    Vector3 quaverPos = new Vector3(0f, 0f, 0f);
    bool quavering = false;

    // Use this for initialization
    void Start()
    {
        //quaverBegin();
    }

    // Update is called once per frame
    void Update()
    {
        if (quavering) quaverStep(Time.deltaTime);
        else quaverBegin();
    }

    void quaverBegin()
    {
        quavering = true;
        quaverTime = 0f;
        quaverStartPos = transform.position;
    }

    void quaverFinish()
    {
        quavering = false;
    }

    void quaverStep(float deltaTime)
    {
        bool _quaverEnded = false;
        quaverTime += deltaTime;
        if (quaverTime >= quaverDuration)
        {
            quaverTime = quaverDuration;
            _quaverEnded = true;
        }

        float timeRatio = quaverTime / quaverDuration;

        quaverPos.x = Mathf.Sin(timeRatio * Mathf.PI * 2) * 3.0f;
        quaverPos.y = Mathf.Sin(timeRatio * Mathf.PI * 4) * 0.5f;

        transform.position = quaverStartPos + quaverPos;

        if (_quaverEnded)
            quaverFinish();
    }
}
