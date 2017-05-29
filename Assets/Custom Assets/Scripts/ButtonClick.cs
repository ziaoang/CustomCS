using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour {

	InputField username;
	InputField password;

	void Start () {
		foreach(Transform t in this.transform.GetComponentsInChildren<Transform>()) {
			if (t.name.CompareTo ("Username") == 0) {
				username = t.GetComponent<InputField> ();
			}
			else if (t.name.CompareTo ("Password") == 0) {
				password = t.GetComponent<InputField> ();
			}
		}
	}

	public void LoginSceneLoginButtonClick() {
		Debug.Log ("login login");
		Debug.Log (username.text);
		Debug.Log (password.text);
		SceneManager.LoadScene ("Main");
	}
	public void LoginSceneRegisterButtonClick() {
		Debug.Log ("login register");
		SceneManager.LoadScene ("Register");
	}
	public void RegisterSceneRegisterButtonClick() {
		Debug.Log ("register register");
		Debug.Log (username.text);
		Debug.Log (password.text);
		SceneManager.LoadScene ("Main");
	}
	public void RegitserSceneLoginButtonClick() {
		Debug.Log ("register login");
		SceneManager.LoadScene ("Login");
	}
}
