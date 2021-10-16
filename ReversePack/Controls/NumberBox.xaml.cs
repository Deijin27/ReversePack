using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReversePack.Controls
{
    public enum NumberBoxBase
    {
        Dec,
        Hex
    }

    /// <summary>
    /// Interaction logic for NumberBox.xaml
    /// </summary>
    public partial class NumberBox : UserControl
    {
        public NumberBox()
        {
            InitializeComponent();
            NumberTextBox.Text = "0";
        }

        public static DependencyProperty ValueProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, uint>(v => v.Value, default, OnValuePropertyChanged);

        public uint Value
        {
            get => (uint)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValuePropertyChanged(NumberBox target, DependencyPropertyChangedEventArgs<uint> e)
        {
            target.NumberTextBox.Text = target.Base switch
            {
                NumberBoxBase.Hex => e.NewValue.ToString("X"),
                _ => e.NewValue.ToString(),
            };
        }

        public static DependencyProperty MinProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, uint>(v => v.Min, uint.MinValue);

        public uint Min
        {
            get => (uint)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public static DependencyProperty MaxProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, uint>(v => v.Max, uint.MaxValue);

        public uint Max
        {
            get => (uint)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public static DependencyProperty BaseProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, NumberBoxBase>(v => v.Base, NumberBoxBase.Dec);

        public NumberBoxBase Base
        {
            get => (NumberBoxBase)GetValue(BaseProperty);
            set => SetValue(BaseProperty, value);
        }

        public static DependencyProperty IncrementProperty = UserControlUtil.RegisterDependencyProperty<NumberBox, uint>(v => v.Increment, 1u);

        public uint Increment
        {
            get => (uint)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            uint newVal = Value + Increment;
            if (newVal <= Max && newVal > Value)
            {
                Value = newVal;
                RaiseValueChanged();
            }
        }

        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            uint newVal = Value - Increment;
            if (newVal >= Min && newVal < Value)
            {
                Value = newVal;
                RaiseValueChanged();
            }
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;
            var newVal = TryParse(text, out uint i) ? i : Min;
            if (Value != newVal)
            {
                Value = newVal;
                RaiseValueChanged();
            }
        }

        private bool TryParse(string numberString, out uint value)
        {
            return Base switch
            {
                NumberBoxBase.Hex => uint.TryParse(numberString, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out value),
                _ => uint.TryParse(numberString, out value),
            };
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // If it's invlaid, mark as handled so it doesn't proceed, else mark as not handled.
            string newText = ((TextBox)sender).Text + e.Text;
            e.Handled = !(TryParse(newText, out uint result) && result >= Min && result <= Max);
        }

        private void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, new EventArgs());
        }
        public event EventHandler ValueChanged;
    }
}
