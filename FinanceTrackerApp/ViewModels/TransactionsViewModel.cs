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

    private void AddTransactionItem(TransactionItem? item)
    {
        if (item is null) return;
        else
        {
            Transactions.Add(item);
        }
    }
}
