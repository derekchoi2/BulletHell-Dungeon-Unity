using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

	public float movespeed = 5f;
	public GameObject projectile;
	public float timeBetweenShots = 1f;
	public float projectileSpeed = 20f;
	public bool canshoot;
	public HealthBar healthBar;

	protected States state;
	protected Directions direction;
	protected GameController gc;
	protected float shotTimer = 0f;
	protected float nextShot;
	protected List<GameObject> projectiles;
	protected Vector3 moveVelocity = Vector3.zero;
	protected bool changeVelocity = true; //enemies start moving when they spawn

	protected virtual void FireProjectile(){}
	protected virtual void Move (){}
	protected virtual void Attack(){}
	protected virtual void Shoot(){}

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

	protected void Update(){
		if (canshoot)
			Shoot ();
		else
			Attack ();

		//movement
		Move();

		//stay in boundary
		transform.position = new Vector3 (
			Mathf.Clamp (transform.position.x, GameController.Instance.boundary.xMin, GameController.Instance.boundary.xMax),
			0f,
			Mathf.Clamp (transform.position.z, GameController.Instance.boundary.zMin, GameController.Instance.boundary.zMax));
	}

	void OnTriggerEnter(Collider collider){
		if ((collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile> ().owner == BasicProjectile.Owner.Player) || (!gameObject.CompareTag("Friendly") && collider.gameObject.CompareTag("Player"))) {
			//player projectile or player collide with Enemy
			int damage;
			if (collider.gameObject.CompareTag ("Player")) {
				damage = healthBar.health; //kill self if hit player, player takes damage equal to current health
				EnemyController.Instance.EnemyKilled (gameObject); //remove enemy from screen
				if (PlayerController.Instance.health > 0)
					PlayerController.Instance.EnemyKilled (); //add score if player isn't dead
			} else {
				damage = collider.gameObject.GetComponent<BasicProjectile> ().Damage;
				projectiles.Remove (collider.gameObject); //remove from list
				Destroy (collider.gameObject); //destroy projectile
				healthBar.ChangeHealth (-damage);
				healthBar.UpdateHealthBar ();
				if (healthBar.isDead ()) {
					LevelController.Instance.EnemyKilled ();
					EnemyController.Instance.EnemyKilled (gameObject);
					PlayerController.Instance.EnemyKilled ();
				}
			}

		}
	}

	public void ClearProjectiles(){
		foreach (GameObject projectile in projectiles) {
			Destroy (projectile);
		}
		projectiles.Clear ();
	}


}
