using UnityEngine;
using System.Collections;

public class CollapseableFootbridge : MonoBehaviour
{
    public float CollapseDuration = 1f;
    public bool CollapseOnJump = true;
    float CollapseTime = 0f;
    bool collapsing = false;
    
    // Use this for initialization
    void Start()
    {
        CollapseTime = 0f;
        collapsing = false;
        GroundMoveable gm = GetComponent<GroundMoveable>();
        if (!gm)
        {
            Debug.LogError(name + " nie ma GroundMoveable!");
            Debug.Break();
        }
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
        if (collapsing) return;
        CollapseTime = 0f;
        collapsing = true;
    }
    public void Collapse()
    {
        CollapseTime = 0f;
        collapsing = false;
        enabled = false;
        GetComponent<GroundMoveable>().BreakOff();
    }

    public void Reset()
    {
        enabled = true;
        CollapseTime = 0f;
        collapsing = false;
    }
}
