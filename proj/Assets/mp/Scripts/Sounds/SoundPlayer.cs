using UnityEngine;
using System.Collections;

public static class SoundPlayer
{
    //public static bool Play(GameObject obj, string SoundTag)
    //{
    //    SoundPlay[] soundPlays = obj.GetComponents<SoundPlay>();
    //    int numberOfSoundPlays = soundPlays.Length;
    //    for (int i = 0; i < numberOfSoundPlays; ++i)
    //    {
    //        if (soundPlays[i].Play(SoundTag)) return true;
    //    }
    //    Debug.LogError("SoundPlayer : " + obj.name + " nie moze odegrac : " + SoundTag);
    //    return false;
    //}

    public static bool Play(GameObject obj, string SoundTag)
    {
        SoundPlay[] soundPlays = obj.GetComponents<SoundPlay>();
        //int SoundTagHash = Animator.StringToHash(SoundTag);
        int numberOfSoundPlays = soundPlays.Length;
        for (int i = 0; i < numberOfSoundPlays; ++i)
        {
            if (soundPlays[i].Play(SoundTag)) return true;
        }
        Debug.LogError("SoundPlayer : " + obj.name + " nie moze odegrac : " + SoundTag);
        return false;
    }

    public static bool Play(GameObject obj, string SoundTag, Vector3 soundPosition)
    {
        SoundPlay[] soundPlays = obj.GetComponents<SoundPlay>();
        //int SoundTagHash = Animator.StringToHash(SoundTag);
        int numberOfSoundPlays = soundPlays.Length;
        for (int i = 0; i < numberOfSoundPlays; ++i)
        {
            if (soundPlays[i].Play(SoundTag,soundPosition)) return true;
        }
        Debug.LogError("SoundPlayer : " + obj.name + " nie moze odegrac : " + SoundTag);
        return false;
    }
}
