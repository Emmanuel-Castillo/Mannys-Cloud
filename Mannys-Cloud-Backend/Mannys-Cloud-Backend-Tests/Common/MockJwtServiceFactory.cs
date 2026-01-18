using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Common
{
    public static class MockJwtServiceFactory
    {
        public static Mock<IJwtService> Create()
        {
            var service = new Mock<IJwtService>();
            service.Setup(s =>
                s.GenerateToken(It.IsAny<User>()))
                .Returns("test-jwt-token");

            return service;
        }
    }
}
