using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Abstractions
{
    public interface IUserService
    {
        public Task<UserEntity> GetUserById(Guid id);
        public Task<Guid> CreateNewUser(string name, string email, string phoneNumber, string password, string notifyType, string role);
        public Task<Guid> UserAuthorization(string login, string password);
        public Task<UserEntity> UpdateUser(Guid id, string name, string email, string phoneNumber, string notifyType);
        public Task<bool> ChangePassword(Guid id, string password);
    }
}
