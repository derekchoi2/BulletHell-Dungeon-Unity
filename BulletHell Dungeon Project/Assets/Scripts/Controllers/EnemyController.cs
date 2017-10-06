using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public static EnemyController Instance = null;

	public List<GameObject> EnemyPrefabs;
	public float EnemySpawnTime = 3f;
	public float EnemySpawnDecay = 0.1f;
	public float EnemySpawnTimeMin = 0.5f;

	private int enemiesPerSpawn = 1;
	public bool spawn; //affected by number of spawns
	private bool enemySpawn; //affected by timer
	private List<GameObject> enemies;
	private float SavedEnemySpawnTime;

	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		enemies = new List<GameObject> ();
		SavedEnemySpawnTime = EnemySpawnTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (enemySpawn && spawn) {
			enemySpawn = false;
			for (int i = 0; i < enemiesPerSpawn; i++) {
				NewEnemy (RandomEnemy ());
			}
			StartCoroutine (EnemySpawnTimer ());
		}
	}

	public void LevelStart(int level){
		enemiesPerSpawn = level;
		EnemySpawnTime = SavedEnemySpawnTime;
		spawn = true;
		StartCoroutine (EnemySpawnTimer ());
	}

	public void LevelEnd(){
		StopSpawning ();
	}

	public void StopSpawning(){
		spawn = false;
		StopAllCoroutines ();
	}

	public void Clear(){
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<NPC> ().ClearProjectiles ();
			Destroy (enemy);
		}
		enemies.Clear ();
	}

	EnemyTypes RandomEnemy(){
		//random enum value
		int enumsize = System.Enum.GetNames (typeof(EnemyTypes)).Length;
		if (enumsize == 1)
			return (EnemyTypes) 0;
		else
			return (EnemyTypes)Random.Range (0, System.Enum.GetNames (typeof(EnemyTypes)).Length);

	}

	public void EnemyKilled(GameObject enemy){
		PickupController.Instance.NewPickup (enemy.transform.position);
		enemies.Remove (enemy);
		Destroy (enemy);
	}

	void NewEnemy(EnemyTypes enemy){
		enemies.Add(Instantiate(EnemyPrefabs[(int)enemy]));
		EnemySpawnTime = EnemySpawnTime - EnemySpawnDecay;
		if (EnemySpawnTime < EnemySpawnTimeMin)
			EnemySpawnTime = EnemySpawnTimeMin;
		LevelController.Instance.EnemySpawned ();
	}

	public void Reset(){
		StopAllCoroutines ();
		EnemySpawnTime = SavedEnemySpawnTime;
		Clear ();
		Start ();
	}

	public IEnumerator EnemySpawnTimer() {
		yield return new WaitForSeconds(EnemySpawnTime); // wait
		enemySpawn = true; // will make the update method pick up 
	}

	public List<GameObject> GetEnemies() {
		return enemies;
	}
}
