using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MiniJSON;

public class ButtonClick : MonoBehaviour {

	public Transform m_toast;
	Transform m_transform;

	InputField username;
	InputField password;
	InputField password2;

	void Start () {
		m_transform = this.transform;
		foreach(Transform t in this.transform.GetComponentsInChildren<Transform>()) {
			if (t.name.CompareTo ("Username") == 0) {
				username = t.GetComponent<InputField> ();
			}
			else if (t.name.CompareTo ("Password") == 0) {
				password = t.GetComponent<InputField> ();
			}
			else if (t.name.CompareTo ("Password2") == 0) {
				password2 = t.GetComponent<InputField> ();
			}
		}
	}

	public void Toast(string message) {
		Transform toast = Instantiate (m_toast, new Vector3 (Screen.width/2, 40, 0),  Quaternion.identity, m_transform);
		toast.GetComponentInChildren<Text> ().text = message;
		Destroy(toast.gameObject, 2.0f);
	}

	public void IntroduceSceneButtonClick() {
		SceneManager.LoadScene ("Login");
	}

	public void LoginSceneLoginButtonClick() {
		if (username.text == "") {
			Toast ("账号不能为空");
			return;
		}
		if (password.text == "") {
			Toast ("密码不能为空");
			return;
		}
		StartCoroutine(LoginPost());
	}

	public void LoginSceneRegisterButtonClick() {
		SceneManager.LoadScene ("Register");
	}

	public void RegisterSceneRegisterButtonClick() {
		if (username.text == "") {
			Toast ("账号不能为空");
			return;
		}
		if (password.text == "") {
			Toast ("密码不能为空");
			return;
		}
		if (password.text != password2.text) {
			Toast ("两次密码不一致");
			return;
		}
		StartCoroutine(RegisterPost());
	}

	public void RegitserSceneLoginButtonClick() {
		SceneManager.LoadScene ("Login");
	}

	IEnumerator LoginPost() {
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers ["Content-Type"] = "application/x-www-form-urlencoded";
		string data = "username=" + username.text + "&password=" + password.text;
		byte[] bs = System.Text.UTF8Encoding.UTF8.GetBytes (data);
		WWW www = new WWW (Share.m_serverUrl + "/login", bs, headers);
		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);

			var dict = Json.Deserialize(www.text) as Dictionary<string, object>;

			Debug.Log (dict);
			if (dict["status"].Equals("succ")) {
				Share.m_username = (string)dict ["username"];
				Share.m_is_save = (bool)dict ["is_save"];
				if (Share.m_is_save) {
					Share.m_remain_time    = (int) (long) dict["remain_time"];
					Share.m_attack         = (int) (long) dict["attack"];
					Share.m_range          = (int) (long) dict["range"];
					Share.m_level          = (int) (long) dict["level"];
					Share.m_experience     = (int) (long) dict["experience"];
					Share.m_max_level      = (int) (long) dict["max_level"];
					Share.m_max_experience = (int) (long) dict["max_experience"];
					Share.m_score          = (int) (long) dict["score"];
					Share.m_shield         = (int) (long) dict["shield"];
					Share.m_max_shield     = (int) (long) dict["max_shield"];
					Share.m_blood          = (int) (long) dict["blood"];
					Share.m_max_blood      = (int) (long) dict["max_blood"];
					Share.m_ammo           = (int) (long) dict["ammo"];
					Share.m_max_ammo       = (int) (long) dict["max_ammo"];
					Share.m_charger        = (int) (long) dict["charger"];
					Share.m_max_charger    = (int) (long) dict["max_charger"];
				}
				SceneManager.LoadScene ("Main");
			} else {
				Toast (dict["message"].ToString());
			}
		} else {
			Toast (www.error);
		}
	}

	IEnumerator RegisterPost() {
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers ["Content-Type"] = "application/x-www-form-urlencoded";
		string data = "username=" + username.text + "&password=" + password.text;
		byte[] bs = System.Text.UTF8Encoding.UTF8.GetBytes (data);
		WWW www = new WWW (Share.m_serverUrl + "/register", bs, headers);
		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);
			var dict = Json.Deserialize(www.text) as Dictionary<string, object>;
			if (dict["status"].Equals("succ")) {
				Share.m_username = username.text;
				Share.m_is_save = false;
				SceneManager.LoadScene ("Main");
			} else {
				Toast (dict["message"].ToString());
			}
		} else {
			Toast (www.error);
		}
	}
}
