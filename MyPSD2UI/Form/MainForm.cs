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
using System.Linq;
using System.Reflection;

namespace MyPSD2UI
{
    public partial class MainForm : Form
	{
        private PsdFile psdFile;
        private MySqlOpration MySql;
        private MyIni myIni;
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

                listView1.Clear();

                if (isShowUI)
                {
                    g.Clear(DefaultBackColor);
                }
                textBox1.Clear();

                //layergroup列表
                for (int i = 0; i < psdFile.LayerGroups.Count; i++)
                {
                    var name = psdFile.LayerGroups[i].Name;
                    listView1.Items.Add(name);
                    //加入右键菜单
                    ListViewComboBox.Items.Add(name);
                    //有标记类型的默认勾选
                    if (name.Contains("_") && !name.Contains("_Null"))
                    {
                        listView1.Items[i].Checked = true;
                    }
                }

                //对比psd和已有的ini配置标注
                CompareConfig();

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

                    MyCtrlParent mcp = new MyCtrlParent(Convert.ToInt32(textBox1.Text));

                    //根据选择的item生成ui
                    List<LayerGroup> layerGroups = new List<LayerGroup>();

                    for (int i = 0; i < listView1.CheckedItems.Count; i++)
                    {
                        var layerGroup = psdFile.LayerGroups.Where(x => x.Name == listView1.CheckedItems[i].Text);
                        if (layerGroup.Count() > 0)
                        {
                            layerGroups.Add(layerGroup.First());
                        }
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

                listView2.Clear();
                iniLayerGroups.Clear();

                myIni = new MyIni(Path.GetFullPath(openFileDialog1.FileName));

                //构建已有ini配置的layergroup list
                foreach (var item in myIni.IniInfo)
                {
                    Dictionary<string, string> section = item.Value;
                    if (section["CtrlType"] != "CMyWnd")
                    {
                        var name = section["Comment"] == null ? "" : section["Comment"];
                        LayerGroup layerGroup = new LayerGroup(new Rectangle(Convert.ToInt16(section["x"]), Convert.ToInt16(section["y"]), Convert.ToInt16(section["Width"]), Convert.ToInt16(section["Height"])), section["Comment"]);
                        iniLayerGroups.Add(layerGroup);
                    }
                }

                for (int i = 0; i < iniLayerGroups.Count; i++)
                {
                    var name = iniLayerGroups[i].Name;
                    listView2.Items.Add(name);
                    //已有配置是全部勾选的
                    listView2.Items[i].Checked = true;
                }

                //对比psd和已有的配置
                CompareConfig();

                //log
                MySql.UseinfoLog("ui工具", log_time, DateTime.Now, "读取ui配置", fileinfo);
            }
        }

