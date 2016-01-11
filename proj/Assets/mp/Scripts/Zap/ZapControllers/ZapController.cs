using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface
using System.Collections.Generic;

//[System.Serializable]
//[Serializable]
public class ZapController : ScriptableObject
{

    //	public ZapController (Zap playerController, string controllerName) {
    //		name = controllerName;
    //		zap = playerController;
    //		transform = zap.transform;
    //	}

    //	public ZapController (string controllerName) {
    //		name = controllerName;
    //		//zap = playerController;
    //		//transform = zap.transform;
    //
    //		//target.mainCamera = myCamera;
    //		//target.touchCamera = myCamera.transform.Find("TouchCamera").GetComponent<Camera>();
    ////		Camera mc = Camera.main;
    ////		if (mc) {
    ////			Transform touchCameraTransform = mc.transform.Find ("TouchCamera");
    ////			if( touchCameraTransform ){
    ////				touchCamera = touchCameraTransform.GetComponent<Camera> ();
    ////			}
    ////		}
    //	}

    //	void OnEnable() {
    //		//print("script was enabled");
    //		Camera mc = Camera.main;
    //		if (mc) {
    //			Transform touchCameraTransform = mc.transform.Find ("TouchCamera");
    //			if( touchCameraTransform ){
    //				touchCamera = touchCameraTransform.GetComponent<Camera> ();
    //			}
    //		}
    //	}

    protected WeaponMenuItem weaponMenuItem;

    virtual public void setZap(Zap playerController)
    {
        zap = playerController;
        transform = zap.transform;
        gfx = zap.Gfx;
        //audio = zap.MyAudio;

        if (weaponMenuItem)
        {
            weaponMenuItem.setState(WeaponMenuItem.State.OFF);
        }
    }

    public virtual void MUpdate(float deltaTime, bool firstUpdateInFrame)
    {
    }

    public virtual void FUpdate(float fDeltaTime)
    {
    }

    public virtual void CheckEnergy(float deltaTime)
    {
        if (zap.isDead()) return;
        zap.MyEnergy = Mathf.Min(zap.MyEnergy + deltaTime, zap.MyMaxEnergy);
    }

    public virtual void selected()
    {
        if (weaponMenuItem)
            weaponMenuItem.setState(WeaponMenuItem.State.ON);
    }
    public virtual void deselected()
    {
        if (weaponMenuItem)
            weaponMenuItem.setState(WeaponMenuItem.State.OFF);
    }

    public void activate(bool restore = false, bool crouch = false)
    {
        if (weaponMenuItem) weaponMenuItem.setState(WeaponMenuItem.State.BLINK);
        zap.GfxLegs.gameObject.SetActive(false);
        zap.GravityGunBeam.gameObject.SetActive(false);
        activateSpec(restore, crouch);
    }
    public virtual void activateSpec(bool restore = false, bool crouch = false)
    {
    }
    public virtual void deactivate()
    {
        if (weaponMenuItem)
        {
            if (zap.choosenController == this)
                weaponMenuItem.setState(WeaponMenuItem.State.ON);
            else
                weaponMenuItem.setState(WeaponMenuItem.State.OFF);
        }
    }

    public virtual bool tryDeactiveate()
    {
        return true;
    }

    public virtual void SetCtrlEnabled(bool newCtrlEnable)
    {
        ctrlEnabled = newCtrlEnable;
        if (weaponMenuItem)
            weaponMenuItem.SetVisibility(ctrlEnabled);
    }
    public virtual void SetCtrlEnable()
    {
        SetCtrlEnabled(true);
    }
    public virtual void SetCtrlDisable()
    {
        SetCtrlEnabled(false);
    }

    protected bool isInState(Zap.State test)
    {
        return zap.isInState(test);
    }
    protected bool isNotInState(Zap.State test)
    {
        return zap.isNotInState(test);
    }


    public virtual int keyLeftDown()
    {
        return 0;
    }
    public virtual int keyLeftUp()
    {
        return 0;
    }

    public virtual int keyRightDown()
    {
        return 0;
    }
    public virtual int keyRightUp()
    {
        return 0;
    }

    public virtual int keyUpDown()
    {
        return 0;
    }
    public virtual int keyUpUp()
    {
        return 0;
    }

    public virtual int keyDownDown()
    {
        return 0;
    }
    virtual public int keyDownUp()
    {
        return 0;
    }

    public virtual int keyRunDown()
    {
        return 0;
    }
    public virtual int keyRunUp()
    {
        return 0;
    }

    public virtual int keyJumpDown()
    {
        return 0;
    }
    public virtual int keyJumpUp()
    {
        return 0;
    }

    public virtual bool crouching()
    {
        return false;
    }

    public virtual void zapDie(Zap.DeathType deathType)
    {
        if (weaponMenuItem)
            weaponMenuItem.setState(WeaponMenuItem.State.ON);
    }
    public virtual void beforeReborn()
    {
    }
    public virtual void reborn()
    {
        if (zap.LastTouchedCheckPoint != null)
        {
            if (zap.LastTouchedCheckPoint.startMounted)
            {
                zap.setState(Zap.State.MOUNT);
            }
        }
    }
    public virtual bool triggerEnter(Collider2D other)
    {
        return false;
    }

    public virtual bool isDodging()
    {
        return false;
    }

