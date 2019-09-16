using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI; //Input Field用に使う
using System.Windows.Forms; //OpenFileDialog用に使う
//using System.Drawing; //OpenFileDialog用に使う

public class OpenFileScript : MonoBehaviour {

    //public InputField _inputfieldpath;
    string _path = "C:\\Temp\\sample.jpg";
    string _file = "sample.jpg";
    string _dir = "C:\\Temp\\";
    Texture2D _texture = null;
    Texture2D _textureSmall = null;

    public string Dir { get { return _dir; } }
    public string File { get { return _file; } }

    int _image_width = 0;
    int _image_height = 0;
    public int ReadImageWidth { get { return _image_width; } }
    public int ReadImageHeight { get { return _image_height; } }

    public void OpenExistFile()
	{
#if UNITY_EDITOR
        if (!OpenFile_Windows())
            return;
#elif UNITY_STANDALONE_WIN 
        if (!OpenFile_Windows())
            return;
#elif UNITY_ANDROID
            //
#elif UNITY_IPHONE
            //
#endif
        // テクスチャを開く
        _texture = OpenTexture(_path);
        {//debug
            byte[] data = _texture.EncodeToJPG();
            System.IO.File.WriteAllBytes("C:/temp/readtex.jpg", data);
        }
        Debug.Log("テクスチャを呼んだ。");
        _textureSmall = ResizeTexture(_texture, 360, 180);
        {//debug
            byte[] data = _textureSmall.EncodeToJPG();
            System.IO.File.WriteAllBytes("C:/temp/readtexsmall.jpg", data);
        }

        // imageに画像を表示する
        GameObject obj = GameObject.Find("Image1");
        Image image = obj.GetComponent<Image>();
        image.material.mainTexture = _textureSmall;

        // skyboxに適用する
        GameObject sky = GameObject.Find("skybox1");
        sky.GetComponent<Renderer>().material.mainTexture = _texture;
    }

    //--------------------------------------------------------------------------
    bool OpenFile_Windows()
    {
        _path = "";
        OpenFileDialog open_file_dialog = new OpenFileDialog();
        open_file_dialog.Filter = "JPEG形式|*.jpg";
        open_file_dialog.CheckFileExists = false;
        if (open_file_dialog.ShowDialog() == DialogResult.OK)
        {
            _path = open_file_dialog.FileName;
            _file = System.IO.Path.GetFileName(_path);
            _dir = System.IO.Path.GetDirectoryName(_path) + "\\";
            return true;
        }
        return false;
    }

    //--------------------------------------------------------------------------
    // テクスチャを開く
    Texture2D OpenTexture(string path)
    {
        System.IO.BinaryReader bin = new System.IO.BinaryReader(new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read));
        byte[] rb = bin.ReadBytes((int)bin.BaseStream.Length);
        bin.Close();

        //int width = 0;
        //int height = 0;
        GetJpgWidthHeight(path, ref _image_width, ref _image_height);
        if (_image_width == 0 || _image_height == 0)
            return null;
        Texture2D texture = new Texture2D(_image_width, _image_height);
        texture.LoadImage(rb);
        texture.Apply();

        return texture;
    }

    //--------------------------------------------------------------------------
    void GetJpgWidthHeight(string path, ref int width, ref int height)
    {
        System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        width = System.Drawing.Image.FromStream(fs).Width;
        height = System.Drawing.Image.FromStream(fs).Height;
        fs.Close();
    }

    //--------------------------------------------------------------------------
    void ImportAssetJpg() {
		string dst = UnityEngine.Application.dataPath + "/Resources/" + _file;
		System.IO.File.Copy (_path, dst, true);
	}

    //--------------------------------------------------------------------------
    void JpgToTexture2D() {

    string aaa = _file.Substring (0, _file.Length - 4);
		_texture = (Texture2D)Resources.Load(aaa);
		//Texture2D t2d = _texture as Texture2D;
		//t2d.
		//Resources.
		//_texture360 = ResizeTexture(_texture as Texture2D, 360, 180);
		_textureSmall = Texture2D.Instantiate(_texture);
		TextureScale.Bilinear(_textureSmall, 360, 180);
		return;
			////
		/*
	//	string path = "";


		System.IO.FileStream stream = new System.IO.FileStream (path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
		System.IO.BinaryReader reader = new System.IO.BinaryReader (stream);
		byte[] data = reader.ReadBytes ((int)reader.BaseStream.Length);
		reader.Close ();

		System.Drawing.Image image = System.Drawing.Image.FromStream (stream);

		_texture = new Texture2D (image.Width, image.Height);
		_texture.LoadImage (data);
	*/	
	}

	public Texture2D ResizeTexture(Texture2D src, int dst_w, int dst_h)
	{
		Texture2D dst = new Texture2D(dst_w, dst_h, src.format, false);
		Color[] destPix = new Color[dst_w * dst_h];
		for (int y = 0; y < dst_h; ++y) {
			for (int x = 0; x < dst_w; ++x) {
				float inv_w = x * 1f / (dst_w - 1);
				float inv_h = y * 1f / (dst_h - 1);
				destPix[y * dst.width + x] = src.GetPixelBilinear(inv_w, inv_h);

				//int inv_w = x * src.width / dst_w;
				//int inv_h = x * src.width / dst_h;
				//Color color = src.GetPixel (inv_w, inv_h);
				//dst.SetPixel (x, y, color);

				//Color color = src.GetPixelBilinear (inv_w, inv_h);
				//dst.SetPixel (x, y, color);
			}
		}
		dst.SetPixels(destPix);
		dst.Apply();
		return dst;
	}
}
