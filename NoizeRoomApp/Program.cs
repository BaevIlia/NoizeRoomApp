using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Database;

namespace NoizeRoomApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            builder.Services.AddControllers();
            builder.Services.AddDbContext<PostgreSQLContext>(
                options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString(nameof(PostgreSQLContext)));
                });
            var app = builder.Build();

            app.MapControllers();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}