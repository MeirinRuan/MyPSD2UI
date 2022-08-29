using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyPSD2UI.Operate;

namespace MyPSD2UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImagePsd _Psd = new ImagePsd(openFileDialog1.FileName);
                pictureBox1.Image = _Psd.PSDImage;
            }
        }
    }
}
