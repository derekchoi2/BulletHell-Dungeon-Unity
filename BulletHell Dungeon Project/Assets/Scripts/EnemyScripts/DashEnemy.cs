using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEnemy : Enemy {


	public float moveTime;
	public float attackTime;

	private bool changeVelocity = false;
	private SpriteAnimator animator;

	// Use this for initialization
	void Start () {
		state = States.Move;
		CalculateDirection (GameController.Instance.RandomPosition().normalized);
		animator = gameObject.GetComponentInChildren<SpriteAnimator> ();
		projectiles = new List<GameObject> ();
		nextShot = timeBetweenShots;
		gc = GameController.Instance;
		StartCoroutine (MoveTimer ());
		transform.position = GameController.Instance.RandomPosition();
	}

	protected override void Move ()
	{
		if (PlayerController.Instance != null) {
			Vector3 dir;
			if (changeVelocity) {
				if (state == States.Attack) {
					//dash towards player
					dir = (PlayerController.Instance.transform.position - transform.position).normalized;
					moveVelocity = dir * movespeed * Time.deltaTime * 5;
				} else {
					//normal walk in random direction
					dir = GameController.Instance.RandomPosition ().normalized;
					moveVelocity = dir * movespeed * Time.deltaTime * 0.8f;
				}
				CalculateDirection (dir);

				animator.ChangeState (state, direction);

				changeVelocity = false;
				StartCoroutine (MoveTimer ());
			}
		}
		transform.position += moveVelocity;

	}

	protected override void Attack(){
		//dash
		Vector3 dir = (PlayerController.Instance.transform.position - transform.position).normalized;
	}

	protected void CalculateDirection(Vector3 dir){
		float x = dir.x;
		if (x < 0) { //left
			direction = Directions.W;
		} else { //right/default
			direction = Directions.E;
		}

	}

	protected IEnumerator MoveTimer(){
		if (state == States.Move) {
			state = States.Attack;
			yield return new WaitForSeconds (moveTime);
		} else {
			state = States.Move;
			yield return new WaitForSeconds (attackTime);
		}
		changeVelocity = true;
	}

}
