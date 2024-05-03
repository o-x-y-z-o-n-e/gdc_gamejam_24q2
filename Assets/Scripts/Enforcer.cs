using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enforcer : Weapon {

	private const int IDLE_ANIMATION = 0;
	private const int FIRE_ANIMATION = 1;
	private const int RELOAD_ANIMATION = 2;

	private Sprite ICON;

	private Frame[] IDLE_FRAMES;
	private Frame[] FIRE_FRAMES;
	private Frame[] RELOAD_FRAMES;

	public Casing CasingPrefab;
	public Vector3 CasingPosition;
	public Vector3 CasingDirection;
	public float CasingSpeed;

	protected override void Awake() {
		base.Awake();

		ICON = Resources.Load<Sprite>("Weapons/Enforcer/Sprites/LIBGX0");

		IDLE_FRAMES = new Frame[] {
			new Frame("Weapons/Enforcer/Sprites/LIBGA0")
		};

		FIRE_FRAMES = new Frame[] {
			new Frame("Weapons/Enforcer/Sprites/LIBFA0"),
			new Frame("Weapons/Enforcer/Sprites/LIBFB0"),
			new Frame("Weapons/Enforcer/Sprites/LIBGB0"),
			new Frame("Weapons/Enforcer/Sprites/LIBGC0"),
			new Frame("Weapons/Enforcer/Sprites/LIBGD0"),
			new Frame("Weapons/Enforcer/Sprites/LIBGE0"),
		};

		RELOAD_FRAMES = new Frame[] {
			new Frame("Weapons/Enforcer/Sprites/LIBMA0"),
			new Frame("Weapons/Enforcer/Sprites/LIBMB0"),
			new Frame("Weapons/Enforcer/Sprites/LIBMC0"),
			new Frame("Weapons/Enforcer/Sprites/LIBMD0"),
			new Frame("Weapons/Enforcer/Sprites/LIBME0"),
			new Frame("Weapons/Enforcer/Sprites/LIBMF0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRA0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRB0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRC0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRD0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRE0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRF0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRG0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRH0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRI0"),
			new Frame("Weapons/Enforcer/Sprites/LIBRJ0"),
			new Frame("Weapons/Enforcer/Sprites/LIBEA0"),
			new Frame("Weapons/Enforcer/Sprites/LIBEB0"),
			new Frame("Weapons/Enforcer/Sprites/LIBEC0"),
			new Frame("Weapons/Enforcer/Sprites/LIBED0"),
			new Frame("Weapons/Enforcer/Sprites/LIBEE0"),
			new Frame("Weapons/Enforcer/Sprites/LIBEF0"),
			new Frame("Weapons/Enforcer/Sprites/LIBEG0"),
			new Frame("Weapons/Enforcer/Sprites/LIBEH0"),
		};

		SetVerticalSpriteClamp(2);
	}

	protected override void OnEnable() {
		base.OnEnable();

		if(Mag == 0 && Ammo > 0) {
			GoTo(RELOAD_ANIMATION);
		} else {
			GoTo(IDLE_ANIMATION);
		}
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerator Idle() {
		SetFrame(IDLE_FRAMES[0]);

		while(true) {
			yield return new WaitForFixedUpdate();

			if(CheckReload()) {
				GoTo(RELOAD_ANIMATION);
				yield return null;
			}

			if(CheckAttack()) {
				GoTo(FIRE_ANIMATION);
				yield return null;
			}
		}
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerator Fire() {
		SetTickRate(70);
		SendAttackRaycast();
		RemoveAmmo(1);

		SetFrame(FIRE_FRAMES[0]); yield return Tick(2);
		SetFrame(FIRE_FRAMES[1]); yield return Tick(2);
		SetFrame(FIRE_FRAMES[2]); yield return Tick(1);
		SetFrame(FIRE_FRAMES[3]); yield return Tick(1);
		SpawnCasing(CasingPrefab, CasingPosition, CasingDirection, CasingSpeed, 0);
		SetFrame(FIRE_FRAMES[4]); yield return Tick(1);
		SetFrame(FIRE_FRAMES[5]); yield return Tick(1);
		SetFrame(FIRE_FRAMES[4]); yield return Tick(2);
		SetFrame(FIRE_FRAMES[3]); yield return Tick(3);
		SetFrame(FIRE_FRAMES[2]); yield return Tick(4);

		SetTickRate(35);

		SetFrame(IDLE_FRAMES[0]); yield return Tick(2);

		if(Mag == 0 && Ammo > 0) {
			GoTo(RELOAD_ANIMATION);
		} else {
			GoTo(IDLE_ANIMATION);
		}
	}

	private IEnumerator Reload() {
		SetTickRate(60);
		SetFrame(RELOAD_FRAMES[00]); yield return Tick(3);
		SetFrame(RELOAD_FRAMES[01]); yield return Tick(3);
		SetFrame(RELOAD_FRAMES[02]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[03]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[04]); yield return Tick(1);
		SetFrame(RELOAD_FRAMES[05]); yield return Tick(1);
		SetFrame(RELOAD_FRAMES[06]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[07]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[08]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[09]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[10]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[11]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[12]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[13]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[14]); yield return Tick(3);
		LoadMag();
		SetFrame(RELOAD_FRAMES[15]); yield return Tick(10);
		SetFrame(RELOAD_FRAMES[16]); yield return Tick(5);
		SetFrame(RELOAD_FRAMES[17]); yield return Tick(4);
		SetFrame(RELOAD_FRAMES[18]); yield return Tick(3);
		SetFrame(RELOAD_FRAMES[19]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[20]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[21]); yield return Tick(2);
		SetFrame(RELOAD_FRAMES[22]); yield return Tick(2);
		SetTickRate(35);
		GoTo(IDLE_ANIMATION);
	}

	protected override Func<IEnumerator>[] GetStateList() {
		return new Func<IEnumerator>[] {
			Idle,
			Fire,
			Reload
		};
	}

}