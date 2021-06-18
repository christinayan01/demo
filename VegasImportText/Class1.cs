/*
 * Vegas Proで動作する自作スクリプト。プロジェクトのイベントトラックのうち、テキストを全部出力します。
 * Scripting code for Vegas Pro. Exporting All text on event tracks of tracks.
 * 
 * Created on 2020/10/15.
 * christinayan01 by Takahiro Yanai.
 * 
 * DLLの格納先はモジュールパス参照
 * C:\Users\Administrator\AppData\Local\VEGAS Pro\17.0\Script Menu\
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ScriptPortal.Vegas;

namespace vegastest1 {
    public class EntryPoint {

        // parameters
        RichTextBox _rtb = null;
        int _currentTime = 0;                   // 開始位置
        int _timeLength = 5;                    // イベントの時間幅
        int _timeInterval = 0;                  // イベント間の間隔
        string _fontFamily = "Yu Gothic UI";    // フォント
        float _fontSize = 10;                   // フォントサイズ
        OFXColor _fontColor = new OFXColor(1, 1, 1);
        string _trackName = "Scripting Text";   // トラック名
        float _locationX = 0.5f;
        float _locationY = 0.5f;
        float _outlineWidth = 0;
        OFXColor _outlineColor = new OFXColor(0, 0, 0);
        int _Align = 4;

        public void FromVegas(Vegas vegas) {

#if true
            // form1
            ImportText.Form1 form = new ImportText.Form1();
            if (form.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            GetSettingsFromForm(form);
            AddMedias(vegas);
#else
            // Common dialog.
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "テキストファイル(*.txt)|*.txt";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            // テキストを追加
            AddMedias(vegas, ofd.FileName);
#endif
            MessageBox.Show("終了しました。");
            _currentTime = 0; //reset
        }

        void AddMedias(Vegas vegas, string file)
        {
            // トラックを追加
            Track track = new VideoTrack(0, _trackName);
            vegas.Project.Tracks.Add(track);
            track.Selected = true;
            // テキストファイルを開く
            // 1行につき1イベントを作ります
            System.IO.StreamReader sr = new System.IO.StreamReader(file, System.Text.Encoding.GetEncoding("shift_jis"));
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                // ビデオイベント作成
                VideoEvent videoEvent = new VideoEvent(Timecode.FromSeconds(_currentTime), Timecode.FromSeconds(_timeLength));
                track.Events.Add(videoEvent);
                // Takeを作成。ここにMediaのText情報が入っている
                Take take = GenerateTakeText(vegas, line);
                if (take != null)
                {
                    videoEvent.Takes.Add(take); // イベントにテキストTakeを登録
                }
                // 次の開始位置を決める
                _currentTime += _timeLength + _timeInterval;
            }
            sr.Close();
        }

        void AddMedias(Vegas vegas)
        {
            // トラックを追加
            Track track = new VideoTrack(0, _trackName);
            vegas.Project.Tracks.Add(track);
            track.Selected = true;
            for (int i=0; i< _rtb.Lines.Length;i++)
            {
                string line = _rtb.Lines[i];
                // ビデオイベント作成
                VideoEvent videoEvent = new VideoEvent(Timecode.FromSeconds(_currentTime), Timecode.FromSeconds(_timeLength));
                track.Events.Add(videoEvent);
                // Takeを作成。ここにMediaのText情報が入っている
                Take take = GenerateTakeText(vegas, line);
                if (take != null)
                {
                    videoEvent.Takes.Add(take); // イベントにテキストTakeを登録
                }
                // 次の開始位置を決める
                _currentTime += _timeLength + _timeInterval;
            }
        }

        Take GenerateTakeText(Vegas vegas, string text)
        {
            if (text.Length == 0)
                return null;

            // テキストを順次追加
            PlugInNode plugin = vegas.Generators.GetChildByName("VEGAS タイトルおよびテキスト"); // 日本語版だとプラグイン名はこれです。英語版は不明
            Media media = new Media(plugin); // これちょっと遅いです
            if (media.Generator == null)
                return null;
            if (!media.Generator.IsOFX)
                return null;
            // Text属性を得る。
            // 全属性を見たい場合はウォッチのOfxEffect.Parameters.Results Viewを見るとある。(多分21個)
            OFXEffect ofxEffect = media.Generator.OFXEffect;
            OFXStringParameter textParam = (OFXStringParameter)ofxEffect.FindParameterByName("Text");
            if (textParam == null)
                return null;

            // テキストをセット
            RichTextBox richtextBox1 = new RichTextBox();
            richtextBox1.Text = text;
            richtextBox1.SelectAll();    // 全テキストが対象
            richtextBox1.SelectionFont = new System.Drawing.Font(_fontFamily, _fontSize);    // フォント変更
            //richtextBox.SaveFile(file); // debug. フォントが変わったか確認してみよう

            // OFXEffectの"Text" Parameterに対して再登録する
            System.Xml.XmlDocument textValDoc = new System.Xml.XmlDocument();
            textValDoc.LoadXml("<OfxParamValue>" + textParam.Value + "</OfxParamValue>");
            System.Xml.XmlNode textPValue = textValDoc.FirstChild;
            textPValue.InnerText = richtextBox1.Rtf; // your new rtf words.
            textParam.Value = textPValue.InnerText;
            textParam.ParameterChanged();  // Apply changed.

            // これらはTextが見つかれば絶対全部見つかるからnullチェックしない
            OFXRGBAParameter textColorParam = (OFXRGBAParameter)ofxEffect.FindParameterByName("TextColor");
            OFXDouble2DParameter locationParam = (OFXDouble2DParameter)ofxEffect.FindParameterByName("Location");
            OFXChoiceParameter alignmentParam = (OFXChoiceParameter)ofxEffect.FindParameterByName("Alignment");
            OFXDoubleParameter outlineWidthParam = (OFXDoubleParameter)ofxEffect.FindParameterByName("OutlineWidth");
            OFXRGBAParameter outlineColorParam = (OFXRGBAParameter)ofxEffect.FindParameterByName("OutlineColor");
            OFXBooleanParameter shadowEnableParam = (OFXBooleanParameter)ofxEffect.FindParameterByName("ShadowEnable");
            //OFXStringParameter shadowColorParam = (OFXStringParameter)ofxEffect.FindParameterByName("ShadowColor");

            // パラメータセット //
            // TextColor
            textColorParam.Value = _fontColor;// new OFXColor(50.0f / 255.0, 100.0f / 255.0f, 150.0f / 255.0f);

            // Alignment
            // alignmentParam.Choiesを確認すること
            // OFXChoiceはReadOnly型なのでなにもできません
            alignmentParam.Value = alignmentParam.Choices[_Align]; //alignmentParam.Choices[7];

            //Location
            OFXDouble2D loc;
            loc.X = _locationX;
            loc.Y = _locationY;
            locationParam.Value = loc;

            // Outline
            outlineWidthParam.Value = _outlineWidth;
            outlineColorParam.Value = _outlineColor;// new OFXColor(10 / 255.0, 10 / 255.0f, 10 / 255.0f);

            MediaStream stream = media.Streams[0];
            //VideoEvent videoEvent = new VideoEvent(Timecode.FromSeconds(_currentTime), Timecode.FromSeconds(_timeLength));
            //track.Events.Add(videoEvent);
            Take take = new Take(stream);
            //videoEvent.Takes.Add(take);

            //_currentTime += _timeLength;

            return take;
        }

        // 画像トラックの作り方
        // https://www.youtube.com/watch?v=GdrXo_HiNZM
        TrackEvent AddMedia(Project project, string mediaPath, int trackIndex, Timecode start, Timecode length)
        {
#if false
            Media media = Media.CreateInstance(project, mediaPath);
            Track track = project.Tracks[trackIndex];
            if (track.MediaType == MediaType.Video)
            {
                VideoTrack videoTrack = (VideoTrack)track;
                VideoEvent videoEvent = videoTrack.AddVideoEvent(start, length);
                Take take = videoEvent.AddTake(media.GetVideoStreamByIndex(0));
                return videoEvent;
            }

            // サンプル：画像はこれで追加できる
            //AddMediaImage(vegas.Project, @"C:\Users\Administrator\Desktop\mouse.png", 0, );
            {
                Media media = Media.CreateInstance(vegas.Project, @"C:\Users\Administrator\Desktop\mouse.png");
                Track track = vegas.Project.Tracks[0];
                if (track.MediaType == MediaType.Video)
                {
                    VideoTrack videoTrack = (VideoTrack)track;
                    VideoEvent videoEvent = videoTrack.AddVideoEvent(Timecode.FromSeconds(1), Timecode.FromSeconds(5));
                    Take take = videoEvent.AddTake(media.GetVideoStreamByIndex(0));
                    return videoEvent;
                }
            }
#endif
            return null;
        }

        void GetSettingsFromForm(ImportText.Form1 form)
        {
            _rtb = form.GetRichText();

            _fontFamily = form.GetFontFamily();
            _fontSize = form.GetFontSize();

            Color fontCol = form.GetFontColor();
            _fontColor.R = fontCol.R;
            _fontColor.G = fontCol.G;
            _fontColor.B = fontCol.B;

            _locationX = form.GetLocationX();
            _locationY = form.GetLocationY();

            _outlineWidth = form.GetOutlineWidth();

            Color outlineCol = form.GetOutlineColor();
            _outlineColor.R = outlineCol.R;
            _outlineColor.G = outlineCol.G;
            _outlineColor.B = outlineCol.B;

            _Align = form.GetAlign();
        }

    }
}

