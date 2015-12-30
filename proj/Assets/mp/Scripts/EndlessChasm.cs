using UnityEngine;
using System.Collections;

public class EndlessChasm : MonoBehaviour
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
        //print("EndlessChasm::OnTriggerEnter2D");
        RLHScene.Instance.CamController.StopFollow();
    }

    //void OnTriggerExit2D()
    //{
    //    //print("EndlessChasm::OnTriggerExit2D");
    //}

    //void OnTriggerStay2D()
    //{
    //    //print("EndlessChasm::OnTriggerStay2D");
    //}
}
