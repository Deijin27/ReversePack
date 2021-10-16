using ReversePack.PluginCore;

namespace ReversePack.HeatMapFilters
{
    public class LinearHeatFilter : IHeatFilter
    {
        public string DisplayName => "Linear";

        public LinearHeatFilter()
        {
            _start = new Color(30, 30, 70);
            var end = new Color(255, 220, 100);
            _differenceR = end.R - _start.R;
            _differenceG = end.G - _start.G;
            _differenceB = end.B - _start.B;
        }

        private readonly int _differenceR;
        private readonly int _differenceG;
        private readonly int _differenceB;

        private readonly Color _start;

        public Color ApplyTo(double mapValue)
        {
            return new Color(
                (byte)(_start.R + (mapValue * _differenceR)),
                (byte)(_start.G + (mapValue * _differenceG)),
                (byte)(_start.B + (mapValue * _differenceB))
                );
        }
    }
}
