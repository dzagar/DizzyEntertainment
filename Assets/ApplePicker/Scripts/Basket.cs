using UnityEngine;
using System.Collections;

public class Basket : MonoBehaviour {

	public GUIText scoreGT;
	public static int score;

	// Use this for initialization
	void Start () {
		GameControl.control.GetComponent<AudioSource> ().Stop ();
		//find a reference to ScoreCounter GameObject
		GameObject scoreGO = GameObject.Find("ScoreCounter"); //2
		//get GUIText Component of that GameObject
		scoreGT = scoreGO.GetComponent<GUIText>(); //3
		//set starting number of points to 0
		scoreGT.text = "0";
	}
	
	// Update is called once per frame
	void Update () {
		//get current screen position of mouse from Input
		Vector3 mousePos2D = Input.mousePosition; //1
		//the camera's z position sets how far to push the mouse into 3D
		mousePos2D.z = -Camera.main.transform.position.z; //2
		//convert point from 2D screen space into 3D game world space
		Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D); //3
		//move x position of this Basket to the x position of the Mouse
		Vector3 pos = this.transform.position;
		pos.x = mousePos3D.x;
		this.transform.position = pos;
	}

	void OnCollisionEnter (Collision coll) {
		//find out what hit the basket
		GameObject collidedWith = coll.gameObject; //3
		if (collidedWith.tag == "Apple") {
			Destroy (collidedWith);
		}
		//parse text of the scoreGT into an int
		score = int.Parse(scoreGT.text);
		//add points for catching the apple
		score += 100;
		//change speed by score
		float fallSpeed = 9.81f;
		if (score % 1000 == 0 && score != 0) {
			AppleTree.speed *= 1.5f;
			fallSpeed *= 5f;
			Physics.gravity = new Vector3 (0, -fallSpeed, 0);
		}
		//convert score back to a string and display it
		scoreGT.text = score.ToString();
		//track high score
		if (score > HighScore.score) {
			HighScore.score = score;
		}
	}
}
