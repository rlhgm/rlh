using System;
using UnityEngine;

//[ExecuteInEditMode]
public class Camera2DFollow : MonoBehaviour
{
    Zap target;

    public Transform[] backgroundsNodes;
    public Vector2[] backgroundsRatios;

    public GameObject[] backgroundsBackgrounds;

    public Vector2 stageSize = new Vector2(20f, 10f);
    public Vector2 stagesOffset = new Vector2(0f, 0f);
    // jak duzo jednosek pokazuje na ekranie
    Vector2 hms = new Vector2();

    public Vector2 targetStage = new Vector2();

    public Vector2 backgroundLimits = new Vector2(-30, 30);

    public Camera myCamera;

    [HideInInspector]
    public CutSceneCameraPassing cutSceneController = null;
    CutSceneCameraPassingControlPoint currentCutSceneControlPoint = null;

    Parallaxed[] parallaxedObjects;

    RLHScene rlhScene = null;
    bool onOff = true;

    public void StartCutScene(CutSceneCameraPassing csc)
    {
        cutSceneController = csc;
        currentCutSceneControlPoint = cutSceneController.next;
    }
    public void StopCutScene()
    {
        cutSceneController = null;
        currentCutSceneControlPoint = null;
    }
    bool UpdateCutScene()
    {
        if (!cutSceneController) return true;

        return false;
    } 

    public void StartFollow()
    {
        onOff = true;
    }

    public void StopFollow()
    {
        onOff = false;
    }
    
    void Awake()
    {
    }

    // Use this for initialization
    private void Start()
    {
        parallaxedObjects = FindObjectsOfType(typeof(Parallaxed)) as Parallaxed[];

        Zap[] zappers = FindObjectsOfType(typeof(Zap)) as Zap[];
        if (zappers.Length == 1)
        {
            target = zappers[0];
        }

        myCamera = GetComponent<Camera>();

        Transform tct = myCamera.transform.Find("TouchCamera");
        if (tct != null)
        {
            Camera tc = tct.GetComponent<Camera>();
            target.setTouchCamera(tc);
        }
        else
        {
            Debug.LogError("Camera2DFollow => nie ma TouchCamer'y");
            Debug.Break();
        }

        hms.x = myCamera.orthographicSize * GetComponent<Camera>().aspect;
        hms.y = myCamera.orthographicSize;

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        transform.parent = null;

        rlhScene = target.RlhScene;

        for (int i = 0; i < backgroundsNodes.Length; ++i)
        {
            if (backgroundsNodes[i] == null) continue;
            if (backgroundsBackgrounds[i] == null) continue;

            backgroundsNodes[i].position = rlhScene.transform.InverseTransformPoint(rlhScene.levelBounds.Center3);

            GameObject bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
            bckg.transform.parent = backgroundsNodes[i];
            bckg.transform.localPosition = (new Vector3(0f, 1f, 0f));


            SpriteRenderer bckgSpriteRend = bckg.GetComponent<SpriteRenderer>();

            Vector3 bckgItemSize = bckgSpriteRend.bounds.extents;

            float bckgDist = -bckgItemSize.x * 2;
            while (bckgDist > -rlhScene.levelBounds.SceneSize.x * 0.5f - (bckgItemSize.x * 2))
            {
                bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
                bckg.transform.parent = backgroundsNodes[i];
                bckg.transform.localPosition = (new Vector3(bckgDist, 1f, 0f));
                bckgDist -= bckgItemSize.x * 2;
            }

            bckgDist = bckgItemSize.x * 2;
            while (bckgDist < rlhScene.levelBounds.SceneSize.x * 0.5f + (bckgItemSize.x * 2))
            {
                //c = 1;
                bckg = Instantiate<GameObject>(backgroundsBackgrounds[i]);
                bckg.transform.parent = backgroundsNodes[i];
                bckg.transform.localPosition = (new Vector3(bckgDist, 1f, 0f));
                bckgDist += bckgItemSize.x * 2;
            }
        }

        RLHScene.RLHAssert(backgroundsNodes.Length == backgroundsRatios.Length, "numberOfBackgrounds != backgroundsRatios.GetLength ()");

    }
    public enum ShakeStatus
    {
        NoShake,
        ShakeImpulse,
        ShakePermanent,
        ShakeFadeIn,
        ShakeFadeOut
    }

