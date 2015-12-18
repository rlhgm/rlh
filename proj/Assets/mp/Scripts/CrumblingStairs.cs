using UnityEngine;
using System.Collections;

public class CrumblingStairs : MonoBehaviour
{
    Transform ground = null;
    Transform handle = null;
    Transform mount = null;
    Transform mountHandle = null;

    public Transform MountHandle
    {
        get { return mountHandle; }
    }

    public Transform Mount
    {
        get { return mount; }
    }

    // Use this for initialization
    void Start()
    {
        ground = transform.Find("ground");
        handle = transform.Find("handle");
        mount = transform.Find("mount");
        mountHandle = transform.Find("mountHandle");

        if (!ground)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma ground");
            Debug.Break();
        }
        if (!handle)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma handle");
            Debug.Break();
        }
        if (!mount)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma mount");
            Debug.Break();
        }
        if (!mountHandle)
        {
            Debug.LogError("CrumblingStairs : " + name + " nie ma mountHandle");
            Debug.Break();
        }

        Rigidbody2D mountBody = mount.GetComponent<Rigidbody2D>();
        //mountBody.centerOfMass = new Vector2(0f, 0f);

        mount.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Crumble()
    {
        ground.gameObject.SetActive(false);
        handle.gameObject.SetActive(false);
        mount.gameObject.SetActive(true);

        Rigidbody2D mountBody = mount.GetComponent<Rigidbody2D>();
        mountBody.isKinematic = false;
    }

    public void Reset()
    {

    }
}
