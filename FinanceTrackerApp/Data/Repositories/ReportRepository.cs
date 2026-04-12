using FinanceTrackerApp.Models;
using System.Net.Http.Json;

namespace FinanceTracker.Data.Repositories;

public class ReportRepository
{
    private readonly HttpClient _httpClient;

    public ReportRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiReportSummary> GetMonthlyAsync(int? month = null, int? year = null)
    {
        var query = new List<string>();
        if (month.HasValue)
        {
            query.Add($"month={month.Value}");
        }

        if (year.HasValue)
        {
            query.Add($"year={year.Value}");
        }

        var path = "api/reports/monthly";
        if (query.Count > 0)
        {
            path = $"{path}?{string.Join("&", query)}";
        }

        return await _httpClient.GetFromJsonAsync<ApiReportSummary>(path)
            ?? new ApiReportSummary();
    }
}
