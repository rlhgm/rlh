using UnityEngine;
using System.Collections;

public class CreepingAITrigger : MonoBehaviour
{
    public int controlID = 0;

    // Use this for initialization
    void Start()
    {
        setState(State.ON_GROUND);
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        stateJustChanged = false;
        currentStateTime += dt;
        currentActionTime += dt;
    }

    //void OnTrigg
    void OnTriggerEnter2D(Collider2D other)
    {

    }

    public float CurrentStateTime
    {
        get { return currentStateTime; }
    }
    
    public enum State
    {
        ON_GROUND = 0,
        IN_AIR,
        DEAD,
        OTHER
    };

    State state;
    float currentStateTime = 0.0f;
    float currentActionTime = 0.0f;
    bool stateJustChanged = true;

    public bool setState(State newState)
    {
        if (state == newState)
            return false;

        currentStateTime = 0.0f;
        stateJustChanged = true;

        state = newState;
        
        return true;
    }
    public bool isInState(State test)
    {
        return state == test;
    }
    public bool isNotInState(State test)
    {
        return state != test;
    }
}
