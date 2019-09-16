using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

//using System.Threading;
//using System.Threading.Tasks;

public class RenderImageScript : MonoBehaviour {
	
	public Camera dispCamera;
	public int _resolutionWidth = 360;
	private Texture2D targetTexture;
	public Texture2D _finalTex;

	// Use this for initialization
	void Start() {
		RenderTexture tex = dispCamera.targetTexture;
		targetTexture = new Texture2D (tex.width, tex.height, TextureFormat.ARGB32, false);

		_finalTex = new Texture2D(_resolutionWidth, _resolutionWidth / 2, TextureFormat.ARGB32, false);
		GetComponent<Image>().material.mainTexture = _finalTex;
		_finalTex.Apply();
	}

	//bool doup = false;

	// Update is called once per frame
	void Update() {

	//	if (doup)
//return;
	//	doup = true;
	}

	int GetSliderVal ()
	{
		//int v = 0;
		GameObject obj = GameObject.Find("Slider1");
		Slider slider1 = obj.GetComponent<Slider>();
		return (int)slider1.value;
	}

	public void RemakeImage()
	{
	//	float sliderval = (float)GetSliderVal ();

		// 指定されたカメラのRenderTexture取得
		RenderTexture tex = dispCamera.targetTexture;

		// 画像データ読み込み
		dispCamera.Render();//レンダリング
		RenderTexture.active = dispCamera.targetTexture;
		targetTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		targetTexture.Apply();

		Vector3 rot = new Vector3 ();

		float idxX = 0;
		float idxY = 90;
		for (int y = 0; y < _finalTex.height; y++) {
			idxX = 0;//sliderval;// / 360.0f;
			for (int x = 0; x < _finalTex.width; x++) {
				// x : Yを0 - 360
				// y : Xを-90 - +90
				rot.Set (idxY, idxX, 0);
				dispCamera.transform.localEulerAngles = rot;

				dispCamera.Render();//レンダリング
				targetTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
				targetTexture.Apply();

				Color32 col = targetTexture.GetPixel (0,0);
				_finalTex.SetPixel(x,y, col);

				idxX += 360.0f / _finalTex.width;
	//			if (idxX >= 360.0f) {
	//				idxX -= 360.0f;
	//			}
			}
			idxY -= 180.0f / _finalTex.height;
		}
		_finalTex.Apply();

		//Color32[] finalcol = _finalTex.GetPixels32();
		//GetComponent<Image>().material.mainTexture = _finalTex;
	}

