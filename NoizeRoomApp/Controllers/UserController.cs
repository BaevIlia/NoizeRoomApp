using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Models;
using System.Security.Cryptography;
using System.Text;
using NoizeRoomApp.Contracts.UserContracts;


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


        public UserController(PostgreSQLContext context)
        {
            _context = context;

        }

        /// <summary>
        /// Метод аутентификации пользоваеля, на вход требует строковые почту и пароль
        /// </summary>
        /// <param name="request">Возвращает идентификатор пользователя и токен доступа</param>
        /// <returns></returns>
        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            //Создание хэшированного пароля, сначала входящий пароль расшифровывается в стандартный вид(DecodingPass), затем хэшируется(EncodingPassword)
            string password = EncodindPassword(DecodingPassword(request.password));

            //Поиск в БД пользователя по его почте и паролю
            UserEntity user = _context.Users
                .Where(u => u.Email.Equals(request.email) && u.Password.Equals(password))
                .FirstOrDefault();
             
            //Если не пользователь не найден, возвращается ошибка 401 
            if (user is null)
            {
                return Unauthorized();
            }
            //Генерируется токен доступа
            user.AccessToken = Guid.NewGuid();

            //Сохранить изменения
            _context.SaveChanges();

            
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

           //дешифровка полученного пароля
            string password = DecodingPassword(registerRequest.password);

            //Создание нового пользователя
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
                //Добавление пользователя в БД
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return Ok(new RegistrationResponse(newUser.Id.ToString()));

            }
            //При ошибке сохранения отправляется сообщение об ошибке
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

            //Поиск пользователя по идентификатору
            UserEntity user = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();

            //Если пльзователь не найден, возвращается ошибка 201
            if (user is null)
            {
                return NoContent();
            }
            //По идентификатору находится тип уведомления
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
            //Поиск пользователя по идентификатору
            UserEntity userForSave = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();

            //Если не найден, ошибка 201
            if (userForSave is null)
            {
                return NoContent();
            }
            try
            {
                //Запись в базу данных новой информации
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
            //Поиск пользователя
            UserEntity userForPassChange = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();

            //Если не найден, ошибка 201
            if (userForPassChange is null)
            {
                return NoContent();
            }

            //Расшифровка полученного пароля
            string password =DecodingPassword(request.password);

            //Хэширование пароля и запись в БД
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
            //Конвертирование Base64 в string
            var encodedBytes = Convert.FromBase64String(cryptedPassword);
            string encodedPassword = Encoding.UTF8.GetString(encodedBytes);

            //Сборка нового пароля StringBuilder
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
