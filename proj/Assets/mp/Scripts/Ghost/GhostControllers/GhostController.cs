using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

//[Serializable]
public class GhostController
{
    virtual public void setOwner(Ghost ghost)
    {
        owner = ghost;
        transform = owner.transform;
    }

    public virtual bool GlobalUpdate(float deltaTime)
    {
        return false;
    }

    public virtual void MUpdate(float deltaTime)
    {
    }

    public virtual void FUpdate(float fDeltaTime)
    {
    }

    public virtual void selected()
    {
    }
    public virtual void deselected()
    {
    }

    public void activate(bool restore = false, bool crouch = false)
    {
        activateSpec(restore, crouch);
    }
    public virtual void activateSpec(bool restore = false, bool crouch = false)
    {
    }

    public virtual void deactivate()
    {
    }
    public virtual bool tryDeactiveate()
    {
        return true;
    }

    protected bool isInState(Ghost.State test)
    {
        return owner.isInState(test);
    }
    protected bool isNotInState(Ghost.State test)
    {
        return owner.isNotInState(test);
    }
    
    public virtual bool crouching()
    {
        return false;
    }

    public virtual void die(Ghost.DeathType deathType)
    {
    }

    public virtual bool triggerEnter(Collider2D other)
    {
        return false;
    }

    public virtual bool isDodging()
    {
        return false;
    }
    
    //protected string name;
    protected Ghost owner = null;
    protected Transform transform = null;
}
