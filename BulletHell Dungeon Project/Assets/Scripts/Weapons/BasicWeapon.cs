using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : Weapon {

	public GameObject projectile;

	public override void Shoot(Vector3 shootVec){
		if (player == null)
			GetPlayer ();

		shotTimer = shotTimer + Time.deltaTime;

		if (shotTimer > nextShot) {
			if (player.state == States.Attack)
				player.ChangeState (States.Idle, player.direction);

			if (shootVec.sqrMagnitude != 0) {
				nextShot = shotTimer + timeBetweenShots;
				FireProjectile (shootVec, projectile);
				nextShot = nextShot - shotTimer;
				shotTimer = 0.0F;
			}
		}
	}
}
