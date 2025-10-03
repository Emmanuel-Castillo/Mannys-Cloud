using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mannys_Cloud_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly BuildPath _buildPath;
        private readonly ConvertDto _convertDto;

        public FolderController(ApplicationDbContext context, IBlobStorage blobStorage,  BuildPath buildPath, ConvertDto convertDto)
        {
            _context = context;
            _blobStorage = blobStorage;
            _buildPath = buildPath;
            _convertDto = convertDto;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFolders()
        {
            try
            {
                var folders = await _context.Folders.Include(f => f.FolderFiles).ToListAsync();
                var folderDtos = folders.Select(f => _convertDto.ConvertToFolderDto(f)).ToList();

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
                var folder = await _context.Folders.Include(f => f.FolderFiles).Include(f => f.ChildFolders).FirstAsync(f => f.FolderId == id);
                if (folder == null) return NotFound();

                var folderDto = _convertDto.ConvertToFolderDto(folder);
                return Ok(new { success = true, message = "Folder successfully retrieved.", folder = folderDto });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFolder(AddFolderRequest request)
        {
            try {

                // Validate request UserId and ParentFolderId existence
                var user = await _context.Users.FindAsync(request.UserId);
                var parentFolder = await _context.Folders.FindAsync(request.ParentFolderId);

                if (user == null || parentFolder == null) return BadRequest("User or Parent Folder not valid.");

                var newFolder = new Folder
                {
                    UserId = request.UserId,
                    FolderName = request.FolderName,
                    ParentFolderId = request.ParentFolderId,
                };

                _context.Folders.Add(newFolder);
                await _context.SaveChangesAsync();

                return Ok(new { success = true,  message = "Folder successfully created." });
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            try
            {
                var folder = await _context.Folders.FindAsync(id);
                if (folder == null) return NotFound();

                // Check if folder is root folder for User
                if (folder.IsRootFolder) return Forbid("Deleting root folders is prohibited.");

                // Grab folder path in Blob Storage namespace
                // Then soft delete the folder in Blob Storage
                var folderPath = _buildPath.BuildFolderPath(folder);
                await _blobStorage.DeleteFolderAsync(folderPath);

                // Set folder and children IsDeleted to true
                await _context.Files.Where(f => f.FolderId == folder.FolderId).
                    ExecuteUpdateAsync(setters => setters.SetProperty(file => file.IsDeleted, true));
                folder.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Folder successfully removed. " });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
