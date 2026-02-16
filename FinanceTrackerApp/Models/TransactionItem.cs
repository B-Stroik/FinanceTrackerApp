using SQLite;

namespace FinanceTracker.Models;

public enum TransactionType
{
    Expense = 0,
    Income = 1
}

public class TransactionItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }
    public string Category { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Today;

    public string? Note { get; set; }
}
