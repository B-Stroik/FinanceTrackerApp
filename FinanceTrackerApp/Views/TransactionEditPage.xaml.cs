using FinanceTrackerApp.ViewModels;

namespace FinanceTrackerApp.Views;

public partial class TransactionEditPage : ContentPage
{
    public TransactionEditPage(TransactionEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
