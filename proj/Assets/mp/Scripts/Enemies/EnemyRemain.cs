using UnityEngine;
using System.Collections;

public class EnemyRemain : MonoBehaviour
{
    Rigidbody2D myRigidBody = null;
    Vector2 startVelocity;
    //public Rigidbody2D RigidBody
    //{
    //    get
    //    {
    //        return rigidBody;
    //    }
    //}

    // Use this for initialization
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myRigidBody.velocity = startVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (myRigidBody.IsSleeping() || !myRigidBody.IsAwake())
        {
            Destroy(gameObject,1f);
        }
    }

    public void setVelocity(Vector2 newVelocity)
    {
        //myRigidBody.velocity = newVelocity;
        startVelocity = newVelocity;
    }
}
