using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

	BoxCollider2D coll;
	Transform front1;
	Transform front2;
	float tm;
	Vector3 front1StartPos;
	Vector3 front2StartPos;
    public GameObject splashParticles = null;

	void Awake(){
		coll = GetComponent<BoxCollider2D> ();
		front1 = transform.Find ("front1");
		front2 = transform.Find ("front2");
	}

	// Use this for initialization
	void Start () {
		//print (front);
		tm = 0.0f;
		front1StartPos = front1.localPosition;
		front2StartPos = front2.localPosition;
	}

	// Update is called once per frame
	void Update () {
		tm += Time.deltaTime;

		Vector3 oldPos = front1.localPosition;

		oldPos.x = front1StartPos.x + Mathf.Cos (tm) * 0.025f;
		oldPos.y = front1StartPos.y + Mathf.Sin (tm * 10.0f) * 0.05f;

		front1.localPosition = oldPos;

		oldPos = front2.localPosition;
		oldPos.x = front2StartPos.x + Mathf.Cos (tm * 2.3f) * 0.025f;
		oldPos.y = front2StartPos.y + Mathf.Sin (tm * 14f) * 0.05f;		
		front2.localPosition = oldPos;
	}

	public float getWidth(){
		return coll.size.x * transform.localScale.x;
	}
	public float getDepth(){
		return coll.size.y * transform.localScale.y;
	}
	public Vector2 getSize(){
		return new Vector2 (getWidth (), getDepth ());;
	}

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        //
        if (otherCollider.gameObject.tag == "Player")
        {
            //print("OnTriggerEnter2D");
            ////Instantiate(object,transform.position,Quaternion.EulerAngles(270,0,0));
            //Instantiate(particles, Vector3(coll.gameObject.transform.position.x, transform.position.y), Quaternion.Euler(270, 0, 0));
            if (splashParticles)
            {
                Object newParticleObject = Instantiate(splashParticles, otherCollider.transform.position, Quaternion.Euler(270, 0, 0));
                Destroy(newParticleObject, 2.0f);
                AudioSource audio = GetComponent<AudioSource>();
                if (audio)
                {
                    audio.Play();
                }
            }
        }
    }
}
