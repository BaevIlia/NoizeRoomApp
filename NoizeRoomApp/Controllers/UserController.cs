using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using System.Security.Cryptography;
using System.Text;
using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Repositories;
using CSharpFunctionalExtensions;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Services;


namespace NoizeRoomApp.Contracts
{
    /// <summary>
    /// Вся логика работы приложения, связаная с пользователем
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(string id) 
        {
            var response = await _userService.GetUserById(Guid.Parse(id));

            return Ok(new GetProfileResponse(response.Id, response.Name, response.Email, response.PhoneNumber, response.NotifyType));
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> Authorization([FromBody] LoginRequest request) 
        {
            var authUserId = await _userService.UserAuthorization(request.email, request.password);


            return Ok(new LoginResponse(authUserId.ToString(), Guid.NewGuid().ToString()));
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest request) 
        {
            var userId = await _userService.CreateNewUser(
                request.name, request.email, request.phone, 
                request.password, request.notifyType, request.role);

            return Ok(new RegistrationResponse(userId.ToString()));
        }

        [HttpPut("changeProfile")]
        public async Task<IActionResult> ChangeProfileInfo([FromBody] UpdateProfileRequest request) 
        {
            var updatedUser = await _userService.UpdateUser(
                Guid.Parse(request.id),
                request.name,
                request.email,
                request.phoneNumber,
                request.notifyType);
            
            return Ok(new UpdateProfileResponce(
                updatedUser.Id.ToString(), 
                updatedUser.Name, 
                updatedUser.Email,
                updatedUser.PhoneNumber,
                updatedUser.NotifyType));
        }

        [HttpPatch("changePassword")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswRequest request) 
        {
           bool result = await _userService.ChangePassword(Guid.Parse(request.id), request.password);

          
            return Ok("Пароль успешно изменён");
        }


    }






 






   


}
