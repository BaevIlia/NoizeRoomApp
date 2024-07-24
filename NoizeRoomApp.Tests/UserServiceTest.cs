using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Contracts;
using NoizeRoomApp.Contracts.UserContracts;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;
using NoizeRoomApp.Repositories;
using NoizeRoomApp.Services;
using Xunit;
using Xunit.Sdk;

namespace NoizeRoomApp.Tests
{
    public class UserServiceTest
    {

        [Fact]
        public void GetUserByIdTest()
        {
            //Arrange
        
            var mock = new Mock<IUserRepository>();
            Guid mockId = Guid.Parse("c95d83e7-07fe-469f-8e34-0118df13fc58");
            UserEntity testUser = new()
            {
                Id = Guid.Parse("c95d83e7-07fe-469f-8e34-0118df13fc58"),
                Name = "Test",
                Email="test@mail.ru",
                PhoneNumber="88005553535",
                NotifyTypeId =1,
                RoleId =2

            };
            mock.Setup(repo => repo.Get(mockId)).ReturnsAsync(testUser);
            UserService userService = new(mock.Object);

            //Act
            var result = userService.GetUserById(mockId).Result;

            //Assert
            Assert.Equal(result.Name, testUser.Name);
        }
        [Fact]
        public void CreateNewUser() 
        {
            var mock = new Mock<IUserRepository>();

            UserEntity userForCreate = new()
            {
                Name = "Test",
                Email= "test@mail.ru",
                Password = "MWEyYjNj",
                PhoneNumber = "88005553535",
                NotifyTypeId =1,
                RoleId =2
            };
            UserService userService = new(mock.Object);

            var result = userService.CreateNewUser("Test", "test@mail.ru", "88005553535", "MWEyYjNj", "noNotify", "user");

            Assert.NotNull(result);
            Assert.IsType<Guid>(result.Result);
       
        }

        [Fact]
        public void AuthorizeTest() 
        {
            var mock = new Mock<IUserRepository>();
            string testUserEmail = "test@mail.ru";
            string testUserPassword = "MWEyYjNj";
            
            UserService userService = new(mock.Object);

            var result = userService.UserAuthorization(testUserEmail, testUserPassword);

            Assert.NotNull(result);
            Assert.IsType<Guid>(result.Result);
            

        }
        [Fact]
        public void UpdateUser() 
        {
            var mock = new Mock<IUserRepository>();

            UserDto testUser = new()
            {
                Id = Guid.Parse("c95d83e7-07fe-469f-8e34-0118df13fc58"),
                Name = "ФанкиМанки",
                Email = "test@mail.ru",
                PhoneNumber = "1234567890",
                NotifyType = "noNotify"


            };
            UserService userService = new(mock.Object);

            var result = userService.UpdateUser(testUser.Id, testUser.Name, testUser.Email, testUser.PhoneNumber, testUser.NotifyType);

            Assert.NotNull(result);
            Assert.IsType<Guid>(result.Result);
        }



    }
}
