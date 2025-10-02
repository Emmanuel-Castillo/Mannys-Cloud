using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mannys_Cloud_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly ConvertDto _convertDto;

        public FileController(ApplicationDbContext context, IBlobStorage bloblStorage, ConvertDto convertDto)
        {
            _context = context;
            _blobStorage = bloblStorage;
            _convertDto = convertDto;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetFile(int id)
        {
            try
            {
                var file = await _context.Files.FindAsync(id);
                if (file == null) return NotFound();

                var fileDto = _convertDto.ConvertToFileDto(file);

                return Ok(new {message = "File retrieved.", file = fileDto });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFile(AddFileRequest request)
        {
            try
            {
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

                return Ok(new { message = "File uploaded successfully", fileId = newFile.FileId });
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFile(int id)
        {
            try {
                var file = await _context.Files.FindAsync(id);
                if (file == null) return NotFound();

                await _blobStorage.DeleteFileAsync(file.BlobPath);

                file.IsDeleted = true;
                await _context.SaveChangesAsync();
                return Ok(new { message = "File successfully deleted." });
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UndeleteFile(int id) {
            try {
                var file = await _context.Files.FindAsync(id);
                if (file == null) return NotFound();

                await _blobStorage.UndeleteFileAsync(file.BlobPath);
                file.IsDeleted = false;

                await _context.SaveChangesAsync();
                return Ok(new { message = "File successfully restored." });
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/download")]
        [Authorize]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                var file = await _context.Files.FindAsync(id);
                if (file == null) return NotFound();

                // Get blob stream from Azure
                var stream = await _blobStorage.DownloadFileAsync(file.BlobPath);

                // Send it to client as a downloadable file
                return File(stream, file.ContentType, file.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
