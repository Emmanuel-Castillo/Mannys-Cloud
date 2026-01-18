using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.JwtServiceTests
{
    public class JwtServiceTests
    {
        [Fact]
        public void GenerateToken_ReturnsToken()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "test_super_secret_key_that_is_long_enough" },
                {"Jwt:Issuer", "testissuer" },
                {"Jwt:Audience", "testaudience" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

           
            var jwtService = new JwtService(config);

            var testUser = new User { FullName = "testuser", Email = "testuser@gmail.com", PasswordHash = "testHashedPw" };

            // Act
            var result = jwtService.GenerateToken(testUser);

            // Assert
            Assert.IsType<string>(result);
        }
    }
}
