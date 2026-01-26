using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend_Tests.Services.Common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Services.FolderServiceTests
{
    public class DeleteSingleFolderTests
    {
        [Fact]
        public async Task DeleteFolder_FolderSuccessfullyDeleted()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFolderAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            var service = new FolderService(context, mockBlobStorage.Object);
            var folderId = 2;

            // Arrange
            await service.DeleteSingleFolder(folderId);

            // Assert
            var folder = context.Folders.Single(f => f.FolderId == folderId);
            var folderFiles = folder.FolderFiles.ToList();

            // Folder files should be deleted
            Assert.True(folder.IsDeleted);
            Assert.True(folderFiles.All(f => f.IsDeleted));

            // Assert folder childFolders and it's files have also been deleted
            var folderChildren = folder.ChildFolders.ToList();
            Assert.True(folderChildren.All(f => f.IsDeleted));
            folderChildren.ForEach(f =>
            {
                var childFiles = f.ChildFolders.ToList();
                Assert.True(childFiles.All(f => f.IsDeleted));
            });
        }

        [Fact]
        public async Task DeleteFolder_PassingNonexistantFolderIdThrowsException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFolderAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            var service = new FolderService(context, mockBlobStorage.Object);
            var folderId = 999;   // Folder w/ id = 999 doesn't exist in seeded db

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.DeleteSingleFolder(folderId));
            Assert.Equal("Folder not found!", exception.Message);
        }

        [Fact]
        public async Task DeleteFolder_PassingRootFolderIdThrowsException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFolderAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            var service = new FolderService(context, mockBlobStorage.Object);
            var folderId = 1;   // Root folder id from seeded db

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async() => await service.DeleteSingleFolder(folderId));
            Assert.Equal("Deleting root folders is prohibited.", exception.Message);
        }
    }
}
