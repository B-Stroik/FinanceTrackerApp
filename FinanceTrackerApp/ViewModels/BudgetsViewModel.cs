using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Models;
using System.Collections.ObjectModel;

namespace FinanceTrackerApp.ViewModels;

/// <summary>
/// ViewModel for the Budgets page.
/// Lets the user create monthly budgets by category
/// and compares each budget to this month's expenses.
/// </summary>
public partial class BudgetsViewModel : ObservableObject
{
    private readonly BudgetRepository _budgetRepo;
    private readonly TransactionRepository _transactionRepo;

    public ObservableCollection<BudgetItem> Budgets { get; } = new();

    private string _category = string.Empty;
    public string Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    private decimal _limitAmount;
    public decimal LimitAmount
    {
        get => _limitAmount;
        set => SetProperty(ref _limitAmount, value);
    }

    private int _selectedMonth = DateTime.Today.Month;
    public int SelectedMonth
    {
        get => _selectedMonth;
        set => SetProperty(ref _selectedMonth, value);
    }

    private int _selectedYear = DateTime.Today.Year;
    public int SelectedYear
    {
        get => _selectedYear;
        set => SetProperty(ref _selectedYear, value);
    }

    private string? _error;
    public string? Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public BudgetsViewModel(
        BudgetRepository budgetRepo,
        TransactionRepository transactionRepo)
    {
        _budgetRepo = budgetRepo;
        _transactionRepo = transactionRepo;
    }

    /// <summary>
    /// Loads budgets for the selected month/year
    /// and calculates how much has been spent in each category.
    /// </summary>
    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        Error = null;

        try
        {
            Budgets.Clear();

            var budgetItems = await _budgetRepo.GetByMonthAsync(SelectedMonth, SelectedYear);

            var transactions = await _transactionRepo.GetAllAsync();

            var monthExpenses = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Where(t => t.Date.Month == SelectedMonth && t.Date.Year == SelectedYear)
                .ToList();

            foreach (var budget in budgetItems)
            {
                budget.Spent = monthExpenses
                    .Where(t => string.Equals(
                        t.Category?.Trim(),
                        budget.Category?.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount);

                Budgets.Add(budget);
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Saves a new budget for the selected month/year.
    /// </summary>
    [RelayCommand]
    public async Task SaveBudgetAsync()
    {
        Error = null;

        if (string.IsNullOrWhiteSpace(Category))
        {
            Error = "Category is required.";
            return;
        }

        if (LimitAmount <= 0)
        {
            Error = "Budget amount must be greater than 0.";
            return;
        }

        var item = new BudgetItem
        {
            Category = Category.Trim(),
            LimitAmount = LimitAmount,
            Month = SelectedMonth,
            Year = SelectedYear
        };

        await _budgetRepo.SaveAsync(item);

        // Clear the input fields after saving
        Category = string.Empty;
        LimitAmount = 0;

        await LoadAsync();
    }

    /// <summary>
    /// Deletes an existing budget.
    /// </summary>
    [RelayCommand]
    public async Task DeleteAsync(BudgetItem item)
    {
        if (item is null) return;

        await _budgetRepo.DeleteAsync(item);
        Budgets.Remove(item);
    }
}