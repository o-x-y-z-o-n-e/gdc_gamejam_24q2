using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ==TODO==
- Weapon swapping & scrolling
- Weapon state machine
- Impact Particles
- SMG
- Shotgun
- Casings
- Audio
-- Weapons
- Footsteps
- Pickups
- HUD
- Main Menu
- End Screen
- Enemies
-- #1
-- #2
-- #3
-- #4
- Death
*/

public class Game : MonoBehaviour {

	//----------------------------------------------------------------------------------------------------------

	public static Game Instance => instance;

	public bool IsPaused => isPaused;

	//----------------------------------------------------------------------------------------------------------

	private bool isPaused;

	private static Game instance;

	//----------------------------------------------------------------------------------------------------------

	private void Awake() {
		instance = this;
	}

	//----------------------------------------------------------------------------------------------------------

	private void OnDestroy() {
		instance = null;
	}

	//----------------------------------------------------------------------------------------------------------

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			isPaused = !isPaused;
		}

		Cursor.visible = isPaused;
		Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
	}

	//----------------------------------------------------------------------------------------------------------

}