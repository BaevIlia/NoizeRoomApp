using Microsoft.Extensions.Caching.Distributed;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NoizeRoomApp.Services
{
    public partial class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;


        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;

        }
   


        public async Task<UserDto> GetUserById(Guid id)
        {

            UserEntity? user = await _userRepository.Get(id);

            UserDto responceUser = new() 
            {
                Id = id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                NotifyType = await _userRepository.GetNotifyType(user.NotifyTypeId)
            };
            return responceUser;
        }

        public async Task<Guid> CreateNewUser(string name, string email, string phoneNumber, string password, string notifyType, string role)
        {
            string decodedPassword = DecodePassword(password);
            string hashedPassword = HashPassword(decodedPassword);

            var userForCreate = new UserEntity()
            {
                Name = name,
                Email = email,
                Password = hashedPassword,
                PhoneNumber = phoneNumber,
                NotifyTypeId = _userRepository.GetNotifyTypeId(notifyType).Result,
                RoleId = _userRepository.GetRoleId(role).Result
            };

            return await _userRepository.Create(userForCreate);
        }

        public async Task<Guid> UserAuthorization(string login, string password)
        {
            string decodedPassword = DecodePassword(password);
            string hashedPassword = HashPassword(decodedPassword);

            return await _userRepository.Authorize(login, hashedPassword);
        }

        public async Task<UserDto> UpdateUser(Guid id, string name, string email, string phoneNumber, string notifyType)
        {
            var userUpdateData = await _userRepository
                .Update(id, name, email, phoneNumber, notifyType);

            if (userUpdateData is null)
                throw new ArgumentNullException("Пользователя не существует");

            return userUpdateData;
        }

        public async Task<bool> ChangePassword(Guid id, string password) 
        {
            string decodedPassword = DecodePassword(password);
            string hashedPassword = HashPassword(decodedPassword);

           bool resultPassword = await _userRepository.ChangePassword(id, hashedPassword);
            if (!resultPassword)
                throw new Exception();
            return resultPassword;
        }


        private string HashPassword(string password) 
        {
            var crypt = MD5.Create();

            return Convert
                .ToBase64String(crypt.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private string DecodePassword(string password) 
        {
            var encodedBytes = Convert.FromBase64String(password);
            string encodedPassword = Encoding.UTF8.GetString(encodedBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < encodedPassword.Length; i+=2)
            {
                builder.Append(encodedPassword[i]);
            }
            return builder.ToString();
        }

    
    }
}
