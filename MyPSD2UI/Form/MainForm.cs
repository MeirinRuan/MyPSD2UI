using PSDFile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyPSD2UI
{
    public partial class MainForm : Form
	{
        List<LayerGroup> layerGroups = new List<LayerGroup>();

        public MainForm()
		{
			InitializeComponent();
		}

        /// <summary>
        /// 按照sectiontype来标记layergroup
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public LayerGroupPostion FindLayerPostion(Layer layer)
        {
            foreach (var layerInfo in layer.AdditionalInfo)
            {
                if (layerInfo.Key == "lsct")
                {
                    var layerSectionInfo = (LayerSectionInfo)layerInfo;

                    //Debug.WriteLine(layerSectionInfo.SectionType);

                    if (layerSectionInfo.SectionType == LayerSectionType.OpenFolder || layerSectionInfo.SectionType == LayerSectionType.ClosedFolder)
                    {
                        return LayerGroupPostion.StartLayer;
                    }
                    else if (layerSectionInfo.SectionType == LayerSectionType.SectionDivider)
                    {
                        return LayerGroupPostion.EndLayer;
                    }
                }
            }

            return LayerGroupPostion.MiddleLayer;
        }

        /// <summary>
        /// 插入layer group
        /// </summary>
        /// <param name="layers"></param>
        public void AddLayerGroup(List<Layer> layers)
        {
            var startIndex = 0;
            LayerGroup layerGroup = new LayerGroup();

            foreach (var layer in layers)
            {
                if (FindLayerPostion(layer) == LayerGroupPostion.StartLayer)
                {
                    if (startIndex == 0)
                    {
                        startIndex++;
                        layerGroup = new LayerGroup(layer);
                        continue;
                    }
                    else
                    {
                        startIndex++;
                        layerGroup.AddLayer(layer);
                        continue;
                    }
                }
                else if (startIndex > 0 && FindLayerPostion(layer) == LayerGroupPostion.EndLayer)
                {

                    if (startIndex == 1)
                    {
                        startIndex--;
                        layerGroups.Add(layerGroup);
                        continue;
                    }
                    else
                    {
                        startIndex--;
                        continue;
                    }
                }
                else if (FindLayerPostion(layer) == LayerGroupPostion.MiddleLayer)
                {
                    if (startIndex > 0)
                    {
                        layerGroup.AddLayer(layer);
                        continue;
                    }
                    else
                    {
                        layerGroup = new LayerGroup(layer);
                        layerGroups.Add(layerGroup);
                        continue;
                    }
                }
            }

           
        }

        private void button1_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				//Bitmap bmp = null;

				var psdFile = new PsdFile(openFileDialog1.FileName, new LoadContext());

                psdFile.Layers.Reverse();

                //构建layergrups
                AddLayerGroup(psdFile.Layers);

                //设置layergroup的坐标
                foreach (var layerGroup in layerGroups)
                {
                    layerGroup.SetRect();
                }

                //layergroup列表
                listBox1.DataSource = layerGroups;
                listBox1.DisplayMember = "Name";

                //预览
                foreach (var layerGroup in layerGroups)
                {
                    Graphics g = CreateGraphics();
                    Pen pen = new Pen(Color.Black);
                    var rect = new Rectangle(
                        layerGroup.Rect.X + flowLayoutPanel1.Location.X,
                        layerGroup.Rect.Y + flowLayoutPanel1.Location.Y,
                        layerGroup.Rect.Width,
                        layerGroup.Rect.Height);
                    g.DrawRectangle(pen, rect);
                }
            }
		}

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //显示坐标
            LayerGroup layerGroup = listBox1.SelectedItem as LayerGroup;
            layerGroup.SetRect();
            textBox1.Text = layerGroup.Rect.ToString();

            //显示layer的位图
            /* LayerGroup layerGroup = listBox1.SelectedItem as LayerGroup;

            flowLayoutPanel1.Controls.Clear();

            foreach (var layer in layerGroup.Layers)
            {
                Bitmap bitmap = layer.GetBitmap();
                PictureBox pic = new PictureBox();
                pic.Image = bitmap;
                pic.BorderStyle = BorderStyle.FixedSingle;
                pic.SizeMode = PictureBoxSizeMode.AutoSize;
                flowLayoutPanel1.Controls.Add(pic);
            }*/

            // 绘制控件
            /* LayerGroup layerGroup = listBox1.SelectedItem as LayerGroup;

            Graphics g = CreateGraphics();
            Pen pen = new Pen(Color.Black);
            var rect = new Rectangle(
                layerGroup.Rect.X + flowLayoutPanel1.Location.X,
                layerGroup.Rect.Y + flowLayoutPanel1.Location.Y,
                layerGroup.Rect.Width,
                layerGroup.Rect.Height);
            g.DrawRectangle(pen, rect);*/
        }
    }
}
