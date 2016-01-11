using UnityEngine;
using System.Collections;

public abstract class RLHAction : MonoBehaviour
{
    public int Perform()
    {
        performed = true;
        return PerformSpec();        
    }

    protected abstract int PerformSpec();

    bool performed = false;
}
