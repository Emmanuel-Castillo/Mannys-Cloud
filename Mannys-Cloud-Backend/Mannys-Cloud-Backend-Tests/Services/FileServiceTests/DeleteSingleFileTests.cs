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
    public class DeleteSingleFileTests
    {
        [Fact]
        public async Task DeleteSingleFile_SuccessfulDeletion()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFileAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = new FileService(context, mockBlobStorage.Object);
            var fileId = 1;

            // Act
            await service.DeleteSingleFile(fileId);

            // Assert
            var deletedFile = context.Files.Single(f => f.FileId == fileId);
            Assert.True(deletedFile.IsDeleted);

            // Verify if blob deletion happens once
            mockBlobStorage.Verify(
                x => x.DeleteFileAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSingleFile_BlobDeletionFails()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFileAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("File deletion failed!"));

            var service = new FileService(context, mockBlobStorage.Object);
            var fileId = 1;

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.DeleteSingleFile(fileId));
            Assert.Equal("File deletion failed!", exception.Message);

            var targetedFile = context.Files.Single(f => f.FileId == fileId);
            Assert.False(targetedFile.IsDeleted);
        }
    }
}
