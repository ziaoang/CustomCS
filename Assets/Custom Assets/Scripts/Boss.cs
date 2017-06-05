using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossState {
	idle,
	run,
	attack,
	death
}

public class Boss : MonoBehaviour {

	public Transform m_drop1;
	public Transform m_drop2;
	public Transform m_drop3;
	public Transform m_drop4;

	Transform m_transform;

	NavMeshAgent m_agent;

	Player m_player;

	Animation m_animation;

	BossState m_bossState = BossState.idle;

	int m_life = 2000;
	int m_damage = 50;
	int m_score = 2000;
	int m_experience = 1000;

	float m_range = 2f;
	float m_rotSpeed = 10.0f;

	float m_cdTimer = 0;
	float m_waitTimer = 0.1f;

	void Awake () {
		m_transform = this.transform;

		m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		m_agent = this.GetComponent<NavMeshAgent> ();

		m_animation = this.GetComponent<Animation>();
	}

	void Update () {
		if (CanvasManager.Instance.GetGameState () == GameState.Pause)
			return;

		if (CanvasManager.Instance.GetGameState () == GameState.End)
			return;

		m_cdTimer -= Time.deltaTime;
		if (m_cdTimer > 0)
			return;
		m_cdTimer = m_waitTimer;

		// 待机状态
		if (m_bossState == BossState.idle) {
			if (Vector3.Distance (m_transform.position, m_player.m_transform.position) < m_range) {
				m_agent.isStopped = true;
				m_bossState = BossState.attack;
				m_animation.wrapMode= WrapMode.Once;
				m_animation.CrossFade("hit");
			} else {
				m_agent.isStopped = false;
				m_agent.SetDestination (m_player.m_transform.position);
				m_bossState = BossState.run;
				m_animation.wrapMode= WrapMode.Loop;
				m_animation.CrossFade("walk");
			}
		}

		// 跑步状态
		if (m_bossState == BossState.run) {
			if (Vector3.Distance (m_transform.position, m_player.m_transform.position) < m_range) {
				m_agent.isStopped = true;
				m_bossState = BossState.attack;
				m_animation.wrapMode= WrapMode.Once;
				m_animation.CrossFade("hit");
			} else {
				m_agent.isStopped = false;
				m_agent.SetDestination (m_player.m_transform.position);
			}
		}

		// 攻击状态
		if (m_bossState == BossState.attack) {
			RotateToUser ();
			m_agent.isStopped = true;
			if (!m_animation.isPlaying) {
				m_bossState = BossState.idle;
				m_animation.wrapMode= WrapMode.Loop;
				m_animation.CrossFade("idle");
				m_player.OnDamage (m_damage);
			}
		}

		// 死亡状态
		if (m_bossState == BossState.death) {
			m_agent.isStopped = true;
			if (!m_animation.isPlaying) {
				Destroy (this.gameObject);

				m_player.OnScore (m_score);
				m_player.OnExperience (m_experience);

				Instantiate (m_drop1, m_transform.position, Quaternion.identity);
				Instantiate (m_drop2, m_transform.position, Quaternion.identity);
				Instantiate (m_drop3, m_transform.position, Quaternion.identity);
				Instantiate (m_drop4, m_transform.position, Quaternion.identity);
			}
		}
	}

	void RotateToUser() {
		Vector3 targetdir = m_player.m_transform.position - m_transform.position;
		Vector3 newdir = Vector3.RotateTowards (m_transform.forward, targetdir, m_rotSpeed * Time.deltaTime, 0.0f);
		m_transform.rotation = Quaternion.LookRotation (newdir);
	}

	public void OnDamage(int damage) {
		if (m_life <= 0)
			return;

		m_life -= damage;

		if (m_life <= 0) {
			m_bossState = BossState.death;
			m_animation.wrapMode= WrapMode.Once;
			m_animation.CrossFade("die");
		}
	}

	public void SetLevel(int level) {
		m_life *= level;
		m_damage += level * 20;
		m_score *= level;
		m_experience *= level;
		m_agent.speed += level;
	}
}
