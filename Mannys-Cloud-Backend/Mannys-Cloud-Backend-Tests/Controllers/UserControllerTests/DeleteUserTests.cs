using Mannys_Cloud_Backend.Controllers;
using Mannys_Cloud_Backend.Interfaces;
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
    public class DeleteUserTests
    {
        [Fact]
        public async Task DeleteUser_ReturnsOkObj()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService.Setup(x => x.DeleteUser(It.IsAny<int>())).Returns(Task.CompletedTask);

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();  // User Id = 1
            var requestingUserId = 1;

            // Act
            var result = await controller.DeleteUser(requestingUserId);

            // Assert
            var okObj = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted.", okObj.Value);
        }

        [Fact]
        public async Task DeleteUser_MismatchingUserIds_CheckUserClassThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService.Setup(x => x.DeleteUser(It.IsAny<int>())).Returns(Task.CompletedTask);

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();  // User Id = 1
            var requestingUserId = 2;

            // Act
            var result = await controller.DeleteUser(requestingUserId);

            // Assert
            var badReqObj = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Mismatch between auth user id and requested id.", badReqObj.Value);
        }

        [Fact]
        public async Task DeleteUser_UserServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService.Setup(x => x.DeleteUser(It.IsAny<int>())).ThrowsAsync(It.IsAny<Exception>());

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();  // User Id = 1
            var requestingUserId = 1;

            // Act
            var result = await controller.DeleteUser(requestingUserId);

            // Assert
            var badReqRes = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<string>(badReqRes.Value);
        }
    }
}
