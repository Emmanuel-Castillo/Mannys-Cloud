using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend.Controllers
{
    public record GetUserResponse(int UserId,string FullName,string Email,ICollection<Models.File> userFiles,ICollection<Models.Folder> userFolders);
    
    public record UserRootFolderResponse(bool success,string message, FolderDto folder);

    public record UserTrashResponse(bool success, string message, List<FileDto> trashFiles, List<FolderDto> trashFolders);
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {

            try
            {
                CheckUser.UserIdMatchesRequestedId(User, id);
                var userData = await _userService.GetUser(id);
                var userDto = userData.userDto;
                var response = new GetUserResponse(userDto.UserId, userDto.FullName, userDto.Email, userData.userFiles, userData.userFolders);
                return Ok(response);
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
                CheckUser.UserIdMatchesRequestedId(User, id);
                await _userService.UpdateUser(id, request);
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
            try { 
                CheckUser.UserIdMatchesRequestedId(User, id);
                await _userService.DeleteUser(id);
                return Ok("User deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("root")]
        [Authorize]
        public async Task<IActionResult> GetUserRootFolder()
        {
            try
            {
                var userId = CheckUser.GrabParsedUserId(User);
                var rootFolderDto = await _userService.GetUserRootFolder(userId);
                var response = new UserRootFolderResponse(true, "Root folder retrieved.", rootFolderDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("trash")]
        [Authorize]
        public IActionResult GetUserTrash()
        {
            try
            {
                var userId = CheckUser.GrabParsedUserId(User);
                var trashData = _userService.GetUserTrash(userId);
                var response = new UserTrashResponse(true, "User trash retrieved.", trashData.trashFiles, trashData.trashFolders);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
