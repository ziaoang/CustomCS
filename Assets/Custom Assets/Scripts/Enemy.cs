using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

	Transform m_transform;

	Animator m_animator;

	NavMeshAgent m_agent;

	Player m_player;

	float m_timer = 1;

	int m_life = 100;

	int m_damage = 10;

	int m_score = 10;

	EnemySpawn m_spawn;

	void Start () {

		m_transform = this.transform;

		m_animator = this.GetComponent<Animator> ();

		m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		m_agent = GetComponent<NavMeshAgent> ();
	}

	void Update () {

		// 过度状态
		if (m_animator.IsInTransition (0)) {
			return;
		}

		AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo (0);

		// 待机状态
		if (stateInfo.IsName ("idle")) {
			m_animator.SetBool ("idle", false);

			m_timer -= Time.deltaTime;
			if (m_timer <= 0) {
				if (Vector3.Distance (m_transform.position, m_player.m_transform.position) < 1.5) {
					m_animator.SetBool ("attack", true);
				} else {
					m_timer = 1;
					m_agent.SetDestination (m_player.m_transform.position);
					m_animator.SetBool ("run", true);
				}
			}
		}

		// 跑步状态
		if (stateInfo.IsName ("run")) {
			m_animator.SetBool ("run", false);

			m_timer -= Time.deltaTime;
			if (m_timer <= 0) {
				m_timer = 1;
				m_agent.SetDestination (m_player.m_transform.position);
			}

			if (Vector3.Distance (m_transform.position, m_player.m_transform.position) < 1.5) {
				m_agent.isStopped = true;
				m_animator.SetBool ("attack", true);
			}
		}

		// 攻击状态
		if (stateInfo.IsName ("attack")) {
			m_animator.SetBool ("attack", false);

			if (stateInfo.normalizedTime >= 1.0f) {
				m_animator.SetBool ("idle", true);
				m_timer = 1;
				m_player.OnDamage (m_damage);
			}
		}

		// 死亡状态
		if (stateInfo.IsName ("death")) {
			if (stateInfo.normalizedTime >= 1.0f) {
				Destroy (this.gameObject);

				m_spawn.m_currEnemyCount--;

				m_player.OnScore (m_score);
			}
		}
	}

	public void OnDamage(int damage) {
		if (m_life <= 0)
			return;

		m_life -= damage;

		if (m_life <= 0) {
			m_animator.SetBool ("death", true);
		}
	}

	public void Init(EnemySpawn spawn) {
		m_spawn = spawn;
	}
}
