using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

	public Transform m_enemy;

	public int m_currEnemyCount = 0;

	public int m_maxEnemyCount = 3;

	Transform m_transform;

	float m_timer = 0;

	void Start () {
		m_transform = this.transform;
	}

	void Update () {
		if (m_currEnemyCount >= m_maxEnemyCount) {
			return;
		}
			
		m_timer -= Time.deltaTime;

		if (m_timer <= 0) {

			m_timer = Random.Range (20, 30);

			Transform obj = Instantiate (m_enemy, m_transform.position, Quaternion.identity);

			Enemy enemy = obj.GetComponent<Enemy> ();

			enemy.Init (this);

			m_currEnemyCount++;
		}
	}
}
