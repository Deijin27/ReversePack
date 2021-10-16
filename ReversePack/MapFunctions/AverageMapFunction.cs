using ReversePack.PluginCore;
using System.Linq;

namespace ReversePack.MapGenerators
{
    public class AverageMapFunction : IMapFunction
    {
        public string DisplayName => "Average";

        public string Description => "The average value of the bit.";

        public double ApplyTo(int[] bits)
        {
            return (double)bits.Sum() / bits.Length;
        }
    }
}
