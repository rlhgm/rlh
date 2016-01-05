using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour, IKnifeCutable, IGResetable
{
    public bool Destroyable = true;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Cut()
    {
        print("Crate::Cut()");

        if( Destroyable )
        {
            gameObject.SetActive(false);
        }
        else
        {

        }
    }

    public void GCacheResetData()
    {

    }

    public void GReset()
    {
        gameObject.SetActive(true);
    }
}
