using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	[SerializeField] private GameObject confirmQuitPanel;
	[SerializeField] private Button resumeButton;
	[SerializeField] private Button confirmQuitNoButton;

	private void OnEnable() {
		confirmQuitPanel.gameObject.SetActive(false);
		resumeButton.Select();
	}

	public void OnResumeClick() {
		Game.Instance.SetPaused(false);
	}

	public void OnOptionsClick() {
		// TODO
	}

	public void OnQuitClick() {
		confirmQuitPanel.gameObject.SetActive(true);
		confirmQuitNoButton.Select();
	}

	public void OnQuitNoClick() {
		confirmQuitPanel.gameObject.SetActive(false);
	}

	public void OnQuitYesClick() {
		SceneManager.LoadScene(0);
	}

}
