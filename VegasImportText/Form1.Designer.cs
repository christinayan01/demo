
namespace ImportText
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox_FontFamily = new System.Windows.Forms.ComboBox();
            this.textBox_FontSize = new System.Windows.Forms.TextBox();
            this.pictureBox_FontColor = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_Location = new System.Windows.Forms.Label();
            this.label_Alignment = new System.Windows.Forms.Label();
            this.label_Outline = new System.Windows.Forms.Label();
            this.textBox_Outline = new System.Windows.Forms.TextBox();
            this.label_Outline2 = new System.Windows.Forms.Label();
            this.pictureBox_Outline = new System.Windows.Forms.PictureBox();
            this.textBox_LocationX = new System.Windows.Forms.TextBox();
            this.textBox_LocationY = new System.Windows.Forms.TextBox();
            this.label_LocationX = new System.Windows.Forms.Label();
            this.label_LocationY = new System.Windows.Forms.Label();
            this.comboBox_Align = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_FontColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Outline)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(0, 147);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(546, 266);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(352, 422);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 29);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(447, 422);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 29);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox_FontFamily
            // 
            this.comboBox_FontFamily.FormattingEnabled = true;
            this.comboBox_FontFamily.Location = new System.Drawing.Point(88, 13);
            this.comboBox_FontFamily.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_FontFamily.Name = "comboBox_FontFamily";
            this.comboBox_FontFamily.Size = new System.Drawing.Size(200, 23);
            this.comboBox_FontFamily.TabIndex = 4;
            this.comboBox_FontFamily.Text = "Yu Gothic UI";
            // 
            // textBox_FontSize
            // 
            this.textBox_FontSize.Location = new System.Drawing.Point(296, 14);
            this.textBox_FontSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_FontSize.Name = "textBox_FontSize";
            this.textBox_FontSize.Size = new System.Drawing.Size(51, 23);
            this.textBox_FontSize.TabIndex = 5;
            this.textBox_FontSize.Text = "10";
            this.textBox_FontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // pictureBox_FontColor
            // 
            this.pictureBox_FontColor.BackColor = System.Drawing.Color.White;
            this.pictureBox_FontColor.Location = new System.Drawing.Point(355, 13);
            this.pictureBox_FontColor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox_FontColor.Name = "pictureBox_FontColor";
            this.pictureBox_FontColor.Size = new System.Drawing.Size(24, 25);
            this.pictureBox_FontColor.TabIndex = 6;
            this.pictureBox_FontColor.TabStop = false;
            this.pictureBox_FontColor.Click += new System.EventHandler(this.pictureBox_FontColor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Font";
            // 
            // label_Location
            // 
            this.label_Location.AutoSize = true;
            this.label_Location.Location = new System.Drawing.Point(8, 52);
            this.label_Location.Name = "label_Location";
            this.label_Location.Size = new System.Drawing.Size(53, 15);
            this.label_Location.TabIndex = 9;
            this.label_Location.Text = "Location";
            // 
            // label_Alignment
            // 
            this.label_Alignment.AutoSize = true;
            this.label_Alignment.Location = new System.Drawing.Point(8, 84);
            this.label_Alignment.Name = "label_Alignment";
            this.label_Alignment.Size = new System.Drawing.Size(62, 15);
            this.label_Alignment.TabIndex = 10;
            this.label_Alignment.Text = "Alignment";
            // 
            // label_Outline
            // 
            this.label_Outline.AutoSize = true;
            this.label_Outline.Location = new System.Drawing.Point(8, 118);
            this.label_Outline.Name = "label_Outline";
            this.label_Outline.Size = new System.Drawing.Size(46, 15);
            this.label_Outline.TabIndex = 11;
            this.label_Outline.Text = "Outline";
            // 
            // textBox_Outline
            // 
            this.textBox_Outline.Location = new System.Drawing.Point(129, 113);
            this.textBox_Outline.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_Outline.Name = "textBox_Outline";
            this.textBox_Outline.Size = new System.Drawing.Size(51, 23);
            this.textBox_Outline.TabIndex = 12;
            this.textBox_Outline.Text = "0.0";
            this.textBox_Outline.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label_Outline2
            // 
            this.label_Outline2.AutoSize = true;
            this.label_Outline2.Location = new System.Drawing.Point(85, 118);
            this.label_Outline2.Name = "label_Outline2";
            this.label_Outline2.Size = new System.Drawing.Size(39, 15);
            this.label_Outline2.TabIndex = 13;
            this.label_Outline2.Text = "Width";
            // 
            // pictureBox_Outline
            // 
            this.pictureBox_Outline.BackColor = System.Drawing.Color.Black;
            this.pictureBox_Outline.Location = new System.Drawing.Point(186, 112);
            this.pictureBox_Outline.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox_Outline.Name = "pictureBox_Outline";
            this.pictureBox_Outline.Size = new System.Drawing.Size(24, 25);
            this.pictureBox_Outline.TabIndex = 14;
            this.pictureBox_Outline.TabStop = false;
            this.pictureBox_Outline.Click += new System.EventHandler(this.pictureBox_Outline_Click);
            // 
            // textBox_LocationX
            // 
            this.textBox_LocationX.Location = new System.Drawing.Point(109, 49);
            this.textBox_LocationX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_LocationX.Name = "textBox_LocationX";
            this.textBox_LocationX.Size = new System.Drawing.Size(51, 23);
            this.textBox_LocationX.TabIndex = 16;
            this.textBox_LocationX.Text = "0.5";
            this.textBox_LocationX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_LocationY
            // 
            this.textBox_LocationY.Location = new System.Drawing.Point(186, 49);
            this.textBox_LocationY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_LocationY.Name = "textBox_LocationY";
            this.textBox_LocationY.Size = new System.Drawing.Size(51, 23);
            this.textBox_LocationY.TabIndex = 17;
            this.textBox_LocationY.Text = "0.5";
            this.textBox_LocationY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label_LocationX
            // 
            this.label_LocationX.AutoSize = true;
            this.label_LocationX.Location = new System.Drawing.Point(89, 54);
            this.label_LocationX.Name = "label_LocationX";
            this.label_LocationX.Size = new System.Drawing.Size(14, 15);
            this.label_LocationX.TabIndex = 18;
            this.label_LocationX.Text = "X";
            // 
            // label_LocationY
            // 
            this.label_LocationY.AutoSize = true;
            this.label_LocationY.Location = new System.Drawing.Point(166, 55);
            this.label_LocationY.Name = "label_LocationY";
            this.label_LocationY.Size = new System.Drawing.Size(14, 15);
            this.label_LocationY.TabIndex = 19;
            this.label_LocationY.Text = "Y";
            // 
            // comboBox_Align
            // 
            this.comboBox_Align.FormattingEnabled = true;
            this.comboBox_Align.Location = new System.Drawing.Point(87, 81);
            this.comboBox_Align.Name = "comboBox_Align";
            this.comboBox_Align.Size = new System.Drawing.Size(123, 23);
            this.comboBox_Align.TabIndex = 20;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 464);
            this.Controls.Add(this.comboBox_Align);
            this.Controls.Add(this.label_LocationY);
            this.Controls.Add(this.label_LocationX);
            this.Controls.Add(this.textBox_LocationY);
            this.Controls.Add(this.textBox_LocationX);
            this.Controls.Add(this.pictureBox_Outline);
            this.Controls.Add(this.label_Outline2);
            this.Controls.Add(this.textBox_Outline);
            this.Controls.Add(this.label_Outline);
            this.Controls.Add(this.label_Alignment);
            this.Controls.Add(this.label_Location);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox_FontColor);
            this.Controls.Add(this.textBox_FontSize);
            this.Controls.Add(this.comboBox_FontFamily);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Import Text Settings";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_FontColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Outline)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox_FontFamily;
        private System.Windows.Forms.TextBox textBox_FontSize;
        private System.Windows.Forms.PictureBox pictureBox_FontColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_Location;
        private System.Windows.Forms.Label label_Alignment;
        private System.Windows.Forms.Label label_Outline;
        private System.Windows.Forms.TextBox textBox_Outline;
        private System.Windows.Forms.Label label_Outline2;
        private System.Windows.Forms.PictureBox pictureBox_Outline;
        private System.Windows.Forms.TextBox textBox_LocationX;
        private System.Windows.Forms.TextBox textBox_LocationY;
        private System.Windows.Forms.Label label_LocationX;
        private System.Windows.Forms.Label label_LocationY;
        private System.Windows.Forms.ComboBox comboBox_Align;
    }
}