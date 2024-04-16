using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security;


namespace JWT_Core_Project.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("ApplicationDbContext");
            }
        }

        // Other DbSet properties and configurations will be added here
        public DbSet<Users> users { get; set; } = null!;
        //public DbSet<Role> role { get; set; } = null!;
        //public DbSet<Permission> permissions { get; set; } = null!;
        //public DbSet<RolePermission> role_permission { get; set; } = null!;
        //public DbSet<CallBack> callback { get; set; } = null!;
        //public DbSet<CallBack> UserAll { get; set; } = null!;
        //public DbSet<CallBackHistory> callback_history { get; set; } = null!;

    }
}
