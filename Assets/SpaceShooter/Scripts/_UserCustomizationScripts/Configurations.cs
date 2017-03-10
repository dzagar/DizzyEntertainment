using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Configurations : MonoBehaviour {

	#region General UI
	public GameObject enemySettingsPanel;
	public GameObject audioSettingsPanel;
	public GameObject backgroundSettingsPanel;
	public Button enemySettings;
	public Button audioSettings;
	public Button bgSettings;
	#endregion

	#region Enemy Settings
	public Button[] enemyButton;
	public GameObject pointsToKillField;
	public Dropdown colorOptions;
	public Color[] colors;
	public static int currentEnemy;
	#endregion

	#region Audio Settings
	public Dropdown backgroundMusicOptions;
	public Slider backgroundMusicVolume;
	public Dropdown winningMusicOptions;
	public Slider winningMusicVolume;
	public Dropdown shootSoundOptions;
	public Slider shootVolume;
	public Dropdown destroySoundOptions;
	public Slider destroyVolume;
	public AudioClip[] backgroundMusic;
	public AudioClip[] winningMusic;
	public AudioClip[] shootSound;
	public AudioClip[] destroySound;
	public Image[] circlePreview;
	#endregion

	#region Background Settings
	public Dropdown backgroundOptionsBr;
	public Dropdown backgroundOptionsSi;
	public Dropdown backgroundOptionsGo;
	public Dropdown screenSize;
	public Texture[] backgrounds;
	public Texture[] dynamicBG;
	public Image[] previewImage;
	public Material[] previewMaterial;
	public Material dynamicBackground;
	#endregion


	// Use this for initialization
	void Start () {
		//disable all panels
		enemySettingsPanel.SetActive (false);
		audioSettingsPanel.SetActive (false);
		backgroundSettingsPanel.SetActive (false);
		//force click first panel (enemy settings)
		enemySettings.onClick.Invoke ();
		AddClicks (); //add click listeners to tab buttons
	}

	void Update(){
		if (Camera.main.GetComponent<AudioSource> ().clip == backgroundMusic [GameControl.control.currentUser.backgroundMusic]) {
			circlePreview [0].fillAmount += 1.0f / 10f * Time.deltaTime;
		} else if (Camera.main.GetComponent<AudioSource> ().clip == winningMusic [GameControl.control.currentUser.winningMusic]) {
			circlePreview [1].fillAmount += 1.0f / Camera.main.GetComponent<AudioSource> ().clip.length * Time.deltaTime;
		} else if (Camera.main.GetComponent<AudioSource> ().clip == shootSound [GameControl.control.currentUser.shootIndex]) {
			circlePreview [2].fillAmount += 1.0f / Camera.main.GetComponent<AudioSource> ().clip.length * Time.deltaTime;
		} else if (Camera.main.GetComponent<AudioSource> ().clip == destroySound [GameControl.control.currentUser.destroyIndex]) {
			circlePreview [3].fillAmount += 1.0f / Camera.main.GetComponent<AudioSource> ().clip.length * Time.deltaTime;
		}
	}

	public void BackToMenu(){
		GameControl.control.SaveData (); //save
		GameControl.control.previousScene = 1; //previous scene is space shooter menu (1)
		StartCoroutine ("BackToMenuClick");
	}

	IEnumerator BackToMenuClick(){
		float fadeTime = Camera.main.GetComponent<Fading> ().BeginFade (1); //fade out
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("_MainMenuScreen");
	}

	void AddClicks(){ //add click listeners after load
		enemySettings.onClick.AddListener (delegate {
			ClickAudio.click.Click ();
		});
		audioSettings.onClick.AddListener (delegate {
			ClickAudio.click.Click ();
		});
		bgSettings.onClick.AddListener (delegate {
			ClickAudio.click.Click ();
		});
	}

	void SelectedButton(){ //change color of most recently active button to green, else white
		if (enemySettingsPanel.activeSelf) {
			enemySettings.GetComponent<Image> ().color = Color.green;
			audioSettings.GetComponent<Image> ().color = Color.white;
			bgSettings.GetComponent<Image> ().color = Color.white;
		} else if (audioSettingsPanel.activeSelf) {
			enemySettings.GetComponent<Image> ().color = Color.white;
			audioSettings.GetComponent<Image> ().color = Color.green;
			bgSettings.GetComponent<Image> ().color = Color.white;
		} else if (backgroundSettingsPanel.activeSelf) {
			enemySettings.GetComponent<Image> ().color = Color.white;
			audioSettings.GetComponent<Image> ().color = Color.white;
			bgSettings.GetComponent<Image> ().color = Color.green;
		}
	}


	//********************************ENEMY SETTINGS************************************//

	public void EnemySettings(){
		enemySettingsPanel.SetActive (true); //enable enemy settings panel
		audioSettingsPanel.SetActive (false);
		backgroundSettingsPanel.SetActive (false);
		SelectedButton (); //select "enemy" button so it is green
		//set current preferences on enemy buttons
		for (int i = 0; i < enemyButton.Length; i++) {
			enemyButton[i].GetComponent<Image>().color = colors [GameControl.control.currentUser.enemyColorIndex[i]];
		}
		enemyButton [0].onClick.Invoke (); //force click enemy 0
		AddEnemyClicks (); //add click listeners to enemy buttons
	}

	void AddEnemyClicks(){ //add click listeners after load
		for (int i = 0; i < enemyButton.Length; i++) {
			enemyButton [i].onClick.AddListener (delegate {
				ClickAudio.click.Click ();
			});
		}
		colorOptions.onValueChanged.AddListener (delegate {
			ClickAudio.click.Click ();
		});
	}

	public void Enemy0Settings(){
		currentEnemy = 0;
		EnemySelect ();
	}

	public void Enemy1Settings(){
		currentEnemy = 1;
		EnemySelect ();
	}

	public void Enemy2Settings(){
		currentEnemy = 2;
		EnemySelect ();
	}

	public void Enemy3Settings(){
		currentEnemy = 3;
		EnemySelect ();
	}

	public void Enemy4Settings(){
		currentEnemy = 4;
		EnemySelect ();
	}

	public void EnemySelect(){ //outline selected enemy and set fields
		for (int i = 0; i < enemyButton.Length; i++) { //disable all outlines
			enemyButton [i].GetComponent<Outline> ().enabled = false;
		}
		enemyButton [currentEnemy].GetComponent<Outline> ().enabled = true; //enable current enemy outline
		pointsToKillField.GetComponent<InputField> ().text = (GameControl.control.currentUser.pointsToKill[currentEnemy]).ToString (); //set user prefs
		pointsToKillField.GetComponent<InputField> ().onEndEdit.AddListener (delegate { //add listener
			pointsToKillFieldValueChangeHandler (pointsToKillField.GetComponent<InputField>());
		});
		pointsToKillFieldValueChangeHandler (pointsToKillField.GetComponent<InputField> ()); //call listener
		colorOptions.value = GameControl.control.currentUser.enemyColorIndex[currentEnemy]; //set user prefs
		colorOptions.onValueChanged.AddListener (delegate { //add listener
			colorOptionsValueChangedHandler (colorOptions);
		});
		colorOptionsValueChangedHandler (colorOptions); //call listener
	}

	void colorOptionsValueChangedHandler(Dropdown target){ //color options dropdown
		GameControl.control.currentUser.enemyColorIndex [currentEnemy] = target.value; //set color index of current enemy to value
		enemyButton [currentEnemy].GetComponent<Image> ().color = colors [GameControl.control.currentUser.enemyColorIndex [currentEnemy]]; //change color of enemy button accordingly
	}

	void pointsToKillFieldValueChangeHandler(InputField input){
		GameControl.control.currentUser.pointsToKill [currentEnemy] = int.Parse (input.text); //set points to kill current enemy to value
	}


	//********************************AUDIO SETTINGS************************************//

	private bool[] disableFirstClick = new bool[4];

	public void AudioSettings(){	
		enemySettingsPanel.SetActive (false);
		audioSettingsPanel.SetActive (true); //enable audio settings panel
		backgroundSettingsPanel.SetActive (false);
		SelectedButton (); //select "audio" button so it is green
		//disable first click if the dropdown isn't already at 0
		if (GameControl.control.currentUser.backgroundMusic != 0)
			disableFirstClick[0] = true;
		if (GameControl.control.currentUser.winningMusic != 0)
			disableFirstClick[1] = true;
		if (GameControl.control.currentUser.shootIndex != 0)
			disableFirstClick[2] = true;
		if (GameControl.control.currentUser.destroyIndex != 0)
			disableFirstClick[3] = true;
		backgroundMusicOptions.onValueChanged.AddListener (delegate { //add listener
			backgroundMusicOptionsValueChangedHandler (backgroundMusicOptions);
		});
		backgroundMusicOptions.value = GameControl.control.currentUser.backgroundMusic; //set user prefs
		backgroundMusicVolume.onValueChanged.AddListener (delegate { //add listener
			backgroundMusicVolumeValueChangedHandler (backgroundMusicVolume);
		});
		backgroundMusicVolume.value = GameControl.control.currentUser.volumeBM; //set user prefs
		winningMusicOptions.onValueChanged.AddListener (delegate { //add listener
			winningMusicOptionsValueChangedHandler (winningMusicOptions);
		});
		winningMusicOptions.value = GameControl.control.currentUser.winningMusic; //set user prefs
		winningMusicVolume.onValueChanged.AddListener (delegate { //add listener
			winningMusicVolumeValueChangedHandler (winningMusicVolume);
		});
		winningMusicVolume.value = GameControl.control.currentUser.volumeWM; //set user prefs
		shootSoundOptions.onValueChanged.AddListener (delegate { //add listener
			shootSoundOptionsValueChangedHandler (shootSoundOptions);
		});
		shootSoundOptions.value = GameControl.control.currentUser.shootIndex; //set user prefs
		shootVolume.onValueChanged.AddListener (delegate { //add listener
			shootVolumeValueChangedHandler (shootVolume);
		});
		shootVolume.value = GameControl.control.currentUser.volumeSS; //set user prefs
		destroySoundOptions.onValueChanged.AddListener (delegate { //add listener
			destroySoundOptionsValueChangedHandler (destroySoundOptions);
		});
		destroySoundOptions.value = GameControl.control.currentUser.destroyIndex; //set user prefs
		destroyVolume.onValueChanged.AddListener (delegate { //add listener
			destroyVolumeValueChangedHandler (destroyVolume);
		});
		destroyVolume.value = GameControl.control.currentUser.volumeDS; //set user prefs
	}

	void NormalDropdownColor(){ //set dropdowns to white
		backgroundMusicOptions.GetComponent<Image>().color = Color.white;
		winningMusicOptions.GetComponent<Image>().color = Color.white;
		shootSoundOptions.GetComponent<Image>().color = Color.white;
		destroySoundOptions.GetComponent<Image>().color = Color.white;
	}

	void KillTheMusic(){ //fade out preview music, fade in menu music
		//disable all volume sliders
		backgroundMusicVolume.enabled = false;
		winningMusicVolume.enabled = false;
		shootVolume.enabled = false;
		destroyVolume.enabled = false;
		//fade out
		float audio = Camera.main.GetComponent<AudioSource> ().volume;
		while (Camera.main.GetComponent<AudioSource> ().volume > 0.1f) { //decrease volume
			audio -= 0.005f * Time.deltaTime;
			Camera.main.GetComponent<AudioSource> ().volume = audio;
		}
		foreach (Image circle in circlePreview) {
			circle.enabled = false;
			circle.gameObject.SetActive (false);
		}
		Camera.main.GetComponent<AudioSource> ().clip = null;
		GameControl.control.GetComponent<AudioSource> ().UnPause (); //unpause menu music
		//fade in
		audio = 0f;
		GameControl.control.GetComponent<AudioSource> ().volume = audio;
		while (GameControl.control.GetComponent<AudioSource> ().volume < GameControl.control.currentUser.menuAudioVolume) { //increase volume
			audio += 0.005f * Time.deltaTime;
			GameControl.control.GetComponent<AudioSource> ().volume = audio;
		}
		GameControl.control.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.menuAudioVolume; //bring back up to user pref volume
		//enable all dropdowns and sliders
		backgroundMusicVolume.enabled = true;
		winningMusicVolume.enabled = true;
		shootVolume.enabled = true;
		destroyVolume.enabled = true;
		backgroundMusicOptions.enabled = true;
		winningMusicOptions.enabled = true;
		shootSoundOptions.enabled = true;
		destroySoundOptions.enabled = true;
		//reset dropdown colors
		NormalDropdownColor ();
	}

	void backgroundMusicOptionsValueChangedHandler(Dropdown choice){ //background music options dropdown
		if (!disableFirstClick[0]) { //if it isn't the first "click" (on load)
			GameControl.control.GetComponent<AudioSource> ().Pause (); //pause menu music
			Camera.main.GetComponent<AudioSource> ().clip = null; //make sure clip is null
			GameControl.control.currentUser.backgroundMusic = choice.value; //set user prefs
			//preview
			Camera.main.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeBM; //set volume
			Camera.main.GetComponent<AudioSource> ().clip = backgroundMusic [GameControl.control.currentUser.backgroundMusic]; //set clip
			Camera.main.GetComponent<AudioSource> ().Play (); //play preview
			circlePreview[0].gameObject.SetActive(true);
			circlePreview [0].fillAmount = 0;
			circlePreview[0].enabled = true; //start circle preview
			backgroundMusicOptions.GetComponent<Image>().color = Color.cyan; //change dropdown color to cyan
			//disable all dropdowns
			backgroundMusicOptions.enabled = false;
			winningMusicOptions.enabled = false;
			shootSoundOptions.enabled = false;
			destroySoundOptions.enabled = false;
			Invoke ("KillTheMusic", 10f); //10 second preview, then kill the music
		}
		disableFirstClick[0] = false;
	}

	void winningMusicOptionsValueChangedHandler (Dropdown choice){ //winning music options dropdown
		if (!disableFirstClick[1]) { //if it isn't the first "click" (on load)
			GameControl.control.GetComponent<AudioSource> ().Pause ();//pause menu music
			Camera.main.GetComponent<AudioSource> ().clip = null; //make sure clip is null
			GameControl.control.currentUser.winningMusic = choice.value; //set user prefs
			//preview
			Camera.main.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeWM; //set volume
			Camera.main.GetComponent<AudioSource> ().clip = winningMusic [GameControl.control.currentUser.winningMusic];  //set clip
			Camera.main.GetComponent<AudioSource> ().Play ();//play preview
			circlePreview[1].gameObject.SetActive(true);
			circlePreview [1].fillAmount = 0;
			circlePreview[1].enabled = true; //start circle preview
			winningMusicOptions.GetComponent<Image>().color = Color.cyan; //change dropdown color to cyan
			//disable all dropdowns
			backgroundMusicOptions.enabled = false;
			winningMusicOptions.enabled = false;
			shootSoundOptions.enabled = false;
			destroySoundOptions.enabled = false;
			Invoke ("KillTheMusic", Camera.main.GetComponent<AudioSource>().clip.length); //preview sound, and after it is done playing, kill the music
		}
		disableFirstClick[1] = false;
	}

	void shootSoundOptionsValueChangedHandler (Dropdown choice){ //shoot sound options dropdown
		if (!disableFirstClick[2]) { //if it isn't the first "click" (on load)
			GameControl.control.GetComponent<AudioSource> ().Pause (); //pause menu music
			Camera.main.GetComponent<AudioSource> ().clip = null; //make sure clip is null
			GameControl.control.currentUser.shootIndex = choice.value; //set user prefs
			//preview
			Camera.main.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeSS; //set volume
			Camera.main.GetComponent<AudioSource> ().clip = shootSound [GameControl.control.currentUser.shootIndex]; //set clip
			Camera.main.GetComponent<AudioSource> ().Play (); //play preview
			circlePreview[2].gameObject.SetActive(true);
			circlePreview [2].fillAmount = 0;
			circlePreview[2].enabled = true; //start circle preview
			shootSoundOptions.GetComponent<Image>().color = Color.cyan; //change dropdown color to cyan
			//disable all dropdowns
			backgroundMusicOptions.enabled = false;
			winningMusicOptions.enabled = false;
			shootSoundOptions.enabled = false;
			destroySoundOptions.enabled = false;
			Invoke ("KillTheMusic", Camera.main.GetComponent<AudioSource>().clip.length); //preview sound, and after it is done playing, kill the music
		}
		disableFirstClick[2] = false;
	}

	void destroySoundOptionsValueChangedHandler (Dropdown choice){ //destroy sound options dropdown
		if (!disableFirstClick[3]) { //if it isn't the first "click" (on load)
			GameControl.control.GetComponent<AudioSource> ().Pause (); //pause menu music
			Camera.main.GetComponent<AudioSource> ().clip = null; //make sure clip is null
			GameControl.control.currentUser.destroyIndex = choice.value; //set user prefs
			//preview
			Camera.main.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeDS; //set volume
			Camera.main.GetComponent<AudioSource> ().clip = destroySound [GameControl.control.currentUser.destroyIndex]; //set clip
			Camera.main.GetComponent<AudioSource> ().Play (); //play preview
			circlePreview[3].gameObject.SetActive(true);
			circlePreview [3].fillAmount = 0;
			circlePreview[3].enabled = true; //start circle preview
			destroySoundOptions.GetComponent<Image>().color = Color.cyan; //change dropdown color to cyan
			//disable all dropdowns
			backgroundMusicOptions.enabled = false;
			winningMusicOptions.enabled = false;
			shootSoundOptions.enabled = false;
			destroySoundOptions.enabled = false;
			Invoke ("KillTheMusic", Camera.main.GetComponent<AudioSource>().clip.length); //preview sound, and after it is done playing, kill the music
		}
		disableFirstClick[3] = false;
	}

	void backgroundMusicVolumeValueChangedHandler (Slider volume){
		GameControl.control.currentUser.volumeBM = volume.value; //set user prefs
		if (Camera.main.GetComponent<AudioSource> ().clip == backgroundMusic [GameControl.control.currentUser.backgroundMusic]) { //if the clip is previewing, change its volume to this value
			Camera.main.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeBM;
		}
	}

	void winningMusicVolumeValueChangedHandler (Slider volume){
		GameControl.control.currentUser.volumeWM = volume.value; //set user prefs
		if (Camera.main.GetComponent<AudioSource> ().clip == winningMusic [GameControl.control.currentUser.winningMusic]) { //if the clip is previewing, change its volume to this value
			Camera.main.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeWM;
		}
	}

	void shootVolumeValueChangedHandler (Slider volume){
		GameControl.control.currentUser.volumeSS = volume.value; //set user prefs
		if (Camera.main.GetComponent<AudioSource> ().clip == shootSound [GameControl.control.currentUser.shootIndex]) { //if the clip is previewing, change its volume to this value
			Camera.main.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeSS;
		}
	}

	void destroyVolumeValueChangedHandler (Slider volume){
		GameControl.control.currentUser.volumeDS = volume.value; //set user prefs
		if (Camera.main.GetComponent<AudioSource> ().clip == destroySound [GameControl.control.currentUser.destroyIndex]) { //if the clip is previewing, change its volume to this value
			Camera.main.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeDS;
		}
	}



	//********************************BACKGROUND SETTINGS************************************//

	public void BackgroundSettings(){
		enemySettingsPanel.SetActive (false);
		audioSettingsPanel.SetActive (false);
		backgroundSettingsPanel.SetActive (true); //enable background settings panel
		SelectedButton (); //select "background" button so it is green
		//set user prefs
		backgroundOptionsBr.value = GameControl.control.currentUser.bgIndex[0];
		backgroundOptionsSi.value = GameControl.control.currentUser.bgIndex[1];
		backgroundOptionsGo.value = GameControl.control.currentUser.bgIndex[2];
		screenSize.value = GameControl.control.currentUser.screenSizeIndex;
		//change previews according to user prefs
		BackgroundChoiceBr ();
		BackgroundChoiceSi ();
		BackgroundChoiceGo ();
		AddBackgroundClicks (); //add click listeners
	}

	void AddBackgroundClicks(){ //add click listeners to dropdowns after initialization
		backgroundOptionsBr.onValueChanged.AddListener (delegate {
			ClickAudio.click.Click ();
		});
		backgroundOptionsSi.onValueChanged.AddListener (delegate {
			ClickAudio.click.Click ();
		});
		backgroundOptionsGo.onValueChanged.AddListener (delegate {
			ClickAudio.click.Click ();
		});

		screenSize.onValueChanged.AddListener (delegate {
			ClickAudio.click.Click ();
		});
	}

	public void BackgroundChoiceBr(){
		GameControl.control.currentUser.bgIndex[0] = backgroundOptionsBr.value; //set user prefs
		//change preview material texture, temporarily make the preview image material something else, then back to its original material (refreshes image)
		previewMaterial[0].mainTexture = backgrounds [GameControl.control.currentUser.bgIndex[0]];
		previewImage[0].material = dynamicBackground;
		previewImage[0].material = previewMaterial[0];
	}

	public void BackgroundChoiceSi(){
		GameControl.control.currentUser.bgIndex[1] = backgroundOptionsSi.value; //set user prefs
		//change preview material texture, temporarily make the preview image material something else, then back to its original material (refreshes image)
		previewMaterial[1].mainTexture = backgrounds [GameControl.control.currentUser.bgIndex[1]];
		previewImage[1].material = dynamicBackground;
		previewImage [1].material = previewMaterial [1];
	}

	public void BackgroundChoiceGo(){
		GameControl.control.currentUser.bgIndex[2] = backgroundOptionsGo.value; //set user prefs
		//change preview material texture, temporarily make the preview image material something else, then back to its original material (refreshes image)
		previewMaterial[2].mainTexture = backgrounds [GameControl.control.currentUser.bgIndex[2]];
		previewImage[2].material = dynamicBackground;
		previewImage[2].material = previewMaterial[2];
	}

	public void ChangeScreenSize(){
		GameControl.control.currentUser.screenSizeIndex = screenSize.value; //set user prefs
	}
		
}
