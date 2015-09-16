using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

public class ZapNormalController : ZapController {
	
	public ZapNormalController (Player2Controller playerController) 
		: base("NormalController",playerController)
	{
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
