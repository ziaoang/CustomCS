using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

	public Transform m_enemy;

	Transform m_transform;

	public int m_currEnemyCount = 0;
	public int m_maxEnemyCount = 3;

	float m_cdTimer = 0f;
	float m_waitTimer = 20f;

	void Start () {
		m_transform = this.transform;
	}

	void Update () {
		if (m_currEnemyCount >= m_maxEnemyCount)
			return;

		m_cdTimer -= Time.deltaTime;

		if (m_cdTimer <= 0) {

			m_cdTimer = m_waitTimer + Random.Range (-10, 10);

			Transform obj = Instantiate (m_enemy, m_transform.position, Quaternion.identity);

			Enemy enemy = obj.GetComponent<Enemy> ();

			enemy.Init (this);

			m_currEnemyCount++;
		}
	}
}
