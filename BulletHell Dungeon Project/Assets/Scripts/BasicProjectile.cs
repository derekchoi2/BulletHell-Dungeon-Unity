using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour {

	private Vector3 projVelocity;
	public Owner owner;

	public enum Owner
	{
		Player, Enemy
	}

	public void SetVelocity(Vector3 vec){
		projVelocity = vec;
	}

	// Update is called once per frame
	void Update () {
		transform.position += projVelocity;
	}
}
