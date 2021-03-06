﻿using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface
using System.Collections.Generic;

[Serializable]
public struct AnimationCallbackData
{
    public float NormTime;
    public string Message;
    public AnimationCallback.Type Type;
}

public class AnimationCallback : StateMachineBehaviour
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
    

    public enum Type
    {
        PlaySound,
        PlaySoundSurface
    }

    //public Zap zap = null;
    public IAnimationCallbackReceiver callbackTarget = null;
    //public float[] NormTimes;
    //public string[] Messages;
    //public Type[] Types;
    public AnimationCallbackData[] callbacks;
    
    float lastNormTime = 0.0f;
    bool[] msgSended;
    bool settingsOK = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastNormTime = 0.0f;

        //if (NormTimes.Length != sounds.Length)
        //	return;

        //if (sounds.Length > 0) {
        //	played = new bool[sounds.Length];
        //	for (int s = 0; s < sounds.Length; ++s) {
        //		played [s] = false;
        //	}
        //}

        //if (!audio)
        //	return;

        settingsOK = false;

        if (callbacks.Length == 0) return;
        //if (NormTimes.Length == 0) return;
        //if (NormTimes.Length != Types.Length) return;
        //if (NormTimes.Length != Messages.Length) return;

        if (callbacks.Length > 0)
        {
            msgSended = new bool[callbacks.Length];
            for (int s = 0; s < callbacks.Length; ++s)
            {
                msgSended[s] = false;
            }
        }

        settingsOK = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!settingsOK)
            return;

        float normTime = Mathf.Floor(stateInfo.normalizedTime);
        float animNormTime = stateInfo.normalizedTime - normTime;

        for (int s = 0; s < callbacks.Length; ++s)
        {
            if (msgSended[s]) continue;
            AnimationCallbackData acd = callbacks[s];
            if ( acd.NormTime <= animNormTime)
            { //gramy dzwiek
                //zap.playSound(sounds[s]);
                //zap.NewsFromAnimator(acd.Type,acd.Message);
                callbackTarget.NewsFromAnimator(acd);
                msgSended[s] = true;
            }
        }

        if (normTime != Mathf.Floor(lastNormTime))
        {
            restartAnim();
        }

        lastNormTime = stateInfo.normalizedTime;
    }

    void restartAnim()
    {
        for (int s = 0; s < msgSended.Length; ++s)
        {
            msgSended[s] = false;
        }
    }
}
