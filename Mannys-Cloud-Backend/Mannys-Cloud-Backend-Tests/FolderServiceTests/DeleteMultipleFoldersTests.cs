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
    public class DeleteMultipleFoldersTests
    {
        [Fact]
        public async Task DeleteMultipleFolders_FoldersSuccessfullyDeleted()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFolderAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            var service = new FolderService(context, mockBlobStorage.Object);
            var folderIds = new List<int>{2, 4};

            // Arrange
            await service.DeleteMultipleFolders(folderIds);

            // Assert
            var folders = context.Folders.Where(f => folderIds.Contains(f.FolderId)).ToList();
            Assert.True(folders.All(f => f.IsDeleted));

            // Assert folder childFolders and it's files have also been deleted
            folders.ForEach(f =>
            {
                var childFiles = f.ChildFolders.ToList();
                Assert.True(childFiles.All(f => f.IsDeleted));
            });
        }

        [Fact]
        public async Task DeleteMultipleFolders_PassingNonexistantFolderIdThrowsException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFolderAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            var service = new FolderService(context, mockBlobStorage.Object);
            var folderIds = new List<int> { 999, 2};   // Folder w/ id = 999 is not in seeded db

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.DeleteMultipleFolders(folderIds));
            Assert.Equal("Folder not found!", exception.Message);

            var requestedFolders = context.Folders.Where(f => folderIds.Contains(f.FolderId));
            Assert.True(requestedFolders.All(f => !f.IsDeleted));
        }

        [Fact]
        public async Task DeleteFolder_PassingRootFolderIdThrowsException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            mockBlobStorage.Setup(x => x.DeleteFolderAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            var service = new FolderService(context, mockBlobStorage.Object);
            var folderIds = new List<int> { 1, 2};   // Root folder id from seeded db

            // Act & assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await service.DeleteMultipleFolders(folderIds));
            Assert.Equal("Deleting root folders is prohibited.", exception.Message);

            var requestedFolders = context.Folders.Where(f => folderIds.Contains(f.FolderId));
            Assert.True(requestedFolders.All(f => !f.IsDeleted));
        }
    }
}
