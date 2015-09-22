using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponMenuItem : MonoBehaviour {

	Image blinkSprite;
	Image sprite;

	void setOpactity(Image target, float newOpacity){
		//print ("setOpacity : " + newOpacity);
		Color targetColor = target.color;
		targetColor.a = newOpacity;
		target.color = targetColor;
	}
	float currentStateTime;

	public enum State{
		UNDEF = 0,
		OFF,
		ON,
		FADE_IN,
		FADE_OUT,
		BLINK
	};

	public State state;

	public void setState(State newState, float duration = 0f){
		switch (newState) {
		case State.ON:
			setOpactity(sprite,1.0f);
			setOpactity(blinkSprite,0.0f);
			break;

		case State.OFF:
			setOpactity(sprite,0.5f);
			setOpactity(blinkSprite,0.0f);
			break;

		case State.FADE_IN:
			break;

		case State.FADE_OUT:
			break;

		case State.BLINK:
			setOpactity(sprite,1.0f);
			setOpactity(blinkSprite,0.0f);
			break;
		};

		currentStateTime = 0f;
		state = newState;
	}

	// Use this for initialization
	void Awake () {
		sprite = transform.GetComponent<Image>();
		blinkSprite = transform.Find ("blink").GetComponent<Image>();
	}

	// Use this for initialization
	void Start () {
		currentStateTime = 0f;
		setState (State.OFF);
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.BLINK:
			setOpactity( blinkSprite, (Mathf.Sin(currentStateTime * Mathf.PI)+1f) * 0.5f );
			break;
		}

		currentStateTime += Time.deltaTime;
	}

//	public void blinkOn(){
//	}
//
//	public void blinkOff(){
//		
//	}
}
