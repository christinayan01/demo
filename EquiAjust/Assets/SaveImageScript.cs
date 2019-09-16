using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveImageScript : MonoBehaviour {


	public void OnClicked()
	{
		ExportJpg ("C:/temp/test.jpg");
	}

	void ExportJpg(string path){
		GameObject obj = GameObject.Find("Image1");
		RenderImageScript image = obj.GetComponent<RenderImageScript>();
		Texture2D tex = image._finalTex;
		byte[] data = tex.EncodeToJPG ();
		System.IO.File.WriteAllBytes (path, data);
	}
}
