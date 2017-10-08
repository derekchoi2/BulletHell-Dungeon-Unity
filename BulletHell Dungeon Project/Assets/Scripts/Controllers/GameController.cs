﻿using System.Collections;
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
	public int sublevel = 1;

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
		levelController.NewLevel (level, sublevel);
	}

	void Update(){
		UpdateText ();
	}

	void UpdateText(){
		scoreText.text = "Score: " + playerController.score;
		timeBetweenShotsText.text = (playerController.CurrentWeapon != null)? "Time Between Shots: " + playerController.CurrentWeapon.GetComponent<Weapon>().timeBetweenShots : " ";
		timeBetweenEnemySpawnText.text = "Time Between Enemy Spawns: " + enemyController.EnemySpawnTime;
		EnemiesLeftText.text = "Enemies Left: " + levelController.enemiesToKill;
		LevelText.text = "Level: " + level + "-" + sublevel;
		PlayerHealthText.text = "Health: " + playerController.health;
	}

	void ClearRoom(){
		enemyController.Clear ();
		pickupController.Clear ();
	}

	void Reset(){
		pickupController.Clear ();
		enemyController.Reset ();
	}

	public void PlayerDie(){
		Reset ();
		level = 1;
		sublevel = 1;
		levelController.NewLevel (level, sublevel);
	}

	public void LevelEnd(){
		ClearRoom ();
	}


	public void LevelUp(){
		sublevel++;
		if (sublevel > 3) {
			sublevel = 1;
			level++;
		}
		levelController.NewLevel (level, sublevel);
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
