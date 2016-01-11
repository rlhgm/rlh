using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface
using System.Collections.Generic;

public class Surface : MonoBehaviour
{
    public enum Type
    {
        Default,
        Ground,
        Grass,
        Stone,
        Wood
    }
    
    public Type type = Type.Default;
    //public ParticleData ZapLandingParticles = null;
    //public ParticleData ZapLandingHardParticles = null;

    // Use this for initialization
    void Start()
    {
        //this.RLHAssert()
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
