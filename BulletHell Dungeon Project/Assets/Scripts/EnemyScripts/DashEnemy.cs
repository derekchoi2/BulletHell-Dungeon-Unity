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

		CalculateDirection ((PlayerController.Instance.transform.position - transform.position).normalized);
		Debug.Log ("State: " + state.ToString() + " Dir: " + direction.ToString ());
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
					moveVelocity = dir * movespeed * Time.deltaTime * 7;
				else
					//maintain velocity, slower
					moveVelocity = dir * movespeed * Time.deltaTime * 0.8f;
				
				CalculateDirection (dir);

				animator.ChangeState (state, direction);

				changeVelocity = false;
				StartCoroutine (MoveTimer ());
			}
		}
		transform.position += moveVelocity;

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
