
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Mannys_Cloud_Backend.Services
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly IConfiguration _configuration;

        public BlobStorage(BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _blobServiceClient = blobServiceClient;

            // Blob container
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetValue<string>("BlobContainer"));
        }

        public async Task DeleteFileAsync(string blobName)
        {
            // Get the Blob Client
            var blobStorageClient = _blobContainerClient.GetBlobClient(blobName);

            // Delete the Blob
            await blobStorageClient.DeleteIfExistsAsync();
        }

        public async Task DeleteFolderAsync(string folderPath)
        {
            // Grab list of blobs using prefix: folderPath
            var blobItemsToDelete = _blobContainerClient.GetBlobsAsync(prefix: folderPath);

            await foreach (BlobItem blobItem in blobItemsToDelete)
            {
                BlobClient blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);
                await blobClient.DeleteIfExistsAsync();
            }
        }

        public async Task<Stream> DownloadFileAsync(string blobName)
        {
            var blobStorageClient = _blobContainerClient.GetBlobClient(blobName);

            var response = await blobStorageClient.DownloadStreamingAsync();
            return response.Value.Content;
        }

        public async Task UndeleteFileAsync(string blobName)
        {
            var blobStorageClient = _blobContainerClient.GetBlobClient(blobName);

            await blobStorageClient.UndeleteAsync();
        }

        public async Task UploadFileAsync(IFormFile file, string blobName)
        {
            // Get the Blob Client
            var blobStorageClient = _blobContainerClient.GetBlobClient(blobName);

            // Upload to Azure Storage
            using (var stream = file.OpenReadStream())
            {
                await blobStorageClient.UploadAsync(stream);
            }
        }
    }
}
