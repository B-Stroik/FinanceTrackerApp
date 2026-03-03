namespace FinanceTrackerApp.Services;

public class TimeBasedThemeService
{
    private readonly TimeProvider _timeProvider;

    public TimeBasedThemeService() : this(TimeProvider.System)
    {
    }

    public TimeBasedThemeService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public bool IsEnabled { get; private set; } = true;

    public event Action<bool>? ModeChanged;

    public void InitializeFromCurrentTime()
    {
        if (!IsEnabled)
        {
            return;
        }

        ApplyTimeBasedTheme();
    }

    public bool Toggle()
    {
        IsEnabled = !IsEnabled;

        if (IsEnabled)
        {
            ApplyTimeBasedTheme();
        }
        else if (Application.Current is not null)
        {
            Application.Current.UserAppTheme = AppTheme.Unspecified;
        }

        ModeChanged?.Invoke(IsEnabled);
        return IsEnabled;
    }

    public string GetButtonText()
    {
        return IsEnabled ? "🌙 Auto theme: On" : "🌙 Auto theme: Off";
    }

    private void ApplyTimeBasedTheme()
    {
        if (Application.Current is null)
        {
            return;
        }

        var currentHour = _timeProvider.GetLocalNow().Hour;
        var isNightTime = currentHour >= 19 || currentHour < 6; // Go here for debugging, change back to 19, 6 when done
        Application.Current.UserAppTheme = isNightTime ? AppTheme.Dark : AppTheme.Light;
    }
}