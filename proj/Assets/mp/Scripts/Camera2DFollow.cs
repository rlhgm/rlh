using System;
using UnityEngine;

//[ExecuteInEditMode]
public class Camera2DFollow : MonoBehaviour
{
    //public Transform target;

    Zap target;

    //Transform camTarget;
    //public float damping = 1;
    //public float lookAheadFactor = 3;
    //public float lookAheadReturnSpeed = 0.5f;
    //public float lookAheadMoveThreshold = 0.1f;

    //private float m_OffsetZ;
    //private Vector3 m_LastTargetPosition;
    //private Vector3 m_CurrentVelocity;
    //private Vector3 m_LookAheadPos;

    public Transform[] backgroundsNodes;
    public Vector2[] backgroundsRatios;

    public GameObject[] backgroundsBackgrounds;

    //private Vector3 lastPos;

    public Vector2 stageSize = new Vector2(20f, 10f);
    public Vector2 stagesOffset = new Vector2(0f, 0f);
    // jak duzo jednosek pokazuje na ekranie
    Vector2 hms = new Vector2();

    public Vector2 targetStage = new Vector2();

    public Vector2 backgroundLimits = new Vector2(-30, 30);

    public Camera myCamera;

    Parallaxed[] parallaxedObjects;

    RLHScene rlhScene = null;

    void Awake()
    {
        //public GameObject[] backgroundsBackgrounds;
        //to niby ma robic ze wszystkie pixele maja taki sam rozmiar w kamerze
        //float s_baseOrthographicSize = Screen.height / 64.0f / 2.0f;
        //Camera.main.orthographicSize = s_baseOrthographicSize;
        //print("Camera2DFollow::Awake()");



        //for (int i = 0; i < backgroundsNodes.Length; ++i)
        //{
        //    if (backgroundsNodes[i] == null) continue;
        //    if (backgroundsBackgrounds[i] == null) continue;

        //    GameObject bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
        //    bckg.transform.position = new Vector3(0f, 1f, 0f);
        //    bckg.transform.parent = backgroundsNodes[i];

        //    SpriteRenderer bckgSpriteRend = bckg.GetComponent<SpriteRenderer>();
        //    //print(bckgSpriteRend.bounds);
        //    //print(bckgSpriteRend.sprite.border);

        //    Vector3 bckgItemSize = bckgSpriteRend.bounds.extents;

        //    float bckgDist = bckgItemSize.x;
        //    int c = 1;

        //    while (bckgDist < backgroundLimits.y)
        //    {
        //        //c = 1;
        //        bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
        //        bckg.transform.position = new Vector3(bckgDist + bckgItemSize.x, 1f, 0f);
        //        bckg.transform.parent = backgroundsNodes[i];
        //        bckgDist = bckgItemSize.x + c * (bckgItemSize.x * 2);
        //        c += 1;
        //    }

        //    bckgDist = -bckgItemSize.x;
        //    c = 1;

        //    while (bckgDist > backgroundLimits.x)
        //    {
        //        //c = 1;
        //        bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
        //        bckg.transform.position = new Vector3(bckgDist - bckgItemSize.x, 1f, 0f);
        //        bckg.transform.parent = backgroundsNodes[i];
        //        bckgDist = -bckgItemSize.x - c * (bckgItemSize.x * 2);
        //        c += 1;
        //    }
        //}
    }

