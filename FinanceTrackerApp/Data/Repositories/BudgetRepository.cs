using FinanceTracker.Models;
using SQLite;

namespace FinanceTracker.Data.Repositories;

public class BudgetRepository
{
    private readonly SQLiteAsyncConnection _db;

    public BudgetRepository(AppDatabase database)
    {
        _db = database.Connection;
    }

    public Task<List<BudgetItem>> GetForMonthAsync(int year, int month) =>
        _db.Table<BudgetItem>()
           .Where(b => b.Year == year && b.Month == month)
           .ToListAsync();

    public Task<int> SaveAsync(BudgetItem item) =>
        item.Id == 0 ? _db.InsertAsync(item) : _db.UpdateAsync(item);

    public Task<int> DeleteAsync(BudgetItem item) =>
        _db.DeleteAsync(item);
}