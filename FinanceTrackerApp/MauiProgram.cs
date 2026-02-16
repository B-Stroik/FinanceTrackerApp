using CommunityToolkit.Maui;
using FinanceTracker.Data;
using FinanceTracker.Data.Repositories;
using FinanceTrackerApp;
using FinanceTrackerApp.ViewModels;
using FinanceTrackerApp.Views;

namespace FinanceTrackerApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit();

        // Database path
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "finance.db3");

        // DI
        builder.Services.AddSingleton(new AppDatabase(dbPath));
        builder.Services.AddSingleton<TransactionRepository>();
        builder.Services.AddSingleton<BudgetRepository>();

        builder.Services.AddSingleton<TransactionsViewModel>();
        builder.Services.AddTransient<TransactionEditViewModel>();
        builder.Services.AddSingleton<BudgetsViewModel>();
        builder.Services.AddSingleton<ReportsViewModel>();

        builder.Services.AddSingleton<TransactionsPage>();
        builder.Services.AddTransient<TransactionEditPage>();
        builder.Services.AddSingleton<BudgetsPage>();
        builder.Services.AddSingleton<ReportsPage>();

        return builder.Build();
    }
}
