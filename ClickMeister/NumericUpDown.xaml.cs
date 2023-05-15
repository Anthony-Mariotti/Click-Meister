using ClickMeister.Utility;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClickMeister;
/// <summary>
/// Interaction logic for NumericUpDown.xaml
/// </summary>
public partial class NumericUpDown : UserControl, INotifyPropertyChanged
{
    #region MinValue Property
    public int MinValue
    {
        get => (int)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }
    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(0, MinValueChangedCallback, CoerceMinValueCallback));
    private static object CoerceMinValueCallback(DependencyObject d, object value)
    {
        var maxValue = ((NumericUpDown)d).MaxValue;
        return (int)value > maxValue ? maxValue : value;
    }
    private static void MinValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var numericUpDown = (NumericUpDown)d;
        numericUpDown.CoerceValue(MaxValueProperty);
        numericUpDown.CoerceValue(ValueProperty);
    }
    #endregion MinValue Property

    #region MaxValue Property
    public int? MaxValue
    {
        get => (int?)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }
    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register(nameof(MaxValue), typeof(int?), typeof(NumericUpDown), new FrameworkPropertyMetadata(null, MaxValueChangedCallback, CoerceMaxValueCallback));
    private static object CoerceMaxValueCallback(DependencyObject d, object value)
    {
        var minValue = ((NumericUpDown)d).MinValue;
        return (int?)value < minValue ? minValue : value;
    }
    private static void MaxValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var numericUpDown = (NumericUpDown)d;
        numericUpDown.CoerceValue(MinValueProperty);
        numericUpDown.CoerceValue(ValueProperty);
    }
    #endregion MaxValue Property

    #region Value Property
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(
            0, ValueChangedCallback, CoerceValueCallback), ValidateValueCallback);

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var numericUpDown = (NumericUpDown)d;
        var ea = new ValueChangedEventArgs(ValueChangedEvent, d, (int)e.OldValue, (int)e.NewValue);

        numericUpDown.RaiseEvent(ea);
        numericUpDown.NudTextBox.Text = e.NewValue.ToString();
    }
    private static bool ValidateValueCallback(object value)
    {
        var val = (int)value;
        return val is > int.MinValue and < int.MaxValue;
    }

    private static object CoerceValueCallback(DependencyObject d, object value)
    {
        var val = (int)value;
        var minValue = ((NumericUpDown)d).MinValue;
        var maxValue = ((NumericUpDown)d).MaxValue;

        var result = val < minValue 
            ? minValue
            : maxValue.HasValue && val > maxValue.Value
                ? maxValue.Value
                : (int)value;

        return result;
    }
    #endregion Value Property

    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(ValueChanged), RoutingStrategy.Direct,
            typeof(ValueChangedEventHandler), typeof(NumericUpDown));

    public event ValueChangedEventHandler ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    public NumericUpDown()
    {
        InitializeComponent();
        NudTextBox.Text = Value.ToString();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    #region Event Handlers
    private void NudTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Up)
        {
            Value += 1;
        }

        if (e.Key == Key.Down)
        {
            Value -= 1;
        }
    }

    private void NudTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var index = NudTextBox.CaretIndex;

        if (!int.TryParse(NudTextBox.Text, out var result))
        {
            var changes = e.Changes.FirstOrDefault();
            NudTextBox.Text = NudTextBox.Text.Remove(changes.Offset, changes.AddedLength);
            NudTextBox.CaretIndex = index > 0 ? index - changes.AddedLength : 0;
        }
        else if (result < MinValue && result > MinValue)
        {
            Value = result;
        }
        else
        {
            Value = result;
            NudTextBox.Text = Value.ToString();
            //NudTextBox.CaretIndex = index > 0 ? index - 1 : 0;
        }
    }

    private void NudTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var regex = RegexHelper.NumericOnly();
        e.Handled = regex.IsMatch(e.Text);
    }

    private void NudButtonUp_Click(object sender, RoutedEventArgs e) => Value += 1;

    private void NudButtonDown_Click(object sender, RoutedEventArgs e) => Value -= 1;
    #endregion Event Handlers
}

public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
public class ValueChangedEventArgs : RoutedEventArgs
{
    public ValueChangedEventArgs(RoutedEvent routedEvent, object source, int oldValue, int newValue)
        : base(routedEvent, source)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }
}