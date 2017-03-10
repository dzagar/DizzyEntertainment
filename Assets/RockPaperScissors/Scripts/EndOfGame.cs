using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndOfGame : MonoBehaviour {
	private static int userFinalScore;
	private static int compFinalScore;
	public GUIText finalResult;
	public Text playerResult;
	public Text compResult;
	public Button playAgain;
	public Button quitGame;

	// Use this for initialization
	void Start () {
		if (compFinalScore > userFinalScore) {
			finalResult.text = "Tragically, the Computer beat you.";
			RPSMain.gamePlay.SetGamePlayScore (-1); //set session score
		} else if (userFinalScore > compFinalScore) {
			finalResult.text = "YOU BEAT THE COMPUTER!";
			RPSMain.gamePlay.SetGamePlayScore (1); //set session score
		} else {
			finalResult.text = "You tied the Computer...";
			RPSMain.gamePlay.SetGamePlayScore (0); //set session score
		}
		GameControl.control.currentUser.rpsGameHistory.Add (RPSMain.gamePlay); //add session to rock paper scissors history
		GameControl.control.SaveData (); //save
		playerResult.text = "You: " + userFinalScore;
		compResult.text = "Computer: " + compFinalScore;
	}

	public static void GetScores(int userScore, int compScore){
		userFinalScore = userScore;
		compFinalScore = compScore;
		return;
	}

	public void Quit(){
		GameControl.control.previousScene = -1; //main menu
		StartCoroutine ("LoadMainMenu");
	}

	public void RestartGame(){
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
		SceneManager.LoadScene ("RockPaperScissorsGame");
	}
}
