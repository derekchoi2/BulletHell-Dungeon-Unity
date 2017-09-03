using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyTypes{
	basic
}

public enum PickupTypes{
	firerate
}

[System.Serializable]
public class Boundary{
	public float xMin, xMax, zMin, zMax;
}

public class GameController : MonoBehaviour {


	public static GameController Instance = null;

	public GameObject playerPrefab;
	public List<GameObject> PickupPrefabs;
	public List<GameObject> EnemyPrefabs;
	public Boundary boundary;
	public Text scoreText;
	public Text timeBetweenShotsText;
	public float PickupSpawnTime = 1.5f;
	public float PickupTextDespawnTime = 1f;
	public float EnemySpawnTime = 3f;
	public float EnemySpawnDecay = 0.2f;
	public float EnemySpawnTimeMin = 0.5f;

	private bool pickupSpawn;
	private bool enemySpawn;
	private List<GameObject> enemies;
	private float SavedEnemySpawnTime;
	private GameObject currentPickupText;
	private GameObject currentPickup;

	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		SavedEnemySpawnTime = EnemySpawnTime;
	}

	// Use this for initialization
	void Start () {
		enemySpawn = false;
		pickupSpawn = false;
		Instantiate (playerPrefab);
		enemies = new List<GameObject> ();
		StartCoroutine (EnemySpawnTimer ());
		StartCoroutine (PickupSpawnTimer ());
	}

	void Update(){
		scoreText.text = "Score: " + PlayerController.Instance.score;
		timeBetweenShotsText.text = "Time Between Shots: " + PlayerController.Instance.timeBetweenShots;

		if (enemySpawn) {
			enemySpawn = false;
			NewEnemy (RandomEnemy());
		}

		if (pickupSpawn) {
			pickupSpawn = false;
			NewPickup ();
		}
	}

	public void ClearRoom(){
		PlayerController.Instance.ResetPos ();
		ClearAllEnemies ();
	}

	public void Reset(){
		ClearAllEnemies ();
		StopAllCoroutines ();
		EnemySpawnTime = SavedEnemySpawnTime;
		StartCoroutine (RestartTimer ());
		Destroy (currentPickup);
		Destroy (currentPickupText);
	}

	EnemyTypes RandomEnemy(){
		//random enum value
		return (EnemyTypes)Random.Range (0, System.Enum.GetNames (typeof(EnemyTypes)).Length - 1);
	}

	void ClearAllEnemies(){
		foreach (GameObject enemy in enemies) {
			Destroy (enemy);
		}
		enemies.Clear ();
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

	PickupTypes RandomPickupType(){
			//random enum value
		return (PickupTypes)Random.Range (0, System.Enum.GetNames (typeof(PickupTypes)).Length - 1);
	}

	void NewPickup(){
		currentPickup = Instantiate (PickupPrefabs [(int)RandomPickupType()], RandomPosition(), Quaternion.identity);
	}

	public void StartPickupTimer(){
		Destroy (currentPickup);
		StartCoroutine (PickupSpawnTimer ());
	}

	public void PickupText(GameObject textObject, Vector3 position, string text){
		currentPickupText = Instantiate (textObject, position, Quaternion.Euler (new Vector3 (90, 0, 0)));
		currentPickupText.GetComponent<TextMesh> ().text = text;
		StartCoroutine (PickupTextTimer ());
	}

	IEnumerator PickupTextTimer(){
		yield return new WaitForSeconds (PickupTextDespawnTime);
		Destroy (currentPickupText);
	}

	IEnumerator PickupSpawnTimer(){
		yield return new WaitForSeconds (PickupSpawnTime);
		pickupSpawn = true;
	}

	public IEnumerator EnemySpawnTimer() {
		yield return new WaitForSeconds(EnemySpawnTime); // wait
		enemySpawn = true; // will make the update method pick up 
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
			if (Vector3.Distance(pos, PlayerController.Instance.transform.position) > 10)
				found = true;
		}
		return pos;
	}

}
