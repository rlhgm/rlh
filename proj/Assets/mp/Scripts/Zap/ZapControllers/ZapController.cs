using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

//[System.Serializable]
//[Serializable]
public class ZapController : ScriptableObject{
	
//	public ZapController (Zap playerController, string controllerName) {
//		name = controllerName;
//		zap = playerController;
//		transform = zap.transform;
//	}

//	public ZapController (string controllerName) {
//		name = controllerName;
//		//zap = playerController;
//		//transform = zap.transform;
//
//		//target.mainCamera = myCamera;
//		//target.touchCamera = myCamera.transform.Find("TouchCamera").GetComponent<Camera>();
////		Camera mc = Camera.main;
////		if (mc) {
////			Transform touchCameraTransform = mc.transform.Find ("TouchCamera");
////			if( touchCameraTransform ){
////				touchCamera = touchCameraTransform.GetComponent<Camera> ();
////			}
////		}
//	}

//	void OnEnable() {
//		//print("script was enabled");
//		Camera mc = Camera.main;
//		if (mc) {
//			Transform touchCameraTransform = mc.transform.Find ("TouchCamera");
//			if( touchCameraTransform ){
//				touchCamera = touchCameraTransform.GetComponent<Camera> ();
//			}
//		}
//	}

	protected WeaponMenuItem weaponMenuItem;

	virtual public void setZap(Zap playerController){
		zap = playerController;
		transform = zap.transform;
		if (weaponMenuItem) {
			weaponMenuItem.setState(WeaponMenuItem.State.OFF);
		}
	}

	public virtual void MUpdate (float deltaTime) {	
	}
	
	public virtual void FUpdate(float fDeltaTime){
	}

	public virtual void selected(){
		if( weaponMenuItem )
			weaponMenuItem.setState (WeaponMenuItem.State.ON);
	}
	public virtual void deselected(){
		if( weaponMenuItem )
			weaponMenuItem.setState (WeaponMenuItem.State.OFF);
	}

	public virtual void activate(bool restore = false){
		if( weaponMenuItem )
			weaponMenuItem.setState (WeaponMenuItem.State.BLINK);
	}
	public virtual void deactivate(){
		if (weaponMenuItem) {
			if( zap.choosenController == this )
				weaponMenuItem.setState (WeaponMenuItem.State.ON);
			else
				weaponMenuItem.setState (WeaponMenuItem.State.OFF);
		}
	}

	public virtual bool tryDeactiveate(){
		return true;
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

	public virtual bool crouching(){
		return false;
	}

	public virtual void zapDie (Zap.DeathType deathType){
		if( weaponMenuItem )
			weaponMenuItem.setState (WeaponMenuItem.State.ON);
	}
	public virtual void reborn (){
		if( zap.getLastTouchedCheckPoint() != null ){
			if (zap.getLastTouchedCheckPoint().GetComponent<CheckPoint> ().startMounted) {
				zap.setState(Zap.State.MOUNT);
			}
		}
	}
	public virtual bool triggerEnter(Collider2D other){
		return false;
	}

//	public string getName(){
//		return name;
//	}

	public void setTouchCamera(Camera newTC){
		touchCamera = newTC;
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

		}
	}

	//protected string name;
	protected Zap zap = null;
	protected Transform transform = null;
	protected Camera touchCamera;
}
