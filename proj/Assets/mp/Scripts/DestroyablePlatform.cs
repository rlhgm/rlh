using UnityEngine;
using System.Collections;

public class DestroyablePlatform : MonoBehaviour {

    public float DestroyEnergy = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (coll.gameObject.tag == "Enemy")
        //    coll.gameObject.SendMessage("ApplyDamage", 10);

        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    //Debug.DrawRay(contact.point, contact.normal, Color.white);
        //    //print(name + " "  + contact.otherCollider.name);
        //    collision.rigidbody
        //}

        Rigidbody2D otherBody = collision.rigidbody;

        if (!otherBody) return;

        //print(collision.relativeVelocity);

        float collisionEnergy = collision.relativeVelocity.magnitude * otherBody.mass;
        //print(collision.relativeVelocity + " " + collision.relativeVelocity.magnitude + " " + body.velocity + " " + body.velocity.magnitude + " " + body.mass  + " = " +collisionEnergy);

        print(collisionEnergy);

        if (collisionEnergy >= DestroyEnergy)
        {
            //DestroyByCollision();
            
            Rigidbody2D myBody = GetComponent<Rigidbody2D>();
            Debug.Assert(myBody);

            myBody.isKinematic = false;

            myBody.velocity = otherBody.velocity;

            print("DestroyByCollision() " + collisionEnergy + " " + otherBody.velocity);

            gameObject.layer = LayerMask.NameToLayer("LA-GROUND");// RLHScene.Instance.LayerIdLAGROUNDMask;

            Destroy(gameObject, 2f);
        }

    }

    void DestroyByCollision()
    {
        
    }

    void Reset()
    {

    }
}
