using FinanceTrackerApi.Models;

namespace FinanceTrackerApi.Services;

public interface IBudgetService
{
    Task<IReadOnlyList<Budget>> GetAllAsync();
    Task<Budget?> GetByIdAsync(int id);
    Task<Budget> CreateAsync(Budget budget);
    Task<bool> UpdateAsync(int id, Budget budget);
    Task<bool> DeleteAsync(int id);
}
