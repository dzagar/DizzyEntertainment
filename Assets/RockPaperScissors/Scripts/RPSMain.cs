using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RPSMain : MonoBehaviour {
	private int result; //value -1 if loss, 0 if tie, 1 if win
	private int userChoice; //the user's choice (0-2)
	private int computerChoice; //computer's choice (0-2)
	public static bool isButtonClicked; //has the user clicked a button
	public static int gamesPlayed; //current number of games played
	public static int userScore; //current user score
	public static int compScore; //current comp score
	public Button rockButton; //button for rock
	public Button paperButton; //button for paper
	public Button scissorsButton; //button for scissors
	public Text numOfGames; //show current number of games
	public Text userScoreText; //show current user score
	public Text compScoreText; //show current comp score
	public static History gamePlay;

	// Use this for initialization
	void Start () {
		GameControl.control.GetComponent<AudioSource> ().Stop ();
		gamePlay = new History (); //start new session
		ElementMovement.displayResult = Camera.main.GetComponentInChildren<GUIText>(); //get GUIText component
		//set defaults; scores to 0, games played to 0
		ElementMovement.displayResult.text = "Rock, paper, or scissors?"; //initialize main text
		numOfGames.text = "Games Played: 0";
		userScoreText.text = "You: 0";
		compScoreText.text = "Computer: 0";
		userScore = 0;
		compScore = 0;			
		gamesPlayed = 0;
		//set user choice and comp choice to -1; no decision has been made
		userChoice = -1;
		computerChoice = -1;
		isButtonClicked = false;
	}

	void OnGUI(){
		//compare results, but only if the user has made a choice
		if (userChoice > -1) {
			//set result to 1, 0, or -1 depending on choices
			SetResult ();
			ElementMovement.GetElements (userChoice, computerChoice); //send choices to ElementMovement
			//initialize elements in ElementMovement script
			Camera.main.GetComponent<ElementMovement> ().OnButtonClick (); //acts like a controlled Start function
			//update scores and number of games played
			ShowResult ();
			//reset user choice
			userChoice = -1;
		}
	}

	public void ShowResult(){
		if (result == 1) { //if the player wins
			userScore++;
			gamesPlayed++;
		} else if (result == -1) { //if the player loses
			compScore++;
			gamesPlayed++;
		}
		//on a tie, the game is replayed; doesn't count as a game or for any score
		return;
	}

	void SetResult(){
		//if the user won, let result = 1; if the user tied, let result = 0; if the user lost, let result = -1
		if (userChoice == computerChoice) { //tie
			result = 0;
		}
		else if (userChoice == 0 && computerChoice == 2) {
			result = 1; //rock > scissors, user wins
		} else if (userChoice == 1 && computerChoice == 0) {
			result = 1; //paper > rock, user wins
		} else if (userChoice == 2 && computerChoice == 1) {
			result = 1; //scissors > paper, user wins
		} else {
			result = -1; //computer wins
		}
		return;
	}

	public void SetRock(){
		//on button click, set choice if button has not been clicked yet
		if (isButtonClicked == false) {
			userChoice = 0;
			computerChoice = Random.Range (0, 3); //random computer choice
			isButtonClicked = true; //button has been clicked; disables buttons temporarily
		}
		return;
	}

	public void SetPaper(){
		//on button click, set choice if button has not been clicked yet
		if (isButtonClicked == false) {
			userChoice = 1;
			computerChoice = Random.Range (0, 3); //random computer choice
			isButtonClicked = true; //button has been clicked; disables buttons temporarily
		}
		return;
	}

	public void SetScissors(){
		//on button click, set choice if button has not been clicked yet
		if (isButtonClicked == false) {
			userChoice = 2;
			computerChoice = Random.Range (0, 3); //random computer choice
			isButtonClicked = true; //button has been clicked; disables buttons temporarily
		}
		return;
	}

	public void UpdateText(){
		//update score displays
		numOfGames.text = "Games Played: " + gamesPlayed; //update number of games that have been played
		userScoreText.text = "You: " + userScore; //update your score
		compScoreText.text = "Computer: " + compScore; //update comp score
	}

}