        //控件同步
        private void button5_Click(object sender, EventArgs e)
        {
            DateTime log_time = DateTime.Now;

            //考虑到id不一定一致，只根据有相同命名的图层来同步
            if (listView1.Items.Count > 0 && listView2.Items.Count > 0)
            {
                //先复制一个一样的文件
                var id = myIni.IniInfo.Keys.First();
                var newFilePath = outputPath + "\\" + id + ".ini";
                File.Copy(myIni.IniPath, newFilePath, true);

                MyIni myini2 = new MyIni(newFilePath);

                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    LayerGroup layerGroup2 = iniLayerGroups.Where(x => x.Name == listView2.Items[i].Text).First();
                    for (int j = 0; j < listView1.Items.Count; j++)
                    {
                        LayerGroup layerGroup1 = psdFile.LayerGroups.Where(x => x.Name == listView1.Items[j].Text).First();
                        if (layerGroup1.Name.ToLower() == layerGroup2.Name.ToLower() && iniLayerGroups[i].Rect != layerGroup1.Rect)
                        {
                            iniLayerGroups[i].Rect = layerGroup1.Rect;
                            var section = myIni.IniInfo.Keys.ElementAt(i+1);
                            myini2.IniWriteValue(section, "x", layerGroup1.Rect.X.ToString());
                            myini2.IniWriteValue(section, "y", layerGroup1.Rect.Y.ToString());
                            myini2.IniWriteValue(section, "Width", layerGroup1.Rect.Width.ToString());
                            myini2.IniWriteValue(section, "Height", layerGroup1.Rect.Height.ToString());
                            break;
                        }
                    }
                }
                
                Process.Start(outputPath);

                //log
                MySql.UseinfoLog("ui工具", log_time, DateTime.Now, "控件同步");
            }
            else
            {
                MessageBox.Show("请先导入配置。");
            }
        }

        /// <summary>
        /// 对比psd和已有的配置
        /// </summary>
        private void CompareConfig()
        {
            if (listView1.Items.Count > 0 && listView2.Items.Count > 0)
            {
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    LayerGroup layerGroup2 = iniLayerGroups.Where(x => x.Name == listView2.Items[i].Text).First();
                    bool isSame = false;
                    for (int j = 0; j < listView1.Items.Count; j++)
                    {
                        LayerGroup layerGroup1 = psdFile.LayerGroups.Where(x => x.Name == listView1.Items[j].Text).First();
                        if (layerGroup1.Name.ToLower() == layerGroup2.Name.ToLower())
                        {
                            isSame = true;
                            break;
                        }
                    }
                    if (!isSame)
                    {
                        listView2.Items[i].BackColor = Color.MistyRose;
                    }
                }
            }
        }

        /// <summary>
        /// 生成ui配置
        /// </summary>
        private void CreateUI()
        {
            g = CreateGraphics();

            for (int i = 0; i < listView1.CheckedItems.Count; i++)
            {
                LayerGroup layerGroup;
                var IlayerGroup = psdFile.LayerGroups.Where(x => x.Name == listView1.CheckedItems[i].Text);
                if (IlayerGroup.Count() > 0)
                {
                    layerGroup = IlayerGroup.First();
                    var rect = new Rectangle(
                    layerGroup.Rect.X + flowLayoutPanel1.Location.X,
                    layerGroup.Rect.Y + flowLayoutPanel1.Location.Y,
                    layerGroup.Rect.Width,
                    layerGroup.Rect.Height);
                    g.DrawRectangle(pen, rect);
                    g.DrawString(layerGroup.Name, new Font("宋体", 14), new SolidBrush(Color.Black), rect);

                    //显示位图(无法适配所有psd文件)
                    /*foreach (var layer in layerGroup.Layers)
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
            }      
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

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (g != null && isShowUI)
            {
                g.Clear(DefaultBackColor);
                CreateUI();
            }
        }

        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {

        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewItem item = listView.GetItemAt(e.X, e.Y);
            if (item != null && e.Button == MouseButtons.Right)
            {
                ListViewRightMenu.Show(listView, e.X, e.Y);
            }
        }

        private void ListViewComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ListViewComboBox.SelectedItem != null)
                {
                    if (MessageBox.Show("图层<" + ListViewComboBox.SelectedItem.ToString() + ">将被同步至ini配置中的<" + listView2.SelectedItems[0].Text + ">", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        LayerGroup layerGroup1 = psdFile.LayerGroups.Where(x => x.Name == ListViewComboBox.SelectedItem.ToString()).First();
                        int index = iniLayerGroups.FindIndex(x => x.Name == listView2.SelectedItems[0].Text);

                        iniLayerGroups[index].Rect = layerGroup1.Rect;
                        var section = myIni.IniInfo.Keys.ElementAt(index + 1);
                        myIni.IniWriteValue(section, "x", layerGroup1.Rect.X.ToString());
                        myIni.IniWriteValue(section, "y", layerGroup1.Rect.Y.ToString());
                        myIni.IniWriteValue(section, "Width", layerGroup1.Rect.Width.ToString());
                        myIni.IniWriteValue(section, "Height", layerGroup1.Rect.Height.ToString());
                    }
                }
            }
        }
    }
}
