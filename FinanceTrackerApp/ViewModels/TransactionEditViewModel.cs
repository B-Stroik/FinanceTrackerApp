using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Models;
using System.Net.Http;

namespace FinanceTrackerApp.ViewModels;

public partial class TransactionEditViewModel : ObservableObject
{
    private readonly TransactionRepository _repo;

    public IReadOnlyList<TransactionType> TransactionTypes { get; } =
    Enum.GetValues<TransactionType>();

    [ObservableProperty] private int id;
    [ObservableProperty] private TransactionType type = TransactionType.Expense;
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private string category = "";
    [ObservableProperty] private DateTime date = DateTime.Today;
    [ObservableProperty] private string? note;

    [ObservableProperty] private string? error;

    public TransactionEditViewModel(TransactionRepository repo)
    {
        _repo = repo;
    }

    public async Task LoadAsync(int transactionId)
    {
        var item = await _repo.GetByIdAsync(transactionId);
        if (item is null) return;

        Id = item.Id;
        Type = item.Type;
        Amount = item.Amount;
        Category = item.Category;
        Date = item.Date;
        Note = item.Note;
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        Error = null;

        if (Amount <= 0) { Error = "Amount must be greater than 0."; return; }
        if (string.IsNullOrWhiteSpace(Category)) { Error = "Category is required."; return; }

        var item = new TransactionItem
        {
            Id = Id,
            Type = Type,
            Amount = Amount,
            Category = Category.Trim(),
            Date = Date,
            Note = string.IsNullOrWhiteSpace(Note) ? null : Note.Trim()
        };

        try
        {
            await _repo.SaveAsync(item);
            await Shell.Current.GoToAsync("..");
        }
        catch (InvalidOperationException ex)
        {
            Error = ex.Message;
        }
        catch (HttpRequestException)
        {
            Error = "Unable to reach the API. Make sure the backend is running and reachable from the app.";
        }
        catch (Exception)
        {
            Error = "Something went wrong while saving the transaction.";
        }
    }
}