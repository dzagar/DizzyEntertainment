using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Admin : UserProfile { //custom class for administrator

	public Admin(){ //set values
		username = "admin";
		accountStatus = AccountStatus.ADMIN;
		loginAttempts = 0;
		userHistory = new Dictionary<DateTime, TimeSpan> ();
		shooterGameHistory = new List<History> ();
		applePickerHistory = new List<History> ();
		memoryGameHistory = new List<History> ();
		rpsGameHistory = new List<History> ();
		Settings ();
	}

	public override bool CreateUser(string name){ //create user
		if (GameControl.control.login.ContainsKey (name) || name == "") { //if the user already exists or the field has nothing in it
			return false;
		} else { //valid user
			GameControl.control.login.Add (name, PasswordSecurity.PasswordStorage.CreateHash (name)); //add username/password to the login dictionary
			GameControl.control.userData.Add (name, new UserProfile (name)); //add username and user profile to the user data dictionary
			GameControl.control.SaveData (); //save
			return true;
		}
	}

	public override bool DeleteUser(string name){ //delete user
		if (GameControl.control.userData [name].accountStatus != AccountStatus.ADMIN) { //cannot delete the admin
			GameControl.control.login.Remove (name); //remove user from login dictionary
			GameControl.control.userData.Remove (name); //remove user data from user data dictionary
			GameControl.control.SaveData (); //save
			return true;
		}
		return false;
	}

	public override bool ReleaseBlocks(string name){ //release block on user
		if (GameControl.control.userData [name].accountStatus != AccountStatus.ADMIN) { //admin can't be blocked in the first place
			GameControl.control.userData [name].accountStatus = AccountStatus.NEW; //set account status to new
			GameControl.control.userData [name].ChangePassword (name); //change the password of the user to the username
			GameControl.control.userData [name].loginAttempts = 0; //login attempts are reset
			return true;
		}
		return false;
	}
}
