using FinanceTracker.Models;
using FinanceTrackerApp.Models;
using System.Net;
using System.Net.Http.Json;

namespace FinanceTracker.Data.Repositories;

public class BudgetRepository
{
    private readonly HttpClient _httpClient;

    public BudgetRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BudgetItem>> GetAllAsync()
    {
        List<ApiBudget> budgets;
        try
        {
            budgets = await _httpClient.GetFromJsonAsync<List<ApiBudget>>("api/budgets")
                ?? new List<ApiBudget>();
        }
        catch (HttpRequestException)
        {
            return new List<BudgetItem>();
        }

        return budgets
            .Select(MapToAppModel)
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .ThenBy(b => b.Category)
            .ToList();
    }

    public async Task<List<BudgetItem>> GetByMonthAsync(int month, int year)
    {
        var all = await GetAllAsync();

        return all
            .Where(b => b.Month == month && b.Year == year)
            .OrderBy(b => b.Category)
            .ToList();
    }

    public async Task SaveAsync(BudgetItem item)
    {
        if (item.Id == 0)
        {
            var response = await _httpClient.PostAsJsonAsync("api/budgets", MapToApiModel(item));
            await EnsureSuccessWithDetailsAsync(response);
            var created = await response.Content.ReadFromJsonAsync<ApiBudget>();
            item.Id = created?.Id ?? item.Id;
            return;
        }

        var updateResponse = await _httpClient.PutAsJsonAsync($"api/budgets/{item.Id}", MapToApiModel(item));
        await EnsureSuccessWithDetailsAsync(updateResponse);
    }

    public async Task DeleteAsync(BudgetItem item)
    {
        var response = await _httpClient.DeleteAsync($"api/budgets/{item.Id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    private static BudgetItem MapToAppModel(ApiBudget budget) => new()
    {
        Id = budget.Id,
        Category = budget.Category,
        LimitAmount = budget.LimitAmount,
        Month = budget.Month,
        Year = budget.Year
    };

    private static ApiBudget MapToApiModel(BudgetItem budget) => new()
    {
        Id = budget.Id,
        Category = budget.Category,
        LimitAmount = budget.LimitAmount,
        Month = budget.Month,
        Year = budget.Year
    };

    private static async Task EnsureSuccessWithDetailsAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync();
        var message = string.IsNullOrWhiteSpace(details)
            ? $"Request failed with status code {(int)response.StatusCode}."
            : details;

        throw new InvalidOperationException(message);
    }
}
