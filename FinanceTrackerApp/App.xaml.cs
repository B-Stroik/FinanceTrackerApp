using FinanceTracker.Data;
using FinanceTrackerApp.Services;

namespace FinanceTrackerApp;

public partial class App : Application
{
    public App(AppDatabase db, TimeBasedThemeService timeBasedThemeService)
    {
        InitializeComponent();

        Task.Run(async () => await db.InitAsync());
        timeBasedThemeService.InitializeFromCurrentTime();
    }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}