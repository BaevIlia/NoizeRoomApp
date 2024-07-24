using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Database;
using NoizeRoomApp.Repositories;
using NoizeRoomApp.Services;


namespace NoizeRoomApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            builder.Services.AddControllers();
            builder.Services.AddDbContext<PostgreSQLContext>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "local";
            });
            var app = builder.Build();

            app.MapControllers();

            app.MapGet("/", () => "Hello world!");
            

            app.Run();
        }
    }
}