using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour {
	public Text SensText;
	public Slider sensSli;

	private void Start() {

		sensSli.value = SwipeRegister.sensitivityOfSwipe;

	}

	public void SliderChangeSli() {
		SwipeRegister.sensitivityOfSwipe = sensSli.value;
		SensText.text = "Sensitivity: " + (int) (Mathf.Abs(sensSli.value - sensSli.maxValue - sensSli.minValue));
	}
}
