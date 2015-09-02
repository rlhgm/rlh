using UnityEngine;
using System.Collections;

public class GravityGun : Weapon {

	public Transform draggedStone = null;
	public Vector3 lastToMoveDist = new Vector3();
	public Vector3 lastMousePosition = new Vector3();
	public int layerIdGroundMoveableMask = 0;

	public GravityGun (Player2Controller playerController, int stonesMask) 
		: base("GravityGun", playerController)
	{
		Debug.Log ("hello world - gravitygun");
		layerIdGroundMoveableMask = stonesMask;
	}
	
	public override void Update () {
		if (Input.GetMouseButtonDown (0)) {
			
			draggedStone = null;
			
			lastToMoveDist.Set(0f,0f,0f);
			
			Vector3 mouseInScene = player.touchCamera.ScreenToWorldPoint(Input.mousePosition);
			
			RaycastHit2D hit = Physics2D.Linecast( mouseInScene, mouseInScene, layerIdGroundMoveableMask );
			if( hit.collider ){
				draggedStone = hit.collider.gameObject.transform;
				if( draggedStone ){
					Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
					if( tsrb ){
						tsrb.gravityScale = 0f;
					}else{
						draggedStone = null;
					}
				}
			}
		}
		
		if (Input.GetMouseButtonUp (0)) {
			
			if( draggedStone ){
				Rigidbody2D tsrb = draggedStone.GetComponent<Rigidbody2D>();
				if( tsrb ){
					draggedStone.GetComponent<Rigidbody2D>().gravityScale = 1f;
					draggedStone.GetComponent<Rigidbody2D>().AddForce( lastToMoveDist, ForceMode2D.Impulse );
				}
				draggedStone = null;
			}
			
		}
	}

	Vector2 T; 			// sila ciagu
	float C = 0.09f; 		// wspolczynnik oporu - u mnie raczej bezwladnosci
	Vector2 V; 			// predkosc
	//float M; 			// masa
	//Vector2 S; 			// polozenie

	public override void FUpdate () {
		Vector3 currentMousePosition = Input.mousePosition;
		//if (currentMousePosition != lastMousePosition) {
		
		if( Input.GetMouseButton(0) ){
			if( player.touchCamera ){
				Vector3 touchInScene = player.touchCamera.ScreenToWorldPoint(currentMousePosition);
				Vector2 tis = touchInScene;

				if( draggedStone ){

					//Rigidbody2D testStoneRigidBody = draggedStone.GetComponent<Rigidbody2D>();
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

						F = T - (C * V);
						//A = F / M;

						//Vnew = V + A * Time.fixedDeltaTime;
						//Snew = S + Vnew * Time.fixedDeltaTime;

						//V = Vnew;
						//S = Snew;

						rb.AddForce(F,ForceMode2D.Impulse);
					}
				}
			}
		}
		lastMousePosition = currentMousePosition;
	}
}
