using FinanceTracker.Models;
using SQLite;

namespace FinanceTracker.Data;

public class AppDatabase
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection? _database;

    public AppDatabase(string dbPath)
    {
        _dbPath = dbPath;
    }

    public SQLiteAsyncConnection Database =>
        _database ?? throw new InvalidOperationException("Database has not been initialized.");

    public async Task InitAsync()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);

        await _database.CreateTableAsync<TransactionItem>();
        await _database.CreateTableAsync<BudgetItem>();
    }
}