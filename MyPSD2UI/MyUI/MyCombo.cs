using PSDFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPSD2UI
{
    public class MyCombo : MyCtrl
    {
        public MyCombo(int id, LayerGroup layerGroup) : base(id, layerGroup)
        {
            SetComboSize(layerGroup);
        }

        public override string CtrlType => "CMyCtrlCombo";
        public string Click => "Yes";
        private List<int> ComboWidthInt = new List<int>();
        public string ComboWidth { get; set; }
        private List<int> ComboHeightInt = new List<int>();
        public string ComboHeight { get; set; }

        private void SetComboSize(LayerGroup layerGroup)
        {
            foreach (var layer in layerGroup.Layers)
            {
                var param = CtrlFactory.MatchCtrlType(layer.Name);
                if (param != "Combo" && !String.IsNullOrEmpty(param))
                {
                    try
                    {
                        if (param == "W")
                        {
                            ComboWidthInt.Add(layer.Width);
                            
                        }
                        else if (param == "H")
                        {
                            ComboHeightInt.Add(layer.Height);
                        }
                    }
                    catch(FormatException ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
            }

            ComboWidth = String.Join("_", ComboWidthInt);
            ComboHeight = String.Join("_", ComboHeightInt);

            //默认取layergroup的size
            if (String.IsNullOrEmpty(ComboWidth))
                ComboWidth = layerGroup.Width.ToString();
            if (String.IsNullOrEmpty(ComboHeight))
                ComboHeight = layerGroup.Height.ToString();

        }
    }
}
