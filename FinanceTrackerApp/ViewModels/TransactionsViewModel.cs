using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Models;
using FinanceTrackerApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace FinanceTrackerApp.ViewModels;

public partial class TransactionsViewModel : ObservableObject
{
    private readonly TransactionRepository _repo;

    public ICommand AddTransaction { get; }

    public ObservableCollection<TransactionItem> Transactions { get; } = new();

    [ObservableProperty] private bool isBusy;

    public TransactionsViewModel(TransactionRepository repo)
    {
        _repo = repo;
        AddTransaction = new Command<TransactionItem>(AddTransactionItem);
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            Transactions.Clear();
            var items = await _repo.GetAllAsync();
            foreach (var i in items) Transactions.Add(i);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteAsync(TransactionItem item)
    {
        if (item is null) return;
        await _repo.DeleteAsync(item);
        Transactions.Remove(item);
    }

    [RelayCommand]
    public async Task ShowTransactionOptionsAsync(TransactionItem item)
    {
        if (item is null) return;

        var page = Shell.Current.CurrentPage;
        if (page is null) return;

        var action = await page.DisplayActionSheetAsync(
            "Transaction Options",
            "Cancel",
            null,
            "View Details",
            "Quick Edit");

        switch (action)
        {
            case "View Details":
                await page.DisplayAlertAsync(
                    item.Category,
                    $"Amount: {item.DisplayAmount}\nType: {item.Type}\nDate: {item.Date:MMM d, yyyy}\nNote: {item.Note ?? "-"}",
                    "Close");
                break;

            case "Quick Edit":
                await Shell.Current.GoToAsync($"{nameof(TransactionEditPage)}?transactionId={item.Id}");
                break;
        }
    }

    private void AddTransactionItem(TransactionItem? item)
    {
        if (item is null) return;
        else
        {
            Transactions.Add(item);
        }
    }
}
