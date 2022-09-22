using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PSDFile;

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
				Bitmap bmp = null;

				var psdFile = new PsdFile(openFileDialog1.FileName, new LoadContext());

				bmp = psdFile.BaseLayer.GetBitmap();

				if (bmp == null)
					throw new ApplicationException();

				pictureBox1.Image = bmp;
				pictureBox1.Size = bmp.Size;

				/*foreach (var layer in psdFile.Layers)
				{
					Debug.WriteLine(layer.Name);
				}*/
				
            }
		}
	}
}
