using UnityEngine;
using System.Collections;

public class BatActivator : MonoBehaviour
{
    public Bat[] bats;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print("BatActivator::OnTriggerEnter2D " + other.name);
    }
}
