using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Buttons : MonoBehaviour {

	public GameObject rock; //rock game object that is acting as a button
	public GameObject paper; //paper game object that is acting as a button
	public GameObject scissors; //empty game object that contains transform coordinates of desired spot for scissors button
	public Button rockButton; //rock button; overlays game object
	public Button paperButton; //paper button; overlays game object
	public Button scissorsButton; //scissors button; contains scissor image already

	// Use this for initialization
	void Start () {
		//convert game object coordinates to screen coordinates; set button coordinates equal to the vector 
		//containing the converted coordinates
		Vector3 rockButtonPos = Camera.main.WorldToScreenPoint (rock.transform.position);
		rockButton.transform.position = rockButtonPos;
		Vector3 paperButtonPos = Camera.main.WorldToScreenPoint (paper.transform.position);
		paperButton.transform.position = paperButtonPos;
		Vector3 scissorsButtonPos = Camera.main.WorldToScreenPoint (scissors.transform.position);
		scissorsButton.transform.position = scissorsButtonPos;
	}

}