    //bool shakingImpulse = false;
    //bool shakingPermanent = false;
    ShakeStatus shakeStatus = ShakeStatus.NoShake;

    float shakeDuration = 0f;
    float shakeTime = 0f;
    float shakeRatio = 0f;
    float shakeFadeDuration = 0f;
    float shakeFadeTime = 0f;

    Vector2 shakeMaxAmplitude = new Vector2(0.25f, 0.25f);
    Vector2 shakeAmplitude = new Vector2(0f, 0f);

    Vector2 shakeMaxSpeed = new Vector2(5f, 5f);
    Vector2 shakeSpeed = new Vector2(5f, 5f);

    float sample = 0f;

    public Vector2 ShakeMaxAmplitude
    {
        get
        {
            return shakeMaxAmplitude;
        }

        set
        {
            shakeMaxAmplitude = value;
        }
    }
    
    public bool IsShaking()
    {
        return shakeStatus != ShakeStatus.NoShake;
    }

    public ShakeStatus GetShakeStatus()
    {
        return shakeStatus;
    }

    public void ShakeImpulseStart(float duration, float amplitude, float speed)
    {
        //shakingImpulse = true;
        //shakingPermanent = false;
        shakeStatus = ShakeStatus.ShakeImpulse;

        shakeDuration = duration;
        shakeTime = 0f;
        shakeMaxAmplitude.x = amplitude;
        shakeMaxAmplitude.y = amplitude;
        shakeAmplitude = shakeMaxAmplitude;

        shakeMaxSpeed.x = speed;
        shakeMaxSpeed.y = speed;
        shakeSpeed = shakeMaxSpeed;
    }    
    public void ShakePermanentStart(float amplitude, float speed)
    {
        //shakingPermanent = true;
        //shakingImpulse = false;
        shakeStatus = ShakeStatus.ShakePermanent;

        //shakeDuration = duration;
        shakeTime = 0f;
        shakeMaxAmplitude.x = amplitude;
        shakeMaxAmplitude.y = amplitude;
        shakeAmplitude = shakeMaxAmplitude;

        shakeMaxSpeed.x = speed;
        shakeMaxSpeed.y = speed;
        shakeSpeed = shakeMaxSpeed;
    }
    public void ShakeStop(float fadeTime)
    {
        //shakingImpulse = false;
        if (fadeTime <= 0f)
        {
            //shakingPermanent = false;
            shakeStatus = ShakeStatus.NoShake;
        }
        else
        {
            //shakeStatus = ShakeStatus.ShakeFadeOut;

            shakeStatus = ShakeStatus.ShakeImpulse;
            shakeDuration = shakeTime+fadeTime;

            //shakeTime = 0f;
            //shakeMaxAmplitude.x = amplitude;
            //shakeMaxAmplitude.y = amplitude;
            //shakeAmplitude = shakeMaxAmplitude;

            //shakeMaxSpeed.x = speed;
            //shakeMaxSpeed.y = speed;
            //shakeSpeed = shakeMaxSpeed;
        }
    }
    void applyShake(float dt)
    {
        shakeTime += dt;
        Vector3 newPos = transform.position;

        switch ( shakeStatus )
        {
            case ShakeStatus.NoShake:
                return;

            case ShakeStatus.ShakeImpulse:
                if (shakeTime > shakeDuration)
                {
                    shakeStatus = ShakeStatus.NoShake;
                    return;
                }
                shakeRatio = 1 - (shakeTime / shakeDuration);

                sample = (Mathf.PerlinNoise(shakeTime * shakeSpeed.x, 0f) - 0.5f) * shakeMaxAmplitude.x;
                newPos.x += (sample * shakeRatio);
                sample = (Mathf.PerlinNoise(0f, shakeTime * shakeSpeed.y) - 0.5f) * shakeMaxAmplitude.y;
                newPos.y += (sample * shakeRatio);
                
                break;

            case ShakeStatus.ShakePermanent:
                newPos.x += (Mathf.PerlinNoise(shakeTime * shakeSpeed.x, 0f) - 0.5f) * shakeMaxAmplitude.x;
                newPos.y += (Mathf.PerlinNoise(0f, shakeTime * shakeSpeed.y) - 0.5f) * shakeMaxAmplitude.y;
                break;

            case ShakeStatus.ShakeFadeIn:
                break;

            case ShakeStatus.ShakeFadeOut:
                break;
        }

        transform.position = newPos;

        // fajny efekt 0.125 - 0.5 
        //shakeMaxAmplitude = new Vector2(0.25f, 0.25f);
        // fajny efekt nawet 8-10
        //shakeSpeed = new Vector2(8f, 8f);


    }

