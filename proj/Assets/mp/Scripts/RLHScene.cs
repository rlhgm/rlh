using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RLHScene : MonoBehaviour
{
    public bool onlyKnife = false;
    public Dictionary<int, bool> ShowInfoTriggersControlls = new Dictionary<int, bool>();
    public Dictionary<int, bool> ShowInfoTriggersControllsApproved = new Dictionary<int, bool>();

    void Awake()
    {
        transform.position = new Vector3(0f,0f,0f);
        Application.targetFrameRate = -1;
    }

    // Use this for initialization
    void Start()
    {
        ShowInfoTrigger[] sits = FindObjectsOfType(typeof(ShowInfoTrigger)) as ShowInfoTrigger[];
        foreach (ShowInfoTrigger sit in sits)
        {
            for( int i = 0; i < sit.controlValues.Length; ++i )
            {
                ShowInfoTriggersControlls[sit.controlValues[i]] = false;
            }
        }
        ShowInfoTriggersControllsApproved = new Dictionary<int, bool>( ShowInfoTriggersControlls );
        //print(ShowInfoTriggersControlls);
    }

    public void activateShowInfoTriggerController(ShowInfoTriggerController sitc)
    {
        for (int i = 0; i < sitc.controlValues.Length; ++i)
        {
            ShowInfoTriggersControlls[sitc.controlValues[i]] = true;
        }
    }

    public bool isActiveShowInfoTrigger(ShowInfoTrigger sit)
    {
        for (int i = 0; i < sit.controlValuesNeg.Length; ++i)
        {
            if (ShowInfoTriggersControlls[sit.controlValuesNeg[i]] == true)
                return false;
        }

        for (int i = 0; i < sit.controlValues.Length; ++i)
        {
            if (ShowInfoTriggersControlls[sit.controlValues[i]] == false)
                return false;
        }

        return true;
    }

    public void printShowInfoTriggersControlls()
    {
        //Dictionary<int,bool>.Enumerator e = ShowInfoTriggersControlls.GetEnumerator();
        //while
        string s = "SITC values : ";
        foreach (KeyValuePair<int, bool> v in ShowInfoTriggersControlls)
        {
            s += "[";
            s += v.Key;
            s += "=>";
            s += v.Value;
            s += "], ";
        }
        print(s);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void reset()
    {
        ShowInfoTriggersControlls = new Dictionary<int, bool>( ShowInfoTriggersControllsApproved );
    }

    public void checkPointReached()
    {
        ShowInfoTriggersControllsApproved = new Dictionary<int, bool>( ShowInfoTriggersControlls );
    }
}
