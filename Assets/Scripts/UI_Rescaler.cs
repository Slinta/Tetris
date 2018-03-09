using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UI_Rescaler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		RectTransform panel = gameObject.GetComponent<RectTransform> ();
		panel.sizeDelta = new Vector2(356,(2500 / Tetris.height));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
