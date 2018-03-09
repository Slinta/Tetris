using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSetUp : MonoBehaviour {

	// Use this for initialization
	public GameObject Upbt;
	public GameObject Downbt;
	public GameObject Rightbt;
	public GameObject Leftbt;
	public GameObject UpSw;
	public GameObject DownSw;
	public GameObject swiper;

	void Start () {

		if (PlayerPrefs.GetInt("ControlScheme") == 0) {
			UpSw.SetActive(false);
			DownSw.SetActive(false);
			swiper.SetActive(false);
		}
		if (PlayerPrefs.GetInt("ControlScheme") == 1 ) {
			Upbt.SetActive(false);
			Downbt.SetActive(false);
			Rightbt.SetActive(false);
			Leftbt.SetActive(false);
		}
		if (PlayerPrefs.GetInt("ControlScheme") == 2) {
			Upbt.SetActive(false);
			Downbt.SetActive(false);
			Rightbt.SetActive(false);
			Leftbt.SetActive(false);
			UpSw.SetActive(false);
			DownSw.SetActive(false);
		}

	}
}
