
namespace PSDFile

{
    //
    // 摘要:
    //     Group layer class
    public class LayerGroup : Layer
    {
        /// <summary>
        /// 摘要:
        /// Gets or sets is folder opened if set to true than group will be in open state
        /// on start up, otherwise in minimized state.
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// 摘要:Gets the layers in layer group
        /// </summary>
        public Layer[] Layers { get; }
        /// <summary>
        /// 摘要:Gets width of the layers group.
        /// </summary>
        public override int Width { get; }
        /// <summary>
        /// 摘要:Gets height of the layers group.
        /// </summary>
        public override int Height { get; }

        /// <summary>
        /// 摘要:Adds the layer to the layer group.
        /// 参数:layer:The layer.
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(Layer layer);
        /// <summary>
        /// 摘要:Adds the layer group.
        /// 参数:groupName:Name of the group.index:The index of the layer to insert after.
        /// 返回结果: Opening group layer
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public LayerGroup AddLayerGroup(string groupName, int index);
    }
}
