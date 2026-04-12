using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Models;
using System.Collections.ObjectModel;

namespace FinanceTrackerApp.ViewModels;

/// <summary>
/// ViewModel for the Reports page.
/// Loads server-side monthly report data from the API.
/// </summary>
public partial class ReportsViewModel : ObservableObject
{
    private readonly ReportRepository _repo;

    public ObservableCollection<CategoryReportItem> CategoryTotals { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private decimal totalIncome;

    [ObservableProperty]
    private decimal totalExpenses;

    [ObservableProperty]
    private decimal netTotal;

    [ObservableProperty]
    private string reportTitle = "This Month";

    public ReportsViewModel(ReportRepository repo)
    {
        _repo = repo;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            CategoryTotals.Clear();

            var report = await _repo.GetMonthlyAsync();

            TotalIncome = report.TotalIncome;
            TotalExpenses = report.TotalExpenses;
            NetTotal = report.NetTotal;
            ReportTitle = $"{report.Month}/{report.Year}";

            foreach (var item in report.CategoryTotals.OrderByDescending(x => x.Total))
            {
                CategoryTotals.Add(new CategoryReportItem
                {
                    Category = item.Category,
                    Total = item.Total
                });
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
