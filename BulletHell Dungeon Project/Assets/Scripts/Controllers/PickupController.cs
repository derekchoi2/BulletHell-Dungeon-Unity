﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupTypes{
	firerateUp, sentry
}
public class PickupController : MonoBehaviour {

	[System.Serializable]
	public class Pickup{
		public PickupTypes type;
		public GameObject PickupPrefab;
		[Range(0f,100f)]public int RelativeRarity;
	}

	public static PickupController Instance = null;

	[Range(0f, 100f)]public int DropChance;

	public List<Pickup> Pickups;
	public GameObject PickupTextPrefab;
	public float PickupTextDespawnTime = 1f;

	private List<GameObject> spawnedTexts = new List<GameObject>();
	private List<GameObject> spawnedPickups = new List<GameObject>();

	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void Clear(){
		StopAllCoroutines ();
		foreach (GameObject p in spawnedPickups)
			Destroy (p);
		foreach (GameObject text in spawnedTexts)
			Destroy (text);
	}

	public void NewPickup(Vector3 position){
		if (Random.Range (0, 100) <= DropChance) {
			//see if enemy drops an item according to DropChance
			spawnedPickups.Add(Instantiate (Pickups [(int)RandomPickupType()].PickupPrefab, position, Quaternion.identity));
		}
	}

	public void ShowPickupText(Vector3 position, string text){
		GameObject newPickupText = Instantiate (PickupTextPrefab, position, Quaternion.Euler (new Vector3 (90, 0, 0)));
		newPickupText.GetComponent<TextMesh> ().text = text;
		spawnedTexts.Add (newPickupText);
		StartCoroutine (PickupTextTimer ());
	}

	PickupTypes RandomPickupType(){
		//random enum value according to drop chaces
		int weight = 0;
		foreach (Pickup p in Pickups)
			weight += p.RelativeRarity;
		
		int rand = Random.Range (0, weight);
		foreach (Pickup p in Pickups) {
			if (rand <= p.RelativeRarity)
				return p.type;
			rand -= p.RelativeRarity;
		}

		//this should never happen. prevents compiler errors
		return (PickupTypes) 0;
	}

	IEnumerator PickupTextTimer(){
		yield return new WaitForSeconds (PickupTextDespawnTime);
		Destroy (spawnedTexts[0]);
		spawnedTexts.RemoveAt (0);
	}
}
