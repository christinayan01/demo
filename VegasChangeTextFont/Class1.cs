/*
 * Vegas Proで動作する自作スクリプト。プロジェクトのイベントトラックのうち、テキストを全部出力します。
 * Scripting code for Vegas Pro. Exporting All text on event tracks of tracks.
 * 
 * Created on 2020/10/15.
 * christinayan01 by Takahiro Yanai.
 * 
 * DLLの格納先はモジュールパス参照
 * C:\Users\tyanai1\AppData\Local\VEGAS Pro\17.0\Script Menu\
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScriptPortal.Vegas;
using ChangeTextFont;

namespace vegastest1 {
    public class EntryPoint {
        public void FromVegas(Vegas vegas) {

            // タイムライン上に何もないならやらない
            if (vegas.Project.MediaPool.Count == 0) {
                return;
            }

            // Common dialog.
#if false
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Input Font Family";
            if (sfd.ShowDialog() != DialogResult.OK) {
                return;
            }

            //Font name
            string font = System.IO.Path.GetFileNameWithoutExtension(sfd.FileName);
#else
            ChangeTextFont.Form1 sfd = new ChangeTextFont.Form1();
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // 2021.6.12
            // fontSize=0のときは、現在のテキストイベントのフォントサイズを使用します
            string fontFamily = sfd.textBox1_get();
            float fontSize = sfd.textBox2_get();
#endif

            // Vegasプロジェクト設定の一時フォルダに一時ファイルを出力するためのファイル名
            string tempRtfFileName = vegas.Project.Video.PrerenderedFilesFolder + "temp.rtf";

            // タイムライン上にあるテキストのイベントを全部集める
            // ただしトラック毎にDictionaryで格納する
            Dictionary<long, Dictionary<long, string>> trackTable = new Dictionary<long, Dictionary<long, string>>();   // トラック毎の箱
            foreach (Track track in vegas.Project.Tracks) {
                Dictionary<long, string> table = new Dictionary<long, string>();    // 1つのトラックのテキストリスト。キーは開始位置。値はテキスト
                foreach (TrackEvent trackEvent in track.Events) {
                    foreach (Take take in trackEvent.Takes) {
                        ChangeTextFont(take.Media, tempRtfFileName, fontFamily, fontSize);
                    }
                }
            }

            MessageBox.Show("終了しました。");
        }

        // Vegasのテキストイベントのフォントを変更する
        void ChangeTextFont(Media media, string file, string fontFamily, float fontSize) {
            if (media.Generator != null) {
                if (media.Generator.IsOFX) { // 2021.2.7 null参照で例外のバグ修正
                    // OFXEffect形式に変換して、Text情報を取得
                    OFXEffect ofxEffect = media.Generator.OFXEffect;
                    OFXStringParameter textParam = (OFXStringParameter)ofxEffect.FindParameterByName("Text");
                    if (textParam != null) {
                        string rtfData = textParam.Value;   // これがテキスト

                        // まず、temp.rtfを書き出します
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
                        writer.WriteLine(rtfData);
                        writer.Close();

                        // 次に、temp.rtfを読み込むとプレーンテキストになっています
                        RichTextBox richtextBox = new RichTextBox();
                        richtextBox.Rtf = System.IO.File.ReadAllText(file);

                        // 2021.6.12
                        // rtf形式のフォントを変更する
                        float fontSizeFinal = richtextBox.SelectionFont.Size;
                        if (fontSizeFinal > 0.0f)
                        {
                            fontSizeFinal = fontSize;
                        }
                        richtextBox.SelectAll();    // 全テキストが対象
                        richtextBox.SelectionFont = new System.Drawing.Font(fontFamily, fontSizeFinal);    // フォント変更
                        richtextBox.SaveFile(file); // debug. フォントが変わったか確認してみよう

                        // OFXEffectの"Text" Parameterに対して再登録する
                        // Referenced source: https://www.reddit.com/r/SonyVegas/comments/5lolln/scripting_changes_to_ofx_parameters/
                        System.Xml.XmlDocument textValDoc = new System.Xml.XmlDocument();
                        textValDoc.LoadXml("<OfxParamValue>" + textParam.Value + "</OfxParamValue>");

                        System.Xml.XmlNode textPValue = textValDoc.FirstChild;
                        textPValue.InnerText = richtextBox.Rtf; // your new rtf words.
                        textParam.Value = textPValue.InnerText;
                        textParam.ParameterChanged();   // Apply changed.

                        // temp.rtfを削除
                        if (System.IO.File.Exists(file)) {
                            System.IO.File.Delete(file);
                        }
                    }
                }
            }
        }
    }
}

