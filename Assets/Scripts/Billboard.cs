using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	[SerializeField] private bool m_verticalAlign;
	
	private Transform m_pivot;
	private SpriteRenderer m_renderer;


	//----------------------------------------------------------------------------------------------------------


	private void Awake() {
		if(transform.childCount > 0) {
			m_pivot = transform.Find("Pivot");
			if(m_pivot == null)
				m_pivot = transform.GetChild(0);

			if(m_pivot != null)
				m_renderer = m_pivot.GetComponentInChildren<SpriteRenderer>();
		}
	}


	//----------------------------------------------------------------------------------------------------------


	private void LateUpdate() {
		if(m_renderer == null)
			return;

		Camera targetCamera = Game.Instance.Player.Camera;
		if(targetCamera == null)
			return;

		Vector3 target = targetCamera.transform.position;
		Vector3 origin = m_pivot.position;

		Vector3 diff = target - origin;

		float y = Mathf.Atan2(diff.x, diff.z)*Mathf.Rad2Deg + 180;
		float x = 0;

		if(m_verticalAlign) {
			float diff_z = Mathf.Sqrt((diff.x * diff.x) + (diff.z * diff.z));
			x = Mathf.Atan2(diff.y, diff_z)*Mathf.Rad2Deg;
		}

		m_pivot.eulerAngles = new Vector3(x, y, 0);
	}


	//----------------------------------------------------------------------------------------------------------


	public void SetSprite(Sprite sprite) => m_renderer.sprite = sprite;


}
