using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

	public static GameControl control;

	public UserProfile currentUser;
	public int previousScene;
	public Dictionary<string, string> login = new Dictionary<string, string>(); //login (username, hashed password) dictionary
	public Dictionary<string, UserProfile> userData = new Dictionary<string, UserProfile>(); //user data (username, user profile) dictionary

	// Use this for initialization
	void Awake () { //persist across all scenes
		if (control == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
			control.previousScene = -1; //main menu index
		} else if (control != this) {
			Destroy (gameObject);
		}
		LoadData (); //load dictionaries
		if (login.Count == 0) { //no users exist; add admin
			login.Add ("admin", PasswordSecurity.PasswordStorage.CreateHash ("admin"));
			userData.Add ("admin", new Admin());
			SaveData ();
		}
	}

	void OnApplicationQuit(){
		if (control.currentUser != null) {
			control.currentUser.LogOff ();
			control.SaveData ();
		}
	}

	public void SaveData(){
		//save login information
		BinaryFormatter bf1 = new BinaryFormatter();
		FileStream file1 = File.Create (Application.persistentDataPath + "/Login.gd");
		Dictionary<string, string> userList = new Dictionary<string, string>(login);
		bf1.Serialize(file1, userList);
		file1.Close();
		//save user information
		BinaryFormatter bf2 = new BinaryFormatter ();
		FileStream file2 = File.Create (Application.persistentDataPath + "/Users.gd");
		Dictionary<string, UserProfile> data = new Dictionary<string, UserProfile>(userData);
		bf2.Serialize (file2, data);
		file2.Close ();
	}

	public void LoadData(){
		//load login information
		if(File.Exists(Application.persistentDataPath + "/Login.gd")) {
			BinaryFormatter bf1 = new BinaryFormatter();
			FileStream file1 = File.Open(Application.persistentDataPath + "/Login.gd", FileMode.Open);
			Dictionary<string, string> userList = (Dictionary<string,string>)bf1.Deserialize(file1);
			file1.Close();
			login = new Dictionary<string, string> (userList);
		}
		//load user information
		if(File.Exists(Application.persistentDataPath + "/Users.gd")) {
			BinaryFormatter bf2 = new BinaryFormatter();
			FileStream file2 = File.Open(Application.persistentDataPath + "/Users.gd", FileMode.Open);
			Dictionary<string, UserProfile> data = (Dictionary<string, UserProfile>)bf2.Deserialize(file2);
			file2.Close();
			userData = new Dictionary<string, UserProfile> (data);
		}

	}

}
