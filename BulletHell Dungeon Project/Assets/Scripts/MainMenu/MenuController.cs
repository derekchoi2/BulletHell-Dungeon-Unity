using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	enum Scenes{
		MainMenu, Play, Help
	}

	public void ButtonClick(GameObject gameObject){
		switch (gameObject.tag) {
		case "MainMenuButton":
			MainMenuButton ();
			break;
		case "PlayButton":
			PlayButton ();
			break;
		case "HelpButton":
			HelpButton ();
			break;
		case "QuitButton":
			QuitButton ();
			break;
		}

	}

	void MainMenuButton(){
		Debug.Log ("MainMenuButton clicked");
		SceneManager.LoadScene ((int)Scenes.MainMenu);
	}

	void PlayButton(){
		Debug.Log ("PlayButton clicked");
		SceneManager.LoadScene ((int)Scenes.Play);
	}

	void QuitButton(){
		Debug.Log ("QuitButton clicked");
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}

	void HelpButton(){
		Debug.Log ("HelpButton clicked");
		SceneManager.LoadScene ((int)Scenes.Help);
	}

}
