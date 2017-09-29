using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {


	public Transform currentValTransform;

	public int MaxHealth;
	private int CurrentHealth;

	void Start(){
		CurrentHealth = MaxHealth;
	}

	public void ChangeHealth(int val){
		CurrentHealth = Mathf.Clamp (CurrentHealth + val, 0, MaxHealth);
	}

	public void UpdateHealthBar(){
		float percentage = (float)CurrentHealth / (float)MaxHealth;
		Vector3 currentScale = currentValTransform.localScale;
		currentValTransform.localScale = new Vector3(percentage, currentScale.y, currentScale.z);

	}

	public bool isDead(){
		if (CurrentHealth <= 0)
			return true;
		else
			return false;
	}

}
