using FinanceTrackerApi.Models;
using FinanceTrackerApi.Repositories;
using FinanceTrackerApi.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository>(sp =>
{
    var options = sp.GetRequiredService<IConfiguration>().GetSection("Storage").Get<StorageOptions>() ?? new StorageOptions();
    return options.UseSqlServer
        ? new SqlTransactionRepository(sp.GetRequiredService<IConfiguration>()) : new JsonTransactionRepository(sp.GetRequiredService<IWebHostEnvironment>());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }))
    .WithName("Health")
    .WithTags("System")
    .Produces(StatusCodes.Status200OK);

app.UseHttpsRedirection();
app.MapControllers();

app.Run();