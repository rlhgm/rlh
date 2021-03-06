﻿using UnityEngine;
using System.Collections;

public static class ParticleInseter
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
    //= Quaternion.Euler(0f, 0f, 0f)

    public static Object Insert(ParticleData particleData, Vector3 position, Quaternion rotation, bool autoDestroyed = true)
    {
        Object newParticleObject = GameObject.Instantiate(particleData.ParticlePrefab, position, rotation);
        if( autoDestroyed ) GameObject.Destroy(newParticleObject, particleData.LifeTime);
        return newParticleObject;
    }

    public static Object Insert(ParticleData particleData, Vector3 position, bool autoDestroyed = true)
    {
        Object newParticleObject = GameObject.Instantiate(particleData.ParticlePrefab, position, Quaternion.Euler(0f, 0f, 0f));
        if( autoDestroyed ) GameObject.Destroy(newParticleObject, particleData.LifeTime);
        return newParticleObject;
        //return false;
        //SoundPlay[] soundPlays = obj.GetComponents<SoundPlay>();
        ////int SoundTagHash = Animator.StringToHash(SoundTag);
        //int numberOfSoundPlays = soundPlays.Length;
        //for (int i = 0; i < numberOfSoundPlays; ++i)
        //{
        //    if (soundPlays[i].Play(SoundTag)) return true;
        //}
        //Debug.LogError("SoundPlayer : " + obj.name + " nie moze odegrac : " + SoundTag);
        //return false;
    }

    public static bool InsertParticle(GameObject obj, string SoundTag)
    {
        return false;
        //SoundPlay[] soundPlays = obj.GetComponents<SoundPlay>();
        ////int SoundTagHash = Animator.StringToHash(SoundTag);
        //int numberOfSoundPlays = soundPlays.Length;
        //for (int i = 0; i < numberOfSoundPlays; ++i)
        //{
        //    if (soundPlays[i].Play(SoundTag)) return true;
        //}
        //Debug.LogError("SoundPlayer : " + obj.name + " nie moze odegrac : " + SoundTag);
        //return false;
    }

    public static bool Play(GameObject obj, string SoundTag, Vector3 soundPosition)
    {
        SoundPlay[] soundPlays = obj.GetComponents<SoundPlay>();
        //int SoundTagHash = Animator.StringToHash(SoundTag);
        int numberOfSoundPlays = soundPlays.Length;
        for (int i = 0; i < numberOfSoundPlays; ++i)
        {
            if (soundPlays[i].Play(SoundTag, soundPosition)) return true;
        }
        Debug.LogError("SoundPlayer : " + obj.name + " nie moze odegrac : " + SoundTag);
        return false;
    }
}
