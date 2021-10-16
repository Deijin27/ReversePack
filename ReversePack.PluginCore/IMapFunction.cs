
namespace ReversePack.PluginCore
{
    public interface IMapFunction
    {
        /// <summary>
        /// Name to be displayed in list
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Description to be shown as tooltip
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Apply map function to bits
        /// </summary>
        /// <param name="bits">Array of 1s and 0s</param>
        /// <returns>Value between 0 and 1</returns>
        double ApplyTo(int[] bits);
    }
}
