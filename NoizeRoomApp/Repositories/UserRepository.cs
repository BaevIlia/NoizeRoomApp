using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Contracts.UserContracts;
namespace NoizeRoomApp.Repositories
{
    public class UserRepository
    {
        private readonly PostgreSQLContext _context;

        public UserRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> AuthorizeUser(string email, string password)
        {
            var user = await _context.Users
                 .Where(u => u.Email.Equals(email) && u.Password.Equals(password))
                 .FirstOrDefaultAsync();

     
            return user;
        }

        public async Task<Guid> Registration(RegistrationRequest registerRequest, string encodedPassword) 
        {
            var newUser = new UserEntity()
            {
                Id = Guid.NewGuid(),
                Name = registerRequest.name,
                Email = registerRequest.email,
                PhoneNumber = registerRequest.phone,
                Password = encodedPassword,
                NotifyTypeId = registerRequest.notifyTypeId,
                RoleId = registerRequest.roleId
            };

          

            try
            {
                _context.Add(newUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }

            return newUser.Id;
        }

        public async Task<UserEntity> GetById(Guid id) 
        {
            var user =  await _context.Users.FindAsync(id);

            return user;
        }

        public async Task<string> GetNotifyType(int id) 
        {
            var notifyType = await _context.Notifies.FindAsync(id);

        

            return notifyType.Name;
        }

        public async Task<UserEntity> UpdateUserProfile(Guid id, string name, string email, string phoneNumber, string notifyType) 
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
                throw new ArgumentNullException("Пользователя с таким идентификатором не существует");

            
            user.NotifyTypeId = _context.Notifies.Where(n => n.Name.Equals(notifyType)).Select(n=>n.Id).FirstOrDefaultAsync().Result;
            user.Email = email;
            user.Name = name;
            user.PhoneNumber = phoneNumber;

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<int> ChangeUserPassword(Guid id, string password) 
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
                throw new ArgumentNullException("Пользователя не существует");

            user.Password = password;

            

            return await _context.SaveChangesAsync();
        }
      
    }
}
