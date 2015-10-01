using UnityEngine;
using System.Collections;

public class BirdEmiter : MonoBehaviour
{

    //BoxCollider2D coll;
    float timeToNext;
    float timeFromLast;

    //public Vector2 dir;
    //public Vector2 timeToNextMinMax;
    //public Vector2 birdsSpeedMinMax;
    public float EmitFrequency;
    public float BirdsSpeed;

    public Bird birdPrefab;

    bool onOff = false;

    public bool OnOff
    {
        get
        {
            return onOff;
        }
        set
        {
            onOff = value;
        }
    }
    void Awake()
    {
        //coll = GetComponent<BoxCollider2D> ();
        //dir = -Vector2.right;
        //timeToNextMinMax = new Vector2 (1, 3);
        //birdsSpeedMinMax = new Vector3 (2, 5);

        //timeToNext = 0f; //EmitFrequency; //timeToNextMinMax.x;
        //timeFromLast = 0f;

        reset();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (onOff)
        {
            timeFromLast += Time.deltaTime;
            if (timeFromLast >= timeToNext)
            {
                emit();
            }
        }
    }

    public void reset()
    {
        timeToNext = 0f;
        timeFromLast = 0f;
    }

    void emit()
    {
        if (!birdPrefab)
            return;
        if (!onOff)
            return;

        //print (transform.right);

        Bird newBird = Instantiate<Bird>(birdPrefab);
        Vector3 startPos = transform.position;
        //float szy = coll.size.y * 0.5f;
        //startPos.y += Random.Range (-szy, szy);
        newBird.transform.position = startPos;
        //newBird.setDir (dir);
        newBird.setDir(transform.right);
        //newBird.setSpeed( Random.Range(birdsSpeedMinMax.x,birdsSpeedMinMax.y) );
        newBird.setSpeed(BirdsSpeed);

        //timeToNext = Random.Range (timeToNextMinMax.x, timeToNextMinMax.y);
        timeToNext = EmitFrequency;
        timeFromLast = 0.0f;
    }
}
