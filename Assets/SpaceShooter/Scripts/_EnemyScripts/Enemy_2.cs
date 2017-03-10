using UnityEngine;
using System.Collections;

public class Enemy_2 : Enemy {
	//Enemy 2 uses a sin wave to modify a 2 pt linear interpolation
	public Vector3[] points;
	public float birthTime;
	public float lifeTime = 10;
	public float sinEccentricity = 0.6f; //determines how much sin wave affects movement

	// Use this for initialization
	void Start () {
		score = GameControl.control.currentUser.pointsToKill [2]; //load the score for destroying this enemy from playerprefs
		//initialize points
		points = new Vector3[2];
		//find utils.cambounds
		Vector3 cbMin = Utils.camBounds.min;
		Vector3 cbMax = Utils.camBounds.max;

		Vector3 v = Vector3.zero;
		//pick any point on left side of screen
		v.x = cbMin.x - Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [0] = v;

		//pick any point on right side of screen
		v = Vector3.zero;
		v.x = cbMax.x + Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [1] = v;

		//possibly swap sides
		if (Random.value < 0.5f){
			//setting .x of each point to its negative will move it to the other side of screen
			points [0].x *= -1;
			points [1].x *= -1;
		}
		//set birthTime to the current time
		birthTime = Time.time;
	}
	
	public override void Move(){
		//bezier curves work based on a u value between 0 and 1
		float u = (Time.time - birthTime)/lifeTime;
		//if u > 1 then its been longer than lifeTime since birthTime
		if (u > 1) {
			//this enemy2 has finished its life
			Main.numOfSpawns--;
			Destroy(this.gameObject);
			return;
		}
		//adjust u by adding an easing curve based on a sine wave
		u = u + sinEccentricity*(Mathf.Sin(u*Mathf.PI*2));
		//interpolate two linear interpolation points
		pos = (1-u)*points[0] + u*points[1];
	}
}
