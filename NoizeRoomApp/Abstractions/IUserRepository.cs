using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;

namespace NoizeRoomApp.Abstractions
{
    public interface IUserRepository
    {
        public Task<UserEntity> Get(Guid id);
        public Task<Guid> Create(UserEntity userForCreate);
        public Task<bool> Delete(Guid id);
        public Task<UserDto> Update(Guid id, string name, string email, string phoneNumber, string notifyType);

        public Task<int> GetNotifyTypeId(string notifyType);

        public Task<int> GetRoleId(string roleName);
        public Task<Guid> Authorize(string email, string password);
        public Task<bool> ChangePassword(Guid id, string password);
        public Task<string> GetNotifyType(int notifyTypeId);
        public Task<string> GetRole(int roleId);
    }
}
