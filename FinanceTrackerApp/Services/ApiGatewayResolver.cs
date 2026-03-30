namespace FinanceTrackerApp.Services;

public static class ApiGatewayResolver
{
    private const string ApiBaseUrlEnvironmentVariable = "FINANCE_TRACKER_API_BASE_URL";

    public static string Resolve()
    {
        var configured = Environment.GetEnvironmentVariable(ApiBaseUrlEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return EnsureTrailingSlash(configured);
        }

#if ANDROID
        // Android Emulator -> host machine loopback
        return "http://10.0.2.2:5113/";
#else
        return "http://localhost:5113/";
#endif
    }

    private static string EnsureTrailingSlash(string value)
    {
        return value.EndsWith('/') ? value : $"{value}/";
    }
}