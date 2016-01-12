using UnityEngine;
using System.Collections;

public class ChandelierCollider : MonoBehaviour
{
    StatueToCrumble myOwner = null;

    // Use this for initialization
    void Start()
    {
        myOwner = transform.parent.GetComponent<StatueToCrumble>();
    }

    //// Update is called once per frame
    //void Update () {

    //}

    void OnCollisionEnter2D(Collision2D coll)
    {
        Chandelier collChandelier = coll.collider.transform.GetComponent<Chandelier>();
        if( collChandelier)
        {
            myOwner.HitByChandelier(collChandelier);
        }
        //public void HitByChandelier(Chandelier chandelier)
        //{
        //}
        //print(coll.collider.gameObject.name);

        //if( print )
        //print("BirdDestroyer colisionEnter");
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
}
