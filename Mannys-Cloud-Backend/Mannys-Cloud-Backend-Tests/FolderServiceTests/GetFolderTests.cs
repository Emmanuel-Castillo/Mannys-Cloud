using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.FolderServiceTests
{
    public class GetFolderTests
    {
        [Fact]
        public async Task GetFolderTests_SuccessfullyReturnsFolder()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            var service = new FolderService(context, mockBlobStorage.Object);
            var folderId = 1;   // Id for root folder

            // Act
            var folderDto = await service.GetFolder(folderId);
            Assert.IsType<FolderDto>(folderDto);

            var childFolders = folderDto.ChildFolders;
            Assert.Equal(2, childFolders.Count);    // 3 total, 1 is deleted
            Assert.True(childFolders.All(f => !f.IsDeleted));

            var childFiles = folderDto.Files;
            Assert.Equal(2, childFiles.Count);  // 3 total, 1 is deleted
            Assert.True(childFiles.All(f => !f.IsDeleted));
        }

        [Fact]
        public async Task GetFolderTests_PassingNullFolderException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            var service = new FolderService(context, mockBlobStorage.Object);
            var folderId = 999;

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async() => await service.GetFolder(folderId));
            Assert.Equal("Folder not found!", exception.Message);
        }
    }
}
