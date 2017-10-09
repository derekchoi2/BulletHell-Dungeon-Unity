using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : State {

	private SpriteAnimator animator;

	public Move(){
		state = States.Move;
	}

	public override void Enter(Directions dir){
		if (player == null)
			player = PlayerController.Instance;
		
		animator = player.GetComponentInChildren<SpriteAnimator> ();
		animator.ChangeState (States.Idle, direction);

		direction = dir;
		weaponDir = dir;

		animator.ChangeState (States.Move, direction);
	}

	public override void Exit(){

	}

	public override void Execute(Directions newDirection){
		
		if (player.CurrentWeaponScript != null && newDirection != weaponDir) {
			weaponDir = newDirection;
			player.CurrentWeaponScript.ChangeState (States.Move, weaponDir);
		}

		//smooth out diagonal movement animations
		if (newDirection == Directions.NE || newDirection == Directions.SE)
			newDirection = Directions.E;
		if (newDirection == Directions.NW || newDirection == Directions.SW)
			newDirection = Directions.W;

		if (direction != newDirection) {
			direction = newDirection;
			animator.ChangeState (States.Move, direction);
		}
	}

}
