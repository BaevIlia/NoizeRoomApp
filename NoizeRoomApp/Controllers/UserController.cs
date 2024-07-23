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


    }






 






   


}
