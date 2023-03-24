using PSDFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyPSD2UI
{
    public abstract class MyCtrl
    {
        public MyCtrl(int id, LayerGroup layerGroup)
        {
            Id = id;
            SetPosition(layerGroup);
            Comment = layerGroup.Name;
            
        }
        public int Id { get; set; }
        public abstract string CtrlType { get; }
        public int x { get; set; }
        public int y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Comment { get; set; }
        private void SetPosition(LayerGroup layerGroup)
        {
            x = layerGroup.Rect.X;
            y = layerGroup.Rect.Y;
            Width = layerGroup.Rect.Width;
            Height = layerGroup.Rect.Height;
        }

        public void Show()
        {
            foreach (PropertyInfo pi in GetType().GetProperties())
            {
                Console.WriteLine(pi.Name + ":" + pi.GetValue(this).ToString());
            }
        }
    }

    public static class CtrlFactory
    {
        public static MyCtrl Load(LayerGroup layerGroup, int id)
        {
            MyCtrl ctrl;

            //忽略大小写
            switch (MatchCtrlType(layerGroup.Name).ToLower())
            {
                case "wnd":
                    ctrl = new MyWnd(id, layerGroup);
                    break;
                case "text":
                    ctrl = new MyText(id, layerGroup);
                    break;
                case "pic":
                    ctrl = new MyText(id, layerGroup);
                    break;
                case "button":
                    ctrl = new MyButton(id, layerGroup);
                    break;
                case "combo":
                    ctrl = new MyCombo(id, layerGroup);
                    break;
                case "progress":
                    ctrl = new MyProgress(id, layerGroup);
                    break;
                case "bg"://背景图层放最底层
                    ctrl = new MyText(1, layerGroup);
                    break;
                case "null":
                    ctrl = null;
                    break;
                default:
                    ctrl = new MyText(id, layerGroup);
                    break;
            }
                

            return ctrl;
        }


        /// <summary>
        /// match CtrlType by layerGroup.Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string MatchCtrlType(string name)
        {
            return Regex.Match(name, @"(?<=_).*").Value;
        }
    }
}
