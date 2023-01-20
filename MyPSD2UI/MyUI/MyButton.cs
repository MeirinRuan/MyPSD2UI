using PSDFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPSD2UI
{
    public class MyButton : MyCtrl
    {
        public MyButton(int id, LayerGroup layerGroup) : base(id, layerGroup)
        {

        }
        public override string CtrlType => "CMyButton";
        public string Click => "Yes";
        public int ClickInterval => 0;

    }
}
