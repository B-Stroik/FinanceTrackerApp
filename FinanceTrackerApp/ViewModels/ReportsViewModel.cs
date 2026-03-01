using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Models;
using System.Collections.ObjectModel;

namespace FinanceTrackerApp.ViewModels;

/// <summary>
/// ViewModel for the Reports page.
/// Loads transaction data and creates summary values:
/// - income
/// - expenses
/// - net
/// - totals grouped by category
/// </summary>
public partial class ReportsViewModel : ObservableObject
{
    private readonly TransactionRepository _repo;

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

    public ReportsViewModel(TransactionRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Loads transactions and builds the report.
    /// This version uses the current month only.
    /// </summary>
    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            CategoryTotals.Clear();

            var allTransactions = await _repo.GetAllAsync();
            var now = DateTime.Now;
            var monthItems = allTransactions
                .Where(t => t.Date.Year == now.Year && t.Date.Month == now.Month)
                .ToList();

            TotalIncome = monthItems
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            TotalExpenses = monthItems
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            NetTotal = TotalIncome - TotalExpenses;

            // Group expenses by category
            var groupedExpenses = monthItems
                .Where(t => t.Amount < 0)
                .GroupBy(t => string.IsNullOrWhiteSpace(t.Category) ? "Uncategorized" : t.Category)
                .Select(g => new CategoryReportItem
                {
                    Category = g.Key,
                    Total = g.Sum(x => Math.Abs(x.Amount))
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            foreach (var item in groupedExpenses)
            {
                CategoryTotals.Add(item);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}