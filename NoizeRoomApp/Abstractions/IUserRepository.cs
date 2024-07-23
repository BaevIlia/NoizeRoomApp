using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Abstractions
{
    public interface IUserRepository
    {
        public Task<UserEntity> Get(Guid id);
        public Task<Guid> Create(UserEntity userForCreate);
        public Task<bool> Delete(Guid id);
        public Task<Guid> Update(Guid id, string name, string email, string phoneNumber, int notifyTypeId);

        public Task<int> GetNotifyTypeId(string notifyType);

        public Task<int> GetRoleId(string roleName);
        public Task<Guid> Authorize(string email, string password);
        public Task<bool> ChangePassword(Guid id, string password);
        public Task<string> GetNotifyType(int notifyTypeId);
        public Task<string> GetRole(int roleId);
    }
}
