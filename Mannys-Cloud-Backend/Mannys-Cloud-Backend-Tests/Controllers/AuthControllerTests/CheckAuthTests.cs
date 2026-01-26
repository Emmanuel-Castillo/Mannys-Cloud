using Mannys_Cloud_Backend.Controllers;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Controllers.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Controllers.AuthControllerTests
{
    public class CheckAuthTests
    {
        [Fact]
        public async Task CheckAuth_ReturnsOkObj()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();

            var controller = new AuthController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();

            var userDto = new UserDto{ Email = "test@gmail.com", FullName = "test", UserId = 1 };
            mockService.Setup(x => x.CheckAuth(It.IsAny<int>())).Returns(Task.FromResult<UserDto>(userDto));

            // Act
            var result = await controller.CheckAuth();

            // Assert
            var okObj = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okObj.Value);

            Assert.True(response.success);
            Assert.Equal("User retrieved.", response.message);
            Assert.Null(response.token);

            var reponseUser = Assert.IsType<UserDto>(response.userData);
            Assert.Equal(userDto.Email, reponseUser.Email);
            Assert.Equal(userDto.FullName, reponseUser.FullName);
            Assert.Equal(userDto.UserId, reponseUser.UserId);
        }

        [Fact]
        public async Task CheckAuth_MissingUserId_CheckUserClassThrowsException()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();

            var controller = new AuthController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetContextWithEmptyClaims();

            var userDto = new UserDto { Email = "test@gmail.com", FullName = "test", UserId = 1 };
            mockService.Setup(x => x.CheckAuth(It.IsAny<int>())).Returns(Task.FromResult<UserDto>(userDto));

            // Act
            var result = await controller.CheckAuth();

            // Assert
            var badReqObj = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Authorized user id required.", badReqObj.Value);
        }

        [Fact]
        public async Task CheckAuth_NonNumericUserId_CheckUserClassThrowsException()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();

            var controller = new AuthController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetContextWithInvalidClaims();

            var userDto = new UserDto { Email = "test@gmail.com", FullName = "test", UserId = 1 };
            mockService.Setup(x => x.CheckAuth(It.IsAny<int>())).Returns(Task.FromResult<UserDto>(userDto));

            // Act
            var result = await controller.CheckAuth();

            // Assert
            var badReqObj = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User id cannot be parsed to an int.", badReqObj.Value);
        }

        [Fact]
        public async Task CheckAuth_AuthServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();
            mockService.Setup(x => x.CheckAuth(It.IsAny<int>())).ThrowsAsync(It.IsAny<Exception>());

            var controller = new AuthController(mockService.Object);
            controller.ControllerContext= HTTPUserContext.SetTestControllerContext();

            // Act
            var result = await controller.CheckAuth();
            var badReqObj = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<string>(badReqObj.Value);
        }
    }
}
