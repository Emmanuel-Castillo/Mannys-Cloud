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
    public record GetUserResponse
    {
        private readonly int UserId;
        private readonly string FullName;
        private readonly string Email;
        private readonly ICollection<Models.File> userFiles;
        private readonly ICollection<Models.Folder> userFolders;

        public GetUserResponse(int _UserId, string _FullName, string _Email, ICollection<Models.File> _userFiles, ICollection<Models.Folder> _userFolders)
        {
            UserId = _UserId;
            FullName = _FullName;
            Email = _Email;
            userFiles = _userFiles;
            userFolders = _userFolders;
        }
    }
    public record UserRootFolderResponse
    {
        public readonly bool success;
        public readonly string message;
        public readonly FolderDto folder;
        public UserRootFolderResponse(bool _success, string _message, FolderDto _folder)
        {
            success = _success;
            message = _message;
            folder = _folder;
        }
    }

    public record UserTrashResponse
    {
        public readonly bool success;
        public readonly string message;
        public readonly List<FileDto> trashFiles;
        public readonly List<FolderDto> trashFolders;
        public UserTrashResponse(bool _success, string _message, List<FileDto> _trashFiles, List<FolderDto> _trashFolders)
        {
            success= _success;
            message = _message;
            trashFiles = _trashFiles;
            trashFolders = _trashFolders;
        }
    }
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
