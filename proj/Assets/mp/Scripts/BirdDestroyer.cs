using UnityEngine;
using System.Collections;

public class BirdDestroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//RaycastHit2D hit; 
		//hit = Physics2D.Linecast(sensorHandleR2.position, sensorHandleR2.position, layerIdGroundHandlesMask); 

	}

	void OnTriggerEnter2D(Collider2D other) {
		//print( "BirdDestroyer OnTriggerEnter" );
		if (other.gameObject.tag == "Bird") {

            if (transform.parent)
            {
                if (transform.parent.tag == "Player")
                {
                    Bird birdToDestroy = other.GetComponent<Bird>();
                    birdToDestroy.cut();
                }
            }
            else
            {
                Destroy(other.gameObject);
            }
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		print( "BirdDestroyer colisionEnter" );
	//		
	//		for (int i = 0; i < coll.contacts.Length; ++i) {
	//			Vector2 n = coll.contacts[i].normal;
	//			float atn = Mathf.Atan2( n.y, n.x );
	//			float atnd = atn * Mathf.Rad2Deg;
	//			if( atnd > 45 && atnd < 135 ) 
	//			{
	//				setState (State.ON_GROUND);
	//				velocity.y = 0.0f;
	//				print("LANDing");
	//				return;
	//			}
	//		}
	}
		
	void OnCollisionExit2D(Collision2D coll) {
		print( "BirdDestroyer colisionExit" );
	//		
	//		for (int i = 0; i < coll.contacts.Length; ++i) {
	//			Vector2 n = coll.contacts[i].normal;
	//			float atn = Mathf.Atan2( n.y, n.x );
	//			float atnd = atn * Mathf.Rad2Deg;
	//			if( atnd > 45 && atnd < 135 ) 
	//			{
	//				setState (State.IN_AIR);
	//				//velocity.y = 0.0f;
	//				print("FLYing");
	//				return;
	//			}
	//		}
	}
		
	void OnCollisionStay2D(Collision2D coll) {
		print( "BirdDestroyer collisionStay" );
	}
}
