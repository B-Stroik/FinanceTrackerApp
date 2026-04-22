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

    private bool _isBusy;
    private string _email = "user@financetracker.local";
    private string _password = "User123$";
    private string _statusMessage = "Not logged in.";

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand LogoutCommand { get; }

    public LoginViewModel(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);
    }

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

    private async Task LogoutAsync()
    {
        await _tokenStore.ClearAsync();
        StatusMessage = "Token cleared.";
    }
}
