using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Transform m_transform;

	public LayerMask m_layer;

	public AudioClip m_audioClip;

	public Transform m_fx;

	public int m_life = 100;

	public int m_bullet = 60;

	public int m_score = 0;

	public int m_range = 100;

	public int m_damage = 30;

	float m_shootTimer = 0;

	AudioSource m_audioSource;

	Transform m_cameraTransform;

	void Start () {
		m_transform = this.transform;

		m_audioSource = this.GetComponent<AudioSource> ();

		m_cameraTransform = Camera.main.transform;
	}

	void Update () {

		m_shootTimer -= Time.deltaTime;
		if (Input.GetMouseButton (0) && m_shootTimer <= 0) {
			m_shootTimer = 0.1f;

			m_audioSource.PlayOneShot (m_audioClip);

			m_bullet--;

			CanvasManager.Instance.SetBullet (m_bullet);

			RaycastHit info;

			Vector3 startPoint = m_cameraTransform.position;
			Vector3 direction = m_cameraTransform.TransformDirection (Vector3.forward);

			if (Physics.Raycast (startPoint, direction, out info, m_range, m_layer)) {

				Instantiate (m_fx, info.point, Quaternion.identity);

				if (info.transform.tag.CompareTo ("Enemy") == 0) {
					Enemy enemy = info.transform.GetComponent<Enemy> ();
					enemy.OnDamage (m_damage);
				}
				if (info.transform.tag.CompareTo ("Environment") == 0) {
					
				}
			}
		}
	}

	public void OnDamage(int damage) {
		m_life -= damage;

		CanvasManager.Instance.SetLife (m_life);
	}

	public void OnScore(int score) {
		m_score += score;

		CanvasManager.Instance.SetScore (m_score);
	}
}
