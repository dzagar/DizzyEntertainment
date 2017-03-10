using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
	//Vector2s.x holds a min value; this is unusual but handy
	//and y a max value for a Random.Range() call later
	public Vector2 rotMinMax = new Vector2(15,90);
	public Vector2 driftMinMax = new Vector2(.25f, 2);
	public float lifeTime = 6f; //seconds PowerUP exists
	public float fadeTime = 4f; //seconds it will fade
	public bool ________________;
	public WeaponType type; //type of powerup
	public GameObject cube; //reference to cube child
	public TextMesh letter; //reference to text mesh
	public Vector3 rotPerSecond; //euler rotation speed
	public float birthTime;
	public AudioClip powerUpSound; //audio clip for powerup

	void Awake () {
		//find cube reference
		cube = transform.Find("Cube").gameObject;
		//find text mesh
		letter = GetComponent<TextMesh>();
		//set random velocity
		Vector3 vel = Random.onUnitSphere; //get random XYZ velocity
		//this fcn gives you a vector pt somewhere on the surface of the sphere w a
		//radius of 1m around origin
		vel.z = 0; //flatten vel to XY plane
		vel.Normalize(); //make length of vel 1
		//normalizing a Vector3 makes it length 1m
		vel *= Random.Range(driftMinMax.x, driftMinMax.y); //sets vel length to something between x and y
		//values of driftMinMax
		GetComponent<Rigidbody>().velocity = vel;
		//set rotation
		transform.rotation = Quaternion.identity; //equal to no rotation
		//set up rotPerSecond for Cube child using rotMinMax x and y
		rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x,rotMinMax.y));
		//check offscreen every 2 seconds
		InvokeRepeating("CheckOffscreen", 2f, 2f);
		birthTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		//manually rotate cube child every update()
		//multiply by time.time causes rotation to be time-based
		cube.transform.rotation = Quaternion.Euler (rotPerSecond * Time.time);
		//fade out powerUp over time; default to exist for 10 seconds and fade for 4
		float u = (Time.time - (birthTime+lifeTime))/fadeTime;
		//for lifetime seconds, u will be <= 0, then it will transition to 1 over fadeTime seconds
		//if u >= 1, destroy this powerup
		if (u >= 1) {
			Destroy (this.gameObject);
			return;
		}
		//use u to determine alpha value of cube and letter
		if (u < 0) {
			Color c = cube.GetComponent<Renderer> ().material.color;
			c.a = 1f - u;
			cube.GetComponent<Renderer> ().material.color = c;
			//fade letter, just not as much
			c = letter.color;
			c.a = 1f - (u * 0.5f);
			letter.color = c;
		}
	}

	//this SetType() differs from those on weapon and projectile
	public void SetType( WeaponType wt ) {
		// Grab the WeaponDefinition from Main
		WeaponDefinition def = Main.GetWeaponDefinition( wt );
		// Set the color of the Cube child
		cube.GetComponent<Renderer>().material.color = def.color;
		//letter.color = def.color; // We could colorize the letter too
		letter.text = def.letter; // Set the letter that is shown
		type = wt; // Finally actually set the type
	}

	public void AbsorbedBy(GameObject target){
		//play powerup audio clip
		GameObject audioGO = new GameObject ();
		audioGO.AddComponent<AudioSource> ();
		audioGO.GetComponent<AudioSource> ().clip = powerUpSound;
		audioGO.GetComponent<AudioSource> ().volume = 0.1f;
		audioGO.GetComponent<AudioSource> ().Play ();
		//fcn is called by Player class when powerup is collected
		//could tween into target and shrink in size, but for now just destroy
		Destroy(this.gameObject);
	}

	void CheckOffscreen(){
		//if powerup has drifted entirely off screen
		if (Utils.ScreenBoundsCheck (cube.GetComponent<Collider> ().bounds, BoundsTest.offScreen) != Vector3.zero) {
			//then destroy this game object
			Destroy(this.gameObject);
		}
	}
}
