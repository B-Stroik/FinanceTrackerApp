using FinanceTrackerApi.Models;
using FinanceTrackerApi.Repositories;

namespace FinanceTrackerApi.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _repository;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(IBudgetRepository repository, ILogger<BudgetService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<IReadOnlyList<Budget>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Budget?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<Budget> CreateAsync(Budget budget)
    {
        ValidateBudget(budget);
        _logger.LogInformation("Creating budget for category {Category}, month {Month}, year {Year}", budget.Category, budget.Month, budget.Year);
        return await _repository.CreateAsync(budget);
    }

    public async Task<bool> UpdateAsync(int id, Budget budget)
    {
        ValidateBudget(budget);
        _logger.LogInformation("Updating budget with id {BudgetId}", id);
        return await _repository.UpdateAsync(id, budget);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting budget with id {BudgetId}", id);
        return await _repository.DeleteAsync(id);
    }

    private static void ValidateBudget(Budget budget)
    {
        if (string.IsNullOrWhiteSpace(budget.Category))
        {
            throw new ArgumentException("Category is required.");
        }

        if (budget.LimitAmount <= 0)
        {
            throw new ArgumentException("LimitAmount must be greater than zero.");
        }

        if (budget.Month < 1 || budget.Month > 12)
        {
            throw new ArgumentException("Month must be between 1 and 12.");
        }

        if (budget.Year < 2000 || budget.Year > 3000)
        {
            throw new ArgumentException("Year is out of range.");
        }
    }
}
