using UnityEngine;
using System.Collections;

public class NewZapTest : MonoBehaviour {

    Rigidbody2D body;
	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
	}

    bool moved = false;

	// Update is called once per frame
	void Update () {
	    if(Input.GetKey(KeyCode.LeftArrow))
        {
            body.velocity = new Vector2(-5f,0f);
            moved = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            body.velocity = new Vector2(0f, 0f);
            moved = false;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            body.velocity = new Vector2(5f, 0f);
            moved = true;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            body.velocity = new Vector2(0f, 0f);
            moved = false;
        }
    }

    void FixedUpdate()
    {
        if (!moved)
        {
            body.velocity = new Vector2(0f, 0f);
        }
    }
}
