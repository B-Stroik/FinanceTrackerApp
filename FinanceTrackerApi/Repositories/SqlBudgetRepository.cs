using FinanceTrackerApi.Models;
using Microsoft.Data.SqlClient;

namespace FinanceTrackerApi.Repositories;

public class SqlBudgetRepository : IBudgetRepository
{
    private readonly string _connectionString;

    public SqlBudgetRepository(IConfiguration configuration, string connectionStringName = "NoAuthConnection")
    {
        _connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Missing SQL Server connection string: {connectionStringName}.");
    }

    public async Task<IReadOnlyList<Budget>> GetAllAsync()
    {
        const string sql = "SELECT Id, Category, LimitAmount, [Month], [Year] FROM Budgets ORDER BY [Year] DESC, [Month] DESC, Category ASC;";
        await EnsureTableAsync();

        var budgets = new List<Budget>();
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            budgets.Add(new Budget
            {
                Id = reader.GetInt32(0),
                Category = reader.GetString(1),
                LimitAmount = reader.GetDecimal(2),
                Month = reader.GetInt32(3),
                Year = reader.GetInt32(4)
            });
        }

        return budgets;
    }

    public async Task<Budget?> GetByIdAsync(int id)
    {
        const string sql = "SELECT Id, Category, LimitAmount, [Month], [Year] FROM Budgets WHERE Id = @Id;";
        await EnsureTableAsync();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync()
            ? new Budget
            {
                Id = reader.GetInt32(0),
                Category = reader.GetString(1),
                LimitAmount = reader.GetDecimal(2),
                Month = reader.GetInt32(3),
                Year = reader.GetInt32(4)
            }
            : null;
    }

    public async Task<Budget> CreateAsync(Budget budget)
    {
        const string sql = @"
            INSERT INTO Budgets (Category, LimitAmount, [Month], [Year])
            OUTPUT INSERTED.Id
            VALUES (@Category, @LimitAmount, @Month, @Year);";
        await EnsureTableAsync();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        AddCommonParameters(command, budget);

        budget.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return budget;
    }

    public async Task<bool> UpdateAsync(int id, Budget budget)
    {
        const string sql = @"
            UPDATE Budgets
            SET Category = @Category, LimitAmount = @LimitAmount, [Month] = @Month, [Year] = @Year
            WHERE Id = @Id;";
        await EnsureTableAsync();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        AddCommonParameters(command, budget);

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Budgets WHERE Id = @Id;";
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
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Budgets' AND xtype='U')
            CREATE TABLE Budgets (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Category NVARCHAR(100) NOT NULL,
                LimitAmount DECIMAL(18,2) NOT NULL,
                [Month] INT NOT NULL,
                [Year] INT NOT NULL
            );";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var createCommand = new SqlCommand(sql, connection);
        await createCommand.ExecuteNonQueryAsync();
    }

    private static void AddCommonParameters(SqlCommand command, Budget budget)
    {
        command.Parameters.AddWithValue("@Category", budget.Category);
        command.Parameters.AddWithValue("@LimitAmount", budget.LimitAmount);
        command.Parameters.AddWithValue("@Month", budget.Month);
        command.Parameters.AddWithValue("@Year", budget.Year);
    }
}
