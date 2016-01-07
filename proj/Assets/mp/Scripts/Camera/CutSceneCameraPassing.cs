using UnityEngine;
using System.Collections;

public class CutSceneCameraPassing : MonoBehaviour, IGResetable
{
    public CutSceneCameraPassingControlPoint next = null;

    public bool OneTimeOnly = false;
    public bool PauseGame = false;
    public bool StopZap = false;
    //bool active = true;

    // Use this for initialization
    void Start()
    {
        if (!next)
        {
            Debug.LogError("CutSceneCameraPassing " + name + "nie ma ustawionego CutSceneCameraPassingControlPoint'a : next");
            Debug.Break();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //print("CutSceneCameraPassing::OnTriggerEnter2D");
        //RLHScene.Instance.Zap.CameraTargetOffset = CameraOffset;

        //Vector3 startPosition = RLHScene.Instance.CamController.myCamera.transform.position;

        //RLHScene.Instance.CamController.StartCutScene(this);
        RLHScene.Instance.StartCutScene(this);

        if (OneTimeOnly)
        {
            gameObject.SetActive(false);
        }
    }

    //void OnTriggerExit2D()
    //{
    //    //print("CutSceneCameraPassing::OnTriggerExit2D");
    //    //RLHScene.Instance.Zap.CameraTargetOffset = new Vector3(0f, 0f, 0f);
    //}

    //void OnTriggerStay2D()
    //{
    //    print("CameraTargetOffseter::OnTriggerStay2D");
    //}

    bool startActive;

    public void GCacheResetData()
    {
        startActive = gameObject.activeSelf;
    }

    public void GReset()
    {
        gameObject.SetActive(startActive);
    }
}
