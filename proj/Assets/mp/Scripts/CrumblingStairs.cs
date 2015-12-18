using UnityEngine;
using System.Collections;

public class CrumblingStairs : MonoBehaviour
{
    Transform ground = null;
    Transform handle = null;
    Transform mount = null;

    // Use this for initialization
    void Start()
    {
        ground = transform.Find("ground");
        handle = transform.Find("handle");
        mount = transform.Find("mount");

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

        mount.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Clumble()
    {
        ground.gameObject.SetActive(false);
        handle.gameObject.SetActive(false);
        mount.gameObject.SetActive(true);
    }

    public void Reset()
    {

    }
}
