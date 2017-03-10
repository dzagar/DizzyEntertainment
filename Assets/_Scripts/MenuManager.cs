using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public MenuAnimation currentMenu;

	// Use this for initialization
	void Start () {
		ShowMenu (currentMenu); //show current menu
	}
	
	public void ShowMenu(MenuAnimation menu){
		if (currentMenu != null) {
			currentMenu.IsOpen = false;
		}
		currentMenu = menu;
		currentMenu.IsOpen = true;
	}
}
