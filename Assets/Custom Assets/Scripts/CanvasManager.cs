using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState {
	Playing,
	Pause,
	End
}

public class CanvasManager : MonoBehaviour {

	public static CanvasManager Instance = null;

	Text m_textAttack;
	Text m_textRange;
	Text m_textLevel;
	Text m_textExperience;

	Text m_textScore;

	Text m_textShield;
	Text m_textBlood;
	Image m_imageShield;
	Image m_imageBlood;

	Text m_textAmmo;
	Text m_textCharger;

	GameState m_gameState;

	GameObject m_pauseMenu;
	GameObject m_endMenu;

	float m_TimeScaleRef = 1.0f;
	float m_VolumeRef = 1.0f;

	void Start () {
		Instance = this;

		foreach(Transform t in this.transform.GetComponentsInChildren<Transform>()) {
			if (t.name.CompareTo ("TextAttack") == 0) {
				m_textAttack = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextRange") == 0) {
				m_textRange = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextLevel") == 0) {
				m_textLevel = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextExperience") == 0) {
				m_textExperience = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextScore") == 0) {
				m_textScore = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextShield") == 0) {
				m_textShield = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextBlood") == 0) {
				m_textBlood = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("ImageShield") == 0) {
				m_imageShield = t.GetComponent<Image> ();
			}
			else if (t.name.CompareTo ("ImageBlood") == 0) {
				m_imageBlood = t.GetComponent<Image> ();
			}
			else if (t.name.CompareTo ("TextAmmo") == 0) {
				m_textAmmo = t.GetComponent<Text> ();
			}
			else if (t.name.CompareTo ("TextCharger") == 0) {
				m_textCharger = t.GetComponent<Text> ();
			}
		}

		Player m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		SetAttack (m_player.m_attack);
		SetRange (m_player.m_range);
		SetLevel (m_player.m_level);
		SetExperience (m_player.m_experience);
		SetScore (m_player.m_score);
		SetShield (m_player.m_shield, m_player.m_max_shield);
		SetBlood (m_player.m_blood, m_player.m_max_blood);
		SetAmmo (m_player.m_ammo);
		SetCharger (m_player.m_charger);

		m_gameState = GameState.Playing;
		m_pauseMenu = this.transform.FindChild ("PauseMenu").gameObject;
		m_endMenu = this.transform.FindChild ("EndMenu").gameObject;

		Time.timeScale = m_TimeScaleRef;
		AudioListener.volume = m_VolumeRef;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void GamePause () {
		m_gameState = GameState.Pause;
		m_pauseMenu.SetActive (true);

		m_TimeScaleRef = Time.timeScale;
		Time.timeScale = 0.0f;

		m_VolumeRef = AudioListener.volume;
		AudioListener.volume = 0.0f;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	void GameResume () {
		m_gameState = GameState.Playing;
		m_pauseMenu.SetActive (false);

		Time.timeScale = m_TimeScaleRef;
		AudioListener.volume = m_VolumeRef;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void GameEnd () {
		m_gameState = GameState.End;
		m_endMenu.SetActive (true);

		m_TimeScaleRef = Time.timeScale;
		Time.timeScale = 0.0f;

		m_VolumeRef = AudioListener.volume;
		AudioListener.volume = 0.0f;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	void Update () {
		if (m_gameState == GameState.Playing && Input.GetKeyUp (KeyCode.Escape)) {
			GamePause ();
		} else if (m_gameState == GameState.Pause && Input.GetKeyUp (KeyCode.Escape)) {
			GameResume ();
		}
	}

	public GameState GetGameState() {
		return m_gameState;
	}

	public void ContinueGame() {
		GameResume ();
	}

	public void RestartGame() {
		SceneManager.LoadScene ("Main");
	}

	public void KeepGame() {
		Debug.Log ("KeepGame");
	}

	public void QuitGame() {
		Application.Quit ();
	}

	public void EndGame() {
		GameEnd ();
	}

	public void SetAttack(int attack) {
		m_textAttack.text = "攻击: " + attack.ToString();
	}

	public void SetRange(int range) {
		m_textRange.text = "射程: " + range.ToString();
	}

	public void SetLevel(int level) {
		m_textLevel.text = "等级: " + level.ToString();
	}

	public void SetExperience(int experience) {
		m_textExperience.text = "经验: " + experience.ToString();
	}

	public void SetScore(int score) {
		m_textScore.text = score.ToString();
	}

	public void SetShield(int shield, int maxShield) {
		m_textShield.text = shield.ToString();
		m_imageShield.fillAmount = (float)shield / maxShield;
	}

	public void SetBlood(int blood, int maxBlood) {
		m_textBlood.text = blood.ToString();
		m_imageBlood.fillAmount = (float)blood / maxBlood;
	}

	public void SetAmmo(int ammo) {
		m_textAmmo.text = ammo.ToString ();
	}

	public void SetCharger(int charger) {
		m_textCharger.text = "/ " + charger.ToString ();
	}
}
