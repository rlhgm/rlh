using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityGun : Weapon {

	public Transform draggedStone = null;
	public Transform lastFlashStone = null;

	//public Vector3 lastToMoveDist = new Vector3();
	//public Vector3 lastMousePosition = new Vector3();
	public int layerIdGroundMoveableMask = 0;
	public int layerIdGroundMask = 0;

	public GravityGun (Zap playerController, int stonesMask, int groundsMask) 
		: base("GravityGun", playerController)
	{
		Debug.Log ("hello world - gravitygun");
		layerIdGroundMoveableMask = stonesMask;
		layerIdGroundMask = groundsMask;
	}

	public override void activate(){
		//if (player.ggps) {
		//	player.ggps.Play ();
			//player.ggps.
		//}
	}
	public override void deactivate(){
		//if( player.ggps )
		//	player.ggps.Stop ();
	}

	public override void Update (float deltaTime) {

		Vector2 mouseInScene = player.touchCamera.ScreenToWorldPoint(Input.mousePosition);
		Debug.Log ("Knife::Update mouseInScene");


//		if( player.ggps ) {
//			player.ggpsTransform.Rotate( Vector3.forward, deltaTime * 360f );
//		}
//
//		if (!Input.GetMouseButton (0)) {
//			if (draggedStone == null) {
//
//				if (lastFlashStone) {
//					unflashStone (lastFlashStone);
//					lastFlashStone = null;
//				}
//					
//				Vector2 mouseInScene = player.touchCamera.ScreenToWorldPoint(Input.mousePosition);
//
//				Vector2 rayOrigin = player.dir() == Vector2.right ? player.sensorRight2.position : player.sensorLeft2.position;
//				Vector3 _df = mouseInScene - rayOrigin;
//				
//				if( _df.magnitude <= maxDistance ){
//
//					RaycastHit2D hit = Physics2D.Linecast (mouseInScene, mouseInScene, layerIdGroundMoveableMask);
//					if( hit.collider ){
//
//						lastFlashStone = hit.collider.gameObject.transform;
//						if( lastFlashStone ){
//							Rigidbody2D tsrb = lastFlashStone.GetComponent<Rigidbody2D>();
//							if( tsrb ){
//
//								//rayOrigin = player.dir() == Vector2.right ? player.sensorRight2.position : player.sensorLeft2.position;
//
//								hit = Physics2D.Linecast (rayOrigin, tsrb.worldCenterOfMass, layerIdGroundMask);
//								if( hit.collider ){
//									lastFlashStone = null;
//								}else{
//									flashStone(lastFlashStone);
//								}
//
//							}else{
//								lastFlashStone = null;
//							}
//						}
//					}
//				}
//			}
//		}
//
//		if (Input.GetMouseButtonDown (0)) {
//			
//			draggedStone = null;
//
//			Vector3 mouseInScene = player.touchCamera.ScreenToWorldPoint(Input.mousePosition);
//			
//			RaycastHit2D hit = Physics2D.Linecast( mouseInScene, mouseInScene, layerIdGroundMoveableMask );
//			if( hit.collider ){
//				draggedStone = hit.collider.gameObject.transform;
//
//				if( canBeDragged(draggedStone) ){
//
//					Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
//					tsrb.gravityScale = 0f;
//					flashStone(draggedStone);
//
//				} else {
//					draggedStone = null;
//				}
//			}
//		}
//		
//		if (Input.GetMouseButtonUp (0)) {
//			releaseStone();
//		}
	}

	Vector2 T; 			// sila ciagu
	public static float inertiaFactor = 0.09f; 		// wspolczynnik oporu - u mnie raczej bezwladnosci
	public static float inertiaFactor2 = 0.03f; 	// wspolczynnik bezwladnosci jak gracz na siebie chce skierowac kamien
	public static float maxDistance = 5f;
	public static float minDistance = 2f;
	public static float pushOutForce = 2f;
	public static float pushOutMassFactor = 10f;

	List<Rigidbody2D> droppedStones = new List<Rigidbody2D> (3);

	Vector2 V; 			// predkosc
	//float M; 			// masa
	//Vector2 S; 			// polozenie
	//RLHOptionsWindow.

	public static float userStoneRotateSpeed = 180f;

	public override void FUpdate (float fDeltaTime) {
		//Debug.Log ("FUpdate : " + fDeltaTime);

		if( draggedStone ){ 

			if( Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) ||
			   Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X) ){

				Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				if( rb ){
					rb.angularVelocity = 0;
				}
			}

			if( Input.GetKey(KeyCode.Z) ){ // obracam kamien w lewo ...

				Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				if( rb ){

					if( rb.angularVelocity < 180 )
						rb.angularVelocity += ( fDeltaTime * userStoneRotateSpeed );

					rb.angularVelocity = Mathf.Min( rb.angularVelocity, 180f);

					//rb.rotation += ( fDeltaTime * userStoneRotateSpeed );
				}
			}
			else if( Input.GetKey(KeyCode.X) ){ // albo w prawo

				Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				if( rb ){

					if( rb.angularVelocity > -180 )
						rb.angularVelocity -= ( fDeltaTime * userStoneRotateSpeed );

					rb.angularVelocity = Mathf.Max( rb.angularVelocity, -180f);

					//rb.rotation -= ( fDeltaTime * userStoneRotateSpeed );
				}

			}
		}

		Vector3 currentMousePosition = Input.mousePosition;
		//if (currentMousePosition != lastMousePosition) {

		for (int i = 0 ; i < droppedStones.Count; ++i) {
			Rigidbody2D rb = droppedStones[i];
			if( rb.IsSleeping() ){
				Debug.Log ( "remove dropped stone: " + rb ); 
				droppedStones.Remove(rb);
			}else{
				Vector2 playerCenterPos = player.transform.position;
				playerCenterPos.y += 1f;
				Vector2 stoneCenterPos = rb.worldCenterOfMass;
						
				Vector2 diff = stoneCenterPos - playerCenterPos;
				Vector2 F = new Vector2(0f,0f);
				float diffMagnitude = diff.magnitude;
						
				if( diffMagnitude < minDistance+0.25f ){
					//F = diff + diff * pushOutForce * (rb.mass / pushOutMassFactor);
					//F = diff.normalized * (rb.velocity.magnitude / 10f) * 20f * (rb.mass / pushOutMassFactor);

					// im blizej srodka i im szybciej tym mocniej wypycha
					F = diff * (diffMagnitude/minDistance) * (rb.velocity.magnitude / 10f) * 20f * (rb.mass / pushOutMassFactor);
					rb.AddForce(F,ForceMode2D.Impulse);
				}
			}
		}

		if( Input.GetMouseButton(0) ){
			if( player.touchCamera ){
				Vector3 touchInScene = player.touchCamera.ScreenToWorldPoint(currentMousePosition);
				Vector2 tis = touchInScene;

				if( draggedStone ){

					Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
					
					if( rb ){

						Vector2 playerCenterPos = player.transform.position;
						playerCenterPos.y += 1f;
						Vector2 stoneCenterPos = rb.worldCenterOfMass;

						Vector2 diff = stoneCenterPos - playerCenterPos;

						Vector2 F = new Vector2(0f,0f);

						float diffMagnitude = diff.magnitude;

						if( diffMagnitude < minDistance+0.25f ){

							F = diff + diff * ( diffMagnitude / minDistance ) * pushOutForce * (rb.mass / pushOutMassFactor);

						}else{

							Vector2 diff2 = tis - playerCenterPos;
							float diffMagnitude2 = diff2.magnitude;

							if( diffMagnitude2 > minDistance ){

								T = (tis - stoneCenterPos);
								V = rb.velocity;

								F = T - (inertiaFactor * V);

							}else{ // jednak musi przyciagac ale slabiej albo do granicy a nie 

								T = (tis - stoneCenterPos);
								V = rb.velocity;
								
								F = T - (inertiaFactor2 * V) ;
								F *= (rb.mass / pushOutMassFactor);
							}
						}

						//Debug.Log("F : " + rb.velocity);
						rb.AddForce(F,ForceMode2D.Impulse);

						if( !canBeDragged( draggedStone, tis) ){
							releaseStone();
						}

					}
				}
			}
		}
		//lastMousePosition = currentMousePosition;
	}

	bool catchStone(Transform stone){
		return false;
	}

	void releaseStone(){
		if( draggedStone ){
			Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
			if( tsrb ){

				//Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				tsrb.gravityScale = 1f;
				//rb.AddForce( lastToMoveDist, ForceMode2D.Impulse );
			}
			unflashStone(draggedStone);

			Debug.Log ( "add dropped stone: " + tsrb );
			droppedStones.Add( tsrb );
			draggedStone = null;
		}
	}

	void flashStone(Transform stone){
		setStoneOpacity (stone, 0.5f);
	}
	void unflashStone(Transform stone){
		setStoneOpacity (stone, 1.0f);
	}
	void setStoneOpacity(Transform stone, float newOpacity){
		SpriteRenderer sr = stone.GetComponent<SpriteRenderer> ();
		if (!sr)
			return;

		Color stoneColor = sr.color;
		stoneColor.a = newOpacity;
		sr.color = stoneColor;
	}


	bool canBeDragged(Transform stone, Vector2 stoneTargetPlace){

		Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
		if (!rb)
			return false;

		if( (rb.worldCenterOfMass - stoneTargetPlace).magnitude > 5f ){
			return false;
		}

		return canBeDragged (stone);
	}

	bool canBeDragged(Transform stone){
		
		Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
		if (!rb)
			return false;
		
		Vector2 rayOrigin = player.dir() == Vector2.right ? player.sensorRight2.position : player.sensorLeft2.position;
		Vector3 _df = rb.worldCenterOfMass - rayOrigin;
		
		if( _df.magnitude > maxDistance ){
			
			return false;
			
		} else {
			
			RaycastHit2D hit = Physics2D.Linecast (rayOrigin, rb.worldCenterOfMass, layerIdGroundMask);
			if( hit.collider ){
				return false;
			}
		}
		
		return true;
	}
}
