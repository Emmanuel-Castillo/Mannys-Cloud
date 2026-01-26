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
    public class UndeleteFileTests
    {
        [Fact]
        public async Task UndeleteFile_SuccessfullyUndeletesFile()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.UndeleteFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var service = new FileService(context, mockBlobStorage.Object);
            var fileId = 3;

            // Act
            await service.UndeleteFile(fileId);

            // Assert
            var file = context.Files.Single(f => f.FileId == 1);
            Assert.False(file.IsDeleted);

            mockBlobStorage.Verify(x => x.UndeleteFileAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UndeleteFile_ThrowsBlobStorageException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.UndeleteFileAsync(It.IsAny<string>())).Throws(new Exception("File undeletion failed."));

            var service = new FileService(context, mockBlobStorage.Object);
            var fileId = 3;

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async() => await service.UndeleteFile(fileId));
            Assert.Equal("File undeletion failed.", exception.Message);

            var file = context.Files.Single(f => f.FileId == fileId);
            Assert.True(file.IsDeleted);
        }
    }
}
