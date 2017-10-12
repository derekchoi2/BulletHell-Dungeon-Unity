using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Boundary{
	public float xMin, xMax, zMin, zMax;
}

public class GameController : MonoBehaviour {

	public static GameController Instance = null;	
	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public Boundary boundary;
	public Text scoreText;
	public Text timeBetweenShotsText;
	public Text timeBetweenEnemySpawnText;
	public Text EnemiesLeftText;
	public Text LevelText;
	public Text PlayerHealthText;
	public int level = 1;
	public int sublevel = 1; //start in hub, gets incremented when leave hub
	public int sublevelMax = 3;

	//controller singletons
	private PickupController pickupController;
	private EnemyController enemyController;
	private LevelController levelController;
	private PlayerController playerController;

	// Use this for initialization
	void Start () {
		pickupController = PickupController.Instance;
		enemyController = EnemyController.Instance;
		levelController = LevelController.Instance;
		playerController = PlayerController.Instance;
		levelController.NewLevel ((levelController.hub)? DoorTypes.hub : DoorTypes.nextLevel, level, sublevel);
	}

	void Update(){
		UpdateText ();
	}

	void UpdateText(){
		scoreText.text = "Score: " + playerController.score;
		timeBetweenShotsText.text = (playerController.CurrentWeapon != null)? "Time Between Shots: " + playerController.CurrentWeapon.GetComponent<Weapon>().timeBetweenShots : " ";
		timeBetweenEnemySpawnText.text = "Time Between Enemy Spawns: " + enemyController.EnemySpawnTime;
		EnemiesLeftText.text = "Enemies Left: " + levelController.enemiesToKill;
		PlayerHealthText.text = "Health: " + playerController.health;
		LevelText.text = (levelController.hub)? "HUB" : "Level: " + level + "-" + sublevel;
	}

	void Reset(){
		pickupController.Clear ();
		enemyController.Reset ();
	}

	public void PlayerDie(){
		Reset ();
		level = 1;
		sublevel = 0;
		levelController.NewLevel (DoorTypes.hub, level, sublevel);
	}

	public void LevelEnd(){
		//clear room once go through doors
		enemyController.Clear ();
		pickupController.Clear ();
	}

	public void LevelUp(DoorTypes type){
		if (type != DoorTypes.hub){
			sublevel++;
			if (sublevel > sublevelMax) {
				sublevel = 1;
				level++;
			}
		}
		levelController.NewLevel (type, level, sublevel);
	}

	public Vector3 RandomPosition(){
		//away from player
		bool found = false;
		Vector3 pos = Vector3.zero;
		while (!found) {
			pos = new Vector3 (Random.Range (boundary.xMin, boundary.xMax), 1, Random.Range (boundary.zMin, boundary.zMax));
			if (playerController != null) {
				if (Vector3.Distance (pos, PlayerController.Instance.transform.position) > 20)
					found = true;
			} else 
				found = true;
		}
		return pos;
	}

}
