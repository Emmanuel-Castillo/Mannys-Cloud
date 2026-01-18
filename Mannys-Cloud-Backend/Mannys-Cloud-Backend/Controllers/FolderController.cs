using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mannys_Cloud_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;
        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFolders()
        {
            try
            {
                var folderDtos = await _folderService.GetFolders();
                return Ok(new { message = "Folders retrieved.", folders = folderDtos });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetFolder(int id)
        {
            try
            {
                var folderDto = await _folderService.GetFolder(id);
                return Ok(new { success = true, message = "Folder successfully retrieved.", folder = folderDto });
            }
            catch (Exception ex)
            {
                // Log exception to App Insights if needed
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFolder(AddFolderRequest request)
        {
            try {
                await _folderService.AddFolder(request);
                return Ok(new { success = true,  message = "Folder successfully created." });
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSingleFolder(int id)
        {
            try
            {
                await _folderService.DeleteSingleFolder(id);
                return Ok(new { message = "Folder successfully removed. " });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("multiple")]
        [Authorize]
        public async Task<IActionResult> DeleteMultipleFolders([FromBody] List<int> ids)
        {
            try {
                await _folderService.DeleteMultipleFolders(ids);
                return Ok(new { message = "Folders successfully removed." });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
