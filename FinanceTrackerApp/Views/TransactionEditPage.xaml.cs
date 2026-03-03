using FinanceTrackerApp.ViewModels;

namespace FinanceTrackerApp.Views;

public partial class TransactionEditPage : ContentPage, IQueryAttributable
{
    private readonly TransactionEditViewModel _vm;
    public TransactionEditPage(TransactionEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("transactionId", out var value)
            && int.TryParse(value?.ToString(), out var transactionId)
            && transactionId > 0)
        {
            await _vm.LoadAsync(transactionId);
        }
    }
}
