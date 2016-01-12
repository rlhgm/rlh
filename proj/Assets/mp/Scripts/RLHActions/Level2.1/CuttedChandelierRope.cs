using UnityEngine;
using System.Collections;

public class CuttedChandelierRope : RLHAction
{
    public Chandelier chandelier = null;
    public CutSceneCameraPassing CameraPassing = null;
    public StatueToCrumble statueToCrumble = null;

    Vector3 chandelierStartPos = new Vector3();

    void Awake()
    {
        SaveResets();
    }

    public override void SaveResets()
    {
        //stone1.GetComponent<GroundMoveable>().SaveResets();
        //stone2.GetComponent<GroundMoveable>().SaveResets();

        //resetLightRay1 = lightray1.activeSelf;
        //resetLightRay2 = lightray2.activeSelf;

        statueToCrumble.CacheResetData();
    }

    public override void Reset()
    {
        //performTime = 0f;
        //performed = false;
        //stone1.GetComponent<GroundMoveable>().Reset();
        //stone2.GetComponent<GroundMoveable>().Reset();
        //lightray1.SetActive(resetLightRay1);
        //lightray2.SetActive(resetLightRay2);

        statueToCrumble.Reset();
    }

    protected override int PerformSpec()
    {
        //Chandelier chandelier = attachedStone.GetComponent<Chandelier>();
        if (chandelier)
        {
            chandelier.gameObject.layer = LayerMask.NameToLayer("GroundMoveable");
            chandelierStartPos = chandelier.transform.position;
        }
        if (CameraPassing)
        {
            CameraPassing.gameObject.SetActive(true);
        }
        if( statueToCrumble )
        {
            statueToCrumble.chandelierCollider.SetActive(true);
        }

        return 0;
    }

    void Update()
    {
        //if (performed)
        //{
        //    Vector2 chandelierPosDiff = chandelier.transform.position - chandelierStartPos;
        //    Vector3 cto = chandelierPosDiff;
        //    RLHScene.Instance.Zap.CameraTargetOffset = cto;
        //}
    }
}
