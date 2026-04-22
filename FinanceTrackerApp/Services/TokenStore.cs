namespace FinanceTrackerApp.Services;

public sealed class TokenStore : ITokenStore
{
    private const string AccessTokenKey = "auth_access_token";

    public Task<string?> GetAccessTokenAsync()
    {
        var token = Preferences.Get(AccessTokenKey, string.Empty);
        return Task.FromResult(string.IsNullOrWhiteSpace(token) ? null : token);
    }

    public Task SetAccessTokenAsync(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            Preferences.Remove(AccessTokenKey);
        }
        else
        {
            Preferences.Set(AccessTokenKey, accessToken);
        }

        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        Preferences.Remove(AccessTokenKey);
        return Task.CompletedTask;
    }
}
