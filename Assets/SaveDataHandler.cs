using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataHandler : MonoBehaviour {

	Game game;
	// Use this for initialization
	void Start () {
		game.date = System.DateTime.Now;
	}
	
	// Save function
	void Save () {
		
	}

	SaveData Load() {
		return 
	}

}

public class SaveData {
	public List<PlayedGame> games;
	
	public PlayedGame lastGame() {
		return games[games.Count - 1];
	}
}

public class PlayedGame {
	public System.DateTime dateStarted;
	public Vector2 field;
	public int score;
	public int durationSeconds;
	
}