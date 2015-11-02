﻿using UnityEngine;
using System.Collections;

public class CutableBush : MonoBehaviour {

	public int LifePoints = 3;
	int currentLifePoints;

    public GameObject cutParticles = null;

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
    }

	void setAlpha(){
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		Color c = sr.color;
		c.a = currentLifePoints / LifePoints;
		sr.color = c;
	}
}
