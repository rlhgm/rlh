using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RLHScene : MonoBehaviour
{
    public bool onlyKnife = false;
    public Dictionary<int, bool> ShowInfoTriggersControlls = new Dictionary<int, bool>();

    void Awake()
    {
        transform.position = new Vector3(0f,0f,0f);
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
        print(ShowInfoTriggersControlls);
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
        for (int i = 0; i < sit.controlValues.Length; ++i)
        {
            if (ShowInfoTriggersControlls[sit.controlValues[i]] == false)
                return false;
        }

        return true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
