using System.Windows.Forms;
using MyPSD2UI.Operate;

namespace MyPSD2UI
{
    public partial class MainWin : Form
    {
        public MainWin()
        {
            InitializeComponent();
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImagePsd _Psd = new ImagePsd(openFileDialog1.FileName);
                pictureBox1.Image = _Psd.PSDImage;
            }
        }
    }
}