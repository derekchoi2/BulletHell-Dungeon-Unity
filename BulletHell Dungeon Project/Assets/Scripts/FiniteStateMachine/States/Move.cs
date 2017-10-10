﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : State {

	public Move(){
		state = States.Move;
	}

	public override void Enter(Directions moveDir, Directions weaponDir){
		if (player == null)
			player = PlayerController.Instance;
		
		animator = player.GetComponentInChildren<SpriteAnimator> ();
	}

	public override void Exit(){

	}

	public override void Execute(Directions moveDir, Directions weaponDir){
		//move state ignores weaponDir

		if (player.CurrentWeaponScript != null)
			player.CurrentWeaponScript.ChangeState (States.Move, moveDir);

		//smooth out diagonal movement animations
		if (moveDir == Directions.NE || moveDir == Directions.SE)
			moveDir = Directions.E;
		if (moveDir == Directions.NW || moveDir == Directions.SW)
			moveDir = Directions.W;

		animator.ChangeState (States.Move, moveDir);
	}

}
