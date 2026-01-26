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
    public class RegisterTests
    {
        [Fact]
        public async Task RegisterUser_ReturnsOkResultObj()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();

            var controller = new AuthController(mockService.Object);
            var request = new RegisterUserRequest
            {
                Email = "test@gmail.com",
                FullName = "test",
                Password = "testpw"
            };
            var authUserData = new AuthenticatedUserData(new UserDto { Email = request.Email, FullName = request.FullName, UserId = 1 }, "testtoken");
            mockService.Setup(x => x.Register(request)).Returns(Task.FromResult<AuthenticatedUserData>(authUserData));

            // Act
            var result = await controller.Register(request);

            // Assert
            var okObj = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okObj.Value);

            Assert.True(response.success);
            Assert.Equal("User successfully registered", response.message);
            Assert.Equal("testtoken", response.token);

            var userDto = Assert.IsType<UserDto>(response.userData);
            Assert.Equal(request.Email, userDto.Email);
            Assert.Equal(request.FullName, userDto.FullName);
            Assert.Equal(authUserData.userData.UserId, userDto.UserId);
        }


        [Fact]
        public async Task RegisterUser_AuthServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();

            var controller = new AuthController(mockService.Object);
            var request = new RegisterUserRequest
            {
                Email = "test@gmail.com",
                FullName = "test",
                Password = "testpw"
            };
            mockService.Setup(x => x.Register(request)).ThrowsAsync(It.IsAny<Exception>());

            // Act
            var result = await controller.Register(request);

            // Assert
            var badRequestObj = Assert.IsType<BadRequestObjectResult>(result);
            var exMessage = Assert.IsType<string>(badRequestObj.Value);
        }
    }
}
