using UnityEngine;
using System.Collections;

public class SpriteOpacityController : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    public float[] TargetsOpacities;
    public bool[] LinearFadeOuts;

    float[] StartsOpacities;
    int numOfTargetSprites = 0;

    // Use this for initialization
    void Start()
    {
        numOfTargetSprites = sprites.Length;
        if (numOfTargetSprites == 0) return;

        if( numOfTargetSprites != TargetsOpacities.Length || numOfTargetSprites != LinearFadeOuts.Length )
        {
            Debug.LogError("SpriteOpacityController " + name + " => nie zgadadzaja sie liczby sprite'ow, opacities'ow i fadeouts'ow" );
            Debug.Break();
        }

        StartsOpacities = new float[numOfTargetSprites];
        for (int i = 0; i < numOfTargetSprites; ++i)
        {
            StartsOpacities[i] = sprites[i].color.a;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (numOfTargetSprites == 0) return;

        //Vector2 zapLocalPos = transform.InverseTransformPoint( RLHScene.Instance.Zap.transform.position );
        ////print(zapLocalPos);

        //if (Mathf.Abs(zapLocalPos.x) > 0.5f || Mathf.Abs(zapLocalPos.y) > 0.5f) return;

        ////float alphaPos
        //for(int i = 0; i < numOfTargetSprites; ++i)
        //{

        //}
    }

    void UpdateSprites()
    {
        if (numOfTargetSprites == 0) return;

        Vector2 zapLocalPos = transform.InverseTransformPoint(RLHScene.Instance.Zap.transform.position);
        //print(zapLocalPos);

        //if (Mathf.Abs(zapLocalPos.x) > 0.5f || Mathf.Abs(zapLocalPos.y) > 0.5f) return;

        float zapLocalPosX = Mathf.Min(0.5f, Mathf.Abs(zapLocalPos.x));
        zapLocalPosX *= 2f;
        float zlpxDiff = 1f - zapLocalPosX; //[0,1]
        float newOpacity = 0f;

        for (int i = 0; i < numOfTargetSprites; ++i)
        {
            if(LinearFadeOuts[i])
            {
                newOpacity = StartsOpacities[i] - zlpxDiff * (StartsOpacities[i]-TargetsOpacities[i]);
                SetOpacity(sprites[i], newOpacity);
            }
            else
            {
                SetOpacity(sprites[i], TargetsOpacities[i]);
            }
        }
    }

    void ResetSprites()
    {
        for (int i = 0; i < numOfTargetSprites; ++i)
        {
            SetOpacity(sprites[i], StartsOpacities[i]);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //print("SpriteOpacityController::OnTriggerEnter2D");
        UpdateSprites();
    }

    void OnTriggerExit2D()
    {
        //print("SpriteOpacityController::OnTriggerExit2D");
        ResetSprites();
    }

    void OnTriggerStay2D()
    {
        //print("SpriteOpacityController::OnTriggerStay2D");
        UpdateSprites();
    }

    void SetOpacity(SpriteRenderer sprite, float newOpacity)
    {
        Color oldColor = sprite.color;
        oldColor.a = newOpacity;
        sprite.color = oldColor;
    }
}
