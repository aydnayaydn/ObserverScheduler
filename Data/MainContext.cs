using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ObserverScheduler.Data.Mappings;
using ObserverScheduler.Entities;

namespace ObserverScheduler.Data;

public class MainContext : DbContext
{
    public DbSet<User> Users { get; set; }
    // private IConfiguration Configuration { get; }

    public MainContext(DbContextOptions<MainContext> options) : base(options)
    {
        // Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {   
        // optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));     
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure your entity mappings here
        _ = modelBuilder.Entity<User>(UserMapping.OnModelCreating);
    }
}