    // Update is called once per frame
    private void Update()
    {
        if( cutSceneController )
        {
            if( UpdateCutScene() )
            {
                StopCutScene();
            }
        }
        if(!onOff)
        {
            applyShake(Time.deltaTime);
            fitPosToSceneBounds();
            return;
        }

        targetStage = getTargetStage();

        Transform camTarget = target.getCameraTarget();

        Vector3 newPos = camTarget.position + target.CameraTargetOffset; //new Vector3(camTarget.position.x, camTarget.position.y, transform.position.z);

        if (target.isInState(Zap.State.CLIMB_ROPE))
        {
            newPos.y -= camTarget.localPosition.y;
        } 

        Vector3 res = fitToStage(targetStage, newPos);
        Vector3 posDiff = res - transform.position;

        //if (target.isInState(Zap.State.CLIMB_ROPE))
        //{
        //    transform.position = Vector3.LerpUnclamped(transform.position, transform.position + posDiff, 0.1f);
        //}
        //else
        //{
        //    //transform.position = transform.position + posDiff;
        //    transform.position = Vector3.LerpUnclamped(transform.position, transform.position + posDiff, 0.05f);
        //}

        float _tt = 0.1f;
        switch(target.getState())
        {
            case Zap.State.CLIMB_ROPE:
                _tt = 0.1f;
                break;
            case Zap.State.IN_AIR:
                _tt = 0.25f;
                break;
            default:
                _tt = 0.1f;
                break;

        }
        transform.position = Vector3.LerpUnclamped(transform.position, transform.position + posDiff, _tt);

        applyShake(Time.deltaTime);
        
        fitPosToSceneBounds();

        
        int numberOfBackgrounds = backgroundsNodes.Length;
        Vector3 _cd = transform.position - rlhScene.levelBounds.Center3;
        for (int i = 0; i < numberOfBackgrounds; ++i)
        {
            Vector3 pos = rlhScene.levelBounds.Center3;
            pos.x += backgroundsRatios[i].x * _cd.x;
            pos.y += backgroundsRatios[i].y * _cd.y;
            backgroundsNodes[i].position = pos;
        }

        foreach (Parallaxed parallaxed in parallaxedObjects)
        {
            if (parallaxed.enabled)
            {
                parallaxed.PUpdate(transform.position);
            }
        }
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
        
        transform.position = newPos;
    }

    Vector2 getTargetStage()
    {
        Vector2 targetStage = new Vector2();
        
        Transform camTarget = target.getCameraTarget();
        Vector3 targetPos = camTarget.position;

        if (target.isInState(Zap.State.CLIMB_ROPE))
        {
            targetPos.y -= camTarget.localPosition.y;
        }

        targetStage.x = (targetPos.x - stagesOffset.x) / stageSize.x;
        targetStage.y = (targetPos.y - stagesOffset.y) / stageSize.y;

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
