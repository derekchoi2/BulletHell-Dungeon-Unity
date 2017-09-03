using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollide : MonoBehaviour {

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag ("Projectile")) {
			//projectile collide with wall
			Destroy(collider.gameObject);
		}
	}

}
