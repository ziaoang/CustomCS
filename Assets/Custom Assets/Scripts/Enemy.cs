using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

	public Transform m_drop1;
	public Transform m_drop2;
	public Transform m_drop3;
	public Transform m_drop4;

	Transform m_transform;

	Animator m_animator;

	NavMeshAgent m_agent;

	Player m_player;

	EnemySpawn m_spawn;

	int m_life = 100;
	int m_damage = 20;
	int m_score = 100;
	int m_experience = 100;

	float m_range = 1.5f;
	float m_rotSpeed = 10.0f;

	float m_cdTimer = 0;
	float m_waitTimer = 0.1f;

	void Start () {
		m_transform = this.transform;

		m_animator = this.GetComponent<Animator> ();

		m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		m_agent = GetComponent<NavMeshAgent> ();
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

		// 过度状态
		if (m_animator.IsInTransition (0)) {
			return;
		}

		AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo (0);

		// 待机状态
		if (stateInfo.IsName ("idle")) {
			if (Vector3.Distance (m_transform.position, m_player.m_transform.position) < m_range) {
				m_agent.isStopped = true;
				m_animator.SetBool ("idle", false);
				m_animator.SetBool ("attack", true);
			} else {
				m_agent.isStopped = false;
				m_agent.SetDestination (m_player.m_transform.position);
				m_animator.SetBool ("idle", false);
				m_animator.SetBool ("run", true);
			}
		}

		// 跑步状态
		if (stateInfo.IsName ("run")) {
			if (Vector3.Distance (m_transform.position, m_player.m_transform.position) < m_range) {
				m_agent.isStopped = true;
				m_animator.SetBool ("run", false);
				m_animator.SetBool ("attack", true);
			} else {
				m_agent.isStopped = false;
				m_agent.SetDestination (m_player.m_transform.position);
			}
		}

		// 攻击状态
		if (stateInfo.IsName ("attack")) {
			RotateToUser ();
			m_agent.isStopped = true;
			if (stateInfo.normalizedTime >= 1.0f) {
				m_animator.SetBool ("attack", false);
				m_animator.SetBool ("idle", true);
				m_player.OnDamage (m_damage);
			}
		}

		// 死亡状态
		if (stateInfo.IsName ("death")) {
			m_agent.isStopped = true;
			if (stateInfo.normalizedTime >= 1.0f) {
				Destroy (this.gameObject);

				m_player.OnScore (m_score);
				m_player.OnExperience (m_experience);

				float seed = Random.Range (0, 8);
				if (seed < 1.0f) {
					Instantiate (m_drop1, m_transform.position, Quaternion.identity);
				} else if (seed < 2.0f) {
					Instantiate (m_drop2, m_transform.position, Quaternion.identity);
				} else if (seed < 2.0f) {
					Instantiate (m_drop3, m_transform.position, Quaternion.identity);
				} else if (seed < 3.0f) {
					Instantiate (m_drop4, m_transform.position, Quaternion.identity);
				}
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
			m_animator.SetBool ("death", true);
		}
	}

	public void SetLevel(int level) {
		m_life += level * 10;
		m_damage += level * 5;
		m_score += level * 10;
		m_experience += level * 10;
	}
}
