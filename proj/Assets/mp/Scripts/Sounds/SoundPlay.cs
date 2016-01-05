﻿using UnityEngine;
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
        return Play(Animator.StringToHash(SoundTag));
    }

    public bool Play(int SoundTagHash)
    {
        AudioClip ac = soundsSets.GetRandomAudioClip(SoundTagHash);
        if (ac)
        {
            audioSource.PlayOneShot(ac);
            return true;
        }
        return false;
    }
}