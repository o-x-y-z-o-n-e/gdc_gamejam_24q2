using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

	[SerializeField] private Text ammoText;
	[SerializeField] private RectTransform healthBarFill;
	
	public void SetAmmo(int mag, int pool) {
		if(mag < 0 && pool < 0) {
			ammoText.text = "Ammo: [N/A]";
		} else if(mag < 0 && pool >= 0) {
			ammoText.text = $"Ammo: [{pool}]";
		} else if(mag >= 0 && pool < 0) {
			ammoText.text = $"Ammo: [{mag}]";
		} else {
			ammoText.text = $"Ammo: [{mag}/{pool}]";
		}
	}

	public void SetHealth(int health, int maxHealth) {
		float percent = Mathf.Clamp01(health / (float)maxHealth);

		Vector2 size = (healthBarFill.parent as RectTransform).sizeDelta;
		size.x *= percent;
		healthBarFill.sizeDelta = size;
	}

}
