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

	protected void FireProjectile(){
		BasicProjectile newProjectile = Instantiate (projectile, transform.position, transform.rotation).GetComponent<BasicProjectile>();
		newProjectile.owner = BasicProjectile.Owner.Enemy;
		Vector3 dir = PlayerController.Instance.transform.position - transform.position;
		newProjectile.SetVelocity (dir.normalized * projectilespeed * Time.deltaTime);
	}

	protected void ProjectileTimer(){
		shotTimer = shotTimer + Time.deltaTime;

		if (shotTimer > nextShot)
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
			gc.EnemyHit(gameObject);
			Destroy (collider.gameObject); //destroy projectile
		}
	}

}
