using ClickMeister.Windows;
using System.Diagnostics;

namespace ClickMeister.Master;

public class ClickRequest
{
    public MouseButtonType MouseButton { get; private set; } = MouseButtonType.Left;

    public MouseClickType MouseClick { get; private set; } = MouseClickType.Single;

    public MousePositionType MousePosition { get; private set; } = MousePositionType.Current;

    public Win32Point? SelectedCursorPosition { get; private set; } = default;

    public TimeSpan Interval { get; private set; } = TimeSpan.FromMilliseconds(100);

    public RepeatType Repeat { get; private set; } = RepeatType.UntilStopped;

    public int MaxRepeatCount { get; private set; } = 1;

    private ClickRequest(TimeSpan interval, int maxRepeat = 1)
    {
        Interval = interval;
        MaxRepeatCount = maxRepeat;
    }

    public static ClickRequest Create(TimeSpan interval, int maxRepeat = 1)
    {
        return new ClickRequest(interval, maxRepeat);
    }

    public void UpdateMouseButton(MouseButtonType mouseButton)
    {
        Debug.WriteLine($"New mouse button: {mouseButton}");
        MouseButton = mouseButton;
    }

    public void UpdateMouseClick(MouseClickType mouseClick)
    {
        Debug.WriteLine($"New click type: {mouseClick}");
        MouseClick = mouseClick;
    }

    public void UpdateMousePosition(MousePositionType mousePositionType, Win32Point? mousePosition = default)
    {
        Debug.WriteLineIf(mousePosition is null, $"Mouse Position: {mousePositionType}");
        Debug.WriteLineIf(mousePosition is not null, $"Mouse Position: {mousePositionType} | {{ x: {mousePosition!.Value.X}, y: {mousePosition!.Value.Y} }}");
        if (mousePositionType  == MousePositionType.Current)
        {
            MousePosition = mousePositionType;
            SelectedCursorPosition = default;
            
            return;
        }

        MousePosition = mousePositionType;
        
        if (mousePosition is not null)
        {
            SelectedCursorPosition = mousePosition;
        }
    }

    public void UpdateMousePosition(Win32Point position)
    {
        Debug.WriteLine($"New mouse position: {{ x: {position.X}, y: {position.Y} }}");
        SelectedCursorPosition = position;
    }

    public void UpdateInterval(TimeSpan interval)
    {
        Debug.WriteLine($"New interval: {{ hour: {interval.Hours}, minute: {interval.Minutes}, second: {interval.Seconds}, milliseconds: {interval.Milliseconds} }}");
        Interval = interval;
    }

    public void UpdateRepeatType(RepeatType repeatType)
    {
        Debug.WriteLine($"New repeat type: {{ repeat: {repeatType} }}");
        Repeat = repeatType;
    }

    public void UpdateMaxRepeatCount(int maxRepeatCount)
    {
        Debug.WriteLine($"New max repeat count: {{ times: {maxRepeatCount} }}");
        MaxRepeatCount = maxRepeatCount;
    }
}
