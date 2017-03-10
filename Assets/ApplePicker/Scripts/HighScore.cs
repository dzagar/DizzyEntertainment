using UnityEngine;
using System.Collections;

public class HighScore : MonoBehaviour {

	static public int score = 1000;
	static public int oldHighScore;

	// Use this for initialization
	void Awake () {
		//if ApplePickerHighScore already exists, read it
		if (PlayerPrefs.HasKey("ApplePickerHighScore")){
			score = PlayerPrefs.GetInt("ApplePickerHighScore");
		}
		//assign high score to ApplePickerHighScore
		PlayerPrefs.SetInt("ApplePickerHighScore", score);
		oldHighScore = PlayerPrefs.GetInt ("ApplePickerHighScore");
	}
	
	// Update is called once per frame
	void Update () {
		GUIText gt = this.GetComponent<GUIText> ();
		gt.text = "High Score: " + score;
		//update ApplePickerHighScore in PlayerPrefs if necessary
		if (score > PlayerPrefs.GetInt ("ApplePickerHighScore")) {
			PlayerPrefs.SetInt ("ApplePickerHighScore", score);
		}
	}
}
