using UnityEngine;
using System.Collections;

public class RatsHole : MonoBehaviour
{
    public RatsHole ExitHole = null;

    // Use this for initialization
    void Start()
    {
        if (!ExitHole)
        {
            Debug.LogError("RatsHole : " + name + " nie ma zdefiniowanego wyjscia!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
