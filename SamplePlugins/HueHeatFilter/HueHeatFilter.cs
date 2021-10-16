using ReversePack.PluginCore;
using System;

namespace HueHeatFilter
{
    public class HueHeatFilter : IHeatFilter
    {
        public string DisplayName => "Hue";

        private const double value = 0.6;
        private const double saturation = 0.6;

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            byte v = (byte)value;
            byte p = (byte)(value * (1 - saturation));
            byte q = (byte)(value * (1 - f * saturation));
            byte t = (byte)(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => new Color(v, t, p),
                1 => new Color(q, v, p),
                2 => new Color(p, v, t),
                3 => new Color(p, q, v),
                4 => new Color(t, p, v),
                _ => new Color(v, p, q)
            };
        }

        public Color ApplyTo(double mapValue)
        {
            return ColorFromHSV(mapValue * 255, saturation, value);
        }
    }
}
