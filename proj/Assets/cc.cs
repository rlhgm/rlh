using UnityEngine;
using System.Collections;

public class cc : MonoBehaviour {
	
//	//Rigidbody2D rb;
//	BoxCollider2D coll;
//	//Vector3 fv;
//	//Vector3 rfv;
//	//float maxSpeed;
//	public Vector2 velocity;
//	public Vector2 impulse;
//	bool jump;
//
//	public float jumpImpulse = 7.5f; //690.0f;
//	public float gravityForce = -12.0f;//-1035.0f;
//	public float MAX_SPEED_Y = 11.0f;
//	public float MAX_SPEED_X_GROUND = 5.0f;
//	public float MAX_SPEED_X_AIR = 5.0f;
//
//	public enum State
//	{
//		ON_GROUND = 0,
//		IN_AIR,
//		//STATE_STRUCKED,
//		//STATE_DIE,
//		//STATE_ATTACK,
//		OTHER
//	};
//
//	public State state;
//
//	State getState() { return state; }
//	bool setState(State newState){
//		if (state == newState)
//			return false;
//
//		state = newState;
//		return true;
//	}
//
//	// Use this for initialization
//	void Start () {
//		jump = false;
//		//rb = GetComponent<Rigidbody2D>();
//		coll = GetComponent<BoxCollider2D> ();
//
//		//maxSpeed = 2;
//		//fv = new Vector3 (maxSpeed, 0, 0);
//		//rfv = new Vector3 (-maxSpeed, 0, 0);
//
//		velocity = new Vector3 (0, 0, 0);
//		impulse = new Vector3 (0, 0, 0);
//
//		setState (State.IN_AIR);
//	}
//
//
//
//	void OnCollisionEnter2D(Collision2D coll) {
//		//if (coll.gameObject.tag == "Enemy")
//		//	coll.gameObject.SendMessage("ApplyDamage", 10);
//		print( "colisionEnter" );
//
////		for (int i = 0; i < coll.contacts.Length; ++i) {
////			Vector2 n = coll.contacts[i].normal;
////			float atn = Mathf.Atan2( n.y, n.x );
////			//print ( "atn : " +  atn * Mathf.Rad2Deg );
////			float atnd = atn * Mathf.Rad2Deg;
////			if( atnd > 45 && atnd < 135 ) 
////			{
////				setState (State.ON_GROUND);
////				velocity.y = 0.0f;
////				return;
////			}
////		}
//	}
//
//	void OnCollisionExit2D(Collision2D coll) {
////		print( "colisionExit" );
////		//if (coll.gameObject.tag == "DodgemCar")
////		//	bumpCount++;
////
////
////		for (int i = 0; i < coll.contacts.Length; ++i) {
////			Vector2 n = coll.contacts[i].normal;
////			float atn = Mathf.Atan2( n.y, n.x );
////			print ( "atn : " +  atn * Mathf.Rad2Deg );
////		}
////
////		//dsetState (State.IN_AIR);
//	}
//
//	void OnCollisionStay2D(Collision2D coll) {
//		//print( "colisionStay" );
//		//if (coll.gameObject.tag == "RechargePoint")
//		//	batteryLevel = Mathwwf.Min(batteryLevel + rechargeRate * Time.deltaTime, 100.0F);
//
////		for (int i = 0; i < coll.contacts.Length; ++i) {
////			Vector2 n = coll.contacts[i].normal;
////			float atn = Mathf.Atan2( n.y, n.x );
////			//print ( "atn : " +  atn * Mathf.Rad2Deg );
////			float atnd = atn * Mathf.Rad2Deg;
////			if( atnd > 45 && atnd < 135 ) return;
////
////		}
//
//		//setState (State.IN_AIR);
//	}
//
//	//void FixedUpdate(){
//		//Vector3 oldPos = transform.position;
//		//transform.position = transform.position + velocity * Time.deltaTime;
//	//}
//
//	// Update is called once per frame
//	void Update () {
//
//		SetImpulse(new Vector2(0.0f, 0.0f));
//
//		StayOnGround();
//
//		if (Input.GetKey("a")) {
//			addImpulse(new Vector2(-MAX_SPEED_X_GROUND,0.0f));
//		}
//		else if (Input.GetKey("d")) {
//			addImpulse(new Vector2(MAX_SPEED_X_GROUND,0.0f));
//		}
//		if (Input.GetKeyDown("w")) {
//			velocity.y = 0.0f;
//			addImpulse(new Vector2(0.0f, jumpImpulse));
//			setState(State.IN_AIR);
//		}
//
//		if( state == State.IN_AIR )
//		{
//			//StayOnGround();
//			//print ("ia");
//			//addImpulse(new Vector2(0.0f, gravityForce * Time.deltaTime));
//		}
//
//
//		// Ustalanie prędkości gracza na podstawie impulse
//		velocity.x = 0.0f;
//		velocity.x += impulse.x;
//		
//		float maxSpeed = state == State.ON_GROUND ? MAX_SPEED_X_GROUND : MAX_SPEED_X_AIR;
//		if(velocity.x > maxSpeed)
//			velocity.x = maxSpeed;
//		if(velocity.x < -maxSpeed)
//			velocity.x = -maxSpeed;
//		
//		//if(state == STATE_ON_GROUND && !onGroundUp && velocity.y < minVelocityOnGround)
//		//	velocity.y = minVelocityOnGround;
//		//else
//		if( state == State.IN_AIR )
//		{
//			velocity.y += impulse.y;
//			if(velocity.y > MAX_SPEED_Y)
//				velocity.y = MAX_SPEED_Y;
//			if(velocity.y < -MAX_SPEED_Y)
//				velocity.y = -MAX_SPEED_Y;
//		}
//
//		//velocity += impulse;
//
//		//print (velocity);
//		//print (impulse);
//		Vector3 v = new Vector3 (velocity.x, velocity.y, 0);
//		Vector3 oldPos = transform.position;
//		transform.position = transform.position + v * Time.deltaTime;
//	}
//
//	void SetImpulse(Vector2 imp) { impulse = imp; }
//	Vector2 getImpulse() { return impulse; }
//	void addImpulse(Vector2 imp) { impulse += imp; }
//
//	bool StayOnGround(){
//		//public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity); 
//		Vector2 rayOrigin = new Vector2( transform.position.x, transform.position.y - coll.size.y * 0.5f);
//		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, 2.0f);
//		if (hit.collider != null) {
//			float distance = Mathf.Abs(hit.point.y - transform.position.y);
//			//print( hit.collider.gameObject.name );
//			//print ( distance );
//			if( distance < 0.001f )
//				return true;
//		}
//		return false;
//	}
}
