using FinanceTrackerApi.Models;
using FinanceTrackerApi.Repositories;

namespace FinanceTrackerApi.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository repository, ILogger<TransactionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<IReadOnlyList<Transaction>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Transaction?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        ValidateTransaction(transaction);
        _logger.LogInformation("Creating transaction for category {Category} and amount {Amount}", transaction.Category, transaction.Amount);
        return await _repository.CreateAsync(transaction);
    }

    public async Task<bool> UpdateAsync(int id, Transaction transaction)
    {
        ValidateTransaction(transaction);
        _logger.LogInformation("Updating transaction with id {TransactionId}", id);
        return await _repository.UpdateAsync(id, transaction);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting transaction with id {TransactionId}", id);
        return await _repository.DeleteAsync(id);
    }

    private static void ValidateTransaction(Transaction transaction)
    {
        if (transaction.Date == default)
        {
            throw new ArgumentException("Date is required.");
        }

        if (string.IsNullOrWhiteSpace(transaction.Category))
        {
            throw new ArgumentException("Category is required.");
        }

        if (string.IsNullOrWhiteSpace(transaction.Description))
        {
            throw new ArgumentException("Description is required.");
        }
    }
}