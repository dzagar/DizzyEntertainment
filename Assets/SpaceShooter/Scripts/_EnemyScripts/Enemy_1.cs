using UnityEngine;
using System.Collections;

//enemy 1 extends the enemy class

public class Enemy_1 : Enemy {
	//because enemy_1 extends enemy, the _____ bool won't work the same way in the inspector pane
	public float waveFrequency = 2; //# seconds for a full sine wave
	public float waveWidth = 4; //sine wave width in meters
	public float waveRotY = 45;

	private float x0 = -12345; //initial x value of pos
	private float birthTime;

	// Use this for initialization
	void Start () {
		// Set x0 to the initial x position of Enemy_1
		// This works fine because the position will have already
		//   been set by Main.SpawnEnemy() before Start() runs
		//   (though Awake() would have been too early!).
		// This is also good because there is no Start() method
		//   on Enemy. 
		x0 = pos.x;
		score = GameControl.control.currentUser.pointsToKill [1]; //load the score for destroying this enemy from playerprefs
		birthTime = Time.time;
	}
	
	//override move function on enemy
	public override void Move() {
		//because pos is a proprty, you cant directly set pos.x so get pos as an editable VECTOR3
		Vector3 tempPos = pos;
		//theta adjusts based on time
		float age = Time.time - birthTime;
		float theta = Mathf.PI * 2 * age / waveFrequency;
		float sin = Mathf.Sin (theta);
		tempPos.x = x0 + waveWidth * sin;
		pos = tempPos;

		//rotate a bit about y
		Vector3 rot = new Vector3(0, sin*waveRotY,0);
		this.transform.rotation = Quaternion.Euler (rot);

		//base.Move() still handles the movement down in y
		base.Move();
	}
}
