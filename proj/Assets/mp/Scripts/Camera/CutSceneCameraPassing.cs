using UnityEngine;
using System.Collections;

public class CutSceneCameraPassing : MonoBehaviour, IGResetable
{
    public CutSceneCameraPassingControlPoint next = null;

    public bool OneTimeOnly = false;
    public bool PauseGame = false;
    public bool StopZap = false;
    //bool active = true;
    public string skipText = "press to skip";
    public float toSkipDuration = 1f;

    public string SoundTagEnter = "";

    bool spriteVisible = false;
    SpriteRenderer spriteRenderer = null;

    // Use this for initialization
    void Start()
    {
        if (!next)
        {
            Debug.LogError("CutSceneCameraPassing " + name + "nie ma ustawionego CutSceneCameraPassingControlPoint'a : next");
            Debug.Break();
        }

        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        spriteVisible = spriteRenderer.enabled;

        //GResetCreated();
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

        if (!currentActive) return;

        RLHScene.Instance.StartCutScene(this);

        if (OneTimeOnly)
        {
            //gameObject.SetActive(false);
            currentActive = false;
            if (spriteVisible) spriteRenderer.enabled = false;
        }

        if( SoundTagEnter != "") SoundPlayer.Play(gameObject, SoundTagEnter);
    }

    bool currentActive = true;
    bool startActive;

    //void OnTriggerExit2D()
    //{
    //    //print("CutSceneCameraPassing::OnTriggerExit2D");
    //    //RLHScene.Instance.Zap.CameraTargetOffset = new Vector3(0f, 0f, 0f);
    //}

    //void OnTriggerStay2D()
    //{
    //    print("CameraTargetOffseter::OnTriggerStay2D");
    //}
    
    //public void GResetCreated()
    //{
    //    RLHScene.Instance.AddIGResetable(this);
    //}

    public void GResetCacheResetData()
    {
        //startActive = gameObject.activeSelf;
        startActive = currentActive;// gameObject.activeSelf;
    }

    public void GReset()
    {
        //gameObject.SetActive(startActive);
        currentActive = startActive;
        if (spriteVisible) spriteRenderer.enabled = startActive;
    }
}
