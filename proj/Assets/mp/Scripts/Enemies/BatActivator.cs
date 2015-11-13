using UnityEngine;
using System.Collections;

public class BatActivator : MonoBehaviour
{
    public Bat[] bats;

    // Use this for initialization
    void Start()
    {
        foreach (Bat bat in bats)
        {
            bat.Activator = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //print("BatActivator::OnTriggerEnter2D " + other.name);
        foreach( Bat bat in bats )
        {
            bat.ZapIsHere();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //print("BatActivator::OnTriggerEnter2D " + other.name);
        foreach (Bat bat in bats)
        {
            bat.ZapIsHere();
        }
    }
}
