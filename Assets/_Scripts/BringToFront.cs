using UnityEngine;
using System.Collections;

public class BringToFront : MonoBehaviour {

	void OnEnable(){ //when object script is attached to (modal panel) is enabled
		transform.SetAsLastSibling (); //bring to front of screen
	}
}
