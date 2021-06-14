using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeTextFont {
    public partial class Form1 : Form {
        public Form1()
        {
            InitializeComponent();
        }

        public string textBox1_get()
        {
            return this.textBox1.Text;
        }

        public float textBox2_get()
        {
            float result = 10.0f;
            if (!float.TryParse(this.textBox2.Text, out result))
            {
                result = 0.0f;
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
