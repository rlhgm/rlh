using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

public class ZapController {
	
	public string name;
	public Player2Controller zap;
	
	public ZapController (string controllerName, Player2Controller playerController) {
		name = controllerName;
		zap = playerController;
	}
	
	public virtual void Update (float deltaTime) {	
	}
	
	public virtual void FUpdate(float fDeltaTime){
	}
	
	public virtual void activate(){
	}
	public virtual void deactivate(){
	}
}
