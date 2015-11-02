using UnityEngine;
using System.Collections;

public class BirdEmiterActivator : MonoBehaviour {

	public BirdEmiter[] birdEmitters;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
            //print ("BirdEmiterActivator::OnTriggerEnter2D");
            for (int i = 0; i < birdEmitters.Length; ++i)
            {
                if (birdEmitters[i])
                    birdEmitters[i].OnOff = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //print ("BirdEmiterActivator::OnTriggerEnter2D");
            for (int i = 0; i < birdEmitters.Length; ++i)
            {
                if (birdEmitters[i])
                    birdEmitters[i].OnOff = false;
            }
        }
    }
}
 