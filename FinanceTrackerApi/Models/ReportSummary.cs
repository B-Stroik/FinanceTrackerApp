namespace FinanceTrackerApi.Models;

public class ReportSummary
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetTotal { get; set; }
    public IReadOnlyList<CategoryReportTotal> CategoryTotals { get; set; } = [];
}

public class CategoryReportTotal
{
    public string Category { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