    // Use this for initialization
    private void Start()
    {
        parallaxedObjects = FindObjectsOfType(typeof(Parallaxed)) as Parallaxed[];
        
        Zap[] zappers = FindObjectsOfType(typeof(Zap)) as Zap[];
        if( zappers.Length == 1)
        {
            target = zappers[0];
        }
        
        //if (target) {
        //Transform camTarget = target.getCameraTarget();
        //}
        ///aaa
        myCamera = GetComponent<Camera>();
        //target.mainCamera = myCamera;
        //target.touchCamera = 
        Transform tct = myCamera.transform.Find("TouchCamera");
        if (tct != null)
        {
            Camera tc = tct.GetComponent<Camera>();
            target.setTouchCamera(tc);
        }
        else
        {
            //				Camera tc = new Camera();
            //				tc.transform.SetParent( myCamera.transform );
            //				tc.name = "TouchCamera";
            //				tc.orthographic = true;
            //				target.setTouchCamera (tc);
        }

        hms.x = myCamera.orthographicSize * GetComponent<Camera>().aspect;
        hms.y = myCamera.orthographicSize;

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        transform.parent = null;

        rlhScene = target.RlhScene;
        //lastPos = transform.position;

        for (int i = 0; i < backgroundsNodes.Length; ++i)
        {
            //if (i > 1) continue;

            if (backgroundsNodes[i] == null) continue;
            if (backgroundsBackgrounds[i] == null) continue;

            backgroundsNodes[i].position = rlhScene.transform.InverseTransformPoint(rlhScene.levelBounds.Center3);

            GameObject bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
            bckg.transform.parent = backgroundsNodes[i];
            bckg.transform.localPosition = ( new Vector3(0f, 1f, 0f) );
            

            SpriteRenderer bckgSpriteRend = bckg.GetComponent<SpriteRenderer>();
            //print(bckgSpriteRend.bounds);
            //print(bckgSpriteRend.sprite.border);

            Vector3 bckgItemSize = bckgSpriteRend.bounds.extents;

            float bckgDist = -bckgItemSize.x * 2;// bckgItemSize.x;
            //int c = 1;
            while (bckgDist > -rlhScene.levelBounds.SceneSize.x * 0.5f - (bckgItemSize.x * 2) ) // (rlhScene.levelBounds.SceneMin.x - (bckgItemSize.x * 2)))
            {
                //c = 1;
                bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
                bckg.transform.parent = backgroundsNodes[i];
                bckg.transform.localPosition = (new Vector3(bckgDist, 1f, 0f));
                bckgDist -= bckgItemSize.x * 2; // + c * (bckgItemSize.x * 2);
                                                //    c += 1;
            }

            bckgDist = bckgItemSize.x * 2;// bckgItemSize.x;
            //int c = 1;
            while (bckgDist < rlhScene.levelBounds.SceneSize.x*0.5f + (bckgItemSize.x * 2))
            {
                //c = 1;
                bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
                bckg.transform.parent = backgroundsNodes[i];
                bckg.transform.localPosition = (new Vector3(bckgDist, 1f, 0f));
                bckgDist += bckgItemSize.x * 2; // + c * (bckgItemSize.x * 2);
                                                //    c += 1;
            }
            //bckgDist = -bckgItemSize.x;
            //c = 1;

            //float sceneRangeX = rlhScene.levelBounds.SceneMin.x; ///Mathf.Max(rlhScene.levelBounds.SceneMin.x, rlhScene.levelBounds.SceneMax.x);

            //while (bckgDist > sceneRangeX) // backgroundLimits.x)
            //{
            //    //c = 1;
            //    bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
            //    bckg.transform.position = new Vector3(bckgDist - bckgItemSize.x, 1f, 0f);
            //    bckg.transform.parent = backgroundsNodes[i];
            //    bckgDist = -bckgItemSize.x - c * (bckgItemSize.x * 2);
            //    c += 1;
            //}

            //float sceneRangeX = rlhScene.levelBounds.SceneMax.x - rlhScene.levelBounds.SceneMin.x; ///Mathf.Max(rlhScene.levelBounds.SceneMin.x, rlhScene.levelBounds.SceneMax.x);

            //c = 0;
            //while (bckgDist > sceneRangeX) // backgroundLimits.x)
            //{
            //    //c = 1;
            //    bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
            //    bckg.transform.position = new Vector3(bckgDist - bckgItemSize.x, 1f, 0f);
            //    bckg.transform.parent = backgroundsNodes[i];
            //    bckgDist = -bckgItemSize.x - c * (bckgItemSize.x * 2);
            //    c += 1;
            //}
        }
    }

    //public void resetPosToTarget()
    //{
    //    Update();
    //}


