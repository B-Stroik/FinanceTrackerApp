using FinanceTrackerApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTrackerApp.Views.Controls;

public partial class ThemeToggleView : ContentView
{
    private TimeBasedThemeService? _timeBasedThemeService;

    public ThemeToggleView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_timeBasedThemeService is null)
        {
            _timeBasedThemeService = ResolveThemeService();
        }

        if (_timeBasedThemeService is null)
        {
            return;
        }

        _timeBasedThemeService.ModeChanged += HandleThemeModeChanged;
        ThemeToggleButton.Text = _timeBasedThemeService.GetButtonText();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (_timeBasedThemeService is null)
        {
            return;
        }

        _timeBasedThemeService.ModeChanged -= HandleThemeModeChanged;
    }

    private void ThemeToggleClicked(object? sender, EventArgs e)
    {
        _timeBasedThemeService?.Toggle();
    }

    private void HandleThemeModeChanged(bool isEnabled)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_timeBasedThemeService is not null)
            {
                ThemeToggleButton.Text = _timeBasedThemeService.GetButtonText();
            }
        });
    }

    private static TimeBasedThemeService? ResolveThemeService()
    {
        var services = Application.Current?.Handler?.MauiContext?.Services;
        return services?.GetService<TimeBasedThemeService>();
    }
}