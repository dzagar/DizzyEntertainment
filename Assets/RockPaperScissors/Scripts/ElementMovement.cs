using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ElementMovement : MonoBehaviour {
	private GameObject player; //representation of player choice
	private GameObject computer; //representation of computer choice
	public static int userPick; //assigned to user choice in Main
	public static int compPick; //assigned to comp choice in Main
	public float speed = 10f; //speed in meter/second
	public static GUIText displayResult; //main GUIText display
	public Rigidbody playerRb; //physical force on player game object
	public Rigidbody compRb; //physical force on computer game object
	public GameObject[] prefabChoices; //array made up of rock(0), paper(1), and scissor(2) game object prefabs

	// Use this for initialization
	public void OnButtonClick(){
		//instantiate game objects corresponding to user and computer choices
		player = Instantiate (prefabChoices[userPick]) as GameObject; 
		computer = Instantiate (prefabChoices[compPick]) as GameObject;
		if (player.gameObject.tag == "Paper") { //change nose of the paper crane in the right direction
			player.transform.Rotate(0,-80,0);
		}
		if (player.gameObject.tag == "Scissors") { //change tip of the scissors to point in the right direction
			player.transform.Rotate(0,180,0);
		}
		//add element collision script to player object
		player.AddComponent<ElementCollision> ();
		//set position of game objects just outside of screen boundaries
		Vector3 playerPos = new Vector3 (-11, 0);
		Vector3 compPos = new Vector3 (11, 0);
		player.transform.position = playerPos;
		computer.transform.position = compPos;
	}

	public void FixedUpdate(){
		if (computer != null && player != null) { //if the game objects exist
			playerRb = player.GetComponent<Rigidbody> ();
			playerRb.AddForce (new Vector3 (speed, 0, 0)); //add force on x-axis moving to the right
			compRb = computer.GetComponent<Rigidbody> ();
			compRb.AddForce (new Vector3 (-speed, 0, 0)); //add force on x-axis moving to the left
		}
	}

	public static void GetElements(int player, int comp){ //retrieve player and computer choices from main
		userPick = player;
		compPick = comp;
		return;
	}

	public void GetResult(string outcome){
		displayResult = Camera.main.GetComponentInChildren<GUIText> (); //locate GUIText component
		displayResult.text = outcome; //text displayed will be string defined in Element Collision script
		if (player.GetComponent<ElementCollision>().numOfCollisions == 3) { //last collision
			GameObject go = GameObject.Find ("Canvas"); //main is attached to canvas
			go.GetComponent<RPSMain> ().UpdateText (); //update scores in main
			RPSMain.isButtonClicked = false; //enable buttons again
		}
		return;
	}

	public void ResetChoices(){ //destroy game objects and set to null so fixed update stops and game objects aren't creepin'
		Destroy (player);
		Destroy (computer);
		player = null;
		computer = null;
	}
}
