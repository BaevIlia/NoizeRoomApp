using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Database;


namespace NoizeRoomApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddControllers();
            builder.Services.AddDbContext<PostgreSQLContext>(
                options =>
                {
                    options.UseNpgsql(connectionString);
                });
            var app = builder.Build();

            app.MapControllers();

            app.MapGet("/", () => "Hello world!");
            

            app.Run();
        }
    }
}