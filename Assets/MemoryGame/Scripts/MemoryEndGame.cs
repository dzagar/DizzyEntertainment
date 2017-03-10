using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MemoryEndGame : MonoBehaviour {

	public Text scoreText;
	public Text timeText;
	public Button playAgain;
	public Image[] snoopy;
	public Sprite[] snoopyOptions;
	public AudioSource endAudio;
	public AudioClip[] gameOverAudio;

	// Use this for initialization
	void Start () {
		//show score
		if (MemoryMain.score <= 0) { //you lost
			scoreText.text = "You lost...";
			snoopy[0].sprite = snoopyOptions [1];
			snoopy[1].sprite = snoopyOptions [1];
			endAudio.clip = gameOverAudio [1];
		} else { //you won
			scoreText.text = "You WON!\nScore: " + MemoryMain.score;
			snoopy[0].sprite = snoopyOptions [0];
			snoopy[1].sprite = snoopyOptions [0];
			endAudio.clip = gameOverAudio [0];
		}
		endAudio.Play ();
		float currentMinutes = Mathf.Floor (MemoryMain.time / 60);
		float currentSeconds = MemoryMain.time % 60;
		timeText.text = "Time: " + currentMinutes.ToString ("00") + ":" + currentSeconds.ToString ("00.00");
	}

	public void PlayAgain(){
		StartCoroutine ("Restart");
	}

	public void Quit(){
		GameControl.control.previousScene = -1; //main menu
		StartCoroutine ("LoadMainMenu");
	}

	IEnumerator LoadMainMenu(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_MainMenuScreen");
	}

	IEnumerator Restart(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("MemoryGamePlay");
	}
}
