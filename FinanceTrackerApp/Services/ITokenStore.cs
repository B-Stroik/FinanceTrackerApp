namespace FinanceTrackerApp.Services;

public interface ITokenStore
{
    Task<string?> GetAccessTokenAsync();
    Task SetAccessTokenAsync(string? accessToken);
    Task ClearAsync();
}
