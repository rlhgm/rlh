using UnityEngine;
using System.Collections;

public class SoundPlayTrigger : MonoBehaviour, IGResetable
{
    public bool OneTimeOnly = false;
    //public SoundSets soundSets = null;
    public string SoundToPlayTag = "";

    bool spriteVisible = false;
    // Use this for initialization
    void Start()
    {
        //this.RLHAssert(soundSets != null, "pusty SoundSet");
        this.RLHAssert(SoundToPlayTag != "", "pusty SoundTag");
        spriteVisible = GetComponent<SpriteRenderer>().enabled;
    }

    //// Update is called once per frame
    //void Update () {

    //}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (currentActive)
        {
            SoundPlayer.Play(gameObject, SoundToPlayTag);
            if (OneTimeOnly)
            {
                currentActive = false;
                //gameObject.SetActive(false);
                if( spriteVisible ) GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
    bool currentActive = true;
    bool startActive;

    public void GResetCacheResetData()
    {
        startActive = currentActive;// gameObject.activeSelf;
        //if (spriteVisible) GetComponent<SpriteRenderer>().enabled = false;
    }

    public void GReset()
    {
        //gameObject.SetActive(startActive);
        currentActive = startActive;
        if (spriteVisible) GetComponent<SpriteRenderer>().enabled = startActive;
    }
}
