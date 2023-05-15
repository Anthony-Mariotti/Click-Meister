using ClickMeister.Windows;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Timer = System.Threading.Timer;

namespace ClickMeister.Master;

public class ClickMaster
{
    public EventHandler? Completed;

    private Timer? _timer;
    private AutoResetEvent? _timerEvent;

    public void Start(ClickRequest clickRequest)
    {
        _timerEvent = new AutoResetEvent(false);
        var performClick = new PerformClick(clickRequest);
        performClick.Completed += (object? sender, EventArgs e) => Completed?.Invoke(sender, e);

        _timer = new Timer(performClick.Execute, _timerEvent, new TimeSpan(), clickRequest.Interval);
    }

    public void Stop()
    {
        _timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _timer?.Dispose();
        _timerEvent?.Dispose();
    }
}

public class PerformClick
{
    public EventHandler? Completed;

    private readonly ClickRequest _clickRequest;

    private int _repeatCount = 0;

    public PerformClick(ClickRequest clickRequest)
    {
        _clickRequest = clickRequest;
    }

    public void Execute(object? state)
    {
        var autoEvent = (AutoResetEvent?)state;

        if (_clickRequest.MaxRepeatCount != -1 &&
            _clickRequest.Repeat is RepeatType.UntilAmount)
        {
            ++_repeatCount;
        }

        var cursor = new Win32Point();
        
        if (!Win32.Mouse.GetCursorPos(ref cursor))
        {
            _ = autoEvent?.Set();
            var errorCode = Marshal.GetLastPInvokeError();
            var errorMessage = Marshal.GetLastPInvokeErrorMessage();
            throw new InvalidOperationException($"Failed to retrieve cursor position. ({errorCode}) {errorMessage}");
        }

        if (_clickRequest.MousePosition is MousePositionType.Selected)
        {
            if (_clickRequest.SelectedCursorPosition is null)
            {
                _ = autoEvent?.Set();
                throw new ArgumentNullException(nameof(_clickRequest.SelectedCursorPosition), "No cursor position was available");
            }

            cursor.X = _clickRequest.SelectedCursorPosition.Value.X;
            cursor.Y = _clickRequest.SelectedCursorPosition.Value.Y;

            if (!Win32.Mouse.SetCursorPos(
                _clickRequest.SelectedCursorPosition.Value.X,
                _clickRequest.SelectedCursorPosition.Value.Y))
            {
                _ = autoEvent?.Set();
                throw new Win32Exception($"Failed to set cursor position.");
            }
        }

        switch (_clickRequest.MouseButton)
        {
            case MouseButtonType.Left:

                Win32.Mouse.Invoke(MouseEvent.LEFTDOWN, cursor.X, cursor.Y, 0, 0);
                Win32.Mouse.Invoke(MouseEvent.LEFTUP, cursor.X, cursor.Y, 0, 0);

                if (_clickRequest.MouseClick is MouseClickType.Double)
                {
                    Win32.Mouse.Invoke(MouseEvent.LEFTDOWN, cursor.X, cursor.Y, 0, 0);
                    Win32.Mouse.Invoke(MouseEvent.LEFTUP, cursor.X, cursor.Y, 0, 0);
                }
                break;
            case MouseButtonType.Middle:
                Win32.Mouse.Invoke(MouseEvent.MIDDLEDOWN, cursor.X, cursor.Y, 0, 0);
                Win32.Mouse.Invoke(MouseEvent.MIDDLEUP, cursor.X, cursor.Y, 0, 0);

                if (_clickRequest.MouseClick is MouseClickType.Double)
                {
                    Win32.Mouse.Invoke(MouseEvent.MIDDLEDOWN, cursor.X, cursor.Y, 0, 0);
                    Win32.Mouse.Invoke(MouseEvent.MIDDLEUP, cursor.X, cursor.Y, 0, 0);
                }
                break;
            case MouseButtonType.Right:
                Win32.Mouse.Invoke(MouseEvent.RIGHTDOWN, cursor.X, cursor.Y, 0, 0);
                Win32.Mouse.Invoke(MouseEvent.RIGHTUP, cursor.X, cursor.Y, 0, 0);

                if (_clickRequest.MouseClick is MouseClickType.Double)
                {
                    Win32.Mouse.Invoke(MouseEvent.RIGHTDOWN, cursor.X, cursor.Y, 0, 0);
                    Win32.Mouse.Invoke(MouseEvent.RIGHTUP, cursor.X, cursor.Y, 0, 0);
                }
                break;
            default:
                throw new NotSupportedException("The supplied mouse button is not supported");
        }

        if (_repeatCount == _clickRequest.MaxRepeatCount)
        {
            _repeatCount = 0;
            _ = autoEvent?.Set();
            Completed?.Invoke(null, new EventArgs());
        }
    }
}
