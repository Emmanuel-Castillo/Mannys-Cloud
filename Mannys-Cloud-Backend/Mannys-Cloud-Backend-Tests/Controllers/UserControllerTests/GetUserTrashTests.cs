using Mannys_Cloud_Backend.Controllers;
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
    public class GetUserTrashTests
    {
        [Fact]
        public async Task GetUserTrash_ReturnsOkObj()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var content = new UserTrashContent([], []);
            mockService.Setup(x => x.GetUserTrash(It.IsAny<int>())).Returns(content);

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();

            // Act
            var result = controller.GetUserTrash();

            // Assert
            var okResObj = Assert.IsType<OkObjectResult>(result);
            var userTrashRes = Assert.IsType<UserTrashResponse>(okResObj.Value);
        }

        [Fact]
        public async Task GetUserTrash_NoUserContext_CheckUserClassException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var content = new UserTrashContent([], []);
            mockService.Setup(x => x.GetUserTrash(It.IsAny<int>())).Returns(content);

            var controller = new UserController(mockService.Object);

            // Act
            var result = controller.GetUserTrash();

            // Arrange
            var badResObj = Assert.IsType<BadRequestObjectResult>(result);
            var exMessage = Assert.IsType<string>(badResObj.Value);
            Assert.Equal("User is null", exMessage);
        }

        [Fact]
        public async Task GetUserTrash_UserServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var content = new UserTrashContent([], []);
            mockService.Setup(x => x.GetUserTrash(It.IsAny<int>())).Throws(It.IsAny<Exception>());

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();

            // Act
            var result = controller.GetUserTrash();

            // Assert
            var badResObj = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<string>(badResObj.Value);
        }
    }
}
