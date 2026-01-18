using Mannys_Cloud_Backend.DTO.Requests;
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
    public class AddFolderTests
    {
        [Fact]
        public async Task AddFolder_SuccessfullyAddedFolder()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            var service = new FolderService(context, mockBlobStorage.Object);
            
            var request = new AddFolderRequest
            {
                FolderName = "testFolder",
                UserId = 1,
                ParentFolderId = 1,
            };

            // Act
            var folderId = await service.AddFolder(request);

            // Assert
            var folder = context.Folders.Single(f => f.FolderId == folderId);
            Assert.NotNull(folder);
            Assert.Equal("testFolder", folder.FolderName);
            Assert.Equal(1, folder.UserId);
            Assert.Equal(1, folder.ParentFolderId);
        }
    
    [Fact]
    public async Task AddFolder_ThrowUserNotValidException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            var service = new FolderService(context, mockBlobStorage.Object);

            // User w/ UserId of 999 is nonexistant in test db
            var request = new AddFolderRequest
            {
                FolderName = "testFolder",
                UserId = 999,
                ParentFolderId = 1,
            };

            // Act && assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.AddFolder(request));
            Assert.Equal("User or Parent Folder not valid.", exception.Message);
        }

    [Fact]
    public async Task AddFolder_ThrowParentFolderNotFoundPException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            var service = new FolderService(context, mockBlobStorage.Object);

            // Folder w/ folderId of 999 is nonexistant in test db
            var request = new AddFolderRequest
            {
                FolderName = "testFolder",
                UserId = 1,
                ParentFolderId = 999,
            };

            // Act && assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.AddFolder(request));
            Assert.Equal("User or Parent Folder not valid.", exception.Message);
        }
    }



}
