using CommunityToolkit.Maui;
using FinanceTrackerApp;
using FinanceTrackerApp.ViewModels;
using FinanceTrackerApp.Services;
using FinanceTrackerApp.Views;
using System.Net.Http.Headers;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Data;

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
        builder.Services.AddSingleton<HttpClient>(_ =>
        {
            var apiBaseUrl = ApiGatewayResolver.Resolve();

            var client = new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        });
        builder.Services.AddSingleton<TransactionRepository>();
        builder.Services.AddSingleton<ITransactionRepository>(sp => sp.GetRequiredService<TransactionRepository>());
        builder.Services.AddSingleton<BudgetRepository>();
        builder.Services.AddSingleton<TimeBasedThemeService>();

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
