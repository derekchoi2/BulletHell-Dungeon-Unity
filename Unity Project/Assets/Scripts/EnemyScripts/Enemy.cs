using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float movespeed = 5f;
	public bool canShoot;
	public GameObject projectile;
	public float timeBetweenShots = 1f;
	public float projectilespeed = 20f;

	protected GameController gc;
	protected float shotTimer = 0f;
	protected float nextShot;
	protected List<GameObject> projectiles;

	protected virtual void FireProjectile(){}

	protected void ProjectileTimer(){
		shotTimer = shotTimer + Time.deltaTime;

		if (shotTimer > nextShot && PlayerController.Instance != null)
		{
			nextShot = shotTimer + timeBetweenShots;
			FireProjectile ();
			nextShot = nextShot - shotTimer;
			shotTimer = 0.0F;
		}
	}

	protected virtual void Movement (){}

	protected void Update(){
		if (canShoot)
			ProjectileTimer ();

		//movement
		Movement();
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile>().owner == BasicProjectile.Owner.Player) {
			//player projectile collide with Enemy
			EnemyController.Instance.EnemyHit(gameObject);
			projectiles.Remove (collider.gameObject);
			Destroy (collider.gameObject); //destroy projectile
		}
	}

	public void ClearProjectiles(){
		foreach (GameObject projectile in projectiles) {
			Destroy (projectile);
		}
		projectiles.Clear ();
	}

}
