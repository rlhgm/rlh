using UnityEngine;
using System.Collections;

public class CameraTargetOffseter : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        print("CameraTargetOffseter::OnTriggerEnter2D");
    }

    void OnTriggerExit2D()
    {
        print("CameraTargetOffseter::OnTriggerExit2D");
    }

    void OnTriggerStay2D()
    {
        print("CameraTargetOffseter::OnTriggerStay2D");
    }
}
