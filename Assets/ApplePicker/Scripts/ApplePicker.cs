using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ApplePicker : MonoBehaviour {

	public GameObject basketPrefab; //placeholder for basket prefab
	public int numBaskets = 3; //number of 'lives'
	public float basketBottomY = -14f; 
	public float basketSpacingY = 2f;
	public List<GameObject> basketList;
	private History gamePlay;

	// Use this for initialization
	void Start () {
		gamePlay = new History (); //start new game session
		basketList = new List<GameObject> ();
		for (int i = 0; i < numBaskets; i++) {
			GameObject tBasketGO = Instantiate (basketPrefab) as GameObject;
			Vector3 pos = Vector3.zero;
			pos.y = basketBottomY + (basketSpacingY * i);
			tBasketGO.transform.position = pos;
			basketList.Add (tBasketGO);
		}
	}

	public void AppleDestroyed () {
		//destroy all the falling Apples
		GameObject[] tAppleArray = GameObject.FindGameObjectsWithTag("Apple"); //3
		foreach (GameObject tGO in tAppleArray) {
			Destroy(tGO);
		}
		//destroy one of the baskets
		//get index of last basket in basketList
		int basketIndex = basketList.Count-1;
		//get reference to that Basket GameObject
		GameObject tBasketGO = basketList[basketIndex];
		//remove basket from the list and destroy the gameobject
		basketList.RemoveAt(basketIndex);
		Destroy (tBasketGO);
		//restart the game which doesn't affect HighScore.score
		if (basketList.Count == 0) {
			gamePlay.SetGamePlayScore (Basket.score); //save score
			GameControl.control.currentUser.applePickerHistory.Add (gamePlay); //add to apple picker history
			GameControl.control.SaveData (); //save
			SceneManager.LoadScene ("ApplePickerEnd");
		}
	}
}
