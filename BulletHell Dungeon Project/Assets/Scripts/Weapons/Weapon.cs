using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

	public WeaponType Type;
	public float ProjectileSpeed = 40f;
	public float timeBetweenShots = 0.5f;

	protected List<GameObject> projectiles = new List<GameObject>();

	protected SpriteAnimator animator;
	protected float nextShot;
	protected float shotTimer = 0f;
	protected float savedTimeBetweenShots;

	protected PlayerController player;

	void Awake(){
		nextShot = timeBetweenShots;
		savedTimeBetweenShots = timeBetweenShots;
		animator = GetComponent<SpriteAnimator> ();
	}

	public abstract void Shoot (Vector3 shootVec);

	protected void GetPlayer(){
		player = PlayerController.Instance;
	}

	protected void FireProjectile(Vector3 shootVec, GameObject projectile){
		//spawn under player sprite
		Vector3 spawnVec = transform.position;
		spawnVec.y -= 1;

		GameObject projectileInstance = Instantiate (projectile, spawnVec, transform.rotation);
		BasicProjectile projectileScript = projectileInstance.GetComponent<BasicProjectile> ();
		projectileScript.owner = BasicProjectile.Owner.Player;

		player.ChangeState(States.Attack, shootVec);
		projectileScript.SetVelocity (shootVec * ProjectileSpeed * Time.fixedDeltaTime);

		projectiles.Add (projectileInstance);
	}

	public void ChangeState(States state, Directions playerDir, Directions dir){
		if (player == null)
			GetPlayer ();
		
		if (player.MoveShootSame (playerDir, dir) || state == States.Idle)
			animator.ChangeState (state, dir);
	}

	public void Hide(){
		GetComponent<SpriteRenderer> ().enabled = false;
	}

	public void Show(){
		GetComponent<SpriteRenderer> ().enabled = true;
	}

	public void Reset(){
		DestroyProjectiles ();
		timeBetweenShots = savedTimeBetweenShots;
	}

	void OnDestroy(){
		DestroyProjectiles ();
	}

	void DestroyProjectiles(){
		foreach (GameObject projectile in projectiles) {
			Destroy (projectile);
		}
		projectiles.Clear ();
	}

}
