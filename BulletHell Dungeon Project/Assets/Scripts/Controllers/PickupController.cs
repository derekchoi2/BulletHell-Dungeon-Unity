using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupTypes{
	firerate
}

public class PickupController : MonoBehaviour {

	public static PickupController Instance = null;

	public List<GameObject> PickupPrefabs;
	public GameObject PickupTextPrefab;
	public float PickupSpawnTime = 8f;
	public float PickupTextDespawnTime = 1f;
	public bool spawn = false;

	private bool pickupSpawn;
	private GameObject currentPickupText;
	private GameObject currentPickup;

	private GameController gameController;

	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		gameController = GameController.Instance;
		pickupSpawn = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (pickupSpawn && spawn) {
			spawn = false;
			NewPickup ();
		}
	}

	public void LevelStart(){
		spawn = true;
		StartCoroutine (PickupSpawnTimer ());
	}

	public void LevelEnd(){
		spawn = false;
		StopAllCoroutines ();
	}

	public void Reset(){
		StopAllCoroutines ();
		Destroy (currentPickup);
		Destroy (currentPickupText);
		Start ();
	}

	void NewPickup(){
		currentPickup = Instantiate (PickupPrefabs [(int)RandomPickupType()], gameController.RandomPosition(), Quaternion.identity);
	}

	public void PickupCollected(){
		StartCoroutine (PickupSpawnTimer ());
	}

	public void ShowPickupText(Vector3 position, string text){
		currentPickupText = Instantiate (PickupTextPrefab, position, Quaternion.Euler (new Vector3 (90, 0, 0)));
		currentPickupText.GetComponent<TextMesh> ().text = text;
		StartCoroutine (PickupTextTimer ());
	}

	PickupTypes RandomPickupType(){
		//random enum value
		return (PickupTypes)Random.Range (0, System.Enum.GetNames (typeof(PickupTypes)).Length);
	}

	public void Clear(){
		Reset ();
	}

	IEnumerator PickupSpawnTimer(){
		yield return new WaitForSeconds (PickupSpawnTime);
		pickupSpawn = true;
	}

	IEnumerator PickupTextTimer(){
		yield return new WaitForSeconds (PickupTextDespawnTime);
		Destroy (currentPickupText);
	}
}
