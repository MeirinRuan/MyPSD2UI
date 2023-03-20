using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using PSDFile;
using MyLib;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MyPSD2UI
{
    public partial class MainForm : Form
	{
        private PsdFile psdFile;
        private MyCtrlParent mcp;
        private MySqlOpration MySql;
        private bool isShowUI = false;
        private static string outputPath = Directory.GetCurrentDirectory() + "\\输出目录\\+ui";
        private Graphics g;
        private Pen pen = new Pen(Color.Black);
        private List<LayerGroup> iniLayerGroups = new List<LayerGroup>();

        public MainForm()
		{
            DateTime log_time = DateTime.Now;

            InitializeComponent();
            InitSql();

            //log
            MySql.UseinfoLog("ui工具", log_time, DateTime.Now, "初始化");
        }

        private void InitSql()
        {
            //工具基础数据库初始化及开启链接
            MySql = new MySqlOpration();
            MySql.Init();
            MySql.OpenConnection();
        }

        //打开psd文件
        private void button1_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
                //Bitmap bmp = null;
                FileInfo fileInfo = new FileInfo(openFileDialog1.FileName);

                if (fileInfo.Extension != ".psd")
                {
                    MessageBox.Show("文件格式不正确。");
                    return;
                }

                DateTime log_time = DateTime.Now;

                string fileinfo = fileInfo.Name + "文件的大小:" + (fileInfo.Length/(1024*1024)) + "MB;";

                psdFile = new PsdFile(openFileDialog1.FileName, new LoadContext());

                //layergroup列表
                checkedListBox1.DataSource = psdFile.LayerGroups;
                checkedListBox1.DisplayMember = "Name";

                //默认全选
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
                textBox1.Clear();

                //log
                MySql.UseinfoLog("ui工具", log_time, DateTime.Now, "打开psd文件", fileinfo);
            }
		}

        //ui预览
        private void button2_Click(object sender, EventArgs e)
        {
            if (IsPsdFile())
            {
                if (!isShowUI)
                {
                    isShowUI = true;

                    Location = new Point(0,0);
                    Width = 1600;
                    Height = 800;

                    CreateUI();
                }
                else
                {
                    g.Clear(DefaultBackColor);
                    CreateUI();
                }
            }
        }

        //生成ui配置
        private void button3_Click(object sender, EventArgs e)
        {
            if (IsPsdFile())
            {
                if (IsNumber(textBox1.Text))
                {
                    DateTime log_time = DateTime.Now;

                    mcp = new MyCtrlParent(Convert.ToInt32(textBox1.Text));

                    //根据选择的item生成ui
                    List<LayerGroup> layerGroups = new List<LayerGroup>();
                    foreach (var item in checkedListBox1.CheckedItems)
                    {
                        layerGroups.Add((LayerGroup)item);
                    }

                    mcp.SaveIni(layerGroups, outputPath);
                    Process.Start(outputPath);

                    //log
                    MySql.UseinfoLog("ui工具", log_time, DateTime.Now, "生成ui配置");
                }
                else
                    MessageBox.Show("请输入数字id");
            }
        }

        //读取ui配置
        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog1.FileName);

                if (fileInfo.Extension != ".ini")
                {
                    MessageBox.Show("文件格式不正确。");
                    return;
                }

                DateTime log_time = DateTime.Now;
                string fileinfo = fileInfo.Name;

                MyIni myIni = new MyIni(Path.GetFullPath(openFileDialog1.FileName));
                var iniInfo = myIni.InitInfo();

                //构建已有ini配置的layergroup list
                foreach (var item in iniInfo)
                {
                    Dictionary<string, string> section = item.Value;
                    if (section["CtrlType"] != "CMyWnd")
                    {
                        LayerGroup layerGroup = new LayerGroup(new Rectangle(Convert.ToInt16(section["x"]), Convert.ToInt16(section["y"]), Convert.ToInt16(section["Width"]), Convert.ToInt16(section["Height"])), section["Comment"]);
                        iniLayerGroups.Add(layerGroup);
                    }
                }

                //layergroup列表
                checkedListBox2.DataSource = iniLayerGroups;
                checkedListBox2.DisplayMember = "Name";

                //默认全选
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    checkedListBox2.SetItemChecked(i, true);
                }

                //对比psd和已有的配置并标注
                if (checkedListBox1.Items.Count > 0 && checkedListBox2.Items.Count > 0)
                {
                    for (int i = 0; i < checkedListBox2.Items.Count; i++)
                    {
                        LayerGroup layerGroup2 = (LayerGroup)checkedListBox2.Items[i];
                        bool isSame = false;
                        for (int j = 0; j < checkedListBox1.Items.Count; j ++)
                        {
                            LayerGroup layerGroup1 = (LayerGroup)checkedListBox1.Items[j];
                            if (layerGroup1.Name == layerGroup2.Name)
                            {
                                isSame = true;
                                break;
                            }
                        }
                        if (!isSame)
                        {
                            layerGroup2.Name += " //add";
                        }
                    }
                }

                //log
                MySql.UseinfoLog("ui工具", log_time, DateTime.Now, "读取ui配置", fileinfo);
            }
        }

        /// <summary>
        /// 生成ui配置
        /// </summary>
        private void CreateUI()
        {
            g = CreateGraphics();

            foreach (var item in checkedListBox1.CheckedItems)
            {
                LayerGroup layerGroup = (LayerGroup)item;
                var rect = new Rectangle(
                    layerGroup.Rect.X + flowLayoutPanel1.Location.X,
                    layerGroup.Rect.Y + flowLayoutPanel1.Location.Y,
                    layerGroup.Rect.Width,
                    layerGroup.Rect.Height);
                g.DrawRectangle(pen, rect);
                g.DrawString(layerGroup.Name, new Font("宋体", 14), new SolidBrush(Color.Black), rect);
            }

            //显示位图(无法适配所有psd文件)
            /*foreach( var layer in layerGroup.Layers)
            {
                if (layer.HasImage)
                    g.DrawImage(layer.GetBitmap(), rect);
            }*/

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
        }

        /// <summary>
        /// psd是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsPsdFile()
        {
            if (psdFile != null)
            {
                return true;
            }
            else
            {
                MessageBox.Show("请先打开一个psd文件。");
                return false;
            }
        }

        /// <summary>
        /// 字符串是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsNumber(string str)
        {
            if (Regex.Match(str, @"\D").Success || String.IsNullOrEmpty(str))
                return false;
            else
                return true;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (g != null && isShowUI)
            {
                g.Clear(DefaultBackColor);
                CreateUI();
            }
        }

        private void checkedListBox2_DrawItem(object sender, DrawItemEventArgs e)
        {

        }
    }
}
