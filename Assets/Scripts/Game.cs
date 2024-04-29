using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game Instance => instance;

	public bool IsPaused => isPaused;

	private bool isPaused;

	private static Game instance;

	private void Awake() {
		instance = this;
	}

	private void OnDestroy() {
		instance = null;
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			isPaused = !isPaused;
		}

		Cursor.visible = isPaused;
		Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
	}

}