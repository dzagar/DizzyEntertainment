using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSettings : MonoBehaviour {

	#region General UI
	public GameObject bronzeSettingsPanel;
	public GameObject silverSettingsPanel;
	public GameObject goldSettingsPanel;
	public Button bronze;
	public Button silver;
	public Button gold;
	public static GameObject numOfEnemiesField;
	public static GameObject levelUpScoreField;
	#endregion

	#region Modal Panel
	public GameObject modalPanelObject;
	public Text errorText;
	#endregion

	#region Enemy Toggle Arrays
	public Toggle[] enemyBr;
	public Toggle[] enemySi;
	public Toggle[] enemyGo;
	#endregion

	public Color[] colors; //enemy toggle colors (correspond with configurations colors)

	// Use this for initialization
	void Start () {
		LoadSettings (); //load toggles from user prefs
		//disable all panels
		bronzeSettingsPanel.SetActive (false);
		silverSettingsPanel.SetActive (false);
		goldSettingsPanel.SetActive (false);
		modalPanelObject.SetActive (false);
		bronze.onClick.Invoke (); //invoke bronze panel button
		AddClicks (); //add click listeners
	}

	void AddClicks(){ //add after load
		bronze.onClick.AddListener (delegate {
			ClickAudio.click.Click ();
		});
		silver.onClick.AddListener (delegate {
			ClickAudio.click.Click ();
		});
		gold.onClick.AddListener (delegate {
			ClickAudio.click.Click ();
		});
	}

	void AddToggleClicks(){ //add after load
		if (bronzeSettingsPanel.activeSelf) {
			for (int i = 0; i < enemyBr.Length; i++) {
				enemyBr [i].onValueChanged.AddListener (delegate {
					ClickAudio.click.Click ();
				});
			}
		} else if (silverSettingsPanel.activeSelf) {
			for (int i = 0; i < enemySi.Length; i++) {
				enemySi [i].onValueChanged.AddListener (delegate {
					ClickAudio.click.Click ();
				});
			}
		} else if (goldSettingsPanel.activeSelf) {
			for (int i = 0; i < enemyGo.Length; i++) {
				enemyGo [i].onValueChanged.AddListener (delegate {
					ClickAudio.click.Click ();
				});
			}
		}
	}

	void SelectedButton(){ //change color of most recently active button to green, else white
		if (bronzeSettingsPanel.activeSelf) {
			bronze.GetComponent<Image> ().color = Color.green;
			silver.GetComponent<Image> ().color = Color.white;
			gold.GetComponent<Image> ().color = Color.white;
		} else if (silverSettingsPanel.activeSelf) {
			bronze.GetComponent<Image> ().color = Color.white;
			silver.GetComponent<Image> ().color = Color.green;
			gold.GetComponent<Image> ().color = Color.white;
		} else if (goldSettingsPanel.activeSelf) {
			bronze.GetComponent<Image> ().color = Color.white;
			silver.GetComponent<Image> ().color = Color.white;
			gold.GetComponent<Image> ().color = Color.green;
		}
	}

	public void ClosePanel(){ //close modal panel (warning windoW)
		modalPanelObject.SetActive (false);
	}

	void numOfEnemiesFieldValueChangeHandler(InputField input){
		if (bronzeSettingsPanel.activeSelf) {
			if (int.Parse (input.text) >= GameControl.control.currentUser.numOfEnemies [1] || int.Parse (input.text) >= GameControl.control.currentUser.numOfEnemies [2]) { //cannot be greater than silver or gold
				modalPanelObject.SetActive (true); //warning window enabled
				errorText.text = "The number of enemies in the Bronze level cannot exceed the number of enemies in the higher levels. Please choose a number less than " + GameControl.control.currentUser.numOfEnemies [1].ToString ()+".";
				numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.red;
			} else {
				GameControl.control.currentUser.numOfEnemies [0] = int.Parse (input.text); //set user prefs
				numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
			}
		} else if (silverSettingsPanel.activeSelf) {
			if (GameControl.control.currentUser.numOfEnemies [0] >= int.Parse (input.text)) {
				modalPanelObject.SetActive (true); //warning window enabled 
				errorText.text = "The number of enemies in the Silver level cannot be less than the number of enemies in the Bronze level. Please choose a number greater than " + GameControl.control.currentUser.numOfEnemies [0].ToString ()+".";
				numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.red;
			} else if (int.Parse (input.text) >= GameControl.control.currentUser.numOfEnemies [2]) {
				modalPanelObject.SetActive (true); //warning window enabled
				errorText.text = "The number of enemies in the Silver level cannot exceed the number of enemies in the Gold level. Please choose a number less than " + GameControl.control.currentUser.numOfEnemies [2].ToString ()+".";
				numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.red;
			} else {
				GameControl.control.currentUser.numOfEnemies[1] = int.Parse (input.text); //set user prefs
				numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
			}
		} else if (goldSettingsPanel.activeSelf) {
			if (GameControl.control.currentUser.numOfEnemies[1] >= int.Parse(input.text) || GameControl.control.currentUser.numOfEnemies[0] >= int.Parse(input.text)) {
				modalPanelObject.SetActive (true); //warning window enabled
				errorText.text = "The number of enemies in the Gold level must be greater than the number of enemies in the lower levels. Please choose a number greater than " + GameControl.control.currentUser.numOfEnemies [1].ToString ()+".";
				numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.red;
			} else {
				GameControl.control.currentUser.numOfEnemies[2] = int.Parse (input.text); //set user prefs
				numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
			}
		}
	}

	void levelUpScoreFieldValueChangeHandler(InputField input){
		if (bronzeSettingsPanel.activeSelf) {
			if (int.Parse (input.text) >= GameControl.control.currentUser.levelUpScore [1] || int.Parse (input.text) >= GameControl.control.currentUser.levelUpScore [2]) { //cannot be greater than silver or gold
				modalPanelObject.SetActive (true); //warning window enabled
				errorText.text = "The Bronze level-up score cannot exceed the level-up scores of the higher levels. Please choose a number less than " + GameControl.control.currentUser.levelUpScore [1].ToString ()+".";
				levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.red;
			} else {
				GameControl.control.currentUser.levelUpScore [0] = int.Parse (input.text); //set user prefs
				levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
			}
		} else if (silverSettingsPanel.activeSelf) {
			if (GameControl.control.currentUser.levelUpScore [0] >= int.Parse (input.text)) {
				modalPanelObject.SetActive (true); //warning window enabled
				errorText.text = "The Silver level-up score cannot be less than the Bronze level-up score. Please choose a number greater than " + GameControl.control.currentUser.levelUpScore [0].ToString ()+".";
				levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.red;
			} else if (int.Parse (input.text) >= GameControl.control.currentUser.levelUpScore [2]) {
				modalPanelObject.SetActive (true); //warning window enabled
				errorText.text = "The Silver level-up score cannot exceed the Gold level-up score. Please choose a number less than " + GameControl.control.currentUser.levelUpScore [2].ToString ()+".";
				levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.red;
			} else {
				GameControl.control.currentUser.levelUpScore[1] = int.Parse (input.text); //set user prefs
				levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
			}
		} else if (goldSettingsPanel.activeSelf) {
			if (GameControl.control.currentUser.levelUpScore[1] >= int.Parse(input.text) || GameControl.control.currentUser.levelUpScore[0] >= int.Parse(input.text)) {
				modalPanelObject.SetActive (true); //warning window enabled
				errorText.text = "The Gold level-up score must be greater than the level-up scores of the lower levels. Please choose a number greater than " + GameControl.control.currentUser.numOfEnemies [1].ToString ()+".";
				levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.red;
			} else {
				GameControl.control.currentUser.levelUpScore[2] = int.Parse (input.text); //set user prefs
				levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
			}
		}
	}

	public void BronzeLevel(){
		silverSettingsPanel.SetActive (false);
		goldSettingsPanel.SetActive (false);
		bronzeSettingsPanel.SetActive (true); //enable bronze settings panel
		SelectedButton (); //select "bronze" button so it is green
		SetEnemyColors (); //set enemy colors based on user configs
		numOfEnemiesField = GameObject.Find ("NumberOfEnemiesInput"); //find number of enemies field in hierarchy
		numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
		numOfEnemiesField.GetComponent<InputField> ().text = (GameControl.control.currentUser.numOfEnemies[0]).ToString (); //set value to user prefs
		numOfEnemiesField.GetComponent<InputField> ().onEndEdit.AddListener (delegate { //add listener
			numOfEnemiesFieldValueChangeHandler (numOfEnemiesField.GetComponent<InputField>());
		});
		levelUpScoreField = GameObject.Find ("LevelUpScoreInput"); //find level up score field in hierarchy
		levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
		levelUpScoreField.GetComponent<InputField> ().text = (GameControl.control.currentUser.levelUpScore[0]).ToString (); //set value to user prefs
		levelUpScoreField.GetComponent<InputField> ().onEndEdit.AddListener (delegate { //add listener
			levelUpScoreFieldValueChangeHandler (levelUpScoreField.GetComponent<InputField>());
		});
		AddToggleClicks (); //add toggle clicks
	}

	public void SilverLevel(){
		bronzeSettingsPanel.SetActive (false);
		goldSettingsPanel.SetActive (false);
		silverSettingsPanel.SetActive (true);
		SelectedButton ();
		SetEnemyColors ();
		numOfEnemiesField = GameObject.Find ("NumberOfEnemiesInput"); //find number of enemies field in hierarchy
		numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
		numOfEnemiesField.GetComponent<InputField> ().text = (GameControl.control.currentUser.numOfEnemies[1]).ToString (); //set value to user prefs
		numOfEnemiesField.GetComponent<InputField> ().onEndEdit.AddListener (delegate { //add listener
			numOfEnemiesFieldValueChangeHandler (numOfEnemiesField.GetComponent<InputField>());
		});
		numOfEnemiesFieldValueChangeHandler (numOfEnemiesField.GetComponent<InputField> ());
		levelUpScoreField = GameObject.Find ("LevelUpScoreInput"); //find level up score field in hierarchy
		levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
		levelUpScoreField.GetComponent<InputField> ().text = (GameControl.control.currentUser.levelUpScore[1]).ToString (); //set value to user prefs
		levelUpScoreField.GetComponent<InputField> ().onEndEdit.AddListener (delegate { //add listener
			levelUpScoreFieldValueChangeHandler (levelUpScoreField.GetComponent<InputField>());
		});
		levelUpScoreFieldValueChangeHandler (levelUpScoreField.GetComponent<InputField> ());
		AddToggleClicks ();
	}

	public void GoldLevel(){
		bronzeSettingsPanel.SetActive (false);
		silverSettingsPanel.SetActive (false);
		goldSettingsPanel.SetActive (true);
		SelectedButton ();
		SetEnemyColors ();
		numOfEnemiesField = GameObject.Find ("NumberOfEnemiesInput"); //find number of enemies field in hierarchy
		numOfEnemiesField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
		numOfEnemiesField.GetComponent<InputField> ().text = (GameControl.control.currentUser.numOfEnemies[2]).ToString (); //set value to user prefs
		numOfEnemiesField.GetComponent<InputField> ().onEndEdit.AddListener (delegate { //add listener
			numOfEnemiesFieldValueChangeHandler (numOfEnemiesField.GetComponent<InputField>());
		});
		numOfEnemiesFieldValueChangeHandler (numOfEnemiesField.GetComponent<InputField> ());
		levelUpScoreField = GameObject.Find ("LevelUpScoreInput"); //find level up score field in hierarchy
		levelUpScoreField.GetComponent<InputField> ().GetComponent<Image> ().color = Color.white;
		levelUpScoreField.GetComponent<InputField> ().text = (GameControl.control.currentUser.levelUpScore[2]).ToString (); //set value to user prefs
		levelUpScoreField.GetComponent<InputField> ().onEndEdit.AddListener (delegate { //add listener
			levelUpScoreFieldValueChangeHandler (levelUpScoreField.GetComponent<InputField>());
		});
		levelUpScoreFieldValueChangeHandler (levelUpScoreField.GetComponent<InputField> ());
		AddToggleClicks ();
	}

	void LoadSettings(){ //set toggles to user prefs
		for (int i = 0; i < enemyBr.Length; i++) {
			enemyBr[i].isOn = GameControl.control.currentUser.enemyBrBool[i];
		}
		for (int i = 0; i < enemySi.Length; i++) {
			enemySi[i].isOn = GameControl.control.currentUser.enemySiBool[i];
		}
		for (int i = 0; i < enemyGo.Length; i++) {
			enemyGo[i].isOn = GameControl.control.currentUser.enemyGoBool[i];
		}
	}

	public void ToggleEnemy(int currentEnemy){
		if (bronzeSettingsPanel.activeSelf) {
			GameControl.control.currentUser.enemyBrBool[currentEnemy] = enemyBr[currentEnemy].isOn; //set user prefs
		} else if (silverSettingsPanel.activeSelf) {
			GameControl.control.currentUser.enemySiBool[currentEnemy] = enemySi[currentEnemy].isOn; //set user prefs
		} else if (goldSettingsPanel.activeSelf) {
			GameControl.control.currentUser.enemyGoBool[currentEnemy] = enemyGo[currentEnemy].isOn; //set user prefs
		}
	}


	public void BackToMenu(){
		GameControl.control.SaveData (); //save
		GameControl.control.previousScene = 1; //previous scene was space shooter menu (1)
		StartCoroutine ("LoadMainMenu");
	}

	IEnumerator LoadMainMenu(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_MainMenuScreen");
	}

	void SetEnemyColors(){
		//set enemy toggle colors equal to their appropriate color by index
		if (bronzeSettingsPanel.activeSelf) {
			for (int i = 0; i < enemyBr.Length; i++) {
				enemyBr [i].GetComponentInParent<Image> ().color = colors [GameControl.control.currentUser.enemyColorIndex [i]];
			}
		} else if (silverSettingsPanel.activeSelf) {
			for (int i = 0; i < enemySi.Length; i++) {
				enemySi [i].GetComponentInParent<Image> ().color = colors [GameControl.control.currentUser.enemyColorIndex [i]];
			}
		} else if (goldSettingsPanel.activeSelf) {
			for (int i = 0; i < enemyGo.Length; i++) {
				enemyGo [i].GetComponentInParent<Image> ().color = colors [GameControl.control.currentUser.enemyColorIndex [i]];
			}
		}
	}
}
