using Microsoft.EntityFrameworkCore;
using AuthApi.Models;

namespace AuthApi.Data
{
    public class DbContext : DbContext
    {
        public DbContext(DbContextOptions<DbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
