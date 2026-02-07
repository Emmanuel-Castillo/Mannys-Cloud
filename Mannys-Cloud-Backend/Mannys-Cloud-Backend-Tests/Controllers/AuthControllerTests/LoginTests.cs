using Mannys_Cloud_Backend.Controllers;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Controllers.AuthControllerTests
{
    public class LoginTests
    {
        [Fact]
        public async Task LoginUser_ReturnsOkObj()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();

            var controller = new AuthController(mockService.Object);
            var request = new LoginUserRequest
            {
                Email = "test@gmail.com",
                Password = "testpw"
            };
            var authUserData = new AuthenticatedUserData(new UserDto { Email = request.Email, FullName = "test", UserId = 1 }, "testtoken");
            mockService.Setup(x => x.Login(request)).Returns(Task.FromResult<AuthenticatedUserData>(authUserData));

            // Act
            var result = await controller.Login(request);

            // Assert
            var okObj = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okObj.Value);

            Assert.True(response.Success);
            Assert.Equal("User successfully logged in", response.Message);
            Assert.Equal("testtoken", response.Token);

            var userDto = Assert.IsType<UserDto>(response.UserData);
            Assert.Equal(request.Email, userDto.Email);
            Assert.Equal(authUserData.userData.FullName, userDto.FullName);
            Assert.Equal(authUserData.userData.UserId, userDto.UserId);
        }

        [Fact]
        public async Task LoginUser_AuthServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();

            var controller = new AuthController(mockService.Object);
            var request = new LoginUserRequest
            {
                Email = "test@gmail.com",
                Password = "testpw"
            };
            mockService.Setup(x => x.Login(request)).ThrowsAsync(It.IsAny<Exception>());

            // Act
            var result = await controller.Login(request);

            // Assert
            var badRequestObj = Assert.IsType<BadRequestObjectResult>(result);
            var exMessage = Assert.IsType<string>(badRequestObj.Value);
        }
    }
}
