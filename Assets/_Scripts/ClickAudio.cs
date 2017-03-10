using UnityEngine;
using System.Collections;

public class ClickAudio : MonoBehaviour {

	public static ClickAudio click;

	void Awake () { //persists over every scene
		if (click == null) {
			DontDestroyOnLoad (gameObject);
			click = this;
		} else if (click != this) {
			Destroy (gameObject);
		}
	}
	
	public void Click(){ //click noise
		click.GetComponent<AudioSource> ().Play ();
	}
}
