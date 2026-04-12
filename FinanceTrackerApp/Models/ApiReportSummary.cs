namespace FinanceTrackerApp.Models;

public sealed class ApiReportSummary
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetTotal { get; set; }
    public List<ApiCategoryReportTotal> CategoryTotals { get; set; } = [];
}

public sealed class ApiCategoryReportTotal
{
    public string Category { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
