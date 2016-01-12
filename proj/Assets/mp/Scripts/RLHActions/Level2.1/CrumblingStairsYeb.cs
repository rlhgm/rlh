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

    void Start()
    {
        stone1 = CrumblingStairsYebObjects.transform.Find("stone1").gameObject;
        stone2 = CrumblingStairsYebObjects.transform.Find("stone2").gameObject;
        stone1.SetActive(false);
        stone2.SetActive(false);

        lightray1 = CrumblingStairsYebObjects.transform.Find("lightray1").gameObject;
        lightray2 = CrumblingStairsYebObjects.transform.Find("lightray2").gameObject;
        lightray1.SetActive(false);
        lightray2.SetActive(false);
    }

    protected override int PerformSpec()
    {     
        RLHScene.Instance.CamController.ShakeImpulseStart(2f, 0.3f, 8f);
        stone1.SetActive(true);
        stone2.SetActive(true);
        lightray1.SetActive(true);
        lightray2.SetActive(true);
        return 0;
    }
}
