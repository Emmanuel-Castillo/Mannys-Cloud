namespace Mannys_Cloud_Backend.Services
{
    public interface IBlobStorage
    {
        public Task UploadFileAsync(IFormFile file, string blobName);

        public Task DeleteFileAsync(string blobName);

        public Task UndeleteFileAsync(string blobName);

        public Task<Stream> DownloadFileAsync(string blobName);

        public Task DeleteFolderAsync(string folderPath);
    }
}
