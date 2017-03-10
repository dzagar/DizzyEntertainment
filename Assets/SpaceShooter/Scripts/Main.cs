using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
	static public Main S;
	static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;
	public GameObject[] prefabEnemies;
	public float enemySpawnPerSecond = 0.5f; //#Enemies/second
	public float enemySpawnPadding = 1.5f; //Padding for position
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public WeaponType[] powerUpFrequency = new WeaponType[] {
		WeaponType.blaster,
		WeaponType.blaster,
		WeaponType.spread,
		WeaponType.shield
	};
	public bool ________________;
	public WeaponType[] activeWeaponTypes;
	public float enemySpawnRate; //delay between enemy spawns

	public static int numOfSpawns = 0; //number of spawns
	public static bool spawning; //is an invoke in progress
	public static int level; //current level
	public static int maxScore; //current maximum score (level up score)
	public static int maxNumEnemies; //current maximum number of enemies to spawn
	public static bool[] prefabIndex; //disabling/enabling enemies in current level

	public static int audioIndexBG; //index of background audio
	public static int audioIndexWM; //index of winning music audio
	public AudioClip[] backgroundMusicLib; //background music library (array)
	public AudioClip[] winningMusicLib; //winning music library (array)
	public AudioSource audio1; //background audio source
	public AudioSource audio2; //winning music audio source
	public Texture[] backgrounds; //background image library (array)
	public Material backgroundMat; //material of game background
	public Material dynamicBackground; //material of dynamic background
	public Texture[] dynamicBG; //dynamic bg image library (array)

	public static History game;

	void Awake () {
		S = this;
		GameControl.control.GetComponent<AudioSource> ().Stop ();
		//set level settings
		level = 0; //start at bronze
		maxScore = GameControl.control.currentUser.levelUpScore[0]; //get max score for bronze level
		maxNumEnemies = GameControl.control.currentUser.numOfEnemies [0]; //get number of enemies to spawn in bronze level
		numOfSpawns = 0; //set number of spawns to 0
		LoadBr (); //set bool prefab index to the toggles in bronze
		//set background audio
		audioIndexBG = GameControl.control.currentUser.backgroundMusic; //get background audio index
		audio1.clip = backgroundMusicLib [audioIndexBG]; //set background audio source to the specified background music
		audio1.volume = GameControl.control.currentUser.volumeBM; //set volume
		audio1.Play (); //play audio
		//0.5 enemies per second = enemySpawnRate of 2
		enemySpawnRate = 1f/enemySpawnPerSecond;
		//Invoke call SpawnEnemy() once after a 2 second delay
		Invoke("SpawnEnemy", enemySpawnRate);
		spawning = true; //going to spawn
		//a generic Dictionary with WeaponType as the key
		W_DEFS = new Dictionary<WeaponType, WeaponDefinition>();
		foreach (WeaponDefinition def in weaponDefinitions) {
			W_DEFS [def.type] = def;
		}
		Canvas.ForceUpdateCanvases (); //update canvas
		//Set Utils.camBounds
		this.GetComponent<Camera>().ResetAspect(); //reset aspect ratio of camera to match screen size
		Utils.SetCameraBounds(this.GetComponent<Camera>());
	}
		
	static public WeaponDefinition GetWeaponDefinition(WeaponType wt){
		// Check to make sure that the key exists in the Dictionary
		// Attempting to retrieve a key that didn't exist, would throw an error, so the following if statement is important.
		if (W_DEFS.ContainsKey(wt)) {
			return(W_DEFS[wt]);
		}
		//returns definition for WeaponType.none, which means it failed to find the WeaponDefinition
		return(new WeaponDefinition());
	}

	void Start(){
		this.GetComponent<Camera> ().ResetAspect (); //reset aspect ratio of camera to match screen size
		Utils.SetCameraBounds(this.GetComponent<Camera>());
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for (int i = 0; i < weaponDefinitions.Length; i++) {
			activeWeaponTypes[i] = weaponDefinitions[i].type;
		}
		numOfSpawns = 0;
	}

	void Update(){
		if (Player.score >= maxScore) { //if player score exceeds the level up score on current level
			LevelUp (); //call level up
		}
		if (numOfSpawns < maxNumEnemies && spawning == false) {
			spawning = true;
			Invoke ("SpawnEnemy", enemySpawnRate);
		}
	}

	void LevelUp(){ //called on level up
		if (level == 0) { //if on bronze level
			maxScore = GameControl.control.currentUser.levelUpScore[1]; //get max score for silver
			maxNumEnemies = GameControl.control.currentUser.numOfEnemies[1]; //get number of enemies to spawn in silver
			LoadSi (); //set bool prefab index to the toggles in silver
		} else if (level == 1) { //if on silver level
			maxScore = GameControl.control.currentUser.levelUpScore[2]; //get max score for gold
			maxNumEnemies = GameControl.control.currentUser.numOfEnemies[2]; //get number of enemies to spawn in gold
			LoadGo (); //set bool prefab index to the toggles in gold
		}
		//do nothing on gold level since game does not stop
		//play winning music
		audioIndexWM = GameControl.control.currentUser.winningMusic;
		audio2.clip = winningMusicLib [audioIndexWM];
		audio2.volume = GameControl.control.currentUser.volumeWM;
		audio2.Play ();
		level++; //increase level
	}

	public void SpawnEnemy () {
		InGameControls.recordTime = true; //start timer as soon as first enemy spawns
		int ndx = -1;
		//pick random prefab to instantiate
		do {
			ndx = Random.Range (0, prefabEnemies.Length);
		} while(!prefabIndex [ndx]); //only instantiate enemy prefabs that are set to true
		GameObject go = Instantiate (prefabEnemies [ndx]) as GameObject;
		numOfSpawns++; //increase number of spawns
		//position enemy above screen with random x position
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x + enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - enemySpawnPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		if (numOfSpawns < maxNumEnemies) { //if the number of spawns is less than the max enemy spawn for current level
			//Call SpawnEnemy() again in a couple of seconds
			spawning = true;
			Invoke ("SpawnEnemy", enemySpawnRate);
		} else {
			spawning = false;
		}
	}

	public void ShipDestroyed(Enemy e){
		numOfSpawns--;
		//potentially generate powerup
		if (Random.value <= e.powerUpDropChance) {
			//random value generates a value between 0 and 1
			//if e.powerUpDropChance is 0.5f, a powerup will be generated 50% of the time
			//choose which powerup
			//pick one possibility in powerUpFrequency
			int ndx = Random.Range(0, powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency[ndx];
			//spawn powerUp
			GameObject go = Instantiate(prefabPowerUp) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp> ();
			//set it to proper weapon type
			pu.SetType(puType);
			//set to position of destroyed ship
			pu.transform.position = e.transform.position;
		}
	}

	void LoadBr(){
		backgroundMat.mainTexture = backgrounds [GameControl.control.currentUser.bgIndex [0]]; //set background texture according to bronze bg index
		//set dynamic background texture according to bg index
		Color tempColor = dynamicBackground.color;
		if (GameControl.control.currentUser.bgIndex[0] != 0) {
			dynamicBackground.mainTexture = dynamicBG [1];
			tempColor.a = 0.5f;
		} else {
			dynamicBackground.mainTexture = dynamicBG [0];
			tempColor.a = 1f;
		}
		dynamicBackground.color = tempColor;
		prefabIndex = new bool[5];
		//set bronze level bools to appropriate user preferences
		for (int i = 0; i < prefabIndex.Length; i++) {
			prefabIndex [i] = GameControl.control.currentUser.enemyBrBool [i];
		}
	}

	void LoadSi(){
		backgroundMat.mainTexture = backgrounds [GameControl.control.currentUser.bgIndex [1]]; //set background texture according to silver bg index
		//set dynamic background texture according to bg index
		Color tempColor = dynamicBackground.color;
		if (GameControl.control.currentUser.bgIndex[1] != 0) {
			dynamicBackground.mainTexture = dynamicBG [1];
			tempColor.a = 0.5f;
		} else {
			dynamicBackground.mainTexture = dynamicBG [0];
			tempColor.a = 1f;
		}
		dynamicBackground.color = tempColor;
		prefabIndex = new bool[5];
		//set silver level bools to appropriate user preferences
		for (int i = 0; i < prefabIndex.Length; i++) {
			prefabIndex [i] = GameControl.control.currentUser.enemySiBool [i];
		}
	}

	void LoadGo(){
		backgroundMat.mainTexture = backgrounds [GameControl.control.currentUser.bgIndex [2]]; //set background texture according to gold bg index
		//set dynamic background texture according to bg index
		Color tempColor = dynamicBackground.color;
		if (GameControl.control.currentUser.bgIndex[2] != 0) {
			dynamicBackground.mainTexture = dynamicBG [1];
			tempColor.a = 0.5f;
		} else {
			dynamicBackground.mainTexture = dynamicBG [0];
			tempColor.a = 1f;
		}
		dynamicBackground.color = tempColor;
		prefabIndex = new bool[5];
		//set gold level bools to appropriate user preferences
		for (int i = 0; i < prefabIndex.Length; i++) {
			prefabIndex [i] = GameControl.control.currentUser.enemyGoBool [i];
		}
	}

}
