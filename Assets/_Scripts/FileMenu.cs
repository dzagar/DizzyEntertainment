using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FileMenu : MonoBehaviour {
	#region General UI
	public Text headerDisplayUser; //header text
	public Text headerDisplayAdmin;
	public GameObject admin; //admin panel
	public GameObject user; //user panel
	public static int panelChoice = new int(); //index of panel chosen in main menu
	public GameObject userUserAccountPanel; //user account panel (user)
	public GameObject userConfigsPanel; //configs panel
	public GameObject adminUserAccountPanel; //user account panel (admin)
	public GameObject adminConfigsPanel; //configs panel
	#endregion

	#region Modal Panel
	public Button submitButton; //submit to create new user
	public Button cancelButton; //cancel create new user
	public InputField createUserField; //field in window to create new user
	public GameObject modalPanelObject; //modal panel
	public Text errorText; //error text in window
	#endregion

	#region User Accounts (admin)
	private GameObject selectedUser; //currently selected user
	public GameObject userSelectable; //prefab for selectable user account gameobject
	private List<GameObject> entries; //number of entries in the scrollrect content pane
	#endregion

	#region User Accounts (user)
	public Button changePassword; //request password change
	#endregion

	#region Menu Configurations
	public Dropdown adminMenuAudioList;
	public Slider adminMenuAudioVolume;
	public Dropdown adminMenuBGList;
	public Image adminMenuBGPreview;
	public Dropdown userMenuAudioList;
	public Slider userMenuAudioVolume;
	public Dropdown userMenuBGList;
	public Image userMenuBGPreview;
	public GameObject menuBackground; //menu background object
	public Sprite[] backgrounds; //backgrounds to choose from
	public AudioClip[] menuAudio; //audio to choose from
	#endregion

	void Start(){
		//set all panels to false
		admin.SetActive(false);
		user.SetActive (false);
		modalPanelObject.SetActive (false);
		if (GameControl.control.currentUser.accountStatus == AccountStatus.ADMIN) { //if it is the admin, set to admin
			admin.SetActive (true);
			switch (panelChoice) { //set inner panel depending on panel choice
			case 0:
				headerDisplayAdmin.text = "User Accounts";
				LoadUserAccounts ();
				break;
			case 1:
				headerDisplayAdmin.text = "Menu Configurations";
				LoadConfigs ();
				break;
			}
		} else {
			//not admin
			user.SetActive (true);
			switch (panelChoice) {
			case 0:
				headerDisplayUser.text = "User Accounts";
				LoadUserAccounts ();
				break;
			case 1:
				headerDisplayUser.text = "Menu Configurations";
				LoadConfigs ();
				break;
			}
		}
	}


	//*****************************************************USER ACCOUNTS***********************************************//
	static int CompareName (GameObject A, GameObject B){ //sorting gameobjects by name
		return A.name.CompareTo(B.name);
	}

	void SetField(GameObject entry, int childIndex, string fieldText){ //set field of entry in the user's "row"
		Transform field = entry.transform.GetChild (childIndex); //get appropriate child
		field.GetComponent<Text> ().text = fieldText; //set string
	}

	void SetColor(GameObject entry, Color color){ //set color of user "row"
		entry.GetComponent<Image> ().color = color;
	}

	public void LoadUserAccounts () {
		if (admin.activeSelf) { //admin panel
			adminUserAccountPanel.SetActive (true); //user accounts is enabled
			adminConfigsPanel.SetActive (false);
			entries = new List<GameObject> ();
			RectTransform content = GameObject.Find ("Content").GetComponent<RectTransform> (); //find content pane
			foreach (KeyValuePair<string, UserProfile> user in GameControl.control.userData.OrderBy(go=>go.Key)) { //for each user stored in user data dictionary; order by username (a-z)
				GameObject entry = Instantiate (userSelectable, Vector3.zero, Quaternion.identity) as GameObject; //instantiate prefab
				entry.transform.SetParent (content); //set parent to content pane
				//set position of entry within content pane
				entry.transform.localPosition = Vector3.zero;
				entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
				entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
				entry.transform.localScale = new Vector3 (1, 1, 1);
				entry.name = user.Value.username; //entry name is username
				SetField (entry, 0, user.Value.username); //set first field (username) to username
				SetField (entry, 1, user.Value.accountStatus.ToString ()); //set second field (status) to account status
				entry.GetComponent<Button> ().onClick.AddListener (delegate { //add onclick listener to entry ("row") for user
					SelectedUser (entry);
				});
				SetColor (entry, Color.gray); //set color to gray
				entries.Add (entry); //add entry to list of entries in content pane
				if (entries.Count*30 >= (content.sizeDelta.y-15)) { //grow size of content pane
					Vector2 temp = content.sizeDelta;
					temp.y = 30 * entries.Count + 60; //60 gives some extra wiggle room at the end of the list
					content.sizeDelta = temp;
				}
				entries.Sort (CompareName); //sort entries by name
			}
		} else {
			//user panel
			userUserAccountPanel.SetActive (true); //user accounts is enabled
			userConfigsPanel.SetActive (false);
		}
	}

	public void SelectedUser(GameObject clickedItem){ //onclick function for entry
		foreach (GameObject e in entries) { //set all entries to gray
			SetColor (e, Color.gray);
		}
		clickedItem.GetComponent<Image> ().color = Color.magenta; //set clicked item to magenta
		selectedUser = clickedItem; //selected user is the currently clicked item
	}

	void ClosePanel(){ //close create user window (modal panel)
		modalPanelObject.SetActive (false);
	}

	public void OpenCreateUserWindow(){  //open create user window (modal panel)
		modalPanelObject.SetActive (true);
		errorText.enabled = false;
		submitButton.onClick.RemoveAllListeners (); //in case there were listeners existing before
		submitButton.onClick.AddListener ( delegate { //add user on click
			AddUser();
		});
		submitButton.onClick.AddListener (delegate { //click noise on click
			ClickAudio.click.Click ();
		});
		cancelButton.onClick.RemoveAllListeners (); //in case there were listeners existing before
		cancelButton.onClick.AddListener (ClosePanel); //close panel on click
		cancelButton.onClick.AddListener (delegate { //click noise on click
			ClickAudio.click.Click ();
		});
		//make buttons active
		submitButton.gameObject.SetActive (true); 
		cancelButton.gameObject.SetActive (true);
	}

	public void AddUser(){ //add user (on submit click)
		bool successful = GameControl.control.currentUser.CreateUser (createUserField.text);
		if (!successful) { //if creating the user was not successful
			errorText.enabled = true; //enable the error text; do nothing else
		} else { //successful create
			ClosePanel(); //close panel
			RectTransform content = GameObject.Find ("Content").GetComponent<RectTransform> (); //find content pane
			GameObject entry = Instantiate (userSelectable) as GameObject; //instantiate prefab
			entry.transform.SetParent (content, false); //set parent to content pane
			//set position within content pane
			entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
			entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
			entry.transform.localScale = new Vector3 (1, 1, 1);
			entry.name = GameControl.control.userData [createUserField.text].username; //set name to username
			SetField (entry, 0, GameControl.control.userData [createUserField.text].username);
			SetField (entry, 1, GameControl.control.userData [createUserField.text].accountStatus.ToString ());
			entry.GetComponent<Button> ().onClick.AddListener (delegate {
				SelectedUser (entry);
			});
			entries.Add (entry); //add to list of entries
			ReloadList (); //refresh list
		}
	}

	public void RemoveUser(){
		bool successful = GameControl.control.currentUser.DeleteUser (selectedUser.name);
		if (successful) { //successfully deleted user
			entries.Remove (selectedUser); //delete from list of entries
			ReloadList (); //refresh list
		}
	}

	public void ReleaseBlock(){
		bool successful = GameControl.control.currentUser.ReleaseBlocks (selectedUser.name);
		if (successful) { //if successfully unblocked
			GameObject result = entries.Find (x => x.name == selectedUser.name); //find appropriate game object
			SetField (result, 0, selectedUser.name);
			SetField (result, 1, GameControl.control.userData [selectedUser.name].accountStatus.ToString ());
			ReloadList (); //refresh list
		}
	}

	void ReloadList(){
		entries.Sort (CompareName); //make sure entries are sorted by name
		RectTransform content = GameObject.Find ("Content").GetComponent<RectTransform>();
		foreach (Transform child in content) {
			if (!entries.Where(obj => obj.name == child.name).SingleOrDefault()) //destroy any children of content pane that don't exist in entries
				GameObject.Destroy(child.gameObject);
		}
		if (entries.Count == 0) { return; } //do nothing
		if (entries.Count*30 >= content.sizeDelta.y) { //change size of content pane
			Vector2 temp = content.sizeDelta;
			temp.y = 30 * entries.Count + 60; //add 60 for wiggle room
			content.sizeDelta = temp;
		}
		int countEntries = 0;
		foreach (GameObject entry in entries)
		{
			entry.transform.SetParent (content); //set parent to content pane
			entry.transform.SetSiblingIndex (countEntries); //set index in hierarchy appropriately
			//set position of entry within content pane
			entry.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
			entry.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, -30);
			entry.transform.localScale = new Vector3 (1, 1, 1);
			countEntries++;
		}
	}

	public void RequestPasswordChange(){ //user change password
		GameControl.control.previousScene = 0; //set previous scene index to file menu (0)
		StartCoroutine ("LoadChangePasswordScreen");
	}

	IEnumerator LoadChangePasswordScreen(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_ChangePasswordScreen");
	}

	//*****************************************************CONFIGURATIONS***********************************************//
	private bool isOriginalMusicPlaying; //check if original music is playing

	public void LoadConfigs(){
		if (GameControl.control.currentUser.menuAudioIndex != 0) //if original music playing isn't the first value in the dropdown (index 0)
			isOriginalMusicPlaying = true;
		else
			isOriginalMusicPlaying = false;
		AddListeners (); //add menu changed handlers
		if (admin.activeSelf) { //admin panel
			adminUserAccountPanel.SetActive (false);
			adminConfigsPanel.SetActive (true); //configs panel is enabled
			//set values in dropdowns/sliders to current values
			adminMenuAudioList.value = GameControl.control.currentUser.menuAudioIndex;
			adminMenuAudioVolume.value = GameControl.control.currentUser.menuAudioVolume;
			adminMenuBGList.value = GameControl.control.currentUser.menuBGIndex;
			adminMenuBGPreview.sprite = backgrounds [GameControl.control.currentUser.menuBGIndex];
		} else {
			//user panel
			userUserAccountPanel.SetActive (false);
			userConfigsPanel.SetActive (true); //configs panel is enabled
			//set values in dropdowns/sliders to current values
			userMenuAudioList.value = GameControl.control.currentUser.menuAudioIndex;
			userMenuAudioVolume.value = GameControl.control.currentUser.menuAudioVolume;
			userMenuBGList.value = GameControl.control.currentUser.menuBGIndex;
			userMenuBGPreview.sprite = backgrounds [GameControl.control.currentUser.menuBGIndex];
		}
		AddClicksConfigs (); //add onclick click sounds
	}

	void AddClicksConfigs(){ //add after load so no clicks occur before
		//add click sounds (onclick) to all dropdowns/sliders in the current panel
		if (admin.activeSelf) {
			adminMenuAudioList.onValueChanged.AddListener ( delegate {
				ClickAudio.click.Click();
			});
			adminMenuAudioVolume.onValueChanged.AddListener (delegate {
				ClickAudio.click.Click();
			});
			adminMenuBGList.onValueChanged.AddListener (delegate {
				ClickAudio.click.Click();
			});
		} else {
			userMenuAudioList.onValueChanged.AddListener (delegate {
				ClickAudio.click.Click();
			});
			userMenuAudioVolume.onValueChanged.AddListener (delegate {
				ClickAudio.click.Click();
			});
			userMenuBGList.onValueChanged.AddListener (delegate {
				ClickAudio.click.Click();
			});
		}
	}

	void AddListeners(){ //add onvaluechanged handlers for each dropdown/slider in current panel
		if (admin.activeSelf) {
			adminMenuAudioList.onValueChanged.AddListener (delegate {
				MenuAudioListValueChangedHandler (adminMenuAudioList);
			});
			adminMenuAudioVolume.onValueChanged.AddListener (delegate {
				MenuAudioVolumeValueChangedHandler (adminMenuAudioVolume);
			});
			adminMenuBGList.onValueChanged.AddListener (delegate {
				MenuBGListValueChangedHandler (adminMenuBGList);
			});
		} else {
			userMenuAudioList.onValueChanged.AddListener (delegate {
				MenuAudioListValueChangedHandler (userMenuAudioList);
			});
			userMenuAudioVolume.onValueChanged.AddListener (delegate {
				MenuAudioVolumeValueChangedHandler (userMenuAudioVolume);
			});
			userMenuBGList.onValueChanged.AddListener (delegate {
				MenuBGListValueChangedHandler (userMenuBGList);
			});
		}

	}

	void MenuAudioListValueChangedHandler(Dropdown target){ //menu audio dropdown
		if (!isOriginalMusicPlaying) {
			GameControl.control.currentUser.menuAudioIndex = target.value; //set menu audio index to current value
			GameControl.control.GetComponent<AudioSource> ().clip = menuAudio [GameControl.control.currentUser.menuAudioIndex]; //set menu audio to audiosource
			GameControl.control.GetComponent<AudioSource> ().Play (); //play new menu audio
		}
		isOriginalMusicPlaying = false;
	}

	void MenuAudioVolumeValueChangedHandler(Slider target){ //menu volume slider
		GameControl.control.currentUser.menuAudioVolume = target.value; //set menu audio volume to current value
		GameControl.control.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.menuAudioVolume; //set audiosource volume to menu audio volume
	}

	void MenuBGListValueChangedHandler(Dropdown target){ //menu background dropdown
		GameControl.control.currentUser.menuBGIndex = target.value; //set menu bg index to current value
		if (admin.activeSelf) {
			adminMenuBGPreview.sprite = backgrounds [GameControl.control.currentUser.menuBGIndex]; //set preview sprite to new background
			menuBackground.GetComponent<SpriteRenderer> ().sprite = backgrounds [GameControl.control.currentUser.menuBGIndex]; //change menu background to new background
		} else {
			userMenuBGPreview.sprite = backgrounds [GameControl.control.currentUser.menuBGIndex];
			menuBackground.GetComponent<SpriteRenderer> ().sprite = backgrounds [GameControl.control.currentUser.menuBGIndex];
		}
	}

	//*****************************************************BYE!!!!!!!!!!***********************************************//

	public void BackToMenu(){
		GameControl.control.SaveData (); //save
		GameControl.control.previousScene = 0; //set previous scene index to file menu (0)
		StartCoroutine ("LoadMainMenu");
	}

	IEnumerator LoadMainMenu(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_MainMenuScreen");
	}

}
