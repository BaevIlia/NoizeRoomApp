using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Configuration;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Database
{
    public class PostgreSQLContext : DbContext
    {
        public PostgreSQLContext(DbContextOptions<PostgreSQLContext> options):base(options) 
        {
            Database.EnsureCreated(); 
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<BookingEntity> Bookings { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<NotifyEntity> Notifies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new NotifyConfiguration());

            base.OnModelCreating(modelBuilder);
        }

    }
}
