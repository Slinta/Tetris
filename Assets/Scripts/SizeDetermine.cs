using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SizeDetermine : MonoBehaviour {

	// Use this for initialization
	public Slider xSli;
	public Slider ySli;
	public Slider sensSli;
	int x;
	int y;

	public Text buttonText;
	public Text SensText;

	private void Start() {
		xSli.value = Tetris.width;
		ySli.value = Tetris.height;
	}

	public void SliderChangeSli(){
		SwipeRegister.sensitivityOfSwipe = sensSli.value;
		SensText.text = "Sens: " + (int)(sensSli.value);
	}

	public void SliderChangeX () {
		Tetris.width = (int)xSli.value;
		x = (int)xSli.value;

		buttonText.text = ("Field size: X = " + x + " Y = " + y);
	}
	public void SliderChangeY () {
		Tetris.height = (int)ySli.value;
		y = (int)ySli.value;

		buttonText.text = ("Field size: X = " + x + " Y = " + y);
	}
	public void ButtonPress(){
		SceneManager.LoadScene ("TetrisGame");
	}
	public void SetToDefalut() {
		Tetris.width = 10;
		Tetris.height = 22;
		Start();
	}

}
