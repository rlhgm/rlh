using UnityEngine;
using System.Collections;

public class AnimationCallbackReceiver : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //public void NewsFromAnimator(AnimationCallbackData acd)
    //{
    //    switch (acd.Type)
    //    {
    //        case AnimationCallback.Type.PlaySound:
    //            //print("NewsFromAnimator : " + acd.Type + " " + acd.Message);
    //            SoundPlayer.Play(gameObject, acd.Message);
    //            break;

    //        case AnimationCallback.Type.PlaySoundSurface:
    //            //print("NewsFromAnimator : " + acd.Type + " " + acd.Message);
    //            Surface surface = groundUnder.GetComponent<Surface>();
    //            if (surface)
    //            {
    //                print(acd.Type + " " + acd.Message + "*" + surface.type);
    //                if (!SoundPlayer.Play(gameObject, acd.Message + "*" + surface.type))
    //                {
    //                    print("PROBUJE odtworzyc : " + acd.Message);
    //                    SoundPlayer.Play(gameObject, acd.Message);
    //                }
    //            }
    //            else
    //            {
    //                SoundPlayer.Play(gameObject, acd.Message);
    //            }
    //            break;
    //    }
    //}
}
