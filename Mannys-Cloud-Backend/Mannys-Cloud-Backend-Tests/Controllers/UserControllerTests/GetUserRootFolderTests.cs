using Mannys_Cloud_Backend.Controllers;
using Mannys_Cloud_Backend.DTO;
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
    public class GetUserRootFolderTests
    {
        [Fact]
        public async Task GetUserRootFolder_ReturnsOkObj()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService.Setup(x => x.GetUserRootFolder(It.IsAny<int>())).ReturnsAsync(It.IsAny<FolderDto>());
            
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();

            // Act
            var result = await controller.GetUserRootFolder();

            // Assert
            var okObjRes = Assert.IsType<OkObjectResult>(result);
            var rootFolderRes = Assert.IsType<UserRootFolderResponse>(okObjRes.Value);
        }

        [Fact]
        public async Task GetUserRootFolder_NoUserContext_CheckUserClassThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var controller = new UserController(mockService.Object);

            // Act
            var result = await controller.GetUserRootFolder();

            // Assert
            var badReqObj = Assert.IsType<BadRequestObjectResult>(result);
            var exMessage = Assert.IsType<string>(badReqObj.Value);
            Assert.Equal("User is null", exMessage);

        }

        [Fact]
        public async Task GetUserRootFolder_UserServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService.Setup(x => x.GetUserRootFolder(It.IsAny<int>())).ThrowsAsync(It.IsAny<Exception>());

            var controller = new UserController(mockService.Object);
            controller.ControllerContext = HTTPUserContext.SetTestControllerContext();

            // Act
            var result = await controller.GetUserRootFolder();

            // Assert
            var badReqObj = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<string>(badReqObj.Value);
        }
    }
}
