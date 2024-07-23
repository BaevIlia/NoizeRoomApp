using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Contracts;
using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Repositories;

using Xunit;
using Xunit.Sdk;

namespace NoizeRoomApp.Tests
{
    public class UserControllerTest
    {

        [Fact]
        public void GetUserByIdTest()
        {
            var mock = new Mock<IUserRepository>();
       
            string id = "82f6a623-603b-4d19-9f78-2f57c6007848";
            GetProfileRequest request = new(id); 
            UserEntity testUser = new UserEntity()
            {
                Id = Guid.Parse("82f6a623-603b-4d19-9f78-2f57c6007848"),
                Name = "Раниль",
                Email = "ranil@mail.ru"
            };
             mock.Setup(repo => repo.GetById(Guid.Parse(id))).ReturnsAsync(testUser);

            UserController userController = new(mock.Object);

            var result = userController.GetUserById(request);


     
            Assert.Equal(OkObjectResult, result);
        }


    }
}
