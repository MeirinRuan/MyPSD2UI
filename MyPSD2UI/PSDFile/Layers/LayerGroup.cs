using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace PSDFile

{
    public class LayerGroup
    {
        public LayerGroup()
        {

        }

        public List<Layer> Layers { get; }

        public int Width { get; }

        public int Height { get; }

        public void AddLayer(Layer layer)
        {
            
        }

        public Layer GetLayerSection(Layer layer)
        {

        }
    }
}
