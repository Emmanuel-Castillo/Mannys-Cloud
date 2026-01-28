using Mannys_Cloud_Backend.Controllers;
using Mannys_Cloud_Backend.DTO.Requests;
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
    public class UpdateUserTests
    {
        [Fact]
        public async Task UpdateUser_ReturnsOkObj()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService.Setup(x => x.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserRequest>())).Returns(Task.CompletedTask);
            
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();
            
            var requestingUserId = 1;
            var updateUserRequest = new UpdateUserRequest { NewEmail = "newEmail@gmail.com", NewFullName = "newName" };

            // Act
            var result = await controller.UpdateUser(requestingUserId, updateUserRequest);
            var okObjRes = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User successfully updated.", okObjRes.Value);
        }

        [Fact]
        public async Task UpdateUser_MismatchingIds_CheckUserClassThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext("2");

            var requestingUserId = 1;
            var updateUserRequest = new UpdateUserRequest { NewEmail = "newEmail@gmail.com", NewFullName = "newName" };

            // Act
            var result = await controller.UpdateUser(requestingUserId, updateUserRequest);
            var badResObj = Assert.IsType<BadRequestObjectResult>(result);
            var exMessage = Assert.IsType<string>(badResObj.Value);
            Assert.Equal("Mismatch between auth user id and requested id.", exMessage);
        }

        [Fact]
        public async Task UpdateUser_UserServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService.Setup(x => x.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserRequest>())).ThrowsAsync(It.IsAny<Exception>());

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();

            var requestingUserId = 1;
            var updateUserRequest = new UpdateUserRequest { NewEmail = "newEmail@gmail.com", NewFullName = "newName" };

            // Act
            var result = await controller.UpdateUser(requestingUserId, updateUserRequest);
            var badResObj = Assert.IsType<BadRequestObjectResult>(result);
            var exMessage = Assert.IsType<string>(badResObj.Value);
        }
    }
}
