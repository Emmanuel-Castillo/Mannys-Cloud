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

namespace Mannys_Cloud_Backend_Tests.Services.FolderServiceTests
{
    public class GetFoldersTests
    {
        [Fact]
        public async Task GetFolders_ReturnsListFolderDto()
        {
            // Arrange
            var context = TestDbContextFactory.CreateSeededDb();
            var mockBlobStorage = new Mock<IBlobStorage>();
            var service = new FolderService(context, mockBlobStorage.Object);

            // Act
            var folderDto = await service.GetFolders();

            // Assert
            Assert.IsType<List<FolderDto>>(folderDto);
        }
    }
}
