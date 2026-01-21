using Azure.Core;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.AuthServiceTests
{
    public class CheckAuthTests
    {
        [Fact]
        public async Task CheckAuth_ReturnsUserDto()
        {
            // Arrange
            var userId = 1; // Seeded db has user of id = 1

            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateSeededDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act
            var userDto = await authService.CheckAuth(userId);

            // Assert
            Assert.NotNull(userDto);
            Assert.IsType<UserDto>(userDto);
            Assert.Equal("testuser", userDto.FullName);
            Assert.Equal("testuser@gmail.com", userDto.Email);
        }

        [Fact]
        public async Task CheckAuth_ThrowsNullUserException()
        {
            // Arrange
            var userId = 100; // Seeded has not user with id = 100

            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateSeededDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act && Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await authService.CheckAuth(userId));
            Assert.Equal("User not found!", exception.Message);
        }
    }
}
