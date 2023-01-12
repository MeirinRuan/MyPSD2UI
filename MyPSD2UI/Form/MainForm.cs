using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PSDFile;

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
                    g.DrawString(layerGroup.Name, new Font("宋体",14), new SolidBrush(Color.Black), rect);
                    
                    //显示位图(无法适配所有psd文件)
                    /*foreach( var layer in layerGroup.Layers)
                    {
                        if (layer.HasImage)
                            g.DrawImage(layer.GetBitmap(), rect);
                    }*/
                }

                //layergroup列表
                checkedListBox1.DataSource = layerGroups;
                checkedListBox1.DisplayMember = "Name";

                //默认全选
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
                textBox1.Clear();
            }
		}

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //显示坐标
            LayerGroup layerGroup = checkedListBox1.SelectedItem as LayerGroup;

            //layerGroup.SetRect();

            textBox1.Text = layerGroup.Rect.ToString();

            //显示位图
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
