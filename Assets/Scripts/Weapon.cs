using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

	//----------------------------------------------------------------------------------------------------------

	private const float CASING_EJECT_VARIANCE = 0.5F;

	//----------------------------------------------------------------------------------------------------------

	public bool UsesAmmo => m_ammoUsage > 0;
	public bool UsesMags => m_magCapacity > 0;
	public string AmmoType => m_ammoType;
	public int Mag => m_mag;
	public int Ammo => Player.Ammo.Get(m_ammoType);

	//----------------------------------------------------------------------------------------------------------

	[SerializeField] private string m_ammoType;
	[SerializeField] private int m_noiseAlert = 15;
	[SerializeField] private int m_magCapacity = 12;
	[SerializeField] private int m_ammoUsage = 1;
	[SerializeField] private int m_damage = 10;
	[SerializeField] private float m_accuracy = 0.5F;
	[SerializeField] private float m_attackRange = 100;
	[SerializeField] private GameObject m_defaultImpactHoleParticle;
	[SerializeField] private bool m_overrideActorImpactParticle;
	[SerializeField] private AttackType m_attackType;

	//----------------------------------------------------------------------------------------------------------

	private int m_mag;
	private LayerMask m_attackMask;

	//----------------------------------------------------------------------------------------------------------

	protected override void Awake() {
		base.Awake();
		m_attackMask = LayerMask.GetMask("Default", "Actor");
	}

	//----------------------------------------------------------------------------------------------------------

	protected override void Start() {
		base.Start();
	}

	//----------------------------------------------------------------------------------------------------------

	protected bool CheckAttack() {
		int amt = GetUsableAmmo();
		if(amt < m_ammoUsage)
			return false;

		return GetPrimaryUseInput();
	}

	//----------------------------------------------------------------------------------------------------------

	protected bool CheckReload() {
		if(!UsesMags || m_mag >= m_magCapacity)
			return false;

		if(Ammo <= 0)
			return false;

		return GetReloadInput();
	}

	//----------------------------------------------------------------------------------------------------------

	private int GetUsableAmmo() => m_magCapacity > 0 ? m_mag : Player.Ammo.Get(m_ammoType);

	//----------------------------------------------------------------------------------------------------------

	protected bool GetReloadInput() => Input.GetKey(KeyCode.R);

	//----------------------------------------------------------------------------------------------------------

	protected void SendAttackRaycast() => SendAttackRaycast(m_damage, GetAccuracyOffset());

	//----------------------------------------------------------------------------------------------------------

	protected void SendAttackRaycast(float damage) => SendAttackRaycast(damage, GetAccuracyOffset());

	//----------------------------------------------------------------------------------------------------------

	protected void SendAttackRaycast(float damage, Vector2 angularOffset) {
		Transform cam = Player.Camera.transform;
		Vector3 directionOffset = Quaternion.Euler(angularOffset) * Vector3.forward;
		Vector3 forward = cam.TransformDirection(directionOffset);

		RaycastHit hit;
		Ray ray = new Ray(cam.position, forward);
		if(Physics.Raycast(ray, out hit, m_attackRange, m_attackMask)) {
			GameObject impactPrefab = null;

			//Damageable target;
			//if(hit.collider.TryGetComponent(out target)) {
			//	if(m_overrideActorImpactParticle)
			//		impactPrefab = m_defaultImpactHoleParticle;
			//	else
			//		impactPrefab = target.GetImpactParticle();
			//
			//	target.ApplyDamage(Player, m_damage);
			//} else {
			//	impactPrefab = m_defaultImpactHoleParticle;
			//}

			if(impactPrefab != null)
				SpawnImpactParticle(impactPrefab, hit);
		}

	}

	//----------------------------------------------------------------------------------------------------------

	protected void SendNoiseAlert() {
		Vector3 position = Player.transform.position;
		Rect rect = new Rect(
			position.x - m_noiseAlert,
			position.y - m_noiseAlert,
			position.x + m_noiseAlert,
			position.y + m_noiseAlert
		);

		// List<Enemy> enemies = Game.Context.GetActors<Enemy>(rect);
		// 
		// foreach(Enemy enemy in enemies) {
		// 	enemy.Alert(Player);
		// }
	}

	//----------------------------------------------------------------------------------------------------------

	protected void LoadMag() {
		if(!UsesAmmo)
			return;

		int want = m_magCapacity - m_mag;
		int ammo = Ammo;

		if(want > ammo) {
			Player.Ammo.Set(m_ammoType, 0);
			want = ammo;
		} else {
			Player.Ammo.Take(m_ammoType, want);
		}

		m_mag += want;

		// UI.GetMenu<HUD>().UpdateItemText(this);
	}

	//----------------------------------------------------------------------------------------------------------

	protected void RemoveAmmo(int amount) => OffsetAmmo(-Mathf.Abs(amount));

	//----------------------------------------------------------------------------------------------------------

	protected void OffsetAmmo(int delta) {
		if(UsesMags) {
			m_mag += delta;

			if(m_mag < 0)
				m_mag = 0;

			if(m_mag > m_magCapacity)
				m_mag = m_magCapacity;

			// UI.GetMenu<HUD>().UpdateItemText(this);
		} else {
			// Player.Ammo.OffsetAmount(m_ammoType, delta);
			// UI is already updated.
		}
	}

	//----------------------------------------------------------------------------------------------------------

	private void SpawnImpactParticle(GameObject prefab, RaycastHit hit) {
		const float IMPACT_PARTICLE_EXTRUDE = 0.05F;

		Vector3 position = hit.point + (hit.normal * IMPACT_PARTICLE_EXTRUDE);
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

		// Game.Context.Spawn(prefab, position, rotation, SpawnCategory.Particle);
	}

	//----------------------------------------------------------------------------------------------------------

	public enum AttackType {
		Raycast,
		Projectile
	}

	//----------------------------------------------------------------------------------------------------------

	protected Vector2 GetAccuracyOffset() {
		float ax = 0.0F; // Util.BoxMuller(0, m_accuracy);
		float ay = 0.0F; // Util.BoxMuller(0, m_accuracy);

		return new Vector2(ax, ay);
	}

	//----------------------------------------------------------------------------------------------------------

	public override string GetHUDText() {
		string text = GetName();

		if(UsesAmmo) {
			int ammo = Player.Ammo.Get(m_ammoType);
			text += ": [" + ammo + "]";
		}

		if(UsesMags) {
			text += " (" + m_mag + ")";
		}

		return text;
	}

	//----------------------------------------------------------------------------------------------------------

	/*
	protected Casing SpawnCasing(Casing prefab, Vector3 offset, Vector3 direction, float speed, int startingFrame) {
		Transform cam = Player.GetCamera().transform;

		direction.x += Random.Range(-CASING_EJECT_VARIANCE, CASING_EJECT_VARIANCE);
		direction.y += Random.Range(-CASING_EJECT_VARIANCE, CASING_EJECT_VARIANCE);
		direction.z += Random.Range(-CASING_EJECT_VARIANCE, CASING_EJECT_VARIANCE);

		Vector3 pos = cam.TransformPoint(offset);
		Vector3 dir = cam.TransformDirection(direction.normalized);

		RaycastHit hit;
		Vector3 origin = Player.GetCamera().transform.parent.position;
		if(Physics.Linecast(origin, pos, out hit, LayerMask.GetMask("Default"))) {
			Vector3 diff = origin - hit.point;

			pos = hit.point + (diff.normalized * 0.1F);
		}

		Casing instance = Game.Context.Spawn(prefab, pos, Quaternion.identity, SpawnCategory.Particle);
		instance.SetFrame(startingFrame);
		instance.SetDirection(dir);
		instance.SetSpeed(speed);

		return instance;
	}
	*/

	//----------------------------------------------------------------------------------------------------------

}