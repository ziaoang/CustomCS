using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DropType {
	BigAmmo,
	Helmet,
	MedicalBox,
	SmallAmmo
}

public class Drop : MonoBehaviour {

	public DropType m_dropType;

	public AudioClip m_sound;

	Transform m_transform;

	Player m_player;

	float m_range = 2.5f;
	
	AudioSource m_audioSource;

	void Start () {
		m_transform = this.transform;

		m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		m_audioSource = m_player.GetComponent<AudioSource> ();
	}

	void Update () {
		if (CanvasManager.Instance.GetGameState () == GameState.Pause)
			return;

		if (CanvasManager.Instance.GetGameState () == GameState.End)
			return;

		if (Input.GetKeyDown (KeyCode.G)) {
			Vector3 screenPoint = Camera.main.WorldToViewportPoint (m_transform.position);
			if (screenPoint.z > 0 && screenPoint.x > 0.25 && screenPoint.x < 0.75 && screenPoint.y > 0.25 && screenPoint.y < 0.75) {
				if (Vector3.Distance (m_transform.position, m_player.m_transform.position) < m_range) {

					m_audioSource.PlayOneShot (m_sound);

					Destroy (m_transform.gameObject);

					switch (m_dropType) {
					case DropType.BigAmmo:
						m_player.OnAmmo (200);
						break;
					case DropType.Helmet:
						m_player.OnHelmet (50);
						break;
					case DropType.MedicalBox:
						m_player.OnTreat (50);
						break;
					case DropType.SmallAmmo:
						m_player.OnAmmo (50);
						break;
					}
				}
			}
		}
	}
}
