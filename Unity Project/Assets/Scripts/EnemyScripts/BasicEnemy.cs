using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy {

	public float directionChangeTime = 2f;
	public float stationaryTime = 1f;

	private Vector3 moveVelocity = Vector3.zero;
	private bool changeVelocity = false;

	// Use this for initialization
	void Start () {
		projectiles = new List<GameObject> ();
		nextShot = timeBetweenShots;
		gc = GameController.Instance;
		StartCoroutine (UpdateVelocityTimer ());
		transform.position = GameController.Instance.RandomPosition();
	}

	protected override void FireProjectile(){
		GameObject newProjectile = Instantiate (projectile, transform.position, transform.rotation);
		BasicProjectile newProjectileScript = newProjectile.GetComponent<BasicProjectile> ();
		newProjectileScript.owner = BasicProjectile.Owner.Enemy;
		Vector3 dir = PlayerController.Instance.transform.position - transform.position;
		newProjectileScript.SetVelocity (dir.normalized * projectilespeed * Time.deltaTime);

		projectiles.Add (newProjectile);
	}

	protected override void Movement(){
		if (PlayerController.Instance != null) {
			//movement
			if (changeVelocity) {
				if (moveVelocity == Vector3.zero)
					moveVelocity = (PlayerController.Instance.transform.position - transform.position).normalized * movespeed * Time.deltaTime;
				else
					moveVelocity = Vector3.zero;
			
				changeVelocity = false;
				StartCoroutine (UpdateVelocityTimer ());
			}
			transform.position += moveVelocity;
		}
	}

	IEnumerator UpdateVelocityTimer(){
		if (moveVelocity == Vector3.zero)
			yield return new WaitForSeconds (stationaryTime);
		else
			yield return new WaitForSeconds (directionChangeTime);
		changeVelocity = true;
	}

}
