using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Util;
using Microsoft.AspNetCore.Mvc;

namespace Mannys_Cloud_Backend.Services
{ 
    public record DownloadableFile
        {
            public Stream stream;
            public string contentType;
            public string fileName;

            public DownloadableFile(Stream stream, string contentType, string fileName)
            {
                this.stream = stream;
                this.contentType = contentType;
                this.fileName = fileName;
            }
        }
    public class FileService : IFileService
    {
       

        private readonly ApplicationDbContext _context;
        private readonly IBlobStorage _blobStorage;

        public FileService(ApplicationDbContext context, IBlobStorage blobStorage)
        {
            _context = context;
            _blobStorage = blobStorage;
        }

        public async Task AddFile(AddFileRequest request)
        {
            try {
                var file = request.File;
                var blobName = $"{request.UserId}/{request.FolderId}/{Guid.NewGuid()}_{file.FileName}";
                await _blobStorage.UploadFileAsync(file, blobName);

                var newFile = new Models.File
                {
                    UserId = request.UserId,
                    FileName = file.FileName,
                    BlobPath = blobName,
                    ContentType = file.ContentType,
                    FolderId = request.FolderId,
                    SizeBytes = (int)file.Length
                };

                _context.Files.Add(newFile);
                await _context.SaveChangesAsync();
            }
            catch {
                throw;
            }
        }

        public async Task DeleteMultipleFiles(List<int> fileIds)
        {
            try
            {
                var tasks = fileIds.Select(id => this.DeleteSingleFile(id));
                await Task.WhenAll(tasks);
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteSingleFile(int fileId)
        {
            try
            {
                var file = await _context.Files.FindAsync(fileId);
                if (file == null) throw new Exception("File not found!");

                await _blobStorage.DeleteFileAsync(file.BlobPath);

                file.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<DownloadableFile> DownloadFile(int fileId)
        {
            try {
                var file = await _context.Files.FindAsync(fileId);
                if (file == null) throw new Exception("File not found!");

                // Get blob stream from Azure
                var stream = await _blobStorage.DownloadFileAsync(file.BlobPath);


                // Send it to client as a downloadable file
                return new DownloadableFile(stream, file.ContentType, file.FileName);
            }
            catch {
                throw;
            }
        }

        public async Task<FileDto> GetFile(int fileId)
        {
            try {
                var file = await _context.Files.FindAsync(fileId);
                if (file == null) throw new Exception("File not found!");

                var fileDto = ConvertToDto.ConvertToFileDto(file);
                return fileDto;
            }
            catch {
                throw;
            }
        }

        public async Task UndeleteFile(int fileId)
        {
            try
            {
                var file = await _context.Files.FindAsync(fileId);
                if (file == null) throw new Exception("File not found!");

                await _blobStorage.UndeleteFileAsync(file.BlobPath);
                file.IsDeleted = false;

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
