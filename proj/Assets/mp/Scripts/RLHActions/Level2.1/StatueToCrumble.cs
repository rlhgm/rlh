using UnityEngine;
using System.Collections;

public class StatueToCrumble : MonoBehaviour
{
    public Chandelier MyKiller = null;
    public GameObject chandelierCollider = null;

    Animator myAnimator = null;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
        myAnimator.speed = 0f;
        chandelierCollider = transform.Find("ChandelierCollider").gameObject;
        chandelierCollider.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HitByChandelier(Chandelier chandelier)
    {
        if( chandelier == MyKiller)
        {
            myAnimator.speed = 1f;
            chandelierCollider.SetActive(false);
        }
    }
}
