using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Models;
using System.Text;

namespace NoizeRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        List<User> users = new()
        {
            new User { Id = Guid.Parse("fe442bb5-514a-4c27-9b26-f6219866035b"), Name="Олег", Email="oleg@mail.ru", Password="123", PhoneNumber="88005553535"}
        };

        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            User? user = users
                .Where(u => u.Email.Equals(request.email) && u.Password.Equals(request.password))
                .FirstOrDefault();

            return Ok(new LoginResponce(user.Id, Guid.NewGuid()));
        }

        [HttpPost("reg")]
        public async Task<IActionResult> Registration([FromBody] RegisterRequest registerRequest)
        {

            var passFromReq = Convert.FromBase64String(registerRequest.password);
            string encodedPassword = Encoding.UTF8.GetString(passFromReq);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < encodedPassword.Length; i += 2)
            {
                builder.Append(encodedPassword[i]);
            };



            User newUser = new()
            {
                Id = Guid.NewGuid(),
                Name = registerRequest.name,
                Email = registerRequest.email,
                PhoneNumber = registerRequest.phone,
                Password = builder.ToString(),
                NotifyType = registerRequest.notifyType,
            };

            try
            {
                users.Append(newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new GetProfileResponce(newUser.Id, newUser.Name, newUser.Email, newUser.PhoneNumber, newUser.NotifyType));
        }

        [HttpGet("getProfile")]
        public async Task<IActionResult> GetUserById([FromBody] GetProfileRequest request)
        {
            User user = users.FirstOrDefault(u => u.Id.ToString() == request.id);
            return Ok(new GetProfileResponce(user.Id, user.Name, user.Email, user.PhoneNumber, "notifEmail"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            return Ok(users.ToList());
        }

        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateProfileRequest request)
        {
            User userForSave = new() { Id = Guid.Parse(request.id), Name = request.name, Email = request.email, PhoneNumber = request.phoneNumber, NotifyType = request.notifyType };

            users.RemoveAt(0);
            users.Insert(0, userForSave);

            return Ok(new UpdateProfileResponce(userForSave.Id.ToString(), userForSave.Name, userForSave.Email, userForSave.PhoneNumber, userForSave.NotifyType));

        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswRequest request)
        {
            var encodedBytes = Convert.FromBase64String(request.encryptedPassw);
            string encodedPassword = Encoding.UTF8.GetString(encodedBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < encodedPassword.Length; i += 2)
            {
                builder.Append(encodedPassword[i]);
            };



            return Ok(builder.ToString());
        }
    }

    public record LoginResponce(Guid userId, Guid accessToken);
    public record LoginRequest(string email, string password);

    public record RegisterRequest(string email, string name, string phone, string password, string notifyType);
    public record RegisterResponce(string error);

    public record GetProfileRequest(string id);
    public record GetProfileResponce(Guid id, string name, string email, string phone, string notifyType);

    public record UpdateProfileRequest(string id, string name, string email, string phoneNumber, string notifyType);
    public record UpdateProfileResponce(string id, string name, string email, string phoneNumber, string notifyType);

    public record ChangePasswRequest(string encryptedPassw, Guid token);
}
