using System.Security.Claims;
using System.Text.Json.Serialization;
using FinanceTrackerApi.Auth;
using FinanceTrackerApi.Data;
using FinanceTrackerApi.Models;
using FinanceTrackerApi.Repositories;
using FinanceTrackerApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var authEnabled = builder.Configuration.GetValue<bool>("Auth:Enabled");
var activeConnectionStringName = authEnabled ? "AuthConnection" : "NoAuthConnection";

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<ITransactionRepository>(sp =>
{
    var options = sp.GetRequiredService<IConfiguration>().GetSection("Storage").Get<StorageOptions>() ?? new StorageOptions();
    return options.UseSqlServer
        ? new SqlTransactionRepository(sp.GetRequiredService<IConfiguration>(), activeConnectionStringName)
        : new JsonTransactionRepository(sp.GetRequiredService<IWebHostEnvironment>());
});

builder.Services.AddScoped<IBudgetRepository>(sp =>
{
    var options = sp.GetRequiredService<IConfiguration>().GetSection("Storage").Get<StorageOptions>() ?? new StorageOptions();
    return options.UseSqlServer
        ? new SqlBudgetRepository(sp.GetRequiredService<IConfiguration>(), activeConnectionStringName)
        : new JsonBudgetRepository(sp.GetRequiredService<IWebHostEnvironment>());
});

if (authEnabled)
{
    var authConnectionString = builder.Configuration.GetConnectionString("AuthConnection")
        ?? throw new InvalidOperationException("Missing AuthConnection connection string.");

    builder.Services.AddDbContext<AuthFinanceTrackerDbContext>(options =>
        options.UseSqlServer(authConnectionString));

    builder.Services
        .AddIdentityApiEndpoints<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AuthFinanceTrackerDbContext>();

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
            options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
        })
        .AddBearerToken(IdentityConstants.BearerScheme);
}
else
{
    var noAuthConnectionString = builder.Configuration.GetConnectionString("NoAuthConnection")
        ?? throw new InvalidOperationException("Missing NoAuthConnection connection string.");

    builder.Services.AddDbContext<NoAuthFinanceTrackerDbContext>(options =>
        options.UseSqlServer(noAuthConnectionString));

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = DemoAuthenticationDefaults.Scheme;
            options.DefaultChallengeScheme = DemoAuthenticationDefaults.Scheme;
        })
        .AddScheme<AuthenticationSchemeOptions, DemoAuthenticationHandler>(DemoAuthenticationDefaults.Scheme, _ => { });
}

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    if (authEnabled)
    {
        using var scope = app.Services.CreateScope();
        var authDb = scope.ServiceProvider.GetRequiredService<AuthFinanceTrackerDbContext>();
        await authDb.Database.MigrateAsync();
        await DbInitializer.SeedUsersAsync(scope.ServiceProvider);
    }
    else
    {
        using var scope = app.Services.CreateScope();
        var noAuthDb = scope.ServiceProvider.GetRequiredService<NoAuthFinanceTrackerDbContext>();
        await noAuthDb.Database.MigrateAsync();
    }
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", (ClaimsPrincipal user) =>
    Results.Ok(new
    {
        status = "ok",
        timestamp = DateTime.UtcNow,
        user = user.Identity?.Name ?? "anonymous",
        roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)
    }))
    .AllowAnonymous()
    .WithName("Health")
    .WithTags("System")
    .Produces(StatusCodes.Status200OK);

if (authEnabled)
{
    app.MapIdentityApi<IdentityUser>();
}

app.MapControllers();

app.Run();
