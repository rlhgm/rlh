using UnityEngine;
using System;
using System.Collections.Generic;

public interface IAnimationCallbackReceiver
{
    void SetAnimatorCallbackTarget();
    void NewsFromAnimator(AnimationCallbackData acd);
    //void GCacheResetData();
    //void GReset();
}
