using ClickMeister.Windows;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ClickMeister;
/// <summary>
/// Interaction logic for CursorPositionOverlay.xaml
/// </summary>
public partial class CursorPositionOverlay : Window, IDisposable
{
    public Win32Point SelectedPosition { get; private set; }

    public Point OverlayPosition { get; private set; }

    private bool _disposed = false;
    private readonly Point? _currentPosition;

    public CursorPositionOverlay(Point? currentPosition)
    {
        InitializeComponent();
        _currentPosition = currentPosition;
        Background = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
        AllowsTransparency = true;
        WindowStyle = WindowStyle.None;
        Topmost = true;
        ShowInTaskbar = false;
        ResizeMode = ResizeMode.NoResize;

        var bounds = GetCombinedScreenBounds();
        Left = bounds.Left;
        Top = bounds.Top;
        Width = bounds.Width;
        Height = bounds.Height;
        

        if (_currentPosition is not null)
        {
            Content = new CursorCrosshair(_currentPosition.Value);
        }
    }

    private Rect GetCombinedScreenBounds()
    {
        var minX = SystemParameters.VirtualScreenLeft;
        var minY = SystemParameters.VirtualScreenTop;
        var width = SystemParameters.VirtualScreenWidth;
        var height = SystemParameters.VirtualScreenHeight;

        return new Rect(minX, minY , width, height);
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            DialogResult = false;
        }
    }

    private void Window_MouseUp(object sender, MouseButtonEventArgs e)
    {
        var position = new Win32Point();

        if (!Win32.Mouse.GetCursorPos(ref position))
        {
            DialogResult = false;
            throw new Win32Exception("Failed to retrieve selected cursor position");
        }

        SelectedPosition = position;
        OverlayPosition = e.GetPosition(this);

        DialogResult = true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    public void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
        }
    }
}
