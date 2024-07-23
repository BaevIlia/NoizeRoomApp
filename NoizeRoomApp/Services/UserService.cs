using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Database.Models;
using System.Security.Cryptography;
using System.Text;

namespace NoizeRoomApp.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<UserEntity> GetUserById(Guid id)
        {
            var user = await _userRepository.Get(id);

            return user;
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

        public async Task<UserEntity> UpdateUser(Guid id, string name, string email, string phoneNumber, string notifyType)
        {
            int notifyTypeId = await _userRepository.GetNotifyTypeId(notifyType);
            var userForUpdate = await _userRepository
                .Update(id, name, email, phoneNumber, notifyTypeId);

            return await _userRepository.Get(id);
        }

        public async Task<bool> ChangePassword(Guid id, string password) 
        {
            string decodedPassword = DecodePassword(password);
            string hashedPassword = HashPassword(decodedPassword);

            return await _userRepository.ChangePassword(id, hashedPassword);
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
