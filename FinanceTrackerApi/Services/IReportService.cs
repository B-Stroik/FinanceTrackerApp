using FinanceTrackerApi.Models;

namespace FinanceTrackerApi.Services;

public interface IReportService
{
    Task<ReportSummary> GetMonthlySummaryAsync(int? month, int? year);
}
