using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

	public Button quitGame;
	public Button restartGame;
	public static int finalScore; //final score
	public static float finalTime; //final time
	public Text scoreText; //final score UI text
	public Text timeText; //final time UI text
	public Text levelText; //level reached ui text
	public AudioClip[] gameOverAudio; //game over audio library (array)

	void Start(){
		InGameControls.gamePlay.SetGamePlayLevel (Main.level); //set session level reached
		InGameControls.gamePlay.SetGamePlayScore (Player.score); //set session score
		GameControl.control.currentUser.shooterGameHistory.Add (InGameControls.gamePlay); //add session to shooter game history
		GameControl.control.SaveData (); //save
		Screen.SetResolution (1200, 900, false); //reset screen resolution to normal standalone setting
		finalScore = Player.score; //get player score on invoke
		finalTime = InGameControls.currentTime; //get the time on invoke
		GameObject audio = GameObject.Find("GameOverSound");
		if (finalScore >= GameControl.control.currentUser.levelUpScore[1]) { //gold status
			//play winning music
			audio.GetComponent<AudioSource>().clip = gameOverAudio[1];
		} else {
			//play an ominous, lurking, dark side voiceover
			audio.GetComponent<AudioSource>().clip = gameOverAudio[0];
		}
		scoreText.text = "Score: " + finalScore;
		levelText.text = "Level Achieved: " + InGameControls.gamePlay.GetGamePlayLevel ();
		float finalMinutes = Mathf.Floor (finalTime / 60); //convert to minutes
		float finalSeconds = finalTime % 60; //get seconds
		timeText.text = "Time: " + finalMinutes.ToString("00") + ":" + finalSeconds.ToString("00.00"); //display time
		audio.GetComponent<AudioSource>().Play();
	}

	public void RestartGame(){ //on button click
		LoadAspectRatio();
		StartCoroutine (RestartClick ()); //for click noise
	}

	public void Quit(){ //on button click
		GameControl.control.previousScene = -1;
		StartCoroutine (QuitClick ()); //for click noise
	}

	IEnumerator QuitClick(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene("_MainMenuScreen");
	}

	IEnumerator RestartClick(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("ShooterGame"); //load game scene
	}

	void LoadAspectRatio(){ //load appropriate resolution from user prefs
		if (GameControl.control.currentUser.screenSizeIndex == 0) //3:4
			Screen.SetResolution (525, 700, false);
		else if (GameControl.control.currentUser.screenSizeIndex == 1) //5:4
			Screen.SetResolution (875, 700, false);
		else if (GameControl.control.currentUser.screenSizeIndex == 2) //4:3 
			Screen.SetResolution (1200, 900, false);
	}

}
