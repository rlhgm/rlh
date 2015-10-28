using UnityEngine;
using System.Collections;

public class SnakeCollider : MonoBehaviour, IKnifeCutable
{

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
        //cutSnake.cut();
        //    return;
        Snake snake = transform.parent.GetComponent<Snake>();
        if (snake != null)
        {
            snake.cut();
        }
    }
}
