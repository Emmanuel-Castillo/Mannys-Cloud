using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Services;

namespace Mannys_Cloud_Backend.Interfaces
{
    public interface IFileService
    {
        public Task<FileDto> GetFile(int fileId);

        public Task AddFile(AddFileRequest request);

        public Task DeleteSingleFile(int fileId);

        public Task DeleteMultipleFiles(List<int> fileIds);

        public Task UndeleteFile(int fileId);

        public Task<DownloadableFile> DownloadFile(int fileId);
    }
}
