using FinanceTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerApi.Data;

public class NoAuthFinanceTrackerDbContext : DbContext
{
    public NoAuthFinanceTrackerDbContext(DbContextOptions<NoAuthFinanceTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");
            entity.Property(x => x.Category).HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.ToTable("Budgets");
            entity.Property(x => x.Category).HasMaxLength(100);
            entity.Property(x => x.LimitAmount).HasColumnType("decimal(18,2)");
        });
    }
}
