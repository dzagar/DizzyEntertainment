using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHistory : MonoBehaviour {
	#region General UI
	public Text headerDisplayUser;
	public Text headerDisplayAdmin;
	public GameObject menuBackground;
	public Sprite[] backgrounds;
	public GameObject adminHistory;
	public GameObject userHistory;
	#endregion

	#region Prefabs
	private List<GameObject> userPlays;
	public GameObject adminBasicEntry;
	public GameObject userBasicEntry;
	public GameObject adminShooterEntry;
	public GameObject userShooterEntry;
	public GameObject adminUserHistoryEntry;
	public GameObject userUserHistoryEntry;
	#endregion

	#region Log Captions
	public GameObject logCaptionAdminBasic;
	public GameObject logCaptionAdminShoot;
	public GameObject logCaptionAdminUserHistory;
	public GameObject logCaptionUserBasic;
	public GameObject logCaptionUserShoot;
	public GameObject logCaptionUserUserHistory;
	#endregion

	public static int game; //game index


	void Awake(){
		menuBackground.GetComponent<SpriteRenderer>().sprite = backgrounds [GameControl.control.currentUser.menuBGIndex]; //set background properly
		if (GameControl.control.currentUser.accountStatus == AccountStatus.ADMIN) { //if admin, enable admin history panel
			adminHistory.SetActive (true);
			userHistory.SetActive (false);
		} else {
			adminHistory.SetActive (false);
			userHistory.SetActive (true);
		}
		switch (game) { //which game/window is to be opened
		case 0: //apple picker
			APHistory ();
			break;
		case 1: //rock paper scissors
			RPSHistory ();
			break;
		case 2: //memory game
			MGHistory ();
			break;
		case 3: //space shooter
			ShooterHistory ();
			break;
		case 4: //user history
			UserHistory ();
			break;
		}
	}


	void SetField(GameObject entry, int childIndex, string fieldText){ //set specified field inside of entry
		Transform field = entry.transform.GetChild (childIndex); //get child index
		field.GetComponent<Text> ().text = fieldText; //set string
	}

	static int CompareTime(GameObject a, GameObject b){ //sort by name (time)
		return DateTime.Parse (b.name).CompareTo (DateTime.Parse (a.name));
	}

	string DisplayOutcome(int outcome){ //translate outcome to string
		if (outcome == -1)
			return "LOST";
		else if (outcome == 1)
			return "WON";
		else
			return "TIED";
	}

	void APHistory(){
		if (GameControl.control.currentUser.accountStatus == AccountStatus.ADMIN) { //admin
			headerDisplayAdmin.text = "Apple Picker Game History";
			logCaptionAdminBasic.SetActive (true); //enable basic admin log caption (username, time, score)
			logCaptionAdminShoot.SetActive (false);
			logCaptionAdminUserHistory.SetActive (false);
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			foreach (KeyValuePair<string, UserProfile> user in GameControl.control.userData) { //each user in user data
				foreach (History play in user.Value.applePickerHistory.OrderByDescending(go=>go.GetGamePlayTime())) { //each history entry in the user's apple picker history list, ordered by time
					if (user.Value.applePickerHistory.Count != 0) {
						GameObject entry = Instantiate (adminBasicEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate admin basic entry prefab
						entry.name = play.GetGamePlayTime ().ToString(); //set name to play time; crucial for ordering
						SetField (entry, 0, user.Value.username); //username field
						SetField (entry, 1, play.GetGamePlayTime ().ToShortDateString () + " at " + play.GetGamePlayTime ().ToShortTimeString ()); //time played field
						SetField (entry, 2, play.GetGamePlayScore ().ToString()); //score field
						userPlays.Add (entry); //add entry
					}
				}
			}
			userPlays.Sort (CompareTime); //sort list of plays
			foreach (GameObject entry in userPlays) {
				entry.transform.SetParent (content); //set parent of entry to content pane
				//set position of entry within content pane
				entry.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 1f);
				entry.transform.localPosition = Vector3.zero;
				entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
				entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
				entry.transform.localScale = new Vector3 (1, 1, 1);
			}
			if (userPlays.Count != 0) { //only check if user plays isn't empty
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		} else {
			GameControl.control.currentUser.applePickerHistory.Sort (); //sort entries by time
			headerDisplayUser.text = "Apple Picker Game History";
			logCaptionUserBasic.SetActive (true); //enable basic user log caption (time, score)
			logCaptionUserShoot.SetActive (false);
			logCaptionUserUserHistory.SetActive (false);
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			if (GameControl.control.currentUser.applePickerHistory.Count != 0) {
				foreach (History play in GameControl.control.currentUser.applePickerHistory) { //each history entry in the user's apple picker history list
					GameObject entry = Instantiate (userBasicEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate user basic entry prefab
					entry.transform.SetParent (content); //set parent of entry to content pane
					//set position of entry within content pane
					entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
					entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
					entry.transform.localScale = new Vector3 (1, 1, 1);
					entry.transform.localPosition = Vector3.zero;
					entry.name = play.GetGamePlayTime ().ToString(); //set name to play time
					SetField (entry, 0, play.GetGamePlayTime ().ToShortDateString ()); //time field
					SetField (entry, 1, play.GetGamePlayScore ().ToString ()); //score field
					userPlays.Add (entry); //add entry
				}
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		}
		GameControl.control.previousScene = 3; //previous scene index is the apple picker menu (3)
		Canvas.ForceUpdateCanvases (); //update canvas
	}

	void RPSHistory(){
		if (GameControl.control.currentUser.accountStatus == AccountStatus.ADMIN) { //admin
			headerDisplayAdmin.text = "Rock, Paper, Scissors History";
			logCaptionAdminBasic.SetActive (true); //enable basic admin log caption (username, time, score)
			logCaptionAdminShoot.SetActive (false);
			logCaptionAdminUserHistory.SetActive (false);
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			foreach (KeyValuePair<string, UserProfile> user in GameControl.control.userData) { //each user in user data
				foreach (History play in user.Value.rpsGameHistory.OrderByDescending(go=>go.GetGamePlayTime())) { //each history entry in the user's rock paper scissors history list, ordered by time
					if (user.Value.rpsGameHistory.Count != 0) {
						GameObject entry = Instantiate (adminBasicEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate admin basic entry prefab
						entry.name = play.GetGamePlayTime ().ToString(); //set name to play time
						SetField (entry, 0, user.Value.username); //username field
						SetField (entry, 1, play.GetGamePlayTime ().ToShortDateString () + " at " + play.GetGamePlayTime ().ToShortTimeString ()); //time field
						SetField(entry, 2, DisplayOutcome (play.GetGamePlayScore ())); //score (outcome) field
						userPlays.Add (entry); //add entry
					}
				}
			}
			userPlays.Sort (CompareTime); //sort list of plays
			foreach (GameObject entry in userPlays) {
				entry.transform.SetParent (content); //set parent of entry to content pane
				//set position of entry within content pane
				entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
				entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
				entry.transform.localScale = new Vector3 (1, 1, 1);
				entry.transform.localPosition = Vector3.zero;
			}
			if (userPlays.Count != 0) {
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		} else {
			GameControl.control.currentUser.rpsGameHistory.Sort (); //sort entries by time
			headerDisplayUser.text = "Rock Paper Scissors History";
			logCaptionUserBasic.SetActive (true); //enable basic user log caption (time, score)
			logCaptionUserShoot.SetActive (false);
			logCaptionUserUserHistory.SetActive (false);
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			if (GameControl.control.currentUser.rpsGameHistory.Count != 0) {
				foreach (History play in GameControl.control.currentUser.rpsGameHistory) { //each history entry in the user's rock paper scissors history list
					GameObject entry = Instantiate (userBasicEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate user basic entry prefab
					entry.transform.SetParent (content); //set parent of entry to content pane
					//set position of entry within content pane
					entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
					entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
					entry.transform.localScale = new Vector3 (1, 1, 1);
					entry.transform.localPosition = Vector3.zero;
					entry.name = play.GetGamePlayTime ().ToString(); //ste name to play time
					SetField (entry, 0, play.GetGamePlayTime ().ToShortDateString ()); //time field
					SetField (entry, 1, DisplayOutcome (play.GetGamePlayScore ())); //score (outcome) field
					userPlays.Add (entry); //add entry
				}
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		}
		GameControl.control.previousScene = 2; //previous scene index is the rock paper scissors menu (2)
		Canvas.ForceUpdateCanvases (); //update canvas
	}

	void MGHistory(){
		if (GameControl.control.currentUser.accountStatus == AccountStatus.ADMIN) { //admin
			headerDisplayAdmin.text = "Memory Game History";
			logCaptionAdminBasic.SetActive (true); //enable basic admin log caption (username, time, score)
			logCaptionAdminShoot.SetActive (false);
			logCaptionAdminUserHistory.SetActive (false);
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			foreach (KeyValuePair<string, UserProfile> user in GameControl.control.userData) { //each user in user data
				foreach (History play in user.Value.memoryGameHistory.OrderByDescending(go=>go.GetGamePlayTime())) { //each history entry in the user's memory game history list, ordered by time
					if (user.Value.memoryGameHistory.Count != 0) {
						GameObject entry = Instantiate (adminBasicEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate admin basic entry prefab
						entry.name = play.GetGamePlayTime ().ToString(); //set name to play time
						SetField (entry, 0, user.Value.username); //username field
						SetField (entry, 1, play.GetGamePlayTime ().ToShortDateString () + " at " + play.GetGamePlayTime ().ToShortTimeString ()); //time field
						SetField (entry, 2, play.GetGamePlayScore ().ToString ()); //score field
						userPlays.Add (entry); //add entry
					}
				}
			}
			userPlays.Sort (CompareTime); //sort list of plays
			foreach (GameObject entry in userPlays) {
				entry.transform.SetParent (content); //set parent of entry to content pane
				//set position of entry within content pane
				entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
				entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
				entry.transform.localScale = new Vector3 (1, 1, 1);
				entry.transform.localPosition = Vector3.zero;
			}
			if (userPlays.Count != 0) {
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		} else {
			GameControl.control.currentUser.memoryGameHistory.Sort (); //sort entries by time
			headerDisplayUser.text = "Memory Game History";
			logCaptionUserBasic.SetActive (true); //enable basic user log caption (time, score)
			logCaptionUserShoot.SetActive (false);
			logCaptionUserUserHistory.SetActive (false);
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			if (GameControl.control.currentUser.memoryGameHistory.Count != 0) {
				foreach (History play in GameControl.control.currentUser.memoryGameHistory) { //each history entry in the user's memory game history list, ordered by time
					GameObject entry = Instantiate (userBasicEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate user basic entry prefab
					entry.transform.SetParent (content); //set parent of entry to content pane
					//set position of entry within content pane
					entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
					entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
					entry.transform.localScale = new Vector3 (1, 1, 1);
					entry.transform.localPosition = Vector3.zero;
					entry.name = play.GetGamePlayTime ().ToString(); //set name to play time
					SetField (entry, 0, play.GetGamePlayTime ().ToString ()); //time field
					SetField (entry, 1, play.GetGamePlayScore ().ToString ()); //score field
					userPlays.Add (entry); //add entry
				}
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		}
		GameControl.control.previousScene = 4; //previous scene index is the memory game menu (4)
		Canvas.ForceUpdateCanvases (); //update canvas
	}

	void ShooterHistory(){
		if (GameControl.control.currentUser.accountStatus == AccountStatus.ADMIN) { //admin
			headerDisplayAdmin.text = "Space Shooter Game History";
			logCaptionAdminBasic.SetActive (false);
			logCaptionAdminShoot.SetActive (true); //enable shooter admin log caption (username, time, score, level reached)
			logCaptionAdminUserHistory.SetActive (false);
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			foreach (KeyValuePair<string, UserProfile> user in GameControl.control.userData) { //each user in user data
				foreach (History play in user.Value.shooterGameHistory.OrderByDescending(go=>go.GetGamePlayTime())) { //each history entry in the user's space shooter history list, ordered by time
					if (user.Value.shooterGameHistory.Count != 0) {
						GameObject entry = Instantiate (adminShooterEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate admin shooter entry prefab
						entry.name = play.GetGamePlayTime ().ToString(); //set name to play time
						SetField (entry, 0, user.Value.username); //username field
						SetField (entry, 1, play.GetGamePlayTime ().ToShortDateString () + " at " + play.GetGamePlayTime ().ToShortTimeString ()); //time field
						SetField (entry, 2, play.GetGamePlayScore ().ToString ()); //score field
						SetField (entry, 3, play.GetGamePlayLevel ()); //level reached field
						userPlays.Add (entry); //add entry
					}
				}
			}
			userPlays.Sort (CompareTime); //sort list of plays
			foreach (GameObject entry in userPlays) {
				entry.transform.SetParent (content); //set parent of entry to content pane
				//set position of entry within content pane
				entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
				entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
				entry.transform.localScale = new Vector3 (1, 1, 1);
				entry.transform.localPosition = Vector3.zero;
			}
			if (userPlays.Count != 0) {
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		} else {
			GameControl.control.currentUser.shooterGameHistory.Sort (); //sort entries by time
			headerDisplayUser.text = "Space Shooter Game History";
			logCaptionUserBasic.SetActive (false);
			logCaptionUserShoot.SetActive (true); //enable shooter user log caption (time, score, level reached)
			logCaptionUserUserHistory.SetActive (false);
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			if (GameControl.control.currentUser.shooterGameHistory.Count != 0) {
				foreach (History play in GameControl.control.currentUser.shooterGameHistory) { //each history entry in the user's space shooter history list, ordered by time
					GameObject entry = Instantiate (userShooterEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate user shooter entry prefab
					entry.transform.SetParent (content); //set parent of entry to content pane
					//set position of entry within content pane
					entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
					entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
					entry.transform.localScale = new Vector3 (1, 1, 1);
					entry.transform.localPosition = Vector3.zero;
					entry.name = play.GetGamePlayTime ().ToString(); //set name to play time
					SetField (entry, 0, play.GetGamePlayTime ().ToString ()); //time field
					SetField (entry, 1, play.GetGamePlayScore ().ToString ()); //score field
					SetField (entry, 2, play.GetGamePlayLevel ()); //level reached field
					userPlays.Add (entry); //add entry
				}
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		}
		GameControl.control.previousScene = 1; //previous scene index is the space shooter menu (1)
		Canvas.ForceUpdateCanvases (); //update canvas
	}

	public void UserHistory(){
		if (GameControl.control.currentUser.accountStatus == AccountStatus.ADMIN) { //admin
			headerDisplayAdmin.text = "User History";
			logCaptionAdminBasic.SetActive (false);
			logCaptionAdminShoot.SetActive (false);
			logCaptionAdminUserHistory.SetActive (true); //enable user history admin log caption (username, time, duration, status)
			userPlays = new List<GameObject> ();
			RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
			content.transform.localPosition = Vector3.zero;
			foreach (KeyValuePair<string, UserProfile> user in GameControl.control.userData) { //each user in user data
				foreach (KeyValuePair<DateTime, TimeSpan> session in user.Value.userHistory.OrderByDescending(go=>go.Key)) { //each history entry in the user's user history list, ordered by time
					if (user.Value.userHistory.Count != 0) {
						GameObject entry = Instantiate (adminUserHistoryEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate admin user history entry prefab
						entry.name = session.Key.ToString (); //set name to play time
						SetField (entry, 0, user.Value.username); //username field
						SetField (entry, 1, session.Key.ToShortDateString () + " at " + session.Key.ToShortTimeString ()); //time field
						SetField(entry, 2, session.Value.TotalMinutes.ToString("0.0") + " minutes"); //duration field
						SetField (entry, 3, user.Value.accountStatus.ToString ()); //status field
						userPlays.Add (entry); //add entry
					}
				}
			}
			userPlays.Sort (CompareTime); //sort entries by time
			foreach (GameObject entry in userPlays) {
				entry.transform.SetParent (content); //set parent of entry to content pane
				//set position of entry within content pane
				entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
				entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
				entry.transform.localScale = new Vector3 (1, 1, 1);
				entry.transform.localPosition = Vector3.zero;
			}
			if (userPlays.Count != 0) {
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		} else {
			//user
			if (GameControl.control.currentUser.userHistory.Count != 0) {
				headerDisplayAdmin.text = "User History";
				logCaptionUserBasic.SetActive (false);
				logCaptionUserShoot.SetActive (false);
				logCaptionUserUserHistory.SetActive (true); //enable user history user log caption (time, duration)
				userPlays = new List<GameObject> ();
				RectTransform content = GameObject.Find ("HistoryContent").GetComponent<RectTransform> (); //find content pane of scrollrect
				content.transform.localPosition = Vector3.zero;
				foreach (KeyValuePair<DateTime, TimeSpan> session in GameControl.control.currentUser.userHistory.OrderByDescending(go=>go.Key)) { //each history entry in the user's user history list, ordered by time
					GameObject entry = Instantiate (userUserHistoryEntry, Vector3.zero, Quaternion.identity) as GameObject; //instantiate user history user entry prefab
					entry.transform.SetParent (content); //set parent of entry to content pane
					//set position of entry within content pane
					entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
					entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
					entry.transform.localScale = new Vector3 (1, 1, 1);
					entry.transform.localPosition = Vector3.zero;
					entry.name = session.Key.ToString (); //set name to play time
					SetField (entry, 0, session.Key.ToShortDateString ()); //time field
					SetField (entry, 1, session.Value.TotalMinutes.ToString ("0.0") + " minutes"); //duration field
					userPlays.Add (entry); //add entry
				}
				userPlays.Sort (CompareTime); //sort entries by time
				if (userPlays.Count*30 >= (content.sizeDelta.y - 15)) { //adjust content pane size if the combined heights of the entries exceed the content pane size
					Debug.Log ("size adjusted");
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * userPlays.Count + 60; //add 60 for wiggle room at bottom of list
					content.sizeDelta = temp;
				}
			}
		}
		GameControl.control.previousScene = 0; //previous scene index is the file menu (0)
		Canvas.ForceUpdateCanvases (); //update canvas
	}

	public void BackToMenu(){
		StartCoroutine ("LoadMainMenu");
	}

	IEnumerator LoadMainMenu(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_MainMenuScreen");
	}
}
