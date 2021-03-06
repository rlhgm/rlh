﻿using UnityEngine;
using System.Collections;

public class DestroyablePlatform : MonoBehaviour {

    public float DestroyEnergy = 10f;

    public ParticleSet particles = null;
    public string SoundTagDestroy;
    public string ParticleTagDestroy;
    // Use this for initialization
    void Start () {
	
	}

    bool toDisable = false;
    float toDisableTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if( toDisable )
        {
            if( (toDisableTime-=Time.deltaTime) < 0f )
            {
                toDisableTime = 0f;
                toDisable = false;
                gameObject.SetActive(false);
            }
        }
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

        //print(collisionEnergy);

        if (collisionEnergy >= DestroyEnergy)
        {
            //DestroyByCollision();
            
            Rigidbody2D myBody = GetComponent<Rigidbody2D>();
            Debug.Assert(myBody);

            myBody.isKinematic = false;

            myBody.velocity = otherBody.velocity;

            //print("DestroyByCollision() " + collisionEnergy + " " + otherBody.velocity);

            gameObject.layer = LayerMask.NameToLayer("LA-GROUND");// RLHScene.Instance.LayerIdLAGROUNDMask;

            //Destroy(gameObject, 2f);
            toDisable = true;
            toDisableTime = 2f;
        }

    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        //if (coll.gameObject.tag == "Enemy")
        //    coll.gameObject.SendMessage("ApplyDamage", 10);

        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    //Debug.DrawRay(contact.point, contact.normal, Color.white);
        //    //print(name + " "  + contact.otherCollider.name);
        //    collision.rigidbody
        //}

        //Rigidbody2D otherBody = otherCollider.GetComponent<Rigidbody2D>();
        //if (!otherBody) return;

        //print(collision.relativeVelocity);

        //float collisionEnergy = DestroyEnergy + 1f;// collision.relativeVelocity.magnitude * otherBody.mass;
        //print(collision.relativeVelocity + " " + collision.relativeVelocity.magnitude + " " + body.velocity + " " + body.velocity.magnitude + " " + body.mass  + " = " +collisionEnergy);

        //print(collisionEnergy);

        //if (collisionEnergy >= DestroyEnergy)
        {
            //DestroyByCollision();

            Rigidbody2D otherRB = otherCollider.transform.GetComponent<Rigidbody2D>();
            Vector2 orbv = otherRB.velocity;
            orbv.y *= 0.25f;
            otherRB.velocity = orbv;

            RLHScene.Instance.CamController.ShakeImpulseStart(1f, 0.25f, 8f);

            Rigidbody2D myBody = GetComponent<Rigidbody2D>();
            Debug.Assert(myBody);

            myBody.isKinematic = false;

            //myBody.velocity = otherBody.velocity;

            //print("DestroyByCollision() " + collisionEnergy + " " + otherBody.velocity);

            gameObject.layer = LayerMask.NameToLayer("LA-GROUND");// RLHScene.Instance.LayerIdLAGROUNDMask;

            //Destroy(gameObject, 2f);
            toDisable = true;
            toDisableTime = 2f;

            if (particles != null && ParticleTagDestroy != "")
            {
                ParticleData _pd = particles.GetParticleData(ParticleTagDestroy);
                Vector3 particlePos = transform.position;
                Rigidbody2D _rb = GetComponent<Rigidbody2D>();
                if (_rb)
                { 
                    particlePos = _rb.worldCenterOfMass;
                }
                ParticleInseter.Insert(_pd, particlePos,transform.rotation);
            }

            if (SoundTagDestroy != "") SoundPlayer.Play(gameObject, SoundTagDestroy);
        }

    }


    void DestroyByCollision()
    {
        
    }

    void Reset()
    {

    }
}
