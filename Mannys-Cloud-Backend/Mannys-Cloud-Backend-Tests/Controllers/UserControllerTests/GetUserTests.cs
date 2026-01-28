using Mannys_Cloud_Backend.Controllers;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Controllers.UserControllerTests
{
    public class GetUserTests
    {
        [Fact]
        public async Task GetUser_ReturnsOkObj()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userDto = new UserDto { Email = "test@gmail.com", FullName = "test", UserId = 1 };
            var userData = new UserData(userDto, [], []);
            mockService.Setup(x => x.GetUser(It.IsAny<int>())).ReturnsAsync(userData);

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();  // User set w/ Id = 1
            var requestingUserId = 1;

            // Act
            var result = await controller.GetUser(requestingUserId);

            // Assert
            var okObj = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GetUserResponse>(okObj.Value);
        }

        [Fact]
        public async Task GetUser_MismatchedIds_CheckUserThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userDto = new UserDto { Email = "test@gmail.com", FullName = "test", UserId = 1 };
            var userData = new UserData(userDto, [], []);
            //mockService.Setup(x => x.GetUser(It.IsAny<int>())).ReturnsAsync(userData);

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext("2");  // User set w/ Id = 1
            var requestingUserId = 1;

            // Act
            var result = await controller.GetUser(requestingUserId);

            // Assert
            var badResObj = Assert.IsType<BadRequestObjectResult>(result);
            var exMessage = Assert.IsType<string>(badResObj.Value);
            Assert.Equal("Mismatch between auth user id and requested id.", exMessage);
        }

        [Fact]
        public async Task GetUser_AuthServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService.Setup(x => x.GetUser(It.IsAny<int>())).ThrowsAsync(It.IsAny<Exception>());

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext("2");  // User set w/ Id = 1
            var requestingUserId = 1;

            // Act
            var result = await controller.GetUser(requestingUserId);

            // Assert
            var badResObj = Assert.IsType<BadRequestObjectResult>(result);
            var exMessage = Assert.IsType<string>(badResObj.Value);
        }
    }

}
