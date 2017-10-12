using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

	public DoorTypes type;
	public DoorPosition position;
	private BoxCollider boxCollider;
	private MeshRenderer meshRenderer;
	public List<Material> materials;
	public bool show;

	void Awake(){
		boxCollider = gameObject.GetComponent<BoxCollider> ();
		meshRenderer = gameObject.GetComponent<MeshRenderer> ();
	}

	public void Hide(){
		boxCollider.enabled = false;
		meshRenderer.enabled = false;
		show = false;
	}

	public void Show(){
		boxCollider.enabled = true;
		meshRenderer.enabled = true;
		show = true;
	}

	public void SetType(DoorTypes type){
		this.type = type;
		if (type == DoorTypes.hub)
			meshRenderer.material = materials [1]; //white
		else
			meshRenderer.material = materials [0]; //black
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag("Player")){
			//collide with player
			//Debug.Log("Player collide with door");
		}
	}
}
