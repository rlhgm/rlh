using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public float ShakeAmplitude = 0.25f;
    public float ShakeSpeed = 8f;
    public float ShakeFadeOutDuration = 0.5f;

    //// Use this for initialization
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void OnTriggerEnter2D(Collider2D collider)
    {
        //print("CameraShaker::OnTriggerEnter2D");
        //RLHScene.Instance.Zap.CameraTargetOffset = CameraOffset;
        RLHScene.Instance.CamController.ShakePermanentStart(ShakeAmplitude,ShakeSpeed);
    }

    void OnTriggerExit2D()
    {
        //print("CameraShaker::OnTriggerExit2D");
        //RLHScene.Instance.Zap.CameraTargetOffset = new Vector3(0f, 0f, 0f);
        RLHScene.Instance.CamController.ShakeStop(ShakeFadeOutDuration);
    }

    //void OnTriggerStay2D()
    //{
    //    print("CameraTargetOffseter::OnTriggerStay2D");
    //}
}
