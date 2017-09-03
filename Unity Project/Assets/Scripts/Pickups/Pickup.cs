using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour {

	public PickupTypes type;
	public float value = -0.1f;
	public string displayText;
	public GameObject textObject;

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag("Player")){
			//collide with player
			GameController.Instance.PickupText(textObject, transform.position, displayText);
		}
	}
}
