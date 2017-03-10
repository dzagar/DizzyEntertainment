using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public enum AccountStatus{ //account status values
	NEW,
	NORMAL,
	BLOCKED,
	ADMIN
}

[Serializable]
public class UserProfile {
	#region UserInfo
	public string username;
	public int loginAttempts;
	public AccountStatus accountStatus;
	public Dictionary<DateTime, TimeSpan> userHistory; //a log of the login time and duration
	#endregion

	#region MainMenuConfigs
	public int menuBGIndex = 0; //menu background image index
	public int menuAudioIndex = 0;
	public float menuAudioVolume = 0.5f;
	#endregion

	#region ShooterConfigs
	public int[] numOfEnemies = new int[3]; //bronze, silver, gold number of enemies
	public int[] levelUpScore = new int[3]; //bronze, silver, gold level up scores
	public bool[] enemyBrBool = new bool[5]; //toggles for each enemy in the bronze level
	public bool[] enemySiBool = new bool[5]; //toggles for each enemy in the silver level
	public bool[] enemyGoBool = new bool[5]; //toggles for each enemy in the gold level
	public int[] pointsToKill = new int[5]; //points to kill each enemy
	public int[] enemyColorIndex = new int[5]; //color index for each enemy
	public int backgroundMusic = 0; //game background music index
	public int winningMusic = 0; //game winning music index
	public int shootIndex = 0; //game shoot sound index
	public int destroyIndex = 0; //game destroy sound index
	public float volumeBM = 0.5f; //game background music volume
	public float volumeWM = 0.5f; //game winning music volume
	public float volumeSS = 0.5f; //game shoot sound volume
	public float volumeDS = 0.5f; //game destroy sound volume
	public int[] bgIndex = new int[3]; //game background image indexes for bronze, silver, gold
	public int screenSizeIndex = 0; //screen resolution index
	#endregion

	#region History
	private DateTime loginTime = new DateTime (); //time user logs into the game package
	private DateTime logoutTime = new DateTime (); //time user logs out of the game package
	private TimeSpan duration = new TimeSpan(); //duration between login and logout
	public List<History> shooterGameHistory; //list of space shooter game sessions
	public List<History> applePickerHistory; //list of apple picker game sessions
	public List<History> memoryGameHistory; //list of memory game sessions
	public List<History> rpsGameHistory; //list of rock paper scissors game sessions
	#endregion

	public UserProfile(){}

	public UserProfile(string name){
		username = name;
		accountStatus = AccountStatus.NEW; //when new user is created, account type is NEW
		loginAttempts = 0; //no login attempts from new user
		userHistory = new Dictionary<DateTime, TimeSpan>();
		shooterGameHistory = new List<History> ();
		applePickerHistory = new List<History> ();
		memoryGameHistory = new List<History> ();
		rpsGameHistory = new List<History> ();
		Settings (); //set defaults
	}

	public bool ChangePassword(string pass){
		if (PasswordSecurity.PasswordStorage.VerifyPassword(pass, GameControl.control.login [username]) || pass == "") { //if the password is the same as before or the password is nothing
			return false;
		} else {
			GameControl.control.login [username] = PasswordSecurity.PasswordStorage.CreateHash (pass); //set user login value
			if (!PasswordSecurity.PasswordStorage.VerifyPassword(username, GameControl.control.login [username])) //if the password isnt the same as the user (double check)
				accountStatus = AccountStatus.NORMAL; //account is normal
			return true;
		}
	}

	public void Settings(){ //default settings for user
		//set number of enemies and level up score in each level
		numOfEnemies [0] = 5;
		numOfEnemies [1] = 10;
		numOfEnemies [2] = 15;
		levelUpScore [0] = 500;
		levelUpScore [1] = 1250;
		levelUpScore [2] = 2000;

		//set all enemy toggles to true
		for (int i = 0; i < enemyBrBool.Length; i++){
			enemyBrBool [i] = true;
		}
		for (int i = 0; i < enemySiBool.Length; i++) {
			enemySiBool [i] = true;
		}
		for (int i = 0; i < enemyGoBool.Length; i++) {
			enemyGoBool [i] = true;
		}

		//set all enemy kill points to 50
		for (int i = 0; i < pointsToKill.Length; i++) {
			pointsToKill [i] = 50;
		}

		//set all enemy color indexes to 0 (yellow)
		for (int i = 0; i < enemyColorIndex.Length; i++) {
			enemyColorIndex [i] = 0;
		}

		//set all shooter game background indexes to 0 (original)
		for (int i = 0; i < bgIndex.Length; i++) {
			bgIndex [i] = 0;
		}
	}

	public void LogOn(){ //user is in the system
		loginTime = System.DateTime.Now;
	}

	public void LogOff(){ //user is leaving the system
		logoutTime = System.DateTime.Now;
		duration = logoutTime-loginTime;
		userHistory.Add (loginTime, duration); //add this session to user history
	}

	//if user somehow gets into the admin user accounts panel, do nothing
	public virtual bool CreateUser(string placeholder){return false;}
	public virtual bool DeleteUser(string placeholder){return false;}
	public virtual bool ReleaseBlocks(string placeholder){return false;}
}
