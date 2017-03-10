using UnityEngine;
using System.Collections;

public class MenuAnimation : MonoBehaviour {
	private Animator anim;
	private CanvasGroup canGroup;

	public bool IsOpen{ //is the menu open
		get {return anim.GetBool ("IsOpen");}
		set { anim.SetBool ("IsOpen", value); }
	}

	void Awake(){
		anim = GetComponent<Animator> ();
		canGroup = GetComponent<CanvasGroup> ();
		RectTransform rect = GetComponent<RectTransform> ();
		rect.offsetMax = rect.offsetMin = new Vector2 (0, 0);
	}

	void Update(){
		if (!anim.GetCurrentAnimatorStateInfo (0).IsName ("Open")) {
			canGroup.blocksRaycasts = canGroup.interactable = false;
		} else {
			canGroup.blocksRaycasts = canGroup.interactable = true;
		}
	}
}
