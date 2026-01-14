using ReversePack.PluginCore;

namespace GrayscaleInverted
{
    public class GrayscaleInvertedHeatFilter : IHeatFilter
    {
        public string DisplayName => "Grayscale Inverted";

        public Color ApplyTo(double mapValue)
        {
            byte part = (byte)(255 - (mapValue * 255));
            return new Color(part, part, part);
        }
    }
}