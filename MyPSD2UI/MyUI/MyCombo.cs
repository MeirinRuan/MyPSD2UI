using PSDFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPSD2UI
{
    public class MyCombo : MyCtrl
    {
        public MyCombo(int id, LayerGroup layerGroup) : base(id, layerGroup)
        {

        }
        public override string CtrlType => "CMyCtrlCombo";
        public string Click => "Yes";
        public string ComboWidth => "50_50";
        public string ComboHeight => "50";
    }
}
