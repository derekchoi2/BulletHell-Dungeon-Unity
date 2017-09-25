using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

	public static LevelController Instance = null;

	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		savedLevelMultiplier = levelMultiplier;
	}

	public float levelTimer = 1f;
	public int totalEnemies;
	public int enemiesToKill;
	public int enemiesSpawned;
	public int levelMultiplier = 5;
	public List<DoorController> Doors;

	private int level = 1;
	private int savedLevelMultiplier;
	private GameController gameController;
	private PickupController pickupController;
	private EnemyController enemyController;
	private PlayerController playerController;

	void Start(){
		gameController = GameController.Instance;
		pickupController = PickupController.Instance;
		enemyController = EnemyController.Instance;
		playerController = PlayerController.Instance;

		HideDoors ();
	}

	public void Update(){
		if (enemiesSpawned == totalEnemies) {
			enemyController.StopSpawning ();
		}
		if (enemiesToKill <= 0) {
			ShowDoors ();
		}
	}

	public void NewLevel(int lvl, int sublevel){
		level = lvl;
		levelMultiplier = savedLevelMultiplier * level;
		totalEnemies = sublevel * levelMultiplier;
		enemiesToKill = totalEnemies;
		enemiesSpawned = 0;
		StartCoroutine (LevelStartTimer ());
	}

	public void EnemyKilled(){
		enemiesToKill--;
	}

	public void EnemySpawned(){
		enemiesSpawned++;
	}

	void LevelStart(){
		playerController.Show ();
		enemyController.LevelStart (level);
		pickupController.LevelStart ();
	}

	void LevelEnd(){
		//open doors stop all spawning
		enemyController.LevelEnd();
		pickupController.LevelEnd ();
		gameController.LevelEnd ();
		HideDoors ();
	}

	public void PlayerDoorCollide(){
		gameController.LevelUp ();
		LevelEnd ();
	}

	void ShowDoors(){
		foreach (DoorController door in Doors) {
			door.Show ();
		}
	}

	void HideDoors(){
		foreach (DoorController door in Doors) {
			door.Hide ();
		}
	}

	IEnumerator LevelStartTimer() {
		yield return new WaitForSeconds (levelTimer);
		LevelStart ();
	}

}
