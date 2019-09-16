using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotationCubeScript : MonoBehaviour {

	public Slider _slider1;
	public Slider _slider2;
	public Slider _slider3;
	//public GameObject _obj;

	public void UpdateCube() {
		Vector3 rot = new Vector3(-90 + _slider1.value, _slider2.value, _slider3.value);
		this.transform.localEulerAngles = rot;
	}
		
}