    public void Shake(float duration, float amplitude)
    {
        shaking = true;
        shakeTime = 0;
        shakeAmplitude = amplitude;

    }

    bool shaking = false;
    float shakeDuration = 0;
    float shakeTime = 0;
    float shakeMaxAmplitude = 0f;
    float shakeAmplitude = 0;

    // Update is called once per frame
    private void Update()
    {
        //			print ("---------------------------------------");
        //			//print (camera.orthographicSize);
        //			float asp = (float)Screen.width / (float)Screen.height;
        //			float aspInv = (float)Screen.height / (float)Screen.width;
        //			float horzExtent = camera.orthographicSize * asp;
        //			print ( Screen.width + " x " +  Screen.height + " aspect: " + asp + " " + aspInv + " " + horzExtent);
        //			
        //			print ("---------------------------------------");
        //Vector3 oldPos = transform.position;

        //parallaxedObjects = FindObjectsOfType(typeof(Parallaxed)) as Parallaxed[];
        


        targetStage = getTargetStage();

        Transform camTarget = target.getCameraTarget();

        //Vector3 newPos = new Vector3( target.transform.position.x, target.transform.position.y, transform.position.z );
        Vector3 newPos = new Vector3(camTarget.position.x, camTarget.position.y, transform.position.z);

        if (target.isInState(Zap.State.CLIMB_ROPE))
        {
            newPos.y -= camTarget.localPosition.y;
        } //else {
          //	newPos.y += target.cameraTargetNormalDiffY;
          //}

        Vector3 res = fitToStage(targetStage, newPos);

        Vector3 posDiff = res - transform.position;

        if (target.isInState(Zap.State.CLIMB_ROPE))
        {

            //float pdm = posDiff.magnitude;

            //if (pdm > 0.35f)
            //    pdm = 0.35f;

            //if (pdm < 0.1f)
            //    pdm = (pdm * pdm);
            //else
            //    pdm = 1f;

            //transform.position = transform.position + posDiff * pdm;

            transform.position = Vector3.LerpUnclamped(transform.position, transform.position + posDiff, 0.1f);
        }
        else
        {
            transform.position = transform.position + posDiff;
        }

        fitPosToSceneBounds();

        int numberOfBackgrounds = backgroundsNodes.Length;

        if (numberOfBackgrounds != backgroundsRatios.Length)
        {
            print("numberOfBackgrounds != backgroundsRatios.GetLength ()");
            return;
        }

        Vector3 _cd = transform.position - rlhScene.levelBounds.Center3;

        for (int i = 0; i < numberOfBackgrounds; ++i)
        {
            //Vector3 br = backgroundsRatios[i];
            Vector3 pos = rlhScene.levelBounds.Center3;
            pos.x += backgroundsRatios[i].x * _cd.x;
            pos.y += backgroundsRatios[i].y * _cd.y;
            //Vector3 pos = backgroundsNodes[i].position;
            //pos.x = transform.position.x * backgroundsRatios[i].x;
            //pos.y = 1.0f + transform.position.y * backgroundsRatios[i].y;
            backgroundsNodes[i].position = pos;
        }

        foreach (Parallaxed parallaxed in parallaxedObjects)
        {
            if (parallaxed.enabled)
            {
                parallaxed.PUpdate(transform.position);
            }
        }

        //lastPos = transform.position;
    }

    void fitPosToSceneBounds()
    {
        Vector3 newPos = transform.position;

        if ((transform.position.x - hms.x) < rlhScene.levelBounds.SceneMin.x)
        {
            newPos.x = rlhScene.levelBounds.SceneMin.x + hms.x;
        }
        if ((transform.position.y - hms.y) < rlhScene.levelBounds.SceneMin.y)
        {
            newPos.y = rlhScene.levelBounds.SceneMin.y + hms.y;
        }

        if ((transform.position.x + hms.x) > rlhScene.levelBounds.SceneMax.x)
        {
            newPos.x = rlhScene.levelBounds.SceneMax.x - hms.x;
        }
        if ((transform.position.y + hms.y) > rlhScene.levelBounds.SceneMax.y)
        {
            newPos.y = rlhScene.levelBounds.SceneMax.y - hms.y;
        }

        //if ((transform.position.y - hms.y) < rlhScene.levelBounds.SceneMin.y)
        //{
        //    newPos.y = rlhScene.levelBounds.SceneMin.y + hms.y;
        //}

        //newPos = Vector3.Max(newPos - hms, rlhScene.levelBounds.SceneMin + hms);
        //newPos = Vector3.Min(newPos + hms, rlhScene.levelBounds.SceneMax - hms);

        transform.position = newPos;
    }

