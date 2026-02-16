using FinanceTracker.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTrackerApp
{
    public partial class App : Application
    {
        public App(AppDatabase db)
        {
            InitializeComponent();
            MainPage = new AppShell();

            Task.Run(async () => await db.InitAsync());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}