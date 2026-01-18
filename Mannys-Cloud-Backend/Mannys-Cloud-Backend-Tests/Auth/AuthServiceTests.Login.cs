using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Auth
{
    public class AuthServiceTests_Login
    {
        [Fact]
        public async Task Login_ReturnsUserData()
        {
            // Arrange
            // Email needs to be correct. Pw doesn't matter, because our password service will pass automatically
            // This is meant to test authService user data retrieval
            var request = new LoginUserRequest
            {
                Email = "testuser@gmail.com",
                Password = "doesn't matter"
            };


            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateSeededDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act
            var authUserData = await authService.Login(request);

            // Assert
            Assert.NotNull(authUserData);
            Assert.IsType<AuthenticatedUserData>(authUserData);
            Assert.Equal("test-jwt-token", authUserData.token);
            Assert.IsType<UserDto>(authUserData.userData);
            Assert.Equal("testuser", authUserData.userData.FullName);
            Assert.Equal("testuser@gmail.com", authUserData.userData.Email);
        }

        [Fact]
        public async Task Login_ThrowsNullUserException()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "fakeuser@gmail.com",
                Password = "testpw"
            };


            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateSeededDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act && Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await authService.Login(request));
            Assert.Equal("User is not registered with this email.", exception.Message);

        }

        [Fact]
        public async Task Login_ThrowsInvalidPasswordException()
        {
            // Arrange
            // Email needs to be correct. Pw doesn't matter, because our password service will fail automatically
            // This is intended only to test authService exception throwing
            var request = new LoginUserRequest
            {
                Email = "testuser@gmail.com",
                Password = "doesntmatter"
            };


            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(false); // Sets any password comparison to false!!!!
            var context = TestDbContextFactory.CreateSeededDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act && Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await authService.Login(request));
            Assert.Equal("Invalid password", exception.Message);

        }
    }
}
