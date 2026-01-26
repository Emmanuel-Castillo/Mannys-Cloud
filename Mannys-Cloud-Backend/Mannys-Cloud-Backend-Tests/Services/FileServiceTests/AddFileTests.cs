using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Services.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Services.FileServiceTests
{
    public class AddFileTests
    {
        [Fact]
        public async Task AddFile_SuccessfulAddition()
        {
            // Arrange
            var context = TestDbContextFactory.CreateEmptyDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.UploadFileAsync(It.IsAny<FormFile>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = new FileService(context, mockBlobStorage.Object);

            var request = new AddFileRequest
            {
                File = FormFileFactory.CreateFormFile(),
                FolderId = 99,
                UserId = 99
            };

            // Act
            await service.AddFile(request);

            // Assert
            var savedFile = await context.Files.SingleAsync();

            Assert.Equal(request.UserId, savedFile.UserId);
            Assert.Equal(request.FolderId, savedFile.FolderId);
            Assert.Equal("test.txt", savedFile.FileName);
            Assert.Equal("text/plain", savedFile.ContentType);
            Assert.False(string.IsNullOrWhiteSpace(savedFile.BlobPath));

            // Checks if file upload was invoked exactly once
            mockBlobStorage.Verify(
                x => x.UploadFileAsync(
                    request.File,
                    It.Is<string>(p => p.Contains("test.txt"))
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task AddFile_BlobUploadFails()
        {
            // Arrange
            var context = TestDbContextFactory.CreateEmptyDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.UploadFileAsync(It.IsAny<FormFile>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Upload failed."));

            var service = new FileService(context, mockBlobStorage.Object);

            var request = new AddFileRequest
            {
                File = FormFileFactory.CreateFormFile(),
                FolderId = 99,
                UserId = 99
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.AddFile(request));
            Assert.Equal("Upload failed.", exception.Message);
        }
    }
}
