using System;
using System.Collections;
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
        List<LayerGroup> layerGroups = new List<LayerGroup>();

        public MainForm()
		{
			InitializeComponent();
		}

        /// <summary>
        /// find layer postion
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
        /// add layer group to layer groups
        /// </summary>
        public void AddLayers(List<Layer> layers)
        {
            var isStart = false;
            LayerGroup layerGroup = new LayerGroup();


            foreach (var layer in layers)
            {
                if (FindLayerPostion(layer) == LayerGroupPostion.StartLayer)
                {
                    if (!isStart)
                    {
                        isStart = true;
                        layerGroup = new LayerGroup();
                        layerGroup.Layers.Add(layer);
                        continue;
                    }
                    else
                    {
                        layerGroup.Layers.Add(layer);
                        continue;
                    }
                }
                else if (isStart && FindLayerPostion(layer) == LayerGroupPostion.EndLayer)
                {
                    if (isStart)
                    {
                        isStart = false;
                        layerGroups.Add(layerGroup);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (FindLayerPostion(layer) == LayerGroupPostion.MiddleLayer)
                {
                    if (isStart)
                    {
                        layerGroup.Layers.Add(layer);
                        continue;
                    }
                    else
                    {
                        layerGroup = new LayerGroup();
                        layerGroup.Layers.Add(layer);
                        layerGroups.Add(layerGroup);
                        continue;
                    }
                }
            }

            Debug.WriteLine("add layer group finish");
        }

        private void button1_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				Bitmap bmp = null;

				var psdFile = new PsdFile(openFileDialog1.FileName, new LoadContext());

                psdFile.Layers.Reverse();

                AddLayers(psdFile.Layers);

                bmp = layerGroups[0].Layers[1].GetBitmap();

                if (bmp == null)
                    throw new ApplicationException();

                pictureBox1.Image = bmp;
                pictureBox1.Size = bmp.Size;
            }
		}
	}
}
