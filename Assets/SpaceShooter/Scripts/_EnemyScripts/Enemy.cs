using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public float speed = 10f; //speed in meter/second
	public float fireRate = 0.3f; //seconds/shot
	public float health = 10;
	public int score = 100; //points scored for destroying this
	public int showDamageForFrames = 2; //# frames to show damage
	public float powerUpDropChance = 1f; //chance to drop a powerup
	public bool ________________;
	public Color[] originalColors;
	public Material[] materials; //all the Materials of this and its children
	public int remainingDamageFrames = 0; //damage frames left
	public Bounds bounds; //the Bounds of this and its children
	public Vector3 boundsCenterOffset; //dist of bounds.center from position

	//Destroy audio
	[HideInInspector]
	public int audioIndexDS; //index of destroy sound
	public AudioClip[] destroySoundLib; //destroy sound library (array of audio clips)
	public AudioSource audio4; //audio source prefab

	void Awake(){
		score = GameControl.control.currentUser.pointsToKill [0]; //load the score for destroying this enemy from playerprefs
		materials = Utils.GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int i = 0; i < materials.Length; i++) {
			originalColors [i] = materials [i].color;
		}
		InvokeRepeating ("CheckOffscreen", 0f, 2f);
	}

	// Update is called once per frame
	void Update () {
		Move ();
		if (remainingDamageFrames > 0) {
			remainingDamageFrames--;
			if (remainingDamageFrames == 0) {
				UnShowDamage ();
			}
		}
	}

	public virtual void Move(){
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}

	//This is a Property, a method that acts like a field
	public Vector3 pos {
		get {
			return (this.transform.position);
		}
		set {
			this.transform.position = value;
		}
	}

	void CheckOffscreen(){
		//If bounds are still default value
		if (bounds.size == Vector3.zero) {
			//then set them
			bounds = Utils.CombineBoundsOfChildren(this.gameObject);
			//also find diff between bounds.center and transform.position
			boundsCenterOffset = bounds.center - transform.position;
		}
		//every time, update bounds to current position
		bounds.center = transform.position + boundsCenterOffset;
		//check to see whether bounds are completely offscreen
		Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen);
		if (off != Vector3.zero) {
			//if this enemy has gone off the bottom edge of screen
			if (off.y < 0) {
				//then destroy it
				Main.numOfSpawns--;
				Destroy(this.gameObject);
			}
		}
	}

	void OnCollisionEnter( Collision coll ) {
		GameObject other = coll.gameObject;
		switch (other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent<Projectile>();
			// Enemies don't take damage unless they're onscreen
			// This stops the player from shooting them before they are visible
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy(other);
				break;
			}
			// Hurt this Enemy
			ShowDamage();
			// Get the damage amount from the Projectile.type & Main.W_DEFS
			health -= Main.W_DEFS[p.type].damageOnHit;
			if (health <= 0) {
				//tell main singleton that this ship has been destroyed
				if (health == 0) //call ShipDestroyed only once to avoid multiple power ups
					Main.S.ShipDestroyed(this);
				//play destroy audio
				audioIndexDS = GameControl.control.currentUser.destroyIndex; //get the index of the audio to be played
				GameObject audioGO = new GameObject (); 
				audioGO.AddComponent<AudioSource> ();
				audioGO.GetComponent<AudioSource>().clip = destroySoundLib [audioIndexDS]; //add the specified clip
				audioGO.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeDS; //get volume of destroy audio
				audioGO.GetComponent<AudioSource>().Play (); //play the audio
				switch (this.GetType ().ToString()) { //check the class of the enemy
				case "Enemy_3":
					InGameControls.enemy3Kills++; //increase enemy 3 kills
					break;
				case "Enemy_2":
					InGameControls.enemy2Kills++; //increase enemy 2 kills
					break;
				case "Enemy_1":
					InGameControls.enemy1Kills++; //increase enemy 1 kills
					break;
				case "Enemy":
					InGameControls.enemy0Kills++; //increase enemy 0 kills
					break;
				}
				// Destroy this Enemy
				Destroy(this.gameObject);
				Player.score += score;
			}
			Destroy(other);
			break;
		}
	}

	void ShowDamage(){
		foreach (Material m in materials) {
			m.color = Color.red;
		}
		remainingDamageFrames = showDamageForFrames;
	}

	void UnShowDamage(){
		for (int i = 0; i < materials.Length; i++) {
			materials [i].color = originalColors [i];
		}
	}
}
