using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour {
	
	public static PickupController Instance = null;
	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	[System.Serializable]
	public class Pickup{
		public PickupTypes type;
		public GameObject PickupPrefab;
		[Range(0f,100f)]public int RelativeRarity;
	}

	[Range(0f, 100f)]public int DropChance;

	public List<Pickup> Pickups;
	public GameObject PickupTextPrefab;
	public float PickupTextDespawnTime = 1f;

	private List<GameObject> spawnedTexts = new List<GameObject>();
	private List<GameObject> spawnedPickups = new List<GameObject>();

	private int activeSentryPickups = 0;

	public void Clear(){
		StopAllCoroutines ();
		foreach (GameObject p in spawnedPickups)
			Destroy (p);
		foreach (GameObject text in spawnedTexts)
			Destroy (text);

		spawnedPickups.Clear ();
		spawnedTexts.Clear ();

		activeSentryPickups = 0;
	}

	public void NewPickup(Vector3 position){
		if (Random.Range (0, 100) <= DropChance) {
			//see if enemy drops an item according to DropChance
			spawnedPickups.Add(Instantiate (Pickups [(int)RandomPickupType()].PickupPrefab, position, Quaternion.identity));
		}
	}

	public void ShowPickupText(GameObject pickup, PickupTypes type, Vector3 position, string text){
		if (type == PickupTypes.sentry)
			activeSentryPickups--;
		spawnedPickups.Remove (pickup);
		Destroy (pickup);
		GameObject newPickupText = Instantiate (PickupTextPrefab, position, Quaternion.Euler (new Vector3 (90, 0, 0)));
		newPickupText.GetComponent<TextMesh> ().text = text;
		spawnedTexts.Add (newPickupText);
		StartCoroutine (PickupTextTimer ());
	}

	PickupTypes RandomPickupType(){
		//random enum value according to drop chaces

		//remove current weapon from list temporarily so it doesn't drop as a pickup
		//exclude items with 0 relative rarity
		List<Pickup> temp = new List<Pickup>();
		int weight = 0;
		foreach (Pickup p in Pickups) {
			if (p.RelativeRarity > 0 && p.type != PlayerController.Instance.CurrentWeapon.GetComponent<Weapon> ().PickupType) {

				if (p.type != PickupTypes.sentry || (p.type == PickupTypes.sentry && DropSentry ())) {
					//add if not a sentry. If it is a sentry check that it can drop
					temp.Add (p);
					weight += p.RelativeRarity;
				}

			}
		}
		
		int rand = Random.Range (0, weight);
		foreach (Pickup p in temp) {
			if (rand <= p.RelativeRarity) {
				if (p.type == PickupTypes.sentry)
					activeSentryPickups++;
				return p.type;
			}
			rand -= p.RelativeRarity;
		}

		//this should never happen. prevents compiler errors
		return (PickupTypes) 0;
	}

	bool DropSentry(){
		int activeSentries = PlayerController.Instance.SentryCount ();

		if (activeSentries + activeSentryPickups < PlayerController.Instance.maxSentries)
			return true;
		return false;

	}

	IEnumerator PickupTextTimer(){
		yield return new WaitForSeconds (PickupTextDespawnTime);
		Destroy (spawnedTexts[0]);
		spawnedTexts.RemoveAt (0);
	}
}
