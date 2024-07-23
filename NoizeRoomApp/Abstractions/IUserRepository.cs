using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Abstractions
{
    public interface IUserRepository
    {
        public Task<UserEntity> Get(Guid id);
        public Task<Guid> Create(UserEntity userForCreate);
        public Task<bool> Delete(Guid id);
        public Task<Guid> Update(Guid id, string name, string email, int notifyTypeId);
    }
}
