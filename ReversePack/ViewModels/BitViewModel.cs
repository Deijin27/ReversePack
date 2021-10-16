using System.Windows.Input;
using System.Windows.Media;

namespace ReversePack.ViewModels
{
    public class BitViewModel
    {
        public BitViewModel(int displayValue, PluginCore.Color color, double mapValue, int offset, int uintIndex, MainWindowViewModel parent)
        {
            Value = displayValue;
            Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));

            DisplayInfoCommand = new RelayCommand(() =>
            {
                parent.Offset = $"{offset}";
                parent.UIntIndex = $"{uintIndex}";
                parent.Value = $"{mapValue}";
            });
        }
        public int Value { get; set; }
        public SolidColorBrush Color { get; }

        public ICommand DisplayInfoCommand { get; }
    }
}
