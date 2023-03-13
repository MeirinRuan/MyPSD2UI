using PSDFile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyLib;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace MyPSD2UI
{
    public class MyCtrlParent
    {
        public MyCtrlParent(int id)
        {
            Id = id;
            ctrls.Add(CtrlFactory.Load(new LayerGroup(new Rectangle(LayerGroup.initX,LayerGroup.initY,LayerGroup.maxWidth,LayerGroup.maxHeight), id + "_Wnd"), id));
        }

        private int Id;
        //起始id
        public const int ctrlId = 20;
        //id间隔
        public const int intervalNum = 5;
        public List<MyCtrl> ctrls = new List<MyCtrl>();

        /// <summary>
        /// save ui.ini
        /// </summary>
        public void SaveIni(List<LayerGroup> layerGroups, string path)
        {
            AddCtrl(layerGroups);
            CreateOutPutDirectory(path);
            WriteIni(path);
        }


        private void AddCtrl(List<LayerGroup> layerGroups)
        {
            int index = 0;
            foreach (var layerGroup in layerGroups)
            {
                var ctrl = CtrlFactory.Load(layerGroup, ctrlId + index);
                if (ctrl != null)
                {
                    //ctrl.Show();
                    ctrls.Add(ctrl);
                    index += intervalNum;
                }
                
            }
        }

        /// <summary>
        /// write ui.ini
        /// </summary>
        /// <param name="path"></param>
        private void WriteIni(string path)
        {
            MyIni ini = new MyIni(path + "\\" + Id + ".ini");
            if (ini != null)
            {
                ini.DeleteAllSection();

                foreach (var ctrl in ctrls)
                {
                    foreach (PropertyInfo pi in ctrl.GetType().GetProperties())
                    {
                        if (pi.Name != "Id")
                        {
                            ini.IniWriteValue(ctrl.Id.ToString(), pi.Name, pi.GetValue(ctrl).ToString());
                            Console.WriteLine(pi.Name + "=" + pi.GetValue(ctrl).ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="TargetDirectory"></param>
        private void CreateOutPutDirectory(string TargetDirectory)
        {
            if (!Directory.Exists(TargetDirectory))
                Directory.CreateDirectory(TargetDirectory);

        }
    }
}
