using UnityEngine;
using System.Collections;

public class CutableBush : MonoBehaviour {

	public int LifePoints = 3;
	int currentLifePoints;

    public GameObject cutParticles = null;
    public Sprite[] lifePointsSprites;

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
		setAlpha (1.0f);
		GetComponent<BoxCollider2D> ().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (lifePointsSprites.Length == LifePoints && lifePointsSprites[currentLifePoints - 1])
        {
            sr.sprite = lifePointsSprites[currentLifePoints - 1];
        }
    }

	public void cut(){
		if (currentLifePoints == 0)
			return;

		currentLifePoints -= 1;

		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
        if (lifePointsSprites.Length == LifePoints && lifePointsSprites[currentLifePoints-1])
        {
            sr.sprite = lifePointsSprites[currentLifePoints-1];
            setAlpha(1.0f);
        }
        else
        {
            setAlpha();
        }

		if (currentLifePoints == 0) {
            sr.enabled = false;
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
		setAlpha( (float)currentLifePoints / (float)LifePoints );
	}
    void setAlpha(float newAlpha)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = newAlpha;
        sr.color = c;
    }
}
