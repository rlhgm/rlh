using UnityEngine;
using System.Collections;

public class cc : MonoBehaviour {
	
	Rigidbody2D rb;
	Vector3 fv;
	Vector3 rfv;
	float maxSpeed;
	Vector2 velocity;
	Vector2 impulse;
	float jumpImpulse = 5.0f; //690.0f;
	float gravityForce = -10.0f;//-1035.0f;
	bool jump;
	float MAX_PLAYER_SPEED_Y = 2.0f;

	enum CharacterState
	{
		STATE_ON_GROUND = 0,
		STATE_IN_AIR,
		STATE_STRUCKED,
		STATE_DIE,
		STATE_ATTACK,
		STATE_OTHER
	};

	CharacterState state;

	CharacterState getState() { return state; }
	bool setState(CharacterState newState){
		if (state == newState)
			return false;

		state = newState;
		return true;
	}

	// Use this for initialization
	void Start () {
		jump = false;
		rb = GetComponent<Rigidbody2D>();

		maxSpeed = 2;

		fv = new Vector3 (maxSpeed, 0, 0);
		rfv = new Vector3 (-maxSpeed, 0, 0);

		velocity = new Vector3 (0, 0, 0);
		impulse = new Vector3 (0, 0, 0);

		setState (CharacterState.STATE_OTHER);
	}



	void OnCollisionEnter2D(Collision2D coll) {
		//if (coll.gameObject.tag == "Enemy")
		//	coll.gameObject.SendMessage("ApplyDamage", 10);

		print( "colisionEnter" );

		for (int i = 0; i < coll.contacts.Length; ++i) {
			print ( coll.contacts[i].normal ); 
		}

		//IEnumerator i = coll.contacts.GetEnumerator ();
		//while (i.MoveNext()) {
		//	i.Current.
		//}

		print( "=========================" );

		setState (CharacterState.STATE_ON_GROUND);

		velocity.y = 0.0f;
	}

	//void FixedUpdate(){
		//Vector3 oldPos = transform.position;
		//transform.position = transform.position + velocity * Time.deltaTime;
	//}

	// Update is called once per frame
	void Update () {
		//print( GetComponent<Rigidbody2D>().velocity );da
		//print (transform.forward);
		//print( Time.deltaTime );

		SetImpulse(new Vector2(0.0f, 0.0f));

		if (Input.GetKey("a")) {
			//print ("a is held down");
			//rb.velocity = new Vector2(-2,0);

			//Vector2 vect = new Vector2(0,0);
			//vect.Set(-10,0);
			//GetComponent<Rigidbody2D>().AddForce(vect);
			//GetComponent<Rigidbody2D>().velocity.Set(vect);

			//rb.MovePosition(transform.position + rfv * Time.deltaTime);

			//transform.position = transform.position + rfv * Time.deltaTime;

			//velocity.x = -1;
			addImpulse(new Vector2(-1.0f,0.0f))
		}
		else if (Input.GetKey("d")) {
			//print ("d is held down");
			//rb.velocity = new Vector2(2,0);
			//GetComponent<Rigidbody2D>().velocity.x = 10;
			//GetComponent<Rigidbody2D>().AddForce(Vector2(10,0));

			//Vector2 vect;
			//vect.Set(10,0);
			//GetComponent<Rigidbody2D>().AddForce(vect);

			//Vector2 vect = new Vector2(0,0);
			//vect.Set(10,0);
			//GetComponent<Rigidbody2D>().AddForce(vect);
			//GetComponent<Rigidbody2D>().veclocity.Set(vect);

			//rb.MovePosition(transform.position + fv * Time.deltaTime);
			//transform.position = transform.position + fv * Time.deltaTime;

			//velocity.x = 1;
			addImpulse(new Vector2(1.0f,0.0f))
		}
		else if (Input.GetKeyDown("w")) {
			print ("w is held down");
			//GetComponent<Rigidbody2D>().velocity.Set(10,0);
			//GetComponent<Rigidbody2D>().velocity.x = 10;
			//GetComponent<Rigidbody2D>().AddForce(Vector2(10,0));
			
			//Vector2 vect;
			//vect.Set(10,0);
			//GetComponent<Rigidbody2D>().AddForce(vect);
			
			//Vector2 vect = new Vector2(0,0);
			//vect.Set(0,550);
			//rb.AddForce(vect);

			velocity.y = 0.0f;
			addImpulse(new Vector2(0.0f, jumpImpulse));
			setState(CharacterState.STATE_IN_AIR);
		}

		if( state == CharacterState.STATE_IN_AIR )
		{
			//print ("ia");
			addImpulse(new Vector2(0.0f, gravityForce * Time.deltaTime));
		}


		// Ustalanie prędkości gracza na podstawie impulse
		velocity.x += impulse.x;
		
		//float maxSpeed = state == STATE_ON_GROUND ? MAX_PLAYER_SPEED_X_GROUND : MAX_PLAYER_SPEED_X_AIR;
		//if(velocity.x > maxSpeed)
		//	velocity.x = maxSpeed;
		//if(velocity.x < -maxSpeed)
		//	velocity.x = -maxSpeed;
		
		//if(state == STATE_ON_GROUND && !onGroundUp && velocity.y < minVelocityOnGround)
		//	velocity.y = minVelocityOnGround;
		//else
		if( state == CharacterState.STATE_IN_AIR )
		{
			velocity.y += impulse.y;
			if(velocity.y > MAX_PLAYER_SPEED_Y)
				velocity.y = MAX_PLAYER_SPEED_Y;
			if(velocity.y < -MAX_PLAYER_SPEED_Y)
				velocity.y = -MAX_PLAYER_SPEED_Y;
		}

		//velocity += impulse;

		print (velocity);
		print (impulse);
		Vector3 v = new Vector3 (velocity.x, velocity.y, 0);
		Vector3 oldPos = transform.position;
		transform.position = transform.position + v * Time.deltaTime;


	}

	void SetImpulse(Vector2 imp) { impulse = imp; }
	Vector2 getImpulse() { return impulse; }
	void addImpulse(Vector2 imp) { impulse += imp; }
}
