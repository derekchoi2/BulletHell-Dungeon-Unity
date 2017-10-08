using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

	public WeaponType Type;
	public float ProjectileSpeed = 40f;
	public float timeBetweenShots = 0.5f;

	protected List<GameObject> projectiles = new List<GameObject>();

	protected float Direction;
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

	protected void FireProjectile(Vector3 Origin, Vector3 shootVec, GameObject projectile){
		GameObject projectileInstance = Instantiate (projectile, transform.position, transform.rotation);
		BasicProjectile projectileScript = projectileInstance.GetComponent<BasicProjectile> ();
		projectileScript.owner = BasicProjectile.Owner.Player;

		Vector3 dir = shootVec.normalized;

		player.ChangeState(States.Attack, dir);
		projectileScript.SetVelocity (dir * ProjectileSpeed * Time.fixedDeltaTime);

		projectiles.Add (projectileInstance);
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
