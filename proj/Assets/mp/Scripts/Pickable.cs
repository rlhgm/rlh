using UnityEngine;
using System.Collections;

public class Pickable : MonoBehaviour {

	public enum Type{
		KNIFE,
		GRAVITY_GUN
	}

	public Type type = Type.KNIFE;
	//public bool isActive = true;

	// Use this for initialization
	void Start () {
        deactivatedPremanently = false;
        activated = false;
        soh ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//public void activate(){
	//	isActive = true;
	//	soh ();
	//}

	//public void deactivate(){
	//	isActive = false;
	//	soh ();
	//}

	void soh(){
        GetComponent<SpriteRenderer> ().enabled = !activated;
        GetComponent<Collider2D>().enabled = !activated;
        //gameObject.SetActive(!activated);
    }

    public void checkPointReached()
    {
        if (activated)
        {
            deactivatedPremanently = true;
        }
    }

    bool activated = false;
    bool deactivatedPremanently = false;

    public void activate()
    {
        activated = true;
        soh();
    }

    public void reset()
    {
        if (deactivatedPremanently) return;

        activated = false;
        soh();
    }
}
