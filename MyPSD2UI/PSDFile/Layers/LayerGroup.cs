using System;
using System.Collections;
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

        public List<Layer> Layers = new List<Layer>();

        public int Width { get; set; }

        public int Height { get; set; }

    }

    public enum LayerGroupPostion
    {
        StartLayer,
        MiddleLayer,
        EndLayer,
    }
}
