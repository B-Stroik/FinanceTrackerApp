using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTrackerApp.Models;
using FinanceTrackerApp.Services;
using System.Net.Http.Json;

namespace FinanceTrackerApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string email = "user@financetracker.local";
    [ObservableProperty] private string password = "User123$";
    [ObservableProperty] private string statusMessage = "Not logged in.";

    public LoginViewModel(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        IsBusy = true;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/login", new LoginRequest
            {
                Email = Email,
                Password = Password
            });

            if (!response.IsSuccessStatusCode)
            {
                StatusMessage = $"Login failed: {(int)response.StatusCode}";
                return;
            }

            var token = await response.Content.ReadFromJsonAsync<LoginResponse>();
            await _tokenStore.SetAccessTokenAsync(token?.AccessToken);
            StatusMessage = "Token saved. Authenticated API calls are enabled.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Login error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _tokenStore.ClearAsync();
        StatusMessage = "Token cleared.";
    }
}
