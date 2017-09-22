using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum States{
	Idle, Attack, Hit, Die, Move
}

public enum Directions{
	Unspecified, S, SW, W, NW, N, NE, E, SE
}

public enum EnemyTypes{
	basic, fast
}

[System.Serializable]
public class Boundary{
	public float xMin, xMax, zMin, zMax;
}

public class GameController : MonoBehaviour {


	public static GameController Instance = null;

	public GameObject playerPrefab;
	public Boundary boundary;
	public Text scoreText;
	public Text timeBetweenShotsText;
	public Text timeBetweenEnemySpawnText;


	//controller singletons
	private PickupController pickupController;
	private EnemyController enemyController;
	private PlayerController playerController;

	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		pickupController = PickupController.Instance;
		enemyController = EnemyController.Instance;

		Instantiate (playerPrefab);
		playerController = PlayerController.Instance;
	}

	void Update(){
		UpdateText ();
	}

	void UpdateText(){
		scoreText.text = "Score: " + playerController.score;
		timeBetweenShotsText.text = "Time Between Shots: " + playerController.timeBetweenShots;
		timeBetweenEnemySpawnText.text = "Time Between Enemy Spawns: " + enemyController.EnemySpawnTime;
	}

	public void ClearRoom(){
		playerController.ResetPos ();
		enemyController.Clear ();
		pickupController.Clear ();
	}

	public void Reset(){
		StopAllCoroutines ();
		StartCoroutine (RestartTimer ());
		pickupController.Reset ();
		enemyController.Reset ();
	}

	public IEnumerator RestartTimer() {
		yield return new WaitForSeconds (2f);
		Start ();
	}

	public void PlayerHit(){
		Reset ();
	}

	public Vector3 RandomPosition(){
		//away from player
		bool found = false;
		Vector3 pos = Vector3.zero;
		while (!found) {
			pos = new Vector3 (Random.Range (boundary.xMin, boundary.xMax), 1, Random.Range (boundary.zMin, boundary.zMax));
			if (playerController != null) {
				if (Vector3.Distance (pos, PlayerController.Instance.transform.position) > 10)
					found = true;
			} else 
				found = true;
		}
		return pos;
	}

}