    Vector2 getTargetStage()
    {
        Vector2 targetStage = new Vector2();

        //			Vector3 targetPos = target.transform.position;
        //			if (target.isInState (Player2Controller.State.CLIMB_ROPE)) {
        //				targetPos.y -= target.cameraTargetRopeDiffY;
        //			} else {
        //				targetPos.y += target.cameraTargetNormalDiffY;
        //			}
        Transform camTarget = target.getCameraTarget();
        Vector3 targetPos = camTarget.position;

        if (target.isInState(Zap.State.CLIMB_ROPE))
        {
            targetPos.y -= camTarget.localPosition.y;
        } //else {

        targetStage.x = (targetPos.x - stagesOffset.x) / stageSize.x;
        targetStage.y = (targetPos.y - stagesOffset.y) / stageSize.y;

        //if (targetStage.x > 0)
        targetStage.x = Mathf.Floor(targetStage.x);
        targetStage.y = Mathf.Floor(targetStage.y);

        return targetStage;
    }

    Vector4 getStageMinMax(Vector2 stage)
    {
        Vector4 stageMinMax = new Vector4();

        //min
        stageMinMax.x = stagesOffset.x + stage.x * stageSize.x;
        stageMinMax.y = stagesOffset.y + stage.y * stageSize.y;

        //max
        stageMinMax.z = stageMinMax.x + stageSize.x;
        stageMinMax.w = stageMinMax.y + stageSize.y;

        return stageMinMax;
    }

    Vector4 getMinMaxPosInStage(Vector2 stage)
    {
        Vector4 stageMinMax = getStageMinMax(stage);

        float _tmp;

        stageMinMax.x += hms.x;
        stageMinMax.y += hms.y;

        stageMinMax.z -= hms.x;
        stageMinMax.w -= hms.y;

        if (stageMinMax.x > stageMinMax.z)
        {
            _tmp = stageMinMax.x;
            stageMinMax.x = stageMinMax.z;
            stageMinMax.z = _tmp;
        }

        if (stageMinMax.y > stageMinMax.w)
        {
            _tmp = stageMinMax.y;
            stageMinMax.y = stageMinMax.w;
            stageMinMax.w = _tmp;
        }

        return stageMinMax;
    }

    Vector3 fitToStage(Vector2 stage, Vector3 pos)
    {
        Vector4 minMaxPosInStage = getMinMaxPosInStage(stage);

        Vector2 stageCenter = getStageCenter(stage);

        if (hms.x * 2.0f > stageSize.x)
        {
            pos.x = stageCenter.x;
        }
        else
        {
            if (pos.x < minMaxPosInStage.x)
                pos.x = minMaxPosInStage.x;
            if (pos.x > minMaxPosInStage.z)
                pos.x = minMaxPosInStage.z;
        }

        if (hms.y * 2.0f > stageSize.y)
        {
            pos.y = stageCenter.y;
        }
        else
        {
            if (pos.y < minMaxPosInStage.y)
                pos.y = minMaxPosInStage.y;
            if (pos.y > minMaxPosInStage.w)
                pos.y = minMaxPosInStage.w;
        }

        Vector3 res = pos;

        return res;
    }

    Vector2 getStageCenter(Vector2 stage)
    {
        Vector2 stageCenter = new Vector2();
        Vector4 stageMinMax = getStageMinMax(stage);

        stageCenter.x = stageMinMax.x + stageSize.x * 0.5f;
        stageCenter.y = stageMinMax.y + stageSize.y * 0.5f;

        return stageCenter;
    }
}
//}
