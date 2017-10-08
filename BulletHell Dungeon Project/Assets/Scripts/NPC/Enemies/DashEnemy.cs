using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEnemy : NPC {

	public float moveTime;
	public float attackTime;

	private SpriteAnimator animator;

	// Use this for initialization
	void Start () {
		state = States.Attack; //so will change to move when first spawn
		CalculateDirection (GameController.Instance.RandomPosition().normalized);
		animator = gameObject.GetComponentInChildren<SpriteAnimator> ();

		animator.ChangeState(state, direction);

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
				dir = (PlayerController.Instance.transform.position - transform.position).normalized;

				if (state == States.Attack)
					//dash towards player
					moveVelocity = dir * movespeed * Time.deltaTime * 10;
				else
					//maintain velocity, slower
					moveVelocity = dir * movespeed * Time.deltaTime;
				
				CalculateDirection (dir);

				animator.ChangeState (state, direction);

				changeVelocity = false;
				StopAllCoroutines ();
				StartCoroutine (MoveTimer ());
			}
			transform.position += moveVelocity;
		}

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
			yield return new WaitForSeconds (attackTime);
		} else {
			state = States.Move;
			yield return new WaitForSeconds (moveTime);
		}
		changeVelocity = true;
	}

}
