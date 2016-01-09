using UnityEngine;
using System;
using System.Collections.Generic;

//interface IResetable : MonoBehaviour
////public class Zap : MonoBehaviour
//{
//    public void IReset();
//}

public interface IGResetable
{
    //void GResetCreated();
    void GResetCacheResetData();
    void GReset();
}
