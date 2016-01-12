using UnityEngine;
using System.Collections;

public class DigToLightRays : RLHAction
{
    GameObject[] rays = new GameObject[3];
    public Bat[] bats;
    //GameObject ray2;
    //GameObject ray3;

    void Start()
    {
        rays[0] = transform.Find("ray1").gameObject;
        rays[1] = transform.Find("ray2").gameObject;
        rays[2] = transform.Find("ray3").gameObject;

        SaveResets();
    }

    protected override int PerformSpec()
    {
        for (int i = 0; i < 3; ++i)
        {
            rays[i].SetActive(true);
        }
        for(int i = 0; i < bats.Length; ++i)
        {
            Bat bat = bats[i];
            bat.gameObject.SetActive(true);
            bat.ZapIsHere(true);
        }
        return 0;
    }

    bool[] resetActives = new bool[3];
    bool[] batsActives;

    public override void SaveResets()
    {
        for (int i = 0; i < 3; ++i)
        {
            resetActives[i] = rays[i].activeSelf;
        }
        batsActives = new bool[bats.Length];
        for (int i = 0; i < bats.Length; ++i)
        {
            Bat bat = bats[i];
            batsActives[i] = bat.gameObject.activeSelf;
        }
    }
    public override void Reset()
    {
        for (int i = 0; i < 3; ++i)
        {
            rays[i].SetActive(resetActives[i]);
        }
        for (int i = 0; i < batsActives.Length; ++i)
        {
            Bat bat = bats[i];
            bat.gameObject.SetActive(batsActives[i]);
        }
    }

    //public GroundMoveable
    //public Chandelier MyKiller = null;
    //public GameObject chandelierCollider = null;
    //public GameObject crumbledGrounds = null;

    //Animator myAnimator = null;

    //bool resetCrumbledGroundsActive = false;
    //bool resetChandelierColliderActive = false;

    //public void CacheResetData()
    //{
    //    resetCrumbledGroundsActive = crumbledGrounds.activeSelf;
    //    resetChandelierColliderActive = chandelierCollider.activeSelf;
    //}
    //public void Reset()
    //{
    //    crumbledGrounds.SetActive(resetCrumbledGroundsActive);
    //    chandelierCollider.SetActive(resetChandelierColliderActive);

    //    if (resetCrumbledGroundsActive == false) // pomnik jeszcze nie przewrocony
    //    {
    //        myAnimator.Play("idle");
    //        myAnimator.speed = 0f;
    //    }
    //}

    //// Use this for initialization
    //void Start()
    //{
    //    myAnimator = GetComponent<Animator>();
    //    myAnimator.speed = 0f;
    //    chandelierCollider = transform.Find("ChandelierCollider").gameObject;
    //    chandelierCollider.SetActive(false);

    //    crumbledGrounds = transform.Find("CrumbledGrounds").gameObject;
    //    crumbledGrounds.SetActive(false);

    //    CacheResetData();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //public void HitByChandelier(Chandelier chandelier)
    //{
    //    if (chandelier == MyKiller)
    //    {
    //        myAnimator.Play("crumble");
    //        myAnimator.speed = 1f;

    //        //myAnimator.speed = 1f;
    //        chandelierCollider.SetActive(false);
    //        crumbledGrounds.SetActive(true);
    //    }
    //}
}
