using Mannys_Cloud_Backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Common
{
    public static class MockPassworkServiceFactory
    {
        public static Mock<IPasswordService> Create(bool verifyPasswordReturnValue)
        {
            var service = new Mock<IPasswordService>();
            service.Setup(s => s.HashPassword(It.IsAny<string>()))
                .Returns("test-hash-pw");

            SetupVerifyPassword(service, verifyPasswordReturnValue);

            return service;
        }

        public static void SetupVerifyPassword(Mock<IPasswordService> service, bool returnValue)
        {
            service.Setup(s => s.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(returnValue);
        }
    }
}
