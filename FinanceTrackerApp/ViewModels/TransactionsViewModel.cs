using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Models;
using System.Collections.ObjectModel;

namespace FinanceTracker.ViewModels;

public partial class TransactionsViewModel : ObservableObject
{
    private readonly TransactionRepository _repo;

    public ObservableCollection<TransactionItem> Transactions { get; } = new();

    [ObservableProperty] private bool isBusy;

    public TransactionsViewModel(TransactionRepository repo)
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
}
