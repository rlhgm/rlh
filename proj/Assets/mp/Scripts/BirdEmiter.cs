using UnityEngine;
using System.Collections;

public class BirdEmiter : MonoBehaviour {

	BoxCollider2D coll;
	float timeToNext;
	float timeFromLast;

	public Vector2 dir;
	public Vector2 timeToNextMinMax;
	public Vector2 birdsSpeedMinMax;

	public Bird birdPrefab;

	void Awake(){
		coll = GetComponent<BoxCollider2D> ();
		dir = -Vector2.right;
		timeToNextMinMax = new Vector2 (1, 3);
		birdsSpeedMinMax = new Vector3 (2, 5);

		timeToNext = timeToNextMinMax.x;
		timeFromLast = 0.0f;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeFromLast += Time.deltaTime;
		if (timeFromLast >= timeToNext) {
			emit ();
		}
	}

	void emit(){
		if (!birdPrefab)
			return;

		Bird newBird = Instantiate<Bird> (birdPrefab);
		newBird.transform.position = transform.position;
		newBird.setDir (dir);
		newBird.setSpeed( Random.Range(birdsSpeedMinMax.x,birdsSpeedMinMax.y) );

		timeToNext = Random.Range (timeToNextMinMax.x, timeToNextMinMax.y);
		timeFromLast = 0.0f;
	}

}
