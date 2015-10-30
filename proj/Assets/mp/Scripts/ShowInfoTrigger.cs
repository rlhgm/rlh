using UnityEngine;
using System.Collections;

public class ShowInfoTrigger : MonoBehaviour {

    //public string Info;
    //public float ShowDuration;

    public string[] Infos;
	public float[] ShowDurations;
	public bool OnlyFirstTime = true;
	public bool used = false;
    public int[] controlValues;

    public void reset()
    {
        used = false;
    }

    public int getNumberOfInfos()
    {
        if (Infos.Length != ShowDurations.Length) return 0;
        return Infos.Length;
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
