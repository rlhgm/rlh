using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource soundSource;
	public static SoundManager instance = null;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	public void PlaySound(AudioClip clip){
		soundSource.clip = clip;
		soundSource.Play ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
