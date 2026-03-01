using FinanceTrackerApp.ViewModels;

namespace FinanceTrackerApp.Views;

public partial class BudgetsPage : ContentPage
{
    private readonly BudgetsViewModel _vm;

    public BudgetsPage(BudgetsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}