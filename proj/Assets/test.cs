using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {

		SpriteRenderer sr = transform.GetComponent<SpriteRenderer> ();
		if (sr) {

			//sr.sprite.
			sr.material.SetTextureScale("_MainTex", new Vector2(20f,20f));
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
