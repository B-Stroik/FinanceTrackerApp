using SQLite;
using FinanceTracker.Models;

namespace FinanceTracker.Data;

public class AppDatabase
{
    private readonly SQLiteAsyncConnection _db;

    public AppDatabase(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitAsync()
    {
        await _db.CreateTableAsync<TransactionItem>();
        await _db.CreateTableAsync<BudgetItem>();
    }

    public SQLiteAsyncConnection Connection => _db;
}