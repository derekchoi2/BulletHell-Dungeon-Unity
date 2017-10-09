using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State {

	private SpriteAnimator animator;

	public Idle(){
		state = States.Idle;
		direction = Directions.Unspecified;
	}

	public override void Enter(Directions dir){
		if (player == null)
			player = PlayerController.Instance;
		
		animator = player.GetComponentInChildren<SpriteAnimator> ();
		animator.ChangeState (States.Idle, direction);
	}

	public override void Exit(){

	}

	public override void Execute(Directions newDirection){
		animator.ChangeState (States.Idle, direction);
		if (player.CurrentWeaponScript != null)
			player.CurrentWeaponScript.ChangeState (States.Idle, direction);
	}

}
