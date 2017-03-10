using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginVerification : MonoBehaviour {

	public Text textDisplay;
	public InputField usernameField;
	public InputField passwordField;
	private string username;
	private string password;

	void Awake(){
		GameControl.control.GetComponent<AudioSource> ().clip = null;
	}

	public void QuitGame(){
		Application.Quit ();
	}

	public void SubmitInformation(){
		username = usernameField.text; //username key
		password = passwordField.text;
		if (!(GameControl.control.login.ContainsKey(username)) || username == "") {
			//username doesn't exist
			textDisplay.color = Color.red;
			textDisplay.text = "Invalid username/password.";
		} else if (PasswordSecurity.PasswordStorage.VerifyPassword (password, GameControl.control.login [username])) { //verify password
			GameControl.control.currentUser = GameControl.control.userData [username]; //set user profile
			if (GameControl.control.currentUser.accountStatus == AccountStatus.NEW) {
				//move to change password scene
				StartCoroutine ("LoadChangePasswordScreen");
			} else if (GameControl.control.currentUser.accountStatus == AccountStatus.BLOCKED) {
				//display appropriate message on screen
				textDisplay.color = Color.red;
				textDisplay.text = "This account is blocked. Contact the administrator to release the block.";
				GameControl.control.currentUser = null;
			} else {
				//password is good, user has changed their password, move to main menu
				GameControl.control.currentUser.LogOn ();
				StartCoroutine("LoadMainMenu");
			}
		} else {
			//password is incorrect
			textDisplay.color = Color.red;
			textDisplay.text = "Invalid username/password.";
			GameControl.control.userData[username].loginAttempts++;
			if (GameControl.control.userData [username].loginAttempts >= 3 && GameControl.control.userData[username].accountStatus != AccountStatus.ADMIN) { //can't block the admin
				GameControl.control.userData [username].accountStatus = AccountStatus.BLOCKED;
				textDisplay.color = Color.red;
				textDisplay.text = "This account is blocked. Contact the administrator to release the block.";
			}
		}
	}

	IEnumerator LoadMainMenu(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_MainMenuScreen");
	}

	IEnumerator LoadChangePasswordScreen(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_ChangePasswordScreen");
	}
}
