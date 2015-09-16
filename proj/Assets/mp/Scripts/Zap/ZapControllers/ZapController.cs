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

	protected string name;
	protected Zap zap = null;
	protected Transform transform = null;
}
