using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	static public Player S; //Singleton

	//score
	public static int score;
	public Text scoreGT;

	//These fields control the movement of the ship
	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;

	//Ship status information
	[SerializeField]
	private float _shieldLevel = 1;
	//weapon fields
	public Weapon[] weapons; 
	public bool ____________________________;
	public Bounds bounds;
	public static int audioIndexSS;
	public AudioClip[] shootSoundLib;
	public AudioSource audio3;

	// Declare a new delegate type WeaponFireDelegate
	public delegate void WeaponFireDelegate();
	// Create a WeaponFireDelegate field named fireDelegate.
	public WeaponFireDelegate fireDelegate;

	void Awake(){
		S = this; //Set the Singleton
		bounds = Utils.CombineBoundsOfChildren(this.gameObject);
		//shoot audio
		audioIndexSS = GameControl.control.currentUser.shootIndex; //get audio index from playerprefs
		audio3.clip = shootSoundLib [audioIndexSS]; //set clip
		audio3.volume = GameControl.control.currentUser.volumeSS; //get volume
	}

	void Start(){
		//reset the weapons to start Player with 1 blaster
		ClearWeapons();
		weapons[0].SetType (WeaponType.blaster);
		score = 0;
		//find a reference to ScoreCounter GameObject
		GameObject scoreGO = GameObject.Find("ScoreCounter"); //2
		//get GUIText Component of that GameObject
		scoreGT = scoreGO.GetComponent<Text>(); //3
	}
	
	// Update is called once per frame
	void Update () {
		//pull info from the Input class
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");
		//change transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis*speed*Time.deltaTime;
		pos.y += yAxis*speed*Time.deltaTime;
		transform.position = pos;

		bounds.center = transform.position;
		//keep ship constrained to screen bounds
		Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
		if (off != Vector3.zero) {
			pos -= off;
			transform.position = pos;
		}
		//rotate ship to make it feel more dynamic
		transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult, 0);

		// Use the fireDelegate to fire Weapons
		// First, make sure the Axis("Jump") button is pressed 
		// Then ensure that fireDelegate isn't null to avoid an error
		if (Input.GetAxis ("Jump") == 1 && fireDelegate != null) { //1
			audio3.Play (); //play shoot sound
			fireDelegate ();
		}
		scoreGT.text = "Score: " + score;
	}
		
	//this variable holds a reference to the last triggering GameObject
	public GameObject lastTriggerGo = null;

	void OnTriggerEnter(Collider other){
		//find the tag of other.gameObject or its parent GameObjects
		GameObject go = Utils.FindTaggedParent(other.gameObject);
		//if there is a parent with a tag
		if (go != null) {
			//Make sure its not the same triggering go as last time
			if (go == lastTriggerGo) {
				return;
			}
			lastTriggerGo = go;
			if (go.tag == "Enemy") {
				//if the shield was triggered by an enemy, decrease shield level by 1
				shieldLevel--;
				//destroy the enemy
				Destroy (go);
				Main.numOfSpawns--;
				Debug.Log ("Num of enemies: " + Main.numOfSpawns);
			} else if (go.tag == "PowerUp") {
				//if shield is triggered by powerup
				AbsorbPowerUp (go);
			} else if (go.tag == "ProjectileEnemy") {
				shieldLevel--;
				Destroy (go);
			} else {
				print ("Triggered: " + go.name);
			}
		} else {
			//otherwise announce the original other.gameObject
			print ("Triggered: " + other.gameObject.name);
		}
	}

	public void AbsorbPowerUp( GameObject go ) {
		PowerUp pu = go.GetComponent<PowerUp>();
		switch (pu.type) {
		case WeaponType.shield: // If it's the shield
			shieldLevel++;
			break;

		default: // If it's any Weapon PowerUp
			// Check the current weapon type
			if (pu.type == weapons[0].type) {
			// then increase the number of weapons of this type
			Weapon w = GetEmptyWeaponSlot(); // Find an available weapon
				if (w != null) {
				// Set it to pu.type
				w.SetType(pu.type);
				}
			} else {
				// If this is a different weapon
				ClearWeapons();
				weapons[0].SetType(pu.type);
			}
			break;
		}
		pu.AbsorbedBy( this.gameObject );
	}

	Weapon GetEmptyWeaponSlot(){
		for (int i = 0; i < weapons.Length; i++) {
			if (weapons [i].type == WeaponType.none) {
				return(weapons [i]);
			}
		}
		return null;
	}

	void ClearWeapons(){
		foreach (Weapon w in weapons) {
			w.SetType (WeaponType.none);
		}
	}

	public float shieldLevel {
		get {
			return (_shieldLevel);
		}
		set {
			_shieldLevel = Mathf.Min (value, 4);
			//if shield is going to be set to less than 0
			if (value < 0) {
				Destroy (this.gameObject);
				InGameControls.recordTime = false; //stop timer
				SceneManager.LoadScene ("ShooterEnd"); //load end scene
			}
		}
	}
}
