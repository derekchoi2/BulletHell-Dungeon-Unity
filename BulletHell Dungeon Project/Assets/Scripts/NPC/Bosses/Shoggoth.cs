	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoggoth : NPC {

	public int shotLimit = 2, dashLimit = 3;

	public float spread = 30f;
	public int NumOfProjectiles = 12;

	public float velocityChangeTime = 1f;


	public float timeBetweenDashes = 0.5f;
	public float dashSpeedMultiplier = 10f;
	private float dashTimer = 0;
	private float nextDash;

	private int attackPhase = 0;
	private int shots = 0;
	private int dashes = 0;

	private SpriteAnimator animator;

	// Use this for initialization
	void Start () {
		state = States.Idle;
		animator = gameObject.GetComponentInChildren<SpriteAnimator> ();
		projectiles = new List<GameObject> ();
		nextShot = timeBetweenShots;
		gc = GameController.Instance;
		transform.position = GameController.Instance.RandomPosition();
	}

	protected override void Shoot(){
		animator.ChangeState (States.Idle, direction); //Idle animation is same as attack animation
		Vector3 shootVec = PlayerController.Instance.transform.position - transform.position;
		for (int i = 0; i < NumOfProjectiles; i++) {
			FireProjectile (BasicProjectile.Owner.Enemy, Quaternion.Euler (0, i * spread, 0) * shootVec);
		}

	}

	protected override void Move(){
		if (PlayerController.Instance != null) {
			//movement
			if (changeVelocity && attackPhase % 2 == 0) { //attack phase not dashing, so move won't overwrite dash
				Vector3 dir = (PlayerController.Instance.transform.position - transform.position).normalized;
				state = States.Move;
				CalculateDirection (dir);
				//perform movement
				moveVelocity = dir * movespeed * Time.deltaTime;

				animator.ChangeState (state, direction);

				StopAllCoroutines ();
				StartCoroutine (VelocityChangeTimer ());

				changeVelocity = false;
			}
			transform.position += moveVelocity;
		}
	}

	protected override void Attack(){
		//attack pattern? dashx3 -> shootx2 -> dashx3 -> shootx2

		if (attackPhase % 2 == 0) { //even attackPhase
			ProjectileTimer ();
			shots++;
			if (shots > shotLimit) {
				shots = 0;
				attackPhase++;
			}
		} else {
			DashTimer ();
			dashes++;
			if (dashes > dashLimit) {
				dashes = 0;
				attackPhase++;
			}
		}
	}

	void DashTimer(){
		dashTimer += Time.deltaTime;

		if (dashTimer > nextDash && PlayerController.Instance != null)
		{
			nextDash = dashTimer + timeBetweenDashes;
			Dash ();
			nextDash -= dashTimer;
			dashTimer = 0.0F;
		}
	}

	void Dash(){
		Vector3 dir = (PlayerController.Instance.transform.position - transform.position).normalized;
		state = States.Attack;
		CalculateDirection (dir);
		//perform movement
		moveVelocity = dir * movespeed * Time.deltaTime * dashSpeedMultiplier;

		animator.ChangeState (state, direction);
	}

	protected void CalculateDirection(Vector3 dir){
		float x = dir.x;
		float z = dir.z;

		if (x < 0) { //left
			if (z < 0.5 && z > -0.5) {
				direction = Directions.W;
			} else if (z >= 0.5) {
				direction = Directions.NW;
			} else if (z <= -0.5) {
				direction = Directions.SW;
			}
		} else if (x > 0) { //right
			if (z < 0.5 && z > -0.5) {
				direction = Directions.E;
			} else if (z >= 0.5) {
				direction = Directions.NE;
			} else if (z <= -0.5) {
				direction = Directions.SE;
			}
		} else if (z > 0) { //up
			direction = Directions.N;
		} else if (z < 0) { //down
			direction = Directions.S;
		} else {
			direction = Directions.Unspecified;
		}
	}

	IEnumerator VelocityChangeTimer(){
		yield return new WaitForSeconds (velocityChangeTime);
		changeVelocity = true;
	}

}
