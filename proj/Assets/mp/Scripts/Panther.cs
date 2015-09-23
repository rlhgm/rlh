using UnityEngine;
using System.Collections;

public class Panther : MonoBehaviour {

	Animator animator = null;

	void Awake(){
		animator = transform.GetComponent<Animator> ();
	}

	void Start () {
		currentActionTime = 0f;
	}
	
	void Update () {
		currentActionTime += Time.deltaTime;

		switch (action) {

		}
	}

	public enum Action{
		UNDEF = 0,
		IDLE,
		PULLOUT_KNIFE,
		HIDE_KNIFE,
		WALK_LEFT,
		WALK_RIGHT,
		WALKBACK_LEFT,
		WALKBACK_RIGHT,
		TURN_STAND_LEFT,
		TURN_STAND_RIGHT,
		ATTACK,
		ATTACK_JUST_FINISHED,
		PREPARE_TO_JUMP,
		JUMP,
		JUMP_LEFT_FRONT,
		JUMP_LEFT_BACK,
		JUMP_RIGHT_FRONT,
		JUMP_RIGHT_BACK,
		ROLL_LEFT_FRONT,
		ROLL_LEFT_BACK,
		ROLL_RIGHT_FRONT,
		ROLL_RIGHT_BACK,
		CROUCH_IN,
		GET_UP,
		CROUCH_IDLE,
		CROUCH_LEFT,
		CROUCH_RIGHT,
		CROUCH_LEFT_BACK,
		CROUCH_RIGHT_BACK,
		CROUCH_ATTACK,
		FALL,
		STOP_WALK,
		STOP_RUN,
		DIE
	};

	Action action;
	float currentActionTime;

