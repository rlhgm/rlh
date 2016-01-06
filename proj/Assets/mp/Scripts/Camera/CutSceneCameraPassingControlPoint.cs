using UnityEngine;
using System.Collections;

public class CutSceneCameraPassingControlPoint : MonoBehaviour
{
    public CutSceneCameraPassingControlPoint next = null;
    public float CameraSpeed = 2f;
    public bool EaseIn = false;
    public bool EaseOut = false;
    public float BreakDuration = -1f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (next)
        {
            Debug.DrawLine(transform.position, next.transform.position);
        }
    }
}
