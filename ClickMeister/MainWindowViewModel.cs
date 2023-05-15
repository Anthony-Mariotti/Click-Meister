using ClickMeister.Master;

namespace ClickMeister;

public class MainWindowViewModel : LanguageViewModel
{
    private bool _isEnabled = true;
    public bool IsEnabled
    {
        get => _isEnabled;
        private set => _ = SetProperty(ref _isEnabled, value);
    }

    private bool _isRunning;
    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            _ = SetProperty(ref _isRunning, value);
            IsEnabled = !value;
        }
    }

    public ClickRequest ClickRequest { get; set; } = ClickRequest.Create(TimeSpan.FromMilliseconds(100));
}
