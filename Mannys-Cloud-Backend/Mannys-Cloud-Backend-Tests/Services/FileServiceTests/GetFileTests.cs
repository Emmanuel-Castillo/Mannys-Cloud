using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Services.Common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Services.FileServiceTests
{
    public class GetFileTests
    {
        [Fact]
        public async Task GetFile_ReturnsFileDto()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();

            var service = new FileService(context, mockBlobStorage.Object);
            int fileId = 1;

            // Act
            var returnedFile = await service.GetFile(fileId);
            Assert.NotNull(returnedFile);
            Assert.IsType<FileDto>(returnedFile);
            Assert.Equal(fileId, returnedFile.FileId);
            Assert.Equal(1, returnedFile.UserId);
            Assert.Equal("test.txt", returnedFile.FileName);
        }

        [Fact]
        public async Task GetFile_ThrowNotFoundException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();

            var service = new FileService(context, mockBlobStorage.Object);
            int fileId = 999;

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.GetFile(fileId));
            Assert.Equal("File not found!", exception.Message);
        }
    }
}
