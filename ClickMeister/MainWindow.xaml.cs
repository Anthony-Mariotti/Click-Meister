using ClickMeister.Master;
using ClickMeister.Utility;
using ClickMeister.Windows;
using NHotkey;
using NHotkey.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClickMeister;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ClickMaster _clickMaster;
    private Point? _overlayPosition;

    public MainWindowViewModel ViewModel { get; set; }

    public MainWindow()
    {
        _clickMaster = new ClickMaster();
        _clickMaster.Completed += (object? sender, EventArgs e) => Dispatcher.BeginInvoke(Stop);

        ViewModel = new MainWindowViewModel();
        InitializeComponent();
        DataContext = ViewModel;

        ComboBoxMouseButton.ItemsSource = Enum.GetValues(typeof(MouseButtonType));
        ComboBoxMouseButton.SelectedIndex = 0;

        ComboBoxClickType.ItemsSource = Enum.GetValues(typeof(MouseClickType));
        ComboBoxClickType.SelectedIndex = 0;

        HotkeyManager.Current.AddOrReplace("StartOrStop", Key.OemTilde, ModifierKeys.Control, HotkeyManager_HotkeyPressed);
    }

    private void HotkeyManager_HotkeyPressed(object? sender, HotkeyEventArgs e)
    {
        if (ViewModel.IsRunning)
        {
            Stop();
        }
        else
        {
            Start();
        }
    }

    private void Start()
    {
        _clickMaster.Start(ViewModel.ClickRequest);
        ButtonStart.IsEnabled = false;
        ButtonStop.IsEnabled = true;
        ButtonStop.Focus();
        ViewModel.IsRunning = true;
    }

    private void Stop()
    {
        _clickMaster.Stop();
        ButtonStart.IsEnabled = true;
        ButtonStop.IsEnabled = false;
        ButtonStart.Focus();
        ViewModel.IsRunning = false;
    }

    private void UpdateMousePosition()
    {
        var resultX = int.TryParse(MousePositionX?.Text, out var mouseX);
        var resultY = int.TryParse(MousePositionY?.Text, out var mouseY);

        if (resultX && resultY)
        {
            var point = new Win32Point(mouseX, mouseY);
            ViewModel.ClickRequest.UpdateMousePosition(point);
        }
    }

    private void UpdateInterval()
    {
        var resultHour = int.TryParse(TextBoxHour?.Text, out var hour);
        var resultMinute = int.TryParse(TextBoxMinute?.Text, out var minute);
        var resultSecond = int.TryParse(TextBoxSecond?.Text, out var second);
        var resultMillisecond = int.TryParse(TextBoxMillisecond?.Text, out var milliseconds);

        if (resultHour && resultMinute && resultSecond && resultMillisecond)
        {
            var interval = new TimeSpan(0, hour, minute, second, milliseconds);
            ViewModel.ClickRequest.UpdateInterval(interval);
        }
    }

    private void ComboBoxMouseButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selection = (sender as ComboBox)?.SelectedItem.ToString();

        if (string.IsNullOrWhiteSpace(selection))
        {
            throw new InvalidOperationException("Failed to change selection of mouse button");
        }

        if (!Enum.TryParse<MouseButtonType>(selection, out var buttonType))
        {
            throw new InvalidOperationException("Failed to retrieve mouse button from selection");
        }

        ViewModel.ClickRequest.UpdateMouseButton(buttonType);
    }

    private void ComboBoxClickType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selection = (sender as ComboBox)?.SelectedItem.ToString();

        if (string.IsNullOrWhiteSpace(selection))
        {
            throw new InvalidOperationException("Failed to change selection of clicking type");
        }

        if (!Enum.TryParse<MouseClickType>(selection, out var mouseClickType))
        {
            throw new InvalidOperationException("Failed to retrieve mouse click type from selection");
        }

        ViewModel.ClickRequest.UpdateMouseClick(mouseClickType);
    }

    private void MousePosition_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox source)
        {
            return;
        }
        
        if (int.TryParse(source.Text, out _))
        {
            UpdateMousePosition();
            return;
        }

        source.Text = $"{0}";
    }

    private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var regex = RegexHelper.NumericOnly();
        e.Handled = regex.IsMatch(e.Text);
    }

    private void Numeric_LostFocus(object sender, RoutedEventArgs e)
    {
        var box = e.Source as TextBox;

        if (string.IsNullOrWhiteSpace(box?.Text))
        {
            box!.Text = $"{0}";
        }
    }

    private void ButtonStart_Click(object sender, RoutedEventArgs e) =>
        Start();

    private void ButtonStop_Click(object sender, RoutedEventArgs e) =>
        Stop();

    private void TextBoxHour_TextChanged(object sender, TextChangedEventArgs e) =>
        UpdateInterval();

    private void TextBoxMinute_TextChanged(object sender, TextChangedEventArgs e) => 
        UpdateInterval();

    private void TextBoxSecond_TextChanged(object sender, TextChangedEventArgs e) =>
        UpdateInterval();

    private void TextBoxMillisecond_TextChanged(object sender, TextChangedEventArgs e) =>
        UpdateInterval();

    private void ButtonPickPosition_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
        using var overlay = new CursorPositionOverlay(_overlayPosition);
        var result = overlay.ShowDialog();

        if (result.HasValue && result.Value)
        {
            MousePositionX.Text = overlay.SelectedPosition.X.ToString();
            MousePositionY.Text = overlay.SelectedPosition.Y.ToString();
            _overlayPosition = overlay.OverlayPosition;
        }

        RadioButtonSelectedLocation.IsChecked = true;
        WindowState = WindowState.Normal;
    }

    private void RadioButtonSelectedLocation_Checked(object sender, RoutedEventArgs e) =>
        ViewModel.ClickRequest.UpdateMousePosition(MousePositionType.Selected);

    private void RadioButtonCurrentPosition_Checked(object sender, RoutedEventArgs e) =>
        ViewModel.ClickRequest.UpdateMousePosition(MousePositionType.Current);

    private void NumericUpDownRepeat_Changed(object sender, ValueChangedEventArgs e) =>
        ViewModel.ClickRequest.UpdateMaxRepeatCount(e.NewValue);

    private void RadioButtonRepeatUntilAmount_Checked(object sender, RoutedEventArgs e) =>
        ViewModel.ClickRequest.UpdateRepeatType(RepeatType.UntilAmount);

    private void RadioButtonRepeatUntilStopped_Checked(object sender, RoutedEventArgs e) =>
        ViewModel.ClickRequest.UpdateRepeatType(RepeatType.UntilStopped);

    private void MenuItemExit_Click(object sender, RoutedEventArgs e) =>
        Application.Current.Shutdown();
}