	bool setAction(Action newAction, int param = 0){
		
		if (action == newAction)
			return false;
		
		action = newAction;
		zap.resetCurrentActionTime ();
		zap.getAnimator().speed = 1f;
		
		switch (newAction) {
			
		case Action.IDLE:
			//if( zap.faceRight() ) zap.getAnimator().Play("Zap_idle_R");
			//else zap.getAnimator().Play ("Zap_idle_L");
			
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_knife_idle");
			else zap.getAnimator().Play ("Zap_knife_idle");
			
			break;
			
		case Action.PULLOUT_KNIFE:
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_knife_pull");
			else zap.getAnimator().Play ("Zap_knife_pull");
			break;
			
		case Action.HIDE_KNIFE:
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_knife_hide");
			else zap.getAnimator().Play ("Zap_knife_hide");
			break;
			
		case Action.ATTACK:
			string animName = "Zap_knife_attack_0";
			if( param == 0 ){
				if( Random.Range(0,2) == 1 )
					animName = "Zap_knife_attack_1";
			}
			//Debug.Log( animName );
			zap.getAnimator().Play(animName,-1,0f);
			
			Vector2 cutStart;
			Vector2 cutEnd;
			
			if( zap.faceRight() ){
				cutStart = zap.rightKnifeHitPointHigh1.position;
				cutEnd = zap.rightKnifeHitPointHigh2.position;
			}else{
				cutStart = zap.leftKnifeHitPointHigh1.position;
				cutEnd = zap.leftKnifeHitPointHigh2.position;
			}
			cut (cutStart,cutEnd);
			
			break;
			
		case Action.DIE:
			Zap.DeathType dt = (Zap.DeathType)param;
			string msgInfo = "";
			
			switch( dt ){
				
			case Zap.DeathType.STONE_HIT:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_stonehit_R");
				else zap.getAnimator().Play("Zap_death_stonehit_L");
				msgInfo = zap.DeathByStoneHitText;
				break;
				
			case Zap.DeathType.VERY_HARD_LANDING:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_hitground_R");
				else zap.getAnimator().Play("Zap_death_hitground_L");
				msgInfo = zap.DeathByVeryHardLandingText;
				break;
				
			case Zap.DeathType.SNAKE:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_poison_R");
				else zap.getAnimator().Play("Zap_death_poison_L");
				msgInfo = zap.DeathBySnakeText;
				break;
				
			case Zap.DeathType.POISON:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_poison_R");
				else zap.getAnimator().Play("Zap_death_poison_L");
				msgInfo = zap.DeathByPoisonText;
				break;
				
			case Zap.DeathType.CROCODILE:
				msgInfo = zap.DeathByCrocodileText;
				break;
				
			default:
				if( zap.faceRight() ) zap.getAnimator().Play("Zap_death_hitground_R");
				else zap.getAnimator().Play("Zap_death_hitground_L");
				msgInfo = zap.DeathByDefaultText;
				break;
				
			};
			
			zap.showInfo (msgInfo, -1);
			
			if( zap.dieSounds.Length != 0 )
				zap.getAudioSource().PlayOneShot(zap.dieSounds[Random.Range(0,zap.dieSounds.Length)], 0.3F);
			break;
			
		case Action.WALK_LEFT:
			zap.getAnimator().Play("Zap_knife_walk");
			break;
		case Action.WALK_RIGHT:
			zap.getAnimator().Play("Zap_knife_walk");
			break;
			
		case Action.WALKBACK_LEFT:
			zap.getAnimator().Play("Zap_knife_walkback");
			break;
		case Action.WALKBACK_RIGHT:
			zap.getAnimator().Play("Zap_knife_walkback");
			break;
			
		case Action.TURN_STAND_LEFT:
			zap.getAnimator().Play("Zap_knife_turnleft");
			wantJumpAfter = false;
			break;
			
		case Action.TURN_STAND_RIGHT:
			zap.getAnimator().Play("Zap_knife_turnright");
			wantJumpAfter = false;
			break;
			
		case Action.PREPARE_TO_JUMP:
			if( zap.faceRight() ) zap.getAnimator().Play("Zap_jump_in_R");
			else zap.getAnimator().Play("Zap_jump_in_L");
			break;
			
			//		case Action.JUMP:
			//			if( param == 0 ){
			//				
			//				if( zap.faceRight() ) zap.getAnimator().Play("Zap_jump_fly_R");
			//				else zap.getAnimator().Play("Zap_jump_fly_L");
			//				
			//			}else if (param == 1) {
			//				if( zap.faceRight() ) zap.getAnimator().Play("zap_rocks_climb_R");
			//				else zap.getAnimator().Play("zap_rocks_climb_L");
			//			}
			//			if( zap.jumpSounds.Length != 0 )
			//				zap.getAudioSource().PlayOneShot(zap.jumpSounds[Random.Range(0,zap.jumpSounds.Length)], 0.2F);
			//			break;
			
		case Action.JUMP_LEFT_FRONT:
			zap.getAnimator().Play("Zap_knife_jumpfront");
			break;
			
		case Action.JUMP_LEFT_BACK:
			zap.getAnimator().Play("Zap_knife_jumpback");
			break;
			
		case Action.JUMP_RIGHT_FRONT:
			zap.getAnimator().Play("Zap_knife_jumpfront");
			break;
			
		case Action.JUMP_RIGHT_BACK:
			zap.getAnimator().Play("Zap_knife_jumpback");
			break;
			
		case Action.ROLL_LEFT_FRONT:
			zap.getAnimator().Play("Zap_knife_crouch_tumblefront");
			break;
			
		case Action.ROLL_LEFT_BACK:
			zap.getAnimator().Play("Zap_knife_crouch_tumbleback");
			break;
			
		case Action.ROLL_RIGHT_FRONT:
			zap.getAnimator().Play("Zap_knife_crouch_tumblefront");
			break;
			
		case Action.ROLL_RIGHT_BACK:
			zap.getAnimator().Play("Zap_knife_crouch_tumbleback");
			break;
			
			//		case Action.JUMP_LEFT:
			//		case Action.JUMP_RIGHT:
			//
			//			if( zap.faceRight() ) zap.getAnimator().Play("Zap_run_jump_fly_R");
			//			else zap.getAnimator().Play("Zap_run_jump_fly_L");
			//			
			//			if( zap.jumpSounds.Length != 0 )
			//				zap.getAudioSource().PlayOneShot(zap.jumpSounds[Random.Range(0,zap.jumpSounds.Length)], 0.2F);
			//			break;
			
		case Action.CROUCH_IN:
			//if( zap.faceRight() ) zap.getAnimator().Play("Zap_crouch_in_R");
			//else zap.getAnimator().Play("Zap_crouch_in_L");
			zap.getAnimator().Play("Zap_knife_crouch_in");
			break;
			
		case Action.GET_UP:
			//if( zap.faceRight() ) zap.getAnimator().Play("Zap_getup_R");
			//else zap.getAnimator().Play("Zap_getup_L");
			zap.getAnimator().Play("Zap_knife_get_up");
			break;
			
		case Action.CROUCH_IDLE:
			if( zap.faceRight () ) zap.getAnimator().Play("Zap_knife_crouch_idle");
			else zap.getAnimator().Play("Zap_knife_crouch_idle");
			//zap.getAnimator().speed = 0f;
			break;
			
		case Action.CROUCH_ATTACK:
			zap.getAnimator().Play("Zap_knife_crouch_attack",-1,0f);
			
			Vector2 cutStartLow;
			Vector2 cutEndLow;
			
			if( zap.faceRight() ){
				cutStartLow = zap.rightKnifeHitPointLow1.position;
				cutEndLow = zap.rightKnifeHitPointLow2.position;
			}else{
				cutStartLow = zap.leftKnifeHitPointLow1.position;
				cutEndLow = zap.leftKnifeHitPointLow2.position;
			}
			cut (cutStartLow,cutEndLow);
			
			break;
			
		case Action.CROUCH_LEFT:
			zap.getAnimator().Play("Zap_knife_crouch_walk");
			break;
		case Action.CROUCH_RIGHT:
			zap.getAnimator().Play("Zap_knife_crouch_walk");
			break;
			
		case Action.CROUCH_LEFT_BACK:
			zap.getAnimator().Play("Zap_knife_crouch_walkback");
			break;
			
		case Action.CROUCH_RIGHT_BACK:
			zap.getAnimator().Play("Zap_knife_crouch_walkback");
			break;
		};
		
		return true;
	}
	bool isInAction(Action test) {
		return action == test;
	}
	bool isNotInAction(Action test){
		return action != test;
	}
}
