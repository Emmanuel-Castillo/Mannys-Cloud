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
    public class DownloadFileTests
    {
        [Fact]
        public async Task DownloadFile_ReturnsDownloadableFile()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DownloadFileAsync(It.IsAny<string>())).Returns(Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes("hello"))));

            var service = new FileService(context, mockBlobStorage.Object);
            var fileId = 1;

            // Act
            var file = await service.DownloadFile(fileId);

            // Assert
            Assert.NotNull(file);
            Assert.IsType<DownloadableFile>(file);

            using (StreamReader reader = new StreamReader(file.stream))
            {
                string text = reader.ReadToEnd();
                Console.WriteLine(text);
                Assert.Equal("hello", text);
            }
            Assert.Equal("text/plain", file.contentType);
            Assert.Equal("test.txt", file.fileName);
        }

        [Fact]
        public async Task DownloadFile_ThrowsBlobStorageException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DownloadFileAsync(It.IsAny<string>())).Throws(new Exception("File download failed."));

            var service = new FileService(context, mockBlobStorage.Object);
            var fileId = 1;

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async() => await service.DownloadFile(fileId));
            Assert.Equal("File download failed.", exception.Message);
        }
    }
}
