using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour {

	public Transform m_boss;

	Transform m_transform;

	float m_cdTimer = 60f;
	float m_waitTimer = 60f;

	int level = 0;

	void Start () {
		m_transform = this.transform;
	}

	void Update () {
		m_cdTimer -= Time.deltaTime;

		if (m_cdTimer <= 0) {
			m_cdTimer = m_waitTimer;

			level++;

			Transform obj = Instantiate (m_boss, m_transform.position, Quaternion.identity);

			Boss boss = obj.GetComponent<Boss> ();

			boss.SetLevel (level);
		}
	}
}
