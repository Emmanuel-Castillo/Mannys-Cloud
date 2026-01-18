using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
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
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetFile(int id)
        {
            try
            {
                var fileDto = await _fileService.GetFile(id);
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
                await _fileService.AddFile(request);
                return Ok(new { success = true, message = "File uploaded successfully" });
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSingleFile(int id)
        {
            try {
                await _fileService.DeleteSingleFile(id);
                return Ok(new { message = "File successfully deleted." });
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("multiple")]
        [Authorize]
        public async Task<IActionResult> DeleteMultipleFiles([FromBody] List<int> ids)
        {
            try {
                await _fileService.DeleteMultipleFiles(ids);
                return Ok(new { message = "Files successfully deleted." });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UndeleteFile(int id) {
            try {
                await _fileService.UndeleteFile(id);
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
                var result = await _fileService.DownloadFile(id);

                // Send it to client as a downloadable file
                return File(result.stream, result.contentType, result.fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
