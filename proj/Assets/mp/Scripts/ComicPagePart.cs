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

		}
		if (fadeOut) {

		}

//		if (flashing) {
//
//			float flashRatio = (flashTime+=Time.deltaTime) / flashDuration;
//
//			if( flashRatio >= 1f ){
//
//				Color newColor = new Color(1f,1f,1f,1f);
//				img.color = newColor;
//				flashing = false;
//
//			}else{
//
//				if( flashRatio < 0.5f ){
//
//					Color newColor = new Color(1f,1f,1f-flashRatio,1f);
//					img.color = newColor;
//
//				}else{
//
//					Color newColor = new Color(1f,1f,0.5f+flashRatio,1f);
//					img.color = newColor;
//
//				}
//
//			}
//		}
	}

	//float flashTime = 0f;
	//float flashDuration = 0.5f;
	//bool flashing = false;
	//bool collected = false;

	bool fadeIn = false;
	bool fadeOut = false;
	float fadeDuration = 0.0f;

	public void collect(){
		if (collected)
			return;

		//flashTime = 0;
		//flashing = true;
		collected = true;
		//Color newColor = new Color(1f,1f,1f,1f);
		//img.color = newColor;
		show (1.0f);
	}

	public void show(float duration){


		if (collected) {
			Color newColor = new Color(1f,1f,1f,0f);
			img.color = newColor;

			fadeDuration = duration;
			fadeIn = true;
		} else {

		}
	}

	public void hide(float duration){
		//Color newColor = new Color(1f,1f,1f,0f);
		//img.color = newColor;
		
		if (collected) {
			Color newColor = new Color(1f,1f,1f,0f);
			img.color = newColor;

			fadeDuration = duration;
			fadeOut = true;
		} else {
			
		}
	}
}
