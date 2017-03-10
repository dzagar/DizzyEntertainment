using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ElementCollision : MonoBehaviour {

	public int numOfCollisions; //count number of collisions

	void Awake(){
		numOfCollisions = 0; //set to zero on awake
	}

	void OnCollisionEnter( Collision coll ) {
		numOfCollisions++; //increase collision counter
		if (numOfCollisions == 1) { //on first collide
			Camera.main.GetComponent<ElementMovement> ().GetResult ("Rock..."); //display on GUIText
		}
		if (numOfCollisions == 2) { //second collide
			Camera.main.GetComponent<ElementMovement> ().GetResult ("Paper..."); //display on GUIText
		}
		if (numOfCollisions == 3) { //third and final collide
			GameObject other = coll.gameObject;
			//depending on the gameobject tags, display GUIText that displays a win, loss, or tie
			switch (other.tag) {
			case "Rock":
				if (this.gameObject.tag == "Rock") { 
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! You tied - replay.\nRock, paper, or scissors?");
					break;
				} else if (this.gameObject.tag == "Paper") { 
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! YOU WIN!\nRock, paper, or scissors?");
					break;
				} else {
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! You lose.\nRock, paper, or scissors?");
					break;
				}
			case "Paper":
				if (this.gameObject.tag == "Rock") {
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! You lose.\nRock, paper, or scissors?");
					break;
				} else if (this.gameObject.tag == "Paper") {
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! You tied - replay.\nRock, paper, or scissors?");
					break;
				} else {
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! YOU WIN!\nRock, paper, or scissors?");
					break;
				}
			case "Scissors":
				if (this.gameObject.tag == "Rock") {
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! YOU WIN!\nRock, paper, or scissors?");
					break;
				} else if (this.gameObject.tag == "Paper") {
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! You lose.\nRock, paper, or scissors?");
					break;
				} else {
					Camera.main.GetComponent<ElementMovement> ().GetResult ("Scissors! You tied - replay.\nRock, paper, or scissors?");
					break;
				}
			}
			this.gameObject.SetActive (false); //deactivate player's gameobject, but don't destroy (or else it destroys this script)
			Destroy (other); //destroy computer's game object
			if (RPSMain.gamesPlayed >= 10) { //if at end of match
				GameObject go = GameObject.Find("Canvas");
				go.GetComponent<RPSMain>().rockButton.enabled = false;
				go.GetComponent<RPSMain> ().paperButton.enabled = false;
				go.GetComponent<RPSMain> ().scissorsButton.enabled = false;
				DelayedRestart (1f); //delay for a little so result can be seen, then go to end screen
			} else {
				Restart (); //restart immediately
			}
		}
	}

	public void DelayedRestart(float delay){ //delay restart function
		Invoke ("Restart", delay);
	}

	public void Restart(){
		if (RPSMain.gamesPlayed >= 10) { //end of match
			EndOfGame.GetScores (RPSMain.userScore, RPSMain.compScore); //retrieve final scores from main
			SceneManager.LoadScene ("RockPaperScissorsEnd"); //load end screen
		} else {
			Camera.main.GetComponent<ElementMovement> ().ResetChoices (); //reset game object representation of choices to null and destroy 
			Destroy(this); //destroy this
		}
	}
}
