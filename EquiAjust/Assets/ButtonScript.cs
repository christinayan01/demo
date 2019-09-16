using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {
	

	public void OnClicked()
	{
		Debug.Log ("onclicke");
		GameObject obj = GameObject.Find("Image1");
		Image image1 = obj.GetComponent<Image>();
		RenderImageScript scr = image1.GetComponent<RenderImageScript> ();
		scr.RemakeImageFast ();
	}
}
