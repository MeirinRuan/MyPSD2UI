using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPSD2UI
{
    public class Wnd
    {
        public Wnd()
        {

        }

        List<Ctrl> ctrls = new List<Ctrl>();

        public void addCtrl(Ctrl ctrl)
        {
            ctrls.Add(ctrl);
        }
    }
}
