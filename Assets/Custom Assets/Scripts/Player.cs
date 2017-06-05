using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using MiniJSON;

public class Player : MonoBehaviour {

	public Transform m_transform;

	public LayerMask m_layer;

	public AudioClip m_soundShoot;
	public AudioClip m_soundNoAmmo;
	public AudioClip m_soundReload;
	public AudioClip m_soundBeattack;
	public AudioClip m_soundDie;
	public AudioClip m_soundTimeover;

	public Transform m_fx;

	public int m_remain_time;

	public int m_attack;
	public int m_range;
	public int m_level;
	public int m_experience;

	public int m_max_level;
	public int m_max_experience;

	public int m_score;

	public int m_shield;
	public int m_max_shield;
	public int m_blood;
	public int m_max_blood;

	public int m_ammo;
	public int m_max_ammo;
	public int m_charger;
	public int m_max_charger;

	AudioSource m_audioSource;

	Transform m_cameraTransform;

	float timer = 1.0f;

	void Start () {
		m_transform = this.transform;

		m_audioSource = this.GetComponent<AudioSource> ();

		m_cameraTransform = Camera.main.transform;

		if (Share.m_is_save) {
			m_remain_time    = Share.m_remain_time;

			m_attack         = Share.m_attack;
			m_range          = Share.m_range;
			m_level          = Share.m_level;
			m_experience     = Share.m_experience;

			m_max_level      = Share.m_max_level;
			m_max_experience = Share.m_max_experience;

			m_score          = Share.m_score;

			m_shield         = Share.m_shield;
			m_max_shield     = Share.m_max_shield;
			m_blood          = Share.m_blood;
			m_max_blood      = Share.m_max_blood;

			m_ammo           = Share.m_ammo;
			m_max_ammo       = Share.m_max_ammo;
			m_charger        = Share.m_charger;
			m_max_charger    = Share.m_max_charger;
		}

		CanvasManager.Instance.SetAttack (m_attack);
		CanvasManager.Instance.SetRange (m_range);
		CanvasManager.Instance.SetLevel (m_level);
		CanvasManager.Instance.SetExperience (m_experience);
		CanvasManager.Instance.SetTime (m_remain_time);
		CanvasManager.Instance.SetScore (m_score);
		CanvasManager.Instance.SetShield (m_shield, m_max_shield);
		CanvasManager.Instance.SetBlood (m_blood, m_max_blood);
		CanvasManager.Instance.SetAmmo (m_ammo);
		CanvasManager.Instance.SetCharger (m_charger);
	}

	void Update () {
		if (CanvasManager.Instance.GetGameState () == GameState.Pause)
			return;
		
		if (CanvasManager.Instance.GetGameState () == GameState.End)
			return;

		if (m_remain_time > 0) {
			timer -= Time.deltaTime;
			if (timer < 0) {
				timer = 1.0f;

				m_remain_time--;
				CanvasManager.Instance.SetTime (m_remain_time);
				
				if (m_remain_time <= 0) {
					m_audioSource.PlayOneShot (m_soundTimeover);
					CanvasManager.Instance.EndGame (EndGameType.Timeout);
				}
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			if (m_ammo > 0) {
				m_ammo--;
				CanvasManager.Instance.SetAmmo (m_ammo);

				m_audioSource.PlayOneShot (m_soundShoot);

				RaycastHit info;
				Vector3 startPoint = m_cameraTransform.position;
				Vector3 direction = m_cameraTransform.TransformDirection (Vector3.forward);
				if (Physics.Raycast (startPoint, direction, out info, m_range, m_layer)) {
					if (info.transform.tag.CompareTo ("Enemy") == 0) {
						Instantiate (m_fx, info.point, Quaternion.identity);
						Enemy enemy = info.transform.GetComponent<Enemy> ();
						enemy.OnDamage (m_attack);
					} else if (info.transform.tag.CompareTo ("Boss") == 0) {
						Instantiate (m_fx, info.point, Quaternion.identity);
						Boss boss = info.transform.GetComponent<Boss> ();
						boss.OnDamage (m_attack);
					}
				}
			} else {
				m_audioSource.PlayOneShot (m_soundNoAmmo);
			}
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			if (m_ammo >= m_max_ammo)
				return;
			if (m_charger <= 0)
				return;
			
			m_audioSource.PlayOneShot (m_soundReload);

			int detal = m_max_ammo - m_ammo;
			m_ammo += Math.Min(detal, m_charger);
			m_charger -= Math.Min(detal, m_charger);
			CanvasManager.Instance.SetAmmo (m_ammo);
			CanvasManager.Instance.SetCharger (m_charger);
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

		if (m_blood <= 0) {
			m_audioSource.PlayOneShot (m_soundDie);
			CanvasManager.Instance.EndGame (EndGameType.Death);
		} else {
			m_audioSource.PlayOneShot (m_soundBeattack);
		}
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

	public void OnExperience(int experience) {
		if (m_level >= m_max_level)
			return;

		m_experience += experience;
		if (m_experience >= m_max_experience) {
			OnLevelUp ();
			if (m_level >= m_max_level) {
				m_experience = 0;
			} else {
				m_experience -= m_max_experience;
			}
		}

		CanvasManager.Instance.SetExperience (m_experience);
		CanvasManager.Instance.SetLevel (m_level);
	}

	void OnLevelUp() {
		if (m_level >= m_max_level)
			return;
		
		m_level++;

		m_attack += 10;
		m_range += 10;
		CanvasManager.Instance.SetAttack (m_attack);
		CanvasManager.Instance.SetRange (m_range);

		m_shield = m_shield * (m_max_shield + 10) / m_max_shield;
		m_max_shield += 10;
		m_shield = Math.Max (0, m_shield);
		m_shield = Math.Min (m_max_shield, m_shield);
		CanvasManager.Instance.SetShield (m_shield, m_max_shield);

		m_blood = m_blood * (m_max_blood + 10) / m_max_blood;
		m_max_blood += 10;
		m_blood = Math.Max (0, m_blood);
		m_blood = Math.Min (m_max_blood, m_blood);
		CanvasManager.Instance.SetBlood (m_blood, m_max_blood);

		m_max_ammo += 5;
		m_max_charger += 20;
	}
}
