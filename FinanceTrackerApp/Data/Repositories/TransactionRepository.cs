using FinanceTracker.Models;
using FinanceTrackerApp.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FinanceTracker.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly HttpClient _httpClient;

    public TransactionRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TransactionItem>> GetAllAsync()
    {
        var transactions = await _httpClient.GetFromJsonAsync<List<ApiTransaction>>("api/transactions")
            ?? new List<ApiTransaction>();

        return transactions
            .Select(MapToAppModel)
            .OrderByDescending(t => t.Date)
            .ToList();
    }

    public async Task<TransactionItem?> GetByIdAsync(int id)
    {
        try
        {
            var transaction = await _httpClient.GetFromJsonAsync<ApiTransaction>($"api/transactions/{id}");
            return transaction is null ? null : MapToAppModel(transaction);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<int> SaveAsync(TransactionItem item)
    {
        if (item.Id == 0)
        {
            var response = await _httpClient.PostAsJsonAsync("api/transactions", MapToApiModel(item));
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<ApiTransaction>();
            item.Id = created?.Id ?? item.Id;
            return created?.Id ?? 0;
        }

        var updateResponse = await _httpClient.PutAsJsonAsync($"api/transactions/{item.Id}", MapToApiModel(item));
        updateResponse.EnsureSuccessStatusCode();
        return item.Id;
    }

    public async Task<int> DeleteAsync(TransactionItem item)
    {
        var response = await _httpClient.DeleteAsync($"api/transactions/{item.Id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return 0;
        }

        response.EnsureSuccessStatusCode();
        return 1;
    }

    public async Task<List<TransactionItem>> GetForMonthAsync(int year, int month)
    {
        var all = await GetAllAsync();
        return all
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .ToList();
    }

    public TransactionItem MapToAppModel(ApiTransaction transaction) => new()
    {
        Id = transaction.Id,
        Date = transaction.Date,
        Amount = transaction.Amount,
        Type = transaction.Type,
        Category = transaction.Category,
        Note = string.IsNullOrWhiteSpace(transaction.Description) ? null : transaction.Description
    };

    public ApiTransaction MapToApiModel(TransactionItem transaction) => new()
    {
        Id = transaction.Id,
        Date = transaction.Date,
        Amount = transaction.Amount,
        Type = transaction.Type,
        Category = transaction.Category,
        Description = transaction.Note ?? string.Empty
    };

}