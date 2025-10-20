using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Mannys_Cloud_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ConvertDto _convertDto;

        public UserController(ApplicationDbContext context, ConvertDto convertDto)
        {
            _context = context;
            _convertDto = convertDto;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                return Ok(new { user.UserId, user.FullName, user.Email, user.UserFiles, user.UserFolders });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            try
            {

                // Validate request
                if (string.IsNullOrEmpty(request.NewFullName) && string.IsNullOrEmpty(request.NewEmail))
                {
                    return BadRequest("Invalid request.");
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                if (!string.IsNullOrEmpty(request.NewEmail))
                {
                    if (!Regex.IsMatch(request.NewEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        return BadRequest("Invalid email format");
                    else
                        user.Email = request.NewEmail;
                }

                if (!string.IsNullOrEmpty(request.NewFullName))
                    user.FullName = request.NewFullName;

                await _context.SaveChangesAsync();
                return Ok("User successfully updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted.");
        }

        [HttpGet("root")]
        [Authorize]
        public async Task<IActionResult> GetUserRootFolder()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized();

                var rootFolder = await _context.Folders.Include(f => f.FolderFiles.Where(ff => ff.IsDeleted == false)).Include(f => f.ChildFolders.Where(cf => cf.IsDeleted == false)).FirstAsync(f => f.IsRootFolder && f.UserId == int.Parse(userId));
                if (rootFolder == null) return NotFound();

                var rootFolderDto = _convertDto.ConvertToFolderDto(rootFolder);

                return Ok(new { success = true, message = "Root folder retrieved.", folder = rootFolderDto });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
