using System.Collections.Generic;
using System.Drawing;

namespace PSDFile

{
    public class LayerGroup
    {
        public LayerGroup()
        {

        }
        public LayerGroup(Layer layer)
        {
            Layers.Add(layer);
            Name = layer.Name;
        }

        const int maxX = 1280;
        const int maxY = 720;
        const int maxWidth = 1280;
        const int maxHeight = 720;

        public List<Layer> Layers = new List<Layer>();

        public Rectangle Rect = new Rectangle(maxX, maxY, 0,0);

        public int Width => Rect.Width;

        public int Height => Rect.Height;

        public string Name { get; set; }

        public void AddLayer(Layer layer)
        {
            Layers.Add(layer);
        }

        /// <summary>
        /// 计算rect范围
        /// </summary>
        public void SetRect()
        {
            var maxRight = 0;
            var maxBottom = 0;

            foreach (Layer layer in Layers)
            {
                if (layer.HasImage)
                {
                    if (layer.Rect.X < Rect.X)
                        Rect.X = layer.Rect.X < 0 ? 0 : layer.Rect.X;
                    if (layer.Rect.Y < Rect.Y)
                        Rect.Y = layer.Rect.Y < 0 ? 0 : layer.Rect.Y;
                    if (layer.Rect.Right > maxRight)
                        maxRight = layer.Rect.Right;
                    if (layer.Rect.Bottom > maxBottom)
                        maxBottom = layer.Rect.Bottom;
                }
            }

            Rect.Width = (Rect.X < 0 ? maxRight + Rect.X : maxRight - Rect.X) > maxWidth ? maxWidth : (Rect.X < 0 ? maxRight + Rect.X : maxRight - Rect.X);
            Rect.Height = (Rect.Y < 0 ? maxBottom + Rect.Y  : maxBottom - Rect.Y) > maxHeight ? maxHeight : (Rect.Y < 0 ? maxBottom + Rect.Y : maxBottom - Rect.Y);
        }
    }

    public enum LayerGroupPostion
    {
        StartLayer,
        MiddleLayer,
        EndLayer,
    }
}
