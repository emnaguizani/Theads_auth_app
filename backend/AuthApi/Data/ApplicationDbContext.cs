using Microsoft.EntityFrameworkCore;
using AuthApi.Models;

namespace AuthApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public ApplicationDbContext() { }                     // design‑time

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=auth_db;Username=postgres;Password=PostGres17.5");
    }                            
        public DbSet<User> Users { get; set; }
    }
}
