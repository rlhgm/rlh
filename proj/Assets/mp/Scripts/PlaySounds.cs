using UnityEngine;
using System.Collections;

public class PlaySounds : StateMachineBehaviour {

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

	public float[] NormTimes;
	public AudioClip[] sounds;

	float lastNormTime = 0.0f;
	bool[] played;
	bool settingsOK = false;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		lastNormTime = 0.0f;

		if (NormTimes.Length != sounds.Length)
			return;

		for (int s = 0 ; s < sounds.Length; ++s) {
			played[s] = false;
		}
	}
	
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (!settingsOK)
			return;

		float animNormTime = Mathf.Floor (stateInfo.normalizedTime);

		if( animNormTime != Mathf.Floor(lastNormTime) ){
			restartAnim();
		}

		lastNormTime = stateInfo.normalizedTime;
	}

	void restartAnim(){
		for (int s = 0 ; s < played.Length; ++s) {
			played[s] = false;
		}
	}
}
