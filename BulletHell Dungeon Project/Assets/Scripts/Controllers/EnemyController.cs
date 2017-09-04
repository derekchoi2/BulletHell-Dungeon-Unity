using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public static EnemyController Instance = null;

	public List<GameObject> EnemyPrefabs;
	public float EnemySpawnTime = 3f;
	public float EnemySpawnDecay = 0.1f;
	public float EnemySpawnTimeMin = 0.5f;

	private bool enemySpawn;
	private List<GameObject> enemies;
	private float SavedEnemySpawnTime;

	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		SavedEnemySpawnTime = EnemySpawnTime;
		enemySpawn = false;
		enemies = new List<GameObject> ();
		StartCoroutine (EnemySpawnTimer ());
	}
	
	// Update is called once per frame
	void Update () {
		if (enemySpawn) {
			enemySpawn = false;
			NewEnemy (RandomEnemy());
		}
	}

	public void Clear(){
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Enemy> ().ClearProjectiles ();
			Destroy (enemy);
		}
		enemies.Clear ();
	}

	EnemyTypes RandomEnemy(){
		//random enum value
		return (EnemyTypes)Random.Range (0, System.Enum.GetNames (typeof(EnemyTypes)).Length - 1);
	}

	public void EnemyHit(GameObject enemy){
		enemies.Remove (enemy);
		Destroy (enemy);
		PlayerController.Instance.score++;
	}

	void NewEnemy(EnemyTypes enemy){
		enemies.Add(Instantiate(EnemyPrefabs[(int)enemy]));
		EnemySpawnTime = Mathf.Clamp (EnemySpawnTime - EnemySpawnDecay, EnemySpawnTimeMin, EnemySpawnTime);
		StartCoroutine (EnemySpawnTimer ());
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
}
