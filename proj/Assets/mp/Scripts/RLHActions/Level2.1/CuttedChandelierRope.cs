using UnityEngine;
using System.Collections;

public class CuttedChandelierRope : RLHAction
{
    public Chandelier chandelier = null;
    public CutSceneCameraPassing CameraPassing = null;

    Vector3 chandelierStartPos = new Vector3();

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
