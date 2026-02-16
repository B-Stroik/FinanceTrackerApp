using FinanceTrackerApp.Views;
using Syncfusion.Maui.Toolkit.Carousel;

namespace FinanceTrackerApp.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly ViewModels.TransactionsViewModel _vm;

    public TransactionsPage(ViewModels.TransactionsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void AddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TransactionEditPage));
    }
}
