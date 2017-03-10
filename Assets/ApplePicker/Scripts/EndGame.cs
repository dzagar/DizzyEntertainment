using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

	public Button quitGame;
	public Button playAgain;
	public Text finalScore;
	public Text beatHighScore;

	void Start(){
		finalScore.text = "Final Score: " + Basket.score;
		if (Basket.score > HighScore.oldHighScore) {
			beatHighScore.text = "You beat the high score of " + HighScore.oldHighScore;
		} else {
			beatHighScore.text = "You didn't beat the high score.";
		}
	}

	public void QuitGame(){
		GameControl.control.previousScene = -1; //main menu
		StartCoroutine ("LoadMainMenu");
	}

	public void PlayAgain(){
		AppleTree.speed = 1f;
		Basket.score = 0;
		Physics.gravity = new Vector3 (0, -9.81f, 0);
		StartCoroutine ("Restart");
	}

	IEnumerator LoadMainMenu(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_MainMenuScreen");
	}

	IEnumerator Restart(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("ApplePickerGame");
	}
}
