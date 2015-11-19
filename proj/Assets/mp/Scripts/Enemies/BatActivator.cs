using UnityEngine;
using System.Collections;

public class BatActivator : MonoBehaviour
{
    public Bat[] bats;
    bool _zapIn = false;

    public bool ZapIn
    {
        get
        {
            return _zapIn;
        }
    }

    // Use this for initialization
    void Start()
    {
        foreach (Bat bat in bats)
        {
            if (bat)
            {
                bat.Activator = this;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      //  print("BatActivator::OnTriggerEnter2D " + other.name);
        _zapIn = true;
        foreach( Bat bat in bats )
        {
            if (bat)
            {
                bat.ZapIsHere();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _zapIn = false;
        //print("BatActivator::OnTriggerExit2D " + other.name);
        foreach (Bat bat in bats)
        {
            if (bat)
            {
                bat.ZapEscape();
            }
        }
    }
}
