using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;

namespace Mannys_Cloud_Backend.Interfaces
{
    public interface IFolderService
    {
        public Task<FolderDto> GetFolder(int folderId);

        public Task<List<FolderDto>> GetFolders();

        public Task<int> AddFolder(AddFolderRequest request);

        public Task DeleteMultipleFolders(List<int> folderIds);

        public Task DeleteSingleFolder(int folderId);
    }
}
