using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsList : MonoBehaviour {

	public static WeaponsList Instance = null; 
	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public List<GameObject> Weapons = new List<GameObject>();
}
