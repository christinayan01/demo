using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using System.Drawing;
using System.Drawing.Text;

namespace ImportText
{
    public partial class Form1 : Form
    {
        FontDialog fontDialog1 { get; set; } = new FontDialog();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton_FontFamily_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                //OKボタンが押されたら、フォントを設定する
                richTextBox1.SelectionFont = fontDialog1.Font;
                richTextBox1.SelectionColor = fontDialog1.Color;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //初期のフォントを設定
            fontDialog1.Font = richTextBox1.Font;
            // 初期の色を設定
            fontDialog1.Color = richTextBox1.ForeColor;
            // ユーザーが選択できるポイントサイズの最大値を設定する
            fontDialog1.MaxSize = 15;
            fontDialog1.MinSize = 10;
            // 存在しないフォントやスタイルをユーザーが選択すると
            // エラーメッセージを表示する
            fontDialog1.FontMustExist = true;
            // 横書きフォントだけを表示する
            fontDialog1.AllowVerticalFonts = false;
            // 色を選択できるようにする
            fontDialog1.ShowColor = true;
            // 取り消し線、下線、テキストの色などのオプションを指定可能にする
            // デフォルトがTrueのため必要はない
            fontDialog1.ShowEffects = true;
            // 固定ピッチフォント以外も表示する
            // デフォルトがFalseのため必要はない
            fontDialog1.FixedPitchOnly = false;
            // ベクタ フォントを選択できるようにする
            // デフォルトがTrueのため必要はない
            fontDialog1.AllowVectorFonts = true;

            FontFamily[] fontAry = GetFontList();
            foreach (FontFamily ff in fontAry)
            {
                comboBox_FontFamily.Items.Add(ff.Name);
            }

            // この順序はOfxChoice.Choiceと同じにすること。英語版だったら英語にしておくのが良い
            //string[] alignAry = new string[] { "左上", "上中央", "右上", "左中央", "中央", "右中央", "左下", "下中央", "右下" };
            string[] alignAry = new string[] { "Top left", "Top center", "Top right", "Middle left", "Middle center", "Middle right", "Bottom left", "Bottom center", "Bottom right" };
            foreach (string align in alignAry)
            {
                comboBox_Align.Items.Add(align);
            }
            comboBox_Align.SelectedIndex = 4; // 初期値は[中央]
        }

        private void toolStripButton_Bold_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size,  richTextBox1.SelectionFont.Style ^ FontStyle.Bold);
        }

        private void toolStripButton_Italic_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size, richTextBox1.SelectionFont.Style ^ FontStyle.Italic);
        }

        private void toolStripButton_Underline_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size, richTextBox1.SelectionFont.Style ^ FontStyle.Underline);
        }

        /////////////////////////////
        FontFamily[] GetFontList()
        {
            InstalledFontCollection fonts = new InstalledFontCollection();
            return fonts.Families;
        }

        /////////////////////////////        
        public RichTextBox GetRichText()
        {
            return richTextBox1;
        }
        public string GetFontFamily()
        {
            return comboBox_FontFamily.Text;
        }
        public float GetFontSize()
        {
            float val = 10;
            if (!float.TryParse(textBox_FontSize.Text, out val))
            {
                val = 10;
            }
            return val;
        }
        public Color GetFontColor()
        {
            return pictureBox_FontColor.BackColor;
        }
        public float GetLocationX()
        {
            float val = 0;
            if (!float.TryParse(textBox_LocationX.Text, out val))
            {
                val = 0;
            }
            return val; 
        }
        public float GetLocationY()
        {
            float val = 0;
            if (!float.TryParse(textBox_LocationY.Text, out val))
            {
                val = 0;
            }
            return val;
        }
        public float GetOutlineWidth()
        {
            float val = 0;
            if (!float.TryParse(textBox_Outline.Text, out val))
            {
                val = 0;
            }
            return val;
        }
        public Color GetOutlineColor()
        {
            return pictureBox_Outline.BackColor;
        }
        public int GetAlign()
        {
            return comboBox_Align.SelectedIndex;
        }
        /////////////////////////////        

        private void button_FontColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = pictureBox_FontColor.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pictureBox_FontColor.BackColor = cd.Color;
            }
        }

        private void button_Outline_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = pictureBox_Outline.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pictureBox_Outline.BackColor = cd.Color;
            }
        }

        private void pictureBox_FontColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = pictureBox_FontColor.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pictureBox_FontColor.BackColor = cd.Color;
            }

        }

        private void pictureBox_Outline_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = pictureBox_Outline.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pictureBox_Outline.BackColor = cd.Color;
            }
        }
    }
}
