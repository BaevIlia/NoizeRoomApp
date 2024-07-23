using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;

namespace NoizeRoomApp.Abstractions
{
    public interface IUserService
    {
        public Task<UserDto> GetUserById(Guid id);
        public Task<Guid> CreateNewUser(string name, string email, string phoneNumber, string password, string notifyType, string role);
        public Task<Guid> UserAuthorization(string login, string password);
        public Task<UserDto> UpdateUser(Guid id, string name, string email, string phoneNumber, string notifyType);
        public Task<bool> ChangePassword(Guid id, string password);
    }
}
