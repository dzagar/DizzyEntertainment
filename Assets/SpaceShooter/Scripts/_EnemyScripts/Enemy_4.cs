using UnityEngine;
using System.Collections;

//part is another serializable data storage class like WeaponDefinition
[System.Serializable]
public class Part {
	//these three fields need to be defined in the Inspector pane
	public string name; //name of this part
	public float health; //amt of health this part has
	public string[] protectedBy; //other parts that protect this
	//these 2 fields are set automatically in Start(), caching like this makes it faster, easier to find later
	public GameObject go; //gameobject of this part
	public Material mat; //material to show dmg 
}

public class Enemy_4 : Enemy {
	// Enemy_4 will start offscreen and then pick a random point on screen to
	//   move to. Once it has arrived, it will pick another random point and
	//   continue until the player has shot it down.

	public Vector3[]        points;  // Stores the p0 & p1 for interpolation
	public float            timeStart;  // Birth time for this Enemy_4
	public float            duration = 4;  // Duration of movement

	public Part[] parts; //the array of ships Parts

	void Start () {
		score = GameControl.control.currentUser.pointsToKill [4]; //load the score for destroying this enemy from playerprefs
		points = new Vector3[2];
		// There is already an initial position chosen by Main.SpawnEnemy()
		//   so add it to points as the initial p0 & p1
		points[0] = pos;
		points[1] = pos;

		InitMovement();

		//cache gameobject and material of each part in parts
		Transform t;
		foreach (Part prt in parts) {
			t = transform.Find (prt.name);
			if (t != null) {
				prt.go = t.gameObject;
				prt.mat = prt.go.GetComponent<Renderer> ().material;
			}
		}
	}

	void InitMovement() {
		// Pick a new point to move to that is on screen
		Vector3 p1 = Vector3.zero;
 		float esp = Main.S.enemySpawnPadding;
		Bounds cBounds = Utils.camBounds;
		p1.x = Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
		p1.y = Random.Range(cBounds.min.y + esp, cBounds.max.y - esp);

		points[0] = points[1];  // Shift points[1] to points[0]
		points[1] = p1;         // Add p1 as points[1]

		// Reset the time
		timeStart = Time.time;
	}

	public override void Move () {
		// This completely overrides Enemy.Move() with a linear interpolation

		float u = (Time.time-timeStart)/duration;
		if (u>=1) {  // if u >=1...
			InitMovement();  // ...then initialize movement to a new point
			u=0;
		}

		u = 1 - Mathf.Pow( 1-u, 2 );         // Apply Ease Out easing to u

		pos = (1-u)*points[0] + u*points[1]; // Simple linear interpolation
	}

	//will override OnCollisionEnter that is in Enemy.cs because of the way
	//MonoBehaviour declares common Unity fcns like OnCollisionEnter, override isnt necessary
	void OnCollisionEnter(Collision coll){
		GameObject other = coll.gameObject;
		switch (other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent<Projectile> ();
			//enemies dont take damage unless they're on screen
			//stops player from shooting them until they are visible
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy (other);
				break;
			}

			//hurt this enemy, find gameobject that was hit
			GameObject goHit = coll.contacts [0].thisCollider.gameObject;
			Part prtHit = FindPart (goHit);
			if (prtHit == null) {
				//just look for otherCollider instead
				goHit = coll.contacts [0].otherCollider.gameObject;
				prtHit = FindPart (goHit);
			}
			//check whether part is still protected
			if (prtHit.protectedBy != null) {
				foreach (string s in prtHit.protectedBy) {
					//if a protecting part hasnt been destroyed
					if (!Destroyed (s)) {
						//then dont damage this part yet
						Destroy (other); //destroy ProjectileHero
						return;
					}
				}
			}
			//not protected, so make it take damage; get dmg amt from Projectile.type & Main.W_DEFS
			prtHit.health -= Main.W_DEFS [p.type].damageOnHit;
			//show dmg on part
			ShowLocalizedDamage (prtHit.mat);
			if (prtHit.health <= 0) {
				//disable damaged part
				prtHit.go.SetActive (false);
			}
			//check to see if whole ship is destroyed
			bool allDestroyed = true; //assume destroyed
			foreach (Part prt in parts) {
				if (!Destroyed (prt)) {
					allDestroyed = false;
					break;
				}
			}
			if (allDestroyed) {
				Main.S.ShipDestroyed (this);
				//destroy audio
				audioIndexDS = GameControl.control.currentUser.destroyIndex; //get the audio index from playerprefs
				GameObject audioGO = new GameObject ();
				audioGO.AddComponent<AudioSource> ();
				audioGO.GetComponent<AudioSource>().clip = destroySoundLib [audioIndexDS]; //set audio source from destroy sound library
				audioGO.GetComponent<AudioSource> ().volume = GameControl.control.currentUser.volumeDS; //get volume
				audioGO.GetComponent<AudioSource>().Play (); //play audio
				InGameControls.enemy4Kills++; //increase enemy 4 kills
				//destroy this enemy
				Destroy (this.gameObject);
				Player.score += score;
			}
			Destroy (other); //destroy projectileHero
			break;
		}
	}

	//these two fcns find a part in this.parts by name or gameobject
	Part FindPart(string n){
		foreach (Part prt in parts) {
			if (prt.name == n) {
				return(prt);
			}
		}
		return(null);
	}

	Part FindPart(GameObject go){
		foreach (Part prt in parts) {
			if (prt.go == go) {
				return(prt);
			}
		}
		return(null);
	}

	//these fcns return true if part has been destroyed
	bool Destroyed(GameObject go){
		return(Destroyed (FindPart (go)));
	}

	bool Destroyed(string n){
		return(Destroyed (FindPart (n)));
	}

	bool Destroyed(Part prt){
		if (prt == null) {
			return (true); //yes it was destroyed
		}
		//returns result of the comparison prt.health <= 0
		return(prt.health <= 0);
	}

	//changes the color of just one part to red instead of whole ship
	void ShowLocalizedDamage(Material m){
		m.color = Color.red;
		remainingDamageFrames = showDamageForFrames;
	}
}
