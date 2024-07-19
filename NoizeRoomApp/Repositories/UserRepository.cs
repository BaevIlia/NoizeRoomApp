using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
namespace NoizeRoomApp.Repositories
{
    public class UserRepository
    {
        private readonly PostgreSQLContext _context;

        public UserRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task<Result<UserEntity>> AuthorizeUser(string email, string password)
        {
            var user = await _context.Users
                 .Where(u => u.Email.Equals(email) && u.Password.Equals(password))
                 .FirstOrDefaultAsync();

           
            if (user is null)
                return Result.Failure<UserEntity>("Пользователь не найден");

     
            return user;
        }

      
    }
}
