using UnityEngine;
using System.Collections;

public class GravityGun : Weapon {

	public Transform draggedStone = null;
	public Transform lastFlashStone = null;

	//public Vector3 lastToMoveDist = new Vector3();
	//public Vector3 lastMousePosition = new Vector3();
	public int layerIdGroundMoveableMask = 0;
	public int layerIdGroundMask = 0;

	public GravityGun (Player2Controller playerController, int stonesMask, int groundsMask) 
		: base("GravityGun", playerController)
	{
		Debug.Log ("hello world - gravitygun");
		layerIdGroundMoveableMask = stonesMask;
		layerIdGroundMask = groundsMask;
	}
	
	public override void Update () {

		if (!Input.GetMouseButton (0)) {
			if (draggedStone == null) {

				if (lastFlashStone) {
					unflashStone (lastFlashStone);
					lastFlashStone = null;
				}
					
				Vector2 mouseInScene = player.touchCamera.ScreenToWorldPoint(Input.mousePosition);

				Vector2 rayOrigin = player.dir() == Vector2.right ? player.sensorRight2.position : player.sensorLeft2.position;
				Vector3 _df = mouseInScene - rayOrigin;
				
				if( _df.magnitude <= maxDistance ){

					RaycastHit2D hit = Physics2D.Linecast (mouseInScene, mouseInScene, layerIdGroundMoveableMask);
					if( hit.collider ){

						lastFlashStone = hit.collider.gameObject.transform;
						if( lastFlashStone ){
							Rigidbody2D tsrb = lastFlashStone.GetComponent<Rigidbody2D>();
							if( tsrb ){

								//rayOrigin = player.dir() == Vector2.right ? player.sensorRight2.position : player.sensorLeft2.position;

								hit = Physics2D.Linecast (rayOrigin, tsrb.worldCenterOfMass, layerIdGroundMask);
								if( hit.collider ){
									lastFlashStone = null;
								}else{
									flashStone(lastFlashStone);
								}

							}else{
								lastFlashStone = null;
							}
						}
					}
				}
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			
			draggedStone = null;
			
			//lastToMoveDist.Set(0f,0f,0f);
			
			Vector3 mouseInScene = player.touchCamera.ScreenToWorldPoint(Input.mousePosition);
			
			RaycastHit2D hit = Physics2D.Linecast( mouseInScene, mouseInScene, layerIdGroundMoveableMask );
			if( hit.collider ){
				draggedStone = hit.collider.gameObject.transform;

				if( canBeDragged(draggedStone) ){

					Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
					tsrb.gravityScale = 0f;
					flashStone(draggedStone);

				} else {
					draggedStone = null;
				}

//				if( draggedStone ){
//					Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
//					if( tsrb ){
//						tsrb.gravityScale = 0f;
//					}else{
//						draggedStone = null;
//					}
//				}
			}
		}
		
		if (Input.GetMouseButtonUp (0)) {
			
//			if( draggedStone ){
//				Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
//				if( tsrb ){
//					draggedStone.GetComponent<Rigidbody2D>().gravityScale = 1f;
//					draggedStone.GetComponent<Rigidbody2D>().AddForce( lastToMoveDist, ForceMode2D.Impulse );
//				}
//				draggedStone = null;
//			}

			releaseStone();
		}
	}

	Vector2 T; 			// sila ciagu
	public static float inertiaFactor = 0.09f; 		// wspolczynnik oporu - u mnie raczej bezwladnosci
	public static float maxDistance = 5f;
	Vector2 V; 			// predkosc
	//float M; 			// masa
	//Vector2 S; 			// polozenie
	//RLHOptionsWindow.

	public override void FUpdate () {
		Vector3 currentMousePosition = Input.mousePosition;
		//if (currentMousePosition != lastMousePosition) {
		
		if( Input.GetMouseButton(0) ){
			if( player.touchCamera ){
				Vector3 touchInScene = player.touchCamera.ScreenToWorldPoint(currentMousePosition);
				Vector2 tis = touchInScene;

				if( draggedStone ){

					Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
					
					if( rb ){

//						//testStone.position = new Vector3( touchInScene.x, touchInScene.y, testStone.position.z );
//						Vector3 tsp = draggedStone.position;
//						Vector3 tsrgCOM = testStoneRigidBody.worldCenterOfMass;
//						Vector3 comdiff = tsrgCOM - tsp;
//
//						Vector3 posDiff = touchInScene - ( tsrgCOM );
//						posDiff.z = draggedStone.position.z;
//						float posDiffLength = posDiff.magnitude;
//
//						//if( posDiffLength < 5f ){
//
//							//testStoneRigidBody.worldCenterOfMass
//
//							//testStoneRigidBody.add
//							//rb2D.MovePosition(rb2D.position + velocity * Time.fixedDeltaTime);
//							//testStoneRigidBody.MovePosition( touchInScene );
//							
//							float coef = 1f;
//							lastToMoveDist = posDiff * coef; //* Time.fixedDeltaTime;
//							lastToMoveDist.z = 0f;
//							
//							testStoneRigidBody.MovePosition( (tsrgCOM-comdiff) + lastToMoveDist * Time.fixedDeltaTime );
							
						//testStoneRigidBody.velocity;
						//}

						Vector2 F;			// sila wypadkowa
						//Vector2 A;			// przyspieszenie
						//float Vnew; 		// nowa predkosc w chwili t + dt
						//float Snew;			// nowe polozenie w chwili t + dt

						T = (tis - rb.worldCenterOfMass);
						V = rb.velocity;

						F = T - (inertiaFactor * V);
						//A = F / M;

						//Vnew = V + A * Time.fixedDeltaTime;
						//Snew = S + Vnew * Time.fixedDeltaTime;

						//V = Vnew;
						//S = Snew;

						rb.AddForce(F,ForceMode2D.Impulse);

//						Vector2 rayOrigin = player.dir() == Vector2.right ? player.sensorRight2.position : player.sensorLeft2.position;
//						Vector3 _df = rb.worldCenterOfMass - rayOrigin;
//
//						if( _df.magnitude > maxDistance ){
//
//							releaseStone();
//
//						} else {
//
//							RaycastHit2D hit = Physics2D.Linecast (rayOrigin, rb.worldCenterOfMass, layerIdGroundMask);
//							if( hit.collider ){
//								releaseStone();
//							}
//						}

						if( !canBeDragged( draggedStone ) ){
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

				Rigidbody2D rb = draggedStone.GetComponent<Rigidbody2D>();
				rb.gravityScale = 1f;
				//rb.AddForce( lastToMoveDist, ForceMode2D.Impulse );
			}
			unflashStone(draggedStone);
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
