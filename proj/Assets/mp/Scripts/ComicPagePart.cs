using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ComicPagePart : MonoBehaviour {

	public int partID = 0;
	public bool collected = false;
	Image img = null;

	// Use this for initialization
	void Start () {
		collected = false;

		img = GetComponent<Image>();
		if( img ){
			Color newColor = new Color(1f,1f,1f,0f);
			img.color = newColor;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (fadeIn) {

			float fadeRatio = (fadeTime+=Time.deltaTime) / fadeDuration;

			if( fadeRatio >= 1f ){
				Color newColor = new Color(1f,1f,1f,1f);
				img.color = newColor;
				fadeIn = false;
			}else{
				Color newColor = new Color(1f,1f,1f,fadeRatio);
				img.color = newColor;
			}

		}
		if (fadeOut) {

			float fadeRatio = (fadeTime+=Time.deltaTime) / fadeDuration;
			
			if( fadeRatio >= 1f ){
				Color newColor = new Color(1f,1f,1f,0f);
				img.color = newColor;
				fadeOut = false;
			}else{
				Color newColor = new Color(1f,1f,1f,1f-fadeRatio);
				img.color = newColor;
			}

		}
	}

	bool fadeIn = false;
	bool fadeOut = false;
	float fadeDuration = 0.0f;
	float fadeTime = 0.0f;

	public void collect(){
		if (collected)
			return;

		collected = true;
		//show (1.0f);
	}

	public void show(float duration){

		if (collected) {
			Color newColor = new Color(1f,1f,1f,0f);
			img.color = newColor;

			fadeDuration = duration;
			fadeTime = 0.0f;
			fadeIn = true;
		} else {

		}
	}

	public void hide(float duration){

		if (collected) {
			Color newColor = new Color(1f,1f,1f,0f);
			img.color = newColor;

			fadeDuration = duration;
			fadeTime = 0.0f;
			fadeOut = true;
		} else {
			
		}
	}
}
