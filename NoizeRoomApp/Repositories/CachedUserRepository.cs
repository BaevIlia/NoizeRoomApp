using Microsoft.Extensions.Caching.Distributed;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;
using System.Text.Json;

namespace NoizeRoomApp.Repositories
{
    public class CachedUserRepository : IUserRepository
    {
        private readonly UserRepository _userRepository;

        private readonly IDistributedCache _distributedCache;

        public CachedUserRepository(UserRepository userRepository, IDistributedCache distributedCache)
        {
            _userRepository = userRepository;
            _distributedCache = distributedCache;
        }

        public Task<Guid> Authorize(string email, string password)
        {
            return _userRepository.Authorize(email, password);
        }

        public Task<bool> ChangePassword(Guid id, string password)
        {
            return _userRepository.ChangePassword(id, password);
        }

        public Task<Guid> Create(UserEntity userForCreate)
        {
            return _userRepository.Create(userForCreate);
        }

        public Task<bool> Delete(Guid id)
        {
            return _userRepository.Delete(id);
        }

        public async Task<UserEntity> Get(Guid id)
        {
            UserEntity user;
            string key = $"user-{id}";

            string cachedUser = await _distributedCache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedUser)) 
            {
                user = await _userRepository.Get(id);

                if (user is null) 
                {
                    return null;
                }
                await _distributedCache.SetStringAsync(key,
                    JsonSerializer.Serialize<UserEntity>(user), new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(1)
                    });

                return user;
            }
            user = JsonSerializer.Deserialize<UserEntity?>(cachedUser);
            return user;
        }

        public Task<string> GetNotifyType(int notifyTypeId)
        {
            return _userRepository.GetNotifyType(notifyTypeId);
        }

        public Task<int> GetNotifyTypeId(string notifyType)
        {
            return _userRepository.GetNotifyTypeId(notifyType);
        }

        public Task<string> GetRole(int roleId)
        {
            return _userRepository.GetRole(roleId);
        }

        public Task<int> GetRoleId(string roleName)
        {
            return _userRepository.GetRoleId(roleName);
        }

        public Task<UserDto> Update(Guid id, string name, string email, string phoneNumber, string notifyType)
        {
            return _userRepository.Update(id, name, email, phoneNumber, notifyType);
        }
    }
}
