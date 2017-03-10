using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class History : IComparable<History> {
	
	private DateTime timePlayed; //time history session starts
	private int score = 0; //score achieved in session
	private int level = 0; //level achieved in session

	public History(){
		timePlayed = System.DateTime.Now; //on creation, start time
	}

	public System.DateTime GetGamePlayTime(){ //getter function for time played
		return timePlayed;
	}

	public int GetGamePlayScore(){ //getter function for score
		return score;
	}

	public void SetGamePlayScore(int playScore){ //setter function for score
		score = playScore;
	}

	public string GetGamePlayLevel(){ //getter function for level
		if (level == 0)
			return "Bronze";
		else if (level == 1)
			return "Silver";
		else
			return "Gold";
	}

	public void SetGamePlayLevel(int playLevel){ //setter function for level
		level = playLevel;
	}

	public int CompareTo(History hist){ //sort by time played
		return DateTime.Compare (hist.timePlayed, this.timePlayed);
	}

}
