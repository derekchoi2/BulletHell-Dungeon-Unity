using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

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
		if (collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile>().owner == BasicProjectile.Owner.Player) {
			//player projectile collide with Enemy
			healthBar.ChangeHealth(collider.gameObject.GetComponent<BasicProjectile>().Damage);
			healthBar.UpdateHealthBar ();
			if (healthBar.isDead()){
				LevelController.Instance.EnemyKilled();
				EnemyController.Instance.EnemyHit(gameObject);
				//projectiles.Remove (collider.gameObject);
			}
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
