using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour {

	public Transform m_transform;

	public LayerMask m_layer;

	public AudioClip m_audioClip;

	public Transform m_fx;

	public int m_attack = 50;
	public int m_range = 100;
	public int m_level = 1;
	public int m_experience = 0;

	public int m_score = 0;

	public int m_shield = 100;
	public int m_max_shield = 100;
	public int m_blood = 100;
	public int m_max_blood = 100;

	public int m_ammo = 30;
	public int m_max_ammo = 30;
	public int m_charger = 90;
	public int m_max_charger = 90;

	float m_cdTimer = 0;
	float m_waitTimer = 0.1f;

	AudioSource m_audioSource;

	Transform m_cameraTransform;

	void Start () {
		m_transform = this.transform;

		m_audioSource = this.GetComponent<AudioSource> ();

		m_cameraTransform = Camera.main.transform;
	}

	void Update () {
		m_cdTimer -= Time.deltaTime;
		if (m_ammo > 0 && m_cdTimer <= 0 && Input.GetMouseButton (0)) {
			m_ammo--;
			CanvasManager.Instance.SetAmmo (m_ammo);

			m_cdTimer = m_waitTimer;
			m_audioSource.PlayOneShot (m_audioClip);

			RaycastHit info;
			Vector3 startPoint = m_cameraTransform.position;
			Vector3 direction = m_cameraTransform.TransformDirection (Vector3.forward);
			if (Physics.Raycast (startPoint, direction, out info, m_range, m_layer)) {
				if (info.transform.tag.CompareTo ("Enemy") == 0) {
					Instantiate (m_fx, info.point, Quaternion.identity);
					Enemy enemy = info.transform.GetComponent<Enemy> ();
					enemy.OnDamage (m_attack);
				}
			}
		}

		if (Input.GetKeyUp (KeyCode.R)) {
			if (m_ammo >= m_max_ammo)
				return;
			if (m_charger <= 0)
				return;
			int detal = m_max_ammo - m_ammo;
			m_ammo += Math.Min(detal, m_charger);
			m_charger -= Math.Min(detal, m_charger);
			CanvasManager.Instance.SetAmmo (m_ammo);
			CanvasManager.Instance.SetCharger (m_charger);
		}

		if (Input.GetKeyUp (KeyCode.G)) {
			RaycastHit info;
			Vector3 startPoint = m_cameraTransform.position;
			Vector3 direction = m_cameraTransform.TransformDirection (Vector3.forward);
			if (Physics.Raycast (startPoint, direction, out info, m_range, m_layer)) {
				if (info.transform.tag.CompareTo ("Drop_BigAmmo") == 0) {
					Destroy (info.transform.gameObject);
					OnAmmo (200);
				}
				else if (info.transform.tag.CompareTo ("Drop_Helmet") == 0) {
					Destroy (info.transform.gameObject);
					OnHelmet (50);
				}
				else if (info.transform.tag.CompareTo ("Drop_MedicalBox") == 0) {
					Destroy (info.transform.gameObject);
					OnTreat (50);
				}
				else if (info.transform.tag.CompareTo ("Drop_SmallAmmo") == 0) {
					Destroy (info.transform.gameObject);
					OnAmmo (50);
				}
			}
		}
	}

	public void OnDamage(int damage) {
		if (m_blood <= 0)
			return;

		m_shield -= damage;
		if (m_shield < 0) {
			m_blood += m_shield;
			m_shield = 0;
			if (m_blood < 0) {
				m_blood = 0;
			}
		}

		CanvasManager.Instance.SetShield (m_shield, m_max_shield);
		CanvasManager.Instance.SetBlood (m_blood, m_max_blood);
	}

	public void OnScore(int score) {
		m_score += score;
		CanvasManager.Instance.SetScore (m_score);
	}

	public void OnAmmo(int ammo) {
		m_charger = Math.Min (m_charger + ammo, m_max_charger);
		CanvasManager.Instance.SetCharger (m_charger);
	}

	public void OnTreat(int blood) {
		m_blood = Math.Min (m_blood + blood, m_max_blood);
		CanvasManager.Instance.SetBlood (m_blood, m_max_blood);
	}

	public void OnHelmet(int shield) {
		m_shield = Math.Min (m_shield + shield, m_max_shield);
		CanvasManager.Instance.SetShield (m_shield, m_max_shield);
	}
}
