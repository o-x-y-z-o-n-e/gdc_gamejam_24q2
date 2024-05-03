using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void OnPlayClick() {
		SceneManager.LoadScene(1);
	}

	public void OnOptionsClick() {
		// TODO
	}

	public void OnQuitClick() {
		Application.Quit();
	}

}
