using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteListWrapper{
	public States state;
	public float TimeBetweenSprites = 0.5f;
	public List<Sprite> sprites;
}


public class SpriteAnimator : MonoBehaviour {
	public List<SpriteListWrapper> sprites;

	private float currentTime;
	private int currentSprite;
	private int currentState;

	private SpriteRenderer spriteRenderer;

	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
		currentTime = 0;
		currentSprite = 0;
		currentState = FindStateIndex(States.Idle);

		UpdateSprite ();
	}

	int FindStateIndex(States state){
		for (int i = 0; i < sprites.Count; i++){
			if (sprites [i].state == state)
				return i;
		}
		return 0;
	}

	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;

		if (currentTime > sprites[currentState].TimeBetweenSprites) {
			currentSprite = (currentSprite + 1) % sprites [currentState].sprites.Count;
			UpdateSprite ();
			currentTime = 0;
		}
	}

	void UpdateSprite(){
		spriteRenderer.sprite = sprites[currentState].sprites[currentSprite];
	}

	void ChangeState(States state){
		currentState = FindStateIndex (state);
		currentSprite = 0;
		currentTime = 0;
	}
}
