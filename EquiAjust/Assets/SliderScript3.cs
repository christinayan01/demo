using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderScript3 : MonoBehaviour {

	public Text _text;
	public GameObject _sky;

	public void OnValueChanged()
	{
		int iValue = (int)GetComponent<Slider> ().value;
		_text.text = iValue.ToString();

		_sky.GetComponent<RotationCubeScript> ().UpdateCube();
	}
}
