using UnityEngine;
using System.Collections;

public class CuttedChandelierRope : RLHAction
{
    public Chandelier chandelier = null;
    public CutSceneCameraPassing CameraPassing = null;

    protected override int PerformSpec()
    {
        //Chandelier chandelier = attachedStone.GetComponent<Chandelier>();
        if (chandelier)
        {
            chandelier.gameObject.layer = LayerMask.NameToLayer("GroundMoveable");
        }
        if (CameraPassing)
        {
            CameraPassing.gameObject.SetActive(true);
        }

        return 0;
    }
}
