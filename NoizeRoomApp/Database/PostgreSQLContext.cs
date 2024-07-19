using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Database.Configuration;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Database
{
    public class PostgreSQLContext : DbContext
    {
        private readonly IConfiguration _configuration;
  
        public PostgreSQLContext(IConfiguration configuration) 
        {
            _configuration = configuration;
        } 
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<BookingEntity> Bookings { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<NotifyEntity> Notifies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Database"));
            
        }

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
