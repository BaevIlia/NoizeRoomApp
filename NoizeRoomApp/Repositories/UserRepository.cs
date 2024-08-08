using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Dtos;

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

        public async Task<UserDto> Update(Guid id, string name, string email, string phoneNumber, string notifyType)
        {
            var userForUpdate = await _context.Users.FindAsync(id);

            if (userForUpdate is null)
                throw new Exception("Пользователя не существует");

            userForUpdate.Name = name;
            userForUpdate.Email = email;
            userForUpdate.PhoneNumber = phoneNumber;
            userForUpdate.NotifyTypeId = await GetNotifyTypeId(notifyType);

            await _context.SaveChangesAsync();
            var changedUserData = await _context.Users.FindAsync(userForUpdate.Id);
            return new UserDto()
            {
                Id = changedUserData.Id,
                Name = changedUserData.Name,
                Email = changedUserData.Email,
                PhoneNumber = changedUserData.PhoneNumber,
                NotifyType = await GetNotifyType(changedUserData.NotifyTypeId)
            };
        }

        public async Task<int> GetNotifyTypeId(string notifyType) 
        {
            var notifyObject =  await _context.Notifies.Where(n=>n.Name.Equals(notifyType)).FirstOrDefaultAsync();

            if (notifyObject is null)
                throw new Exception("Такого типа уведомлений не существует");
            return notifyObject.Id;

        }

        public async Task<int> GetRoleId(string roleName) 
        {
            var roleObject = await _context.Roles.Where(r=>r.Name.Equals(roleName)).FirstOrDefaultAsync();

            if (roleObject is null)
                throw new Exception("Такой роли не существует");

            return roleObject.Id;
        }

        public async Task<Guid> Authorize(string email, string password) 
        {
            var user = await _context.Users
                .Where(u => u.Email.Equals(email) && u.Password.Equals(password)).FirstOrDefaultAsync();

            if (user is null)
                throw new Exception("Введены неправильные данные или пользователя не существует");

            return user.Id;
        }

        public async Task<bool> ChangePassword(Guid id, string password) 
        {
            var user = await _context.Users.FindAsync(id);

            if (user.Password.Equals(password))
                throw new Exception("Вы пытаетесь сменить текущий пароль на идентичный, пожалуйста, придумайте новый пароль");
            user.Password = password;
            var resultCode = await _context.SaveChangesAsync();
            if (resultCode == 0)
                throw new Exception("Возникла ошибка при смене пароля");

            return true;
        }


        public async Task<string> GetNotifyType(int notifyTypeId)
        {
            var notifyObject = await _context.Notifies.FindAsync(notifyTypeId);

            if (notifyObject is null)
                throw new Exception("Такого типа уведомлений не существует");
            return notifyObject.Name;

        }

        public async Task<string> GetRole(int roleId)
        {
            var roleObject = await _context.Roles.FindAsync(roleId);

            if (roleObject is null)
                throw new Exception("Такой роли не существует");

            return roleObject.Name;
        }

    }
}
