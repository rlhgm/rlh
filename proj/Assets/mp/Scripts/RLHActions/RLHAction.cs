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

    protected bool performed = false;

    public virtual void SaveResets()
    {
    }
    public virtual void Reset()
    {

    }
}
