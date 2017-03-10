using UnityEngine;
using System.Collections;

public class Enemy_3 : Enemy {
	//enemy 3 will move following a bezier curve which is a linear interpolation between
	//more than 2 pts
	public Vector3[] points;
	public float birthTime;
	public float lifeTime = 10;

	// Use this for initialization
	void Start () {
		score = GameControl.control.currentUser.pointsToKill [3]; //load the score for destroying this enemy from playerprefs
		points = new Vector3[3];//initialize pts
		//start position has already been set by Main.SpawnEnemy()
		points[0] = pos;
		//set xMin and xMax the same way that Main.SpawnEnemy() does
		float xMin = Utils.camBounds.min.x + Main.S.enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - Main.S.enemySpawnPadding;

		Vector3 v;
		//pick random middle position in bottom half of the screen
		v = Vector3.zero;
		v.x = Random.Range (xMin, xMax);
		points [2] = v;

		//set birthTime to the current time
		birthTime = Time.time;
	}
	
	public override void Move(){
		//bezier curves work based on a u value between 0 and 1
		float u = (Time.time - birthTime)/lifeTime;

		if (u > 1) {
			//this enemy 3 is dead
			Destroy(this.gameObject);
			Main.numOfSpawns--;
			return;
		}

		//interpolate three bezier curves
		Vector3 p01, p12;
		u = u - 0.2f * Mathf.Sin (u * Mathf.PI * 2);
		p01 = (1 - u) * points [0] + u * points [1];
		p12 = (1 - u) * points [1] + u * points [2];
		pos = (1 - u) * p01 + u * p12;
	}
}
