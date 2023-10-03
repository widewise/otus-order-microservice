using Microsoft.EntityFrameworkCore;
using Otus.Microservice.Payment.Models;

namespace Otus.Microservice.Payment;

public class AppDbContext: DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(
        IConfiguration configuration,
        DbContextOptions<AppDbContext> options) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Account>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
        builder.Entity<Transaction>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
    }
}