    public Texture2D ResizeTexture(Texture2D src, int dst_w, int dst_h) {
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

    public void RemakeImageFast()
	{
        GameObject obj = GameObject.Find("Button3");
        OpenFileScript filesc = obj.GetComponent<OpenFileScript>();
        _resolutionWidth = filesc.ReadImageWidth;

        RenderTexture tex = new RenderTexture(_resolutionWidth /4, _resolutionWidth/ 4, dispCamera.targetTexture.depth, dispCamera.targetTexture.format);
        GameObject camobj = GameObject.Find("BGCamera");
        Camera cam = camobj.GetComponent<Camera>();
        cam.targetTexture = tex;

        CreateEquirectangular(_resolutionWidth);
		//_finalTex.Apply ();
        //GetComponent<Image>().material.mainTexture = _finalTex;
    }

	//------------------------------------------------------------------------------
	// 【技術参考】https://stackoverflow.com/questions/34250742/converting-a-cubemap-into-equirectangular-panorama
	// 現在視点からキューブマップを作成する
	// ┌─────-┐	以下の内容で作成
	// ｜   +Z		｜	aspect=4:3
	// ｜-X +Y +X -Y｜
	// ｜   -Z      ｜
	// └─────-┘
	Texture2D CreateCubeMap(int resolution)	
	{
        Debug.Log("解像度=" + resolution.ToString());
		//CWaitCursor wait;

		// 360パノラマ画像
		int resolutionWidth = resolution;
		int resolutionHeight = resolution * 3 / 4;

		int viewWidth  = resolution / 4;
		int viewHeight = viewWidth;

		Texture2D image = new Texture2D(resolutionWidth, resolutionHeight);
#if false
        //image->allocateImage(resolutionWidth, resolutionHeight, 1, GL_RGBA, GL_UNSIGNED_BYTE); 
        //byte[] data = image.GetPixels();

        //osgViewer::Viewer* v = new osgViewer::Viewer; 
        //osg::ref_ptr<osg::GraphicsContext::Traits> traits = new osg::GraphicsContext::Traits; 
        //traits->target = osg::Camera::FRAME_BUFFER_OBJECT; 
        //traits->x =0; 
        //traits->y = 0; 
        //traits->width = viewWidth; 
        //traits->height = viewHeight; 
        //traits->windowDecoration = false; 
        //traits->doubleBuffer = false; 
        //traits->sharedContext = 0; 
        //traits->pbuffer = true; 

        //osg::GraphicsContext* _gc = osg::GraphicsContext::createGraphicsContext(traits.get()); 

        //osg::Camera* camera = v->getCamera(); 
        //camera->setGraphicsContext(_gc); 
        //camera->setViewport(new osg::Viewport(0, 0, viewWidth, viewHeight)); 

        // カラーバッファとデプスバッファを格納する画像を用意
        //osg::ref_ptr<osg::Image> colorImage = new osg::Image; 
        //colorImage->allocateImage(viewWidth, viewHeight, 1, GL_RGBA, GL_UNSIGNED_BYTE); 
        //camera->attach(osg::Camera::COLOR_BUFFER, colorImage.get()); 

        //v->setSceneData(GetTopRoot());
        //v->setDataVariance(osg::Object::DYNAMIC); 
        //v->setThreadingModel(osgViewer::Viewer::SingleThreaded); 
        //v->realize(); 

        //osg::Matrix viewMat, projMat;	// ビューマトリクス、プロジェクションマトリクス
        //osg::Vec3 eye = AirManipulator::Instance()->GetPosition();					

        //		std::vector<osg::Vec3> centers;	// 向き
        //		centers.push_back(osg::Vec3(-1, 0, 0));
        //		centers.push_back(osg::Vec3( 1, 0, 0));
        //		centers.push_back(osg::Vec3( 0,-1, 0));
        //		centers.push_back(osg::Vec3( 0, 1, 0));
        //		centers.push_back(osg::Vec3( 0, 0,-1));
        //		centers.push_back(osg::Vec3( 0, 0, 1));

        //		std::vector<osg::Vec3> ups;		// アップベクトル
        //		ups.push_back(osg::Vec3( 0, 0, 1));
        //		ups.push_back(osg::Vec3( 0, 0, 1));
        //		ups.push_back(osg::Vec3( 0, 0, 1));
        //		ups.push_back(osg::Vec3( 0, 0, 1));
        //		ups.push_back(osg::Vec3( 0, 1, 0));
        //		ups.push_back(osg::Vec3( 0,-1, 0));

        //		std::vector<osg::Vec2> pos;		// キューブマップに貼り付ける際のuv位置
        //		pos.push_back( osg::Vec2(0,				viewHeight) );
        //		pos.push_back( osg::Vec2(viewWidth*2,	viewHeight) );
        //		pos.push_back( osg::Vec2(viewWidth*3,	viewHeight) );
        //		pos.push_back( osg::Vec2(viewWidth,		viewHeight) );
        //		pos.push_back( osg::Vec2(viewWidth,		0) );
        //		pos.push_back( osg::Vec2(viewWidth,		viewHeight*2) );
#endif
        List<Vector3> rotList = new List<Vector3>();
		rotList.Add(new Vector3(0,-90,0));
		rotList.Add(new Vector3(0,90,0));
		rotList.Add(new Vector3(0,180,0));
		rotList.Add(new Vector3(0,0,0));
		rotList.Add(new Vector3(-90,0,0));
		rotList.Add(new Vector3(90,0,0));

		List<Vector2> posList = new List<Vector2>();
		posList.Add(new Vector2(0, viewHeight));
		posList.Add(new Vector2(viewWidth*2, viewHeight));
		posList.Add(new Vector2(viewWidth*3, viewHeight));
		posList.Add(new Vector2(viewWidth, viewHeight));
		posList.Add(new Vector2(viewWidth, viewHeight * 2));
		posList.Add(new Vector2(viewWidth, 0));

       // dispCamera.targetTexture.width = viewWidth;
        //dispCamera.targetTexture.height = viewHeight;

        //dispCamera.targetTexture ;
        //RenderTexture rendTex = new RenderTexture(viewWidth, viewHeight, 24);
        RenderTexture tex = dispCamera.targetTexture;
        //RenderTexture tex = rendTex;

        dispCamera.fieldOfView = 90f;
        // 画像データ読み込み
        //dispCamera.Render();//レンダリング
        RenderTexture.active = dispCamera.targetTexture;
        //RenderTexture.active = rendTex;

        for (int i=0; i<6; i++) {
			dispCamera.transform.localEulerAngles = rotList[i];
            Debug.Log(string.Format("rot = {0} {1} {2}", rotList[i].x , rotList[i].y, rotList[i].z));
            dispCamera.Render();//レンダリング
            targetTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            //Texture2D resizeTex = ResizeTexture(targetTexture, viewWidth, viewWidth);  // Rendertargetの解像度が変更できないので。。。

            targetTexture.Apply();

			// 所定の場所にセットする
			for (int j=0; j<viewHeight; j++) {
				//unsigned char* d1 = colorImage->data(0, j);	// 色
				//unsigned char* d2 = image->data(pos.at(i)._v[0], pos.at(i)._v[1] + j);	// 
				//memcpy(&d2[0], &d1[0], sizeof(unsigned char) * 4 * viewWidth);	// 1行だけコピー
				for (int k=0; k<viewWidth; k++) {
                    Color d1 = targetTexture.GetPixel(k, j);
                    //Color d1 = resizeTex.GetPixel(k, j);
                    image.SetPixel ((int)posList[i].x + k, (int)posList[i].y + j, d1);
				}
			}

            //debug
            {
                byte[] data = image.EncodeToPNG();
                System.IO.File.WriteAllBytes("C:/temp/imagecube_" + i.ToString() + ".jpg", data);
            }
        }
		//delete v; 
		//debug

		return image;
	}

    //------------------------------------------------------------------------------
    // 現在視点から全方位パノラマ画像を作成する
    Texture2D CreateEquirectangular(int width) 
    {
        Texture2D imageCube = CreateCubeMap(width);
        _finalTex = imageCube;
        //debug
        {
            byte[] data = imageCube.EncodeToPNG();
            System.IO.File.WriteAllBytes("C:/temp/imagecube.jpg", data);
        }

        /*
                //StartCoroutine(sub111());
                var task1 = await sub111();
                Debug.Log(task1);
               // return imageCube;
            }

            async Task<string> sub111()
            {
                return await Task.Run<string>(() =>
        */

        {

            //Texture2D imageCube = _finalTex;
            Texture2D equiImage = new Texture2D(imageCube.width, imageCube.width / 2);
            //equiImage->allocateImage(imageCube->s(), imageCube->s()/2, 1, GL_RGBA, GL_UNSIGNED_BYTE); 
            float u, v;         // Normalised texture coordinates, from 0 to 1, starting at lower left corner
            float phi, theta;   // Polar coordinates
            int cubeFaceWidth, cubeFaceHeight;

            cubeFaceWidth = imageCube.width / 4;    // 4 horizontal faces
            cubeFaceHeight = imageCube.height / 3;  // 3 vertical faces

            int equiW = equiImage.width;
            int equiH = equiImage.height;


            // スライダ用
            //float posCur = 0f;
            //float posMax = (float)equiH;
            GameObject obj1 = GameObject.Find("SliderProgressBar");
            Slider slider = obj1.GetComponent<Slider>();

            for (int j = 0; j < equiH; j++)
            {
                // スライダ
                slider.value = (float)j / equiH;

                //Rows start from the bottom
                v = 1f - ((float)j / (float)equiH);
                theta = v * Mathf.PI;

                for (int i = 0; i < equiW; i++)
                {
                    //Columns start from the left
                    u = (float)i / (float)equiW;
                    phi = u * 2f * Mathf.PI;

                    float x, y, z; // ベクトル
                    x = Mathf.Sin(phi) * Mathf.Sin(theta) * -1f;
                    y = Mathf.Cos(theta);
                    z = Mathf.Cos(phi) * Mathf.Sin(theta) * -1f;

                    float a;    // Intensity
                    a = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    a = Mathf.Max(a, Mathf.Abs(z));

                    //Vector Parallel to the unit vector that lies on one of the cube faces
                    float xa, ya, za;    // 単位ベクトル
                    xa = x / a;
                    ya = y / a;
                    za = z / a;

                    //Color color;
                    int xPixel, yPixel;
                    int xOffset, yOffset;

                    if (xa == 1)
                    {//Right
                        xPixel = (int)((((za + 1f) / 2f) - 1f) * cubeFaceWidth);
                        xOffset = 2 * cubeFaceWidth;
                        yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else if (xa == -1)
                    {//Left
                        xPixel = (int)((((za + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = 0;
                        yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else if (ya == 1)
                    {//Up
                        xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = cubeFaceWidth;
                        yPixel = (int)((((za + 1f) / 2f) - 1f) * cubeFaceHeight);
                        yOffset = 2 * cubeFaceHeight;
                    }
                    else if (ya == -1)
                    {//Down
                        xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = cubeFaceWidth;
                        yPixel = (int)((((za + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = 0;
                    }
                    else if (za == 1)
                    {//Front
                        xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = cubeFaceWidth;
                        yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else if (za == -1)
                    {//Back
                        xPixel = (int)((((xa + 1f) / 2f) - 1f) * cubeFaceWidth);
                        xOffset = 3 * cubeFaceWidth;
                        yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else
                    {// ここに来たらおかしいと判断すべき
                        xPixel = 0;
                        yPixel = 0;
                        xOffset = 0;
                        yOffset = 0;
                    }

                    xPixel = Mathf.Abs(xPixel);
                    yPixel = Mathf.Abs(yPixel);

                    xPixel += xOffset;
                    yPixel += yOffset;

                    // データ不正チェック
                    if (imageCube.width <= xPixel)
                    {
                        xPixel = imageCube.width - 1;
                    }
                    else if (imageCube.height <= yPixel)
                    {
                        yPixel = imageCube.height - 1;
                    }
                    else if (equiImage.width <= i)
                    {
                        i = equiImage.width - 1;
                    }
                    else if (equiImage.height <= j)
                    {
                        j = equiImage.height - 1;
                    }

                    //unsigned char* data1 = imageCube->data(xPixel, yPixel);
                    //unsigned char* data2 = equiImage->data(i, j);
                    //memcpy(&data2[0], &data1[0], sizeof(unsigned char) * 4);
                    //data2[3] = (unsigned char)255;
                    Color col1 = imageCube.GetPixel(xPixel, yPixel);
                    //Color col2 = equiImage.GetPixel (i, j);
                    equiImage.SetPixel(equiImage.width - 1 - i, j, col1);   // Xは逆にする

                    // デバッグ用(注意：超遅くなります)
                    //CString msg;
                    //msg.Format("x=%4d y=%4d | i=%4d, j=%4d", xPixel, yPixel, i, j);
                    //CAirStatusBar::Instance()->SetTextMessage(msg);
                    //Log::Instance()->Write("x=%4d y=%4d | i=%4d, j=%4d", xPixel, yPixel, i, j);
                }
            }

            //  slider.value = 0f;

            //debug
            {
                GameObject obj = GameObject.Find("Button3");
                OpenFileScript filesc = obj.GetComponent<OpenFileScript>();

                string filename = filesc.File.Substring(0, filesc.File.Length - 4); // 拡張子抜き
                string ext = "_adj.png";
                string path = filesc.Dir + filename + ext;
                byte[] data = equiImage.EncodeToPNG();
                System.IO.File.WriteAllBytes(path, data);
            }

            return equiImage;
            // yield return equiImage;
            //return "完了";

            //});
        }
    }

}
