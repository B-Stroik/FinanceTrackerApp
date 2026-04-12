using System.Text.Json;
using FinanceTrackerApi.Models;

namespace FinanceTrackerApi.Repositories;

public class JsonBudgetRepository : IBudgetRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonBudgetRepository(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "Data", "budgets.json");
        var dataDir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(dataDir) && !Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<IReadOnlyList<Budget>> GetAllAsync()
    {
        var records = await ReadAllInternalAsync();
        return records
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .ThenBy(b => b.Category)
            .ToList();
    }

    public async Task<Budget?> GetByIdAsync(int id)
    {
        var records = await ReadAllInternalAsync();
        return records.FirstOrDefault(b => b.Id == id);
    }

    public async Task<Budget> CreateAsync(Budget budget)
    {
        await _lock.WaitAsync();
        try
        {
            var records = await ReadAllInternalUnsafeAsync();
            budget.Id = records.Count == 0 ? 1 : records.Max(b => b.Id) + 1;
            records.Add(budget);
            await SaveUnsafeAsync(records);
            return budget;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> UpdateAsync(int id, Budget budget)
    {
        await _lock.WaitAsync();
        try
        {
            var records = await ReadAllInternalUnsafeAsync();
            var index = records.FindIndex(b => b.Id == id);
            if (index < 0)
            {
                return false;
            }

            budget.Id = id;
            records[index] = budget;
            await SaveUnsafeAsync(records);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            var records = await ReadAllInternalUnsafeAsync();
            var removed = records.RemoveAll(b => b.Id == id) > 0;
            if (!removed)
            {
                return false;
            }

            await SaveUnsafeAsync(records);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<Budget>> ReadAllInternalAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return await ReadAllInternalUnsafeAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<Budget>> ReadAllInternalUnsafeAsync()
    {
        await using var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var data = await JsonSerializer.DeserializeAsync<List<Budget>>(stream);
        return data ?? new List<Budget>();
    }

    private async Task SaveUnsafeAsync(List<Budget> records)
    {
        await using var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, records, new JsonSerializerOptions { WriteIndented = true });
    }
}
