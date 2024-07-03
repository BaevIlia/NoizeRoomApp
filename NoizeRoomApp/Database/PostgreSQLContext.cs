using Microsoft.EntityFrameworkCore;

namespace NoizeRoomApp.Database
{
    public class PostgreSQLContext : DbContext
    {
        public PostgreSQLContext(DbContextOptionsBuilder<PostgreSQLContext> options): base(options) 
        {

        }
        public DbSet<>
    }
}
