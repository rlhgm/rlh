using UnityEngine;
using System.Collections;

public class ShowInfoTrigger : MonoBehaviour {

	public string Info = "";
	public float ShowDuration = 1f;
	public bool OnlyFirstTime = true;
	public bool used = false;

    public void reset()
    {
        used = false;
    }

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
