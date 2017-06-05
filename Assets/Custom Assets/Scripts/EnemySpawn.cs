using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

	public Transform m_enemy;

	Transform m_transform;

	float m_cdTimer = 0f;
	float m_waitTimer = 15f;

	int level = 0;

	void Start () {
		m_transform = this.transform;
	}

	void Update () {
		m_cdTimer -= Time.deltaTime;

		if (m_cdTimer <= 0) {

			level++;

			m_cdTimer = m_waitTimer + Random.Range (-5, 5);

			Transform obj = Instantiate (m_enemy, m_transform.position, Quaternion.identity);

			Enemy enemy = obj.GetComponent<Enemy> ();

			enemy.SetLevel (level);
		}
	}
}
