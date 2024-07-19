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


        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
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
       
            string password = EncodingPassword(DecodingPassword(request.password));

            var user = await _userRepository.AuthorizeUser(request.email, password);

            if (user is null)
                return NotFound("Такого пользователя не существует");

            user.AccessToken = Guid.NewGuid();
            
            return Ok(new LoginResponse(user.Id.ToString(), user.AccessToken.ToString()));
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

            Guid newUserId =  await _userRepository.Registration(registerRequest, EncodingPassword(password));

            if (string.IsNullOrEmpty(newUserId.ToString())) 
            
                return BadRequest();

            return Ok(newUserId.ToString());
        }
        /// <summary>
        /// Запрос о получении пользователя, на вход принимает идентификатор пользователя
        /// </summary>
        /// <param name="request">Возвращает идентификатор, почту, пароль, номер телефона, тип уведомления и идентификатор роли</param>
        /// <returns></returns>
        [HttpGet("getProfile")]
        public async Task<IActionResult> GetUserById([FromBody] GetProfileRequest request)
        {

            var user =  await _userRepository.GetById(Guid.Parse(request.id));

            var notifyType = await _userRepository.GetNotifyType(user.NotifyTypeId);


            if (user is null)
            {
                return NoContent();
            }
            if (string.IsNullOrEmpty(notifyType)) 
            {
                return BadRequest();
            }

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

           
            try
            {

                var user = await _userRepository.UpdateUserProfile(Guid.Parse(request.id), request.name, request.email, request.phoneNumber, request.notifyType);

                return Ok(new UpdateProfileResponce(user.Id.ToString(), user.Name, user.Email, user.PhoneNumber, request.notifyType));
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
            string password =DecodingPassword(request.password);

            if (_userRepository.ChangeUserPassword(Guid.Parse(request.id), EncodingPassword(password)).Result ==0)
            {
                return NoContent();
            }

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
        public string EncodingPassword(string password)
        {

            var crypt = MD5.Create();
            return Convert.ToBase64String(crypt.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }



    }






 






   


}
