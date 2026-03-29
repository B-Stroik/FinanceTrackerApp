using System.Text.Json;
using FinanceTrackerApi.Models;

namespace FinanceTrackerApi.Repositories;

public class JsonTransactionRepository : ITransactionRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonTransactionRepository(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "Data", "transactions.json");
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

    public async Task<IReadOnlyList<Transaction>> GetAllAsync()
    {
        var records = await ReadAllInternalAsync();
        return records.OrderByDescending(t => t.Date).ToList();
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        var records = await ReadAllInternalAsync();
        return records.FirstOrDefault(t => t.Id == id);
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        await _lock.WaitAsync();
        try
        {
            var records = await ReadAllInternalUnsafeAsync();
            transaction.Id = records.Count == 0 ? 1 : records.Max(t => t.Id) + 1;
            records.Add(transaction);
            await SaveUnsafeAsync(records);
            return transaction;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> UpdateAsync(int id, Transaction transaction)
    {
        await _lock.WaitAsync();
        try
        {
            var records = await ReadAllInternalUnsafeAsync();
            var index = records.FindIndex(t => t.Id == id);
            if (index < 0)
            {
                return false;
            }

            transaction.Id = id;
            records[index] = transaction;
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
            var removed = records.RemoveAll(t => t.Id == id) > 0;
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

    private async Task<List<Transaction>> ReadAllInternalAsync()
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

    private async Task<List<Transaction>> ReadAllInternalUnsafeAsync()
    {
        await using var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var data = await JsonSerializer.DeserializeAsync<List<Transaction>>(stream);
        return data ?? new List<Transaction>();
    }

    private async Task SaveUnsafeAsync(List<Transaction> records)
    {
        await using var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, records, new JsonSerializerOptions { WriteIndented = true });
    }
}