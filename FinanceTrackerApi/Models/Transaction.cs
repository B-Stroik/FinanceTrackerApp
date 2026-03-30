namespace FinanceTrackerApi.Models;

public enum TransactionType
{
    Expense = 0,
    Income = 1
}

public class Transaction
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}