using UnityEngine;
using System.Collections;

public class SmashStoneActivator : MonoBehaviour
{
    /// <summary>
    /// bla bla bal
    /// </summary>
    public Collider2D targetStone;
    public Pickable pickablePrefab;

    // Use this for initialization
    void Start()
    {

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
                Pickable newPickable = Instantiate<Pickable>(pickablePrefab);
                Vector3 startPos = transform.position;
                newPickable.transform.position = startPos;

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
}
