using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FinanceTrackerApi.Auth;

public sealed class DemoAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DemoAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "demo-user-id"),
            new(ClaimTypes.Name, "Demo Admin"),
            new(ClaimTypes.Email, "demo.admin@local.test"),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, DemoAuthenticationDefaults.Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, DemoAuthenticationDefaults.Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
