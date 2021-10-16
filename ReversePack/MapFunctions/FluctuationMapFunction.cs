
using ReversePack.PluginCore;

namespace ReversePack.MapGenerators
{
    public class FluctuationMapFunction : IMapFunction
    {
        public string DisplayName => "Fluctuation";

        public string Description =>
            "A high fluctuation means the bit frequently changes from 1 to 0 when moving from one item to the next.";

        public double ApplyTo(int[] bits)
        {
            int changeCount = 0;
            int previous = 0;
            foreach (int bit in bits)
            {
                if (bit != previous)
                {
                    changeCount++;
                }
                previous = bit;
            }
            return (double)changeCount / bits.Length;
        }
    }
}
