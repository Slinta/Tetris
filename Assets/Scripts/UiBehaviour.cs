using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiBehaviour : MonoBehaviour {

	public delegate void TriggerPause();
	public static event TriggerPause OnPause;
	public Toggle buttons;
	public Toggle swipeMult;
	public Toggle swipe;

	public GameObject Exit;
	public GameObject Reset;

	private void Start() {
		if (buttons != null) {
			if (PlayerPrefs.GetInt("ControlScheme") == 0) {
				buttons.isOn = true;
				swipeMult.isOn = false;
				swipe.isOn = false;
			}
			else if (PlayerPrefs.GetInt("ControlScheme") == 1) {
				buttons.isOn = false;
				swipeMult.isOn = true;
				swipe.isOn = false;
			}
			else if (PlayerPrefs.GetInt("ControlScheme") == 2) {
				buttons.isOn = false;
				swipeMult.isOn = false;
				swipe.isOn = true;
			}
		}
	}

	public void PausePress () {

		Exit.SetActive(!Exit.activeSelf);
		Reset.SetActive(!Reset.activeSelf);
		OnPause();

	}

	public void ExitPress() {
		SceneManager.LoadScene(0);
	}

	public void ResetPress() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LeaderboardEnter () {
		SceneManager.LoadScene (2);
	}

	public void OptionsEnter() {
		SceneManager.LoadScene (3);
	}

	public void ToggleChange(string name){
		if (name == "Buttons") {
			if (buttons.isOn == true) {
				PlayerPrefs.SetInt ("ControlScheme", 0);
				swipeMult.isOn = false;
				swipe.isOn = false;
			}
		} else if (name == "SwipeMult") {
			if (swipeMult.isOn == true) {
				PlayerPrefs.SetInt ("ControlScheme", 1);
				buttons.isOn = false;
				swipe.isOn = false;
			}
		} else if (name == "Swipe") {
			if (swipe.isOn == true) {
				PlayerPrefs.SetInt ("ControlScheme", 2);
				buttons.isOn = false;
				swipeMult.isOn = false;
			}
		} else {
			Debug.LogError ("Unrecogniseable String sent");
		}
	}
}
