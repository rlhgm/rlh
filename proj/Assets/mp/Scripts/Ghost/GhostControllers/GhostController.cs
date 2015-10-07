using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

//[System.Serializable]
//[Serializable]
public class GhostController : ScriptableObject{
	
    virtual public void setOwner(Ghost ghost)
    {
        owner = ghost;
        transform = owner.transform;
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
    
    protected bool isInState(Ghost.State test){
		return owner.isInState (test);
	}
	protected bool isNotInState(Ghost.State test) {
		return owner.isNotInState(test);
	}
    
	public virtual int keyLeftDown(){
		return 0;
	}
	public virtual int keyLeftUp(){
		return 0;
	}
	
	public virtual int keyRightDown(){
		return 0;
	}
	public virtual int keyRightUp(){
		return 0;
	}
	
	public virtual int keyUpDown(){
		return 0;
	}
	public virtual int keyUpUp(){
		return 0;
	}
	
	public virtual int keyDownDown(){
		return 0;
	}
	virtual public int keyDownUp(){
		return 0;
	}
	
	public virtual int keyRunDown(){
		return 0;
	}
	public virtual int keyRunUp(){
		return 0;
	}
	
	public virtual int keyJumpDown(){
		return 0;
	}
	public virtual int keyJumpUp(){
		return 0;
	}

	public virtual bool crouching(){
		return false;
	}

    public virtual void die(Ghost.DeathType deathType)
    {
    }

    public virtual bool triggerEnter(Collider2D other){
		return false;
	}

	public virtual bool isDodging(){
		return false;
	}
    
	public void cut(Vector2 cutStart, Vector2 cutEnd){
		RaycastHit2D[] hits = Physics2D.LinecastAll (cutStart, cutEnd);
		for (int i = 0; i < hits.Length; ++i) {
			
			Collider2D coll = hits[i].collider;
			RopeLink cutRopeLink = coll.GetComponent<RopeLink>();
			if( cutRopeLink ){
				//Debug.Log( "trafionione : " + hits[i].collider.name );
				cutRopeLink.cut();
				return;
			}
			
			Snake cutSnake = coll.GetComponent<Snake>();
			if( cutSnake ){
				cutSnake.cut();
				return;
			}
			
			CutableBush cutBush = coll.GetComponent<CutableBush>();
			if( cutBush ){
				cutBush.cut();
				return;
			}

			Bird cutBird = coll.GetComponent<Bird>();
			if( cutBird ){
				cutBird.cut();
				return;
			}

			if( coll.tag == "PantherHitRegion" ){
                IKnifeCutable cutable = coll.transform.parent.GetComponent<IKnifeCutable>();
                if (cutable != null)
                {
                    cutable.Cut();
                }
				//Panther cutPanther = coll.transform.parent.GetComponent<Panther>();
				//if( cutPanther ){
				//	cutPanther.Cut();
				//	return;
				//}
			}
            
		}
	}

	//protected string name;
	protected Ghost owner = null;
	protected Transform transform = null;	
}
