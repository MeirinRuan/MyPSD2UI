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

            switch (MatchCtrlType(layerGroup.Name))
            {
                case "Wnd":
                    ctrl = new MyWnd(id, layerGroup);
                    break;
                case "Text":
                    ctrl = new MyText(id, layerGroup);
                    break;
                case "Pic":
                    ctrl = new MyText(id, layerGroup);
                    break;
                case "Button":
                    ctrl = new MyButton(id, layerGroup);
                    break;
                case "Combo":
                    ctrl = new MyCombo(id, layerGroup);
                    break;
                case "Progress":
                    ctrl = new MyProgress(id, layerGroup);
                    break;
                case "Bg"://背景图层放最底层
                    ctrl = new MyText(1, layerGroup);
                    break;
                default://未定义则跳过
                    ctrl = null;
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
