using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Abstractions;

namespace NoizeRoomApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PostgreSQLContext _context;

        public UserRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create(UserEntity userForCreate)
        {
            userForCreate.Id = Guid.NewGuid();
            await _context.Users.AddAsync(userForCreate);
            await _context.SaveChangesAsync();

            return userForCreate.Id;
        }

        public async Task<bool> Delete(Guid id)
        {
            var userForDelete = await _context.Users.FindAsync(id);

            if (userForDelete is null)
                throw new Exception("Пользователя не существует");


            _context.Users.Remove(userForDelete);

            return true;
        }

        public async Task<UserEntity> Get(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
                throw new Exception("Пользователя не существует");

            return user;

        }

        public async Task<Guid> Update(Guid id, string name, string email, int notifyTypeId)
        {
            var userForUpdate = await _context.Users.FindAsync(id);

            if (userForUpdate is null)
                throw new Exception("Пользователя не существует");

            userForUpdate.Name = name;
            userForUpdate.Email = email;
            userForUpdate.NotifyTypeId = notifyTypeId;

            await _context.SaveChangesAsync();

            return userForUpdate.Id;
        }
    }
}
