using UnityEngine;
using System.Collections;

public class StatueToCrumble : MonoBehaviour
{
    public Chandelier MyKiller = null;
    public GameObject chandelierCollider = null;
    public GameObject crumbledGrounds = null;

    Animator myAnimator = null;

    bool resetCrumbledGroundsActive = false;
    bool resetChandelierColliderActive = false;

    public void CacheResetData()
    {
        resetCrumbledGroundsActive = crumbledGrounds.activeSelf;
        resetChandelierColliderActive = chandelierCollider.activeSelf;
    }
    public void Reset()
    {
        crumbledGrounds.SetActive(resetCrumbledGroundsActive);
        chandelierCollider.SetActive(resetChandelierColliderActive);

        if( resetCrumbledGroundsActive == false) // pomnik jeszcze nie przewrocony
        {
            myAnimator.Play("idle");
            myAnimator.speed = 0f;
        }
    }

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
        myAnimator.speed = 0f;
        chandelierCollider = transform.Find("ChandelierCollider").gameObject;
        chandelierCollider.SetActive(false);

        crumbledGrounds = transform.Find("CrumbledGrounds").gameObject;
        crumbledGrounds.SetActive(false);

        CacheResetData();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HitByChandelier(Chandelier chandelier)
    {
        if( chandelier == MyKiller)
        {
            myAnimator.Play("crumble");
            myAnimator.speed = 1f;

            //myAnimator.speed = 1f;
            chandelierCollider.SetActive(false);
            crumbledGrounds.SetActive(true);
        }
    }
}
