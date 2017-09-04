using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour {

	public PickupTypes type;
	public float value = -0.02f;
	public string displayText;

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag("Player")){
			//collide with player
			PickupController.Instance.ShowPickupText(transform.position, displayText);
			Destroy (gameObject);
		}
	}
}
