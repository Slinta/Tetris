using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayALeaderboard : MonoBehaviour {

	public GameObject textPrefab;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 10; i++){
			if (PlayerPrefs.HasKey ("Result" + i)) {
				GameObject newText = GameObject.Find("Text (" + i + ")");
				if (PlayerPrefs.GetInt ("Result" + i) != 0) {
					newText.GetComponent<Text> ().text = (PlayerPrefs.GetInt ("Result" + i) + PlayerPrefs.GetString ("Listing" + i));
				} 
				else {
					newText.GetComponent<Text> ().text = "Empty";
				}
			}
		}
	}
	
	// Update is called once per frame
	public void DeleteResult (int theOne) {
		GameObject.Find("Text (" + theOne + ")").GetComponent<Text>().text = "Deleted";
		if (PlayerPrefs.HasKey ("Result" + theOne) && PlayerPrefs.HasKey ("Listing" + theOne)) {
			for (int i = theOne; i < 9; i++) {
				PlayerPrefs.SetInt("Result" + (i), PlayerPrefs.GetInt ("Result" + (i + 1)));
				PlayerPrefs.SetString("Listing" + (i), PlayerPrefs.GetString ("Listing" + (i + 1)));
			}
			PlayerPrefs.SetInt("Result9", 0);
			PlayerPrefs.SetString("Listing9", null);
		}
	}
}
