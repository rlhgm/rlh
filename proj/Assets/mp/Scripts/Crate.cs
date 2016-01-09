using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour, IKnifeCutable, IGResetable
{
    public bool Destroyable = true;

    // Use this for initialization
    void Start()
    {
        //GResetCreated();
        //startPosition = transform.position;
        //startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Cut()
    {
        //print("Crate::Cut()");

        if( Destroyable )
        {
            gameObject.SetActive(false);
        }
        else
        {

        }
    }
    
    Vector3 startPosition;
    Quaternion startRotation;
    bool startActive;

    //public void GResetCreated()
    //{
    //    RLHScene.Instance.AddIGResetable(this);
    //}

    public void GResetCacheResetData()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startActive = gameObject.activeSelf;
    }

    public void GReset()
    {
        gameObject.SetActive(startActive);
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
