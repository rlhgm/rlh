using UnityEngine;
using System.Collections;

public class AutoDestroyer : MonoBehaviour
{
    public float TimeToDestroy = 2f;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, TimeToDestroy);
    }

    //// Update is called once per frame
    //void Update () {

    //}
}
