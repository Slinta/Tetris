using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveDataHandler : MonoBehaviour {

	string path;
	bool noFileYet;
	SaveData currentSaveData;
	// Use this for initialization
	void Start () {
		path = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + "SaveData.tet";
	}
	
	// Save function
	public void Save (System.DateTime date, Vector2 fieldSize, bool[,] fieldData, int scored, int durationInSeconds) {
		if (currentSaveData == null) {
			NewSave(date,fieldSize,fieldData,scored,durationInSeconds);
		}

		BinaryFormatter formatter = new BinaryFormatter();
		using (FileStream file = File.Create(path)) {
			formatter.Serialize(file, currentSaveData);
			file.Close();
		}
	}
	void NewSave(System.DateTime date, Vector2 fieldSize, bool[,] fieldData, int scored, int durationInSeconds)
	{
		currentSaveData = new SaveData();
		currentSaveData.games.Add(new PlayedGame(date,fieldSize,fieldData,scored,durationInSeconds));
		BinaryFormatter formatter = new BinaryFormatter();
		using (FileStream file = File.Create(path)) {
			formatter.Serialize(file, currentSaveData);
			file.Close();
		}
	}

	SaveData Load() {
		BinaryFormatter formatter = new BinaryFormatter();
		if (File.Exists(path)) {
			using (FileStream file = File.Open(path, FileMode.Open)) {
				SaveData save = (SaveData)formatter.Deserialize(file);
				return save;
			}
		}
		else {
			noFileYet = true;
			return null;
		}
	}

}

[System.Serializable]
public class SaveData {
	public List<PlayedGame> games;
	
	public PlayedGame lastGame() {
		return games[games.Count - 1];
	}
}

[System.Serializable]
public class PlayedGame {

	public PlayedGame(System.DateTime date, Vector2 fieldSize, bool[,] fieldData, int scored, int durationInSeconds) {

		dateStarted = date;
		field = fieldSize;
		lastState = fieldData;
		score = scored;
		durationSeconds = durationInSeconds;

	}

	public System.DateTime dateStarted;
	public Vector2 field;
	public bool[,] lastState;
	public int score;
	public int durationSeconds;
}