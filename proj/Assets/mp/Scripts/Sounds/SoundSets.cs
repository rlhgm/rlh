using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface
using System.Collections.Generic;

[Serializable]
public struct SoundSet
{
    //public SoundSet()
    //{
    //    SoundTagHash = Animator.StringToHash(SoundTag);
    //}

    public void OnEnable()
    {
        //Debug.Log("SoundSet() => " + SoundTag + " => " + Animator.StringToHash(SoundTag));
        SoundTagHash = Animator.StringToHash(SoundTag);
    }

    public string SoundTag;
    public AudioClip[] clips;
    [HideInInspector]
    /*public */int SoundTagHash;

    public int SoundTagHash1
    {
        get
        {
            return SoundTagHash;
        }

        //set
        //{
        //    SoundTagHash = value;
        //}
    }
}

//[System.Serializable]
//[Serializable]
public class SoundSets : ScriptableObject
{
    public SoundSet[] SndSet;

    void OnEnable()
    {
        Debug.Log("SoundSets::OnEnable()");
        int numberOfSndSets = SndSet.Length;
        for (int i = 0; i < numberOfSndSets; ++i)
        {
            SoundSet ss = SndSet[i];
            ss.OnEnable();
        }
    }

    public AudioClip GetRandomAudioClip(string SndTag)
    {
        return GetRandomAudioClip(Animator.StringToHash(SndTag));
    }

    public AudioClip GetRandomAudioClip(int SndTagHash)
    {
        int numberOfSndSets = SndSet.Length;
        for (int i = 0; i < numberOfSndSets; ++i)
        {
            SoundSet ss = SndSet[i];
            if (ss.SoundTagHash1 != SndTagHash) continue;
            if (ss.clips.Length == 0) return null;
            return ss.clips[UnityEngine.Random.Range(0, ss.clips.Length)];
        }
        return null;
    }
}
