namespace FinanceTrackerApi.Models;

public class Budget
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal LimitAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
