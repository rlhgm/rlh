using UnityEngine;
using System.Collections;

public class CrumblingStairsYeb : RLHAction
{
    //public Chandelier chandelier = null;
    //public CutSceneCameraPassing CameraPassing = null;
    public GameObject CrumblingStairsYebObjects = null;

    GameObject stone1;
    GameObject stone2;
    GameObject lightray1;
    GameObject lightray2;

    bool resetLightRay1 = false;
    bool resetLightRay2 = false;

    public override void SaveResets()
    {     
        stone1.GetComponent<GroundMoveable>().SaveResets();
        stone2.GetComponent<GroundMoveable>().SaveResets();

        resetLightRay1 = lightray1.activeSelf;
        resetLightRay2 = lightray2.activeSelf;
    }

    public override void Reset()
    {
        performTime = 0f;
        performed = false;
        stone1.GetComponent<GroundMoveable>().Reset();
        stone2.GetComponent<GroundMoveable>().Reset();
        lightray1.SetActive(resetLightRay1);
        lightray2.SetActive(resetLightRay2);
    }

    float performTime = 0f;

    void Start()
    {
        stone1 = CrumblingStairsYebObjects.transform.Find("stone1").gameObject;
        stone2 = CrumblingStairsYebObjects.transform.Find("stone2").gameObject;
        //stone1.SetActive(false);
        //stone2.SetActive(false);

        lightray1 = CrumblingStairsYebObjects.transform.Find("lightray1").gameObject;
        lightray2 = CrumblingStairsYebObjects.transform.Find("lightray2").gameObject;
        lightray1.SetActive(false);
        lightray2.SetActive(false);
    }

    void Update()
    {
        if (performed)
        {
            performTime += Time.deltaTime;
            if( performTime > 4f )
            {
                stone1.SetActive(false);
                stone2.SetActive(false);
            }
            //Vector2 chandelierPosDiff = chandelier.transform.position - chandelierStartPos;
            //Vector3 cto = chandelierPosDiff;
            //RLHScene.Instance.Zap.CameraTargetOffset = cto;
        }
    }

    protected override int PerformSpec()
    {
        if (CrumblingStairsYebObjects)
        {
            CrumblingStairsYebCutScene _action = CrumblingStairsYebObjects.GetComponent<CrumblingStairsYebCutScene>();
            if (_action)
            {
                if (_action.SoundTagStart != null) SoundPlayer.Play(CrumblingStairsYebObjects.gameObject, _action.SoundTagStart);
            }
        }

        RLHScene.Instance.CamController.ShakeImpulseStart(3f, 0.35f, 8f);
        //stone1.SetActive(true);
        //stone2.SetActive(true);
        stone1.GetComponent<Rigidbody2D>().isKinematic = false;
        stone2.GetComponent<Rigidbody2D>().isKinematic = false;
        lightray1.SetActive(true);
        lightray2.SetActive(true);
        return 0;
    }
}
