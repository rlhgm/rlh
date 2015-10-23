using UnityEngine;
using System.Collections;

public class SmashStoneActivator : MonoBehaviour
{
    /// <summary>
    /// bla bla bal
    /// </summary>
    public Collider2D targetStone;
    Vector3 targetStoneStartPos;

    public Pickable pickablePrefab;
    Pickable newPickable = null;

    // Use this for initialization
    void Start()
    {
        if( targetStone )
        {
            targetStoneStartPos = targetStone.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //print("ssa::OnTriggerEnter2D(Collider2D other)");
        if( other == targetStone )
        {
            //print("ssa::OnTriggerEnter2D(Collider2D other)");
            if( pickablePrefab )
            {
                newPickable = Instantiate<Pickable>(pickablePrefab);
                Vector3 startPos = transform.position;
                newPickable.transform.position = startPos;

                targetStone.gameObject.SetActive(false);

                //DestroyObject()
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //print("ssa::OnTriggerStay2D(Collider2D other)");

        //if (isDead())
        //    return;

        //int lid = other.transform.gameObject.layer;
        //if (lid == LayerMask.NameToLayer("GroundMoveable"))
        //{ // layerIdGroundMoveableMask) { // to jest kamien 
        //    if (hitByStone(other.transform))
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        //touchStone(other.transform);
        //    }
        //    return;
        //}
    }

    public void reset()
    {
        if( targetStone )
        {
            if(!targetStone.gameObject.activeSelf)
            {
                targetStone.transform.position = targetStoneStartPos;
                targetStone.gameObject.SetActive(true);
            }
            if( newPickable != null)
            {
                Destroy(newPickable.gameObject);
            }
        }
    }
}
