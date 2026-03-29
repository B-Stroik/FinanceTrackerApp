using FinanceTrackerApi.Models;

namespace FinanceTrackerApi.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<bool> UpdateAsync(int id, Transaction transaction);
    Task<bool> DeleteAsync(int id);
}