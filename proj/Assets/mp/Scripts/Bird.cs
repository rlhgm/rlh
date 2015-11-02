using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour {

	Vector3 dir;
	float speed;

    public GameObject cutParticles = null;

    void Awake(){
		dir = new Vector2(0,0);
		speed = 0.0f;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (dir.x == 0.0f && dir.y == 0.0f)
			return;
		if (speed == 0.0f)
			return;

		Vector3 pos = transform.position;
		Vector3 distToFly = dir * (speed * Time.deltaTime);
		transform.position = pos + distToFly;
	}

	public void setDir(Vector2 newDir){
		Vector2 dir2n = newDir.normalized;
		dir = dir2n;
		if (dir.x < 0.0)
			turnLeft ();
		else 
			turnRight ();
	}
	public void setSpeed(float newSpeed){
		speed = newSpeed;
	}

	void turnLeft(){
		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x) * -1.0f;
		transform.localScale = scl;
	}
	void turnRight(){
		Vector3 scl = transform.localScale;
		scl.x = Mathf.Abs(scl.x) * 1.0f;
		transform.localScale = scl;
	}

    public void cut()
    {
        if (cutParticles)
        {
            Object newParticleObject = Instantiate(cutParticles, transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(newParticleObject, 2.0f);
            //AudioSource audio = GetComponent<AudioSource>();
            //if (audio)
            //{
            //    audio.Play();
            //}
        }
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }
}
