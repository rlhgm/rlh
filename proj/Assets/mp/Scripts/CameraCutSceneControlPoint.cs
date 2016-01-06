using UnityEngine;
using System.Collections;

public class CameraCutSceneControlPoint : MonoBehaviour
{
    public CameraCutSceneControlPoint next = null;

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
