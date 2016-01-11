using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface
using System.Collections.Generic;

//[Serializable]
//public class AudioClipData
//{
//    public AudioClip clip = null;
//    public Vector2 VolumeMinMax = new Vector2(1f, 1f);
//}

[Serializable]
public class ParticleSet
{
    //public void GenerateHash()
    //{
    //    SoundTagHash = Animator.StringToHash(SoundTag);
    //}

    public string SoundTag;
    public AudioClipData[] clips;
    //[HideInInspector]
    ///*public */int SoundTagHash;


    //public int SoundTagHash1
    //{
    //    get
    //    {
    //        return SoundTagHash;
    //    }

    //    //set
    //    //{
    //    //    SoundTagHash = value;
    //    //}
    //}

    //public string SoundTag1
    //{
    //    get
    //    {
    //        return SoundTag;
    //    }

    //    set
    //    {
    //        SoundTag = value;
    //    }
    //}
}

////[System.Serializable]
////[Serializable]
//public class SoundSets : ScriptableObject
//{
//    public SoundSet[] SndSet;

//    void OnEnable()
//    {
//        //Debug.Log("SoundSets::OnEnable()");
//        //int numberOfSndSets = SndSet.Length;
//        //for (int i = 0; i < numberOfSndSets; ++i)
//        //{
//        //    SoundSet ss = SndSet[i];
//        //    ss.OnEnable();
//        //}

//        //GenerateHashes();
//    }
    
//    public AudioClipData GetRandomAudioClip(string SndTag/*int SndTagHash*/)
//    {
//        int numberOfSndSets = SndSet.Length;
//        for (int i = 0; i < numberOfSndSets; ++i)
//        {
//            SoundSet ss = SndSet[i];
//            if (ss.SoundTag != SndTag) continue;
//            if (ss.clips.Length == 0) return null;
//            return ss.clips[UnityEngine.Random.Range(0, ss.clips.Length)];
//        }
//        return null;
//    }
//}
