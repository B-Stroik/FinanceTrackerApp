using FinanceTracker.Models;
using SQLite;

namespace FinanceTracker.Data.Repositories;

public class TransactionRepository
{
    private readonly SQLiteAsyncConnection _db;

    public TransactionRepository(AppDatabase database)
    {
        _db = database.Connection;
    }

    public Task<List<TransactionItem>> GetAllAsync() =>
        _db.Table<TransactionItem>()
           .OrderByDescending(t => t.Date)
           .ToListAsync();

    public Task<TransactionItem?> GetByIdAsync(int id) =>
        _db.Table<TransactionItem>().Where(t => t.Id == id).FirstOrDefaultAsync();

    public Task<int> SaveAsync(TransactionItem item) =>
        item.Id == 0 ? _db.InsertAsync(item) : _db.UpdateAsync(item);

    public Task<int> DeleteAsync(TransactionItem item) =>
        _db.DeleteAsync(item);

    public Task<List<TransactionItem>> GetForMonthAsync(int year, int month) =>
        _db.Table<TransactionItem>()
           .Where(t => t.Date.Year == year && t.Date.Month == month)
           .ToListAsync();
}