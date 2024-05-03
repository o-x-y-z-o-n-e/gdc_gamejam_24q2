using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Item : MonoBehaviour {

	//----------------------------------------------------------------------------------------------------------

	private const int DEFAULT_TICK_RATE = 35;

	private const float SWAY_MAX_X = 0.1F;
	private const float SWAY_MAX_Y = 0.05F;
	private const float SWAY_SCALE_X = 0.001F;
	private const float SWAY_SCALE_Y = 0.001F;
	private const float DROP_SCALE = 0.8F;
	private const float JUMP_LAG_SCALE = 0.075F;
	private const float JUMP_LAG_MAX = 0.5F;

	//----------------------------------------------------------------------------------------------------------

	public Player Player => player;
	public bool Has => has;
	protected int CurrentState => currentStateIndex;

	//----------------------------------------------------------------------------------------------------------

	[SerializeField] private bool has = false;

	//----------------------------------------------------------------------------------------------------------

	private int tickRate = DEFAULT_TICK_RATE;
	private SpriteRenderer spriteRenderer;
	private AudioSource audioSource;
	private Player player;
	private Func<IEnumerator>[] states;
	private int currentStateIndex = -1;
	private Coroutine currentState;
	private Vector2 spriteOffset;
	private Vector2 bobStrength = new Vector2(0.1F, 0.04F);
	private Vector2 bob;
	private int verticalClamp = 100;

	//----------------------------------------------------------------------------------------------------------

	protected virtual void Awake() {
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();
		player = GetComponentInParent<Player>();
		states = GetStateList();
	}

	//----------------------------------------------------------------------------------------------------------

	protected virtual void Start() {

	}

	//----------------------------------------------------------------------------------------------------------

	protected virtual void OnDisable() {
		StopCurrentState();
	}

	protected virtual void OnEnable() {
		
	}

	//----------------------------------------------------------------------------------------------------------

	private void Update() {
		if(Game.Instance.IsPaused) return;

		UpdatePosition();
	}

	//----------------------------------------------------------------------------------------------------------

	public virtual void Holster() { } // => player.EquipQueuedItem();

	//----------------------------------------------------------------------------------------------------------

	protected virtual Func<IEnumerator>[] GetStateList() => new Func<IEnumerator>[0];

	//----------------------------------------------------------------------------------------------------------

	protected void GoTo(int state) {
		if (state < 0 || state >= states.Length)
			return;

		StopCurrentState();

		currentStateIndex = state;
		currentState = StartCoroutine(states[state].Invoke());
	}

	//----------------------------------------------------------------------------------------------------------

	private void StopCurrentState() {
		if(currentStateIndex < 0 || currentStateIndex >= states.Length)
			return;

		StopCoroutine(currentState);
		currentState = null;
		currentStateIndex = -1;
	}

	//----------------------------------------------------------------------------------------------------------

	protected void SetFrame(Frame frame) {
		SetSprite(frame.Sprite);
		SetOffset(frame.X, frame.Y);
	}

	//----------------------------------------------------------------------------------------------------------

	public virtual string GetName() => "Unknown Item";

	//----------------------------------------------------------------------------------------------------------

	public virtual string GetHUDText() => GetName();

	//----------------------------------------------------------------------------------------------------------

	public virtual Sprite GetHUDIcon() => null;

	//----------------------------------------------------------------------------------------------------------

	protected void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;

	//----------------------------------------------------------------------------------------------------------

	protected bool GetPrimaryUseInput() => Input.GetKey(KeyCode.Mouse0);

	//----------------------------------------------------------------------------------------------------------

	protected bool GetSecondaryUseInput() => Input.GetKey(KeyCode.Mouse1);

	//----------------------------------------------------------------------------------------------------------

	protected void SetOffset(float x, float y) {
		spriteOffset.x = PixelToUnit(x);
		spriteOffset.y = PixelToUnit(y);
		// spriteRenderer.transform.localPosition = spriteOffset;
	}

	//----------------------------------------------------------------------------------------------------------

	private void UpdatePosition() {
		Vector2 position = Vector2.zero;

		position += GetBob();
		position += GetSway();
		position += GetDrop();
		position += GetJumpLag();
		position += GetGlide();

		float clamp = PixelToUnit(verticalClamp);
		if(position.y > clamp)
			position.y = clamp;

		transform.localPosition = position;
	}

	//----------------------------------------------------------------------------------------------------------

	private Vector2 GetBob() {
		bob.x = Mathf.Sin(player.BobCounter) * bobStrength.x;
		bob.y = (Mathf.Cos(player.BobCounter * 2) * bobStrength.y) - bobStrength.y;
		return bob * player.BobMoveMultiplier;
	}

	//----------------------------------------------------------------------------------------------------------

	private Vector2 GetSway() {
		// Vector2 rotate = player.RotateVelocity;

		// m_sway_x.Target = Mathf.Clamp(-rotate.y * SWAY_SCALE_X, -SWAY_MAX_X, SWAY_MAX_X);
		// m_sway_y.Target = Mathf.Clamp(rotate.x * SWAY_SCALE_Y, -SWAY_MAX_Y, SWAY_MAX_Y);

		// m_sway_x.Step();
		// m_sway_y.Step();

		return Vector2.zero; // new Vector2(m_sway_x.Value, m_sway_y.Value);
	}

	//----------------------------------------------------------------------------------------------------------

	private Vector2 GetDrop() {
		return new Vector2(0, player.CameraDrift * DROP_SCALE);
	}

	//----------------------------------------------------------------------------------------------------------

	private Vector2 GetJumpLag() {
		return Vector2.zero;// new Vector2(0, Mathf.Clamp(-player.JumpLag * JUMP_LAG_SCALE, -JUMP_LAG_MAX, 0));
	}

	//----------------------------------------------------------------------------------------------------------

	private Vector2 GetGlide() {
		// m_glide_x.Step();
		// m_glide_y.Step();

		return Vector2.zero;// new Vector2(m_glide_x.Value, m_glide_y.Value);
	}

	//----------------------------------------------------------------------------------------------------------

	protected WaitForSeconds Tick(int ticks) => new WaitForSeconds(TickToInterval(ticks));

	//----------------------------------------------------------------------------------------------------------

	protected Coroutine Lerp(float from_x, float from_y, float to_x, float to_y, int ticks) {
		return StartCoroutine(SmoothLerp(from_x, from_y, to_x, to_y, ticks));
	}

	//----------------------------------------------------------------------------------------------------------

	protected Coroutine Lerp(Vector2 from, Vector2 to, int ticks) {
		return StartCoroutine(SmoothLerp(from.x, from.y, to.x, to.y, ticks));
	}

	//----------------------------------------------------------------------------------------------------------

	private IEnumerator SmoothLerp(float from_x, float from_y, float to_x, float to_y, int ticks) {
		float t = 0;
		float T = TickToInterval(ticks);
		while(t < 1) {
			yield return new WaitForEndOfFrame();

			t += Time.deltaTime / T;

			SetOffset(
				Mathf.Lerp(from_x, to_x, t),
				Mathf.Lerp(from_y, to_y, t)
			);
		}
		yield return null;
	}

	//----------------------------------------------------------------------------------------------------------

	protected float TickToInterval(int ticks) => ticks / (float)tickRate;

	//----------------------------------------------------------------------------------------------------------

	public virtual bool CanDequip() => true;

	//----------------------------------------------------------------------------------------------------------

	private float PixelToUnit(float value) => value / 100.0F;

	//----------------------------------------------------------------------------------------------------------

	protected void PlaySound(AudioClip clip) => audioSource.PlayOneShot(clip);

	//----------------------------------------------------------------------------------------------------------

	protected void SetTickRate(int tps) => tickRate = Mathf.Clamp(tps, 30, 200);

	//----------------------------------------------------------------------------------------------------------

	protected void SetVerticalSpriteClamp(int clamp) => verticalClamp = clamp;

	//------------------------------------------------------------------------------------------------------

	public void SetHas() => has = true;

	//------------------------------------------------------------------------------------------------------

	protected void SetPitch(AudioSource source, float mean, float variance) {
		source.pitch = mean + UnityEngine.Random.Range(-variance, variance);
	}

	//----------------------------------------------------------------------------------------------------------

	protected AudioSource CreateAudioSource(string name) {
		GameObject obj = new GameObject(name + " [Audio]");
		obj.transform.SetParent(transform);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		AudioSource source = obj.AddComponent<AudioSource>();
		source.playOnAwake = false;
		return source;
	}

	//----------------------------------------------------------------------------------------------------------

	protected class Frame {

		//------------------------------------------------------------------------------------------------------

		public Sprite Sprite => m_sprite;
		public int X => m_xOffset;
		public int Y => m_yOffset;

		//------------------------------------------------------------------------------------------------------

		private Sprite m_sprite;
		private int m_xOffset;
		private int m_yOffset;

		//------------------------------------------------------------------------------------------------------

		public Frame(Sprite sprite) {
			m_sprite = sprite;
			m_xOffset = 0;
			m_yOffset = 0;
		}

		//------------------------------------------------------------------------------------------------------

		public Frame(Sprite sprite, int x, int y) {
			m_sprite = sprite;
			m_xOffset = x;
			m_yOffset = y;
		}

		//------------------------------------------------------------------------------------------------------

		public Frame(string sprite) {
			m_sprite = Resources.Load<Sprite>(sprite);
			m_xOffset = 0;
			m_yOffset = 0;
		}

		//------------------------------------------------------------------------------------------------------

		public Frame(string sprite, int x, int y) {
			m_sprite = Resources.Load<Sprite>(sprite);
			m_xOffset = x;
			m_yOffset = y;
		}

		//------------------------------------------------------------------------------------------------------

	}

	//----------------------------------------------------------------------------------------------------------

}