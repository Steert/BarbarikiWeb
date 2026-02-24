using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class AppContext(DbContextOptions<AppContext> options) : DbContext(options)
{
    public DbSet<Delivery> Deliveries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Delivery>().HasKey(x => x.id);
        base.OnModelCreating(modelBuilder);
    }
}