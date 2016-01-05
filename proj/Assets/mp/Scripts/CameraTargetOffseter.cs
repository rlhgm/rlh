using UnityEngine;
using System.Collections;

public class CameraTargetOffseter : MonoBehaviour
{
    public Vector2 CameraOffset;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //print("CameraTargetOffseter::OnTriggerEnter2D");
        RLHScene.Instance.Zap.CameraTargetOffset = CameraOffset;
    }

    void OnTriggerExit2D()
    {
        //print("CameraTargetOffseter::OnTriggerExit2D");
        RLHScene.Instance.Zap.CameraTargetOffset = new Vector3(0f, 0f, 0f);
    }

    //void OnTriggerStay2D()
    //{
    //    print("CameraTargetOffseter::OnTriggerStay2D");
    //}
}
