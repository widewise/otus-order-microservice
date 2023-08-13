using Microsoft.EntityFrameworkCore;

namespace Otus.Microservice.Order;

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

    public DbSet<Models.Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Models.Order>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
    }
}