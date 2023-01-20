using PSDFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPSD2UI
{
    public class MyWnd : MyCtrl
    {
        public MyWnd(int id, LayerGroup layerGroup) : base(id, layerGroup)
        {

        }
        public override string CtrlType => "CMyWnd";
        public string Click => "Yes";
        public string ShowClose => "No";
    }
}
