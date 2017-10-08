using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : Weapon {

	public GameObject projectile;

	public override void Shoot(Vector3 shootVec){
		if (player == null)
			player = PlayerController.Instance;

		shotTimer = shotTimer + Time.deltaTime;

		if (shotTimer > nextShot) {
			if (player.state == States.Attack)
				player.ChangeState (States.Idle, Vector3.zero);

			if (shootVec.sqrMagnitude != 0) {
				nextShot = shotTimer + timeBetweenShots;
				FireProjectile (player.transform.position, shootVec, projectile);
				nextShot = nextShot - shotTimer;
				shotTimer = 0.0F;
			}
		}
	}
}
