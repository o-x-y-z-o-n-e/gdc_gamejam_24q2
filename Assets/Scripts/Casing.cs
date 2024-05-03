using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Billboard), typeof(Rigidbody), typeof(SphereCollider))]
public class Casing : MonoBehaviour {


	private const int SUSTAIN_STAGE = 0;
	private const int DECAY_STAGE = 1;


	[SerializeField] private float m_sustainTime;
	[SerializeField] private float m_decayTime;
	[Space]
	[SerializeField] private Sprite[] m_frames;
	[SerializeField] private float m_frameInterval;
	[SerializeField] private int m_groundFrameIndex;

	private Billboard m_billboard;
	private Rigidbody m_rigidbody;
	private SpriteRenderer m_spriteRenderer;

	private float m_timeCounter;
	private float m_frameCounter;
	private int m_frame = -1;
	private bool m_hitGround;
	private Vector3 m_direction = Vector3.up;
	private int m_stage = 0;
	private Color m_color;


	//----------------------------------------------------------------------------------------------------------

	
	protected virtual void Awake() {
		m_billboard = GetComponent<Billboard>();
		m_rigidbody = GetComponent<Rigidbody>();
		m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		m_color = m_spriteRenderer.color;
	}
	
	
	//----------------------------------------------------------------------------------------------------------
	
	
    protected virtual void Start() {
		if(m_frames.Length > 0 && m_frame < 0)
			SetFrame(0);

		m_timeCounter = m_sustainTime;
	}
	
	
	//----------------------------------------------------------------------------------------------------------
	
	
    protected virtual void Update() {
		if(Game.Instance.IsPaused)
			return;

		if(m_timeCounter > 0) {
			m_timeCounter -= Time.deltaTime;

			if(m_timeCounter <= 0) {
				if(m_stage == SUSTAIN_STAGE) {
					m_stage = DECAY_STAGE;
					m_timeCounter = m_decayTime;
				} else if(m_stage == DECAY_STAGE) {
					Destroy(gameObject);
					return;
				}
			}
		}

		if(m_stage == DECAY_STAGE) {
			float t = m_timeCounter / m_decayTime;
			m_color.a = t;
			m_spriteRenderer.color = m_color;
		}

		if(m_frames.Length > 1) {
			if(!m_hitGround || m_frame != m_groundFrameIndex) {
				m_frameCounter += Time.deltaTime;

				if(m_frameCounter > m_frameInterval) {
					m_frameCounter -= m_frameInterval;

					SetFrame(m_frame + 1);
				}
			}
		}
    }


	//----------------------------------------------------------------------------------------------------------


	public void SetDirection(Vector3 direction) => m_direction = direction;


	//----------------------------------------------------------------------------------------------------------


	public void SetSpeed(float speed) => m_rigidbody.velocity = m_direction * speed;


	//----------------------------------------------------------------------------------------------------------


	public void SetFrame(int index) {
		m_frame = index % m_frames.Length;
		m_billboard.SetSprite(m_frames[m_frame]);
	}


	//----------------------------------------------------------------------------------------------------------


	private void OnTriggerEnter(Collider other) {
		m_hitGround = true;
	}

}
