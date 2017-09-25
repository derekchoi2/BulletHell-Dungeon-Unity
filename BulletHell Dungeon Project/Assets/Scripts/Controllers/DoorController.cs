using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

	private BoxCollider boxCollider;
	private MeshRenderer meshRenderer;

	void Awake(){
		boxCollider = gameObject.GetComponent<BoxCollider> ();
		meshRenderer = gameObject.GetComponent<MeshRenderer> ();
	}

	public void Hide(){
		boxCollider.enabled = false;
		meshRenderer.enabled = false;
	}

	public void Show(){
		boxCollider.enabled = true;
		meshRenderer.enabled = true;
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag("Player")){
			//collide with player
			Debug.Log("Player collide with door");
		}
	}
}
