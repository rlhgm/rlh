using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour {

	Vector3 dir;
	float speed;

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
	}
	public void setSpeed(float newSpeed){
		speed = newSpeed;
	}
}
