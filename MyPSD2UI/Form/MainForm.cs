using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using PSDFile;
using MyLib;

namespace MyPSD2UI
{
    public partial class MainForm : Form
	{
        public PsdFile psdFile;
        public MyCtrlParent mcp;
        public MySqlOpration MySql;
        public bool isShowUI = false;
        public static string outputPath = Directory.GetCurrentDirectory() + "\\输出目录\\+ui";

        public MainForm()
		{
            DateTime log_time = DateTime.Now;

            InitializeComponent();
            InitSql();

            //log
            MySql.logToDB("ui工具", log_time, DateTime.Now, "初始化");
        }

        public void InitSql()
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
                DateTime log_time = DateTime.Now;

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
                MySql.logToDB("ui工具", log_time, DateTime.Now, "打开psd文件");
            }
		}

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //显示坐标
            //LayerGroup layerGroup = checkedListBox1.SelectedItem as LayerGroup;
            //layerGroup.SetRect();
            //textBox1.Text = layerGroup.Rect.ToString();

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
                    Width = 1500;
                    Height = 800;

                    foreach (var layerGroup in psdFile.LayerGroups)
                    {
                        Graphics g = CreateGraphics();
                        Pen pen = new Pen(Color.Black);
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

        //生成ui配置
        private void button3_Click(object sender, EventArgs e)
        {
            if (IsPsdFile())
            {
                if (IsNumber(textBox1.Text))
                {
                    DateTime log_time = DateTime.Now;

                    mcp = new MyCtrlParent(Convert.ToInt32(textBox1.Text));
                    mcp.SaveIni(psdFile.LayerGroups, outputPath);
                    Process.Start(outputPath);

                    //log
                    MySql.logToDB("ui工具", log_time, DateTime.Now, "生成ui配置");
                }
                else
                    MessageBox.Show("请输入数字id");
            }
        }

        //psd是否存在
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

        //字符串是否为数字
        public bool IsNumber(string str)
        {
            if (Regex.Match(str, @"\D").Success || String.IsNullOrEmpty(str))
                return false;
            else
                return true;
        }
    }
}
