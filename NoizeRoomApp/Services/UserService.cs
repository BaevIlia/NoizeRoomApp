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
        IDistributedCache _cache;

        public UserService(IUserRepository userRepository, IDistributedCache cache)
        {
            _userRepository = userRepository;
            _cache = cache;
        }
        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }


        public async Task<UserDto> GetUserById(Guid id)
        {
            UserEntity? user = null;
            var userString = await _cache.GetStringAsync(id.ToString());
            if (userString!=null)
            {
                user = JsonSerializer.Deserialize<UserEntity>(userString);
            }
            if (user == null) 
            {
                user = await _userRepository.Get(id);
                if (user != null) 
                {
                    userString = JsonSerializer.Serialize(user);
                    await _cache.SetStringAsync(user.Id.ToString(), userString, new DistributedCacheEntryOptions 
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(2)
                    });
                }
            }

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
            int notifyTypeId = await _userRepository.GetNotifyTypeId(notifyType);
            var userForUpdate = await _userRepository
                .Update(id, name, email, phoneNumber, notifyTypeId);

            var responce =  await _userRepository.Get(id);

            return new UserDto
            {
                Id = responce.Id,
                Name = responce.Name,
                Email = responce.Email,
                NotifyType = await _userRepository.GetNotifyType(responce.NotifyTypeId)
            };
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
