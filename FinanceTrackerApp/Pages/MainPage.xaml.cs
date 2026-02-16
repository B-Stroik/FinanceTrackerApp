using FinanceTrackerApp.Models;
using FinanceTrackerApp.PageModels;

namespace FinanceTrackerApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}