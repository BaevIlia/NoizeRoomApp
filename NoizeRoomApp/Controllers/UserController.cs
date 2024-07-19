using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using System.Security.Cryptography;
using System.Text;
using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Repositories;
using CSharpFunctionalExtensions;


namespace NoizeRoomApp.Contracts
{
    /// <summary>
    /// Вся логика работы приложения, связаная с пользователем
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly PostgreSQLContext _context;

        private readonly UserRepository _userRepository;

        public UserController(PostgreSQLContext context, UserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Метод авторизации пользоваеля, на вход требует строковые почту и пароль
        /// </summary>
        /// <param name="request">Возвращает идентификатор пользователя и токен доступа</param>
        /// <returns></returns>
        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
       
            string password = EncodindPassword(DecodingPassword(request.password));

            var userResult = await _userRepository.AuthorizeUser(request.email, password);

            userResult.Value.AccessToken = Guid.NewGuid();
            
            return Ok(new LoginResponse(userResult.Value.Id.ToString(), userResult.Value.AccessToken.ToString()));
        }

        /// <summary>
        /// Метод регистрации, на вход требует идентификатор, имя, почту, телефон, пароль, идентификатор типа уведомления и идентификатор роли
        /// </summary>
        /// <param name="registerRequest">Возвращает иденти</param>
        /// <returns></returns>
        [HttpPost("reg")]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest registerRequest)
        {


            string password = DecodingPassword(registerRequest.password);

            UserEntity newUser = new()
            {
                Id = Guid.NewGuid(),
                Name = registerRequest.name,
                Email = registerRequest.email,
                PhoneNumber = registerRequest.phone,
                Password = EncodindPassword(password),
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
        /// <summary>
        /// Запрос о получении пользователя, на вход принимает идентификатор пользователя
        /// </summary>
        /// <param name="request">Возвращает идентификатор, почту, пароль, номер телефона, тип уведомления и идентификатор роли</param>
        /// <returns></returns>
        [HttpGet("getProfile")]
        public async Task<IActionResult> GetUserById([FromBody] GetProfileRequest request)
        {


            UserEntity user = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();


            if (user is null)
            {
                return NoContent();
            }

            string notifyType = _context.Notifies.Find(user.NotifyTypeId).Name;
            return Ok(new GetProfileResponse(user.Id, user.Name, user.Email, user.PhoneNumber, notifyType, user.RoleId));
        }


        /// <summary>
        /// Запрос на обновление профиля, на вход принимает имя, почту, номер телефона и тип уведомления
        /// </summary>
        /// <param name="request">Возвращает обновлённого пользователя</param>
        /// <returns></returns>
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

        /// <summary>
        /// Запрос на смену пароля, на вход пароль и идентификатор пользователя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Метод дешифровки пароля, полученного в запросе. Пароль шифруется путём добавления после каждого символа пароля случайного символа(123 - 1a2b3c)
        /// </summary>
        /// <param name="cryptedPassword">Возвращает пароль в чистом виде</param>
        /// <returns></returns>
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
        /// <summary>
        /// Метод хэширования пароля хэшем MD5
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string EncodindPassword(string password)
        {

            var crypt = MD5.Create();
            return Convert.ToBase64String(crypt.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }



    }






 






   


}