    //	public string getName(){
    //		return name;
    //	}

    public void setTouchCamera(Camera newTC)
    {
        touchCamera = newTC;
    }

    //ArrayList lastCuttedObjects = new ArrayList();
    HashSet<GameObject> lastCuttedObjects = new HashSet<GameObject>();

    public void cutHigh()
    {
        lastCuttedObjects.Clear();

        Vector2 cutStart;
        Vector2 cutEnd;

        if (zap.faceRight())
        {
            cutStart = zap.rightKnifeHitPointHigh1.position;
            cutEnd = zap.rightKnifeHitPointHigh2.position;
        }
        else
        {
            cutStart = zap.leftKnifeHitPointHigh1.position;
            cutEnd = zap.leftKnifeHitPointHigh2.position;
        }
        cut(cutStart, cutEnd);

        if (zap.faceRight())
        {
            cutStart = zap.rightKnifeHitPointLow1.position;
            cutEnd = zap.rightKnifeHitPointLow2.position;
        }
        else
        {
            cutStart = zap.leftKnifeHitPointLow1.position;
            cutEnd = zap.leftKnifeHitPointLow2.position;
        }
        cut(cutStart, cutEnd);

        lastCuttedObjects.Clear();
    }
    public void cut(Vector2 cutStart, Vector2 cutEnd)
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(cutStart, cutEnd);
        for (int i = 0; i < hits.Length; ++i)
        {
            Collider2D coll = hits[i].collider;
            RopeLink cutRopeLink = coll.GetComponent<RopeLink>();
            if (cutRopeLink)
            {
                if (!lastCuttedObjects.Contains(cutRopeLink.gameObject))
                {
                    //Debug.Log( "trafionione : " + hits[i].collider.name );
                    cutRopeLink.cut();
                    zap.RlhScene.ropeBreakOff(cutRopeLink.rope);
                    zap.playSound(zap.ropeCutSound);

                    lastCuttedObjects.Add(cutRopeLink.gameObject);
                    continue;
                }
            }

            //Snake cutSnake = coll.GetComponent<Snake>();
            //if (cutSnake)
            //{
            //    cutSnake.cut();
            //    return;
            //}

            if (coll.tag == "SnakeHitRegion")
            {
                if (!lastCuttedObjects.Contains(coll.gameObject))
                {
                    IKnifeCutable snakecutable = coll.transform.GetComponent<IKnifeCutable>();
                    if (snakecutable != null)
                    {
                        snakecutable.Cut();
                        lastCuttedObjects.Add(coll.gameObject);
                        continue;
                    }
                    //Panther cutPanther = coll.transform.parent.GetComponent<Panther>();
                    //if( cutPanther ){
                    //	cutPanther.Cut();
                    //	return;
                    //}
                }
            }

                //IKnifeCutable cutable = coll.transform.GetComponent<IKnifeCutable>();
                //if (cutable != null)
                //{
                //    cutable.Cut();
                //}

            CutableBush cutBush = coll.GetComponent<CutableBush>();
            if (cutBush)
            {
                if (!lastCuttedObjects.Contains(cutBush.gameObject))
                {
                    cutBush.cut();
                    lastCuttedObjects.Add(cutBush.gameObject);
                    continue;
                }
            }

            Bird cutBird = coll.GetComponent<Bird>();
            if (cutBird)
            {
                if (!lastCuttedObjects.Contains(cutBird.gameObject))
                {
                    cutBird.cut();
                    lastCuttedObjects.Add(cutBird.gameObject);
                    continue;
                }
            }
            //Bat cutBat = coll.GetComponent<Bat>();
            //if (cutBat)
            //{
            //    if (!lastCuttedObjects.Contains(cutBat.gameObject))
            //    {
            //        cutBat.cut();
            //        lastCuttedObjects.Add(cutBat.gameObject);
            //        continue;
            //    }
            //}

            //Rat cutRat = coll.GetComponent<Rat>();
            //if (cutRat)
            //{
            //    if (!lastCuttedObjects.Contains(cutRat.gameObject))
            //    {
            //        cutRat.cut();
            //        lastCuttedObjects.Add(cutRat.gameObject);
            //        continue;
            //    }
            //}

            if (coll.tag == "PantherHitRegion")
            {
                IKnifeCutable cutablePanthi = coll.transform.parent.GetComponent<IKnifeCutable>();
                if (cutablePanthi != null)
                {
                    if (!lastCuttedObjects.Contains(coll.gameObject))
                    {
                        lastCuttedObjects.Add(coll.gameObject);
                        cutablePanthi.Cut();
                        continue;
                    }
                }
                //Panther cutPanther = coll.transform.parent.GetComponent<Panther>();
                //if( cutPanther ){
                //	cutPanther.Cut();
                //	return;
                //}
            }

            IKnifeCutable otherKnifeCutable = coll.GetComponent<IKnifeCutable>();
            if( otherKnifeCutable != null )
            {
                lastCuttedObjects.Add(coll.gameObject);
                otherKnifeCutable.Cut();
                continue;
            }
        }
    }

    //protected string name;
    protected Zap zap = null;
    protected Transform transform = null;
    protected Transform gfx = null;
    //protected AudioSource audio = null;
    protected Camera touchCamera;
    protected bool ctrlEnabled;
    protected bool firstUpdateInFrame;
}
