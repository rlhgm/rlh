using UnityEngine;
using System.Collections;

public class MountMoveable : MonoBehaviour, IGResetable
{
    //Vector3 startPosition;
    //Quaternion startRotation;
    //bool startActive;
    float ToResetToCollapseTime = -1;
    bool ToResetCollapsed = false;

    //public void GResetCreated()
    //{
    //    RLHScene.Instance.AddIGResetable(this);
    //}

    public void GResetCacheResetData()
    {
        //startPosition = transform.position;
        //startRotation = transform.rotation;
        //startActive = gameObject.activeSelf;

        ToResetToCollapseTime = ToCollapseTime;
        ToResetCollapsed = Collapsed;
    }

    public void GReset()
    {
        //gameObject.SetActive(startActive);
        //transform.position = startPosition;
        //transform.rotation = startRotation;

        gameObject.SetActive(!ToResetCollapsed);
        Collapsed = ToResetCollapsed;
        ToCollapseTime = ToResetToCollapseTime;
    }

    public bool MovingXEnabled = false;
    public bool MovingYEnabled = false;
    public bool MovingInLocal = false;

    public Vector2 mySize = new Vector2();
    BoxCollider2D myBoxCollider = null;

    public float CollapseDuration = -1;
    public bool CollapseOnJump = true;
    public bool ResetOnJump = true;
    public GameObject CollapseParticles = null;

    public string SoundTagCatch = "";
    public string SoundTagCollapse = "";

    float ToCollapseTime = -1;
    bool Collapsed = false;
    
    // Use this for initialization
    void Start()
    {
        myBoxCollider = GetComponent<BoxCollider2D>();
        if (!myBoxCollider)
        {
            Debug.LogError("MountMoveable : " + name + " nie ma BoxCollider2D");
            Debug.Break();
        }

        mySize.x = myBoxCollider.size.x * transform.localScale.x;
        mySize.y = myBoxCollider.size.y * transform.localScale.y;

        ToCollapseTime = CollapseDuration;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool TryToCollapse(float hangTime, Vector3 zapHandPos)
    {
        if (Collapsed) return false;

        if (CollapseDuration > 0f)
        {
            ToCollapseTime -= hangTime;
            if (ToCollapseTime < 0f)
            {
                Collapse(zapHandPos);
                return true;
            }
        }
        return false;
    }

    public void JumpedOut(Vector3 zapHandPos)
    {
        if (ResetOnJump) ToCollapseTime = CollapseDuration;
        if (CollapseOnJump) Collapse(zapHandPos);
    }

    void Collapse(Vector3 collapsePosition)
    {
        gameObject.SetActive(false);
        Collapsed = true;

        if (SoundTagCollapse != "") SoundPlayer.Play(gameObject, SoundTagCollapse, transform.position);

        if (CollapseParticles)
        {
            Object newParticleObject = Instantiate(CollapseParticles, collapsePosition, Quaternion.Euler(0, 0, 0));
            Destroy(newParticleObject, 2.0f);
        }
    }

    public bool LocalPointHandable(Vector3 point)
    {
        //Vector3 rlp = new Vector3();
        //rlp.x = point.x * transform.localScale.x;
        //rlp.y = point.y * transform.localScale.y;

        if (MovingYEnabled)
        {
            if (point.x < 0.0f || point.x > (mySize.x)) return false;
        }
        else
        {
            if (point.x < 0.3f || point.x > (mySize.x - 0.3f)) return false;
        }

        if (MovingXEnabled)
        {
            if (point.y > 0.05f || point.y < (-mySize.y-0.05f)) return false;
        }
        else
        {
            if (point.y > 0.0f || point.y < (-mySize.y)) return false;
        }

        return true;
    }
    public Vector3 ConvertToPointSize(Vector3 point)
    {
        Vector3 rlp = new Vector3();
        rlp.x = point.x * transform.localScale.x;
        rlp.y = point.y * transform.localScale.y;
        return rlp;
    }

    public void Reset()
    {
        ToCollapseTime = CollapseDuration;
        Collapsed = false;
        gameObject.SetActive(true);
    }
}
