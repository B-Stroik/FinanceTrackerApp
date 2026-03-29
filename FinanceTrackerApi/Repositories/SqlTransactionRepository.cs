using FinanceTrackerApi.Models;
using Microsoft.Data.SqlClient;

namespace FinanceTrackerApi.Repositories;

public class SqlTransactionRepository : ITransactionRepository
{
    private readonly string _connectionString;

    public SqlTransactionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Missing SQL Server connection string.");
    }

    public async Task<IReadOnlyList<Transaction>> GetAllAsync()
    {
        const string sql = "SELECT Id, [Date], Amount, Category, Description FROM Transactions ORDER BY [Date] DESC;";
        await EnsureTableAsync();

        var transactions = new List<Transaction>();
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(MapTransaction(reader));
        }

        return transactions;
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        const string sql = "SELECT Id, [Date], Amount, Category, Description FROM Transactions WHERE Id = @Id;";
        await EnsureTableAsync();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? MapTransaction(reader) : null;
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        const string sql = @"
            INSERT INTO Transactions ([Date], Amount, Category, Description)
            OUTPUT INSERTED.Id
            VALUES (@Date, @Amount, @Category, @Description);";
        await EnsureTableAsync();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        AddCommonParameters(command, transaction);

        transaction.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return transaction;
    }

    public async Task<bool> UpdateAsync(int id, Transaction transaction)
    {
        const string sql = @"
            UPDATE Transactions
            SET [Date] = @Date, Amount = @Amount, Category = @Category, Description = @Description
            WHERE Id = @Id;";
        await EnsureTableAsync();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        AddCommonParameters(command, transaction);

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Transactions WHERE Id = @Id;";
        await EnsureTableAsync();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private async Task EnsureTableAsync()
    {
        const string sql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U')
            CREATE TABLE Transactions (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                [Date] DATETIME2 NOT NULL,
                Amount DECIMAL(18,2) NOT NULL,
                Category NVARCHAR(100) NOT NULL,
                Description NVARCHAR(500) NOT NULL
            );";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private static Transaction MapTransaction(SqlDataReader reader)
    {
        return new Transaction
        {
            Id = reader.GetInt32(0),
            Date = reader.GetDateTime(1),
            Amount = reader.GetDecimal(2),
            Category = reader.GetString(3),
            Description = reader.GetString(4)
        };
    }

    private static void AddCommonParameters(SqlCommand command, Transaction transaction)
    {
        command.Parameters.AddWithValue("@Date", transaction.Date);
        command.Parameters.AddWithValue("@Amount", transaction.Amount);
        command.Parameters.AddWithValue("@Category", transaction.Category);
        command.Parameters.AddWithValue("@Description", transaction.Description);
    }
}