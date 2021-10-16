using ReversePack.PluginCore;
using System.Linq;

namespace FixedMapFunction
{
    public class FixedMapFunction : IMapFunction
    {
        public string DisplayName => "Fixed";

        public string Description => "Bits that are always 1 or 0";

        public double ApplyTo(int[] bits)
        {
            if (bits.All(i => i == 0))
            {
                return 0;
            }
            if (bits.All(i => i == 1))
            {
                return 1;
            }
            return 0.5;
        }
    }
}
