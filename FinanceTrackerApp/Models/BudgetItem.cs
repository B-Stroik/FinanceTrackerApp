using SQLite;

namespace FinanceTracker.Models;

/// <summary>
/// Represents a monthly budget for one category.
/// Example:
/// Category = "Food"
/// LimitAmount = 200
/// Month = 3
/// Year = 2026
/// </summary>
public class BudgetItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Budget category, like Food, Gas, Games, etc.
    public string Category { get; set; } = "Uncategorized";

    // The budget limit for that category for the month
    public decimal LimitAmount { get; set; }

    // Store month/year separately so filtering is easy in SQLite
    public int Month { get; set; }

    public int Year { get; set; }

    // These are display/calculated values only
    [Ignore]
    public decimal Spent { get; set; }

    [Ignore]
    public decimal Remaining => LimitAmount - Spent;

    [Ignore]
    public string MonthLabel => $"{Month}/{Year}";
}