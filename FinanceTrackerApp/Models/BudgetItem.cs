using SQLite;

namespace FinanceTracker.Models;

public class BudgetItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }
    public string Category { get; set; } = "";

    public decimal MonthlyLimit { get; set; }
}
