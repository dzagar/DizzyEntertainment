using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameControls : MonoBehaviour {

	public Text playPauseButtonText; 
	public Text timeText;
	private bool isPaused = false;

	public GameObject[] enemySprites;
	public Material[] enemyMaterials;
	public Color[] colors;
	public static int enemy0Kills; //number of enemy 0 kills
	public static int enemy1Kills; //number of enemy 1 kills
	public static int enemy2Kills; //number of enemy 2 kills
	public static int enemy3Kills; //number of enemy 3 kills
	public static int enemy4Kills; //number of enemy 4 kills
	public Text[] enemyKills; //enemy kills text array(5)
	public Text bronzeLevel;
	public Text silverLevel;
	public Text goldLevel;
	private Color gold;
	private Color silver;
	private Color fadedLevel = new Color (0f, 0f, 0f, 0.5f); //faded color
	public static bool recordTime; //keep track of timer
	public static float currentTime; //game time
	public static History gamePlay;

	// Use this for initialization
	void Awake () {
		gamePlay = new History ();
		playPauseButtonText.text = "Pause";
		timeText.text = "Time: 00:00.00";
		recordTime = false;
		currentTime = 0f;
		for (int i = 0; i < enemyMaterials.Length; i++) {
			enemyMaterials[i].SetColor("_Color", colors [GameControl.control.currentUser.enemyColorIndex[i]]); //set enemy material colors
		}
		for (int i = 0; i < enemySprites.Length; i++) {
			enemySprites [i].GetComponent<Image>().color = enemyMaterials [i].color; //set enemy sprite colors
		}
		//set enemy kills to 0 as default
		enemy0Kills = 0;
		enemy1Kills = 0;
		enemy2Kills = 0;
		enemy3Kills = 0;
		enemy4Kills = 0;
		//get temps, then fade out silver and gold levels
		gold = goldLevel.color;
		silver = silverLevel.color;
		silverLevel.color = fadedLevel;
		goldLevel.color = fadedLevel;

	}
	
	// Update is called once per frame
	void Update () {
		//update text based on number of kills
		enemyKills [0].text = enemy0Kills.ToString ();
		enemyKills [1].text = enemy1Kills.ToString ();
		enemyKills [2].text = enemy2Kills.ToString ();
		enemyKills [3].text = enemy3Kills.ToString ();
		enemyKills [4].text = enemy4Kills.ToString ();
		//update color of level text based on level
		if (Main.level == 1) {
			silverLevel.color = silver;
			bronzeLevel.color = fadedLevel;
		} else if (Main.level == 2) {
			goldLevel.color = gold;
			silverLevel.color = fadedLevel;
		}
		if (recordTime) { //when record time is true, keep the time
			currentTime += Time.deltaTime;
			float currentMinutes = Mathf.Floor(currentTime / 60);
			float currentSeconds = currentTime % 60;
			timeText.text = "Time: " + currentMinutes.ToString("00") + ":" + currentSeconds.ToString("00.00");
		}
	}

	public void PlayPause(){
		GameObject audio = GameObject.Find ("BackgroundAudio");
		if (!isPaused) { //when the button is clicked and isPaused == false
			//pause everything
			Time.timeScale = 0f;
			playPauseButtonText.text = "Play";
			audio.GetComponent<AudioSource> ().Pause ();
			isPaused = true;
		} else { //game is going from paused->play
			Time.timeScale = 1f;
			playPauseButtonText.text = "Pause";
			audio.GetComponent<AudioSource> ().UnPause ();
			isPaused = false;
		}
	}

	public void RestartGame(){ //restart the game
		isPaused = true;
		Time.timeScale = 1f;
		SceneManager.LoadScene ("ShooterGame");
	}

	public void StopGame(){ //stop the game
		isPaused = true;
		Time.timeScale = 1f;
		recordTime = false;
		SceneManager.LoadScene ("ShooterEnd");
	}
}
