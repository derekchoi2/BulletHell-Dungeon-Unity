﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour {

	public float movespeed = 5f;
	public GameObject projectile;
	public float timeBetweenShots = 1f;
	public float projectileSpeed = 20f;
	public HealthBar healthBar;

	protected States state;
	protected Directions direction;
	protected GameController gc;
	protected float shotTimer = 0f;
	protected float nextShot;
	protected List<GameObject> projectiles;
	protected Vector3 moveVelocity = Vector3.zero;
	protected bool changeVelocity = true; //enemies start moving when they spawn

	protected virtual void Move (){}
	protected virtual void Attack (){}
	protected virtual void Shoot () {}

	private bool colliding = false;

	protected bool ProjectileTimer(){
		shotTimer = shotTimer + Time.deltaTime;

		if (shotTimer > nextShot && PlayerController.Instance != null)
		{
			nextShot = shotTimer + timeBetweenShots;
			Shoot ();
			nextShot = nextShot - shotTimer;
			shotTimer = 0.0F;
			return true;
		}
		return false;
	}

	protected void FireProjectile(BasicProjectile.Owner owner, Vector3 shootVec){
		GameObject newProjectile = Instantiate (projectile, transform.position, Quaternion.identity);
		BasicProjectile newProjectileScript = newProjectile.GetComponent<BasicProjectile> ();
		newProjectileScript.owner = owner;
		newProjectileScript.SetVelocity (shootVec.normalized * projectileSpeed * Time.fixedDeltaTime);

		projectiles.Add (newProjectile);
	}

	protected void Update(){
		Attack ();

		//movement
		Move();

		//stay in boundary
		transform.position = new Vector3 (
			Mathf.Clamp (transform.position.x, GameController.Instance.boundary.xMin, GameController.Instance.boundary.xMax),
			0f,
			Mathf.Clamp (transform.position.z, GameController.Instance.boundary.zMin, GameController.Instance.boundary.zMax));
	}

	void LateUpdate(){
		colliding = false;
	}

	void OnTriggerEnter(Collider collider){
		if ((collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile> ().owner == BasicProjectile.Owner.Player) || (!gameObject.CompareTag ("Friendly") && collider.gameObject.CompareTag ("Player"))) {
			//player projectile or player collide with Enemy
			if (!colliding) { //prevent multiple collisions per frame
				colliding = true;
				float damage;
				if (collider.gameObject.CompareTag ("Player")) {
					damage = healthBar.health; //kill self if hit player, player takes damage equal to current health
					EnemyController.Instance.EnemyKilled (gameObject); //remove enemy from screen
					if (PlayerController.Instance.health > 0) {
						PlayerController.Instance.EnemyKilled (); //add score if player isn't dead
						LevelController.Instance.EnemyKilled ();
						PickupController.Instance.NewPickup (transform.position);
					}
				} else {
					damage = collider.gameObject.GetComponent<BasicProjectile> ().Damage;
					//if (projectiles != null)
						//projectiles.Remove (collider.gameObject); //remove from list
					Destroy (collider.gameObject); //destroy projectile
					healthBar.ChangeHealth (-damage);
					healthBar.UpdateHealthBar ();
					if (healthBar.isDead ()) {
						LevelController.Instance.EnemyKilled ();
						EnemyController.Instance.EnemyKilled (gameObject);
						PlayerController.Instance.EnemyKilled ();
						PickupController.Instance.NewPickup (transform.position);
					}
				}
			}
		}
	}

	public void ClearProjectiles(){
		if (projectiles != null) {
			foreach (GameObject projectile in projectiles)
				Destroy (projectile);
			projectiles.Clear ();
		}
	}


}
