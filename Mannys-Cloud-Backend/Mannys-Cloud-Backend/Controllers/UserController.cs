using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
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
                return Ok(new { userDto.UserId, userDto.FullName, userDto.Email, userData.userFiles, userData.userFolders });
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
                return Ok(new { success = true, message = "Root folder retrieved.", folder = rootFolderDto });
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
                return Ok(new { success = true, message = "User trash retrieved.", trashData.trashFolders, trashData.trashFiles });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
