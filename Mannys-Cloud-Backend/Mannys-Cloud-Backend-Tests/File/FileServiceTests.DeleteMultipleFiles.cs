using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.File
{
    public class FileServiceTests_DeleteMultipleFiles
    {
        [Fact]
        public async Task DeleteMultipleFiles_TaskEndsSuccessfully()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var service = new FileService(context, mockBlobStorage.Object);
            var fileIds = new List<int> { 1, 2};

            // Act
            await service.DeleteMultipleFiles(fileIds);

            // Assert
            var files = context.Files.Where(f => f.FileId == 1 || f.FileId == 2).ToList();
            var markedDeleted = files.All(f => f.IsDeleted);
            Assert.True(markedDeleted);
            mockBlobStorage.Verify(x => x.DeleteFileAsync(It.IsAny<string>()), Times
                .Exactly(fileIds.Count));
        }

        [Fact]
        public async Task DeleteMultipleFiles_ThrowsBlobStorageException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFileAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Deletion failed."));

            var service = new FileService(context, mockBlobStorage.Object);
            var fileIds = new List<int> { 1, 2 };

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.DeleteMultipleFiles(fileIds));
            Assert.Equal("Deletion failed.", exception.Message);

            var files = context.Files;
            var filesAreDeleted = files.All(f => f.IsDeleted);
            Assert.False(filesAreDeleted);
        }
    }
}
