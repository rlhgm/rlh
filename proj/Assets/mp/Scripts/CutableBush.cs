using UnityEngine;
using System.Collections;

public class CutableBush : MonoBehaviour {

	public int LifePoints = 3;
	int currentLifePoints;

	// Use this for initialization
	void Start () {
		currentLifePoints = LifePoints;
		reset ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void reset(){
		currentLifePoints = LifePoints;
		setAlpha ();
		GetComponent<BoxCollider2D> ().enabled = true;
	}

	public void cut(){
		if (currentLifePoints == 0)
			return;

		currentLifePoints -= 1;

		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		Color c = sr.color;
		c.a = (float)currentLifePoints / (float)LifePoints;
		sr.color = c;

		if (currentLifePoints == 0) {
			GetComponent<BoxCollider2D> ().enabled = false;
		}
	}

	void setAlpha(){
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		Color c = sr.color;
		c.a = currentLifePoints / LifePoints;
		sr.color = c;
	}
}
