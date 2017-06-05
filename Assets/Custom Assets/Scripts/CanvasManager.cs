using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MiniJSON;

public enum GameState {
	Playing,
	Pause,
	End
}

public enum EndGameType {
	Death,
	Timeout
}

public class CanvasManager : MonoBehaviour {

	public static CanvasManager Instance = null;

	public Transform m_toast;
	Transform m_transform;

	Text m_textEnterType;

	Text m_textAttack;
	Text m_textRange;
	Text m_textLevel;
	Text m_textExperience;

	Text m_textTime;
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

	Text m_textEndState;
	Text m_textEndScore;
	Text m_textEndRank;

	Text[] m_textTopName = new Text[10];
	Text[] m_textTopScore = new Text[10];

	float m_TimeScaleRef = 1.0f;

	void Awake () {
		Instance = this;

		m_transform = this.transform;

		foreach(Transform t in this.transform.GetComponentsInChildren<Transform>()) {
			if (t.name.CompareTo ("TextAttack") == 0) {
				m_textAttack = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextRange") == 0) {
				m_textRange = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextLevel") == 0) {
				m_textLevel = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextExperience") == 0) {
				m_textExperience = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextTime") == 0) {
				m_textTime = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextScore") == 0) {
				m_textScore = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextShield") == 0) {
				m_textShield = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextBlood") == 0) {
				m_textBlood = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("ImageShield") == 0) {
				m_imageShield = t.GetComponent<Image> ();
			} else if (t.name.CompareTo ("ImageBlood") == 0) {
				m_imageBlood = t.GetComponent<Image> ();
			} else if (t.name.CompareTo ("TextAmmo") == 0) {
				m_textAmmo = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextCharger") == 0) {
				m_textCharger = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextEnterType") == 0) {
				m_textEnterType = t.GetComponent<Text> ();
			}
		}

		m_gameState = GameState.Playing;
		m_pauseMenu = this.transform.Find ("PauseMenu").gameObject;
		m_endMenu = this.transform.Find ("EndMenu").gameObject;

		Time.timeScale = m_TimeScaleRef;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		if (Share.m_is_save) {
			m_textEnterType.text = "恢复游戏";
		} else {
			m_textEnterType.text = "开始游戏";
		}
		Destroy (m_textEnterType, 2.0f);
	}

	void Start() {
	
	}

	public void Toast(string message) {
		Transform toast = Instantiate (m_toast, new Vector3 (Screen.width/2, 40, 0),  Quaternion.identity, m_transform);
		toast.GetComponentInChildren<Text> ().text = message;
		Destroy(toast.gameObject, 2.0f);
	}

	void GamePause () {
		m_gameState = GameState.Pause;
		m_pauseMenu.SetActive (true);

		m_TimeScaleRef = Time.timeScale;
		Time.timeScale = 0.0f;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	void GameResume () {
		m_gameState = GameState.Playing;
		m_pauseMenu.SetActive (false);

		Time.timeScale = m_TimeScaleRef;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
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

	public void SaveGame() {
		StartCoroutine(SavePost());
	}

	IEnumerator SavePost() {
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers ["Content-Type"] = "application/x-www-form-urlencoded";
		Player m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		string data = "username=" + Share.m_username
				    + "&remain_time="    + m_player.m_remain_time.ToString ()
					+ "&attack="         + m_player.m_attack.ToString()
					+ "&range="          + m_player.m_range.ToString()
					+ "&level="          + m_player.m_level.ToString()
					+ "&experience="     + m_player.m_experience.ToString()
					+ "&max_level="      + m_player.m_level.ToString()
					+ "&max_experience=" + m_player.m_max_experience.ToString()
					+ "&score="          + m_player.m_score.ToString()
					+ "&shield="         + m_player.m_shield.ToString()
					+ "&max_shield="     + m_player.m_max_shield.ToString()
					+ "&blood="          + m_player.m_blood.ToString()
					+ "&max_blood="      + m_player.m_max_blood.ToString()
					+ "&ammo="           + m_player.m_ammo.ToString()
					+ "&max_ammo="       + m_player.m_max_ammo.ToString()
					+ "&charger="        + m_player.m_charger.ToString()
					+ "&max_charger="    + m_player.m_max_charger.ToString();
				byte[] bs = System.Text.UTF8Encoding.UTF8.GetBytes (data);
		WWW www = new WWW (Share.m_serverUrl + "/save", bs, headers);
		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);
			var dict = Json.Deserialize(www.text) as Dictionary<string, object>;
			if (dict["status"].Equals("succ")) {
				Toast ("保存成功");
			} else {
				Toast ("保存失败");
			}
		} else {
			Toast (www.error);
		}
	}

	public void QuitGame() {
		Application.Quit ();
	}

	public void EndGame(EndGameType type) {
		m_gameState = GameState.End;

		m_TimeScaleRef = Time.timeScale;
		Time.timeScale = 0.0f;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		m_endMenu.SetActive (true);

		foreach (Transform t in this.transform.GetComponentsInChildren<Transform>()) {
			if (t.name.CompareTo ("TextEndState") == 0) {
				m_textEndState = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextEndScore") == 0) {
				m_textEndScore = t.GetComponent<Text> ();
			} else if (t.name.CompareTo ("TextEndRank") == 0) {
				m_textEndRank = t.GetComponent<Text> ();
			} else if (t.name.StartsWith ("TextTopName")) {
				int index = int.Parse (t.name.Replace ("TextTopName", ""));
				m_textTopName[index-1] = t.GetComponent<Text> ();
			} else if (t.name.StartsWith ("TextTopScore")) {
				int index = int.Parse (t.name.Replace ("TextTopScore", ""));
				m_textTopScore[index-1] = t.GetComponent<Text> ();
			}
		}

		if (type == EndGameType.Death) {
			m_textEndState.text = "你已死亡";
		} else if (type == EndGameType.Timeout) {
			m_textEndState.text = "时间已到";
		}

		Player m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		m_textEndScore.text = m_player.m_score.ToString ();

		StartCoroutine (EndPost ());
	}

	IEnumerator EndPost() {
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers ["Content-Type"] = "application/x-www-form-urlencoded";
		Player m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		string data = "username=" + Share.m_username + "&score="    + m_player.m_score.ToString ();
		byte[] bs = System.Text.UTF8Encoding.UTF8.GetBytes (data);
		WWW www = new WWW (Share.m_serverUrl + "/end", bs, headers);
		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);
			var dict = Json.Deserialize(www.text) as Dictionary<string, object>;
			m_textEndRank.text = ((int)(long)dict ["rank"]).ToString ();
			List<object> top10 = (List<object>)dict ["top10"];
			for (int i = 0; i < 10; i++) {
				Dictionary<string, object> item = (Dictionary<string, object>) top10 [i];
				m_textTopName [i].text = (string) item ["username"];
				m_textTopScore [i].text = ( (int) (long) item ["max_score"] ).ToString ();
			}
		} else {
			Toast (www.error);
		}
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

	public void SetTime(int time) {
		int second = time % 60;
		int minute = (time - second) / 60;
		m_textTime.text = minute.ToString().PadLeft(2, '0') + ":" + second.ToString().PadLeft(2, '0');
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
