using FinanceTracker.Models;

namespace FinanceTracker.Data.Repositories;

public class BudgetRepository
{
    private readonly AppDatabase _db;

    public BudgetRepository(AppDatabase db)
    {
        _db = db;
    }

    public async Task<List<BudgetItem>> GetAllAsync()
    {
        await _db.InitAsync();

        return await _db.Database.Table<BudgetItem>()
            .OrderBy(b => b.Year)
            .ThenBy(b => b.Month)
            .ThenBy(b => b.Category)
            .ToListAsync();
    }

    public async Task<List<BudgetItem>> GetByMonthAsync(int month, int year)
    {
        await _db.InitAsync();

        return await _db.Database.Table<BudgetItem>()
            .Where(b => b.Month == month && b.Year == year)
            .OrderBy(b => b.Category)
            .ToListAsync();
    }

    public async Task SaveAsync(BudgetItem item)
    {
        await _db.InitAsync();

        if (item.Id == 0)
            await _db.Database.InsertAsync(item);
        else
            await _db.Database.UpdateAsync(item);
    }

    public async Task DeleteAsync(BudgetItem item)
    {
        await _db.InitAsync();
        await _db.Database.DeleteAsync(item);
    }
}