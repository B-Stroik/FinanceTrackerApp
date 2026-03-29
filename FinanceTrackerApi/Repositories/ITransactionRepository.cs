using FinanceTrackerApi.Models;

namespace FinanceTrackerApi.Repositories;

public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<bool> UpdateAsync(int id, Transaction transaction);
    Task<bool> DeleteAsync(int id);
}