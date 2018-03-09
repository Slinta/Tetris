using UnityEngine;
using System.Collections;

public class SwipeRegister : MonoBehaviour {

	public static float sensitivityOfSwipe = 200;
	Vector3 startOftouch;
	Vector3 currentPoint;
	public GameObject tetrisScriptObj;
	bool mult = false;
	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt("ControlScheme") == 1) {
			mult = true;
		}
		else {
			mult = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) {

			Touch touch = Input.touches[0];
				if (touch.phase == TouchPhase.Began) {
					startOftouch = touch.position;
			} 
				else if (touch.phase == TouchPhase.Moved) {
				currentPoint = touch.position;
				float f = Mathf.Abs(startOftouch.x) - Mathf.Abs(currentPoint.x);
				if (f <= -sensitivityOfSwipe) {
					tetrisScriptObj.SendMessage("Right");
					startOftouch = currentPoint;
				}
				if (f >= sensitivityOfSwipe) {
					tetrisScriptObj.SendMessage("Left");
					startOftouch = currentPoint;
				}

			} 
				else if (touch.phase == TouchPhase.Ended) {
				if (mult == false) {
					Vector2 line = (Vector2)touch.position - (Vector2)startOftouch;
					if (line.magnitude < 5 * (tetrisScriptObj.GetComponent<Camera>().orthographicSize / 14)) {
						return;
					}
					line = new Vector2( Mathf.Floor(line.x * 10), Mathf.Floor(line.y*10));
					if (Mathf.Max(Mathf.Abs(line.x), Mathf.Abs(line.y)) == Mathf.Abs(line.x)) {
						if (line.x > 0) {
							tetrisScriptObj.SendMessage("Right");
						}
						else {
							tetrisScriptObj.SendMessage("Left");
						}
					}
					else {
						if (line.y > 0) {
							tetrisScriptObj.SendMessage("Up");
						}
						else {
							tetrisScriptObj.SendMessage("DownSpecial", Mathf.Abs(line.y) / sensitivityOfSwipe);
							Debug.Log(Mathf.Abs(line.y) / sensitivityOfSwipe);
						}
					}
				}

				currentPoint = Vector3.zero;
				startOftouch = Vector3.zero;
			}
			

			
		}
		if (Input.GetMouseButtonDown(0)) {
			startOftouch = Input.mousePosition;
		} 
		else if (mult == true && Input.GetMouseButton(0)) {
			currentPoint = Input.mousePosition;
			float f = Mathf.Abs (startOftouch.x) - Mathf.Abs (currentPoint.x);
			if (f <= -sensitivityOfSwipe) {
				tetrisScriptObj.SendMessage ("Right");
				startOftouch = currentPoint;
			}
			if (f >= sensitivityOfSwipe) {
				tetrisScriptObj.SendMessage ("Left");
				startOftouch = currentPoint;
			}
		} 
		else if (Input.GetMouseButtonUp(0)) {
			if (mult == false) {
				Vector2 line = (Vector2)Input.mousePosition - (Vector2)startOftouch;
				if(line.magnitude < 5 * (tetrisScriptObj.GetComponent<Camera>().orthographicSize / 14)) {
					return;
				}
				line = new Vector2(Mathf.Floor(line.x * 10), Mathf.Floor(line.y * 10));
				if (Mathf.Max(Mathf.Abs(line.x), Mathf.Abs(line.y)) == Mathf.Abs(line.x)) {
					if (line.x > 0) {
						tetrisScriptObj.SendMessage("Right");
					}
					else {
						tetrisScriptObj.SendMessage("Left");
					}
				}
				else {
					if (line.y > 0) {
						tetrisScriptObj.SendMessage("Up");
					}
					else {
						tetrisScriptObj.SendMessage("DownSpecial", Mathf.Abs(line.y) / sensitivityOfSwipe);
						Debug.Log(Mathf.Abs(line.y) / sensitivityOfSwipe);
					}
				}
			}

			currentPoint = Vector3.zero;
			startOftouch = Vector3.zero;
		}
	}
}
