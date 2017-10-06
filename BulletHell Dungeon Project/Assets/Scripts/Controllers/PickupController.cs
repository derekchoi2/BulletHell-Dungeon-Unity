using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupTypes{
	firerate, sentry
}

public class PickupController : MonoBehaviour {

	public static PickupController Instance = null;

	public List<GameObject> PickupPrefabs;
	public GameObject PickupTextPrefab;
	public float PickupSpawnTime = 8f;
	public float PickupTextDespawnTime = 1f;
	public bool spawn = false;

	private bool pickupSpawn;
	private List<GameObject> pickupTexts = new List<GameObject>();
	private List<GameObject> pickups = new List<GameObject>();

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
		foreach (GameObject pickup in pickups)
			Destroy (pickup);
		foreach (GameObject pickupText in pickupTexts)
			Destroy (pickupText);
		pickupSpawn = false;
	}

	void NewPickup(){
		pickups.Add(Instantiate (PickupPrefabs [(int)RandomPickupType()], gameController.RandomPosition(), Quaternion.identity));
	}

	public void PickupCollected(){
		StartCoroutine (PickupSpawnTimer ());
	}

	public void ShowPickupText(Vector3 position, string text){
		GameObject newPickupText = Instantiate (PickupTextPrefab, position, Quaternion.Euler (new Vector3 (90, 0, 0)));
		newPickupText.GetComponent<TextMesh> ().text = text;
		pickupTexts.Add (newPickupText);
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
		Destroy (pickupTexts[0]);
		pickupTexts.RemoveAt (0);
	}
}
