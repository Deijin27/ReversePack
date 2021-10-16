using ReversePack.PluginCore;
using System;
using System.Linq;

namespace ReversePack.MapGenerators
{
    public class ParityMapFunction : IMapFunction
    {
        public string DisplayName => "Parity";

        public string Description => 
            "A high parity means number of times the bit is 0 is equal to the number of times it is 1.";

        public double ApplyTo(int[] bits)
        {
            int zeroCount = bits.Count(i => i == 1);
            return (double)(bits.Length - Math.Abs(bits.Length - (zeroCount * 2))) / bits.Length;
        }
    }
}
