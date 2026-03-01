using CommunityToolkit.Mvvm.ComponentModel;

namespace FinanceTracker.Models;

/// <summary>
/// Represents a single category summary row on the Reports page.
/// Example:
/// Category = "Food"
/// Total = 125.50
/// </summary>
public partial class CategoryReportItem : ObservableObject
{
    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private decimal total;
}