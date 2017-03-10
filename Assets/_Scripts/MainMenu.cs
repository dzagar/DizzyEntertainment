using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public Text usernameDisplay;
	public GameObject menuBackground;
	public Button[] submenuButton;
	public Sprite[] backgrounds;
	public AudioClip[] menuAudio;

	void Start(){
		usernameDisplay.text = GameControl.control.currentUser.username; //set panel header text to the current user's username
		menuBackground.GetComponent<SpriteRenderer>().sprite = backgrounds [GameControl.control.currentUser.menuBGIndex]; //set menu background to user preference
		//load appropriate submenu
		if (GameControl.control.previousScene > -1) { //not main menu
			ClickAudio.click.GetComponent<AudioSource>().mute = true; //mute click sound
			submenuButton [GameControl.control.previousScene].onClick.Invoke (); //invoke appropriate submenu
			Invoke ("Unmute", ClickAudio.click.GetComponent<AudioSource> ().clip.length + 0.1f); //unmute click sound after the length of the sound + 0.1f (to make sure sound doesn't come through)
		}
		GameControl.control.previousScene = -1; //set prev scene to -1
		if (!GameControl.control.GetComponent<AudioSource>().isPlaying) { //if no menu music is playing
			GameControl.control.GetComponent<AudioSource>().clip = menuAudio [GameControl.control.currentUser.menuAudioIndex]; //set menu music to user preference
			GameControl.control.GetComponent<AudioSource>().volume = GameControl.control.currentUser.menuAudioVolume; //set menu music volume to user preference
			GameControl.control.GetComponent<AudioSource>().Play(); //play menu music
		}
	}

	void Unmute(){ //unmute click sound
		ClickAudio.click.GetComponent<AudioSource>().mute = false;
	}

	public void LogOut(){
		GameControl.control.currentUser.LogOff (); //add session to user history
		GameControl.control.SaveData (); //save
		FadeOutMenuMusic();
		StartCoroutine ("LoadLoginScreen");
	}

	public void ExitPackage(){
		GameControl.control.currentUser.LogOff ();
		GameControl.control.SaveData (); //save
		FadeOutMenuMusic();
		StartCoroutine ("ExitGame");
	}

	public void StartApplePicker(){
		FadeOutMenuMusic();
		StartCoroutine("LoadApplePicker");
	}

	public void ApplePickerHistory(){
		GameHistory.game = 0;
		StartCoroutine ("LoadApplePickerHistory");
	}

	public void StartSpaceShooter(){
		FadeOutMenuMusic();
		LoadAspectRatio ();
		StartCoroutine ("LoadSpaceShooter");
	}

	public void SpaceShooterGameLevels(){
		StartCoroutine ("LoadSpaceShooterGameLevels");
	}

	public void SpaceShooterConfigs(){
		StartCoroutine ("LoadSpaceShooterConfigs");
	}

	public void SpaceShooterHistory(){
		GameHistory.game = 3;
		StartCoroutine ("LoadSpaceShooterHistory");
	}

	public void StartRockPaperScissors(){
		FadeOutMenuMusic();
		StartCoroutine ("LoadRockPaperScissors");
	}

	public void RockPaperScissorsHistory(){
		GameHistory.game = 1;
		StartCoroutine ("LoadRockPaperScissorsHistory");
	}

	public void StartMemoryGame(){
		FadeOutMenuMusic();
		StartCoroutine ("LoadMemoryGame");
	}

	public void MemoryGameHistory(){
		GameHistory.game = 2;
		StartCoroutine ("LoadMemoryGameHistory");
	}

	public void UserAccounts(){
		FileMenu.panelChoice = 0;
		StartCoroutine ("LoadFileMenu");
	}

	public void MenuConfigurations(){
		FileMenu.panelChoice = 1;
		StartCoroutine ("LoadFileMenu");
	}

	public void UserHistory(){
		GameHistory.game = 4;
		StartCoroutine ("LoadUserHistory");
	}

	IEnumerator LoadFileMenu(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_FileMenu");
	}

	IEnumerator LoadUserHistory(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_GameHistory");
	}

	IEnumerator LoadApplePicker(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("ApplePickerGame");
	}

	IEnumerator LoadApplePickerHistory(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_GameHistory");
	}

	IEnumerator LoadSpaceShooter(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("ShooterGame");
	}

	IEnumerator LoadSpaceShooterGameLevels(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("ShooterGameLevels");
	}

	IEnumerator LoadSpaceShooterConfigs(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("ShooterConfigurations");
	}

	IEnumerator LoadSpaceShooterHistory(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_GameHistory");
	}

	IEnumerator LoadRockPaperScissors(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("RockPaperScissorsGame");
	}

	IEnumerator LoadRockPaperScissorsHistory(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_GameHistory");
	}

	IEnumerator LoadMemoryGame(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("MemoryGamePlay");
	}

	IEnumerator LoadMemoryGameHistory(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_GameHistory");
	}

	IEnumerator LoadLoginScreen(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		menuBackground.GetComponent<SpriteRenderer>().sprite = backgrounds[0];
		SceneManager.LoadScene ("_UserLoginScreen");
	}

	IEnumerator ExitGame(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		Application.Quit ();
	}

	void FadeOutMenuMusic(){
		float audio = GameControl.control.GetComponent<AudioSource> ().volume;
		while (GameControl.control.GetComponent<AudioSource> ().volume > 0.1f) { //fade out audio
			audio -= 0.01f * Time.deltaTime;
			GameControl.control.GetComponent<AudioSource> ().volume = audio;
		}
		GameControl.control.GetComponent<AudioSource> ().volume = 0f;
	}

	void LoadAspectRatio(){
		if (GameControl.control.currentUser.screenSizeIndex == 0) //option 1: 3:4 ratio
			Screen.SetResolution (525, 700, false);
		else if (GameControl.control.currentUser.screenSizeIndex == 1) //option 2: 5:4 ratio
			Screen.SetResolution (875, 700, false);
		else if (GameControl.control.currentUser.screenSizeIndex == 2) //option 3: 4:3 ratio
			Screen.SetResolution (1200, 900, false);
	}
}

