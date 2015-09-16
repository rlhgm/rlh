using UnityEngine;
using System.Collections;

public class zap_idle1_beh : StateMachineBehaviour {

	public Zap playerController = null;
	//public AudioClip[] dieSounds;
	public int stateIdleNum = 0;

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

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
//	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//		if (playerController) {
//			playerController.StateIdleExit();
//		}
//	}

	float lastNormTime = 0.0f;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		lastNormTime = 0.0f;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (playerController) {
			//playerController.StateIdleExit();
			//if( stateInfo.normalizedTime >= 1.0f )
			if( Mathf.Floor( stateInfo.normalizedTime ) != Mathf.Floor(lastNormTime) ){
				playerController.StateIdleFinish(stateIdleNum);
				//playerController.StateIdleUpdate(stateInfo.normalizedTime);
			}
			lastNormTime = stateInfo.normalizedTime;
		}
	}
}
