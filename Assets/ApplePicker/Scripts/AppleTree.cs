using UnityEngine;
using System.Collections;

public class AppleTree : MonoBehaviour {

	//Prefab for instantiating apples
	public GameObject applePrefab;

	//Speed at which AppleTree moves in meters/second
	public static float speed = 1f;

	//Distance where AppleTree turns around
	public float leftAndRightEdge = 10f;

	//Chance that AppleTree will change direction
	public float chanceToChangeDirection = 0.05f;

	//Rate at which Apples will be instantiated
	public float secondsBetweenAppleDrops = 0.3f;

	// Use this for initialization
	void Start () {
	//Dropping Apples every second
		InvokeRepeating("DropApple", 2f, secondsBetweenAppleDrops);
	}

	void DropApple () {
		GameObject apple = Instantiate (applePrefab) as GameObject;
		apple.transform.position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//Basic movement
		Vector3 pos = transform.position;
		pos.x += speed * Time.deltaTime;
		transform.position = pos;

		//Changing direction
		if (pos.x < -leftAndRightEdge){
			speed = Mathf.Abs (speed); //move right
		}
		else if (pos.x > leftAndRightEdge){
			speed = -Mathf.Abs (speed); //move left
		}
	}

	void FixedUpdate () {
		if (Random.value < chanceToChangeDirection){
			speed *= -1; //change direction
		}
	}
}
