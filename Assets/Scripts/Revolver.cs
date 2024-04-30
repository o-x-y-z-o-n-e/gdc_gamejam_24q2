using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : Weapon {

	//----------------------------------------------------------------------------------------------------------

	private const int EQUIP_ANIMATION = 0;
	private const int DEQUIP_ANIMATION = 1;
	private const int IDLE_ANIMATION = 2;
	private const int COCK_ANIMATION = 3;
	private const int FIRE_ANIMATION = 4;
	private const int OPEN_ANIMATION = 5;
	private const int LOAD_ANIMATION = 6;

	//----------------------------------------------------------------------------------------------------------

	// private Casing CASING_PREFAB;

	private Sprite ICON;

	private Frame[] EQUIP_FRAMES;
	private Frame[] IDLE_FRAMES;
	private Frame[] COCK_FRAMES;
	private Frame[] FIRE_FRAMES;
	private Frame[] OPEN_FRAMES;
	private Frame[] LOAD_FRAMES;

	private AudioClip EQUIP_SOUND;
	private AudioClip[] RELOAD_SOUNDS;

	private AudioSource m_fireAudio;

	private bool m_isCocked = true;

	//----------------------------------------------------------------------------------------------------------

	protected override void Awake() {
		base.Awake();

		// Prefabs.
		// CASING_PREFAB = Resources.Load<Casing>("Prefabs/Casings/Magnum");


		// Sprites.
		string root = "Revolver/";

		ICON = Resources.Load<Sprite>(root+"Extra/icon");

		EQUIP_FRAMES = new Frame[] {
			new Frame(root+"Equip/equip_0", 0, 0),
			new Frame(root+"Equip/equip_1", 0, 0),
			new Frame(root+"Equip/equip_2", 0, 0),
			new Frame(root+"Equip/equip_3", 0, 0),
		};

		IDLE_FRAMES = new Frame[] {
			new Frame(root+"Idle/idle_0", 0, 0),
			new Frame(root+"Idle/idle_1", 0, 0),
		};

		COCK_FRAMES = new Frame[8];
		for(int i = 0; i < COCK_FRAMES.Length; i++)
			COCK_FRAMES[i] = new Frame(root + "Cock/cock_" + i);

		FIRE_FRAMES = new Frame[19];
		for(int i = 0; i < FIRE_FRAMES.Length; i++)
			FIRE_FRAMES[i] = new Frame(root + "Fire/fire_" + i);

		OPEN_FRAMES = new Frame[11];
		for(int i = 0; i < OPEN_FRAMES.Length; i++)
			OPEN_FRAMES[i] = new Frame(root + "Open/open_" + i);

		LOAD_FRAMES = new Frame[23];
		for(int i = 0; i < LOAD_FRAMES.Length; i++)
			LOAD_FRAMES[i] = new Frame(root + "Load/load_" + i);


		// Sounds.
		EQUIP_SOUND = Resources.Load<AudioClip>("Audio/Items/Misc/weapon_equip_2");

		m_fireAudio = CreateAudioSource("Fire");
		m_fireAudio.clip = Resources.Load<AudioClip>("Audio/Items/Revolver/revolver_fire_1");

		RELOAD_SOUNDS = new AudioClip[3];
		for(int i = 0; i < RELOAD_SOUNDS.Length; i++)
			RELOAD_SOUNDS[i] = Resources.Load<AudioClip>("Audio/Items/Revolver/revolver_reload_" + (i+1));

		SetVerticalSpriteClamp(4);
	}

	//----------------------------------------------------------------------------------------------------------

	private void OnEnable() {
		GoTo(EQUIP_ANIMATION);
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerable Idle() {
		SetFrame(IDLE_FRAMES[m_isCocked ? 1 : 0]);

		while(true) {
			yield return new WaitForFixedUpdate();

			if(CheckReload()) {
				GoTo(OPEN_ANIMATION);
				yield return null;
			}

			if(CheckAttack()) {
				if(m_isCocked) {
					GoTo(FIRE_ANIMATION);
					yield return null;
				} else {
					GoTo(COCK_ANIMATION);
					yield return null;
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerable Fire() {
		SetTickRate(45);
		SendAttackRaycast();
		RemoveAmmo(1);

		// SetPitch(m_fireAudio, 1, 0.15F);
		// m_fireAudio.Play();
		// PlaySound(FIRE_SOUNDS[Random.Range(0, 3)]);
		// SendNoiseAlert();

		for(int i = 0; i < 3; i++) {
			SetFrame(FIRE_FRAMES[i]);
			yield return Tick(1);
		}

		// Player.Recoil(5, 5);

		for(int i = 3; i < 19; i++) {
			SetFrame(FIRE_FRAMES[i]);
			yield return Tick(1);
		}

		SetFrame(IDLE_FRAMES[0]);

		yield return Tick(1);

		m_isCocked = false;

		SetTickRate(35);

		if(Mag > 0)
			GoTo(COCK_ANIMATION);
		else
			GoTo(IDLE_ANIMATION);
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerable Equip() {
		m_isCocked = true;

		// PlaySound(EQUIP_SOUND);

		for(int i = 0; i < 4; i++) {
			SetFrame(EQUIP_FRAMES[i]);
			yield return Tick(1);
		}

		GoTo(IDLE_ANIMATION);
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerable Dequip() {
		for(int i = 3; i >= 0; i--) {
			SetFrame(EQUIP_FRAMES[i]);
			yield return Tick(1);
		}

		// Player.EquipQueuedItem();
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerable Cock() {
		// Cock hammer.
		for(int i = 0; i < 8; i++) {
			SetFrame(COCK_FRAMES[i]);

			yield return Tick(1);
		}

		m_isCocked = true;

		GoTo(IDLE_ANIMATION);
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerable Open() {
		// Uncock hammer.
		if(m_isCocked) {
			for(int i = 7; i >= 0; i--) {
				SetFrame(COCK_FRAMES[i]);
				yield return Tick(1);
			}

			SetFrame(IDLE_FRAMES[0]);
			yield return Tick(5);
		}

		// Open housing.
		for(int i = 0; i < 9; i++) {
			SetFrame(OPEN_FRAMES[i]);

			yield return Tick(1);
		}

		// Spawn casings.
		Vector3 center = new Vector3(0.2F, -0.7F, 1);
		for(int i = 0; i < 6; i++) {
			float t = (i / 3F) * Mathf.PI;

			Vector3 offset = new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0) * 0.05F;

			/*
			SpawnCasing(
				CASING_PREFAB,
				center+offset,
				new Vector3(Mathf.Cos(t), 0, Mathf.Sin(t)),
				0.25F,
				6
			);
			*/
		}

		SetFrame(OPEN_FRAMES[9]);
		yield return Tick(1);

		SetFrame(OPEN_FRAMES[10]);
		yield return Tick(1);


		yield return Tick(11);

		GoTo(LOAD_ANIMATION);
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerable Load() {
		for(int i = 0; i < 4; i++) {
			SetFrame(LOAD_FRAMES[i]);
			yield return Tick(1);
		}

		// PlaySound(RELOAD_SOUNDS[1]);

		for(int i = 4; i < 12; i++) {
			SetFrame(LOAD_FRAMES[i]);
			yield return Tick(1);
		}

		SetFrame(LOAD_FRAMES[12]);
		yield return Tick(6);

		for(int i = 13; i < 17; i++) {
			SetFrame(LOAD_FRAMES[i]);
			yield return Tick(1);
		}

		LoadMag();
		// PlaySound(RELOAD_SOUNDS[2]);

		for(int i = 17; i < 23; i++) {
			SetFrame(LOAD_FRAMES[i]);
			yield return Tick(1);
		}

		GoTo(COCK_ANIMATION);
	}

	//----------------------------------------------------------------------------------------------------------

	protected override Func<IEnumerable>[] GetStateList() {
		return new Func<IEnumerable>[] {
			Equip,
			Dequip,
			Idle,
			Cock,
			Fire,
			Open,
			Load
		};
	}

	//----------------------------------------------------------------------------------------------------------

	public override void Holster() => GoTo(DEQUIP_ANIMATION);

	//----------------------------------------------------------------------------------------------------------

	public override bool CanDequip() => CurrentState == IDLE_ANIMATION;

	//----------------------------------------------------------------------------------------------------------

	public override string GetName() => "Revolver";

	//----------------------------------------------------------------------------------------------------------

	public override Sprite GetHUDIcon() => ICON;

	//----------------------------------------------------------------------------------------------------------

}