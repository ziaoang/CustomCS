using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

	public static CanvasManager Instance = null;

	Text m_textLife;
	Text m_textBullet;
	Text m_textScore;

	void Start () {
		Instance = this;

		foreach(Transform t in this.transform.GetComponentsInChildren<Transform>()) {
			if (t.name.CompareTo ("TextLife") == 0) {
				m_textLife = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextBullet") == 0) {
				m_textBullet = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextScore") == 0) {
				m_textScore = t.GetComponent<Text> ();
			}
		}
	}

	void Update () {
		
	}

	public void SetLife(int life) {
		m_textLife.text = "生命值 " + life.ToString();
	}

	public void SetBullet(int bullet) {
		m_textBullet.text = "子弹数 " + bullet.ToString();
	}

	public void SetScore(int score) {
		m_textScore.text = "得分值 " + score.ToString();
	}
}
