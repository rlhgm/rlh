using UnityEngine;
using System.Collections;

public class CollapseableFootbridge : MonoBehaviour
{
    public float CollapseDuration = 1f;
    float CollapseTime = 0f;
    bool collapsing = false;
    
    // Use this for initialization
    void Start()
    {
        CollapseTime = 0f;
        collapsing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (collapsing)
        {
            if ((CollapseTime += Time.deltaTime) >= CollapseDuration)
            {
                Collapse();
            }
        }
    }

    public void StartCollapse()
    {
        CollapseTime = 0;
        collapsing = true;
    }
    void Collapse()
    {
        CollapseTime = 0f;
        collapsing = false;
    }
}
