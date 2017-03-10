using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangePasswordScreen : MonoBehaviour {
	public Text usernameDisplay;
	public InputField newPasswordField;

	// Use this for initialization
	void Start () {
		usernameDisplay.text = "Change your password,\n" + GameControl.control.currentUser.username; //personalized message
	}
		
	public void SubmitNewPassword(){
		bool successful = GameControl.control.currentUser.ChangePassword (newPasswordField.text);
		if (successful) { //if the change password was successful
			GameControl.control.currentUser.LogOn (); //log on the system
			GameControl.control.SaveData (); //save
			StartCoroutine ("LoadMainMenu");
		} else { //unsuccessful log on; either due to same password or empty password
			usernameDisplay.color = Color.red; //change text to red; warning
			usernameDisplay.text = "Invalid password. Change your password,\n" + GameControl.control.currentUser.username;
		}
	}

	IEnumerator LoadMainMenu(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_MainMenuScreen");
	}
}
