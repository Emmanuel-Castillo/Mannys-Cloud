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
    public class AuthServiceTests_Register
    {
        [Fact]
        public async Task Register_ReturnUserData()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                FullName = "test",
                Email = "test@gmail.com",
                Password = "testpw"
            };

            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateEmptyDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act
            var authUserData = await authService.Register(request);

            // Assert
            Assert.NotNull(authUserData);
            Assert.IsType<AuthenticatedUserData>(authUserData);
            Assert.Equal("test-jwt-token", authUserData.token);
            Assert.IsType<UserDto>(authUserData.userData);
            Assert.Equal("test", authUserData.userData.FullName);
            Assert.Equal("test@gmail.com", authUserData.userData.Email);
        }

        [Fact]
        public async Task Register_FullNameRequired() {
            // Arrange
            var request = new RegisterUserRequest
            {
                FullName = "",
                Email = "test@gmail.com",
                Password = "testpw"
            };

            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateEmptyDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await authService.Register(request));
            Assert.Equal("Full name is required.", exception.Message);
        }

        [Fact]
        public async Task Register_InvalidEmail() {
            // Arrange
            var request = new RegisterUserRequest
            {
                FullName = "test",
                Email = "gmail.com",
                Password = "testpw"
            };

            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateEmptyDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await authService.Register(request));
            Assert.Equal("Invalid email format", exception.Message);
        }

        [Fact]
        public async Task Register_PasswordRequired() {
            // Arrange
            var request = new RegisterUserRequest
            {
                FullName = "test",
                Email = "test@gmail.com",
                Password = ""
            };

            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateEmptyDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await authService.Register(request));
            Assert.Equal("Password is required.", exception.Message);
        }

        [Fact]
        public async Task Register_EmailAlreadyRegistered() {
            // Arrange
            var request1 = new RegisterUserRequest
            {
                FullName = "test",
                Email = "test@gmail.com",
                Password = "testpw"
            };

            var request2 = new RegisterUserRequest
            {
                FullName = "test2",
                Email = "test@gmail.com",
                Password = "test2pw"
            };

            var jwtService = MockJwtServiceFactory.Create();
            var passwordService = MockPassworkServiceFactory.Create(true);
            var context = TestDbContextFactory.CreateEmptyDb();
            var authService = new AuthService(context, jwtService.Object, passwordService.Object);

            await authService.Register(request1);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await authService.Register(request2));
            Assert.Equal("Email already registered.", exception.Message);
        }


    }
}
