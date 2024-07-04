using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Models;
using System.Security.Cryptography;
using System.Text;
using NoizeRoomApp.Contracts.UserContracts;


namespace NoizeRoomApp.Contracts
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        List<User> users = new()
        {
            new User { Id = Guid.Parse("fe442bb5-514a-4c27-9b26-f6219866035b"), Name="Олег", Email="oleg@mail.ru", Password="123", PhoneNumber="88005553535"}
        };

        private readonly PostgreSQLContext _context;



        public UserController(PostgreSQLContext context)
        {
            _context = context;

        }

        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            string password = EncodindPassword(DecodingPassword(request.password));

            UserEntity user = _context.Users
                .Where(u => u.Email.Equals(request.email) && u.Password.Equals(password))
                .FirstOrDefault();
                      
            if (user is null)
            {
                return Unauthorized();
            }
            user.AccessToken = Guid.NewGuid();
            _context.SaveChanges();
            return Ok(new LoginResponse(user.Id.ToString(), user.AccessToken.ToString()));
        }

        [HttpPost("reg")]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest registerRequest)
        {

            var passFromReq = Convert.FromBase64String(registerRequest.password);
            string encodedPassword = Encoding.UTF8.GetString(passFromReq);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < encodedPassword.Length; i += 2)
            {
                builder.Append(encodedPassword[i]);
            };



            UserEntity newUser = new()
            {
                Id = Guid.NewGuid(),
                Name = registerRequest.name,
                Email = registerRequest.email,
                PhoneNumber = registerRequest.phone,
                Password = builder.ToString(),
                AccessToken = Guid.NewGuid(),
                NotifyTypeId = registerRequest.notifyTypeId,
                RoleId = registerRequest.roleId
            };

            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return Ok(new RegistrationResponse(newUser.Id.ToString()));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("getProfile")]
        public async Task<IActionResult> GetUserById([FromBody] GetProfileRequest request)
        {


            UserEntity user = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();
            if (user is null)
            {
                return NoContent();
            }
            string notifyType = _context.Notifies.Find(user.NotifyTypeId).ToString();
            return Ok(new GetProfileResponse(user.Id, user.Name, user.Email, user.PhoneNumber, notifyType, user.RoleId));
        }

        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateProfileRequest request)
        {
            UserEntity userForSave = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();

            if (userForSave is null)
            {
                return NoContent();
            }
            try
            {
                userForSave.Name = request.name;
                userForSave.Email = request.email;
                userForSave.PhoneNumber = request.phoneNumber;
                userForSave.NotifyTypeId = request.notifyTypeId;

                _context.SaveChanges();
                string notifyType = _context.Notifies.Find(userForSave.NotifyTypeId).ToString();

                return Ok(new UpdateProfileResponce(userForSave.Id.ToString(), userForSave.Name, userForSave.Email, userForSave.PhoneNumber, notifyType));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswRequest request)
        {
            UserEntity userForPassChange = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();

            if (userForPassChange is null)
            {
                return NoContent();
            }

            string password =DecodingPassword(request.password);


            userForPassChange.Password =EncodindPassword(password);

            _context.SaveChanges();

            return Ok();
        }


        public string DecodingPassword(string cryptedPassword)
        {
            var encodedBytes = Convert.FromBase64String(cryptedPassword);
            string encodedPassword = Encoding.UTF8.GetString(encodedBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < encodedPassword.Length; i += 2)
            {
                builder.Append(encodedPassword[i]);
            };
            return builder.ToString();

        }

        public string EncodindPassword(string password)
        {

            var crypt = MD5.Create();
            return Convert.ToBase64String(crypt.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }



    }






 






   


}
