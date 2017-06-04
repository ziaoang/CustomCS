using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour {

	public Transform m_boss;

	Transform m_transform;

	public int m_currBossCount = 0;
	public int m_maxBossCount = 1;

	float m_cdTimer = 60f;
	float m_waitTimer = 60f;

	void Start () {
		m_transform = this.transform;
	}

	void Update () {
		if (m_currBossCount >= m_maxBossCount)
			return;

		m_cdTimer -= Time.deltaTime;

		if (m_cdTimer <= 0) {

			m_cdTimer = m_waitTimer + Random.Range (-10, 10);

			Transform obj = Instantiate (m_boss, m_transform.position, Quaternion.identity);

			Boss boss = obj.GetComponent<Boss> ();

			boss.Init (this);

			m_currBossCount++;
		}
	}
}
