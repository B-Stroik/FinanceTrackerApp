using FinanceTracker.Models;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Data.Repositories
{
    public interface ITransactionRepository
    {
        public Task<List<TransactionItem>> GetAllAsync();

        public Task<TransactionItem?> GetByIdAsync(int id);

        public Task<int> SaveAsync(TransactionItem item);

        public Task<int> DeleteAsync(TransactionItem item);

        public Task<List<TransactionItem>> GetForMonthAsync(int year, int month);

        public TransactionItem MapToAppModel(ApiTransaction transaction);

        public ApiTransaction MapToApiModel(TransactionItem transaction);

    }
}
