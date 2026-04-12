using FinanceTrackerApi.Models;
using FinanceTrackerApi.Repositories;

namespace FinanceTrackerApi.Services;

public class ReportService : IReportService
{
    private readonly ITransactionRepository _transactionRepository;

    public ReportService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<ReportSummary> GetMonthlySummaryAsync(int? month, int? year)
    {
        var now = DateTime.UtcNow;
        var targetMonth = month ?? now.Month;
        var targetYear = year ?? now.Year;

        if (targetMonth is < 1 or > 12)
        {
            throw new ArgumentException("Month must be between 1 and 12.");
        }

        if (targetYear < 2000 || targetYear > 3000)
        {
            throw new ArgumentException("Year is out of range.");
        }

        var transactions = await _transactionRepository.GetAllAsync();
        var monthItems = transactions
            .Where(t => t.Date.Month == targetMonth && t.Date.Year == targetYear)
            .ToList();

        var totalIncome = monthItems
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpenses = monthItems
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        var categoryTotals = monthItems
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Category) ? "Uncategorized" : t.Category.Trim())
            .Select(group => new CategoryReportTotal
            {
                Category = group.Key,
                Total = group.Sum(item => item.Amount)
            })
            .OrderByDescending(item => item.Total)
            .ToList();

        return new ReportSummary
        {
            Month = targetMonth,
            Year = targetYear,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetTotal = totalIncome - totalExpenses,
            CategoryTotals = categoryTotals
        };
    }
}
