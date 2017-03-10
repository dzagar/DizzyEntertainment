using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {

	public Texture2D fadeOutTexture; //black texture for fade to black
	public float fadeSpeed = 0.8f;

	private int drawDepth = -1000; //for redraw
	private float alpha = 1.0f; //alpha value
	private int fadeDir = -1; //fading in; 1 is fading out

	void OnGUI(){
		alpha += fadeDir * fadeSpeed * Time.deltaTime; //increase/decrease alpha w/ time
		alpha = Mathf.Clamp01 (alpha);

		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha); //create new gui color
		GUI.depth = drawDepth; //depth is -1000
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeOutTexture); //redraw the texture
	}

	public float BeginFade(int direction){
		fadeDir = direction;
		return fadeSpeed;
	}

	void OnLevelWasLoaded(){ //on level load, automatically starts fade
		alpha = 1.0f;
		BeginFade (-1);
	}
}
