using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

public class ZapController {
	
	public ZapController (Zap playerController, string controllerName) {
		name = controllerName;
		zap = playerController;
		transform = zap.transform;
	}
	
	public virtual void Update (float deltaTime) {	
	}
	
	public virtual void FUpdate(float fDeltaTime){
	}
	
	public virtual void activate(){
	}
	public virtual void deactivate(){
	}

	protected bool isInState(Zap.State test){
		return zap.isInState (test);
	}
	protected bool isNotInState(Zap.State test) {
		return zap.isNotInState(test);
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


	protected string name;
	protected Zap zap = null;
	protected Transform transform = null;
}
