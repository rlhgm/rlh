using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface
using System.Collections.Generic;

[Serializable]
public class ParticlesData
{
    public string ParticleTag;
    public GameObject ParticlePrefab = null;
    public float LifeTime = 2f;
}

