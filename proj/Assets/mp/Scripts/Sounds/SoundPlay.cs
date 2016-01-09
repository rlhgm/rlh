using UnityEngine;
using System.Collections;

public class SoundPlay : MonoBehaviour
{
    //public string MyTag;
    //public AudioClip[] AudioClips;
    //public Vector2 MinMaxPitch = new Vector2(1f, 1f);
    //public Vector2 MinMaxVolume = new Vector2(1f, 1f);

    public SoundSets soundsSets;

    AudioSource audioSource = null;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            Debug.LogError("SoundPlay " + name + " z " + transform.parent.name + " nie ma AudioSource'a");
            Debug.Break();
        }
        //if (soundsSets) soundsSets.GenerateHashes();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //public bool Play(string SoundTag)
    //{
    //    //if (AudioClips.Length == 0) return false;
    //    //if (MyTag != SoundTag) return false;
    //    //audioSource.pitch = Random.Range(MinMaxPitch.x, MinMaxPitch.y);
    //    //audioSource.volume = Random.Range(MinMaxVolume.x, MinMaxVolume.y);
    //    //audioSource.PlayOneShot(AudioClips[Random.Range(0, AudioClips.Length)]);

    //    //soundsSets.
    //    return true;
    //}

    public bool Play(string SoundTag)
    {
        //return Play(Animator.StringToHash(SoundTag));
        AudioClipData acd = soundsSets.GetRandomAudioClip(SoundTag);
        if (acd != null)
        {
            audioSource.PlayOneShot(acd.clip,Random.Range(acd.VolumeMinMax.x, acd.VolumeMinMax.y));
            //AudioSource.PlayClipAtPoint(ac, RLHScene.Instance.Zap.transform.position);
            return true;
        }
        return false;
    }

    //public bool Play(int SoundTagHash)
    //{
    //    AudioClip ac = soundsSets.GetRandomAudioClip(SoundTagHash);
    //    if (ac)
    //    {
    //        //audioSource.PlayOneShot(ac);
    //        AudioSource.PlayClipAtPoint(ac,RLHScene.Instance.Zap.transform.position);
    //        return true;
    //    }
    //    return false;
    //}
}
