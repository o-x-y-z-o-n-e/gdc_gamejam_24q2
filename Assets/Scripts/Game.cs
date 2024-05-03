using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

	public HUD HUD => hud;
	public PauseMenu PauseMenu => pauseMenu;

	public Player Player => player;

	//----------------------------------------------------------------------------------------------------------

	[SerializeField] private Player player;
	[SerializeField] private HUD hud;
	[SerializeField] private PauseMenu pauseMenu;

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

	public void SetPaused(bool paused) {
		isPaused = paused;
		hud.gameObject.SetActive(!isPaused);
		pauseMenu.gameObject.SetActive(isPaused);
	}

	//----------------------------------------------------------------------------------------------------------

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			SetPaused(!isPaused);
		}

		Cursor.visible = isPaused;
		Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
	}

	//----------------------------------------------------------------------------------------------------------

}