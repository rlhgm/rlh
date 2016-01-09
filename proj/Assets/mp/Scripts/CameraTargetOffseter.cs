using UnityEngine;
using System.Collections;

public class CameraTargetOffseter : MonoBehaviour
{
    public Vector2 CameraOffset;
    public Vector2 ToOffsetSpeedInOut = new Vector2(1f,1f);

    // Use this for initialization
    void Start()
    {
        //print(GetType());
        //if(ToOffsetSpeed <= 0f)
        //{
        //    //Debug.LogError();
        //    RLHScene.RLHAssert(ToOffsetSpeed > 0f, "CameraTargetOffseter");
        //}
        this.RLHAssert(ToOffsetSpeedInOut.x > 0f, "ToOffsetSpeed musi byc wiekszy od 0");
        this.RLHAssert(ToOffsetSpeedInOut.y > 0f, "ToOffsetSpeed musi byc wiekszy od 0");
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void OnTriggerEnter2D(Collider2D collider)
    {
        //print("CameraTargetOffseter::OnTriggerEnter2D");
        //RLHScene.Instance.Zap.CamTargetOffseter = this;
        RLHScene.Instance.Zap.CameraOffsetIn(this);
    }

    void OnTriggerExit2D()
    {
        //print("CameraTargetOffseter::OnTriggerExit2D");
        //RLHScene.Instance.Zap.CameraTargetOffset = new Vector3(0f, 0f, 0f);
        RLHScene.Instance.Zap.CameraOffsetOut(this);
    }

    //void OnTriggerStay2D()
    //{
    //    print("CameraTargetOffseter::OnTriggerStay2D");
    //}
}
