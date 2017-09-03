using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollide : MonoBehaviour {

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag("Player")){
			//collide with player
			Debug.Log("Player collide with door");
		}
	}
}
