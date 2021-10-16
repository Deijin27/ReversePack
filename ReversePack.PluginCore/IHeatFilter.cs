
namespace ReversePack.PluginCore
{
    public interface IHeatFilter
    {
        /// <summary>
        /// Name to be displayed in list
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Apply heat filter to map value
        /// </summary>
        /// <param name="mapValue">Value from 0 to 1</param>
        /// <returns>Color to be displayed</returns>
        public Color ApplyTo(double mapValue);
    }
}
