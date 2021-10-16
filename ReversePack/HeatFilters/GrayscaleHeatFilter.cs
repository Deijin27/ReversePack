using ReversePack.PluginCore;

namespace ReversePack.HeatMapFilters
{
    public class GrayscaleHeatFilter : IHeatFilter
    {
        public string DisplayName => "Grayscale";

        public Color ApplyTo(double mapValue)
        {
            byte part = (byte)(mapValue * 255);
            return new Color(part, part, part);
        }
    }
}
