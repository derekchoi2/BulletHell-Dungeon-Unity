using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAttack : State {

	public MoveAttack(){
		state = States.MoveAttack;
	}

	public override void Enter(Directions moveDir, Directions weaponDir){
		if (player == null)
			player = PlayerController.Instance;

		animator = player.GetComponentInChildren<SpriteAnimator> ();
	}

	public override void Exit(){

	}

	public override void Execute(Directions moveDir, Directions weaponDir){
		
		//smooth out diagonal movement animations
		if (moveDir == Directions.NE || moveDir == Directions.SE)
			moveDir = Directions.E;
		if (moveDir == Directions.NW || moveDir == Directions.SW)
			moveDir = Directions.W;

		if (player.CurrentWeaponScript != null && moveDir != Directions.Unspecified && weaponDir != Directions.Unspecified) {

			if (MoveShootOpposite (moveDir, weaponDir)) {
				//move back animation
				animator.ChangeState (state, moveDir);
			} else {
				animator.ChangeState (States.Move, weaponDir); //character faces the direction the gun is aimed
			}
				
			player.CurrentWeaponScript.ChangeState (States.Attack, weaponDir);
		}
	}

	bool MoveShootOpposite(Directions moveDir, Directions weaponDir){
		//determine when to change to move backwards animation
		//Debug.Log ("Move: " + moveDirection.ToString ());
		//Debug.Log ("Weapon: " + weaponDirection.ToString ());
		if (moveDir == Directions.W && (weaponDir == Directions.NE || weaponDir == Directions.E || weaponDir == Directions.SE))
			return true;
		if (moveDir == Directions.E && (weaponDir == Directions.NW || weaponDir == Directions.W || weaponDir == Directions.SW))
			return true;
		return false;
	}

}
